using System.Text;
using System.Security.Cryptography;

namespace FCP.Util
{
    /// <summary>
    /// 加密助手
    /// </summary>
    public static class EncryptHelper
    {
        #region 密钥和初始化向量 缓存
        //随机选8个字节既为密钥也为初始向量 
        private static byte[] KEY_64 = { 33, 16, 93, 156, 78, 4, 218, 32 };
        private static byte[] IV_64 = { 99, 103, 246, 79, 36, 99, 167, 3 };

        //对TripleDES,采取24字节或192位的密钥和初始向量 
        private static byte[] KEY_192 = { 42, 16, 93, 156, 78, 4, 218, 32, 15, 167, 44, 80, 26, 250, 155, 112, 2, 94, 11, 204, 119, 35, 184, 194 };
        private static byte[] IV_192 = { 55, 103, 246, 79, 36, 99, 167, 3, 42, 5, 62, 83, 184, 7, 209, 13, 145, 23, 200, 58, 173, 10, 121, 181 };

        //Rijndael缓存密钥和初始化向量
        private static byte[] KEY_Legal = getLegalKey();
        private static byte[] IV_Legal = getLegalIV();
        #endregion        

        #region 计算 Rijndael算法 密钥和初始化向量
        /// <summary>
        /// 初始化密钥
        /// </summary>
        /// <returns>密钥</returns>
        private static byte[] getLegalKey()
        {
            string sTemp = "(RWPCore)WooXNGX";
            int KeyLength = 16;
            //将密钥调整为16位，多减少加
            if (sTemp.Length > KeyLength)
            {
                sTemp = sTemp.Substring(0, KeyLength);
            }
            else if (sTemp.Length < KeyLength)
            {
                sTemp = sTemp.PadRight(KeyLength, ' ');
            }
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        /// <summary>
        /// 初始化向量
        /// </summary>
        /// <returns>向量</returns>
        private static byte[] getLegalIV()
        {
            string sTemp = "EnShiDeWoHenNiuA";
            int IVLength = 16;
            //将向量调整为16位，多减少加
            if (sTemp.Length > IVLength)
            {
                sTemp = sTemp.Substring(0, IVLength);
            }
            else if (sTemp.Length < IVLength)
            {
                sTemp = sTemp.PadRight(IVLength, ' ');
            }
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        #endregion                

        #region 根据加密类型获取对称加密算法
        /// <summary>
        /// 根据加密类型获取对称加密算法
        /// </summary>
        /// <param name="encryptType">加密类型</param>
        /// <returns></returns>
        private static SymmetricAlgorithm getSymAlgorithmByEncryptType(EncryptType encryptType)
        {
            if (encryptType == EncryptType.None)
                return null;

            SymmetricAlgorithm symAlgorithm = null;
            switch (encryptType)
            {
                case EncryptType.Rijndael:
                    symAlgorithm = new RijndaelManaged() { Key = KEY_Legal, IV = IV_Legal };
                    break;
                case EncryptType.Des:
                    symAlgorithm = new DESCryptoServiceProvider() { Key = KEY_64, IV = IV_64 };
                    break;
                case EncryptType.TripleDES:
                    symAlgorithm = new TripleDESCryptoServiceProvider() { Key = KEY_192, IV = IV_192 };
                    break;
            }
            return symAlgorithm;
        }
        #endregion

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="encryptType">加密类型</param>
        /// <returns></returns>
        public static string encrypt(string originalStr, EncryptType encryptType)
        {
            if (encryptType == EncryptType.None)
                return originalStr;

            using (SymmetricAlgorithm symAlgorithm = getSymAlgorithmByEncryptType(encryptType))
            {
                return EncryptUtil.encrypt(originalStr, symAlgorithm);
            }
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptStr">加密字符串</param>
        /// <param name="encryptType">加密类型</param>
        /// <returns></returns>
        public static string decrypt(string encryptStr, EncryptType encryptType)
        {
            if (encryptType == EncryptType.None)
                return encryptStr;

            using (SymmetricAlgorithm symAlgorithm = getSymAlgorithmByEncryptType(encryptType))
            {
                return EncryptUtil.decrypt(encryptStr, symAlgorithm);
            }                        
        }
    }
}