using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FCP.Util;
using ExprTranslator.Query;
using FluentData;

namespace FCP.Data
{
    /// <summary>
    /// 表达式查询translator工厂
    /// </summary>
    public class ExprQueryTranslatorFactory
    {
        private static readonly IDictionary<string, Type> _exprQueryTranslatorTypes;  //表达式查询translator类型缓存        
        private static readonly IDictionary<string, string> _providerTranslatorMap;  //数据库provider与表达式查询translator名称映射

        static ExprQueryTranslatorFactory()
		{
            _providerTranslatorMap = new Dictionary<string, string>();
            setProviderTranslatorMap();  //配置 数据库provider与表达式查询translator名称映射

            Assembly assembly = typeof(IExprQueryTranslator).Assembly;
            List<Type> list = assembly.GetExportedTypes().Where(type => type.IsConcrete() && type.Is<IExprQueryTranslator>()).ToList<Type>();
            SortedDictionary<string, Type> sortedDictionary = new SortedDictionary<string, Type>();
			foreach (Type currentType in list)
			{                
                string keyName = currentType.Name.Replace("QueryTranslator", string.Empty);
                sortedDictionary.Add(keyName, currentType);
			}
            _exprQueryTranslatorTypes = sortedDictionary;
		}

        private static void setProviderTranslatorMap()
        {
            _providerTranslatorMap.Add("System.Data.SqlClient", "TSql");
            _providerTranslatorMap.Add("System.Data.SqlServerCe.4.0", "SqlCe");
            _providerTranslatorMap.Add("System.Data.SQLite", "SQLite");
            _providerTranslatorMap.Add("Oracle.DataAccess.Client", "Oracle");
            _providerTranslatorMap.Add("MySql.Data.MySqlClient", "MySql");
            _providerTranslatorMap.Add("System.Data.OleDb", "Access");
        }

        /// <summary>
        /// 获取表达式查询translator
        /// </summary>
        /// <param name="translatorName"></param>
        /// <returns></returns>
        public IExprQueryTranslator getExprQueryTranslator(string translatorName)
        {
            var matchQueryTranslatorType = _exprQueryTranslatorTypes
                .Where(m => StringUtil.compareIgnoreCase(m.Key, translatorName) || m.Key.isNullOrEmpty())
                .OrderByDescending(m => m.Key).Select(m => m.Value).FirstOrDefault();

            return (IExprQueryTranslator)Activator.CreateInstance(matchQueryTranslatorType);
        }

        /// <summary>
        /// 通过数据库provider获取表达式查询translator
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        public IExprQueryTranslator getExprQueryTranslatorByDbProvider(IDbProvider dbProvider)
        {
            string providerName = dbProvider.ProviderName;
            var matchTranslatorName = _providerTranslatorMap.Where(m => StringUtil.compareIgnoreCase(m.Key, providerName))
                .Select(m => m.Value).FirstOrDefault();

            matchTranslatorName = matchTranslatorName ?? string.Empty;

            return getExprQueryTranslator(matchTranslatorName);
        }

        /// <summary>
        /// 列出支持的表达式查询translator
        /// </summary>
        /// <returns></returns>
        public string listAvailableExprQueryTranslatorTypes()
        {
            return string.Join(", ", _exprQueryTranslatorTypes.Keys.ToArray());
        }
    }
}
