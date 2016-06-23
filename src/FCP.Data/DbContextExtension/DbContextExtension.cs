using FluentData;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace FCP.Data
{
    /// <summary>
    /// 数据库context扩展
    /// </summary>
    public static class DbContextExtension
    {
        private static IDbConfiguration _dbConfiguration;
        private static Func<IDbConfiguration, IDbProvider, IDbContextImplementor> _dbContextImplFactory;
        /// <summary>
        /// 数据库context扩展实现 缓存
        /// </summary>
        private static ConcurrentDictionary<string, IDbContextImplementor> _dbContextImplCache = new ConcurrentDictionary<string, IDbContextImplementor>();

        static DbContextExtension()
        {
            configure(new DbConfiguration());
        }

        #region 静态参数配置
        /// <summary>
        /// Configure DapperExtensions extension methods.
        /// </summary>
        /// <param name="dbConfiguration"></param>        
        public static void configure(IDbConfiguration dbConfiguration)
        {
            _dbConfiguration = dbConfiguration;
            _dbContextImplCache.Clear();
        }

        /// <summary>
        /// Configure DapperExtensions extension methods.
        /// </summary>
        /// <param name="defaultClassMapper"></param>
        /// <param name="mappingAssemblies"></param>        
        public static void configure(Type defaultClassMapper, IList<Assembly> mappingAssemblies)
        {
            configure(new DbConfiguration(defaultClassMapper, mappingAssemblies));
        }

        /// <summary>
        /// Gets or sets the default class mapper to use when generating class maps. If not specified, AutoClassMapper<T> is used.        
        /// </summary>
        public static Type defaultClassMapper
        {
            get
            {
                return _dbConfiguration.defaultClassMapper;
            }
            set
            {
                configure(value, _dbConfiguration.mappingAssemblies);
            }
        }

        /// <summary>
        /// Get or sets the Dapper Extensions Implementation Factory.
        /// </summary>
        public static Func<IDbConfiguration, IDbProvider, IDbContextImplementor> dbContextImplFactory
        {
            get
            {
                if (_dbContextImplFactory == null)
                {
                    _dbContextImplFactory = (config, provider) => new DbContextImplementor(config, provider);
                }

                return _dbContextImplFactory;
            }
            set
            {
                _dbContextImplFactory = value;
                configure(_dbConfiguration);
            }
        }

        /// <summary>
        /// Add other assemblies that Dapper Extensions will search if a mapping is not found in the same assembly of the POCO.
        /// </summary>
        /// <param name="assemblies"></param>
        public static void setMappingAssemblies(IList<Assembly> assemblies)
        {
            configure(_dbConfiguration.defaultClassMapper, assemblies);
        }
        #endregion

        #region 获取数据库context扩展实现
        /// <summary>
        /// 获取数据库context扩展实现
        /// </summary>
        /// <param name="fluentDbProvider"></param>
        /// <returns></returns>
        private static IDbContextImplementor getDbContextImpl(IDbProvider fluentDbProvider)
        {
            string providerName = fluentDbProvider.ProviderName;
            IDbContextImplementor dbContextImpl = null;
            if (!_dbContextImplCache.TryGetValue(providerName, out dbContextImpl))
            {
                dbContextImpl = dbContextImplFactory(_dbConfiguration, fluentDbProvider);
                if (dbContextImpl != null)
                {
                    _dbContextImplCache.TryAdd(providerName, dbContextImpl);  //添加到缓存
                }
            }
            return dbContextImpl;
        }

        /// <summary>
        /// 获取数据库context扩展实现
        /// </summary>        
        /// <returns></returns>
        public static IDbContextImplementor dbContextImpl(this IDbContext dbContext)
        {
            return getDbContextImpl(dbContext.Data.FluentDataProvider);
        }
        #endregion

        #region select查询
        /// <summary>
        /// 按主键查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public static ISelectBuilder<TEntity> selectEntityByKey<TEntity>(this IDbContext dbContext, object id,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.selectEntityByKey<TEntity>(dbContext, id, ignorePropertyExpressions);
        }

        /// <summary>
        /// 按where条件查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext">数据库context</param>
        /// <param name="wherePredicate">where条件</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public static ISelectBuilder<TEntity> selectEntityByWhere<TEntity>(this IDbContext dbContext, Expression<Func<TEntity, bool>> wherePredicate,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.selectEntityByWhere<TEntity>(dbContext, wherePredicate, ignorePropertyExpressions);
        }

        /// <summary>
        /// 按where条件查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="dbContext"></param>
        /// <param name="selectSql">查询字段Sql</param>
        /// <param name="wherePredicate">where条件</param>
        /// <returns></returns>
        public static ISelectBuilder<TResult> selectByWhere<TEntity, TResult>(this IDbContext dbContext, string selectSql,
            Expression<Func<TEntity, bool>> wherePredicate) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.selectByWhere<TEntity, TResult>(dbContext, selectSql, wherePredicate);
        }   
        #endregion

        #region delete删除
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public static IDeleteBuilder deleteEntityByKey<TEntity>(this IDbContext dbContext, object id) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.deleteEntityByKey<TEntity>(dbContext, id);
        }

        /// <summary>
        /// 按实体主键删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public static IDeleteBuilder deleteEntity<TEntity>(this IDbContext dbContext, TEntity entity) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.deleteEntity<TEntity>(dbContext, entity);
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext">数据库context</param>
        /// <param name="propertyWheres">where条件</param>
        /// <returns></returns>
        public static IDeleteBuilder deleteEntityByWhere<TEntity>(this IDbContext dbContext,
            params KeyValuePair<Expression<Func<TEntity, object>>, object>[] propertyWheres) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.deleteEntityByWhere<TEntity>(dbContext, propertyWheres);
        }
        #endregion

        #region update更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public static IUpdateBuilder updateEntity<TEntity>(this IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.updateEntity<TEntity>(dbContext, entity, includePropertyExpressions);
        }

        /// <summary>
        /// 按主键更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <param name="propertyUpdates">更新字段值</param>
        /// <returns></returns>
        public static IUpdateBuilder updateEntityByKey<TEntity>(this IDbContext dbContext, object id,
            params KeyValuePair<Expression<Func<TEntity, object>>, object>[] propertyUpdates) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.updateEntityByKey<TEntity>(dbContext, id, propertyUpdates);
        }

        /// <summary>
        /// 按主键更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <param name="propertyUpdates">更新字段值</param>
        /// <returns></returns>
        public static IUpdateBuilder updateEntityByKey<TEntity>(this IDbContext dbContext, object id,
            params KeyValuePair<IPropertyMap, object>[] propertyUpdates) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.updateEntityByKey<TEntity>(dbContext, id, propertyUpdates);
        }

        /// <summary>
        /// 按where条件更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="propertyWheres">where条件</param>
        /// <param name="propertyUpdates">更新字段值</param>
        /// <returns></returns>
        public static IUpdateBuilder updateEntityByWhere<TEntity>(this IDbContext dbContext, KeyValuePair<Expression<Func<TEntity, object>>, object>[] propertyWheres,
            params KeyValuePair<Expression<Func<TEntity, object>>, object>[] propertyUpdates) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.updateEntityByWhere<TEntity>(dbContext, propertyWheres, propertyUpdates);
        }
        #endregion

        #region insert插入
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public static IInsertBuilder insertEntity<TEntity>(this IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            IDbContextImplementor dbContextImpl = dbContext.dbContextImpl();
            return dbContextImpl.insertEntity<TEntity>(dbContext, entity, ignorePropertyExpressions);
        }
        #endregion
    }
}
