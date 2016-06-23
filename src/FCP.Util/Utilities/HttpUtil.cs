using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace FCP.Util
{
    /// <summary>
    /// Http请求核心
    /// </summary>
    public static class HttpUtil
    {
        #region POST请求
        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="parameterStr">请求参数字符串</param>      
        /// <param name="isSuccess">是否连接成功</param>
        /// <returns></returns>
        public static string postHttpRequest(string requestUrl, string parameterStr, out bool isSuccess)
        {
            return postHttpRequest(requestUrl, parameterStr, Encoding.UTF8, out isSuccess);
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="parameterStr">请求参数字符串</param>         
        /// <param name="dataEncoding">字符编码</param>        
        /// <param name="isSuccess">是否连接成功</param>
        /// <returns></returns>
        public static string postHttpRequest(string requestUrl, string parameterStr, Encoding dataEncoding, out bool isSuccess)
        {
            return postHttpRequest(requestUrl, parameterStr, dataEncoding, string.Empty, out isSuccess);
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="parameterStr">请求参数字符串</param>     
        /// <param name="dataEncoding">字符编码</param>
        /// <param name="contentType">请求内容格式</param>
        /// <param name="isSuccess">是否连接成功</param>
        /// <returns></returns>
        public static string postHttpRequest(string requestUrl, string parameterStr, Encoding dataEncoding,
            string contentType, out bool isSuccess)
        {
            return sendHttpRequest(requestUrl, parameterStr, "POST", dataEncoding, contentType, out isSuccess);
        }
        #endregion

        #region 发送Http请求
        /// <summary>
        /// 发送Http请求
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="parameterStr">请求参数字符串</param>
        /// <param name="httpMethod">请求方式</param>
        /// <param name="isSuccess">是否连接成功</param>
        /// <returns></returns>
        public static string sendHttpRequest(string requestUrl, string parameterStr, string httpMethod, out bool isSuccess)
        {
            return sendHttpRequest(requestUrl, parameterStr, httpMethod, Encoding.UTF8, out isSuccess);
        }

        /// <summary>
        /// 发送Http请求
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="parameterStr">请求参数字符串</param>
        /// <param name="httpMethod">请求方式</param>        
        /// <param name="dataEncoding">字符编码</param>        
        /// <param name="isSuccess">是否连接成功</param>
        /// <returns></returns>
        public static string sendHttpRequest(string requestUrl, string parameterStr, string httpMethod,
            Encoding dataEncoding, out bool isSuccess)
        {
            return sendHttpRequest(requestUrl, parameterStr, httpMethod, dataEncoding, string.Empty, out isSuccess);
        }

        /// <summary>
        /// 发送Http请求
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="parameterStr">请求参数字符串</param>
        /// <param name="httpMethod">请求方式</param>        
        /// <param name="dataEncoding">字符编码</param>
        /// <param name="contentType">请求内容格式</param>
        /// <param name="isSuccess">是否连接成功</param>
        /// <returns></returns>
        public static string sendHttpRequest(string requestUrl, string parameterStr, string httpMethod,
            Encoding dataEncoding, string contentType, out bool isSuccess)
        {
            try
            {
                WebClient webClient = new WebClient();
                if (!string.IsNullOrEmpty(contentType))
                {
                    webClient.Headers.Add("Content-Type", contentType);
                }
                byte[] responseData = null;
                if (StringUtil.compareIgnoreCase(httpMethod, "GET"))
                {
                    string fullRequestUrl = requestUrl;
                    if (!parameterStr.isNullOrEmpty())  //存在QueryString参数
                    {
                        fullRequestUrl = string.Format("{0}?{1}", requestUrl, Uri.EscapeDataString(parameterStr));
                    }
                    responseData = webClient.DownloadData(fullRequestUrl);
                }
                else
                {
                    if (string.IsNullOrEmpty(parameterStr)) parameterStr = string.Empty;
                    byte[] parameterData = dataEncoding.GetBytes(parameterStr);
                    responseData = webClient.UploadData(requestUrl, httpMethod, parameterData);//得到返回字符流
                }
                isSuccess = true;
                return dataEncoding.GetString(responseData);
            }
            catch (WebException webEx)
            {
                Trace.TraceError(webEx.FormatLogMessage());
                if (webEx.Status == WebExceptionStatus.ProtocolError)
                {
                    isSuccess = true;
                    var httpResponse = (HttpWebResponse)webEx.Response;
                    using (var stream = httpResponse.GetResponseStream())
                    {
                        return StreamHelper.getStringFromStream(stream, dataEncoding.WebName);
                    }
                }
                else
                {
                    isSuccess = false;
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.FormatLogMessage());
                isSuccess = false;
                return string.Empty;
            }
        }
        #endregion
    }
}
