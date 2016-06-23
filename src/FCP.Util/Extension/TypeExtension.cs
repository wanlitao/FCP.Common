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
    }
}
