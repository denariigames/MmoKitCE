#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        public void CreateOrUpdateCharacterMount(SqliteTransaction transaction, string characterId, CharacterMount characterMount)
        {
            ExecuteNonQuery(transaction, @"INSERT INTO charactermount 
                (id, type, sourceId, mountRemainsDuration, level, currentHp) VALUES 
                (@id, @type, @sourceId, @mountRemainsDuration, @level, @currentHp)
                ON CONFLICT(id) DO UPDATE SET
                type = @type,
                sourceId = @sourceId,
                mountRemainsDuration = @mountRemainsDuration,
                level = @level,
                currentHp = @currentHp",
                new SqliteParameter("@id", characterId),
                new SqliteParameter("@type", (byte)characterMount.type),
                new SqliteParameter("@sourceId", characterMount.sourceId != null ? characterMount.sourceId : string.Empty),
                new SqliteParameter("@mountRemainsDuration", characterMount.mountRemainsDuration),
                new SqliteParameter("@level", characterMount.level),
                new SqliteParameter("@currentHp", characterMount.currentHp));
        }

        public void DeleteCharacterMount(SqliteTransaction transaction, string characterId)
        {
            ExecuteNonQuery(transaction, "DELETE FROM charactermount WHERE id=@id", new SqliteParameter("@id", characterId));
        }
    }
}
#endif