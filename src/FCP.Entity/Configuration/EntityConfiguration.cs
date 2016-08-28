using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FCP.Entity
{
    /// <summary>
    /// Entity配置
    /// </summary>
    public class EntityConfiguration : IEntityConfiguration
    {
        /// <summary>
        /// 实体Mapper缓存
        /// </summary>
        private readonly ConcurrentDictionary<Type, IClassMapper> _classMappers = new ConcurrentDictionary<Type, IClassMapper>();

        #region 构造函数
        public EntityConfiguration()
            : this(typeof(AutoClassMapper<>))
        {

        }

        public EntityConfiguration(Type defaultClassMapper)
            : this(defaultClassMapper, null)
        {

        }

        public EntityConfiguration(Type defaultClassMapper, IList<Assembly> mappingAssemblies)
        {
            this.defaultClassMapper = defaultClassMapper;
            this.mappingAssemblies = mappingAssemblies ?? new List<Assembly>();
        }
        #endregion        

        #region 属性
        public Type defaultClassMapper { get; private set; }
        public IList<Assembly> mappingAssemblies { get; private set; }
        #endregion        

        #region 获取实体Mapper
        /// <summary>
        /// 获取实体Mapper
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IClassMapper getClassMapper<TEntity>() where TEntity : class
        {
            return getClassMapper(typeof(TEntity));
        }

        /// <summary>
        /// 获取实体Mapper
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public IClassMapper getClassMapper(Type entityType)
        {
            IClassMapper mapper;
            if (!_classMappers.TryGetValue(entityType, out mapper))  //优先从缓存读取
            {
                Type mapperType = getMapperTypeFromAssembly(entityType);
                if (mapperType == null)
                {
                    mapperType = defaultClassMapper.MakeGenericType(entityType);
                }

                mapper = Activator.CreateInstance(mapperType) as IClassMapper;
                _classMappers[entityType] = mapper;
            }

            return mapper;
        }

        /// <summary>
        /// 从程序集中搜索实体Mapper
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected virtual Type getMapperTypeFromAssembly(Type entityType)
        {
            Func<Assembly, Type> searchMapperTypeFromAssembly = a =>
            {
                Type[] types = a.GetTypes();
                return (from type in types
                        let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName)
                        where
                            interfaceType != null &&
                            interfaceType.GetGenericArguments()[0] == entityType
                        select type).SingleOrDefault();
            };

            Type result = searchMapperTypeFromAssembly(entityType.Assembly);
            if (result != null)
            {
                return result;
            }

            foreach (var mappingAssembly in mappingAssemblies)
            {
                result = searchMapperTypeFromAssembly(mappingAssembly);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        #endregion        

        #region 缓存操作
        /// <summary>
        /// 清除Mapper类型缓存
        /// </summary>
        public void clearCache()
        {
            _classMappers.Clear();
        }
        #endregion
    }
}
