using FCP.Util;

namespace FCP.Service
{
    /// <summary>
    /// FCP结果基类
    /// </summary>
    /// <typeparam name="TResultType"></typeparam>
    public abstract class FCPResult<TResultType> : FCPResult<TResultType, object>
    {
        #region 构造函数
        protected FCPResult()
        {
              
        }
        #endregion
    }

    /// <summary>
    /// FCP结果基类
    /// </summary>
    /// <typeparam name="TResultType"></typeparam>
    /// <typeparam name="TResultData"></typeparam>
    public abstract class FCPResult<TResultType, TResultData>
    {
        #region 构造函数
        protected FCPResult()
        {
            type = TypeHelper.parseString(default(TResultType));
            data = default(TResultData);
        }
        #endregion

        /// <summary>
        /// 结果类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 结果消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 结果数据
        /// </summary>
        public TResultData data { get; set; }
    }
}
