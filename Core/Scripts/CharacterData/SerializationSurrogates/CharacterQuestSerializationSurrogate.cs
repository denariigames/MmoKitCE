using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class CharacterQuestSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            CharacterQuest data = (CharacterQuest)obj;
            info.AddValue("dataId", data.dataId);
            info.AddValue("randomTasksIndex", data.randomTasksIndex);
            info.AddValue("isComplete", data.isComplete);
            info.AddValue("completeTime", data.completeTime);
            info.AddValue("isTracking", data.isTracking);
            info.AddValue("killedMonsters", data.killedMonsters);
            info.AddValue("completedTasks", data.completedTasks);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            CharacterQuest data = (CharacterQuest)obj;
            data.dataId = info.GetInt32("dataId");
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.randomTasksIndex = info.GetByte("randomTasksIndex");
            }
            catch { }
            data.isComplete = info.GetBoolean("isComplete");
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.completeTime = info.GetInt64("completeTime");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.isTracking = info.GetBoolean("isTracking");
            }
            catch { }
            data.killedMonsters = (Dictionary<int, int>)info.GetValue("killedMonsters", typeof(Dictionary<int, int>));
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.completedTasks = (List<int>)info.GetValue("completedTasks", typeof(List<int>));
            }
            catch { }
            obj = data;
            return obj;
        }
    }
}
