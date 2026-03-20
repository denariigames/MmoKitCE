using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial interface IDatabase
    {
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
        UniTask DoMigration();
        UniTask<string> ValidateUserLogin(string username, string password);
        UniTask<bool> ValidateAccessToken(string userId, string accessToken);
        UniTask<bool> ValidateEmailVerification(string userId);
        UniTask<long> FindEmail(string email);
        UniTask<byte> GetUserLevel(string userId);
        UniTask<int> GetGold(string userId);
        UniTask<int> ChangeGold(string userId, int amount);
        UniTask<int> GetCash(string userId);
        UniTask<int> ChangeCash(string userId, int amount);
        UniTask UpdateAccessToken(string userId, string accessToken);
        UniTask CreateUserLogin(string username, string password, string email);
        UniTask<long> FindUsername(string username);
        UniTask<long> GetUserUnbanTime(string userId);
        UniTask SetUserUnbanTimeByCharacterName(string characterName, long unbanTime);
        UniTask SetCharacterUnmuteTimeByName(string characterName, long unmuteTime);

        UniTask CreateCharacter(string userId, IPlayerCharacterData characterData);
        UniTask<PlayerCharacterData> GetCharacter(
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
        UniTask<List<PlayerCharacterData>> GetCharacters(string userId);
        UniTask UpdateCharacter(TransactionUpdateCharacterState state, IPlayerCharacterData character, List<CharacterBuff> summonBuffs, bool deleteStorageReservation);
        UniTask DeleteCharacter(string userId, string id);
        UniTask<List<CharacterBuff>> GetSummonBuffs(string characterId);
        UniTask<long> FindCharacterName(string characterName);
        UniTask<List<SocialCharacterData>> FindCharacters(string finderId, string characterName, int skip, int limit);
        UniTask CreateFriend(string id1, string id2, byte state);
        UniTask DeleteFriend(string id1, string id2);
        UniTask<List<SocialCharacterData>> GetFriends(string id, bool readById2, byte state, int skip, int limit);
        UniTask<int> GetFriendRequestNotification(string characterId);
        UniTask<string> GetIdByCharacterName(string characterName);
        UniTask<string> GetUserIdByCharacterName(string characterName);

        UniTask CreateBuilding(string channel, string mapName, IBuildingSaveData saveData);
        UniTask<List<BuildingSaveData>> GetBuildings(string channel, string mapName);
        UniTask UpdateBuilding(string channel, string mapName, IBuildingSaveData building);
        UniTask DeleteBuilding(string channel, string mapName, string id);

        UniTask<int> CreateParty(bool shareExp, bool shareItem, string leaderId);
        UniTask<PartyData> GetParty(int id);
        UniTask UpdatePartyLeader(int id, string leaderId);
        UniTask UpdateParty(int id, bool shareExp, bool shareItem);
        UniTask DeleteParty(int id);
        UniTask UpdateCharacterParty(string characterId, int partyId);

        UniTask<int> CreateGuild(string guildName, string leaderId);
        UniTask<GuildData> GetGuild(int id, IEnumerable<GuildRoleData> defaultGuildRoles);
        UniTask UpdateGuildLevel(int id, int level, int exp, int skillPoint);
        UniTask UpdateGuildLeader(int id, string leaderId);
        UniTask UpdateGuildMessage(int id, string guildMessage);
        UniTask UpdateGuildMessage2(int id, string guildMessage);
        UniTask UpdateGuildScore(int id, int score);
        UniTask UpdateGuildOptions(int id, string options);
        UniTask UpdateGuildAutoAcceptRequests(int id, bool autoAcceptRequests);
        UniTask UpdateGuildRank(int id, int rank);
        UniTask UpdateGuildRole(int id, byte guildRole, GuildRoleData guildRoleData);
        UniTask UpdateGuildMemberRole(string characterId, byte guildRole);
        UniTask UpdateGuildSkillLevel(int id, int dataId, int skillLevel, int skillPoint);
        UniTask DeleteGuild(int id);
        UniTask<long> FindGuildName(string guildName);
        UniTask UpdateCharacterGuild(string characterId, int guildId, byte guildRole);
        UniTask<int> GetGuildGold(int guildId);
        UniTask<int> ChangeGuildGold(int guildId, int gold);
        UniTask<List<GuildListEntry>> FindGuilds(string finderId, string guildName, int skip, int limit);
        UniTask CreateGuildRequest(int guildId, string requesterId);
        UniTask DeleteGuildRequest(int guildId, string requesterId);
        UniTask<List<SocialCharacterData>> GetGuildRequests(int guildId, int skip, int limit);
        UniTask<int> GetGuildRequestsNotification(int guildId);
        UniTask UpdateGuildMemberCount(int guildId, int maxGuildMembers);

        UniTask UpdateStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> storageCharacterItems);
        UniTask UpdateStorageAndCharacterItems(
            StorageType storageType,
            string storageOwnerId,
            List<CharacterItem> storageItems,
            string characterId,
            List<EquipWeapons> selectableWeaponSets,
            List<CharacterItem> equipItems,
            List<CharacterItem> nonEquipItems);
        UniTask<List<CharacterItem>> GetStorageItems(StorageType storageType, string storageOwnerId);

        UniTask<long> FindReservedStorage(StorageType storageType, string storageOwnerId);
        UniTask UpdateReservedStorage(StorageType storageType, string storageOwnerId, string reserverId);
        UniTask DeleteReservedStorage(StorageType storageType, string storageOwnerId);
        UniTask DeleteReservedStorageByReserver(string reserverId);
        UniTask DeleteAllReservedStorage();

        UniTask<List<MailListEntry>> MailList(string userId, bool onlyNewMails);
        UniTask<Mail> GetMail(string mailId, string userId);
        UniTask<long> UpdateReadMailState(string mailId, string userId);
        UniTask<long> UpdateClaimMailItemsState(string mailId, string userId);
        UniTask<long> UpdateDeleteMailState(string mailId, string userId);
        UniTask<int> CreateMail(Mail mail);
        UniTask<int> GetMailNotification(string userId);

        UniTask UpdateUserCount(int userCount);
#endif
    }
}
