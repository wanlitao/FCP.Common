using FCP.Entity;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FCP.Data
{
    /// <summary>
    /// Fluent数据库配置接口
    /// </summary>
    public interface IDbConfiguration
    {
        /// <summary>
        /// 默认实体Mapper类型
        /// </summary>
        Type defaultClassMapper { get; }
        /// <summary>
        /// 映射文件所在程序集
        /// </summary>
        IList<Assembly> mappingAssemblies { get; }

        /// <summary>
        /// 获取实体Mapper
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        IClassMapper getClassMapper(Type entityType);

        IClassMapper getClassMapper<TEntity>() where TEntity : class;

        /// <summary>
        /// 清除Mapper类型缓存
        /// </summary>
        void clearCache();
    }
}
