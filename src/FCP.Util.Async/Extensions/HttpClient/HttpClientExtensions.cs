using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FCP.Util.Async
{
    public static class HttpClientExtensions
    {
        #region Post Extensions
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
        {
            return client.PostAsJsonAsync(requestUri, value, null);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T value, Encoding encoding)
        {
            return client.PostAsJsonAsync(requestUri, value, encoding, CancellationToken.None);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T value,
            Encoding encoding, CancellationToken cancellationToken)
        {
            var content = new JsonStringContent<T>(value, encoding);

            return client.PostAsync(requestUri, content, cancellationToken);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, Uri requestUri, T value)
        {
            return client.PostAsJsonAsync(requestUri, value, null);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, Uri requestUri, T value, Encoding encoding)
        {
            return client.PostAsJsonAsync(requestUri, value, encoding, CancellationToken.None);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, Uri requestUri, T value,
            Encoding encoding, CancellationToken cancellationToken)
        {
            var content = new JsonStringContent<T>(value, encoding);

            return client.PostAsync(requestUri, content, cancellationToken);
        }
        #endregion
    }
}
