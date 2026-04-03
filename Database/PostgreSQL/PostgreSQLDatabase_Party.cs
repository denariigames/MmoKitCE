#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public const string CACHE_KEY_INSERT_PARTY = "INSERT_PARTY";
        public const string CACHE_KEY_INSERT_PARTY_UPDATE = "INSERT_PARTY_UPDATE";
        public override async UniTask<int> CreateParty(bool shareExp, bool shareItem, string leaderId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            int id = (int)await PostgreSQLHelpers.ExecuteInsertScalar(
                CACHE_KEY_INSERT_PARTY,
                connection,
                "parties",
                new[] {
                    new PostgreSQLHelpers.ColumnInfo("share_exp", shareExp),
                    new PostgreSQLHelpers.ColumnInfo("share_item", shareItem),
                    new PostgreSQLHelpers.ColumnInfo("leader_id", leaderId),
                }, "id");
            if (id <= 0)
                return id;
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_INSERT_PARTY_UPDATE,
                connection, null,
                "characters",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("party_id", id),
                },
                PostgreSQLHelpers.WhereEqualTo("id", leaderId));
            return id;
        }

        public const string CACHE_KEY_GET_PARTY_PARTIES = "GET_PARTY_PARTIES";
        public const string CACHE_KEY_GET_PARTY_MEMBERS = "GET_PARTY_MEMBERS";
        public override async UniTask<PartyData> GetParty(int id)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var readerParty = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_GET_PARTY_PARTIES,
                connection,
                "parties", "share_exp, share_item, leader_id", "LIMIT 1",
                PostgreSQLHelpers.WhereEqualTo("id", id));
            PartyData result = null;
            if (readerParty.Read())
            {
                result = new PartyData(id,
                    readerParty.GetBoolean(0),
                    readerParty.GetBoolean(1),
                    readerParty.GetString(2));
            }
            readerParty.Dispose();
            if (result == null)
                return null;
            // Party members
            var readerMembers = await PostgreSQLHelpers.ExecuteSelect(
                CACHE_KEY_GET_PARTY_MEMBERS,
                connection,
                "characters", "id, data_id, character_name, level",
                PostgreSQLHelpers.WhereEqualTo("party_id", id));
            SocialCharacterData partyMemberData;
            while (readerMembers.Read())
            {
                // Get some required data, other data will be set at server side
                partyMemberData = new SocialCharacterData();
                partyMemberData.id = readerMembers.GetString(0);
                partyMemberData.dataId = readerMembers.GetInt32(1);
                partyMemberData.characterName = readerMembers.GetString(2);
                partyMemberData.level = readerMembers.GetInt32(3);
                result.AddMember(partyMemberData);
            }
            readerMembers.Dispose();
            return result;
        }

        public const string CACHE_KEY_UPDATE_PARTY_LEADER = "UPDATE_PARTY_LEADER";
        public override async UniTask UpdatePartyLeader(int id, string leaderId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_PARTY_LEADER,
                connection, null,
                "parties",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("leader_id", leaderId),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_UPDATE_PARTY = "UPDATE_PARTY";
        public override async UniTask UpdateParty(int id, bool shareExp, bool shareItem)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_PARTY,
                connection, null,
                "parties",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("share_exp", shareExp),
                    new PostgreSQLHelpers.ColumnInfo("share_item", shareItem),
                },
                PostgreSQLHelpers.WhereEqualTo("id", id));
        }

        public const string CACHE_KEY_DELETE_PARTY_PARTY = "DELETE_PARTY_PARTY";
        public const string CACHE_KEY_DELETE_PARTY_CHARACTER = "DELETE_PARTY_CHARACTER";
        public override async UniTask DeleteParty(int id)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await PostgreSQLHelpers.ExecuteDelete(
                    CACHE_KEY_DELETE_PARTY_PARTY,
                    connection, transaction,
                    "parties",
                    PostgreSQLHelpers.WhereEqualTo("id", id));
                await PostgreSQLHelpers.ExecuteUpdate(
                    CACHE_KEY_DELETE_PARTY_CHARACTER,
                    connection, transaction,
                    "characters",
                    new[]
                    {
                        new PostgreSQLHelpers.ColumnInfo("party_id", 0),
                    },
                    PostgreSQLHelpers.WhereEqualTo("party_id", id));
                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while delete party: " + id);
                LogException(LogTag, ex);
                await transaction.RollbackAsync();
            }
        }

        public const string CACHE_KEY_UPDATE_CHARACTER_PARTY = "UPDATE_CHARACTER_PARTY";
        public override async UniTask UpdateCharacterParty(string characterId, int partyId)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            await PostgreSQLHelpers.ExecuteUpdate(
                CACHE_KEY_UPDATE_CHARACTER_PARTY,
                connection, null,
                "characters",
                new[]
                {
                    new PostgreSQLHelpers.ColumnInfo("party_id", partyId),
                },
                PostgreSQLHelpers.WhereEqualTo("id", characterId));
        }
    }
}
#endif