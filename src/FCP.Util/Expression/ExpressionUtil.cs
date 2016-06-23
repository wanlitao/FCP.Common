using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FCP.Util
{
    /// <summary>
    /// 表达式 核心
    /// </summary>
    public static class ExpressionUtil
    {
        /// <summary>
        /// 获取属性表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyInfo">属性信息</param>
        /// <returns></returns>
        public static Expression<Func<TEntity, object>> getPropertyExpression<TEntity>(PropertyInfo propertyInfo) where TEntity : class
        {
            if (propertyInfo == null)
                return null;

            var parameterExpr = Expression.Parameter(typeof(TEntity), "entity");

            var propertyExpr = Expression.Property(parameterExpr, propertyInfo);
            var conversionExpr = Expression.Convert(propertyExpr, typeof(object));

            var lambdaExpr = Expression.Lambda<Func<TEntity, object>>(conversionExpr, parameterExpr);
            return lambdaExpr;
        }

        /// <summary>
        /// 获取属性值Predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyInfo">属性信息</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> getPropertyPredicate<TEntity>(PropertyInfo propertyInfo, object propertyValue) where TEntity : class
        {
            if (propertyInfo == null)
                return null;

            var parameterExpr = Expression.Parameter(typeof(TEntity), "entity");
            var propertyExpr = Expression.Property(parameterExpr, propertyInfo);
            var propertyValueExpr = Expression.Constant(propertyValue, propertyInfo.PropertyType);
            var propertyEqualExpr = Expression.Equal(propertyExpr, propertyValueExpr);

            var lambdaExpr = Expression.Lambda<Func<TEntity, bool>>(propertyEqualExpr, parameterExpr);
            return lambdaExpr;
        }
    }
}
