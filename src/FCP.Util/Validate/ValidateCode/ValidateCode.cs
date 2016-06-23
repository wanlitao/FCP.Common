using System;

namespace FCP.Util
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class ValidateCode
    {
        #region code集合
        /// <summary>
        /// 纯数字
        /// </summary>
        public const string numberCodeGens = "0123456789";

        /// <summary>
        /// 纯字母
        /// </summary>
        public const string letterCodeGens = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// 数字+字母
        /// </summary>
        public const string numberLetterCodeGens = numberCodeGens + letterCodeGens;
        #endregion

        /// <summary>
        /// 根据code种子集合
        /// </summary>
        /// <param name="codeType">验证码类型</param>
        /// <returns></returns>
        private static string getCodeGensByCodeType(ValidateCodeType codeType)
        {
            string codeGens = string.Empty;
            switch(codeType)
            {
                case ValidateCodeType.number:  //纯数字
                    codeGens = numberCodeGens;
                    break;
                case ValidateCodeType.letter:  //纯字母
                    codeGens = letterCodeGens;
                    break;
                case ValidateCodeType.numberletter:  //数字+字母
                    codeGens = numberLetterCodeGens;
                    break;
            }
            return codeGens;
        }

        /// <summary>
        /// 获取随机验证码
        /// </summary>
        /// <param name="codeType">验证码类型</param>
        /// <param name="codeLength">验证码长度</param>
        /// <returns></returns>
        public static string createRandomCode(ValidateCodeType codeType, int codeLength)
        {            
            if (codeLength < 1)
            {
                return string.Empty;
            }
            string codeGens = getCodeGensByCodeType(codeType);
            if (string.IsNullOrEmpty(codeGens))
            {
                return string.Empty;
            }
            char[] codeGenCharArr = codeGens.ToCharArray();   //code种子 字符数组
            char[] codeCharArr = new char[codeLength];
            Random rnd = new Random();
            for (int i = 0; i < codeLength; i++)
            {
                int rndIndex = rnd.Next(100000) % codeGenCharArr.Length;  //获取随机索引
                codeCharArr[i] = codeGenCharArr[rndIndex];
            }
            return new String(codeCharArr);
        }
    }

    /// <summary>
    /// 验证码类型
    /// </summary>
    public enum ValidateCodeType
    {
        /// <summary>
        /// 数字
        /// </summary>
        number,
        /// <summary>
        /// 字母
        /// </summary>
        letter,
        /// <summary>
        /// 数字+字典
        /// </summary>
        numberletter        
    }
}
