using System.Collections.Generic;

namespace FCP.Core
{
    /// <summary>
    /// FCP处理结果
    /// </summary>
    public class FCPDoResult : FCPDoResult<object>
    {

    }

    /// <summary>
    /// FCP处理结果
    /// </summary>
    /// <typeparam name="TResultData"></typeparam>
    public class FCPDoResult<TResultData> : FCPResult<FCPDoResultType, TResultData>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool isSuc
        {
            get
            {
                return string.Compare(type, FCPDoResultType.success.ToString()) == 0;
            }
        }

        /// <summary>
        /// 是否验证失败
        /// </summary>
        public bool isValidFail
        {
            get
            {
                return string.Compare(type, FCPDoResultType.validFail.ToString()) == 0;
            }
        }

        /// <summary>
        /// 验证失败结果
        /// </summary>
        public IDictionary<string, string> validFailResults { get; set; }
    }
}
