using LiteNetLib.Utils;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientGameMessageHandlers : MonoBehaviour, IClientGameMessageHandlers
    {
        public void HandleGameMessage(MessageHandlerData messageHandler)
        {
            GameMessage gameMessage = messageHandler.ReadMessage<GameMessage>();
            ClientGenericActions.ClientReceiveGameMessage(gameMessage.message);
        }

        public void HandleFormattedGameMessage(MessageHandlerData messageHandler)
        {
            FormattedGameMessage formattedGameMessage = messageHandler.ReadMessage<FormattedGameMessage>();
            ClientGenericActions.ClientReceiveGameMessage(formattedGameMessage.format, formattedGameMessage.args);
        }

        public void HandleNotifyRewardExp(MessageHandlerData messageHandler)
        {
            ClientGenericActions.NotifyRewardExp(
                (RewardGivenType)messageHandler.Reader.GetByte(),
                messageHandler.Reader.GetPackedInt());
        }

        public void HandleNotifyRewardGold(MessageHandlerData messageHandler)
        {
            ClientGenericActions.NotifyRewardGold(
                (RewardGivenType)messageHandler.Reader.GetByte(),
                messageHandler.Reader.GetPackedInt());
        }

        public void HandleNotifyRewardItem(MessageHandlerData messageHandler)
        {
            ClientGenericActions.NotifyRewardItem(
                (RewardGivenType)messageHandler.Reader.GetByte(),
                messageHandler.Reader.GetPackedInt(),
                messageHandler.Reader.GetPackedInt());
        }

        public void HandleNotifyRewardCurrency(MessageHandlerData messageHandler)
        {
            ClientGenericActions.NotifyRewardCurrency(
                (RewardGivenType)messageHandler.Reader.GetByte(),
                messageHandler.Reader.GetPackedInt(),
                messageHandler.Reader.GetPackedInt());
        }

        public void HandleNotifyStorageOpened(MessageHandlerData messageHandler)
        {
            ClientStorageActions.NotifyStorageOpened(
                (StorageType)messageHandler.Reader.GetByte(),
                messageHandler.Reader.GetString(),
                messageHandler.Reader.GetPackedUInt(),
                messageHandler.Reader.GetPackedInt(),
                messageHandler.Reader.GetPackedInt());
        }

        public void HandleNotifyStorageClosed(MessageHandlerData messageHandler)
        {
            ClientStorageActions.NotifyStorageClosed(
                (StorageType)messageHandler.Reader.GetByte(),
                messageHandler.Reader.GetString());
        }

        public void HandleNotifyStorageItems(MessageHandlerData messageHandler)
        {
            ClientStorageActions.NotifyStorageItemsUpdated(
                (StorageType)messageHandler.Reader.GetByte(),
                messageHandler.Reader.GetString(),
                messageHandler.Reader.GetList<CharacterItem>());
        }

        public void HandleNotifyPartyInvitation(MessageHandlerData messageHandler)
        {
            ClientPartyActions.NotifyPartyInvitation(messageHandler.ReadMessage<PartyInvitationData>());
        }

        public void HandleNotifyGuildInvitation(MessageHandlerData messageHandler)
        {
            ClientGuildActions.NotifyGuildInvitation(messageHandler.ReadMessage<GuildInvitationData>());
        }

        public void HandleUpdatePartyMember(MessageHandlerData messageHandler)
        {
            UpdateSocialMemberMessage updateMessage = messageHandler.ReadMessage<UpdateSocialMemberMessage>();
            if (GameInstance.JoinedParty != null)
                GameInstance.JoinedParty.UpdateSocialGroupMember(updateMessage);
            ClientPartyActions.NotifyPartyMemberUpdated(updateMessage.type, updateMessage.socialId, updateMessage.character);
            ClientPartyActions.NotifyPartyUpdated(UpdatePartyMessage.UpdateType.Member, GameInstance.JoinedParty);
        }

        public void HandleUpdateParty(MessageHandlerData messageHandler)
        {
            UpdatePartyMessage message = messageHandler.ReadMessage<UpdatePartyMessage>();
            if (message.type == UpdatePartyMessage.UpdateType.Create)
            {
                GameInstance.JoinedParty = new PartyData(message.id, message.shareExp, message.shareItem, message.characterId);
            }
            else if (GameInstance.JoinedParty != null && GameInstance.JoinedParty.id == message.id)
            {
                switch (message.type)
                {
                    case UpdatePartyMessage.UpdateType.ChangeLeader:
                        GameInstance.JoinedParty.SetLeader(message.characterId);
                        break;
                    case UpdatePartyMessage.UpdateType.Setting:
                        GameInstance.JoinedParty.Setting(message.shareExp, message.shareItem);
                        break;
                    case UpdatePartyMessage.UpdateType.Terminate:
                        GameInstance.JoinedParty = null;
                        break;
                }
            }
            ClientPartyActions.NotifyPartyUpdated(message.type, GameInstance.JoinedParty);
        }

        public void HandleUpdateGuildMember(MessageHandlerData messageHandler)
        {
            UpdateSocialMemberMessage updateMessage = messageHandler.ReadMessage<UpdateSocialMemberMessage>();
            if (GameInstance.JoinedGuild != null)
                GameInstance.JoinedGuild.UpdateSocialGroupMember(updateMessage);
            ClientGuildActions.NotifyGuildMemberUpdated(updateMessage.type, updateMessage.socialId, updateMessage.character);
            ClientGuildActions.NotifyGuildUpdated(UpdateGuildMessage.UpdateType.Member, GameInstance.JoinedGuild);
        }

        public void HandleUpdateGuild(MessageHandlerData messageHandler)
        {
            UpdateGuildMessage message = messageHandler.ReadMessage<UpdateGuildMessage>();
            if (message.type == UpdateGuildMessage.UpdateType.Create)
            {
                GameInstance.JoinedGuild = new GuildData(message.id, message.guildName, message.characterId, GameInstance.Singleton.SocialSystemSetting.GuildMemberRoles);
            }
            else if (GameInstance.JoinedGuild != null && GameInstance.JoinedGuild.id == message.id)
            {
                switch (message.type)
                {
                    case UpdateGuildMessage.UpdateType.ChangeLeader:
                        GameInstance.JoinedGuild.SetLeader(message.characterId);
                        break;
                    case UpdateGuildMessage.UpdateType.SetGuildMessage:
                        GameInstance.JoinedGuild.guildMessage = message.guildMessage;
                        break;
                    case UpdateGuildMessage.UpdateType.SetGuildMessage2:
                        GameInstance.JoinedGuild.guildMessage2 = message.guildMessage;
                        break;
                    case UpdateGuildMessage.UpdateType.SetGuildRole:
                        GameInstance.JoinedGuild.SetRole(message.guildRole, message.guildRoleData);
                        break;
                    case UpdateGuildMessage.UpdateType.SetGuildMemberRole:
                        GameInstance.JoinedGuild.SetMemberRole(message.characterId, message.guildRole);
                        break;
                    case UpdateGuildMessage.UpdateType.SetSkillLevel:
                        GameInstance.JoinedGuild.SetSkillLevel(message.dataId, message.level);
                        if (GameInstance.PlayingCharacterEntity != null)
                            GameInstance.PlayingCharacterEntity.ForceMakeCaches();
                        break;
                    case UpdateGuildMessage.UpdateType.SetGold:
                        GameInstance.JoinedGuild.gold = message.gold;
                        break;
                    case UpdateGuildMessage.UpdateType.SetScore:
                        GameInstance.JoinedGuild.score = message.score;
                        break;
                    case UpdateGuildMessage.UpdateType.SetOptions:
                        GameInstance.JoinedGuild.options = message.options;
                        break;
                    case UpdateGuildMessage.UpdateType.SetAutoAcceptRequests:
                        GameInstance.JoinedGuild.autoAcceptRequests = message.autoAcceptRequests;
                        break;
                    case UpdateGuildMessage.UpdateType.SetRank:
                        GameInstance.JoinedGuild.rank = message.rank;
                        break;
                    case UpdateGuildMessage.UpdateType.LevelExpSkillPoint:
                        GameInstance.JoinedGuild.level = message.level;
                        GameInstance.JoinedGuild.exp = message.exp;
                        GameInstance.JoinedGuild.skillPoint = message.skillPoint;
                        break;
                    case UpdateGuildMessage.UpdateType.Terminate:
                        GameInstance.JoinedGuild = null;
                        if (GameInstance.PlayingCharacterEntity != null)
                            GameInstance.PlayingCharacterEntity.ForceMakeCaches();
                        break;
                }
            }
            ClientGuildActions.NotifyGuildUpdated(message.type, GameInstance.JoinedGuild);
        }
    }
}
