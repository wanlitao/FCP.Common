using FCP.Data;
using FCP.Util;
using FluentData;
using System;
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

        #region 构造函数
        public Repository(IDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

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
            if (id == null)
                throw new ArgumentNullException(nameof(id));

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
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

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
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return dbContext.deleteEntityByKey<TEntity>(id).Execute();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return dbContext.deleteEntity(entity).Execute();
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public virtual int deleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (includePropertyExpressions.isEmpty())
                throw new ArgumentNullException(nameof(includePropertyExpressions));

            return dbContext.deleteEntityByWhere(entity, includePropertyExpressions).Execute();
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
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (includePropertyExpressions.isEmpty())
                throw new ArgumentNullException(nameof(includePropertyExpressions));

            return dbContext.updateEntity(entity, includePropertyExpressions).Execute();
        }

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public virtual int updateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return dbContext.updateEntityIgnore(entity, ignorePropertyExpressions).Execute();
        }

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public virtual int updateByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (includePropertyExpressions.isEmpty())
                throw new ArgumentNullException(nameof(includePropertyExpressions));

            return dbContext.updateEntityByKey(id, entity, includePropertyExpressions).Execute();
        }

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public virtual int updateIgnoreByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return dbContext.updateEntityIgnoreByKey(id, entity, ignorePropertyExpressions).Execute();
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
