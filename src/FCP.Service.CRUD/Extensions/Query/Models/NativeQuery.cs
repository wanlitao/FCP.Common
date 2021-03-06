﻿using FCP.Util;
using System.Collections.Generic;
using System.Linq;

namespace FCP.Service.CRUD
{
    public class NativeQuery
    {
        public NativeQuery()
        {
            SelectColumns = new List<string>();
            FromTables = new List<string>();
            WhereConditions = new List<string>();
            GroupByColumns = new List<string>();
            OrderByColumns = new Dictionary<string, OrderByType>();
            Parameters = new List<object>();
        }
                
        #region 查询字段
        /// <summary>
        /// select语句
        /// </summary>
        public string SelectSqlStr { get { return string.Join(",", SelectColumns); } }

        /// <summary>
        /// 查询字段列表
        /// </summary>
        protected IList<string> SelectColumns { get; set; }

        /// <summary>
        /// 添加查询字段
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public NativeQuery Select(string column)
        {
            if (!column.isNullOrEmpty())
            {
                SelectColumns.Add(column);
            }

            return this;
        }
        #endregion

        #region From Table
        /// <summary>
        /// from语句
        /// </summary>
        public string FromSqlStr { get { return string.Join(",", FromTables); } }

        /// <summary>
        /// from table列表
        /// </summary>
        protected IList<string> FromTables { get; set; }

        /// <summary>
        /// 添加表连接
        /// </summary>
        /// <param name="tableStr"></param>
        /// <returns></returns>
        public NativeQuery From(string tableStr)
        {
            if (!tableStr.isNullOrEmpty())
            {
                FromTables.Add(tableStr);
            }

            return this;
        }
        #endregion

        #region where条件
        /// <summary>
        /// Where条件语句
        /// </summary>
        public string WhereSqlStr { get { return string.Join(" and ", WhereConditions); } }

        /// <summary>
        /// where条件列表
        /// </summary>
        protected IList<string> WhereConditions { get; set; }

        /// <summary>
        /// 添加where条件
        /// </summary>
        /// <param name="whereConditionStr">where条件</param>
        /// <param name="parameterValues">参数值</param>
        /// <returns></returns>
        public NativeQuery Where(string whereConditionStr, params object[] parameterValues)
        {
            if (!whereConditionStr.isNullOrEmpty())
            {
                if (parameterValues.isNotEmpty())
                {
                    foreach (var parameterValue in parameterValues)
                    {
                        Parameters.Add(parameterValue);
                    }
                }

                WhereConditions.Add(whereConditionStr);
            }

            return this;
        }
        #endregion

        #region 分组字段
        /// <summary>
        /// groupby分组语句
        /// </summary>
        public string GroupBySqlStr { get { return string.Join(",", GroupByColumns); } }

        /// <summary>
        /// 分组字段列表
        /// </summary>
        protected IList<string> GroupByColumns { get; set; }

        /// <summary>
        /// 添加分组字段
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public NativeQuery GroupBy(string column)
        {
            if (!column.isNullOrEmpty())
            {
                GroupByColumns.Add(column);
            }

            return this;
        }
        #endregion

        #region 排序字段
        /// <summary>
        /// orderby排序语句
        /// </summary>
        public string OrderBySqlStr { get { return string.Join(",", OrderByColumns.Select(m => $"{m.Key} {m.Value.ToString()}")); } }

        /// <summary>
        /// orderby排序字段字典
        /// </summary>
        protected IDictionary<string, OrderByType> OrderByColumns { get; set; }

        /// <summary>
        /// 添加升序排序字段
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public NativeQuery OrderByAsc(string column)
        {
            if (!column.isNullOrEmpty())
            {
                OrderByColumns.Add(column, OrderByType.Asc);
            }

            return this;
        }

        /// <summary>
        /// 添加降序排序字段
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public NativeQuery OrderByDesc(string column)
        {
            if (!column.isNullOrEmpty())
            {
                OrderByColumns.Add(column, OrderByType.Desc);
            }

            return this;
        }
        #endregion

        #region 查询参数
        /// <summary>
        /// 查询参数
        /// </summary>
        public object[] ParameterArr { get { return Parameters.ToArray(); } }

        /// <summary>
        /// 查询参数
        /// </summary>
        protected IList<object> Parameters { get; set; }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public NativeQuery Param(object parameterValue)
        {
            if (parameterValue != null)
            {
                Parameters.Add(parameterValue);
            }

            return this;
        }
        #endregion
    }
}
