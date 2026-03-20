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
        private bool GetCharacterCurrency(SqliteDataReader reader, out CharacterCurrency result)
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

        public void CreateCharacterCurrency(SqliteTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, CharacterCurrency characterCurrency)
        {
            string id = ZString.Concat(characterId, "_", characterCurrency.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Currency {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, "INSERT INTO charactercurrency (id, idx, characterId, dataId, amount) VALUES (@id, @idx, @characterId, @dataId, @amount)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@idx", idx),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@dataId", characterCurrency.dataId),
                new SqliteParameter("@amount", characterCurrency.amount));
        }

        public List<CharacterCurrency> ReadCharacterCurrencies(string characterId, List<CharacterCurrency> result = null)
        {
            if (result == null)
                result = new List<CharacterCurrency>();
            ExecuteReader((reader) =>
            {
                CharacterCurrency tempCurrency;
                while (GetCharacterCurrency(reader, out tempCurrency))
                {
                    result.Add(tempCurrency);
                }
            }, "SELECT dataId, amount FROM charactercurrency WHERE characterId=@characterId ORDER BY id ASC",
                new SqliteParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterCurrencies(SqliteTransaction transaction, string characterId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM charactercurrency WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }
    }
}
#endif