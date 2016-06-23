using System;

namespace FCP.Util
{
    /// <summary>
    /// 类型转换
    /// </summary>
    public abstract class TypeParse
    {
        /// <summary>
        /// 类型转换，根据类型生成默认值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">要转换的值</param>
        /// <returns></returns>
        public static T parse<T>(object value)
        {
            return parse<T>(value, default(T));
        }
        /// <summary>
        /// 类型转换，输入默认值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">要转换的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T parse<T>(object value, T defaultValue)
        {
            object r = null;
            if (tryParse(typeof(T), value, out r))
            {
                return (T)r;
            }
            return defaultValue;
        }
        /// <summary>
        /// 验证类型，不带返回值
        /// </summary>
        /// <typeparam name="T">要验证的类型</typeparam>
        /// <param name="value">要验证的值</param>
        /// <returns></returns>
        public static bool tryParse<T>(object value)
        {
            object r = null;
            if (tryParse(typeof(T), value, out r))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 验证类型，带返回值
        /// </summary>
        /// <typeparam name="T">要验证的类型</typeparam>
        /// <param name="value">要验证的值</param>
        /// <param name="result">返回值</param>
        /// <returns></returns>
        public static bool tryParse<T>(string value, out T result)
        {
            object r = null;

            if (tryParse(typeof(T), value, out r))
            {
                result = (T)r;
                return true;
            }
            result = default(T);
            return false;
        }
        /// <summary>
        /// 尝试解析字符串
        /// </summary>
        /// <param name="type">所要解析成的类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool tryParse(Type type, object obj)
        {
            object result;
            return tryParse(type, obj, out result);
        }
        /// <summary>
        /// 尝试解析字符串
        /// </summary>
        /// <param name="type">所要解析成的类型</param>
        /// <param name="value">字符串</param>
        /// <param name="result">解析结果，解析失败将返回null</param>
        /// <returns>解析失败将返回具体错误消息，否则将返回null，解析结果通过result获得</returns>
        public static bool tryParse(Type type, object obj, out object result)
        {
            string value = obj == null ? string.Empty : obj.ToString();
            if (string.IsNullOrEmpty(value))
            {
                result = null;
                return false;
            }

            bool succeed = false;
            object parsedValue = null;

            bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (isNullable)
                type = type.GetGenericArguments()[0];
            if (type.IsEnum)
            {
                try
                {
                    parsedValue = Enum.Parse(type, value, true);
                    succeed = true;
                }
                catch
                {
                }
            }
            else if (type == typeof(Guid))
            {
                //TODO:此处需要改善性能
                try
                {
                    parsedValue = new Guid(value);
                    succeed = true;
                }
                catch
                {
                }
            }
            else
            {
                TypeCode typeCode = Type.GetTypeCode(type);
                switch (typeCode)
                {
                    case TypeCode.String:
                        parsedValue = value;
                        succeed = true;
                        break;
                    case TypeCode.Boolean:
                        {
                            if (value == "1")
                            {
                                parsedValue = true;
                                succeed = true;
                            }
                            else if (value == "0")
                            {
                                parsedValue = false;
                                succeed = true;
                            }
                            else
                            {
                                Boolean temp;
                                succeed = Boolean.TryParse(value, out temp);
                                if (succeed) parsedValue = temp;
                            }
                        }
                        break;
                    case TypeCode.Byte:
                        {
                            Byte temp;
                            succeed = Byte.TryParse(value, out temp);

                            if (succeed) parsedValue = temp;
                        }
                        break;
                    case TypeCode.Decimal:
                        {
                            Decimal temp;
                            succeed = Decimal.TryParse(value, out temp);

                            if (succeed) parsedValue = temp;
                        }
                        break;
                    case TypeCode.Double:
                        {
                            Double temp;
                            succeed = Double.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            Int16 temp;
                            succeed = Int16.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            Int32 temp;
                            succeed = Int32.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Int64:
                        {
                            Int64 temp;
                            succeed = Int64.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.SByte:
                        {
                            SByte temp;
                            succeed = SByte.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Single:
                        {
                            Single temp;
                            succeed = Single.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            UInt16 temp;
                            succeed = UInt16.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.UInt32:
                        {
                            UInt32 temp;
                            succeed = UInt32.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.UInt64:
                        {
                            UInt64 temp;
                            succeed = UInt64.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.DateTime:
                        {
                            DateTime temp;
                            succeed = DateTime.TryParse(value, out temp);
                            if (succeed)
                            {
                                parsedValue = temp;
                            }
                        }
                        break;
                }
            }
            result = parsedValue;
            return succeed;
        }
    }
}
