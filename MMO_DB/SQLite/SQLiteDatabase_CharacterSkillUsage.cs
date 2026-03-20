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
        private bool GetCharacterSkillUsage(SqliteDataReader reader, out CharacterSkillUsage result)
        {
            if (reader.Read())
            {
                result = new CharacterSkillUsage();
                result.type = (SkillUsageType)reader.GetByte(0);
                result.dataId = reader.GetInt32(1);
                result.coolDownRemainsDuration = reader.GetFloat(2);
                return true;
            }
            result = CharacterSkillUsage.Empty;
            return false;
        }

        public void CreateCharacterSkillUsage(SqliteTransaction transaction, HashSet<string> insertedIds, string characterId, CharacterSkillUsage characterSkillUsage)
        {
            string id = ZString.Concat(characterId, "_", (int)characterSkillUsage.type, "_", characterSkillUsage.dataId);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Skill usage {id}, for character {characterId}, already inserted");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuery(transaction, "INSERT INTO characterskillusage (id, characterId, type, dataId, coolDownRemainsDuration) VALUES (@id, @characterId, @type, @dataId, @coolDownRemainsDuration)",
                new SqliteParameter("@id", id),
                new SqliteParameter("@characterId", characterId),
                new SqliteParameter("@type", (byte)characterSkillUsage.type),
                new SqliteParameter("@dataId", characterSkillUsage.dataId),
                new SqliteParameter("@coolDownRemainsDuration", characterSkillUsage.coolDownRemainsDuration));
        }

        public List<CharacterSkillUsage> ReadCharacterSkillUsages(string characterId, List<CharacterSkillUsage> result = null)
        {
            if (result == null)
                result = new List<CharacterSkillUsage>();
            ExecuteReader((reader) =>
            {
                CharacterSkillUsage tempSkillUsage;
                while (GetCharacterSkillUsage(reader, out tempSkillUsage))
                {
                    result.Add(tempSkillUsage);
                }
            }, "SELECT type, dataId, coolDownRemainsDuration FROM characterskillusage WHERE characterId=@characterId ORDER BY coolDownRemainsDuration ASC",
                new SqliteParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterSkillUsages(SqliteTransaction transaction, string characterId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM characterskillusage WHERE characterId=@characterId", new SqliteParameter("@characterId", characterId));
        }
    }
}
#endif