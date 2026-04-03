using Cysharp.Text;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct CharacterQuest : INetSerializable
    {
        public static readonly CharacterQuest Empty = new CharacterQuest();
        public int dataId;
        public byte randomTasksIndex;
        public bool isComplete;
        public long completeTime;
        public bool isTracking;
        public Dictionary<int, int> killedMonsters;
        public List<int> completedTasks;

        public Dictionary<int, int> ReadKilledMonsters(string killedMonstersString)
        {
            if (killedMonsters == null)
                killedMonsters = new Dictionary<int, int>();
            killedMonsters.Clear();
            string[] splitSets = killedMonstersString.Split(';');
            foreach (string set in splitSets)
            {
                if (string.IsNullOrEmpty(set))
                    continue;
                string[] splitData = set.Split(':');
                if (splitData.Length != 2)
                    continue;
                killedMonsters[int.Parse(splitData[0])] = int.Parse(splitData[1]);
            }
            return killedMonsters;
        }

        public string WriteKilledMonsters()
        {
            using (Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder(true))
            {
                if (killedMonsters != null && killedMonsters.Count > 0)
                {
                    foreach (KeyValuePair<int, int> keyValue in killedMonsters)
                    {
                        stringBuilder.Append(keyValue.Key);
                        stringBuilder.Append(':');
                        stringBuilder.Append(keyValue.Value);
                        stringBuilder.Append(';');
                    }
                }
                return stringBuilder.ToString();
            }
        }

        public List<int> ReadCompletedTasks(string completedTasksString)
        {
            if (completedTasks == null)
                completedTasks = new List<int>();
            completedTasks.Clear();
            string[] splitTexts = completedTasksString.Split(';');
            foreach (string text in splitTexts)
            {
                if (string.IsNullOrEmpty(text))
                    continue;
                completedTasks.Add(int.Parse(text));
            }
            return completedTasks;
        }

        public string WriteCompletedTasks()
        {
            using (Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder(true))
            {
                if (completedTasks != null && completedTasks.Count > 0)
                {
                    foreach (int completedTask in completedTasks)
                    {
                        stringBuilder.Append(completedTask);
                        stringBuilder.Append(';');
                    }
                }
                return stringBuilder.ToString();
            }
        }

        public CharacterQuest Clone()
        {
            CharacterQuest clone = new CharacterQuest();
            clone.dataId = dataId;
            clone.randomTasksIndex = randomTasksIndex;
            clone.isComplete = isComplete;
            clone.completeTime = completeTime;
            clone.isTracking = isTracking;
            // Clone killed monsters
            Dictionary<int, int> killedMonsters = new Dictionary<int, int>();
            if (this.killedMonsters != null && this.killedMonsters.Count > 0)
            {
                foreach (KeyValuePair<int, int> cloneEntry in this.killedMonsters)
                {
                    killedMonsters[cloneEntry.Key] = cloneEntry.Value;
                }
            }
            clone.killedMonsters = killedMonsters;
            // Clone complete tasks
            clone.completedTasks = completedTasks == null ? new List<int>() : new List<int>(completedTasks);
            return clone;
        }

        public static CharacterQuest Create(int dataId, byte randomTasksIndex)
        {
            return new CharacterQuest()
            {
                dataId = dataId,
                randomTasksIndex = randomTasksIndex,
                isComplete = false,
                killedMonsters = new Dictionary<int, int>(),
                completedTasks = new List<int>(),
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(dataId);
            writer.Put(randomTasksIndex);
            writer.Put(isComplete);
            writer.PutPackedLong(completeTime);
            writer.Put(isTracking);
            byte killedMonstersCount = 0;
            if (killedMonsters != null)
                killedMonstersCount = (byte)killedMonsters.Count;
            writer.Put(killedMonstersCount);
            if (killedMonstersCount > 0)
            {
                foreach (KeyValuePair<int, int> killedMonster in killedMonsters)
                {
                    writer.PutPackedInt(killedMonster.Key);
                    writer.PutPackedInt(killedMonster.Value);
                }
            }
            byte completedTasksCount = 0;
            if (completedTasks != null)
                completedTasksCount = (byte)completedTasks.Count;
            writer.Put(completedTasksCount);
            if (completedTasksCount > 0)
            {
                foreach (int talkedNpc in completedTasks)
                {
                    writer.PutPackedInt(talkedNpc);
                }
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            if (killedMonsters == null)
                killedMonsters = new Dictionary<int, int>();
            if (completedTasks == null)
                completedTasks = new List<int>();
            dataId = reader.GetPackedInt();
            randomTasksIndex = reader.GetByte();
            isComplete = reader.GetBool();
            completeTime = reader.GetPackedLong();
            isTracking = reader.GetBool();
            int killMonstersCount = reader.GetByte();
            killedMonsters.Clear();
            for (int i = 0; i < killMonstersCount; ++i)
            {
                killedMonsters.Add(reader.GetPackedInt(), reader.GetPackedInt());
            }
            int completedTasksCount = reader.GetByte();
            completedTasks.Clear();
            for (int i = 0; i < completedTasksCount; ++i)
            {
                completedTasks.Add(reader.GetPackedInt());
            }
        }
    }
}
