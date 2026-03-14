using System.Collections.Generic;

namespace MultiplayerARPG
{
    public partial interface IServerGameMessageHandlers
    {
        void SendGameMessage(long connectionId, UITextKeys message);
        void SendFormattedGameMessage(long connectionId, UIFormatKeys format, params string[] args);
        void NotifyRewardExp(long connectionId, RewardGivenType givenType, int exp);
        void NotifyRewardGold(long connectionId, RewardGivenType givenType, int gold);
        void NotifyRewardItem(long connectionId, RewardGivenType givenType, int dataId, int amount);
        void NotifyRewardCurrency(long connectionId, RewardGivenType givenType, int dataId, int amount);
        // Storage
        void NotifyStorageItems(long connectionId, StorageType storageType, string storageOwnerId, List<CharacterItem> storageItems);
        void NotifyStorageOpened(long connectionId, StorageType storageType, string storageOwnerId, uint objectId, int weightLimit, int slotLimit);
        void NotifyStorageClosed(long connectionId, StorageType storageType, string storageOwnerId);
        // Party
        void SendSetPartyData(long connectionId, int id, bool shareExp, bool shareItem, string leaderId);
        void SendSetPartyLeader(long connectionId, int id, string leaderId);
        void SendSetPartySetting(long connectionId, int id, bool shareExp, bool shareItem);
        void SendClearPartyData(long connectionId, int id);
        void SendAddPartyMember(long connectionId, int id, SocialCharacterData member);
        void SendUpdatePartyMember(long connectionId, int id, SocialCharacterData member);
        void SendRemovePartyMember(long connectionId, int id, string characterId);
        void SendNotifyPartyInvitation(long connectionId, PartyInvitationData invitation);
        // Guild
        void SendSetGuildData(long connectionId, int id, string guildName, string leaderId);
        void SendSetGuildLeader(long connectionId, int id, string leaderId);
        void SendSetGuildMessage(long connectionId, int id, string message);
        void SendSetGuildMessage2(long connectionId, int id, string message);
        void SendSetGuildOptions(long connectionId, int id, string options);
        void SendSetGuildAutoAcceptRequests(long connectionId, int id, bool autoAcceptRequests);
        void SendSetGuildScore(long connectionId, int id, int score);
        void SendSetGuildRank(long connectionId, int id, int rank);
        void SendSetGuildRole(long connectionId, int id, byte guildRole, GuildRoleData guildRoleData);
        void SendSetGuildMemberRole(long connectionId, int id, string characterId, byte guildRole);
        void SendClearGuildData(long connectionId, int id);
        void SendAddGuildMember(long connectionId, int id, SocialCharacterData member);
        void SendUpdateGuildMember(long connectionId, int id, SocialCharacterData member);
        void SendRemoveGuildMember(long connectionId, int id, string characterId);
        void SendSetGuildSkillLevel(long connectionId, int id, int dataId, int level);
        void SendSetGuildGold(long connectionId, int id, int gold);
        void SendSetGuildLevelExpSkillPoint(long connectionId, int id, int level, int exp, int skillPoint);
        void SendNotifyGuildInvitation(long connectionId, GuildInvitationData invitation);
    }
}
