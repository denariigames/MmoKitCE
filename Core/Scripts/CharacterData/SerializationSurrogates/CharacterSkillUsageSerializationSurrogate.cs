using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class CharacterSkillUsageSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            CharacterSkillUsage data = (CharacterSkillUsage)obj;
            info.AddValue("type", (byte)data.type);
            info.AddValue("dataId", data.dataId);
            info.AddValue("coolDownRemainsDuration", data.coolDownRemainsDuration);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            CharacterSkillUsage data = (CharacterSkillUsage)obj;
            data.type = (SkillUsageType)info.GetByte("type");
            data.dataId = info.GetInt32("dataId");
            data.coolDownRemainsDuration = info.GetSingle("coolDownRemainsDuration");
            obj = data;
            return obj;
        }
    }
}
