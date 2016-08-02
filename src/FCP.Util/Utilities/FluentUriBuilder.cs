using System;
using System.Collections.Specialized;
using System.Linq;

namespace FCP.Util
{
    public class FluentUriBuilder
    {
        private const string queryParamSplit = "&";
        private const string queryParamOperator = "=";

        private string _path;
        private NameValueCollection _paramCollection;

        public FluentUriBuilder()
        {
            _path = string.Empty;
            _paramCollection = new NameValueCollection();
        }

        public FluentUriBuilder FromUri(Uri uri)
        {
            if (uri != null)
            {
                _path = uri.AbsolutePath;                
                ParseQueryParams(uri.Query);
            }

            return this;
        }

        public FluentUriBuilder Query(string queryString)
        {            
            ParseQueryParams(queryString);
            
            return this;
        }

        public FluentUriBuilder Path(string path)
        {
            if (!path.isNullOrEmpty())
            {
                _path = path;
            }

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

        public Uri Build()
        {
            var uriBuilder = new UriBuilder { Path = _path };

            uriBuilder.Query = string.Join(queryParamSplit, _paramCollection.AllKeys.SelectMany(
                key => _paramCollection.GetValues(key).Select(value => FormatQueryParam(key, value))));

            return new Uri(uriBuilder.Uri.PathAndQuery, UriKind.RelativeOrAbsolute);
        }

        #region 辅助方法
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

        private string FormatQueryParam(string paramKey, string paramValue)
        {
            if (paramKey.isNullOrEmpty())
                return string.Empty;

            if (paramValue.isNullOrEmpty())
                return Uri.EscapeDataString(paramKey);

            return string.Format("{0}{1}{2}", Uri.EscapeDataString(paramKey), queryParamOperator, Uri.EscapeDataString(paramValue));
        }
        #endregion

    }
}
