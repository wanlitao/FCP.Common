using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace FCP.Util
{
    /// <summary>
    /// 集合 扩展方法
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 序列（或集合）不包含元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool isEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return true;
            }
            return !source.Any();
        }

        /// <summary>
        /// 序列（或集合）包含元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool isNotEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return false;
            }
            return source.Any();
        }

        /// <summary>
        /// NameValueCollection转换为字典
        /// </summary>
        /// <param name="paramCollection"></param>
        /// <returns></returns>
        public static IDictionary<string, string> toDictionary(this NameValueCollection paramCollection)
        {
            if (paramCollection == null || paramCollection.Count == 0)
                return null;

            return paramCollection.AllKeys.ToDictionary(m => m, m => paramCollection[m]);
        }
    }
}