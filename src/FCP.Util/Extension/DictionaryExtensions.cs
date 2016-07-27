using System.Collections.Generic;

namespace FCP.Util
{
    /// <summary>
    /// 字典 扩展方法
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 批量移除指定键的元素
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ChainRemove<TKey, TValue>(this IDictionary<TKey, TValue> dict, params TKey[] keys)
        {
            if (dict != null)
            {
                if (keys.isNotEmpty())
                {
                    foreach(var key in keys)
                    {
                        dict.Remove(key);
                    }
                }
            }

            return dict;
        }
    }
}
