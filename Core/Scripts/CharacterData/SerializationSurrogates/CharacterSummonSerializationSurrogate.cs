using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class CharacterSummonSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            CharacterSummon data = (CharacterSummon)obj;
            info.AddValue("id", data.id);
            info.AddValue("type", (byte)data.type);
            info.AddValue("sourceId", data.sourceId);
            info.AddValue("dataId", data.dataId);
            info.AddValue("summonRemainsDuration", data.summonRemainsDuration);
            info.AddValue("level", data.Level);
            info.AddValue("exp", data.Exp);
            info.AddValue("currentHp", data.CurrentHp);
            info.AddValue("currentMp", data.CurrentMp);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            CharacterSummon data = (CharacterSummon)obj;
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.id = info.GetString("id");
            }
            catch
            {
                data.id = GenericUtils.GetUniqueId();
            }
            data.type = (SummonType)info.GetByte("type");
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.sourceId = info.GetString("sourceId");
            }
            catch { }
            data.dataId = info.GetInt32("dataId");
            data.summonRemainsDuration = info.GetSingle("summonRemainsDuration");
            data.level = info.GetInt32("level");
            data.exp = info.GetInt32("exp");
            data.currentHp = info.GetInt32("currentHp");
            data.currentMp = info.GetInt32("currentMp");
            obj = data;
            return obj;
        }
    }
}
