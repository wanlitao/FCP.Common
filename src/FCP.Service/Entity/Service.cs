using FCP.Repository;
using System;

namespace FCP.Service
{
    /// <summary>
    /// 泛型数据服务实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        protected readonly IRepository<TEntity> _repository;

        #region 构造函数
        public Service(IRepository<TEntity> entityRepository)
        {
            if (entityRepository == null)
                throw new ArgumentNullException(nameof(entityRepository));

            _repository = entityRepository;
        }
        #endregion

        /// <summary>
        /// 实体仓储
        /// </summary>
        public IRepository<TEntity> repository { get { return _repository; } }

        /// <summary>
        /// 打开操作单元
        /// </summary>
        public IUnitOfWork openWorkUnit()
        {
            return new UnitOfWork(_repository.dbContext);
        }
    }
}
