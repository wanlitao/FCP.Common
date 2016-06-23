using System;
using System.Security.Cryptography;
using System.Text;

namespace FCP.Util
{
    /// <summary>
    /// 类型常用
    /// </summary>
    public class TypeHelper
    {
        #region String
        /// <summary>
        /// 转换字符串
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <returns></returns>
        public static string parseString(object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }
        #endregion
        #region Int
        /// <summary>
        /// 是否为整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool isInt(string value)
        {
            int temp;
            return isInt(value, out temp);
        }
        /// <summary>
        /// 是否为整数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool isInt(string value, out int val)
        {
            if (string.IsNullOrEmpty(value))
            {
                val = 0;
                return false;
            }
            if (int.TryParse(value, out val))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 转换对象为Int型，默认返回0
        /// </summary>
        /// <param name="source">原始对象</param>
        /// <returns></returns>
        public static int parseInt(object source)
        {
            return parseInt(source, 0);
        }
        /// <summary>
        /// 转换对象为Int型，需输入默认值
        /// </summary>
        /// <param name="obj">原始对象</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static int parseInt(object source, int DefaultValue)
        {
            string ValueString = parseString(source);
            if (string.IsNullOrEmpty(ValueString))
            {
                return DefaultValue;
            }
            else
            {
                int Value;
                if (isInt(ValueString, out Value))
                {
                    return Value;
                }
                else
                {
                    return DefaultValue;
                }
            }
        }
        #endregion
        #region Long
        /// <summary>
        /// 是否为长整型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool isLong(string value)
        {
            long temp;
            return isLong(value, out temp);
        }
        /// <summary>
        /// 是否为长整型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool isLong(string value, out long val)
        {
            if (string.IsNullOrEmpty(value))
            {
                val = 0;
                return false;
            }
            if (long.TryParse(value, out val))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 转换对象为Long型，默认返回0
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static long parseLong(object source)
        {
            return parseLong(source, 0);
        }
        /// <summary>
        /// 转换对象为Long型，需输入默认值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static long parseLong(object source, long DefaultValue)
        {
            string ValueString = parseString(source);
            if (string.IsNullOrEmpty(ValueString))
            {
                return DefaultValue;
            }
            else
            {
                long Value;
                if (isLong(ValueString, out Value))
                {
                    return Value;
                }
                else
                {
                    return DefaultValue;
                }
            }
        }
        #endregion
        #region Decimal
        /// <summary>
        /// 是否为decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool isDecimal(string value)
        {
            decimal temp;
            return isDecimal(value, out temp);
        }
        /// <summary>
        /// 是否为decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool isDecimal(string value, out decimal val)
        {
            if (string.IsNullOrEmpty(value))
            {
                val = 0;
                return false;
            }
            if (decimal.TryParse(value, out val))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 转换对象为Decimal型，默认返回0
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal parseDecimal(object source)
        {
            return parseDecimal(source, 0);
        }
        /// <summary>
        /// 转换对象为Decimal型，需输入默认值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static decimal parseDecimal(object source, decimal DefaultValue)
        {
            string ValueString = parseString(source);
            if (string.IsNullOrEmpty(ValueString))
            {
                return DefaultValue;
            }
            else
            {
                decimal Value;
                if (isDecimal(ValueString, out Value))
                {
                    return Value;
                }
                else
                {
                    return DefaultValue;
                }
            }
        }
        #endregion
        #region Uint
        /// <summary>
        /// 是否为正整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool isUint(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            uint temp;
            if (uint.TryParse(value, out temp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region 日期时间
        /// <summary>
        /// 验证日期
        /// </summary>
        /// <param name="source">原始对象</param>
        /// <returns></returns>
        /// <remarks>
        /// 可判断格式如下（其中-可替换为/，不影响验证)
        /// YYYY | YYYY-MM | YYYY-MM-DD | YYYY-MM-DD HH:MM:SS | YYYY-MM-DD HH:MM:SS.FFF
        /// </remarks>
        public static bool isDateTime(object source)
        {
            string ValueString = parseString(source);
            if (string.IsNullOrEmpty(ValueString))
            {
                return false;
            }
            DateTime Temp;
            if (DateTime.TryParse(ValueString, out Temp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 转换对象为日期时间型，默认值Now
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime parseDateTime(object source)
        {
            return parseDateTime(source, DateTime.Now);
        }
        /// <summary>
        /// 转换对象为日期时间型，需要输入默认值
        /// </summary>
        /// <param name="source">原始对象</param>
        /// <param name="DefalutValue">默认值</param>
        /// <returns></returns>
        public static DateTime parseDateTime(object source, DateTime DefalutValue)
        {
            string ValueString = parseString(source);
            if (string.IsNullOrEmpty(ValueString))
            {
                return DefalutValue;
            }
            else
            {
                DateTime Value;
                if (DateTime.TryParse(ValueString, out Value))
                {
                    //1/1/1753 12:00:00 AM 和 12/31/9999 11:59:59 PM 之间。
                    if (Value >= DateTime.Parse("1753-1-2 00:00:00") && Value <= DateTime.Parse("9999-12-31 11:59:59"))
                    {
                        return Value;
                    }
                    else
                    {
                        return DefalutValue;
                    }
                }
                else
                {
                    return DefalutValue;
                }
            }
        }
        /// <summary>
        /// 格式化日期 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string formatDateTime(DateTime val)
        {
            return val.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 获取下一个周几 是几号
        /// </summary>
        /// <param name="DTe"></param>
        /// <param name="Week"></param>
        /// <returns></returns>
        public static DateTime getNextWeekDay(DateTime DTe, int Week)
        {
            //开始周几
            int sW = Convert.ToInt32(DTe.DayOfWeek);
            sW = sW == 0 ? 7 : sW;
            int Day = 0;
            if (sW > Week)
            {
                Day = (7 - sW) + Week;
            }
            else if (sW < Week)
            {
                Day = Week - sW;
            }
            else
            {
                Day = 7;
            }
            return DTe.AddDays(Day);
        }
        /// <summary>
        /// 检查日期的有效性（那些年月日分开的日期， 有时后面的Day会超出当月最大值）
        /// </summary>
        /// <param name="birthYear"></param>
        /// <param name="birthMonth"></param>
        /// <param name="birthday"></param>
        public static DateTime checkDateTime(int year, int month, int day)
        {
            if (month <= 0 && month > 12) month = 1;
            if (day <= 0) day = 1;
            if (year < 1900) year = 1900;

            if (year > 9999) year = 9999;

            if (month > 0 && month < 13)//天数检查
            {
                int temp = DateTime.DaysInMonth(year <= 0 || year > 9999 ? 2000 : year, month);
                if (day > temp)
                {
                    day = (short)temp;
                }
            }
            return new DateTime(year, month, day);
        }
        #endregion
        #region MD5加密
        /// <summary>
        /// MD5加密 字符串
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string MD5(string Str)
        {
            return MD5(Str, "utf-8");
        }
        /// <summary>
        /// MD5加密 字符串
        /// </summary>
        /// <param name="Str"></param>
        /// <param name="encodeName">编码格式</param>
        /// <returns></returns>
        public static string MD5(string Str, string encodeName)
        {
            if (Str.isNullOrEmpty()) return string.Empty;

            return MD5(Encoding.GetEncoding(encodeName).GetBytes(Str));
        }

        /// <summary>
        /// MD5加密 二进制流
        /// </summary>
        /// <param name="b">二进制流</param>
        /// <returns></returns>
        public static string MD5(byte[] b)
        {
            if (b == null)
            {
                return string.Empty;
            }
            byte[] md5 = new MD5CryptoServiceProvider().ComputeHash(b);
            string result = System.BitConverter.ToString(md5);
            result = result.Replace("-", "");
            return result;
        }
        #endregion
        #region Enumerable
        /// <summary>
        /// 判断集合是否为空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>true 为空, false 为不空</returns>
        //internal static bool IsNull<T>(IEnumerable<T> obj)
        //{
        //    if (obj != null)
        //    {
        //        foreach (var item in obj)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        #endregion
        
        
        #region BooleanToInt返回
        /// <summary>
        /// 将bool型转为int
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int boolToInt(bool val)
        {
            return val ? 1 : 0;
        }
        /// <summary>
        /// 将int型转为bool
        /// </summary>
        /// <param name="source">数字</param>
        /// <returns></returns>
        public static bool intToBool(int val)
        {
            return val == 1;
        }
        #endregion


    }
}
