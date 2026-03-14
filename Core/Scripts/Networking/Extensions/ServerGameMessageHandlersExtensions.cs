using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static partial class ServerGameMessageHandlersExtensions
    {
        public static void SendGameMessageByCharacterId(this IServerGameMessageHandlers handler, string id, UITextKeys message)
        {
            long connectionId;
            if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(id, out connectionId))
                handler.SendGameMessage(connectionId, message);
        }

        public static void SendSetPartyData(this IServerGameMessageHandlers handlers, long connectionId, PartyData party)
        {
            if (party == null)
                return;
            handlers.SendSetPartyData(connectionId, party.id, party.shareExp, party.shareItem, party.leaderId);
        }

        public static void SendSetFullPartyData(this IServerGameMessageHandlers handlers, long connectionId, PartyData party)
        {
            if (party == null)
                return;
            handlers.SendSetPartyData(connectionId, party);
            handlers.SendAddPartyMembersToOne(connectionId, party);
        }

        public static void SendSetPartyLeaderToMembers(this IServerGameMessageHandlers handlers, PartyData party)
        {
            if (party == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in party.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetPartyLeader(connectionId, party.id, party.leaderId);
            }
        }

        public static void SendSetPartySettingToMembers(this IServerGameMessageHandlers handlers, PartyData party)
        {
            if (party == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in party.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetPartySetting(connectionId, party.id, party.shareExp, party.shareItem);
            }
        }

        public static void SendAddPartyMembersToOne(this IServerGameMessageHandlers handlers, long connectionId, PartyData party)
        {
            if (party == null)
                return;
            foreach (SocialCharacterData member in party.GetMembers())
            {
                handlers.SendAddPartyMember(connectionId, party.id, member);
            }
        }

        public static void SendAddPartyMemberToMembers(this IServerGameMessageHandlers handlers, PartyData party, SocialCharacterData newMember)
        {
            if (party == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in party.GetMembers())
            {
                if (!member.id.Equals(newMember.id) && GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendAddPartyMember(connectionId, party.id, newMember);
            }
        }

        public static void SendUpdatePartyMembersToOne(this IServerGameMessageHandlers handlers, long connectionId, PartyData party)
        {
            if (party == null)
                return;
            foreach (SocialCharacterData member in party.GetMembers())
            {
                handlers.SendUpdatePartyMember(connectionId, party.id, member);
            }
        }

        public static void SendRemovePartyMemberToMembers(this IServerGameMessageHandlers handlers, PartyData party, string characterId)
        {
            if (party == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in party.GetMembers())
            {
                if (!member.id.Equals(characterId) && GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendRemovePartyMember(connectionId, party.id, characterId);
            }
        }
        public static void SendSetGuildData(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildData(connectionId, guild.id, guild.guildName, guild.leaderId);
        }

        public static void SendSetFullGuildData(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildData(connectionId, guild);
            handlers.SendSetGuildMessage(connectionId, guild);
            handlers.SendSetGuildMessage2(connectionId, guild);
            handlers.SendSetGuildRank(connectionId, guild);
            handlers.SendSetGuildScore(connectionId, guild);
            handlers.SendSetGuildOptions(connectionId, guild);
            handlers.SendSetGuildRolesToOne(connectionId, guild);
            handlers.SendAddGuildMembersToOne(connectionId, guild);
            handlers.SendSetGuildMemberRolesToOne(connectionId, guild);
            handlers.SendSetGuildSkillLevelsToOne(connectionId, guild);
            handlers.SendSetGuildGold(connectionId, guild);
            handlers.SendSetGuildLevelExpSkillPoint(connectionId, guild);
            handlers.SendSetGuildAutoAcceptRequests(connectionId, guild);
        }

        public static void SendSetGuildLeaderToMembers(this IServerGameMessageHandlers handlers, GuildData guild)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildLeader(connectionId, guild.id, guild.leaderId);
            }
        }

        public static void SendSetGuildMessage(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildMessage(connectionId, guild.id, guild.guildMessage);
        }

        public static void SendSetGuildMessageToMembers(this IServerGameMessageHandlers handlers, GuildData guild)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildMessage(connectionId, guild.id, guild.guildMessage);
            }
        }

        public static void SendSetGuildMessage2(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildMessage2(connectionId, guild.id, guild.guildMessage2);
        }

        public static void SendSetGuildMessage2ToMembers(this IServerGameMessageHandlers handlers, GuildData guild)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildMessage2(connectionId, guild.id, guild.guildMessage2);
            }
        }

        public static void SendSetGuildOptions(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildOptions(connectionId, guild.id, guild.options);
        }

        public static void SendSetGuildOptionsToMembers(this IServerGameMessageHandlers handlers, GuildData guild)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildOptions(connectionId, guild.id, guild.options);
            }
        }

        public static void SendSetGuildAutoAcceptRequests(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildAutoAcceptRequests(connectionId, guild.id, guild.autoAcceptRequests);
        }

        public static void SendSetGuildAutoAcceptRequestsToMembers(this IServerGameMessageHandlers handlers, GuildData guild)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildAutoAcceptRequests(connectionId, guild.id, guild.autoAcceptRequests);
            }
        }

        public static void SendSetGuildScore(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildScore(connectionId, guild.id, guild.score);
        }

        public static void SendSetGuildScoreToMembers(this IServerGameMessageHandlers handlers, GuildData guild)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildScore(connectionId, guild.id, guild.score);
            }
        }

        public static void SendSetGuildRank(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildRank(connectionId, guild.id, guild.score);
        }

        public static void SendSetGuildRankToMembers(this IServerGameMessageHandlers handlers, GuildData guild)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildScore(connectionId, guild.id, guild.rank);
            }
        }

        public static void SendSetGuildRoleToMembers(this IServerGameMessageHandlers handlers, GuildData guild, byte guildRole, GuildRoleData guildRoleData)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildRole(connectionId, guild.id, guildRole, guildRoleData);
            }
        }

        public static void SendSetGuildRolesToOne(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            List<GuildRoleData> roles = guild.GetRoles();
            GuildRoleData guildRoleData;
            for (byte role = 0; role < roles.Count; ++role)
            {
                guildRoleData = roles[role];
                handlers.SendSetGuildRole(connectionId, guild.id, role, guildRoleData);
            }
        }

        public static void SendSetGuildMemberRoleToMembers(this IServerGameMessageHandlers handlers, GuildData guild, string characterId, byte guildRole)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildMemberRole(connectionId, guild.id, characterId, guildRole);
            }
        }

        public static void SendSetGuildMemberRolesToOne(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            byte role;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (guild.TryGetMemberRole(member.id, out role))
                    handlers.SendSetGuildMemberRole(connectionId, guild.id, member.id, role);
            }
        }

        public static void SendAddGuildMembersToOne(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                handlers.SendAddGuildMember(connectionId, guild.id, member);
            }
        }

        public static void SendAddGuildMemberToMembers(this IServerGameMessageHandlers handlers, GuildData guild, SocialCharacterData newMember)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (!member.id.Equals(newMember.id) && GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendAddGuildMember(connectionId, guild.id, newMember);
            }
        }

        public static void SendUpdateGuildMembersToOne(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                handlers.SendUpdateGuildMember(connectionId, guild.id, member);
            }
        }

        public static void SendRemoveGuildMemberToMembers(this IServerGameMessageHandlers handlers, GuildData guild, string characterId)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (!member.id.Equals(characterId) && GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendRemoveGuildMember(connectionId, guild.id, characterId);
            }
        }

        public static void SendSetGuildSkillLevelsToOne(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            foreach (KeyValuePair<int, int> guildSkillLevel in guild.GetSkillLevels())
            {
                handlers.SendSetGuildSkillLevel(connectionId, guild.id, guildSkillLevel.Key, guildSkillLevel.Value);
            }
        }

        public static void SendSetGuildSkillLevelToMembers(this IServerGameMessageHandlers handlers, GuildData guild, int dataId)
        {
            if (guild == null)
                return;
            int skillLevel = guild.GetSkillLevel(dataId);
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildSkillLevel(connectionId, guild.id, dataId, skillLevel);
            }
        }

        public static void SendSetGuildGold(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildGold(connectionId, guild.id, guild.gold);
        }

        public static void SendSetGuildGoldToMembers(this IServerGameMessageHandlers handlers, GuildData guild)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildGold(connectionId, guild.id, guild.gold);
            }
        }

        public static void SendSetGuildLevelExpSkillPoint(this IServerGameMessageHandlers handlers, long connectionId, GuildData guild)
        {
            if (guild == null)
                return;
            handlers.SendSetGuildLevelExpSkillPoint(connectionId, guild.id, guild.level, guild.exp, guild.skillPoint);
        }

        public static void SendSetGuildLevelExpSkillPointToMembers(this IServerGameMessageHandlers handlers, GuildData guild)
        {
            if (guild == null)
                return;
            long connectionId;
            foreach (SocialCharacterData member in guild.GetMembers())
            {
                if (GameInstance.ServerUserHandlers.TryGetConnectionIdById(member.id, out connectionId))
                    handlers.SendSetGuildLevelExpSkillPoint(connectionId, guild.id, guild.level, guild.exp, guild.skillPoint);
            }
        }

        public static void NotifyStorageItemsToClients(this IServerGameMessageHandlers handlers, IEnumerable<long> connectionIds, StorageType storageType, string storageOwnerId, List<CharacterItem> storageItems)
        {
            foreach (long connectionId in connectionIds)
            {
                handlers.NotifyStorageItems(connectionId, storageType, storageOwnerId, storageItems);
            }
        }
    }
}
