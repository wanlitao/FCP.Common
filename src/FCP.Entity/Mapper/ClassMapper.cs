using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using FCP.Util;

namespace FCP.Entity
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

            //删除标识配置
            deleteFlagPropertyName = _defaultDeleteFlagPropertyName;
            propertyTypeDeleteFlagTrueValueMapping = _defaultPropertyTypeDeleteFlagTrueValueMapping;

            properties = new List<IPropertyMap>();
            table(typeof(TEntity).Name);
        }

        public ClassMapper(string deleteFlagName)
            : this()
        {
            if (!deleteFlagName.isNullOrEmpty())
            {
                deleteFlagPropertyName = deleteFlagName;
            }
        }

        #region Mapping配置
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
        /// <summary>
        /// 默认删除标识属性名
        /// </summary>
        private const string _defaultDeleteFlagPropertyName = "Is_Del";

        /// <summary>
        /// 默认属性类型与删除标识True值对应字典
        /// </summary>
        private static IDictionary<Type, object> _defaultPropertyTypeDeleteFlagTrueValueMapping = new Dictionary<Type, object>
                                            {
                                                { typeof(bool), true }, { typeof(bool?), true },
                                                { typeof(byte), Convert.ToByte(1) }, { typeof(byte?), Convert.ToByte(1) },
                                                { typeof(sbyte), Convert.ToSByte(1) }, { typeof(sbyte?), Convert.ToSByte(1) },
                                                { typeof(short), Convert.ToInt16(1) }, { typeof(short?), Convert.ToInt16(1) },
                                                { typeof(ushort), Convert.ToUInt16(1) }, { typeof(ushort?), Convert.ToUInt16(1) },
                                                { typeof(string), "Y" }
                                            };

        protected IDictionary<Type, KeyType> propertyTypeKeyTypeMapping { get; private set; }

        protected string deleteFlagPropertyName { get; private set; }

        protected IDictionary<Type, object> propertyTypeDeleteFlagTrueValueMapping { get; private set; }
        #endregion

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

            bool hasDefinedDeleteFlag = properties.Any(p => p.isDeleteFlag);
            PropertyMap deleteFlagMap = null;

            foreach (var propertyInfo in ReflectionHelper.getProperties(type))
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

                if (!hasDefinedDeleteFlag)  //自动映射属性之前没有定义删除标识
                {
                    if (string.Equals(map.name, deleteFlagPropertyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        deleteFlagMap = map;
                    }
                }
            }

            if (keyMap != null)
            {
                keyMap.key(getKeyTypeFromPropertyType(keyMap.propertyInfo.PropertyType));
            }
            if (deleteFlagMap != null)
            {
                deleteFlagMap.deleteFlag(getDeleteFlagTrueValueFromPropertyType(deleteFlagMap.propertyInfo.PropertyType));
            }
        }

        /// <summary>
        /// Fluently, maps an entity property to a column
        /// </summary>
        protected PropertyMap mapProperty(Expression<Func<TEntity, object>> expression)
        {
            var propertyInfo = ReflectionHelper.parseProperty(expression);
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

        private object getDeleteFlagTrueValueFromPropertyType(Type propertyType)
        {
            if (propertyTypeDeleteFlagTrueValueMapping.ContainsKey(propertyType))
            {
                return propertyTypeDeleteFlagTrueValueMapping[propertyType];
            }

            throw new ArgumentException(string.Format("'{0}' Type cannot determine true value of delete flag", propertyType.FullName));
        }
    }
}
