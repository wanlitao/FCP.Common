using System.IO;
using System.Text;

namespace FCP.Util
{
    /// <summary>
    /// Stream助手
    /// </summary>
    public static class StreamHelper
    {
        #region 转换Stream为二进制字节
        /// <summary>
        /// 转换Stream为二进制字节
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] getBytesFromStream(Stream stream)
        {
            if (stream != null)
            {
                byte[] fileBytes = new byte[stream.Length];
                stream.Read(fileBytes, 0, fileBytes.Length);                
                return fileBytes;
            }
            return null;
        }
        #endregion

        #region 转换Stream为字符串
        /// <summary>
        /// 转换Stream为字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encodeName">编码格式</param>
        /// <returns></returns>
        public static string getStringFromStream(Stream stream)
        {
            return getStringFromStream(stream, "utf-8");
        }

        /// <summary>
        /// 转换Stream为字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encodeName">编码格式</param>
        /// <returns></returns>
        public static string getStringFromStream(Stream stream, string encodeName)
        {
            byte[] streamBytes = getBytesFromStream(stream);
            if (streamBytes.isNotEmpty())
            {
                return Encoding.GetEncoding(encodeName).GetString(streamBytes);
            }
            return string.Empty;
        }
        #endregion
    }
}
