using LiteNetLib;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static partial class SocialMessageExtensions
    {
        public const byte SOCIAL_MSG_DATA_CHANNEL = 5;

        public static UpdateSocialMemberMessage MakeAddSocialMember(int id, SocialCharacterData character)
        {
            return new UpdateSocialMemberMessage()
            {
                type = UpdateSocialMemberMessage.UpdateType.Add,
                socialId = id,
                character = character,
            };
        }

        public static void SendAddSocialMember(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, SocialCharacterData character)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeAddSocialMember(id, character)));
        }

        public static void SendAddSocialMember(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, SocialCharacterData character)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeAddSocialMember(id, character)));
        }

        public static UpdateSocialMemberMessage MakeUpdateSocialMember(int id, SocialCharacterData character)
        {
            return new UpdateSocialMemberMessage()
            {
                type = UpdateSocialMemberMessage.UpdateType.Update,
                socialId = id,
                character = character,
            };
        }

        public static void SendUpdateSocialMember(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, SocialCharacterData member)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeUpdateSocialMember(id, member)));
        }

        public static void SendUpdateSocialMember(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, SocialCharacterData member)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeUpdateSocialMember(id, member)));
        }

        public static UpdateSocialMemberMessage MakeRemoveSocialMember(int id, string characterId)
        {
            return new UpdateSocialMemberMessage()
            {
                type = UpdateSocialMemberMessage.UpdateType.Remove,
                socialId = id,
                character = new SocialCharacterData() { id = characterId },
            };
        }

        public static void SendRemoveSocialMember(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, string characterId)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeRemoveSocialMember(id, characterId)));
        }

        public static void SendRemoveSocialMember(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, string characterId)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeRemoveSocialMember(id, characterId)));
        }

        public static UpdateSocialMemberMessage MakeClearSocialMember(int id)
        {
            return new UpdateSocialMemberMessage()
            {
                type = UpdateSocialMemberMessage.UpdateType.Clear,
                socialId = id,
            };
        }

        public static void SendClearSocialMember(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeClearSocialMember(id)));
        }

        public static void SendClearSocialMember(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeClearSocialMember(id)));
        }

        public static UpdateSocialMembersMessage MakeSocialMembers(List<SocialCharacterData> members)
        {
            return new UpdateSocialMembersMessage()
            {
                members = members,
            };
        }

        public static void SendSocialMembers(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, List<SocialCharacterData> members)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSocialMembers(members)));
        }

        public static void SendSocialMembers(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, List<SocialCharacterData> members)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSocialMembers(members)));
        }

        public static UpdatePartyMessage MakeCreateParty(int id, bool shareExp, bool shareItem, string characterId)
        {
            return new UpdatePartyMessage()
            {
                type = UpdatePartyMessage.UpdateType.Create,
                id = id,
                shareExp = shareExp,
                shareItem = shareItem,
                characterId = characterId,
            };
        }

        public static void SendCreateParty(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, bool shareExp, bool shareItem, string characterId)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeCreateParty(id, shareExp, shareItem, characterId)));
        }

        public static void SendCreateParty(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, bool shareExp, bool shareItem, string characterId)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeCreateParty(id, shareExp, shareItem, characterId)));
        }

        public static UpdatePartyMessage MakeChangePartyLeader(int id, string characterId)
        {
            return new UpdatePartyMessage()
            {
                type = UpdatePartyMessage.UpdateType.ChangeLeader,
                id = id,
                characterId = characterId,
            };
        }

        public static void SendChangePartyLeader(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, string characterId)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeChangePartyLeader(id, characterId)));
        }

        public static void SendChangePartyLeader(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, string characterId)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeChangePartyLeader(id, characterId)));
        }

        public static UpdatePartyMessage MakePartySetting(int id, bool shareExp, bool shareItem)
        {
            return new UpdatePartyMessage()
            {
                type = UpdatePartyMessage.UpdateType.Setting,
                id = id,
                shareExp = shareExp,
                shareItem = shareItem,
            };
        }

        public static void SendPartySetting(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, bool shareExp, bool shareItem)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakePartySetting(id, shareExp, shareItem)));
        }

        public static void SendPartySetting(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, bool shareExp, bool shareItem)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakePartySetting(id, shareExp, shareItem)));
        }

        public static UpdatePartyMessage MakePartyTerminate(int id)
        {
            return new UpdatePartyMessage()
            {
                type = UpdatePartyMessage.UpdateType.Terminate,
                id = id,
            };
        }

        public static void SendPartyTerminate(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakePartyTerminate(id)));
        }

        public static void SendPartyTerminate(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakePartyTerminate(id)));
        }

        public static UpdateGuildMessage MakeCreateGuild(int id, string guildName, string characterId)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.Create,
                id = id,
                guildName = guildName,
                characterId = characterId,
            };
        }

        public static void SendCreateGuild(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, string guildName, string characterId)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeCreateGuild(id, guildName, characterId)));
        }

        public static void SendCreateGuild(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, string guildName, string characterId)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeCreateGuild(id, guildName, characterId)));
        }

        public static UpdateGuildMessage MakeChangeGuildLeader(int id, string characterId)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.ChangeLeader,
                id = id,
                characterId = characterId,
            };
        }

        public static void SendChangeGuildLeader(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, string characterId)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeChangeGuildLeader(id, characterId)));
        }

        public static void SendChangeGuildLeader(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, string characterId)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeChangeGuildLeader(id, characterId)));
        }

        public static UpdateGuildMessage MakeSetGuildMessage(int id, string message)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetGuildMessage,
                id = id,
                guildMessage = message,
            };
        }

        public static void SendSetGuildMessage(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, string message)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildMessage(id, message)));
        }

        public static void SendSetGuildMessage(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, string message)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildMessage(id, message)));
        }

        public static UpdateGuildMessage MakeSetGuildMessage2(int id, string message)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetGuildMessage2,
                id = id,
                guildMessage = message,
            };
        }

        public static void SendSetGuildMessage2(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, string message)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildMessage2(id, message)));
        }

        public static void SendSetGuildMessage2(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, string message)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildMessage2(id, message)));
        }

        public static UpdateGuildMessage MakeSetGuildRole(int id, byte guildRole, GuildRoleData guildRoleData)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetGuildRole,
                id = id,
                guildRole = guildRole,
                guildRoleData = guildRoleData,
            };
        }

        public static void SendSetGuildRole(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, byte guildRole, GuildRoleData guildRoleData)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildRole(id, guildRole, guildRoleData)));
        }

        public static void SendSetGuildRole(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, byte guildRole, GuildRoleData guildRoleData)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildRole(id, guildRole, guildRoleData)));
        }

        public static UpdateGuildMessage MakeSetGuildMemberRole(int id, string characterId, byte guildRole)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetGuildMemberRole,
                id = id,
                characterId = characterId,
                guildRole = guildRole,
            };
        }

        public static void SendSetGuildMemberRole(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, string characterId, byte guildRole)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildMemberRole(id, characterId, guildRole)));
        }

        public static void SendSetGuildMemberRole(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, string characterId, byte guildRole)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildMemberRole(id, characterId, guildRole)));
        }

        public static UpdateGuildMessage MakeGuildTerminate(int id)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.Terminate,
                id = id,
            };
        }

        public static void SendGuildTerminate(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeGuildTerminate(id)));
        }

        public static void SendGuildTerminate(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeGuildTerminate(id)));
        }

        public static UpdateGuildMessage MakeSetGuildLevelExpSkillPoint(int id, int level, int exp, int skillPoint)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.LevelExpSkillPoint,
                id = id,
                level = level,
                exp = exp,
                skillPoint = skillPoint,
            };
        }

        public static void SendSetGuildLevelExpSkillPoint(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, int level, int exp, int skillPoint)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildLevelExpSkillPoint(id, level, exp, skillPoint)));
        }

        public static void SendSetGuildLevelExpSkillPoint(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, int level, int exp, int skillPoint)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildLevelExpSkillPoint(id, level, exp, skillPoint)));
        }

        public static UpdateGuildMessage MakeSetGuildSkillLevel(int id, int dataId, int level)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetSkillLevel,
                id = id,
                dataId = dataId,
                level = level,
            };
        }

        public static void SendSetGuildSkillLevel(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, int dataId, int level)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildSkillLevel(id, dataId, level)));
        }

        public static void SendSetGuildSkillLevel(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, int dataId, int level)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildSkillLevel(id, dataId, level)));
        }

        public static UpdateGuildMessage MakeSetGuildGold(int id, int gold)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetGold,
                id = id,
                gold = gold,
            };
        }

        public static void SendSetGuildGold(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, int gold)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildGold(id, gold)));
        }

        public static void SendSetGuildGold(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, int gold)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildGold(id, gold)));
        }

        public static UpdateGuildMessage MakeSetGuildScore(int id, int score)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetScore,
                id = id,
                score = score,
            };
        }

        public static void SendSetGuildScore(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, int score)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildScore(id, score)));
        }

        public static void SendSetGuildScore(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, int score)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildScore(id, score)));
        }

        public static UpdateGuildMessage MakeSetGuildOptions(int id, string options)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetOptions,
                id = id,
                options = options,
            };
        }

        public static void SendSetGuildOptions(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, string options)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildOptions(id, options)));
        }

        public static void SendSetGuildOptions(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, string options)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildOptions(id, options)));
        }

        public static UpdateGuildMessage MakeSetGuildAutoAcceptRequests(int id, bool autoAcceptRequests)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetAutoAcceptRequests,
                id = id,
                autoAcceptRequests = autoAcceptRequests,
            };
        }

        public static void SendSetGuildAutoAcceptRequests(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, bool autoAcceptRequests)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildAutoAcceptRequests(id, autoAcceptRequests)));
        }

        public static void SendSetGuildAutoAcceptRequests(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, bool autoAcceptRequests)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildAutoAcceptRequests(id, autoAcceptRequests)));
        }

        public static UpdateGuildMessage MakeSetGuildRank(int id, int rank)
        {
            return new UpdateGuildMessage()
            {
                type = UpdateGuildMessage.UpdateType.SetRank,
                id = id,
                rank = rank,
            };
        }

        public static void SendSetGuildRank(this LiteNetLibManager.LiteNetLibServer server, long connectionId, ushort msgType, int id, int rank)
        {
            server.SendPacket(connectionId, SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildRank(id, rank)));
        }

        public static void SendSetGuildRank(this LiteNetLibManager.LiteNetLibClient client, ushort msgType, int id, int rank)
        {
            client.SendPacket(SOCIAL_MSG_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, msgType, (writer) => writer.Put(MakeSetGuildRank(id, rank)));
        }
    }
}
