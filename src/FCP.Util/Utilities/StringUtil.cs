using System;
using System.Text;

namespace FCP.Util
{
    /// <summary>
    /// 字符串操作 核心
    /// </summary>
    public abstract class StringUtil
    {
        #region 字符串操作
        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns>字符长度</returns>
        public static int getStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }
        /// <summary>
        /// 截取字符串函数
        /// </summary>
        /// <param name="Str">所要截取的字符串</param>
        /// <param name="Num">截取字符串的长度</param>
        /// <returns></returns>
        public static string getSubString(string str, int num)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            string outstr = string.Empty;
            int n = 0;
            foreach (char ch in str)
            {
                n += System.Text.Encoding.Default.GetByteCount(ch.ToString());
                if (n > num)
                {
                    break;
                }
                else
                {
                    outstr += ch;
                }
            }
            return outstr;
        }
        /// <summary>
        /// 取字符左函数
        /// </summary>
        /// <param name="Object">要操作的 string  数据</param>
        /// <param name="MaxLength">最大长度</param>
        /// <returns>string</returns>
        public static string left(object objectval, int maxLength)
        {
            if (objectval == null) return "";
            return objectval.ToString().Substring(0, Math.Min(objectval.ToString().Length, maxLength));
        }
        /// <summary>
        /// 取字符中间函数
        /// </summary>
        /// <param name="Object">要操作的 string  数据</param>
        /// <param name="StarIndex">开始的位置索引</param>
        /// <param name="MaxLength">最大长度</param>
        /// <returns>string</returns>
        public static string mid(string objectval, int starIndex, int maxLength)
        {
            if (objectval == null) return "";
            if (starIndex >= objectval.Length) return "";
            return objectval.Substring(starIndex, maxLength);
        }
        /// <summary>
        /// 取字符右函数
        /// </summary>
        /// <param name="Object">要操作的 string  数据</param>
        /// <param name="MaxLength">最大长度</param>
        /// <returns>string</returns>
        public static string right(object objectval, int maxLength)
        {
            if (objectval == null) return "";
            int i = objectval.ToString().Length;
            if (i < maxLength) { maxLength = i; i = 0; } else { i = i - maxLength; }
            return objectval.ToString().Substring(i, maxLength);
        }
        /// <summary>
        /// 判断字符串首字符
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="lookfor">首字符CHAR</param>
        /// <returns></returns>
        public static bool startsWith(string text, char lookfor)
        {
            return (text.Length > 0 && text[0] == lookfor);
        }
        /// <summary>
        /// 快速判断字符串起始部分
        /// </summary>
        /// <param name="target">字符串</param>
        /// <param name="lookfor">起始字符串</param>
        /// <returns></returns>
        public static bool startsWith(string target, string lookfor)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(lookfor))
            {
                return false;
            }

            if (lookfor.Length > target.Length)
            {
                return false;
            }
            return (0 == string.Compare(target, 0, lookfor, 0, lookfor.Length, StringComparison.Ordinal));
        }
        /// <summary>
        /// 忽略大小写快速判断字符串起始部分
        /// </summary>
        /// <param name="target">字符串</param>
        /// <param name="lookfor">起始字符串</param>
        /// <returns></returns>
        public static bool startsWithIgnoreCase(string target, string lookfor)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(lookfor))
            {
                return false;
            }

            if (lookfor.Length > target.Length)
            {
                return false;
            }
            return (0 == string.Compare(target, 0, lookfor, 0, lookfor.Length, StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// 快速判断字符串结束字符
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="lookfor">结束字符Char</param>
        /// <returns></returns>
        public static bool endsWith(string text, char lookfor)
        {
            return (text.Length > 0 && text[text.Length - 1] == lookfor);
        }
        /// <summary>
        /// 快速判断字符串结束部分
        /// </summary>
        /// <param name="target">字符串</param>
        /// <param name="lookfor">字符串结束部分</param>
        /// <returns></returns>
        public static bool endsWith(string target, string lookfor)
        {
            int indexA = target.Length - lookfor.Length;

            if (indexA < 0)
            {
                return false;
            }
            return (0 == string.Compare(target, indexA, lookfor, 0, lookfor.Length, StringComparison.Ordinal));
        }
        /// <summary>
        /// 忽略大小写快速判断字符串结束部分
        /// </summary>
        /// <param name="target">字符串</param>
        /// <param name="lookfor">字符串结束部分</param>
        /// <returns></returns>
        public static bool endsWithIgnoreCase(string target, string lookfor)
        {
            int indexA = target.Length - lookfor.Length;

            if (indexA < 0)
            {
                return false;
            }
            return (0 == string.Compare(target, indexA, lookfor, 0, lookfor.Length, StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// 忽略大小写判断字符串是否相等
        /// </summary>
        /// <param name="target">原始字符串</param>
        /// <param name="looktarget">对比字符串</param>
        /// <returns></returns>
        public static bool compareIgnoreCase(string target, string looktarget)
        {
            return (0 == string.Compare(target, looktarget, true));
        }

        /// <summary>
        /// 获得某个特定字符在指定字符串中出现的次数
        /// </summary>
        /// <param name="argString">指定字符串</param>
        /// <param name="argChar">特定字符</param>
        /// <returns>出现的次数</returns>
        public static int getDisplayCount(string argString, char argChar)
        {
            if (argString == null || argString.Length == 0)
                return 0;

            int count = 0;
            int currentPos = 0;
            while (true)
            {
                currentPos = argString.IndexOf(argChar, currentPos) + 1;
                if (currentPos == 0)
                    break;
                else
                    count = count + 1;
            }
            return count;
        }

        /// <summary>
        /// 获得某个特定字符串在指定字符串中出现的次数
        /// </summary>
        /// <param name="argString">指定字符串</param>
        /// <param name="argDestString">特定字符串</param>
        /// <returns>出现次数</returns>
        public static int getDisplayCount(string argString, string argDestString)
        {
            if (argString == null || argString.Length == 0 ||
                argDestString == null || argDestString.Length == 0)
                return 0;
            int count = 0;
            int currentPos = 0;
            while (true)
            {
                currentPos = argString.IndexOf(argDestString, currentPos) + argDestString.Length;
                if (currentPos == argDestString.Length - 1)
                    break;
                else
                    count = count + 1;
            }
            return count;
        }
        #endregion
        #region 转换字符串中的非英文字符编码
        /// <summary>
        /// 转换字符串中的非英文字符编码
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string parseEnString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            char[] chars = str.ToCharArray();
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < chars.Length; index++)
            {
                if (needToEncode(chars[index]))
                {
                    string encodedString = toHexString(chars[index]);
                    builder.Append(encodedString);
                }
                else
                {
                    builder.Append(chars[index]);
                }
            }
            return builder.ToString();
        }
        /// <summary>
        ///指定 一个字符是否应该被编码
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        private static bool needToEncode(char chr)
        {
            string reservedChars = "$-_.+!*'(),@=&";

            if (chr > 127)
                return true;
            if (char.IsLetterOrDigit(chr) || reservedChars.IndexOf(chr) >= 0)
                return false;

            return true;
        }
        /// <summary>
        /// 为非英文字符串编码
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        private static string toHexString(char chr)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(chr.ToString());
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < encodedBytes.Length; index++)
            {
                builder.AppendFormat("%{0}", Convert.ToString(encodedBytes[index], 16));
            }
            return builder.ToString();
        }
        #endregion

        #region html标签过滤
        /// <summary>
        /// 过滤html标签代码
        /// </summary>
        /// <param name="html"> 要过滤的内容</param>
        /// <returns>纯文本内容</returns>
        public static string checkStr(string html)
        {
            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S]+</script *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script *:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex3 = new System.Text.RegularExpressions.Regex(@" no[\s\S]*=", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+</iframe *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+</frameset *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex6 = new System.Text.RegularExpressions.Regex(@"\<img[^\>]+\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex7 = new System.Text.RegularExpressions.Regex(@"</p>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex8 = new System.Text.RegularExpressions.Regex(@"<p>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex9 = new System.Text.RegularExpressions.Regex(@"<[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = regex1.Replace(html, ""); //过滤<script></script>标记 
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性 
            html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件 
            html = regex4.Replace(html, ""); //过滤iframe 
            html = regex5.Replace(html, ""); //过滤frameset 
            html = regex6.Replace(html, ""); //过滤frameset 
            html = regex7.Replace(html, ""); //过滤frameset 
            html = regex8.Replace(html, ""); //过滤frameset 
            html = regex9.Replace(html, "");
            html = html.Replace(" ", "");
            html = html.Replace("</strong>", "");
            html = html.Replace("<strong>", "");
            return html;
        }
        #endregion
    }
}

