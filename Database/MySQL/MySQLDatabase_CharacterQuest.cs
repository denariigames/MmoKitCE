#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using MySqlConnector;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private bool GetCharacterQuest(MySqlDataReader reader, out CharacterQuest result)
        {
            if (reader.Read())
            {
                result = new CharacterQuest();
                result.dataId = reader.GetInt32(0);
                result.randomTasksIndex = reader.GetByte(1);
                result.isComplete = reader.GetBoolean(2);
                result.completeTime = reader.GetInt64(3);
                result.isTracking = reader.GetBoolean(4);
                result.ReadKilledMonsters(reader.GetString(5));
                result.ReadCompletedTasks(reader.GetString(6));
                return true;
            }
            result = CharacterQuest.Empty;
            return false;
        }

        public async UniTask CreateCharacterQuest(MySqlConnection connection, MySqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterQuest characterQuest)
        {
            string id = ZString.Concat(characterId, "_", characterQuest.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Quest {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterquest (id, characterId, dataId, randomTasksIndex, isComplete, completeTime, isTracking, killedMonsters, completedTasks) VALUES (@id, @characterId, @dataId, @randomTasksIndex, @isComplete, @completeTime, @isTracking, @killedMonsters, @completedTasks)",
                new MySqlParameter("@id", id),
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@dataId", characterQuest.dataId),
                new MySqlParameter("@randomTasksIndex", characterQuest.randomTasksIndex),
                new MySqlParameter("@isComplete", characterQuest.isComplete),
                new MySqlParameter("@completeTime", characterQuest.completeTime),
                new MySqlParameter("@isTracking", characterQuest.isTracking),
                new MySqlParameter("@killedMonsters", characterQuest.WriteKilledMonsters()),
                new MySqlParameter("@completedTasks", characterQuest.WriteCompletedTasks()));
        }

        public async UniTask<List<CharacterQuest>> ReadCharacterQuests(string characterId, List<CharacterQuest> result = null)
        {
            if (result == null)
                result = new List<CharacterQuest>();
            await ExecuteReader((reader) =>
            {
                CharacterQuest tempQuest;
                while (GetCharacterQuest(reader, out tempQuest))
                {
                    result.Add(tempQuest);
                }
            }, "SELECT dataId, randomTasksIndex, isComplete, completeTime, isTracking, killedMonsters, completedTasks FROM characterquest WHERE characterId=@characterId ORDER BY id ASC",
                new MySqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterQuests(MySqlConnection connection, MySqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterquest WHERE characterId=@characterId", new MySqlParameter("@characterId", characterId));
        }
    }
}
#endif