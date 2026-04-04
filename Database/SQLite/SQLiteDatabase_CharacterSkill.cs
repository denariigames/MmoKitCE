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
        private bool GetCharacterSkill(SqliteDataReader reader, out CharacterSkill result)
        {
            if (reader.Read())
            {
                result = new CharacterSkill();
                result.dataId = reader.GetInt32(0);
                result.level = reader.GetInt32(1);
                return true;
            }
            result = CharacterSkill.Empty;
            return false;
        }

        public void CreateCharacterSkill(SqliteTransaction transaction, HashSet<string> insertedIds, int idx, string characterId, CharacterSkill characterSkill)
        {
            string id = ZString.Concat(characterId, "_", characterSkill.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Skill {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, "INSERT INTO characterskill (id, idx, characterId, dataId, level, coolDownRemainsDuration) VALUES (@id, @idx, @characterId, @dataId, @level, 0)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@idx", idx),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@dataId", characterSkill.dataId),
                new SqliteParameter("@level", characterSkill.level));
        }

        public List<CharacterSkill> ReadCharacterSkills(string characterId, List<CharacterSkill> result = null)
        {
            if (result == null)
                result = new List<CharacterSkill>();
            ExecuteReader((reader) =>
            {
                CharacterSkill tempSkill;
                while (GetCharacterSkill(reader, out tempSkill))
                {
                    result.Add(tempSkill);
                }
            }, "SELECT dataId, level FROM characterskill WHERE characterId=@characterId ORDER BY id ASC",
                new SqliteParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterSkills(SqliteTransaction transaction, string characterId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM characterskill WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }
    }
}
#endif