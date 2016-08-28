using System;
using System.Linq;
using FluentData;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using FCP.Util;
using FCP.Entity;

namespace FCP.Data
{
    /// <summary>
    /// Sql生成器
    /// </summary>
    public class SqlGenerator : ISqlGenerator
    {
        public SqlGenerator(IDbConfiguration configuration, IDbProvider dbProvider)
        {
            dbConfiguration = configuration;
            this.dbProvider = dbProvider;
        }

        #region 属性
        public IDbConfiguration dbConfiguration { get; private set; }

        public IDbProvider dbProvider { get; private set; }
        #endregion        

        #region 获取表名
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="alias">表的别名</param>
        /// <returns></returns>
        public string getTableName<TEntity>(string alias) where TEntity : class
        {
            IClassMapper entityMapper = dbConfiguration.getClassMapper<TEntity>();

            return getTableName(entityMapper, alias);
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="entityMapper">实体映射</param>
        /// <param name="alias">表的别名</param>
        /// <returns></returns>
        protected virtual string getTableName(IClassMapper entityMapper, string alias)
        {
            if (string.IsNullOrWhiteSpace(entityMapper.tableName))
            {
                throw new ArgumentNullException("TableName", "tableName cannot be null or empty.");
            }

            string tableFullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(entityMapper.schemaName))
            {
                tableFullName = dbProvider.EscapeColumnName(entityMapper.schemaName) + ".";
            }
            tableFullName += dbProvider.EscapeColumnName(entityMapper.tableName);

            if (!string.IsNullOrWhiteSpace(alias))
            {
                tableFullName = dbProvider.GetSelectBuilderAlias(tableFullName,
                    dbProvider.EscapeColumnName(alias));
            }
            return tableFullName;
        }
        #endregion        

        #region 获取列名
        /// <summary>
        /// 获取单一主键列名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="includeTableName">是否包含表名</param>
        /// <param name="includeAlias">是否包含别名</param>
        /// <returns></returns>
        public string getSingleKeyColumnName<TEntity>(bool includeTableName,
            bool includeAlias) where TEntity : class
        {
            var keyPropertyMap = getKeyProperties<TEntity>().SingleOrDefault();

            return getColumnName<TEntity>(keyPropertyMap, includeTableName, includeAlias);
        }

        /// <summary>
        /// 获取列名 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyMap">属性Map</param>
        /// <param name="includeTableName">是否包含表名</param>
        /// <param name="includeAlias">是否包含别名</param>
        /// <returns></returns>
        public string getColumnName<TEntity>(IPropertyMap propertyMap, bool includeTableName,
            bool includeAlias) where TEntity : class
        {
            IClassMapper entityMapper = dbConfiguration.getClassMapper<TEntity>();

            return getColumnName(entityMapper, propertyMap, includeTableName, includeAlias);
        }

        /// <summary>
        /// 获取列名 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="includeTableName">是否包含表名</param>
        /// <param name="includeAlias">是否包含别名</param>
        /// <returns></returns>
        public string getColumnName<TEntity>(string propertyName, bool includeTableName,
            bool includeAlias) where TEntity : class
        {
            return getColumnName(typeof(TEntity), propertyName, includeTableName, includeAlias);
        }

