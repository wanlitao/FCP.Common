using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FCP.Util
{
    /// <summary>
    /// Json序列化助手
    /// </summary>
    public static class JsonSerializeHelper
    {
        #region Json序列化
        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string serializeObject(object value)
        {
            return serializeObject(value, true);
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isIgnoreNull">是否忽略null</param>
        /// <returns></returns>
        public static string serializeObject(object value, bool isIgnoreNull)
        {
            return serializeObject(value, isIgnoreNull, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dateTimeFormat">时间格式化字符串</param>
        /// <returns></returns>
        public static string serializeObject(object value, string dateTimeFormat)
        {
            return serializeObject(value, true, dateTimeFormat);
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isIgnoreNull">是否忽略null</param>
        /// <param name="dateTimeFormat">时间格式化字符串</param>
        /// <returns></returns>
        public static string serializeObject(object value, bool isIgnoreNull, string dateTimeFormat)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (isIgnoreNull)
            {
                settings.NullValueHandling = NullValueHandling.Ignore;
            }
            settings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = dateTimeFormat });

            return JsonConvert.SerializeObject(value, settings);
        }
        #endregion

        #region Json反序列化
        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object deserializeObject(string value)
        {
            if (value.isNullOrEmpty())
                return null;

            return JsonConvert.DeserializeObject(value);
        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T deserializeObject<T>(string value)
        {
            if (value.isNullOrEmpty())
                return default(T);

            return JsonConvert.DeserializeObject<T>(value);
        }
        #endregion
    }
}
