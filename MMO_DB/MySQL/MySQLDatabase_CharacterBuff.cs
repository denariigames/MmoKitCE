#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using MySqlConnector;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private bool GetCharacterBuff(MySqlDataReader reader, out CharacterBuff result)
        {
            if (reader.Read())
            {
                result = new CharacterBuff();
                result.id = reader.GetString(0);
                result.type = (BuffType)reader.GetByte(1);
                result.dataId = reader.GetInt32(2);
                result.level = reader.GetInt32(3);
                result.buffRemainsDuration = reader.GetFloat(4);
                return true;
            }
            result = CharacterBuff.Empty;
            return false;
        }

        public async UniTask CreateCharacterBuff(MySqlConnection connection, MySqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterBuff characterBuff)
        {
            string id = characterBuff.id;
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Buff {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO characterbuff (id, characterId, type, dataId, level, buffRemainsDuration) VALUES (@id, @characterId, @type, @dataId, @level, @buffRemainsDuration)",
                new MySqlParameter("@id", id),
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@type", (byte)characterBuff.type),
                new MySqlParameter("@dataId", characterBuff.dataId),
                new MySqlParameter("@level", characterBuff.level),
                new MySqlParameter("@buffRemainsDuration", characterBuff.buffRemainsDuration));
        }

        public async UniTask<List<CharacterBuff>> ReadCharacterBuffs(string characterId, List<CharacterBuff> result = null)
        {
            if (result == null)
                result = new List<CharacterBuff>();
            await ExecuteReader((reader) =>
            {
                CharacterBuff tempBuff;
                while (GetCharacterBuff(reader, out tempBuff))
                {
                    result.Add(tempBuff);
                }
            }, "SELECT id, type, dataId, level, buffRemainsDuration FROM characterbuff WHERE characterId=@characterId ORDER BY buffRemainsDuration ASC",
                new MySqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterBuffs(MySqlConnection connection, MySqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM characterbuff WHERE characterId=@characterId", new MySqlParameter("@characterId", characterId));
        }
    }
}
#endif