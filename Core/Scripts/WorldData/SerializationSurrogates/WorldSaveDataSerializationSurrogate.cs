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
            info.AddValue("timeOfDay", data.timeOfDay);
            info.AddValue("currentDay", data.currentDay);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            WorldSaveData data = (WorldSaveData)obj;
            data.buildings = new List<BuildingSaveData>(info.GetListValue<BuildingSaveData>("buildings"));

            // Sync time and day
            try
            {
                data.timeOfDay = info.GetSingle("timeOfDay");
            }
            catch { data.timeOfDay = 0; }

            try
            {
                data.currentDay = info.GetInt32("currentDay");
            }
            catch { data.currentDay = 0; }

            if (GameInstance.Singleton.DayNightTimeUpdater != null)
                GameInstance.Singleton.DayNightTimeUpdater.LoadFromWorldSaveData(data);

            obj = data;
            return obj;
        }
    }
}
