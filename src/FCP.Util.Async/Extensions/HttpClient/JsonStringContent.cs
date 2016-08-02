using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FCP.Util.Async
{
    public class JsonStringContent : StringContent
    {
        private const string defaultMediaType = "application/json";

        internal JsonStringContent(string content)
            : this(content, null)
        { }

        internal JsonStringContent(string content, Encoding encoding)
            : base(content, encoding, defaultMediaType)            
        { }
    }

    public class JsonStringContent<T> : JsonStringContent
    {
        internal JsonStringContent(T value)
            : this(value, null)
        { }

        internal JsonStringContent(T value, Encoding encoding)
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
