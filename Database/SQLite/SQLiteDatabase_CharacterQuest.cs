#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using System.Collections.Generic;
using Cysharp.Text;

namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        private bool GetCharacterQuest(SqliteDataReader reader, out CharacterQuest result)
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

        public void CreateCharacterQuest(SqliteTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, CharacterQuest characterQuest)
        {
            string id = ZString.Concat(characterId, "_", characterQuest.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Quest {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, "INSERT INTO characterquest (id, idx, characterId, dataId, randomTasksIndex, isComplete, completeTime, isTracking, killedMonsters, completedTasks) VALUES (@id, @idx, @characterId, @dataId, @randomTasksIndex, @isComplete, @completeTime, @isTracking, @killedMonsters, @completedTasks)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@idx", idx),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@dataId", characterQuest.dataId),
                new SqliteParameter("@randomTasksIndex", characterQuest.randomTasksIndex),
                new SqliteParameter("@isComplete", characterQuest.isComplete),
                new SqliteParameter("@completeTime", characterQuest.completeTime),
                new SqliteParameter("@isTracking", characterQuest.isTracking),
                new SqliteParameter("@killedMonsters", characterQuest.WriteKilledMonsters()),
                new SqliteParameter("@completedTasks", characterQuest.WriteCompletedTasks()));
        }

        public List<CharacterQuest> ReadCharacterQuests(string characterId, List<CharacterQuest> result = null)
        {
            if (result == null)
                result = new List<CharacterQuest>();
            ExecuteReader((reader) =>
            {
                CharacterQuest tempQuest;
                while (GetCharacterQuest(reader, out tempQuest))
                {
                    result.Add(tempQuest);
                }
            }, "SELECT dataId, randomTasksIndex, isComplete, completeTime, isTracking, killedMonsters, completedTasks FROM characterquest WHERE characterId=@characterId ORDER BY id ASC",
                new SqliteParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterQuests(SqliteTransaction transaction, string characterId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM characterquest WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }
    }
}
#endif