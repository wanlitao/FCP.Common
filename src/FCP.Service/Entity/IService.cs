using FCP.Repository;

namespace FCP.Service
{
    /// <summary>
    /// 泛型数据服务接口
    /// </summary>
    public interface IService<TEntity> where TEntity : class
    {
        /// <summary>
        /// 实体仓储接口
        /// </summary>
        IRepository<TEntity> repository { get; }

        /// <summary>
        /// 操作单元
        /// </summary>
        IUnitOfWork workUnit { get; }
    }
}
