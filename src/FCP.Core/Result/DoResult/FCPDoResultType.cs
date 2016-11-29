namespace FCP.Core
{
    /// <summary>
    /// FCP处理结果类型
    /// </summary>
    public enum FCPDoResultType
    {
        /// <summary>
        /// 成功
        /// </summary>
        success,
        /// <summary>
        /// 失败
        /// </summary>
        fail,
        /// <summary>
        /// 验证失败
        /// </summary>
        validFail,
        /// <summary>
        /// 未找到
        /// </summary>
        notFound,
        /// <summary>
        /// 未授权
        /// </summary>
        unauthorized
    }
}
