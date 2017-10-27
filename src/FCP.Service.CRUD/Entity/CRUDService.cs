using FCP.Core;
using FCP.Entity;
using FCP.Repository;
using FluentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FCP.Service.CRUD
{
    public class CRUDService<TEntity> : Service<TEntity>, ICRUDService<TEntity> where TEntity : class
    {
        public CRUDService(IRepository<TEntity> entityRepository)
            : base(entityRepository)
        { }

        #region 查询
        public virtual FCPDoResult<TEntity> GetByKey(object id)
        {
            var entity = repository.getByKey(id);

            return entity == null ? FCPDoResultHelper.doNotFound<TEntity>($"not find the record of pkid: {id}")
                : FCPDoResultHelper.doSuccess(entity);
        }
        
        public virtual FCPDoResult<TEntity> GetSingle(Expression<Func<TEntity, bool>> wherePredicate,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var selectBuilder = repository.query(wherePredicate, ignorePropertyExpressions);

            return selectBuilder.GetSingle();
        }        
        
        public virtual FCPDoResult<IList<TEntity>> GetList(Expression<Func<TEntity, bool>> wherePredicate,
            Action<ISelectBuilder<TEntity>> queryAction, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var selectBuilder = repository.query(wherePredicate, ignorePropertyExpressions);
            queryAction?.Invoke(selectBuilder);

            return selectBuilder.GetList();
        }

        public virtual FCPDoResult<FCPPageData<TEntity>> GetPageList(int currentPage, int pageSize,
            Expression<Func<TEntity, bool>> wherePredicate, Action<ISelectBuilder<TEntity>> queryAction,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var selectBuilder = repository.query(wherePredicate, ignorePropertyExpressions);
            queryAction?.Invoke(selectBuilder);

            return selectBuilder.GetPageList(currentPage, pageSize);
        }
        #endregion

        #region 插入
        public virtual FCPDoResult<TKey> Insert<TKey>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var validResult = ValidateByExclude<TKey>(entity, ignorePropertyExpressions);
            if (validResult.isValidFail)
                return validResult;  //验证失败

            var id = repository.insert<TKey>(entity, ignorePropertyExpressions);

            return id == null ? FCPDoResultHelper.doFail<TKey>("insert fail") : FCPDoResultHelper.doSuccess(id);
        }
        #endregion

        #region 更新
        public virtual FCPDoResult<int> Update(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            var validResult = ValidateByInclude<int>(entity, includePropertyExpressions);
            if (validResult.isValidFail)
                return validResult;  //验证失败

            var count = repository.update(entity, includePropertyExpressions);

            return count > 0 ? FCPDoResultHelper.doSuccess(count) : FCPDoResultHelper.doFail<int>("update fail");
        }

        public virtual FCPDoResult<int> UpdateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var validResult = ValidateByExclude<int>(entity, ignorePropertyExpressions);
            if (validResult.isValidFail)
                return validResult;  //验证失败

            var count = repository.updateIgnore(entity, ignorePropertyExpressions);

            return count > 0 ? FCPDoResultHelper.doSuccess(count) : FCPDoResultHelper.doFail<int>("update fail");
        }

        public virtual FCPDoResult<int> UpdateByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            var validResult = ValidateByInclude<int>(entity, includePropertyExpressions);
            if (validResult.isValidFail)
                return validResult;  //验证失败

            var count = repository.updateByKey(id, entity, includePropertyExpressions);

            return count > 0 ? FCPDoResultHelper.doSuccess(count) : FCPDoResultHelper.doFail<int>("update fail");
        }        

        public virtual FCPDoResult<int> UpdateIgnoreByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var validResult = ValidateByExclude<int>(entity, ignorePropertyExpressions);
            if (validResult.isValidFail)
                return validResult;  //验证失败

            var count = repository.updateIgnoreByKey(id, entity, ignorePropertyExpressions);

            return count > 0 ? FCPDoResultHelper.doSuccess(count) : FCPDoResultHelper.doFail<int>("update fail");            
        }
        #endregion

        #region 删除
        public virtual FCPDoResult<int> DeleteByKey(object id)
        {
            var count = repository.deleteByKey(id);

            return count > 0 ? FCPDoResultHelper.doSuccess(count) : FCPDoResultHelper.doFail<int>("delete fail");            
        }

        public virtual FCPDoResult<int> Delete(TEntity entity)
        {
            var count = repository.delete(entity);

            return count > 0 ? FCPDoResultHelper.doSuccess(count) : FCPDoResultHelper.doFail<int>("delete fail");
        }        

        public virtual FCPDoResult<int> DeleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            var count = repository.deleteByWhere(entity, includePropertyExpressions);

            return count > 0 ? FCPDoResultHelper.doSuccess(count) : FCPDoResultHelper.doFail<int>("delete fail");
        }
        #endregion

        #region 实体验证
        protected FCPDoResult<TResult> ValidateByExclude<TResult>(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var validateResults = entity.ValidateByExclude(ignorePropertyExpressions);

            return FCPDoResultHelper.doValidate<TResult>(validateResults.ToArray());
        }

        protected FCPDoResult<TResult> ValidateByInclude<TResult>(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var validateResults = entity.ValidateByInclude(includePropertyExpressions);

            return FCPDoResultHelper.doValidate<TResult>(validateResults.ToArray());
        }
        #endregion
    }
}
