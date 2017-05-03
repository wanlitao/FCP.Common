using FCP.Core;
using FCP.Data;
using FCP.Entity;
using FCP.Util;
using FluentData;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var queryAction = BuildOrderByQueryAction(service.repository.dbContext, queryInfo.OrderByPropertyArr);

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

            var queryAction = BuildOrderByQueryAction(service.repository.dbContext, queryInfo.OrderByPropertyArr);

            return service.GetPageList(currentPage, pageSize, queryInfo.WherePredicate,
                queryAction, queryInfo.IgnorePropertyExprArr);
        }

        /// <summary>
        /// 构造查询排序Action
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="orderByPropertyArr">排序属性列表</param>
        /// <returns></returns>
        private static Action<ISelectBuilder<TEntity>> BuildOrderByQueryAction<TEntity>(IDbContext dbContext,
            params KeyValuePair<Expression<Func<TEntity, object>>, OrderByType>[] orderByPropertyArr) where TEntity : class
        {
            if (orderByPropertyArr.isEmpty())
                return null;

            var sqlGenerator = dbContext.dbContextImpl().sqlGenerator;

            return (selectBuilder) =>
            {
                var orderByPropertyMaps = orderByPropertyArr.Select(m => 
                    new KeyValuePair<IPropertyMap, OrderByType>(sqlGenerator.getProperty(m.Key), m.Value));

                var orderByColumnSqls = orderByPropertyMaps.Select(m =>
                    $"{sqlGenerator.getColumnName<TEntity>(m.Key, false, false)} {m.Value.ToString()}");

                selectBuilder.OrderBy(string.Join(",", orderByColumnSqls));
            };
        }
    }
}
