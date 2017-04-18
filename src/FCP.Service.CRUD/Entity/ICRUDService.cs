using FCP.Core;
using FluentData;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FCP.Service.CRUD
{
    public interface ICRUDService<TEntity> : IService<TEntity> where TEntity : class
    {
        #region 查询
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FCPDoResult<TEntity> GetByKey(object id);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="wherePredicate">where条件</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        FCPDoResult<TEntity> GetSingle(Expression<Func<TEntity, bool>> wherePredicate,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="wherePredicate">where条件</param>
        /// <param name="queryAction">查询设置动作</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        FCPDoResult<IList<TEntity>> GetList(Expression<Func<TEntity, bool>> wherePredicate,
            Action<ISelectBuilder<TEntity>> queryAction, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="currentPage">页号</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="wherePredicate">where条件</param>
        /// <param name="queryAction">查询设置动作</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        FCPDoResult<FCPPageData<TEntity>> GetPageList(int currentPage, int pageSize,
            Expression<Func<TEntity, bool>> wherePredicate, Action<ISelectBuilder<TEntity>> queryAction,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 插入
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        FCPDoResult<TKey> Insert<TKey>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        FCPDoResult<int> Update(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        FCPDoResult<int> UpdateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);

        /// <summary>
        /// 更新对应主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        FCPDoResult<int> UpdateByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        /// <summary>
        /// 更新对应主键的实体（忽略属性）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        FCPDoResult<int> UpdateIgnoreByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 删除
        /// <summary>
        /// 删除指定主键的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FCPDoResult<int> DeleteByKey(object id);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        FCPDoResult<int> Delete(TEntity entity);

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        FCPDoResult<int> DeleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);
        #endregion
    }
}
