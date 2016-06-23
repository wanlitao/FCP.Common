using FCP.Repository;

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
        public Service()
            : this(new Repository<TEntity>())
        {

        }
       
        public Service(IRepository<TEntity> entityRepository)
        {
            _repository = entityRepository;
        }
        #endregion

        /// <summary>
        /// 实体仓储
        /// </summary>
        public IRepository<TEntity> repository { get { return _repository; } }

        /// <summary>
        /// 操作单元
        /// </summary>
        public IUnitOfWork workUnit
        {
            get { return new UnitOfWork(_repository.dbContext); }
        }
    }
}
