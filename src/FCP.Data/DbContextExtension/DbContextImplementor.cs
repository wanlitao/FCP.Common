using FluentData;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using FCP.Util;
using ExprTranslator.Query;
using System.Reflection;
using FCP.Entity;

namespace FCP.Data
{
    /// <summary>
    /// 数据库context扩展实现
    /// </summary>
    public class DbContextImplementor : IDbContextImplementor
    {
        private const string deleteFlagValueParameterName = "pDeleteFlag";  //删除标识值对应参数名
        private static ExprQueryTranslatorFactory exprQueryTranslatorFactory = new ExprQueryTranslatorFactory();

        public DbContextImplementor(IEntityConfiguration configuration, IDbProvider fluentDbProvider)
        {
            sqlGenerator = new SqlGenerator(configuration, fluentDbProvider);

            exprQueryTranslator = exprQueryTranslatorFactory.getExprQueryTranslatorByDbProvider(fluentDbProvider);
            exprQueryTranslator.MemberColumnNameConverter = exprQueryEntityMemberColumnNameConverter;
        }

        /// <summary>
        /// Fluent查询Sql生成器
        /// </summary>
        public ISqlGenerator sqlGenerator { get; private set; }

        /// <summary>
        /// 表达式查询translator
        /// </summary>
        public IExprQueryTranslator exprQueryTranslator { get; private set; }

        #region 公共方法
        /// <summary>
        /// 表达式查询中实体属性转换列名
        /// </summary>
        /// <param name="entityMember"></param>
        /// <returns></returns>
        protected string exprQueryEntityMemberColumnNameConverter(MemberInfo entityMember)
        {            
            Type entityType = entityMember.ReflectedType;
            if (entityType.IsConcrete())
            {
                return sqlGenerator.getColumnName(entityType, entityMember.Name, false, false);
            }
            return entityMember.Name;               
        }        

        /// <summary>
        /// 生成主键查询的lambda表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        protected Expression<Func<TEntity, bool>> getKeyPredicate<TEntity>(object id) where TEntity : class
        {
            var keyPropertyMaps = sqlGenerator.getKeyProperties<TEntity>();

            IDictionary<string, object> keyValues = null;
            bool isSimpleType = Entity.ReflectionHelper.isSimpleType(id.GetType());
            if (!isSimpleType)
            {
                keyValues = id.toDictionaryByDynamicEmit(); //复杂类型时转换成字典
            }

            object keyValue = id;
            Expression bodyExpr = null;
            var parameterExpr = Expression.Parameter(typeof(TEntity), "entity");
            foreach (var keyPropertyMap in keyPropertyMaps)
            {
                if (!isSimpleType)
                {
                    keyValue = keyValues[keyPropertyMap.name];
                }
                //code: entity.ID == keyValue
                var propertyExpr = Expression.Property(parameterExpr, keyPropertyMap.propertyInfo);
                var propertyValueExpr = Expression.Constant(keyValue, keyPropertyMap.propertyInfo.PropertyType);
                var propertyEqualExpr = Expression.Equal(propertyExpr, propertyValueExpr);
                
                bodyExpr = (bodyExpr == null) ? propertyEqualExpr
                    : Expression.AndAlso(bodyExpr, propertyEqualExpr); //多个主键字段 进行AND运算
            }
            var lambdaExpr = Expression.Lambda<Func<TEntity, bool>>(bodyExpr, parameterExpr);
            return lambdaExpr;
        }

        #region 生成属性值集合
        /// <summary>
        /// 生成主键值集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        protected KeyValuePair<IPropertyMap, object>[] getKeyPropertyValues<TEntity>(object id) where TEntity : class
        {
            var keyPropertyMaps = sqlGenerator.getKeyProperties<TEntity>();

            IDictionary<string, object> keyValues = null;
            bool isSimpleType = Entity.ReflectionHelper.isSimpleType(id.GetType());
            if (!isSimpleType)
            {
                keyValues = id.toDictionaryByDynamicEmit(); //复杂类型时转换成字典
            }

            var keyPropertyValueDic = new Dictionary<IPropertyMap, object>();

            object keyValue = id;
            foreach (var keyPropertyMap in keyPropertyMaps)
            {
                if (!isSimpleType)
                {
                    keyValue = keyValues[keyPropertyMap.name];
                }
                keyPropertyValueDic.Add(keyPropertyMap, keyValue);
            }

            return keyPropertyValueDic.ToArray();
        }

        /// <summary>
        /// 生成主键值集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        protected KeyValuePair<IPropertyMap, object>[] getKeyPropertyValues<TEntity>(TEntity entity) where TEntity : class
        {
            var keyPropertyMaps = sqlGenerator.getKeyProperties<TEntity>();

            return getPropertyValues(entity, keyPropertyMaps.ToArray());
        }

