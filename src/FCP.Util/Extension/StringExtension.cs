using System;
using System.Text;

namespace FCP.Util
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 判断空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isNullOrEmpty(this string str)
        {
            return (str ?? "").Length == 0;
        }

        #region Base64转换
        /// <summary>
        /// 转换为Base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string toBase64String(this string str)
        {
            return str.toBase64String("utf-8");
        }

        /// <summary>
        /// 转换为Base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string toBase64String(this string str, string encodeName)
        {
            if (str.isNullOrEmpty())
                return string.Empty;            

            var bytes = Encoding.GetEncoding(encodeName).GetBytes(str);

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 解密Base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string fromBase64String(this string str)
        {
            return str.fromBase64String("utf-8");
        }

        /// <summary>
        /// 解密Base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string fromBase64String(this string str, string encodeName)
        {
            if (str.isNullOrEmpty())
                return string.Empty;

            var bytes = Convert.FromBase64String(str);            

            return Encoding.GetEncoding(encodeName).GetString(bytes);
        }
        #endregion
    }
}
