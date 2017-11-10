using FluentData;
using ExprTranslator.Query;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Reflection;
using FCP.Entity;

namespace FCP.Data
{
    /// <summary>
    /// 数据库context扩展接口
    /// </summary>
    public interface IDbContextImplementor
    {
        /// <summary>
        /// Fluent查询Sql生成器
        /// </summary>
        ISqlGenerator sqlGenerator { get; }

        /// <summary>
        /// 表达式查询translator
        /// </summary>
        IExprQueryTranslator exprQueryTranslator { get; }

        #region 获取属性表达式
        /// <summary>
        /// 获取属性表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        Expression<Func<TEntity, object>> getPropertyExpression<TEntity>(string propertyName) where TEntity : class;

        /// <summary>
        /// 获取属性表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyMap">属性Map信息</param>
        /// <returns></returns>
        Expression<Func<TEntity, object>> getPropertyExpression<TEntity>(IPropertyMap propertyMap) where TEntity : class;        
        #endregion

        #region 获取属性值Predicate
        /// <summary>
        /// 获取属性值Predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        Expression<Func<TEntity, bool>> getPropertyPredicate<TEntity>(string propertyName, object propertyValue) where TEntity : class;

        /// <summary>
        /// 获取属性值Predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyMap">属性Map信息</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        Expression<Func<TEntity, bool>> getPropertyPredicate<TEntity>(IPropertyMap propertyMap, object propertyValue) where TEntity : class;
        #endregion

        #region 获取Sql
        /// <summary>
        /// 获取查询字段Sql
        /// </summary>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        string getEntitySelectSql<TEntity>(params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;        
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
        ISelectBuilder<TEntity> selectEntityByKey<TEntity>(IDbContext dbContext, object id,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 按where条件查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext">数据库context</param>
        /// <param name="wherePredicate">where条件</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        ISelectBuilder<TEntity> selectEntityByWhere<TEntity>(IDbContext dbContext, Expression<Func<TEntity, bool>> wherePredicate,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 按where条件查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="dbContext"></param>
        /// <param name="selectSql">查询字段Sql</param>
        /// <param name="wherePredicate">where条件</param>
        /// <returns></returns>
        ISelectBuilder<TResult> selectByWhere<TEntity, TResult>(IDbContext dbContext, string selectSql,
            Expression<Func<TEntity, bool>> wherePredicate) where TEntity : class;
        #endregion

        #region delete删除
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        IExecute deleteEntityByKey<TEntity>(IDbContext dbContext, object id) where TEntity : class;

        /// <summary>
        /// 按实体主键删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        IExecute deleteEntity<TEntity>(IDbContext dbContext, TEntity entity) where TEntity : class;

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext">数据库context</param>
        /// <param name="entity">实体</param>
        /// <param name="includePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        IExecute deleteEntityByWhere<TEntity>(IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class;
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
        IExecute updateEntity<TEntity>(IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        IExecute updateEntityIgnore<TEntity>(IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 按主键更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体</param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        IExecute updateEntityByKey<TEntity>(IDbContext dbContext, object id,
            TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        IExecute updateEntityIgnoreByKey<TEntity>(IDbContext dbContext, object id,
            TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;        
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
        IInsertBuilder insertEntity<TEntity>(IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;
        #endregion
    }
}
