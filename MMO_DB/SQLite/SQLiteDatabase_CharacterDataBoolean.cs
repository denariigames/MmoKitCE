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
        private bool GetCharacterDataBoolean(SqliteDataReader reader, out CharacterDataBoolean result)
        {
            if (reader.Read())
            {
                result = new CharacterDataBoolean();
                result.hashedKey = reader.GetInt32(0);
                result.value = reader.GetBoolean(1);
                return true;
            }
            result = default;
            return false;
        }

        public void CreateCharacterDataBoolean(SqliteTransaction transaction, string tableName, HashSet<string> insertedIds, string characterId, CharacterDataBoolean characterDataBoolean)
        {
            string id = ZString.Concat(characterId, "_", characterDataBoolean.hashedKey);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Custom Boolean {id}, for character {characterId}, already inserted to table {tableName}");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, $"INSERT INTO {tableName} (id, characterId, hashedKey, value) VALUES (@id, @characterId, @hashedKey, @value)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@hashedKey", characterDataBoolean.hashedKey),
                new SqliteParameter("@value", characterDataBoolean.value));
        }

        public List<CharacterDataBoolean> ReadCharacterDataBooleans(string tableName, string characterId, List<CharacterDataBoolean> result = null)
        {
            if (result == null)
                result = new List<CharacterDataBoolean>();
            ExecuteReader((reader) =>
            {
                CharacterDataBoolean tempData;
                while (GetCharacterDataBoolean(reader, out tempData))
                {
                    result.Add(tempData);
                }
            }, $"SELECT hashedKey, value FROM {tableName} WHERE characterId=@characterId",
                new SqliteParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterDataBooleans(SqliteTransaction transaction, string tableName, string characterId)
        {
            ExecuteNonQuery(transaction, $"DELETE FROM {tableName} WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }
    }
}
#endif