using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FCP.Entity
{
    /// <summary>
    /// 反射助手
    /// </summary>
    public static class ReflectionHelper
    {
        private static List<Type> _simpleTypes = new List<Type>
                               {
                                   typeof(byte),
                                   typeof(sbyte),
                                   typeof(short),
                                   typeof(ushort),
                                   typeof(int),
                                   typeof(uint),
                                   typeof(long),
                                   typeof(ulong),
                                   typeof(float),
                                   typeof(double),
                                   typeof(decimal),
                                   typeof(bool),
                                   typeof(string),
                                   typeof(char),
                                   typeof(Guid),
                                   typeof(DateTime),
                                   typeof(DateTimeOffset),
                                   typeof(byte[])
                               };

        /// <summary>
        /// 从Lambda表达式中获取MemberInfo
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public static MemberInfo getProperty(LambdaExpression lambda)
        {
            Expression expr = lambda;
            for (; ; )
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        MemberExpression memberExpression = (MemberExpression)expr;
                        MemberInfo mi = memberExpression.Member;
                        return mi;
                    default:
                        return null;
                }
            }
        }

        #region 获取属性值
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object getPropertyValue(object item, PropertyInfo property)
        {
            if (item == null || property == null)
                return null;

            return property.GetValue(item, null);
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="item"></param>
        /// <param name="propertyName">属性名称，支持跨层级获取</param>
        /// <returns></returns>
        public static object getPropertyValue(object item, string propertyName)
        {
            PropertyInfo property;
            foreach (var part in propertyName.Split('.'))
            {
                if (item == null)
                    return null;

                var type = item.GetType();

                property = type.GetProperty(part);
                if (property == null)
                    return null;

                item = getPropertyValue(item, property);
            }
            return item;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="item"></param>
        /// <param name="propertyExpr">属性表达式</param>
        /// <returns></returns>
        public static object getPropertyValue(object item, LambdaExpression propertyExpr)
        {
            var propertyInfo = getProperty(propertyExpr) as PropertyInfo;

            return getPropertyValue(item, propertyInfo);
        }
        #endregion

        public static bool isSimpleType(Type type)
        {
            Type actualType = type;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                actualType = type.GetGenericArguments()[0];
            }

            return _simpleTypes.Contains(actualType);
        }
    }
}
