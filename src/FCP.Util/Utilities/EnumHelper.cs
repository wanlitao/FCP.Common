using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace FCP.Util
{
    /// <summary>
    /// 枚举助手
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 返回枚举值
        /// </summary>
        public static TEnum getValue<TEnum>(object value, bool ignoreCase = false) where TEnum : struct
        {
            TEnum result;
            tryValue<TEnum>(value, ignoreCase, out result);

            return result;
        }

        #region 验证枚举值
        /// <summary>
        /// 验证枚举值
        /// </summary>
        public static bool tryValue<TEnum>(object value, out TEnum result) where TEnum : struct
        {
            return tryValue<TEnum>(value, false, out result);
        }

        /// <summary>
        /// 验证枚举值
        /// </summary>
        public static bool tryValue<TEnum>(object value, bool ignoreCase, out TEnum result) where TEnum : struct
        {
            result = default(TEnum);
            if (value == null) return false;

            return Enum.TryParse<TEnum>(value.ToString(), ignoreCase, out result);
        }
        #endregion

        #region 获取枚举名称
        /// <summary>
        /// 获取枚举名称
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string getEnumName<TEnum>(object value) where TEnum : struct
        {
            if (value == null)
                return string.Empty;

            var enumType = typeof(TEnum);

            if (!Enum.IsDefined(enumType, value))
                return string.Empty;

            return Enum.GetName(enumType, value);
        }
        #endregion

        #region 获取枚举说明
        /// <summary>
        /// 枚举说明字典缓存
        /// </summary>
        private static ConcurrentDictionary<Type, IDictionary<string, string>> _enumDescriptionCache = new ConcurrentDictionary<Type, IDictionary<string, string>>();

        /// <summary>
        /// 获取枚举值说明
        /// </summary>        
        /// <param name="enumObj">枚举值</param>
        /// <returns></returns>
        public static string getDescription(this Enum enumObj)
        {
            var enumDescriptionDic = getEnumDescriptionDic(enumObj.GetType());
            return enumDescriptionDic[enumObj.ToString()];
        }

        /// <summary>
        /// 获取枚举说明字典
        /// </summary>
        /// <param name="enumType">枚举类型</param>        
        /// <returns></returns>
        private static IDictionary<string, string> getEnumDescriptionDic(Type enumType)
        {
            IDictionary<string, string> enumDescriptionDic;            
            if (!_enumDescriptionCache.TryGetValue(enumType, out enumDescriptionDic))
            {
                var enumNames = Enum.GetNames(enumType);
                enumDescriptionDic = enumNames.ToDictionary(m => m, m => getEnumDescriptionInternal(enumType, m));
                _enumDescriptionCache[enumType] = enumDescriptionDic;
            }
            return enumDescriptionDic;                
        }

        /// <summary>
        /// 获取枚举值说明 核心方法
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="enumName">枚举名称</param>
        /// <returns></returns>
        private static string getEnumDescriptionInternal(Type enumType, string enumName)
        {
            FieldInfo fi = enumType.GetField(enumName);

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.isNotEmpty())
                return attributes[0].Description;
            else
                return enumName;
        }
        #endregion        
    }
}
