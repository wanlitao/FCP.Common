using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace FCP.Util
{
    /// <summary>
    /// 加密 核心
    /// </summary>
    public static class EncryptUtil
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="symAlgorithm">对称加密算法</param>        
        /// <returns></returns>
        public static string encrypt(string originalStr, SymmetricAlgorithm symAlgorithm)
        {
            if (originalStr.isNullOrEmpty() || symAlgorithm == null)
                return string.Empty;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms,
                        symAlgorithm.CreateEncryptor(symAlgorithm.Key, symAlgorithm.IV), CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs) { AutoFlush = true })
                        {
                            sw.Write(originalStr);
                        }                        

                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
                
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.FormatLogMessage());
                return originalStr;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptStr">加密字符串</param>
        /// <param name="symAlgorithm">对称加密算法</param>        
        /// <returns></returns>
        public static string decrypt(string encryptStr, SymmetricAlgorithm symAlgorithm)
        {
            if (encryptStr.isNullOrEmpty() || symAlgorithm == null)
                return string.Empty;

            try
            {
                //从字符串转换为字节组 
                byte[] buffer = Convert.FromBase64String(encryptStr);
                
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cs = new CryptoStream(ms,
                        symAlgorithm.CreateDecryptor(symAlgorithm.Key, symAlgorithm.IV), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.FormatLogMessage());
                return encryptStr;
            }
        }
    }
}
