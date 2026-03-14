using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class StorageCharacterItemSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            StorageCharacterItem data = (StorageCharacterItem)obj;
            info.AddValue("storageType", (byte)data.storageType);
            info.AddValue("storageId", data.storageOwnerId);
            info.AddValue("id", data.characterItem.id);
            info.AddValue("dataId", data.characterItem.dataId);
            info.AddValue("level", data.characterItem.level);
            info.AddValue("amount", data.characterItem.amount);
            info.AddValue("durability", data.characterItem.durability);
            info.AddValue("exp", data.characterItem.exp);
            info.AddValue("lockRemainsDuration", data.characterItem.lockRemainsDuration);
            info.AddValue("expireTime", data.characterItem.expireTime);
            info.AddValue("randomSeed", data.characterItem.randomSeed);
            info.AddValue("ammo", data.characterItem.ammo);
            info.AddValue("ammoDataId", data.characterItem.ammoDataId);
            info.AddValue("sockets", data.characterItem.sockets);
            info.AddValue("version", data.characterItem.version);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            StorageCharacterItem data = (StorageCharacterItem)obj;
            data.storageType = (StorageType)info.GetByte("storageType");
            data.storageOwnerId = info.GetString("storageId");
            CharacterItem characterItem = new CharacterItem();
            characterItem.dataId = info.GetInt32("dataId");
            characterItem.level = info.GetInt32("level");
            characterItem.amount = info.GetInt32("amount");
            characterItem.durability = info.GetSingle("durability");
            characterItem.exp = info.GetInt32("exp");
            characterItem.lockRemainsDuration = info.GetSingle("lockRemainsDuration");
            characterItem.ammo = info.GetInt32("ammo");
            characterItem.sockets = (List<int>)info.GetValue("sockets", typeof(List<int>));
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                characterItem.id = info.GetString("id");
            }
            catch
            {
                characterItem.id = GenericUtils.GetUniqueId();
            }
            try
            {
                characterItem.expireTime = info.GetInt64("expireTime");
            }
            catch { }
            try
            {
                characterItem.randomSeed = info.GetInt32("randomSeed");
            }
            catch { }
            try
            {
                characterItem.version = info.GetByte("version");
            }
            catch { }
            try
            {
                characterItem.ammoDataId = info.GetInt32("ammoDataId");
            }
            catch { }
            if (string.IsNullOrEmpty(characterItem.id))
                characterItem.id = GenericUtils.GetUniqueId();
            data.characterItem = characterItem;
            obj = data;
            return obj;
        }
    }
}
