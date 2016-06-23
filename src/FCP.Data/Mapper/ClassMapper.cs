using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace FCP.Data
{
    /// <summary>
    /// 实体映射
    /// </summary>
    public class ClassMapper<TEntity> : IClassMapper<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets or sets the schema to use when referring to the corresponding table name in the database.
        /// </summary>
        public string schemaName { get; protected set; }

        /// <summary>
        /// Gets or sets the table to use in the database.
        /// </summary>
        public string tableName { get; protected set; }

        /// <summary>
        /// A collection of properties that will map to columns in the database table.
        /// </summary>
        public IList<IPropertyMap> properties { get; private set; }

        public Type entityType
        {
            get { return typeof(TEntity); }
        }

        public ClassMapper()
        {
            propertyTypeKeyTypeMapping = _defaultPropertyTypeKeyTypeMapping;
            properties = new List<IPropertyMap>();
            table(typeof(TEntity).Name);
        }

        /// <summary>
        /// 默认属性类型与键类型对应字典
        /// </summary>
        private static IDictionary<Type, KeyType> _defaultPropertyTypeKeyTypeMapping = new Dictionary<Type, KeyType>
                                             {
                                                 { typeof(byte), KeyType.identity }, { typeof(byte?), KeyType.identity },
                                                 { typeof(sbyte), KeyType.identity }, { typeof(sbyte?), KeyType.identity },
                                                 { typeof(short), KeyType.identity }, { typeof(short?), KeyType.identity },
                                                 { typeof(ushort), KeyType.identity }, { typeof(ushort?), KeyType.identity },
                                                 { typeof(int), KeyType.identity }, { typeof(int?), KeyType.identity },
                                                 { typeof(uint), KeyType.identity}, { typeof(uint?), KeyType.identity },
                                                 { typeof(long), KeyType.identity }, { typeof(long?), KeyType.identity },
                                                 { typeof(ulong), KeyType.identity }, { typeof(ulong?), KeyType.identity },
                                                 { typeof(BigInteger), KeyType.identity }, { typeof(BigInteger?), KeyType.identity },
                                                 { typeof(Guid), KeyType.guid }, { typeof(Guid?), KeyType.guid },
                                             };

        protected IDictionary<Type, KeyType> propertyTypeKeyTypeMapping { get; private set; }

        public virtual void schema(string schemaName)
        {
            this.schemaName = schemaName;
        }

        public virtual void table(string tableName)
        {
            this.tableName = tableName;
        }

        protected virtual void autoMap()
        {
            autoMap(null);
        }

        protected virtual void autoMap(Func<Type, PropertyInfo, bool> canMap)
        {
            var type = entityType;
            bool hasDefinedKey = properties.Any(p => p.keyType != KeyType.notAKey);
            PropertyMap keyMap = null;
            foreach (var propertyInfo in type.GetProperties())
            {
                if (properties.Any(p => string.Equals(p.name, propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                if ((canMap != null && !canMap(type, propertyInfo)))
                {
                    continue;
                }
                
                PropertyMap map = mapProperty(propertyInfo);
                if (!hasDefinedKey)   //自动映射属性之前没有定义Key
                {
                    if (string.Equals(map.name, "id", StringComparison.InvariantCultureIgnoreCase))
                    {
                        keyMap = map;   //优先取属性名为id的作为key
                    }

                    if (keyMap == null && map.name.EndsWith("id", true, CultureInfo.InvariantCulture))
                    {
                        keyMap = map;   //不存在id属性时 就将第一个以id结尾的属性作为key
                    }
                }
            }
            if (keyMap != null)
            {
                keyMap.key(getKeyTypeFromPropertyType(keyMap.propertyInfo.PropertyType));
            }
        }

        /// <summary>
        /// Fluently, maps an entity property to a column
        /// </summary>
        protected PropertyMap mapProperty(Expression<Func<TEntity, object>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.getProperty(expression) as PropertyInfo;
            return mapProperty(propertyInfo);
        }

        /// <summary>
        /// Fluently, maps an entity property to a column
        /// </summary>
        protected PropertyMap mapProperty(PropertyInfo propertyInfo)
        {
            PropertyMap result = new PropertyMap(propertyInfo);
            guardForDuplicatePropertyMap(result);
            properties.Add(result);
            return result;
        }

        /// <summary>
        /// 检测是否存在重复的属性名
        /// </summary>
        /// <param name="result"></param>
        private void guardForDuplicatePropertyMap(PropertyMap result)
        {
            if (properties.Any(p => p.name.Equals(result.name)))
            {
                throw new ArgumentException(string.Format("Duplicate mapping for property {0} detected.", result.name));
            }
        }

        private KeyType getKeyTypeFromPropertyType(Type propertyType)
        {
            if (propertyTypeKeyTypeMapping.ContainsKey(propertyType))
            {
                return propertyTypeKeyTypeMapping[propertyType];
            }
            return KeyType.assigned;
        }
    }
}
