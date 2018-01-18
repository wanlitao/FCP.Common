using FCP.Core;
using FCP.Data;
using FCP.Entity;
using FCP.Util;
using FluentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace FCP.Service.CRUD
{
    /// <summary>
    /// CRUD查询扩展
    /// </summary>
    public static class CRUDServiceQueryExtensions
    {
        #region EntityQuery
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
        #endregion

        #region NativeQuery
        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns></returns>
        public static FCPDoResult<TResult> GetSingle<TEntity, TResult>(this ICRUDService<TEntity> service,
            NativeQuery queryInfo, Action<TResult, IDataReader> customMapper = null) where TEntity : class
        {
            if (queryInfo == null)
                throw new ArgumentNullException(nameof(queryInfo));

            var selectBuilder = BuildNativeQuerySelectBuilder<TEntity, TResult>(service, queryInfo);

            return selectBuilder.GetSingle(customMapper);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns></returns>
        public static FCPDoResult<TResult> GetSingle<TEntity, TResult>(this ICRUDService<TEntity> service,
            NativeQuery queryInfo, Action<TResult, dynamic> customMapper) where TEntity : class
        {
            return GetSingle(service, queryInfo, ConvertDataReaderMapper(customMapper));
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns></returns>
        public static FCPDoResult<IList<TResult>> GetList<TEntity, TResult>(this ICRUDService<TEntity> service,
            NativeQuery queryInfo, Action<TResult, IDataReader> customMapper = null) where TEntity : class
        {
            if (queryInfo == null)
                throw new ArgumentNullException(nameof(queryInfo));

            var selectBuilder = BuildNativeQuerySelectBuilder<TEntity, TResult>(service, queryInfo);

            return selectBuilder.GetList(customMapper);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns></returns>
        public static FCPDoResult<IList<TResult>> GetList<TEntity, TResult>(this ICRUDService<TEntity> service,
            NativeQuery queryInfo, Action<TResult, dynamic> customMapper) where TEntity : class
        {
            return GetList(service, queryInfo, ConvertDataReaderMapper(customMapper));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns></returns>
        public static FCPDoResult<FCPPageData<TResult>> GetPageList<TEntity, TResult>(this ICRUDService<TEntity> service,
            int currentPage, int pageSize, NativeQuery queryInfo, Action<TResult, IDataReader> customMapper = null)
            where TEntity : class
            where TResult : class
        {
            if (queryInfo == null)
                throw new ArgumentNullException(nameof(queryInfo));

            var selectBuilder = BuildNativeQuerySelectBuilder<TEntity, TResult>(service, queryInfo);

            return selectBuilder.GetPageList(currentPage, pageSize, customMapper);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns></returns>
        public static FCPDoResult<FCPPageData<TResult>> GetPageList<TEntity, TResult>(this ICRUDService<TEntity> service,
            int currentPage, int pageSize, NativeQuery queryInfo, Action<TResult, dynamic> customMapper)
            where TEntity : class
            where TResult : class
        {
            return GetPageList(service, currentPage, pageSize, queryInfo, ConvertDataReaderMapper(customMapper));
        }

        #region Helper Functions
        /// <summary>
        /// Convert to DataReader Mapper
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="customMapper"></param>
        /// <returns></returns>
        private static Action<TResult, IDataReader> ConvertDataReaderMapper<TResult>(Action<TResult, dynamic> customMapper)
        {
            if (customMapper == null)
                return null;

            return (result, reader) => customMapper.Invoke(result, new DynamicDataReader(reader.InnerReader));
        }

        /// <summary>
        /// 构造Native查询SelectBuilder
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="service"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        private static ISelectBuilder<TResult> BuildNativeQuerySelectBuilder<TEntity, TResult>(ICRUDService<TEntity> service,
            NativeQuery queryInfo) where TEntity : class
        {
            var selectBuilder = service.repository.query<TResult>();

            var newParameters = new List<object>();
            var whereSql = queryInfo.WhereSqlStr;
            var dbProvider = service.repository.dbContext.Data.FluentDataProvider;
            whereSql = processParams(whereSql, queryInfo.ParameterArr, dbProvider, newParameters);

            return selectBuilder.Select(queryInfo.SelectSqlStr)
                                .From(queryInfo.FromSqlStr)
                                .Where(whereSql)
                                .GroupBy(queryInfo.GroupBySqlStr)
                                .OrderBy(queryInfo.OrderBySqlStr)
                                .Parameters(newParameters.ToArray());
        }

        // Helper to handle named parameters from object properties
        private static string processParams(string sql, object[] args_src, IDbProvider dbProvider, List<object> args_dest)
        {
            return rxParams.Replace(sql, m =>
            {
                string param = m.Value.Substring(1);

                object arg_val;

                int paramIndex;
                if (int.TryParse(param, out paramIndex))
                {
                    // Numbered parameter
                    if (paramIndex < 0 || paramIndex >= args_src.Length)
                        throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", paramIndex, args_src.Length, sql));
                    arg_val = args_src[paramIndex];
                }
                else
                {
                    // Look for a property on one of the arguments with this name
                    bool found = false;
                    arg_val = null;
                    foreach (var o in args_src)
                    {
                        var pi = o.GetType().GetProperty(param);
                        if (pi != null)
                        {
                            arg_val = pi.GetValue(o, null);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        throw new ArgumentException(string.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", param, sql));
                }
                
                args_dest.Add(arg_val);
                return dbProvider.GetParameterName((args_dest.Count - 1).ToString());
            }
            );
        }

        private static Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
        #endregion;

        #endregion
    }
}
