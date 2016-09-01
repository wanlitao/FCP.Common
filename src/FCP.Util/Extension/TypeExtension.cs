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
            return type.Is(typeof(T));
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
        /// 是否继承特定类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool Is(this Type type, Type baseType)
        {
            if (baseType == null)
                throw new ArgumentNullException(nameof(baseType));
           
            if (type == null)
                return false;
            
            if (baseType.IsAssignableFrom(type))
                return true;

            if (type == typeof(object))
                return false;

            if (baseType.IsGenericTypeDefinition &&
                type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                return true;

            var interfaceTypes = type.GetInterfaces();
            foreach(var interfaceType in interfaceTypes)
            {
                if (interfaceType.Is(baseType))
                    return true;
            }

            return type.BaseType.Is(baseType);
        }
    }
}
