#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
#endif

namespace MultiplayerARPG.MMO
{
    public abstract partial class BaseDatabase : IDatabase, IDatabaseLogging
    {
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
        public const byte AUTH_TYPE_NORMAL = 1;
        public IDatabaseUserLogin UserLoginManager { get; set; }
        protected delegate UniTask MigrationAction();
        protected MigrationAction _doMigrationAction;

        public virtual void Initialize() { }
        public virtual void Destroy() { }
        public virtual UniTask DoMigration()
        {
            return UniTask.CompletedTask;
        }

        public abstract UniTask<string> ValidateUserLogin(string username, string password);
        public abstract UniTask<bool> ValidateAccessToken(string userId, string accessToken);
        public abstract UniTask<bool> ValidateEmailVerification(string userId);
        public abstract UniTask<long> FindEmail(string email);
        public abstract UniTask<byte> GetUserLevel(string userId);
        public abstract UniTask<int> GetGold(string userId);
        public abstract UniTask<int> ChangeGold(string userId, int amount);
        public abstract UniTask<int> GetCash(string userId);
        public abstract UniTask<int> ChangeCash(string userId, int amount);
        public abstract UniTask UpdateAccessToken(string userId, string accessToken);
        public abstract UniTask CreateUserLogin(string username, string password, string email);
        public abstract UniTask<long> FindUsername(string username);
        public abstract UniTask<long> GetUserUnbanTime(string userId);
        public abstract UniTask SetUserUnbanTimeByCharacterName(string characterName, long unbanTime);
        public abstract UniTask SetCharacterUnmuteTimeByName(string characterName, long unmuteTime);

        public abstract UniTask CreateCharacter(string userId, IPlayerCharacterData characterData);
        public abstract UniTask<PlayerCharacterData> GetCharacter(
            string id,
            bool withEquipWeapons = true,
            bool withAttributes = true,
            bool withSkills = true,
            bool withSkillUsages = true,
            bool withBuffs = true,
            bool withEquipItems = true,
            bool withNonEquipItems = true,
            bool withSummons = true,
            bool withHotkeys = true,
            bool withQuests = true,
            bool withCurrencies = true,
            bool withServerCustomData = true,
            bool withPrivateCustomData = true,
            bool withPublicCustomData = true);
        public abstract UniTask<List<PlayerCharacterData>> GetCharacters(string userId);
        public abstract UniTask UpdateCharacter(TransactionUpdateCharacterState state, IPlayerCharacterData character, List<CharacterBuff> summonBuffs, bool deleteStorageReservation);
        public abstract UniTask DeleteCharacter(string userId, string id);
        public abstract UniTask<List<CharacterBuff>> GetSummonBuffs(string characterId);
        public abstract UniTask<long> FindCharacterName(string characterName);
        public abstract UniTask<List<SocialCharacterData>> FindCharacters(string finderId, string characterName, int skip, int limit);
        public abstract UniTask CreateFriend(string id1, string id2, byte state);
        public abstract UniTask DeleteFriend(string id1, string id2);
        public abstract UniTask<List<SocialCharacterData>> GetFriends(string id, bool readById2, byte state, int skip, int limit);
        public abstract UniTask<int> GetFriendRequestNotification(string characterId);
        public abstract UniTask<string> GetIdByCharacterName(string characterName);
        public abstract UniTask<string> GetUserIdByCharacterName(string characterName);

        public abstract UniTask CreateBuilding(string channel, string mapName, IBuildingSaveData saveData);
        public abstract UniTask<List<BuildingSaveData>> GetBuildings(string channel, string mapName);
        public abstract UniTask UpdateBuilding(string channel, string mapName, IBuildingSaveData building);
        public abstract UniTask DeleteBuilding(string channel, string mapName, string id);