        /// <summary>
        /// 生成属性值集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="propertyMaps"></param>
        /// <returns></returns>
        protected KeyValuePair<IPropertyMap, object>[] getPropertyValues<TEntity>(TEntity entity,
            params IPropertyMap[] propertyMaps) where TEntity : class
        {
            var propertyValueDic = new Dictionary<IPropertyMap, object>();

            if (propertyMaps.isNotEmpty())
            {
                foreach(var propertyMap in propertyMaps.Where(m => m != null))
                {
                    var propertyValue = Entity.ReflectionHelper.getPropertyValue(entity, propertyMap.propertyInfo);
                    propertyValueDic.Add(propertyMap, propertyValue);
                }
            }

            return propertyValueDic.ToArray();
        }
        #endregion

        #endregion

        #region 获取属性表达式
        /// <summary>
        /// 获取属性表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public Expression<Func<TEntity, object>> getPropertyExpression<TEntity>(string propertyName) where TEntity : class
        {
            var propertyMap = sqlGenerator.getProperty<TEntity>(propertyName);

            return getPropertyExpression<TEntity>(propertyMap);
        }

        /// <summary>
        /// 获取属性表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyMap">属性Map信息</param>
        /// <returns></returns>
        public Expression<Func<TEntity, object>> getPropertyExpression<TEntity>(IPropertyMap propertyMap) where TEntity : class
        {
            return ExpressionUtil.getPropertyExpression<TEntity>(propertyMap.propertyInfo);
        }        
        #endregion

        #region 获取属性值Predicate
        /// <summary>
        /// 获取属性值Predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> getPropertyPredicate<TEntity>(string propertyName, object propertyValue) where TEntity : class
        {
            var propertyMap = sqlGenerator.getProperty<TEntity>(propertyName);

            return getPropertyPredicate<TEntity>(propertyMap, propertyValue);
        }

        /// <summary>
        /// 获取属性值Predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyMap">属性Map信息</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> getPropertyPredicate<TEntity>(IPropertyMap propertyMap, object propertyValue) where TEntity : class
        {
            return ExpressionUtil.getPropertyPredicate<TEntity>(propertyMap.propertyInfo, propertyValue);
        }
        #endregion

        #region 获取Sql
        /// <summary>
        /// 获取查询字段Sql
        /// </summary>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public string getEntitySelectSql<TEntity>(
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            var selectPropertyMaps = sqlGenerator.getSelectProperties(ignorePropertyExpressions);

            return string.Join(",", selectPropertyMaps.Select(p => sqlGenerator.getColumnName<TEntity>(p, false, true)));
        }        
        #endregion

        #region select查询

        #region 辅助方法
        /// <summary>
        /// 获取实体查询Builder
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext">数据库context</param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        protected ISelectBuilder<TEntity> getEntitySelectBuilder<TEntity>(IDbContext dbContext, string tableAlias,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            var selectSql = getEntitySelectSql(ignorePropertyExpressions);

            ISelectBuilder<TEntity> entitySelectBuilder = getSelectBuilder<TEntity, TEntity>(dbContext, tableAlias, selectSql);

