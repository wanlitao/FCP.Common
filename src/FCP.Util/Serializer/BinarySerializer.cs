using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FCP.Util
{
    /// <summary>
    /// Binary Serializer
    /// </summary>
    public class BinarySerializer : BaseSerializer, ISerializer
    {
        #region Serialize
        protected override byte[] SerializeInternal<TValue>(TValue value)
        {            
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, value);
                return memoryStream.ToArray();
            }
        }

        protected override string SerializeStringInternal<TValue>(TValue value)
        {
            var bytes = SerializeInternal(value);

            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region Deserialize
        protected override TValue DeserializeInternal<TValue>(byte[] data)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(data))
            {
                var result = binaryFormatter.Deserialize(memoryStream);
                return (TValue)result;
            }
        }

        protected override TValue DeserializeStringInternal<TValue>(string dataStr)
        {
            var bytes = Encoding.UTF8.GetBytes(dataStr);

            return DeserializeInternal<TValue>(bytes);
        }
        #endregion
    }
}
