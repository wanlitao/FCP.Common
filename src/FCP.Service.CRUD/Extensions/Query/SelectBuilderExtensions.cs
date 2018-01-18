using FCP.Core;
using FCP.Util;
using FluentData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FCP.Service.CRUD
{
    /// <summary>
    /// SelectBuilder扩展
    /// </summary>
    internal static class SelectBuilderExtensions
    {
        internal static FCPDoResult<TResult> GetSingle<TResult>(this ISelectBuilder<TResult> selectBuilder,
            Action<TResult, IDataReader> customMapper = null)
        {
            var result = selectBuilder.QuerySingle(customMapper);

            return result == null ? FCPDoResultHelper.doNotFound<TResult>("not found any record")
                : FCPDoResultHelper.doSuccess(result);
        }

        internal static FCPDoResult<IList<TResult>> GetList<TResult>(this ISelectBuilder<TResult> selectBuilder,
            Action<TResult, IDataReader> customMapper = null)
        {
            var results = selectBuilder.QueryMany(customMapper);

            return results.isEmpty() ? FCPDoResultHelper.doNotFound<IList<TResult>>("not found any record")
                : FCPDoResultHelper.doSuccess<IList<TResult>>(results);
        }

        internal static FCPDoResult<FCPPageData<TResult>> GetPageList<TResult>(this ISelectBuilder<TResult> selectBuilder,
            int currentPage, int pageSize, Action<TResult, IDataReader> customMapper = null)
            where TResult : class
        {
            if (currentPage < 1)
                throw new ArgumentOutOfRangeException(nameof(currentPage), "current page must greater than zero");

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "page size must greater than zero");

            var pageData = new FCPPageData<TResult>() { pageIndex = currentPage, pageSize = pageSize };

            pageData.data = selectBuilder.Paging(currentPage, pageSize).QueryMany(customMapper);
            pageData.total = selectBuilder.FormatRecordCountSelectBuilder().QuerySingle();

            return pageData.data.isEmpty() ? FCPDoResultHelper.doNotFound<FCPPageData<TResult>>("not found any record of target page")
                : FCPDoResultHelper.doSuccess(pageData);
        }

        #region Helper Functions
        /// <summary>
        /// 格式化Count查询Builder
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selectBuilder"></param>
        /// <returns></returns>
        private static ISelectBuilder<int> FormatRecordCountSelectBuilder<TResult>(this ISelectBuilder<TResult> selectBuilder)
        {
            var countSelectBuilder = selectBuilder.Data.Command.Data.Context
                                               .Select<int>("Count(*)")
                                               .From(selectBuilder.Data.From)
                                               .Where(selectBuilder.Data.WhereSql)
                                               .GroupBy(selectBuilder.Data.GroupBy);

            foreach (var namedParameter in selectBuilder.Data.NamedParameters)
            {
                countSelectBuilder.Parameter(namedParameter.name, namedParameter.value,
                    namedParameter.parameterType, namedParameter.direction, namedParameter.size);
            }
            countSelectBuilder.Parameters(selectBuilder.Data.ObjectParameters.ToArray());

            return countSelectBuilder;
        }
        #endregion
    }
}
