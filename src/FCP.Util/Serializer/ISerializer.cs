namespace FCP.Util
{
    public interface ISerializer
    {
        #region Serialize
        byte[] Serialize<TValue>(TValue value);

        string SerializeString<TValue>(TValue value);
        #endregion

        #region Deserialize
        TValue Deserialize<TValue>(byte[] data);

        TValue DeserializeString<TValue>(string dataStr);
        #endregion
    }
}