            return entitySelectBuilder;
        }

        /// <summary>
        /// 获取查询Builder
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="dbContext"></param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="selectSql">查询语句</param>
        /// <returns></returns>
        protected ISelectBuilder<TResult> getSelectBuilder<TEntity, TResult>(IDbContext dbContext, string tableAlias, string selectSql) where TEntity : class
        {
            selectSql = selectSql.isNullOrEmpty() ? "*" : selectSql;            

            return dbContext.Select<TResult>(selectSql).From(sqlGenerator.getTableName<TEntity>(tableAlias));            
        }

        /// <summary>
        /// 设置SelectBuilder的where条件
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="selectBuilder">selectBuilder</param>
        /// <param name="predicateExpr">where条件表达式</param>
        protected void setSelectBuilderWherePredicate<TEntity, TResult>(ISelectBuilder<TResult> selectBuilder,
            Expression<Func<TEntity, bool>> predicateExpr) where TEntity : class
        {
            if (selectBuilder == null || predicateExpr == null)
                return;

            QuerySql keyWhereSql = exprQueryTranslator.TranslateSql(predicateExpr);

            selectBuilder.AndWhere(keyWhereSql.whereStr);
            if (keyWhereSql.parameters.isNotEmpty())
            {
                foreach (var parameter in keyWhereSql.parameters)
                {
                    selectBuilder.Parameter(parameter.Name, parameter.Value, DataTypes.Object,
                        ParameterDirection.Input, parameter.QueryType.Length);
                }
            }

            //设置 删除标识 where条件
            checkSetSelectBuilderDeleteFlagWhere<TEntity, TResult>(selectBuilder);
        }

        /// <summary>
        /// 设置SelectBuilder 删除标识where条件
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selectBuilder"></param>
        protected void checkSetSelectBuilderDeleteFlagWhere<TEntity, TResult>(ISelectBuilder<TResult> selectBuilder) where TEntity : class
        {
            var deleteFlagProperty = sqlGenerator.getDeleteFlagProperty<TEntity>();
            if (deleteFlagProperty != null)
            {
                var deleteFlagTrueParameterName = sqlGenerator.dbProvider.GetParameterName(deleteFlagValueParameterName);

                selectBuilder.AndWhere(string.Format("{0} <> {1}",
                    sqlGenerator.getColumnName<TEntity>(deleteFlagProperty, false, false), deleteFlagTrueParameterName));
                selectBuilder.Parameter(deleteFlagTrueParameterName, deleteFlagProperty.deleteFlagTrueValue);
            }
        }
        #endregion

        /// <summary>
        /// 按主键查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public ISelectBuilder<TEntity> selectEntityByKey<TEntity>(IDbContext dbContext, object id,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            Expression<Func<TEntity, bool>> keyPredicate = getKeyPredicate<TEntity>(id);

            return selectEntityByWhere(dbContext, keyPredicate, ignorePropertyExpressions);
        }

        /// <summary>
        /// 按where条件查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext">数据库context</param>
        /// <param name="wherePredicate">where条件</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public ISelectBuilder<TEntity> selectEntityByWhere<TEntity>(IDbContext dbContext, Expression<Func<TEntity, bool>> wherePredicate,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            ISelectBuilder<TEntity> entitySelectBuilder = getEntitySelectBuilder(dbContext, null, ignorePropertyExpressions);

            setSelectBuilderWherePredicate(entitySelectBuilder, wherePredicate);

            return entitySelectBuilder;
        }

        /// <summary>
        /// 按where条件查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="dbContext"></param>
        /// <param name="selectSql">查询字段Sql</param>
        /// <param name="wherePredicate">where条件</param>
        /// <returns></returns>
        public ISelectBuilder<TResult> selectByWhere<TEntity, TResult>(IDbContext dbContext, string selectSql,
            Expression<Func<TEntity, bool>> wherePredicate) where TEntity : class
        {
            ISelectBuilder<TResult> selectBuilder = getSelectBuilder<TEntity, TResult>(dbContext, null, selectSql);

            setSelectBuilderWherePredicate(selectBuilder, wherePredicate);

            return selectBuilder;
        }
        #endregion

        #region delete删除
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public IDeleteBuilder deleteEntityByKey<TEntity>(IDbContext dbContext, object id) where TEntity : class
        {
            var keyPropertyWheres = getKeyPropertyValues<TEntity>(id);

            return deleteWhere<TEntity>(dbContext, keyPropertyWheres);
        }

        /// <summary>
        /// 按实体主键删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public IDeleteBuilder deleteEntity<TEntity>(IDbContext dbContext, TEntity entity) where TEntity : class
        {
            var keyPropertyWheres = getKeyPropertyValues(entity);

            return deleteWhere<TEntity>(dbContext, keyPropertyWheres);
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <param name="includePropertyExpressions">where属性表达式</param>
        /// <returns></returns>
        public IDeleteBuilder deleteEntityByWhere<TEntity>(IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class
        {
            var propertyMaps = sqlGenerator.getProperties(includePropertyExpressions);
            var propertyWheres = getPropertyValues(entity, propertyMaps.ToArray());            

            return deleteWhere<TEntity>(dbContext, propertyWheres);
        }

        /// <summary>
        /// 按where条件删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="propertyWheres">where条件</param>
        /// <returns></returns>
        protected IDeleteBuilder deleteWhere<TEntity>(IDbContext dbContext,
            params KeyValuePair<IPropertyMap, object>[] propertyWheres) where TEntity : class
        {
            IDeleteBuilder deleteBuilder = dbContext.Delete(sqlGenerator.getTableName<TEntity>(null));

            if (propertyWheres.isNotEmpty())
            {
                foreach (var propertyWhere in propertyWheres)
                {                    
                    deleteBuilder.Where(propertyWhere.Key.columnName, propertyWhere.Value);
                }
            }

            return deleteBuilder;
        }
        #endregion

        #region update更新        
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public IUpdateBuilder updateEntity<TEntity>(IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class
        {
            var keyPropertyWheres = getKeyPropertyValues(entity);
            
            var updatePropertyMaps = sqlGenerator.getUpdateProperties(false, includePropertyExpressions);
            var propertyUpdates = getPropertyValues(entity, updatePropertyMaps.ToArray());

            return updateWhere<TEntity>(dbContext, keyPropertyWheres, propertyUpdates);
        }

        /// <summary>
        /// 更新实体（忽略属性）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public IUpdateBuilder updateEntityIgnore<TEntity>(IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            var keyPropertyWheres = getKeyPropertyValues(entity);

            var updatePropertyMaps = sqlGenerator.getUpdateProperties(true, ignorePropertyExpressions);
            var propertyUpdates = getPropertyValues(entity, updatePropertyMaps.ToArray());

            return updateWhere<TEntity>(dbContext, keyPropertyWheres, propertyUpdates);
        }

        /// <summary>
        /// 按主键更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体</param>
        /// <param name="includePropertyExpressions">更新的属性表达式</param>
        /// <returns></returns>
        public IUpdateBuilder updateEntityByKey<TEntity>(IDbContext dbContext, object id,
            TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions) where TEntity : class
        {
            var keyPropertyWheres = getKeyPropertyValues<TEntity>(id);

            var updatePropertyMaps = sqlGenerator.getUpdateProperties(false, includePropertyExpressions);
            var propertyUpdates = getPropertyValues(entity, updatePropertyMaps.ToArray());

            return updateWhere<TEntity>(dbContext, keyPropertyWheres, propertyUpdates);            
        }

        /// <summary>
        /// 按主键更新（忽略属性）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public IUpdateBuilder updateEntityIgnoreByKey<TEntity>(IDbContext dbContext, object id,
            TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            var keyPropertyWheres = getKeyPropertyValues<TEntity>(id);

            var updatePropertyMaps = sqlGenerator.getUpdateProperties(true, ignorePropertyExpressions);
            var propertyUpdates = getPropertyValues(entity, updatePropertyMaps.ToArray());

            return updateWhere<TEntity>(dbContext, keyPropertyWheres, propertyUpdates);
        }

        /// <summary>
        /// 按where条件更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="propertyWheres">where条件</param>
        /// <param name="propertyUpdates">更新字段值</param>
        /// <returns></returns>
        protected IUpdateBuilder updateWhere<TEntity>(IDbContext dbContext, KeyValuePair<IPropertyMap, object>[] propertyWheres,
            params KeyValuePair<IPropertyMap, object>[] propertyUpdates) where TEntity : class
        {
            IUpdateBuilder updateBuilder = dbContext.Update(sqlGenerator.getTableName<TEntity>(null));

            if (propertyWheres.isNotEmpty())  //where条件
            {
                foreach (var propertyWhere in propertyWheres)
                {
                    updateBuilder.Where(propertyWhere.Key.columnName, propertyWhere.Value);
                }
            }

            if (propertyUpdates.isNotEmpty())  //更新字段
            {
                foreach (var propertyUpdate in propertyUpdates)
                {                    
                    updateBuilder.Column(propertyUpdate.Key.columnName, propertyUpdate.Value);
                }
            }

            return updateBuilder;
        }
        #endregion

        #region insert插入

        #region 辅助方法
        /// <summary>
        /// 设置InsertBuilder的插入属性列表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateBuilder"></param>
        /// <param name="entity"></param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        protected void setInsertBuilderColumns<TEntity>(IInsertBuilder insertBuilder, TEntity entity,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            var insertPropertyMaps = sqlGenerator.getInsertProperties(ignorePropertyExpressions);

            if (insertPropertyMaps.isNotEmpty())
            {
                foreach (var insertPropertyMap in insertPropertyMaps)
                {
                    var insertPropertyValue = Entity.ReflectionHelper.getPropertyValue(entity, insertPropertyMap.propertyInfo);
                    insertBuilder.Column(insertPropertyMap.columnName, insertPropertyValue);
                }
            }
        }
        #endregion

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity">实体</param>
        /// <param name="ignorePropertyExpressions">忽略的属性表达式</param>
        /// <returns></returns>
        public IInsertBuilder insertEntity<TEntity>(IDbContext dbContext, TEntity entity,
            params Expression<Func<TEntity, object>>[] ignorePropertyExpressions) where TEntity : class
        {
            IInsertBuilder insertBuilder = dbContext.Insert(sqlGenerator.getTableName<TEntity>(null));

            setInsertBuilderColumns(insertBuilder, entity, ignorePropertyExpressions);

            return insertBuilder;
        }
        #endregion
    }
}
