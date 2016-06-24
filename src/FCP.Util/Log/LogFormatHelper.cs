using System;

namespace FCP.Util
{
    /// <summary>
    /// 记录异常
    /// </summary>
    public static class LogFormatHelper
    {
        /// <summary>
        /// 格式化异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns></returns>
        public static string FormatLogMessage(this Exception ex)
        {
            if (ex != null)
            {
                return string.Format("{0}{1}{2}{3}", ex.Message, Environment.NewLine, ex.StackTrace, Environment.NewLine);
            }
            return string.Empty;
        }
    }
}
