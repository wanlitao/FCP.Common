using System.Reflection;

namespace FCP.Entity
{
    /// <summary>
    /// 属性映射接口
    /// </summary>
    public interface IPropertyMap
    {
        string name { get; }
        string columnName { get; }
        bool ignored { get; }
        /// <summary>
        /// 是否只读(update排除字段)
        /// </summary>
        bool isReadOnly { get; }
        /// <summary>
        /// 是否删除标识
        /// </summary>
        bool isDeleteFlag { get; }
        /// <summary>
        /// 删除标识True值
        /// </summary>
        object deleteFlagTrueValue { get; }

        KeyType keyType { get; }
        PropertyInfo propertyInfo { get; }
    }
}
