using FCP.Data;
using FCP.Entity;
using FluentData;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FCP.Repository
{
    /// <summary>
    /// 泛型数据仓储实现
    /// </summary>
    /// <typeparam name="TEnitty"></typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IDbContext _dbContext;

        /// <summary>
        /// 默认数据库会话工厂
        /// </summary>
        private static readonly IDbContextFactory defaultDbContextFactory = new DbContextFactory();

        #region 构造函数
        public Repository()
            : this(defaultDbContextFactory.openDbContext())
        {

        }

        public Repository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion

        #region 属性
        /// <summary>
        /// DbContext属性
        /// </summary>
        public IDbContext dbContext { get { return _dbContext; } }

        /// <summary>
        /// DbContext扩展类
        /// </summary>
        protected IDbContextImplementor dbContextImpl { get { return dbContext.dbContextImpl(); } }

        /// <summary>
        /// 数据库Sql生成器
        /// </summary>
        protected ISqlGenerator dbSqlGenerator { get { return dbContextImpl.sqlGenerator; } }        
        #endregion

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity getByKey(object id)
        {
            if (id == null) return null;
            return dbContext.selectEntityByKey<TEntity>(id).QuerySingle();
        }

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public virtual ISelectBuilder<TEntity> getListBuilder(params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            return getListBuilder(null, ignorePropertyExpressions);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="wherePredicate">where条件</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public virtual ISelectBuilder<TEntity> getListBuilder(Expression<Func<TEntity, bool>> wherePredicate,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            return dbContext.selectEntityByWhere(wherePredicate, ignorePropertyExpressions);
        }
        #endregion

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public virtual TKey insert<TKey>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            if (entity == null) return default(TKey);
            return dbContext.insertEntity(entity, ignorePropertyExpressions).ExecuteReturnLastId<TKey>();
        }

        #region 删除
        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int deleteByKey(object id)
        {
            if (id == null) return 0;
            return dbContext.deleteEntityByKey<TEntity>(id).Execute();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int delete(TEntity entity)
        {
            if (entity == null) return 0;
            return dbContext.deleteEntity(entity).Execute();
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyWheres">where条件</param>
        /// <returns></returns>
        public virtual int deleteByWhere(params KeyValuePair<Expression<Func<TEntity, object>>, object>[] propertyWheres)
        {
            return dbContext.deleteEntityByWhere(propertyWheres).Execute();
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public virtual int update(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            if (entity == null) return 0;
            return dbContext.updateEntity(entity, includePropertyExpressions).Execute();
        }

        /// <summary>
        /// 按主键更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>        
        /// <param name="id">主键值</param>
        /// <param name="propertyUpdates">更新字段值</param>
        /// <returns></returns>
        public virtual int updateByKey(object id, params KeyValuePair<Expression<Func<TEntity, object>>, object>[] propertyUpdates)
        {
            if (id == null) return 0;
            return dbContext.updateEntityByKey<TEntity>(id, propertyUpdates).Execute();
        }

        /// <summary>
        /// 按主键更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>        
        /// <param name="id">主键值</param>
        /// <param name="propertyUpdates">更新字段值</param>
        /// <returns></returns>
        public virtual int updateByKey(object id, params KeyValuePair<IPropertyMap, object>[] propertyUpdates)
        {
            if (id == null) return 0;
            return dbContext.updateEntityByKey<TEntity>(id, propertyUpdates).Execute();
        }

        /// <summary>
        /// 按where条件更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>       
        /// <param name="propertyWheres">where条件</param>
        /// <param name="propertyUpdates">更新字段值</param>
        /// <returns></returns>
        public virtual int updateByWhere(KeyValuePair<Expression<Func<TEntity, object>>, object>[] propertyWheres,
            params KeyValuePair<Expression<Func<TEntity, object>>, object>[] propertyUpdates)
        {
            return dbContext.updateEntityByWhere(propertyWheres, propertyUpdates).Execute();
        }
        #endregion

        /// <summary>
        /// 首行首列查询
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="selectSql">查询字段Sql</param>
        /// <param name="wherePredicate">where条件</param>
        /// <returns></returns>
        public virtual TResult executeScalar<TResult>(string selectSql, Expression<Func<TEntity, bool>> wherePredicate)
        {
            return dbContext.selectByWhere<TEntity, TResult>(selectSql, wherePredicate).QuerySingle();
        }
    }
}
