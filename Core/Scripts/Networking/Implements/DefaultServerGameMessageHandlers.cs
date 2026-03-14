using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultServerGameMessageHandlers : MonoBehaviour, IServerGameMessageHandlers
    {
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public void SendGameMessage(long connectionId, UITextKeys type)
        {
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableUnordered, GameNetworkingConsts.GameMessage, new GameMessage()
            {
                message = type,
            });
        }

        public void SendFormattedGameMessage(long connectionId, UIFormatKeys format, params string[] args)
        {
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableUnordered, GameNetworkingConsts.FormattedGameMessage, new FormattedGameMessage()
            {
                format = format,
                args = args,
            });
        }

        public void NotifyRewardExp(long connectionId, RewardGivenType givenType, int exp)
        {
            if (exp <= 0)
                return;
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableUnordered, GameNetworkingConsts.NotifyRewardExp, (writer) =>
            {
                writer.Put((byte)givenType);
                writer.PutPackedInt(exp);
            });
        }

        public void NotifyRewardGold(long connectionId, RewardGivenType givenType, int gold)
        {
            if (gold <= 0)
                return;
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableUnordered, GameNetworkingConsts.NotifyRewardGold, (writer) =>
            {
                writer.Put((byte)givenType);
                writer.PutPackedInt(gold);
            });
        }

        public void NotifyRewardItem(long connectionId, RewardGivenType givenType, int dataId, int amount)
        {
            if (amount <= 0)
                return;
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableUnordered, GameNetworkingConsts.NotifyRewardItem, (writer) =>
            {
                writer.Put((byte)givenType);
                writer.PutPackedInt(dataId);
                writer.PutPackedInt(amount);
            });
        }

        public void NotifyRewardCurrency(long connectionId, RewardGivenType givenType, int dataId, int amount)
        {
            if (amount <= 0)
                return;
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableUnordered, GameNetworkingConsts.NotifyRewardCurrency, (writer) =>
            {
                writer.Put((byte)givenType);
                writer.PutPackedInt(dataId);
                writer.PutPackedInt(amount);
            });
        }

        // Storage
        public void NotifyStorageItems(long connectionId, StorageType storageType, string storageOwnerId, List<CharacterItem> storageItems)
        {
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableOrdered, GameNetworkingConsts.NotifyStorageItemsUpdated, (writer) =>
            {
                writer.Put((byte)storageType);
                writer.Put(storageOwnerId);
                writer.PutList(storageItems);
            });
        }

        public void NotifyStorageOpened(long connectionId, StorageType storageType, string storageOwnerId, uint objectId, int weightLimit, int slotLimit)
        {
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableOrdered, GameNetworkingConsts.NotifyStorageOpened, (writer) =>
            {
                writer.Put((byte)storageType);
                writer.Put(storageOwnerId);
                writer.PutPackedUInt(objectId);
                writer.PutPackedInt(weightLimit);
                writer.PutPackedInt(slotLimit);
            });
        }

        public void NotifyStorageClosed(long connectionId, StorageType storageType, string storageOwnerId)
        {
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableOrdered, GameNetworkingConsts.NotifyStorageClosed, (writer) =>
            {
                writer.Put((byte)storageType);
                writer.Put(storageOwnerId);
            });
        }

        // Party
        public void SendSetPartyData(long connectionId, int id, bool shareExp, bool shareItem, string leaderId)
        {
            Manager.Server.SendCreateParty(connectionId, GameNetworkingConsts.UpdateParty, id, shareExp, shareItem, leaderId);
        }

        public void SendSetPartyLeader(long connectionId, int id, string leaderId)
        {
            Manager.Server.SendChangePartyLeader(connectionId, GameNetworkingConsts.UpdateParty, id, leaderId);
        }

        public void SendSetPartySetting(long connectionId, int id, bool shareExp, bool shareItem)
        {
            Manager.Server.SendPartySetting(connectionId, GameNetworkingConsts.UpdateParty, id, shareExp, shareItem);
        }

        public void SendClearPartyData(long connectionId, int id)
        {
            Manager.Server.SendPartyTerminate(connectionId, GameNetworkingConsts.UpdateParty, id);
        }

        public void SendAddPartyMember(long connectionId, int id, SocialCharacterData member)
        {
            Manager.Server.SendAddSocialMember(connectionId, GameNetworkingConsts.UpdatePartyMember, id, member);
        }

        public void SendUpdatePartyMember(long connectionId, int id, SocialCharacterData member)
        {
            Manager.Server.SendUpdateSocialMember(connectionId, GameNetworkingConsts.UpdatePartyMember, id, member);
        }

        public void SendRemovePartyMember(long connectionId, int id, string characterId)
        {
            Manager.Server.SendRemoveSocialMember(connectionId, GameNetworkingConsts.UpdatePartyMember, id, characterId);
        }

        public void SendNotifyPartyInvitation(long connectionId, PartyInvitationData invitation)
        {
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableUnordered, GameNetworkingConsts.NotifyPartyInvitation, invitation);
        }

        // Guild
        public void SendSetGuildData(long connectionId, int id, string guildName, string leaderId)
        {
            Manager.Server.SendCreateGuild(connectionId, GameNetworkingConsts.UpdateGuild, id, guildName, leaderId);
        }

        public void SendSetGuildLeader(long connectionId, int id, string leaderId)
        {
            Manager.Server.SendChangeGuildLeader(connectionId, GameNetworkingConsts.UpdateGuild, id, leaderId);
        }

        public void SendSetGuildMessage(long connectionId, int id, string message)
        {
            Manager.Server.SendSetGuildMessage(connectionId, GameNetworkingConsts.UpdateGuild, id, message);
        }

        public void SendSetGuildMessage2(long connectionId, int id, string message)
        {
            Manager.Server.SendSetGuildMessage2(connectionId, GameNetworkingConsts.UpdateGuild, id, message);
        }

        public void SendSetGuildRole(long connectionId, int id, byte guildRole, GuildRoleData guildRoleData)
        {
            Manager.Server.SendSetGuildRole(connectionId, GameNetworkingConsts.UpdateGuild, id, guildRole, guildRoleData);
        }

        public void SendSetGuildMemberRole(long connectionId, int id, string characterId, byte guildRole)
        {
            Manager.Server.SendSetGuildMemberRole(connectionId, GameNetworkingConsts.UpdateGuild, id, characterId, guildRole);
        }

        public void SendClearGuildData(long connectionId, int id)
        {
            Manager.Server.SendGuildTerminate(connectionId, GameNetworkingConsts.UpdateGuild, id);
        }

        public void SendAddGuildMember(long connectionId, int id, SocialCharacterData member)
        {
            Manager.Server.SendAddSocialMember(connectionId, GameNetworkingConsts.UpdateGuildMember, id, member);
        }

        public void SendUpdateGuildMember(long connectionId, int id, SocialCharacterData member)
        {
            Manager.Server.SendUpdateSocialMember(connectionId, GameNetworkingConsts.UpdateGuildMember, id, member);
        }

        public void SendRemoveGuildMember(long connectionId, int id, string characterId)
        {
            Manager.Server.SendRemoveSocialMember(connectionId, GameNetworkingConsts.UpdateGuildMember, id, characterId);
        }

        public void SendSetGuildSkillLevel(long connectionId, int id, int dataId, int level)
        {
            Manager.Server.SendSetGuildSkillLevel(connectionId, GameNetworkingConsts.UpdateGuild, id, dataId, level);
        }

        public void SendSetGuildGold(long connectionId, int id, int gold)
        {
            Manager.Server.SendSetGuildGold(connectionId, GameNetworkingConsts.UpdateGuild, id, gold);
        }

        public void SendSetGuildScore(long connectionId, int id, int score)
        {
            Manager.Server.SendSetGuildScore(connectionId, GameNetworkingConsts.UpdateGuild, id, score);
        }

        public void SendSetGuildOptions(long connectionId, int id, string options)
        {
            Manager.Server.SendSetGuildOptions(connectionId, GameNetworkingConsts.UpdateGuild, id, options);
        }

        public void SendSetGuildAutoAcceptRequests(long connectionId, int id, bool autoAcceptRequests)
        {
            Manager.Server.SendSetGuildAutoAcceptRequests(connectionId, GameNetworkingConsts.UpdateGuild, id, autoAcceptRequests);
        }

        public void SendSetGuildRank(long connectionId, int id, int rank)
        {
            Manager.Server.SendSetGuildRank(connectionId, GameNetworkingConsts.UpdateGuild, id, rank);
        }

        public void SendSetGuildLevelExpSkillPoint(long connectionId, int id, int level, int exp, int skillPoint)
        {
            Manager.Server.SendSetGuildLevelExpSkillPoint(connectionId, GameNetworkingConsts.UpdateGuild, id, level, exp, skillPoint);
        }

        public void SendNotifyGuildInvitation(long connectionId, GuildInvitationData invitation)
        {
            Manager.ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableUnordered, GameNetworkingConsts.NotifyGuildInvitation, invitation);
        }
    }
}
