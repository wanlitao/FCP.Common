using System;
using System.Linq;
using System.Collections.Concurrent;
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

        private static readonly ConcurrentDictionary<Type, IDictionary<string, PropertyInfo>> _cachedProperties = new ConcurrentDictionary<Type, IDictionary<string, PropertyInfo>>();

        #region 获取属性
        public static PropertyInfo[] getProperties(Type type)
        {
            var propertyDict = getPropertyDict(type);

            return propertyDict.Select(m => m.Value).ToArray();
        }

        public static IDictionary<string, PropertyInfo> getPropertyDict(Type type)
        {
            var propertyDict = _cachedProperties.GetOrAdd(type, buildPropertyDictionary);

            return propertyDict;
        }

        private static IDictionary<string, PropertyInfo> buildPropertyDictionary(Type type)
        {
            var result = new Dictionary<string, PropertyInfo>();

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                result.Add(property.Name, property);
            }
            return result;
        }
        #endregion

        #region 属性表达式
        /// <summary>
        /// 从Lambda表达式中获取PropertyInfo
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public static PropertyInfo parseProperty(LambdaExpression lambda)
        {
            Expression expr = lambda;
            for (;;)
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
                        return memberExpression.Member as PropertyInfo;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// 解析属性名
        /// </summary>
        /// <param name="propertyExpr"></param>
        /// <returns></returns>
        public static string parsePropertyName(LambdaExpression propertyExpr)
        {
            var propertyInfo = parseProperty(propertyExpr);

            return propertyInfo?.Name;
        }

        /// <summary>
        /// 解析属性值
        /// </summary>
        /// <param name="item"></param>
        /// <param name="propertyExpr">属性表达式</param>
        /// <returns></returns>
        public static object parsePropertyValue(object item, LambdaExpression propertyExpr)
        {
            var propertyInfo = parseProperty(propertyExpr);

            return getPropertyValue(item, propertyInfo);
        }
        #endregion

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
        #endregion

        #region Type
        public static bool isNullable(Type type)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;

            return false;
        }

        public static Type getPropertyType(PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            if (isNullable(propertyType))
                return propertyType.GetGenericArguments()[0];

            return propertyType;
        }

        public static bool isSimpleType(Type type)
        {
            Type actualType = type;
            if (isNullable(type))
            {
                actualType = type.GetGenericArguments()[0];
            }

            return _simpleTypes.Contains(actualType);
        }
        #endregion
    }
}
