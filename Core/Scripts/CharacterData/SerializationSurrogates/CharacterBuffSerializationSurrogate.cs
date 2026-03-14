using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class CharacterBuffSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            CharacterBuff data = (CharacterBuff)obj;
            info.AddValue("id", data.id);
            info.AddValue("type", (byte)data.type);
            info.AddValue("dataId", data.dataId);
            info.AddValue("level", data.level);
            info.AddValue("buffRemainsDuration", data.buffRemainsDuration);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            CharacterBuff data = (CharacterBuff)obj;
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.id = info.GetString("id");
            }
            catch
            {
                data.id = GenericUtils.GetUniqueId();
            }
            data.type = (BuffType)info.GetByte("type");
            data.dataId = info.GetInt32("dataId");
            data.level = info.GetInt32("level");
            data.buffRemainsDuration = info.GetSingle("buffRemainsDuration");
            obj = data;
            return obj;
        }
    }
}
