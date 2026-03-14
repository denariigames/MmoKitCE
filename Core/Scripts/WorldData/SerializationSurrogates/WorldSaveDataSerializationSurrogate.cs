using Insthync.SerializationSurrogates;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class WorldSaveDataSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            WorldSaveData data = (WorldSaveData)obj;
            info.AddListValue("buildings", data.buildings);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            WorldSaveData data = (WorldSaveData)obj;
            data.buildings = new List<BuildingSaveData>(info.GetListValue<BuildingSaveData>("buildings"));
            obj = data;
            return obj;
        }
    }
}
