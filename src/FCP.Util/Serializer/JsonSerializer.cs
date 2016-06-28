using Newtonsoft.Json;
using System;
using System.Text;

namespace FCP.Util
{
    /// <summary>
    /// Newtonsoft.Json Serializer
    /// </summary>
    internal class JsonSerializer : BaseSerializer, ISerializer
    {
        public JsonSerializer()
        {
            SerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        public JsonSerializer(JsonSerializerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            SerializerSettings = settings;
        }

        public JsonSerializerSettings SerializerSettings { get; private set; }

        #region Serialize
        protected override byte[] SerializeInternal<TValue>(TValue value)
        {
            var dataStr = SerializeStringInternal(value);

            return Encoding.UTF8.GetBytes(dataStr);
        }

        protected override string SerializeStringInternal<TValue>(TValue value)
        {
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }
        #endregion

        #region Deserialize
        protected override TValue DeserializeInternal<TValue>(byte[] data)
        {
            var dataStr = Encoding.UTF8.GetString(data);

            return DeserializeStringInternal<TValue>(dataStr);
        }

        protected override TValue DeserializeStringInternal<TValue>(string dataStr)
        {
            return JsonConvert.DeserializeObject<TValue>(dataStr, SerializerSettings);
        }
        #endregion
    }
}
