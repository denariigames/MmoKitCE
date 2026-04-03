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
        private bool GetCharacterSummon(SqliteDataReader reader, out CharacterSummon result)
        {
            if (reader.Read())
            {
                result = new CharacterSummon();
                result.type = (SummonType)reader.GetByte(0);
                result.sourceId = reader.GetString(1);
                result.dataId = reader.GetInt32(2);
                result.summonRemainsDuration = reader.GetFloat(3);
                result.level = reader.GetInt32(4);
                result.exp = reader.GetInt32(5);
                result.currentHp = reader.GetInt32(6);
                result.currentMp = reader.GetInt32(7);
                return true;
            }
            result = CharacterSummon.Empty;
            return false;
        }

        public void CreateCharacterSummon(SqliteTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, CharacterSummon characterSummon)
        {
            string id = ZString.Concat(characterId, "_", (int)characterSummon.type, "_", idx);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Summon {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, "INSERT INTO charactersummon (id, characterId, type, sourceId, dataId, summonRemainsDuration, level, exp, currentHp, currentMp) VALUES (@id, @characterId, @type, @sourceId, @dataId, @summonRemainsDuration, @level, @exp, @currentHp, @currentMp)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@type", (byte)characterSummon.type),
                new SqliteParameter("@sourceId", characterSummon.sourceId),
                new SqliteParameter("@dataId", characterSummon.dataId),
                new SqliteParameter("@summonRemainsDuration", characterSummon.summonRemainsDuration),
                new SqliteParameter("@level", characterSummon.level),
                new SqliteParameter("@exp", characterSummon.exp),
                new SqliteParameter("@currentHp", characterSummon.currentHp),
                new SqliteParameter("@currentMp", characterSummon.currentMp));
        }

        public List<CharacterSummon> ReadCharacterSummons(string characterId, List<CharacterSummon> result = null)
        {
            if (result == null)
                result = new List<CharacterSummon>();
            ExecuteReader((reader) =>
            {
                CharacterSummon tempSummon;
                while (GetCharacterSummon(reader, out tempSummon))
                {
                    result.Add(tempSummon);
                }
            }, "SELECT type, sourceId, dataId, summonRemainsDuration, level, exp, currentHp, currentMp FROM charactersummon WHERE characterId=@characterId ORDER BY type DESC",
                new SqliteParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterSummons(SqliteTransaction transaction, string characterId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM charactersummon WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }
    }
}
#endif