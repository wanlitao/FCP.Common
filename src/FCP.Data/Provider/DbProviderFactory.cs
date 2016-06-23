using System;
using System.Collections.Generic;
using System.Linq;
using FluentData;
using System.Reflection;
using FCP.Util;

namespace FCP.Data
{
    /// <summary>
    /// Fluent数据库provider工厂
    /// </summary>
    public class DbProviderFactory
    {
        private static readonly IDictionary<string, IDbProvider> _dbProviders;  //DbProvider缓存

        static DbProviderFactory()
        {            
            var typeList = getDbProviderTypesFromAssemblies(Assembly.GetExecutingAssembly(), typeof(IDbProvider).Assembly);
            SortedDictionary<string, IDbProvider> sortedDictionary = new SortedDictionary<string, IDbProvider>();
            foreach (Type currentType in typeList)
            {
                IDbProvider fluentDbProvider = (IDbProvider)Activator.CreateInstance(currentType);
                var providerName = fluentDbProvider.ProviderName;
                if (!sortedDictionary.ContainsKey(providerName))
                {
                    sortedDictionary.Add(providerName, fluentDbProvider);
                }
            }

            _dbProviders = sortedDictionary;
        }

        /// <summary>
        /// 获取DbProvider实现类型
        /// </summary>
        /// <param name="assemblies">程序集列表</param>
        /// <returns></returns>
        private static IEnumerable<Type> getDbProviderTypesFromAssemblies(params Assembly[] assemblies)
        {
            if (assemblies.isEmpty())
                return null;

            return assemblies.SelectMany(m => m.GetExportedTypes().Where(type => type.IsConcrete() && type.Is<IDbProvider>()));
        }

        /// <summary>
        /// 获取数据库provider
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public IDbProvider getDbProvider(string providerName)
        {
            return _dbProviders.Where(m => StringUtil.compareIgnoreCase(m.Key, providerName))
                .Select(m => m.Value).FirstOrDefault<IDbProvider>();
        }

        /// <summary>
        /// 列出支持的数据库provider
        /// </summary>
        /// <returns></returns>
        public string listAvailableDbProviderTypes()
        {
            return string.Join(", ", _dbProviders.Keys.ToArray());
        }
    }
}
