using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class CharacterCurrencySerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            CharacterCurrency data = (CharacterCurrency)obj;
            info.AddValue("dataId", data.dataId);
            info.AddValue("amount", data.amount);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            CharacterCurrency data = (CharacterCurrency)obj;
            data.dataId = info.GetInt32("dataId");
            data.amount = info.GetInt32("amount");
            obj = data;
            return obj;
        }
    }
}
