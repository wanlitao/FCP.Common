using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FCP.Util;

namespace FCP.Service.CRUD
{
    public class EntityQuery<TEntity> where TEntity : class
    {
        public EntityQuery()
        {
            WhereConditions = new List<Expression<Func<TEntity, bool>>>();
            IgnorePropertyExpressions = new List<Expression<Func<TEntity, object>>>();
            OrderByProperties = new Dictionary<Expression<Func<TEntity, object>>, OrderByType>();
        }

        #region where条件
        /// <summary>
        /// where条件
        /// </summary>
        public Expression<Func<TEntity, bool>> WherePredicate { get { return CombineWhereConditions(WhereConditions.ToArray()); } }

        /// <summary>
        /// where条件列表
        /// </summary>
        protected IList<Expression<Func<TEntity, bool>>> WhereConditions { get; set; }

        /// <summary>
        /// 组合where条件
        /// </summary>
        /// <param name="whereConditionArr"></param>
        /// <returns></returns>
        private static Expression<Func<TEntity, bool>> CombineWhereConditions(
            params Expression<Func<TEntity, bool>>[] whereConditionArr)
        {
            if (whereConditionArr.isEmpty())
                return null;
            
            var parameterExpr = Expression.Parameter(typeof(TEntity), "entity");
            var replaceVisitor = new ParameterReplaceVisitor(parameterExpr);
            Expression predicateBodyExpr = null;

            foreach (var whereCondition in whereConditionArr)
            {
                var newConditionBodyExpr = replaceVisitor.Visit(whereCondition.Body);

                predicateBodyExpr = (predicateBodyExpr == null) ? newConditionBodyExpr
                    : Expression.AndAlso(predicateBodyExpr, newConditionBodyExpr);
            }

            return Expression.Lambda<Func<TEntity, bool>>(predicateBodyExpr, parameterExpr);
        }

        /// <summary>
        /// 添加where条件
        /// </summary>        
        /// <param name="whereCondition"></param>
        /// <returns></returns>
        public EntityQuery<TEntity> Where(Expression<Func<TEntity, bool>> whereCondition)
        {
            WhereConditions.Add(whereCondition);

            return this;
        }
        #endregion

        #region 忽略属性表达式
        /// <summary>
        /// 忽略属性表达式 数组
        /// </summary>
        public Expression<Func<TEntity, object>>[] IgnorePropertyExprArr { get { return IgnorePropertyExpressions.ToArray(); } }

        /// <summary>
        /// 忽略属性表达式列表
        /// </summary>
        protected IList<Expression<Func<TEntity, object>>> IgnorePropertyExpressions { get; set; }

        /// <summary>
        /// 添加忽略字段
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public EntityQuery<TEntity> Ignore(Expression<Func<TEntity, object>> propertyExpression)
        {
            IgnorePropertyExpressions.Add(propertyExpression);

            return this;
        }
        #endregion

        #region 排序字段
        /// <summary>
        /// 排序字段 数组
        /// </summary>
        public KeyValuePair<Expression<Func<TEntity, object>>, OrderByType>[] OrderByPropertyArr { get { return OrderByProperties.ToArray(); } }

        /// <summary>
        /// 排序字段字典
        /// </summary>
        protected IDictionary<Expression<Func<TEntity, object>>, OrderByType> OrderByProperties { get; set; }

        /// <summary>
        /// 添加升序排序字段
        /// </summary>        
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public EntityQuery<TEntity> OrderByAsc(Expression<Func<TEntity, object>> propertyExpression)
        {
            OrderByProperties.Add(propertyExpression, OrderByType.Asc);

            return this;
        }

        /// <summary>
        /// 添加降序排序字段
        /// </summary>        
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public EntityQuery<TEntity> OrderByDesc(Expression<Func<TEntity, object>> propertyExpression)
        {
            OrderByProperties.Add(propertyExpression, OrderByType.Desc);

            return this;
        }
        #endregion
    }

    /// <summary>
    /// 参数替换 ExpressionVisitor
    /// </summary>
    internal class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _newParameter;

        internal ParameterReplaceVisitor(ParameterExpression newParameter)
        {
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _newParameter;
        }
    }
}
