#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using MySqlConnector;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private bool GetCharacterCurrency(MySqlDataReader reader, out CharacterCurrency result)
        {
            if (reader.Read())
            {
                result = new CharacterCurrency();
                result.dataId = reader.GetInt32(0);
                result.amount = reader.GetInt32(1);
                return true;
            }
            result = CharacterCurrency.Empty;
            return false;
        }

        public async UniTask CreateCharacterCurrency(MySqlConnection connection, MySqlTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterCurrency characterCurrency)
        {
            string id = ZString.Concat(characterId, "_", characterCurrency.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Currency {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            await ExecuteNonQuery(connection, transaction, "INSERT INTO charactercurrency (id, characterId, dataId, amount) VALUES (@id, @characterId, @dataId, @amount)",
                new MySqlParameter("@id", id),
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@dataId", characterCurrency.dataId),
                new MySqlParameter("@amount", characterCurrency.amount));
        }

        public async UniTask<List<CharacterCurrency>> ReadCharacterCurrencies(string characterId, List<CharacterCurrency> result = null)
        {
            if (result == null)
                result = new List<CharacterCurrency>();
            await ExecuteReader((reader) =>
            {
                CharacterCurrency tempCurrency;
                while (GetCharacterCurrency(reader, out tempCurrency))
                {
                    result.Add(tempCurrency);
                }
            }, "SELECT dataId, amount FROM charactercurrency WHERE characterId=@characterId ORDER BY id ASC",
                new MySqlParameter("@characterId", characterId));
            return result;
        }

        public async UniTask DeleteCharacterCurrencies(MySqlConnection connection, MySqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM charactercurrency WHERE characterId=@characterId", new MySqlParameter("@characterId", characterId));
        }
    }
}
#endif