        /// <summary>
        /// 获取列名 
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="includeTableName">是否包含表名</param>
        /// <param name="includeAlias">是否包含别名</param>
        /// <returns></returns>
        public string getColumnName(Type entityType, string propertyName, bool includeTableName,
            bool includeAlias)
        {
            if (propertyName.isNullOrEmpty())
                return string.Empty;

            IClassMapper entityMapper = dbConfiguration.getClassMapper(entityType);
            IPropertyMap propertyMap = entityMapper.properties.SingleOrDefault(
                p => p.name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (propertyMap == null)
            {
                throw new ArgumentException(string.Format("Could not find '{0}' in Mapping.", propertyName));
            }

            return getColumnName(entityMapper, propertyMap, includeTableName, includeAlias);
        }

        /// <summary>
        /// 获取列名 
        /// </summary>
        /// <param name="entityMapper">实体映射</param> 
        /// <param name="propertyMap">属性Map</param>
        /// <param name="includeTableName">是否包含表名</param>
        /// <param name="includeAlias">是否包含别名</param>
        /// <returns></returns>
        protected virtual string getColumnName(IClassMapper entityMapper, IPropertyMap propertyMap,
            bool includeTableName, bool includeAlias)
        {
            if (string.IsNullOrWhiteSpace(propertyMap.columnName))
            {
                throw new ArgumentNullException("ColumnName", "columnName cannot be null or empty.");
            }

            string columnFullName = string.Format("{0}{1}",
                includeTableName ? (getTableName(entityMapper, null) + ".") : string.Empty,
                dbProvider.EscapeColumnName(propertyMap.columnName));

            //属性名和列名不一致并且包含别名时 添加别名Alias
            if (propertyMap.columnName != propertyMap.name && includeAlias)
            {
                columnFullName = dbProvider.GetSelectBuilderAlias(columnFullName,
                    dbProvider.EscapeColumnName(propertyMap.name));
            }
            return columnFullName;
        }
        #endregion

        #region 获取属性
        /// <summary>
        /// 获取主键属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IEnumerable<IPropertyMap> getKeyProperties<TEntity>() where TEntity : class
        {
            IClassMapper entityMapper = dbConfiguration.getClassMapper<TEntity>();
            var keyPropertyMaps = entityMapper.properties.Where(p => p.keyType != KeyType.notAKey);

            if (keyPropertyMaps.isEmpty())
            {
                throw new ArgumentException("Could not find key column in Mapping.");
            }

            return keyPropertyMaps;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public IPropertyMap getProperty<TEntity>(string propertyName) where TEntity : class
        {
            if (propertyName.isNullOrEmpty())
                return null;

            IClassMapper entityMapper = dbConfiguration.getClassMapper<TEntity>();
            IPropertyMap propertyMap = entityMapper.properties.SingleOrDefault(
                p => p.name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            if (propertyMap == null)
            {
                throw new ArgumentException(string.Format("Could not find '{0}' in Mapping.", propertyName));
            }

            return propertyMap;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyExpression">属性表达式</param>
        /// <returns></returns>
        public IPropertyMap getProperty<TEntity>(
            Expression<Func<TEntity, object>> propertyExpression) where TEntity : class
        {
            if (propertyExpression == null)
                return null;

            var propertyName = getPropertyNameByExpression(propertyExpression);

            return getProperty<TEntity>(propertyName);             
        }

        /// <summary>
        /// 获取属性集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyExpressions">属性表达式集合</param>
        /// <returns></returns>
        public IEnumerable<IPropertyMap> getProperties<TEntity>(
            params Expression<Func<TEntity, object>>[] propertyExpressions) where TEntity : class
        {
            if (propertyExpressions.isNotEmpty())
            {
                foreach (var propertyExpression in propertyExpressions)
                {
                    yield return getProperty(propertyExpression);
                }
            }
        }

        /// <summary>
        /// 获取查询的属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public IEnumerable<IPropertyMap> getSelectProperties<TEntity>(
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            IClassMapper entityMapper = dbConfiguration.getClassMapper<TEntity>();
            var selectProperties = entityMapper.properties.Where(p => !p.ignored);  //排除忽略的属性

            var ignorePropertyNames = getPropertyNamesByExpression(ignorePropertyExpressions);
            if (ignorePropertyNames.isNotEmpty()) //忽略指定属性列表
            {
                selectProperties = selectProperties.Where(p => !ignorePropertyNames.Contains(p.name));
            }            

            return selectProperties;
        }

        /// <summary>
        /// 获取更新的属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public IEnumerable<IPropertyMap> getUpdateProperties<TEntity>(
            params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class
        {
            IClassMapper entityMapper = dbConfiguration.getClassMapper<TEntity>();
            var updateProperties = entityMapper.properties.Where(
                p => !p.ignored && !p.isReadOnly && p.keyType == KeyType.notAKey);  //排除主键和只读忽略的属性

            var includePropertyNames = getPropertyNamesByExpression(includePropertyExpressions);
            if (includePropertyNames.isNotEmpty()) //只获取指定更新的属性列表
            {               
                updateProperties = updateProperties.Where(p => includePropertyNames.Contains(p.name));
            }

            return updateProperties;
        }

        /// <summary>
        /// 获取插入的属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public IEnumerable<IPropertyMap> getInsertProperties<TEntity>(
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            IClassMapper entityMapper = dbConfiguration.getClassMapper<TEntity>();
            var insertProperties = entityMapper.properties.Where(
                p => !p.ignored && !p.isReadOnly && p.keyType != KeyType.identity);  //排除自增主键和只读忽略的属性

            var ignorePropertyNames = getPropertyNamesByExpression(ignorePropertyExpressions);
            if (ignorePropertyNames.isNotEmpty()) //忽略指定属性列表
            {               
                insertProperties = insertProperties.Where(p => !ignorePropertyNames.Contains(p.name));
            }

            return insertProperties;
        }

        #region 根据表达式获取属性名称
        /// <summary>
        /// 根据表达式获取属性名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public string getPropertyNameByExpression<TEntity>(
            Expression<Func<TEntity, object>> propertyExpression) where TEntity : class
        {
            if (propertyExpression == null)
                return string.Empty;

            var propertyInfo = ReflectionHelper.getProperty(propertyExpression) as PropertyInfo;
            if (propertyInfo == null)
                return string.Empty;

            return propertyInfo.Name;
        }

        /// <summary>
        /// 根据表达式获取属性名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyExpressions">属性表达式</param>
        /// <returns></returns>
        public IEnumerable<string> getPropertyNamesByExpression<TEntity>(
            params Expression<Func<TEntity, object>>[] propertyExpressions) where TEntity : class
        {
            if (propertyExpressions.isNotEmpty())
            {
                foreach (var propertyExpression in propertyExpressions)
                {
                    yield return getPropertyNameByExpression(propertyExpression);
                }
            }
        }
        #endregion

        #endregion
    }
}
