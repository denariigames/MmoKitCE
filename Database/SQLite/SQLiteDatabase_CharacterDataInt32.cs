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
        private bool GetCharacterDataInt32(SqliteDataReader reader, out CharacterDataInt32 result)
        {
            if (reader.Read())
            {
                result = new CharacterDataInt32();
                result.hashedKey = reader.GetInt32(0);
                result.value = reader.GetInt32(1);
                return true;
            }
            result = default;
            return false;
        }

        public void CreateCharacterDataInt32(SqliteTransaction transaction, string tableName, HashSet<string> insertedIds, string characterId, CharacterDataInt32 characterDataInt32)
        {
            string id = ZString.Concat(characterId, "_", characterDataInt32.hashedKey);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Custom Int32 {id}, for character {characterId}, already inserted to table {tableName}");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, $"INSERT INTO {tableName} (id, characterId, hashedKey, value) VALUES (@id, @characterId, @hashedKey, @value)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@hashedKey", characterDataInt32.hashedKey),
                new SqliteParameter("@value", characterDataInt32.value));
        }

        public List<CharacterDataInt32> ReadCharacterDataInt32s(string tableName, string characterId, List<CharacterDataInt32> result = null)
        {
            if (result == null)
                result = new List<CharacterDataInt32>();
            ExecuteReader((reader) =>
            {
                CharacterDataInt32 tempData;
                while (GetCharacterDataInt32(reader, out tempData))
                {
                    result.Add(tempData);
                }
            }, $"SELECT hashedKey, value FROM {tableName} WHERE characterId=@characterId",
                new SqliteParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterDataInt32s(SqliteTransaction transaction, string tableName, string characterId)
        {
            ExecuteNonQuery(transaction, $"DELETE FROM {tableName} WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }
    }
}
#endif