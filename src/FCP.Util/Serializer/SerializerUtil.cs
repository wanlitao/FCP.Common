using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace FCP.Util
{
    /// <summary>
    /// 序列化助手
    /// </summary>
    public abstract class SerializerUtil
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象名称</param>
        /// <returns>string</returns>
        public static string serializeObject(object obj)
        {
            IFormatter bf = new BinaryFormatter();
            string result = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                byte[] byt = new byte[ms.Length];
                byt = ms.ToArray();
                result = System.Convert.ToBase64String(byt);
                ms.Flush();
            }
            return result;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="str">需要反序列化的字符串</param>
        /// <returns>类T对象</returns>
        public static object deserializeObject(string str)
        {
            object obj;
            IFormatter bf = new BinaryFormatter();
            byte[] byt = Convert.FromBase64String(str);
            using (MemoryStream ms = new MemoryStream(byt, 0, byt.Length))
            {
                obj = (object)bf.Deserialize(ms);
            }
            return obj;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="objectvalue">值</param>
        /// <returns></returns>
        public static T deserializeT<T>(string objectvalue)
        {
            T t = default(T);
            if (!string.IsNullOrEmpty(objectvalue))
            {
                t = (T)deserializeObject(objectvalue);
            }
            return t;
        }
    }
}