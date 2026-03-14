using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class CharacterItemSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            CharacterItem data = (CharacterItem)obj;
            info.AddValue("id", data.id);
            info.AddValue("dataId", data.dataId);
            info.AddValue("level", data.level);
            info.AddValue("amount", data.amount);
            info.AddValue("equipSlotIndex", data.equipSlotIndex);
            info.AddValue("durability", data.durability);
            info.AddValue("exp", data.exp);
            info.AddValue("lockRemainsDuration", data.lockRemainsDuration);
            info.AddValue("expireTime", data.expireTime);
            info.AddValue("randomSeed", data.randomSeed);
            info.AddValue("ammo", data.ammo);
            info.AddValue("ammoDataId", data.ammoDataId);
            info.AddValue("sockets", data.sockets);
            info.AddValue("version", data.version);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            CharacterItem data = (CharacterItem)obj;
            data.dataId = info.GetInt32("dataId");
            data.level = info.GetInt32("level");
            data.amount = info.GetInt32("amount");
            data.equipSlotIndex = info.GetByte("equipSlotIndex");
            data.durability = info.GetSingle("durability");
            data.exp = info.GetInt32("exp");
            data.lockRemainsDuration = info.GetSingle("lockRemainsDuration");
            data.ammo = info.GetInt32("ammo");
            data.sockets = (List<int>)info.GetValue("sockets", typeof(List<int>));
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.id = info.GetString("id");
            }
            catch
            {
                data.id = GenericUtils.GetUniqueId();
            }
            try
            {
                data.expireTime = info.GetInt64("expireTime");
            }
            catch { }
            try
            {
                data.randomSeed = info.GetInt32("randomSeed");
            }
            catch { }
            try
            {
                data.version = info.GetByte("version");
            }
            catch { }
            try
            {
                data.ammoDataId = info.GetInt32("ammoDataId");
            }
            catch { }
            if (string.IsNullOrEmpty(data.id))
                data.id = GenericUtils.GetUniqueId();
            obj = data;
            return obj;
        }
    }
}
