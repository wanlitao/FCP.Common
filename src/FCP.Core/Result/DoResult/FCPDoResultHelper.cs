using FCP.Util;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FCP.Core
{
    /// <summary>
    /// FCP处理结果助手
    /// </summary>
    public static class FCPDoResultHelper
    {
        #region 执行成功
        /// <summary>
        /// 执行成功
        /// </summary>        
        /// <returns></returns>
        public static FCPDoResult doSuccess()
        {
            return doSuccess(null);
        }
        /// <summary>
        /// 执行成功
        /// </summary>
        /// <param name="resultData">结果数据</param>
        /// <returns></returns>
        public static FCPDoResult doSuccess(object resultData)
        {
            return doSuccess(resultData, null);
        }
        /// <summary>
        /// 执行成功
        /// </summary>
        /// <param name="resultData">结果数据</param>
        /// <param name="resultMsg">结果消息</param>       
        /// <returns></returns>
        public static FCPDoResult doSuccess(object resultData, string resultMsg)
        {
            return new FCPDoResult { type = FCPDoResultType.success.ToString(), msg = resultMsg, data = resultData };
        }

        /// <summary>
        /// 执行成功
        /// </summary>
        /// <param name="stateMsg">结果消息</param>
        /// <returns></returns>
        public static FCPDoResult<TResultData> doSuccess<TResultData>()
        {
            return doSuccess(default(TResultData));
        }
        /// <summary>
        /// 执行成功
        /// </summary>
        /// <param name="resultData">结果数据</param>
        /// <returns></returns>
        public static FCPDoResult<TResultData> doSuccess<TResultData>(TResultData resultData)
        {
            return doSuccess(resultData, null);
        }
        /// <summary>
        /// 执行成功
        /// </summary>
        /// <param name="resultData">结果数据</param>
        /// <param name="resultMsg">结果消息</param>       
        /// <returns></returns>
        public static FCPDoResult<TResultData> doSuccess<TResultData>(TResultData resultData, string resultMsg)
        {
            return new FCPDoResult<TResultData> { type = FCPDoResultType.success.ToString(), msg = resultMsg, data = resultData };
        }
        #endregion

        #region 执行失败
        /// <summary>
        /// 执行失败
        /// </summary>
        /// <param name="resultMsg">结果消息</param>
        /// <returns></returns>
        public static FCPDoResult doFail(string resultMsg)
        {
            return new FCPDoResult { type = FCPDoResultType.fail.ToString(), msg = resultMsg };
        }

        /// <summary>
        /// 执行失败
        /// </summary>
        /// <param name="resultMsg">结果消息</param>
        /// <returns></returns>
        public static FCPDoResult<TResultData> doFail<TResultData>(string resultMsg)
        {
            return new FCPDoResult<TResultData> { type = FCPDoResultType.fail.ToString(), msg = resultMsg };
        }
        #endregion

        #region 执行验证
        /// <summary>
        /// 执行验证
        /// </summary>        
        /// <param name="validateResults">验证结果集合</param>
        /// <returns></returns>
        public static FCPDoResult doValidate(params ValidationResult[] validateResults)
        {
            if (validateResults.isEmpty())
            {
                return doSuccess();
            }

            var validFailMsg = string.Join(Environment.NewLine, validateResults.Select(m => m.ErrorMessage));
            var validFailData = validateResults.ToDictionary(m => m.MemberNames.FirstOrDefault(), m => m.ErrorMessage);
            return new FCPDoResult { type = FCPDoResultType.validFail.ToString(), msg = validFailMsg, validFailResults = validFailData };
        }

        /// <summary>
        /// 执行验证
        /// </summary>        
        /// <param name="validateResults">验证结果集合</param>
        /// <returns></returns>
        public static FCPDoResult<TResultData> doValidate<TResultData>(params ValidationResult[] validateResults)
        {
            if (validateResults.isEmpty())
            {
                return doSuccess<TResultData>();
            }

            var validFailMsg = string.Join(Environment.NewLine, validateResults.Select(m => m.ErrorMessage));
            var validFailData = validateResults.ToDictionary(m => m.MemberNames.FirstOrDefault(), m => m.ErrorMessage);
            return new FCPDoResult<TResultData> { type = FCPDoResultType.validFail.ToString(), msg = validFailMsg, validFailResults = validFailData };
        }
        #endregion

        #region 执行未找到
        /// <summary>
        /// 执行未找到
        /// </summary>
        /// <param name="resultMsg">结果消息</param>
        /// <returns></returns>
        public static FCPDoResult doNotFound(string resultMsg)
        {
            return new FCPDoResult { type = FCPDoResultType.notFound.ToString(), msg = resultMsg };
        }

        /// <summary>
        /// 执行未找到
        /// </summary>
        /// <param name="resultMsg">结果消息</param>
        /// <returns></returns>
        public static FCPDoResult<TResultData> doNotFound<TResultData>(string resultMsg)
        {
            return new FCPDoResult<TResultData> { type = FCPDoResultType.notFound.ToString(), msg = resultMsg };
        }
        #endregion

        #region 执行未认证
        /// <summary>
        /// 执行未认证
        /// </summary>
        /// <param name="resultMsg">结果消息</param>
        /// <returns></returns>
        public static FCPDoResult doUnauthorized(string resultMsg)
        {
            return new FCPDoResult { type = FCPDoResultType.unauthorized.ToString(), msg = resultMsg };
        }

        /// <summary>
        /// 执行未认证
        /// </summary>
        /// <param name="resultMsg">结果消息</param>
        /// <returns></returns>
        public static FCPDoResult<TResultData> doUnauthorized<TResultData>(string resultMsg)
        {
            return new FCPDoResult<TResultData> { type = FCPDoResultType.unauthorized.ToString(), msg = resultMsg };
        }
        #endregion

        #region 执行未授权
        /// <summary>
        /// 执行未授权
        /// </summary>
        /// <param name="resultMsg">结果消息</param>
        /// <returns></returns>
        public static FCPDoResult doForbidden(string resultMsg)
        {
            return new FCPDoResult { type = FCPDoResultType.forbidden.ToString(), msg = resultMsg };
        }

        /// <summary>
        /// 执行未授权
        /// </summary>
        /// <param name="resultMsg">结果消息</param>
        /// <returns></returns>
        public static FCPDoResult<TResultData> doForbidden<TResultData>(string resultMsg)
        {
            return new FCPDoResult<TResultData> { type = FCPDoResultType.forbidden.ToString(), msg = resultMsg };
        }
        #endregion
    }
}
