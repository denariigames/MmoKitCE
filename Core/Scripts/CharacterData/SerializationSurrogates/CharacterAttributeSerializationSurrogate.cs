using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class CharacterAttributeSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            CharacterAttribute data = (CharacterAttribute)obj;
            info.AddValue("dataId", data.dataId);
            info.AddValue("amount", data.amount);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            CharacterAttribute data = (CharacterAttribute)obj;
            data.dataId = info.GetInt32("dataId");
            data.amount = info.GetInt32("amount");
            obj = data;
            return obj;
        }
    }
}
