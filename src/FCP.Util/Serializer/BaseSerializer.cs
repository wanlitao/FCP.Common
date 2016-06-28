namespace FCP.Util
{
    public abstract class BaseSerializer : ISerializer
    {
        #region Serialize
        public byte[] Serialize<TValue>(TValue value)
        {
            if (value == null)
                return null;

            return SerializeInternal(value);
        }

        protected abstract byte[] SerializeInternal<TValue>(TValue value);

        public string SerializeString<TValue>(TValue value)
        {
            if (value == null)
                return null;

            return SerializeStringInternal(value);
        }

        protected abstract string SerializeStringInternal<TValue>(TValue value);
        #endregion

        #region Deserialize
        public TValue Deserialize<TValue>(byte[] data)
        {
            if (data == null || data.Length < 1)
                return default(TValue);

            return DeserializeInternal<TValue>(data);
        }

        protected abstract TValue DeserializeInternal<TValue>(byte[] data);

        public TValue DeserializeString<TValue>(string dataStr)
        {
            if (dataStr.isNullOrEmpty())
                return default(TValue);

            return DeserializeStringInternal<TValue>(dataStr);
        }

        protected abstract TValue DeserializeStringInternal<TValue>(string dataStr);
        #endregion
    }
}
