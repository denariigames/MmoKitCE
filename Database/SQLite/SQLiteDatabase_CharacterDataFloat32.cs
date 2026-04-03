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
        private bool GetCharacterDataFloat32(SqliteDataReader reader, out CharacterDataFloat32 result)
        {
            if (reader.Read())
            {
                result = new CharacterDataFloat32();
                result.hashedKey = reader.GetInt32(0);
                result.value = reader.GetFloat(1);
                return true;
            }
            result = default;
            return false;
        }

        public void CreateCharacterDataFloat32(SqliteTransaction transaction, string tableName, HashSet<string> insertedIds, string characterId, CharacterDataFloat32 characterDataFloat32)
        {
            string id = ZString.Concat(characterId, "_", characterDataFloat32.hashedKey);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Custom Float32 {id}, for character {characterId}, already inserted to table {tableName}");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, $"INSERT INTO {tableName} (id, characterId, hashedKey, value) VALUES (@id, @characterId, @hashedKey, @value)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@hashedKey", characterDataFloat32.hashedKey),
                new SqliteParameter("@value", characterDataFloat32.value));
        }

        public List<CharacterDataFloat32> ReadCharacterDataFloat32s(string tableName, string characterId, List<CharacterDataFloat32> result = null)
        {
            if (result == null)
                result = new List<CharacterDataFloat32>();
            ExecuteReader((reader) =>
            {
                CharacterDataFloat32 tempData;
                while (GetCharacterDataFloat32(reader, out tempData))
                {
                    result.Add(tempData);
                }
            }, $"SELECT hashedKey, value FROM {tableName} WHERE characterId=@characterId",
                new SqliteParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterDataFloat32s(SqliteTransaction transaction, string tableName, string characterId)
        {
            ExecuteNonQuery(transaction, $"DELETE FROM {tableName} WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }
    }
}
#endif