#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using MySqlConnector;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        public async UniTask CreateOrUpdateCharacterMount(MySqlConnection connection, MySqlTransaction transaction, string characterId, CharacterMount characterMount)
        {
            await ExecuteNonQuery(connection, transaction, @"INSERT INTO charactermount 
                (id, type, sourceId, mountRemainsDuration, level, currentHp) VALUES 
                (@id, @type, @sourceId, @mountRemainsDuration, @level, @currentHp)
                ON DUPLICATE KEY UPDATE
                type = @type,
                sourceId = @sourceId,
                mountRemainsDuration = @mountRemainsDuration,
                level = @level,
                currentHp = @currentHp",
                new MySqlParameter("@id", characterId),
                new MySqlParameter("@type", (byte)characterMount.type),
                new MySqlParameter("@sourceId", characterMount.sourceId),
                new MySqlParameter("@mountRemainsDuration", characterMount.mountRemainsDuration),
                new MySqlParameter("@level", characterMount.level),
                new MySqlParameter("@currentHp", characterMount.currentHp));
        }

        public async UniTask DeleteCharacterMount(MySqlConnection connection, MySqlTransaction transaction, string characterId)
        {
            await ExecuteNonQuery(connection, transaction, "DELETE FROM charactermount WHERE id=@id", new MySqlParameter("@id", characterId));
        }
    }
}
#endif