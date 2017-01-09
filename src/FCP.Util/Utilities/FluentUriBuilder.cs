using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace FCP.Util
{
    public class FluentUriBuilder
    {
        private const string queryParamSplit = "&";
        private const string queryParamOperator = "=";

        private UriBuilder _uriBuilder;
        private IList<string> _appendSegments;
        private NameValueCollection _paramCollection;

        public FluentUriBuilder()
        {
            _uriBuilder = new UriBuilder();
            _appendSegments = new List<string>();
            _paramCollection = new NameValueCollection();
        }

        public FluentUriBuilder FromUri(Uri uri)
        {
            if (uri != null)
            {
                Scheme(uri.Scheme)
                    .Host(uri.Host)
                    .Port(uri.Port)
                    .Path(uri.AbsolutePath);
                                
                ParseQueryParams(uri.Query);
            }

            return this;
        }

        public FluentUriBuilder Scheme(string scheme)
        {
            _uriBuilder.Scheme = scheme;

            return this;
        }

        public FluentUriBuilder Host(string host)
        {
            _uriBuilder.Host = host;

            return this;
        }

        public FluentUriBuilder Port(int port)
        {
            _uriBuilder.Port = port;

            return this;
        }

        public FluentUriBuilder Path(string path)
        {
            _uriBuilder.Path = path;

            return this;
        }

        public FluentUriBuilder AppendSegment(string segment)
        {
            if (!segment.isNullOrEmpty())
            {
                _appendSegments.Add(segment);
            }

            return this;
        }

        public FluentUriBuilder Query(string queryString)
        {            
            ParseQueryParams(queryString);
            
            return this;
        }

        public FluentUriBuilder Param(string paramKey, string paramValue)
        {
            if (!paramKey.isNullOrEmpty())
            {
                _paramCollection.Add(paramKey, paramValue);
            }

            return this;
        }

        public FluentUriBuilder SegmentParam(string paramValue)
        {
            if (!paramValue.isNullOrEmpty())
            {
                _appendSegments.Add(paramValue);
            }

            return this;
        }

        public Uri Build()
        {
            BuildPathAndQuery();

            return new Uri(_uriBuilder.Uri.PathAndQuery, UriKind.RelativeOrAbsolute);
        }

        public Uri BuildAbsolute()
        {
            BuildPathAndQuery();

            return _uriBuilder.Uri;
        }

        #region 辅助方法
        private static string CombineUriPath(params string[] paths)
        {
            if (paths.isEmpty())
                return string.Empty;

            return string.Join("/", paths.Where(m => !m.isNullOrEmpty()).Select(m => m.Trim('/')));
        }

        private void BuildPathAndQuery()
        {
            if (_appendSegments.isNotEmpty())
            {
                var appendPath = CombineUriPath(_appendSegments.ToArray());
                _uriBuilder.Path = CombineUriPath(_uriBuilder.Path, appendPath);
            }

            _uriBuilder.Query = string.Join(queryParamSplit, _paramCollection.AllKeys.SelectMany(
                key => _paramCollection.GetValues(key).Select(value => FormatQueryParam(key, value))));           
        }

        /// <summary>
        /// 解析Query参数
        /// </summary>
        /// <param name="queryString"></param>
        private void ParseQueryParams(string queryString)
        {
            if (queryString.isNullOrEmpty())
                return;

            var queryParams = queryString.Split(new[] { queryParamSplit }, StringSplitOptions.RemoveEmptyEntries);
            if (queryParams.isNotEmpty())
            {
                foreach (var queryParam in queryParams)
                {
                    var queryConditionParts = queryParam.Split(new[] { queryParamOperator }, StringSplitOptions.None);
                    if (queryConditionParts.Length < 2)
                    {
                        _paramCollection.Add(queryConditionParts[0], string.Empty);
                    }
                    else
                    {
                        _paramCollection.Add(queryConditionParts[0], queryConditionParts[1]);
                    }
                }
            }
        }

        private static string FormatQueryParam(string paramKey, string paramValue)
        {
            if (paramKey.isNullOrEmpty())
                return string.Empty;

            if (paramValue.isNullOrEmpty())
                return Uri.EscapeDataString(paramKey);

            return $"{Uri.EscapeDataString(paramKey)}{queryParamOperator}{Uri.EscapeDataString(paramValue)}";
        }
        #endregion
    }
}
