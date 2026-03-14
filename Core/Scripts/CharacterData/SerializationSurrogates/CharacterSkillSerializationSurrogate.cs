using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class CharacterSkillSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            CharacterSkill data = (CharacterSkill)obj;
            info.AddValue("dataId", data.dataId);
            info.AddValue("level", data.level);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            CharacterSkill data = (CharacterSkill)obj;
            data.dataId = info.GetInt32("dataId");
            data.level = info.GetInt32("level");
            obj = data;
            return obj;
        }
    }
}
