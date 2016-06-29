using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FCP.Util
{
    public static class SerializerFactory
    {
        private static ISerializer _binarySerializer = new BinarySerializer();
        private static ISerializer _defaultJsonSerializer = new JsonSerializer();        

        public static ISerializer BinarySerializer { get { return _binarySerializer; } }

        public static ISerializer JsonSerializer { get { return _defaultJsonSerializer; } }

        public static ISerializer GetJsonSerializer(string dateTimeFormat)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = dateTimeFormat });

            return GetJsonSerializer(settings);
        }

        public static ISerializer GetJsonSerializer(JsonSerializerSettings settings)
        {
            return new JsonSerializer(settings);
        }
    }
}
