using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCP.Data
{
    /// <summary>
    /// 实体映射接口
    /// </summary>
    public interface IClassMapper
    {
        string schemaName { get; }
        string tableName { get; }
        IList<IPropertyMap> properties { get; }
        Type entityType { get; }
    }

    /// <summary>
    /// 泛型实体映射接口
    /// </summary>
    public interface IClassMapper<TEntity> : IClassMapper where TEntity : class
    {

    }
}
