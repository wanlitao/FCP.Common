using FCP.Core;
using FluentData;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FCP.Service.CRUD
{
    /// <summary>
    /// CRUD查询扩展
    /// </summary>
    public static class CRUDServiceQueryExtensions
    {
        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns></returns>
        public static FCPDoResult<TEntity> GetSingle<TEntity>(this ICRUDService<TEntity> service,
            EntityQuery<TEntity> queryInfo) where TEntity : class
        {
            if (queryInfo == null)
                throw new ArgumentNullException(nameof(queryInfo));

            return service.GetSingle(queryInfo.WherePredicate, queryInfo.IgnorePropertyExprArr);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns></returns>
        public static FCPDoResult<IList<TEntity>> GetList<TEntity>(this ICRUDService<TEntity> service,
            EntityQuery<TEntity> queryInfo) where TEntity : class
        {
            if (queryInfo == null)
                throw new ArgumentNullException(nameof(queryInfo));

            var queryAction = BuildOrderByQueryAction(queryInfo.OrderByPropertyArr);

            return service.GetList(queryInfo.WherePredicate, queryAction, queryInfo.IgnorePropertyExprArr);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns></returns>
        public static FCPDoResult<FCPPageData<TEntity>> GetPageList<TEntity>(this ICRUDService<TEntity> service,
            int currentPage, int pageSize, EntityQuery<TEntity> queryInfo) where TEntity : class
        {
            if (queryInfo == null)
                throw new ArgumentNullException(nameof(queryInfo));

            var queryAction = BuildOrderByQueryAction(queryInfo.OrderByPropertyArr);

            return service.GetPageList(currentPage, pageSize, queryInfo.WherePredicate,
                queryAction, queryInfo.IgnorePropertyExprArr);
        }

        private static Action<ISelectBuilder<TEntity>> BuildOrderByQueryAction<TEntity>(
            params KeyValuePair<Expression<Func<TEntity, object>>, OrderByType>[] orderByPropertyArr) where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
