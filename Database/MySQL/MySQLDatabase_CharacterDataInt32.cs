#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using MySqlConnector;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private bool GetCharacterDataInt32(MySqlDataReader reader, out CharacterDataInt32 result)
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

        public async UniTask CreateCharacterDataInt32(MySqlConnection connection, MySqlTransaction transaction, string tableName, HashSet<string> insertedIds, string characterId, CharacterDataInt32 characterDataInt32)
        {
            string id = ZString.Concat(characterId, "_", characterDataInt32.hashedKey);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Custom Int32 {id}, for character {characterId}, already inserted to table {tableName}");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, $"INSERT INTO {tableName} (id, characterId, hashedKey, value) VALUES (@id, @characterId, @hashedKey, @value)",
                new MySqlParameter("@id", id),
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@hashedKey", characterDataInt32.hashedKey),
                new MySqlParameter("@value", characterDataInt32.value));
        }

        public async UniTask<List<CharacterDataInt32>> ReadCharacterDataInt32s(string tableName, string characterId, List<CharacterDataInt32> result = null)
        {
            if (result == null)
                result = new List<CharacterDataInt32>();
            await ExecuteReader((reader) =>
            {
                CharacterDataInt32 tempData;
                while (GetCharacterDataInt32(reader, out tempData))
                {
                    result.Add(tempData);
                }
            }, $"SELECT hashedKey, value FROM {tableName} WHERE characterId=@characterId",
                new MySqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterDataInt32s(MySqlConnection connection, MySqlTransaction transaction, string tableName, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, $"DELETE FROM {tableName} WHERE characterId=@characterId", new MySqlParameter("@characterId", characterId));
        }
    }
}
#endif