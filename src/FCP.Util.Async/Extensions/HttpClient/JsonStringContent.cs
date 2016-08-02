using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FCP.Util.Async
{
    public class JsonStringContent : StringContent
    {
        private const string defaultMediaType = "application/json";

        public JsonStringContent(string content)
            : this(content, null)
        { }

        public JsonStringContent(string content, Encoding encoding)
            : base(content, encoding, defaultMediaType)            
        { }
    }

    public class JsonStringContent<T> : JsonStringContent
    {
        public JsonStringContent(T value)
            : this(value, null)
        { }

        public JsonStringContent(T value, Encoding encoding)
            : base(GetSerializeJson(value), encoding)
        { }

        protected static string GetSerializeJson(object value)
        {
            if (value == null)
                return string.Empty;

            return JsonConvert.SerializeObject(value);
        }
    }
}
