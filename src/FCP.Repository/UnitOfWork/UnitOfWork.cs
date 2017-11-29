using FluentData;
using System;

namespace FCP.Repository
{
    /// <summary>
    /// 数据库操作单元
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContext _dbContext;

        public UnitOfWork(IDbContext fluentDbContext)
        {
            _dbContext = fluentDbContext.UseTransaction(true);
        }

        /// <summary>
        /// DbContext属性
        /// </summary>
        public IDbContext dbContext { get { return _dbContext; } }

        /// <summary>
        /// 保存更改
        /// </summary>
        public void saveChanges()
        {
            _dbContext.Commit();
        }        

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Rollback();
                    _dbContext.UseTransaction(false);
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
