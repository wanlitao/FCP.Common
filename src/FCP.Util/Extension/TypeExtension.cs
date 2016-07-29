using System;

namespace FCP.Util
{
    public static class TypeExtension
    {
        /// <summary>
        /// 是否继承特定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool Is<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否为具体类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsConcrete(this Type type)
        {
            return type.IsClass && !type.IsAbstract;
        }

        /// <summary>
        /// 是否继承特定泛型类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsGeneric(this Type type, Type genericType)
        {
            if (genericType == null)
                throw new ArgumentNullException(nameof(genericType));

            if (!genericType.IsGenericTypeDefinition)
                throw new ArgumentException("generic type must be a GenericTypeDefinition");

            if (type == null)
                return false;

            if (type == typeof(object))
                return false;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                return true;

            return type.BaseType.IsGeneric(genericType);
        }
    }
}
