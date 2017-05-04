using FCP.Util;
using System.Collections.Generic;
using System.Linq;

namespace FCP.Service.CRUD
{
    public class NativeQuery
    {
        public NativeQuery()
        {

        }
    }

    internal class SqlBuilder
    {
        private const string paramPrefix = "@";   //参数前缀        
        private const string paramPlaceHolderStr = "?";  //参数 占位符

        private int _paramIndex = 0;

        internal SqlBuilder()
        {
            SelectColumns = new List<string>();
            FromTables = new List<string>();
            WhereConditions = new List<string>();
            OrderByColumns = new Dictionary<string, OrderByType>();
            Parameters = new List<object>();
        }
                
        #region 查询字段
        /// <summary>
        /// select语句
        /// </summary>
        public string SelectSqlStr { get { return string.Join(",", SelectColumns); } }

        /// <summary>
        /// select语句
        /// </summary>
        protected IList<string> SelectColumns { get; set; }

        /// <summary>
        /// 添加查询字段
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public SqlBuilder Select(string column)
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
        public string FromSqlStr { get { return string.Join(" ", FromTables); } }

        /// <summary>
        /// from table列表
        /// </summary>
        protected IList<string> FromTables { get; set; }

        /// <summary>
        /// 添加表连接
        /// </summary>
        /// <param name="tableStr"></param>
        /// <returns></returns>
        public SqlBuilder From(string tableStr)
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
        /// <param name="parameterValue">参数值</param>
        /// <returns></returns>
        public SqlBuilder Where(string whereConditionStr, object parameterValue = null)
        {
            if (!whereConditionStr.isNullOrEmpty())
            {
                if (parameterValue != null)
                {
                    Parameters.Add(parameterValue);
                    //替换参数占位符
                    whereConditionStr = whereConditionStr.Replace(paramPlaceHolderStr, paramPrefix + (_paramIndex++));
                }

                WhereConditions.Add(whereConditionStr);
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
        public SqlBuilder OrderByAsc(string column)
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
        public SqlBuilder OrderByDesc(string column)
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
        #endregion
    }
}
