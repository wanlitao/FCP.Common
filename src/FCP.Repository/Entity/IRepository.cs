using FluentData;
using System;
using System.Linq.Expressions;

namespace FCP.Repository
{
    /// <summary>
    /// 泛型数据仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        #region 属性
        /// <summary>
        /// 当前数据库上下文
        /// </summary>
        IDbContext dbContext { get; }
        #endregion

        #region 查询
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity getByKey(object id);

        /// <summary>
        /// 获取查询Builder
        /// </summary>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        ISelectBuilder<TEntity> query(params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);

        /// <summary>
        /// 获取查询Builder
        /// </summary>
        /// <param name="wherePredicate">where条件</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        ISelectBuilder<TEntity> query(Expression<Func<TEntity, bool>> wherePredicate,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        TKey insert<TKey>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        int update(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        int updateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);

        /// <summary>
        /// 按主键更新
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        int updateByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>        
        /// <param name="id">主键值</param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        int updateIgnoreByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 删除
        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int deleteByKey(object id);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int delete(TEntity entity);

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        int deleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);
        #endregion 

        /// <summary>
        /// 首行首列查询
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="selectSql">查询字段Sql</param>
        /// <param name="wherePredicate">where条件</param>
        /// <returns></returns>
        TResult executeScalar<TResult>(string selectSql, Expression<Func<TEntity, bool>> wherePredicate);        
    }
}