        public abstract UniTask<int> CreateParty(bool shareExp, bool shareItem, string leaderId);
        public abstract UniTask<PartyData> GetParty(int id);
        public abstract UniTask UpdatePartyLeader(int id, string leaderId);
        public abstract UniTask UpdateParty(int id, bool shareExp, bool shareItem);
        public abstract UniTask DeleteParty(int id);
        public abstract UniTask UpdateCharacterParty(string characterId, int partyId);

        public abstract UniTask<int> CreateGuild(string guildName, string leaderId);
        public abstract UniTask<GuildData> GetGuild(int id, IEnumerable<GuildRoleData> defaultGuildRoles);
        public abstract UniTask UpdateGuildLevel(int id, int level, int exp, int skillPoint);
        public abstract UniTask UpdateGuildLeader(int id, string leaderId);
        public abstract UniTask UpdateGuildMessage(int id, string guildMessage);
        public abstract UniTask UpdateGuildMessage2(int id, string guildMessage);
        public abstract UniTask UpdateGuildScore(int id, int score);
        public abstract UniTask UpdateGuildOptions(int id, string options);
        public abstract UniTask UpdateGuildAutoAcceptRequests(int id, bool autoAcceptRequests);
        public abstract UniTask UpdateGuildRank(int id, int rank);
        public abstract UniTask UpdateGuildRole(int id, byte guildRole, GuildRoleData guildRoleData);
        public abstract UniTask UpdateGuildMemberRole(string characterId, byte guildRole);
        public abstract UniTask UpdateGuildSkillLevel(int id, int dataId, int skillLevel, int skillPoint);
        public abstract UniTask DeleteGuild(int id);
        public abstract UniTask<long> FindGuildName(string guildName);
        public abstract UniTask UpdateCharacterGuild(string characterId, int guildId, byte guildRole);
        public abstract UniTask<int> GetGuildGold(int guildId);
        public abstract UniTask<int> ChangeGuildGold(int guildId, int gold);
        public abstract UniTask<List<GuildListEntry>> FindGuilds(string finderId, string guildName, int skip, int limit);
        public abstract UniTask CreateGuildRequest(int guildId, string requesterId);
        public abstract UniTask DeleteGuildRequest(int guildId, string requesterId);
        public abstract UniTask<List<SocialCharacterData>> GetGuildRequests(int guildId, int skip, int limit);
        public abstract UniTask<int> GetGuildRequestsNotification(int guildId);
        public abstract UniTask UpdateGuildMemberCount(int guildId, int maxGuildMembers);

        public abstract UniTask UpdateStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> storageCharacterItems);
        public abstract UniTask UpdateStorageAndCharacterItems(
            StorageType storageType,
            string storageOwnerId,
            List<CharacterItem> storageItems,
            string characterId,
            List<EquipWeapons> selectableWeaponSets,
            List<CharacterItem> equipItems,
            List<CharacterItem> nonEquipItems);
        public abstract UniTask<List<CharacterItem>> GetStorageItems(StorageType storageType, string storageOwnerId);

        public abstract UniTask<long> FindReservedStorage(StorageType storageType, string storageOwnerId);
        public abstract UniTask UpdateReservedStorage(StorageType storageType, string storageOwnerId, string reserverId);
        public abstract UniTask DeleteReservedStorage(StorageType storageType, string storageOwnerId);
        public abstract UniTask DeleteReservedStorageByReserver(string reserverId);
        public abstract UniTask DeleteAllReservedStorage();

        public abstract UniTask<List<MailListEntry>> MailList(string userId, bool onlyNewMails);
        public abstract UniTask<Mail> GetMail(string mailId, string userId);
        public abstract UniTask<long> UpdateReadMailState(string mailId, string userId);
        public abstract UniTask<long> UpdateClaimMailItemsState(string mailId, string userId);
        public abstract UniTask<long> UpdateDeleteMailState(string mailId, string userId);
        public abstract UniTask<int> CreateMail(Mail mail);
        public abstract UniTask<int> GetMailNotification(string userId);

        public abstract UniTask UpdateUserCount(int userCount);
#endif
    }
}