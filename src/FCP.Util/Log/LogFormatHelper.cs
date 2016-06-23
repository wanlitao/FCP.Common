using System;
using System.IO;

namespace FCP.Util
{
    /// <summary>
    /// 记录异常
    /// </summary>
    public static class LogFormatHelper
    {
        private static string LogPrefix = "Log";

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

        /// <summary>
        /// 文本文件日志记录信息函数
        /// </summary>
        /// <param name="message">具体的信息</param>
        public static void WriteTxtLog(string message)
        {
            try
            {
                lock (LogPrefix)
                {
                    if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\LOG\\"))
                        Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\LOG\\");
                    using (StreamWriter sw = File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory + "\\LOG\\" + LogPrefix + DateTime.Now.ToString("yyyyMMdd") + ".log"))
                    {
                        sw.WriteLine(message);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
