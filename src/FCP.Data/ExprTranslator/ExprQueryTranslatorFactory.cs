using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FCP.Util;
using ExprTranslator.Query;
using FluentData;
using System.Linq.Expressions;

namespace FCP.Data
{
    /// <summary>
    /// 表达式查询translator工厂
    /// </summary>
    public class ExprQueryTranslatorFactory
    {
        private static readonly IDictionary<string, Func<IExprQueryTranslator>> _exprQueryTranslatorFactories;  //表达式查询translator类型工厂缓存        
        private static readonly IDictionary<string, string> _providerTranslatorMap;  //数据库provider与表达式查询translator名称映射

        static ExprQueryTranslatorFactory()
		{
            _providerTranslatorMap = new Dictionary<string, string>();
            setProviderTranslatorMap();  //配置 数据库provider与表达式查询translator名称映射

            Assembly assembly = typeof(IExprQueryTranslator).Assembly;
            List<Type> list = assembly.GetExportedTypes().Where(type => type.IsConcrete() && type.Is<IExprQueryTranslator>()).ToList<Type>();
            var sortedDictionary = new SortedDictionary<string, Func<IExprQueryTranslator>>();
			foreach (Type currentType in list)
			{                
                string keyName = currentType.Name.Replace("QueryTranslator", string.Empty);
                var instanceFactory = buildExprQueryTranslatorInstanceFactory(currentType);

                sortedDictionary.Add(keyName, instanceFactory);
			}
            _exprQueryTranslatorFactories = sortedDictionary;
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

        private static Func<IExprQueryTranslator> buildExprQueryTranslatorInstanceFactory(Type exprQueryTranslatorType)
        {
            var newQueryTranslatorExpr = Expression.New(exprQueryTranslatorType);
            var conversionExpr = Expression.Convert(newQueryTranslatorExpr, typeof(IExprQueryTranslator));

            return Expression.Lambda<Func<IExprQueryTranslator>>(conversionExpr).Compile();
        }

        /// <summary>
        /// 获取表达式查询translator
        /// </summary>
        /// <param name="translatorName"></param>
        /// <returns></returns>
        public IExprQueryTranslator getExprQueryTranslator(string translatorName)
        {
            var matchQueryTranslatorFactory = _exprQueryTranslatorFactories
                .Where(m => StringUtil.compareIgnoreCase(m.Key, translatorName) || m.Key.isNullOrEmpty())
                .OrderByDescending(m => m.Key).Select(m => m.Value).FirstOrDefault();

            return matchQueryTranslatorFactory?.Invoke();
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
            return string.Join(", ", _exprQueryTranslatorFactories.Keys.ToArray());
        }
    }
}
