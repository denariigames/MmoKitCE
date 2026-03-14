using Insthync.SerializationSurrogates;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class StorageSaveDataSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            StorageSaveData data = (StorageSaveData)obj;
            info.AddListValue("storageItems", data.storageItems);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            StorageSaveData data = (StorageSaveData)obj;
            data.storageItems = new List<StorageCharacterItem>(info.GetListValue<StorageCharacterItem>("storageItems"));
            obj = data;
            return obj;
        }
    }
}
