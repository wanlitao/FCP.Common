using System.Text.RegularExpressions;

namespace FCP.Util
{
    /// <summary>
    /// 常用正则
    /// </summary>
    public abstract class RegexPatterns
    {
        /// <summary>
        /// 数字
        /// </summary>
        public const string numeric = @"^\-?[0-9]*\.?[0-9]*$";
        #region 整数
        /// <summary>
        /// 整数
        /// </summary>
        public const string integer = @"^[+-]?[0-9]+$";
        /// <summary>
        /// 正整数 大于0的整数
        /// </summary>
        public const string integerPositive = @"^[1-9]\d*$";
        /// <summary>
        /// 非正整数 小于等于0
        /// </summary>
        public const string integerNoPositive = @"^-[1-9]\d*$";
        /// <summary>
        /// 负整数 小于0
        /// </summary>
        public const string integerNegative = @"^-[1-9]\d*$";
        /// <summary>
        /// 非负整数 大于等于0
        /// </summary>
        public const string integerNoNegative = @"^[1-9]\d*|0$";
        #endregion
        #region 浮点数
        /// <summary>
        /// 浮点数
        /// </summary>
        public const string floating = @"^-?([1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|0)$";
        /// <summary>
        /// 正浮点数
        /// </summary>
        public const string floatPositive = @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$";
        /// <summary>
        /// 非正浮点数 小于等于0
        /// </summary>
        public const string floatNoPositive = @"^(-([1-9]\d*\.\d*|0\.\d*[1-9]\d*))|0?\.0+|0$";
        /// <summary>
        /// 负浮点数 小于0
        /// </summary>
        public const string floatNegative = @"^(-([1-9]\d*\.\d*|0\.\d*[1-9]\d*))|0?\.0+|0$";
        /// <summary>
        /// 非负浮点数 大于等于0
        /// </summary>
        public const string floatNoNegative = @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|0$";
        /// <summary>
        /// 正数 大于0的整数或浮点数
        /// </summary>
        public const string numberNoNegative = @"^([1-9]\d*\.?\d*)|(0\.\d*[1-9]+)$";
        ///add by wanlitao 2012.9.29
        /// <summary>
        /// 非负数 大于等于0的整数或浮点数
        /// </summary>
        public const string numberNoNegativeWithZero = @"^([1-9]\d*\.?\d*)|^(0\.\d*[1-9]+)|0?\.0+|0$";
        #endregion
        #region 字符串
        /// <summary>
        /// 中文
        /// </summary>
        public const string zhCn = @"[\u4e00-\u9fa5]";
        /// <summary>
        /// 字母
        /// </summary>
        public const string alpha = @"^[a-zA-Z]*$";
        /// <summary>
        /// 大写字母
        /// </summary>
        public const string alphaUpperCase = @"^[A-Z]*$";
        /// <summary>
        /// 小写字母
        /// </summary>
        public const string alphaLowerCase = @"^[a-z]*$";
        /// <summary>
        /// 字母+整数数字
        /// </summary>
        public const string alphaNumeric = @"^[a-zA-Z0-9]*$";
        /// <summary>
        /// 字母+整数数字+空格
        /// </summary>
        public const string alphaNumericSpace = @"^[a-zA-Z0-9 ]*$";
        /// <summary>
        /// 字母+整数数字+空格+（-）
        /// </summary>
        public const string alphaNumericSpaceDash = @"^[a-zA-Z0-9 \-]*$";
        /// <summary>
        /// 字母+整数数字+空格+（-）+(_)
        /// </summary>
        public const string alphaNumericSpaceDashUnderscore = @"^[a-zA-Z0-9 \-_]*$";
        /// <summary>
        /// 字母+整数数字+空格+（-）+(_)+（.）
        /// </summary>
        public const string alphaNumericSpaceDashUnderscorePeriod = @"^[a-zA-Z0-9\. \-_]*$";
        /// <summary>
        /// 0-100的整数
        /// </summary>
        public const string numHundred = @"^(?:0|[1-9][0-9]?|100)$";
        #endregion
        #region 日期
        /// <summary>
        /// 月份
        /// </summary>
        public const string month = @"^([1-9]|1[0-2])$";

        /// <summary>
        /// 日
        /// </summary>
        public const string day = @"^([1-2][0-9])|(3[0-1])|[1-9]$";

        /// <summary>
        /// 时
        /// </summary>
        public const string hour = @"^([0-9]|1[0-9]|2[0-3])$";

        /// <summary>
        /// 分
        /// </summary>
        public const string minute = @"^([0-9]|[1-5][0-9])$";

        /// <summary>
        /// 日期YYYY-MM-DD
        /// </summary>
        public const string date = @"^(([0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3})-(((0[13578]|1[02])-(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)-(0[1-9]|[12][0-9]|30))|(02-(0[1-9]|[1][0-9]|2[0-8]))))|((([0-9]{2})(0[48]|[2468][048]|[13579][26])|((0[48]|[2468][048]|[3579][26])00))-02-29)$";
        #endregion
        #region 常用正则
        /// <summary>
        /// 身份证
        /// </summary>
        //public const string identityCard = @"(^\d{15}$)|(^\d{18}$)|(\d{17}(?:\d|x|X)$)";
        public const string identityCard = @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$";
        /// <summary>
        /// Email
        /// </summary>
        public const string email = @"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$";
        /// <summary>
        /// Url
        /// </summary>
        public const string url = @"^^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_=]*)?$";
        /// <summary>
        /// 邮编
        /// </summary>
        public const string zipCode = @"^[1-9]\d{5}$ ";
        /// <summary>
        /// 手机号码
        /// </summary>
        public const string mobilePhone = @"^[1][34578][0-9]{9}$";
        /// <summary>
        /// 电话号码
        /// </summary>
        public const string telPhone = @"^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$";
        /// <summary>
        /// IP地址
        /// </summary>
        public const string ip = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";
        #endregion
       /// <summary>
       /// 正则表达式验证 匹配返回true
       /// </summary>
       /// <param name="Value">搜索项</param>
       /// <param name="Pattern">匹配项</param> 
       /// <returns></returns> 
        public static bool isMatch(string value, string pattern)
        {
            RegexOptions Options = RegexOptions.IgnoreCase;
            Regex Reg = new Regex(pattern, Options);
            return Reg.IsMatch(value);
        }
    }
}
