using System;

namespace FCP.Repository
{
    /// <summary>
    /// 操作单元接口
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 保存更改
        /// </summary>
        void saveChanges();
    }
}
