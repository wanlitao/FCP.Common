using System;

namespace FCP.Util
{
    /// <summary>
    /// 日期助手
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// 计算相差天数
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">终止日期</param>
        /// <param name="ignoreTime">是否忽略时间部分</param>
        /// <returns></returns>
        public static int daysBetween(DateTime startDate, DateTime endDate, bool ignoreTime = false)
        {
            if (ignoreTime)
            {
                startDate = startDate.Date;
                endDate = endDate.Date;
            }
            if (startDate >= endDate)
            {
                return 0;
            }
            TimeSpan ts = endDate - startDate;
            return ts.Days;
        }

        /// <summary>
        /// 时间 对应的时辰细分(凌晨、早上、上午、中午、下午、傍晚、晚上)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string dayTimeSection(this DateTime dt)
        {
            int hour = dt.Hour;
            string timeSection = string.Empty;
            if (hour >= 0 && hour < 6)
            {
                timeSection = "凌晨";
            }
            else if (hour >= 6 && hour < 9)
            {
                timeSection = "早上";
            }
            else if (hour >= 9 && hour < 11)
            {
                timeSection = "上午";
            }
            else if (hour >= 11 && hour < 13)
            {
                timeSection = "中午";
            }
            else if (hour >= 13 && hour < 17)
            {
                timeSection = "下午";
            }
            else if (hour >= 17 && hour < 19)
            {
                timeSection = "傍晚";
            }
            else if (hour >= 19)
            {
                timeSection = "晚上";
            }
            return timeSection;
        }

        #region Javascript日期转换
        /// <summary>
        /// javascript初始日期Ticks
        /// </summary>
        private static readonly long initialJavaScriptDateTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        /// <summary>
        /// 从javascript日期Ticks获取时间对象
        /// </summary>
        /// <param name="javascriptTicks">javascript日期Ticks</param>
        /// <param name="isToLocal">是否转换为本地时间</param>
        /// <returns></returns>
        public static DateTime getDateTimeFromJavascriptTicks(long javascriptTicks, bool isToLocal = false)
        {
            DateTime result = new DateTime(javascriptTicks * 10000L + initialJavaScriptDateTicks, DateTimeKind.Utc);
            if (isToLocal)
            {
                result = result.ToLocalTime();  //转换为本地时间
            }            
            return result;
        }
        #endregion        
    }
}
