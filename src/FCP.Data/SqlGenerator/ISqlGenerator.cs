using FCP.Entity;
using FluentData;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FCP.Data
{
    /// <summary>
    /// Sql生成器接口
    /// </summary>
    public interface ISqlGenerator
    {
        #region 属性
        IEntityConfiguration entityConfiguration { get; }

        IDbProvider dbProvider { get; }
        #endregion        

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="alias">表的别名</param>
        /// <returns></returns>
        string getTableName<TEntity>(string alias) where TEntity : class;

        #region 获取列名
        /// <summary>
        /// 获取单一主键名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="includeTableName">是否包含表名</param>
        /// <param name="includeAlias">是否包含别名</param>
        /// <returns></returns>
        string getSingleKeyColumnName<TEntity>(bool includeTableName, bool includeAlias) where TEntity : class;

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyMap">属性Map</param>
        /// <param name="includeTableName">是否包含表名</param>
        /// <param name="includeAlias">是否包含别名</param>
        /// <returns></returns>
        string getColumnName<TEntity>(IPropertyMap propertyMap, bool includeTableName, bool includeAlias) where TEntity : class;
        string getColumnName<TEntity>(string propertyName, bool includeTableName, bool includeAlias) where TEntity : class;        
        string getColumnName(Type entityType, string propertyName, bool includeTableName, bool includeAlias);
        #endregion

        #region 获取属性
        /// <summary>
        /// 获取主键属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IEnumerable<IPropertyMap> getKeyProperties<TEntity>() where TEntity : class;

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        IPropertyMap getProperty<TEntity>(string propertyName) where TEntity : class;

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyExpression">属性表达式</param>
        /// <returns></returns>
        IPropertyMap getProperty<TEntity>(
            Expression<Func<TEntity, object>> propertyExpression) where TEntity : class;

        /// <summary>
        /// 获取属性集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyExpressions">属性表达式集合</param>
        /// <returns></returns>
        IEnumerable<IPropertyMap> getProperties<TEntity>(
            params Expression<Func<TEntity, object>>[] propertyExpressions) where TEntity : class;

        /// <summary>
        /// 获取查询的属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        IEnumerable<IPropertyMap> getSelectProperties<TEntity>(
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        /// <summary>
        /// 获取更新的属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="isExclude">是否排除</param>
        /// <param name="propertyExpressions">属性表达式</param>
        /// <returns></returns>
        IEnumerable<IPropertyMap> getUpdateProperties<TEntity>(bool isExclude,
            params Expression<Func<TEntity, object>>[] propertyExpressions) where TEntity : class;

        /// <summary>
        /// 获取插入的属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        IEnumerable<IPropertyMap> getInsertProperties<TEntity>(
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class;

        #region 根据表达式获取属性名称
        /// <summary>
        /// 根据表达式获取属性名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        string getPropertyNameByExpression<TEntity>(
            Expression<Func<TEntity, object>> propertyExpression) where TEntity : class;        

        /// <summary>
        /// 根据表达式获取属性名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyExpressions">属性表达式</param>
        /// <returns></returns>
        IEnumerable<string> getPropertyNamesByExpression<TEntity>(
            params Expression<Func<TEntity, object>>[] propertyExpressions) where TEntity : class;
        #endregion

        #endregion
    }
}
