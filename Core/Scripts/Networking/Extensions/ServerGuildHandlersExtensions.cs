namespace MultiplayerARPG
{
    public static partial class ServerGuildHandlersExtensions
    {
        public static ValidateGuildRequestResult CanCreateGuild(this IPlayerCharacterData playerCharacter, string guildName)
        {
            UITextKeys gameMessage;
            if (string.IsNullOrEmpty(guildName) || guildName.Length < GameInstance.Singleton.SocialSystemSetting.MinGuildNameLength)
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_NAME_TOO_SHORT;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guildName.Length > GameInstance.Singleton.SocialSystemSetting.MaxGuildNameLength)
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_NAME_TOO_LONG;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (playerCharacter.GuildId > 0)
            {
                gameMessage = UITextKeys.UI_ERROR_JOINED_ANOTHER_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!GameInstance.Singleton.SocialSystemSetting.CanCreateGuild(playerCharacter, out gameMessage))
                return new ValidateGuildRequestResult(false, gameMessage);
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage);
        }

        public static ValidateGuildRequestResult CanChangeGuildLeader(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter, string memberId)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.IsLeader(playerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_GUILD_LEADER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guild.IsLeader(memberId))
            {
                gameMessage = UITextKeys.UI_ERROR_ALREADY_IS_GUILD_LEADER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.ContainsMemberId(memberId))
            {
                gameMessage = UITextKeys.UI_ERROR_CHARACTER_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanChangeGuildMessage(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter, string guildMessage)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.IsLeader(playerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_GUILD_LEADER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guildMessage.Length > GameInstance.Singleton.SocialSystemSetting.MaxGuildMessageLength)
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_MESSAGE_TOO_LONG;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanChangeGuildMessage2(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter, string guildMessage)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.IsLeader(playerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_GUILD_LEADER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guildMessage.Length > GameInstance.Singleton.SocialSystemSetting.MaxGuildMessage2Length)
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_MESSAGE_TOO_LONG;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanChangeGuildOptions(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.IsLeader(playerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_GUILD_LEADER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanChangeGuildRole(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter, byte guildRole, string roleName)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.IsLeader(playerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_GUILD_LEADER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.IsRoleAvailable(guildRole))
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_ROLE_NOT_AVAILABLE;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (string.IsNullOrEmpty(roleName) || roleName.Length < GameInstance.Singleton.SocialSystemSetting.MinGuildRoleNameLength)
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_ROLE_NAME_TOO_SHORT;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (roleName.Length > GameInstance.Singleton.SocialSystemSetting.MaxGuildRoleNameLength)
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_ROLE_NAME_TOO_LONG;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanChangeGuildMemberRole(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter, string memberId)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.IsLeader(playerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_GUILD_LEADER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guild.IsLeader(memberId))
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_CHANGE_GUILD_LEADER_ROLE;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.ContainsMemberId(memberId))
            {
                gameMessage = UITextKeys.UI_ERROR_CHARACTER_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanSendGuildRequest(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData requesterCharacter, int guildId)
        {
            UITextKeys gameMessage;
            GuildData guild;
            if (requesterCharacter.GuildId > 0)
            {
                gameMessage = UITextKeys.UI_ERROR_JOINED_ANOTHER_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_NOT_FOUND;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanAcceptGuildRequest(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData managerCharacter)
        {
            UITextKeys gameMessage;
            GuildData guild;
            if (managerCharacter.GuildId <= 0 || !serverGuildHandlers.TryGetGuild(managerCharacter.GuildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_NOT_FOUND;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.CanInvite(managerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_ACCEPT_GUILD_REQUEST;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guild.CountMember() >= guild.MaxMember())
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_MEMBER_REACHED_LIMIT;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, managerCharacter.GuildId, guild);
        }

        public static ValidateGuildRequestResult CanDeclineGuildRequest(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData managerCharacter)
        {
            UITextKeys gameMessage;
            GuildData guild;
            if (managerCharacter.GuildId <= 0 || !serverGuildHandlers.TryGetGuild(managerCharacter.GuildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_NOT_FOUND;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.CanInvite(managerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_DECLINE_GUILD_REQUEST;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guild.CountMember() >= guild.MaxMember())
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_MEMBER_REACHED_LIMIT;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, managerCharacter.GuildId, guild);
        }

        public static ValidateGuildRequestResult CanSendGuildInvitation(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData inviterCharacter, IPlayerCharacterData inviteeCharacter)
        {
            UITextKeys gameMessage;
            int guildId = inviterCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.CanInvite(inviterCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_SEND_GUILD_INVITATION;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (inviteeCharacter.GuildId > 0)
            {
                gameMessage = UITextKeys.UI_ERROR_CHARACTER_JOINED_ANOTHER_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanAcceptGuildInvitation(this IServerGuildHandlers serverGuildHandlers, int guildId, IPlayerCharacterData inviteeCharacter)
        {
            UITextKeys gameMessage;
            GuildData guild;
            if (!serverGuildHandlers.HasGuildInvitation(guildId, inviteeCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_INVITATION_NOT_FOUND;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (inviteeCharacter.GuildId > 0)
            {
                gameMessage = UITextKeys.UI_ERROR_JOINED_ANOTHER_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_NOT_FOUND;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guild.CountMember() >= guild.MaxMember())
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_MEMBER_REACHED_LIMIT;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanDeclineGuildInvitation(this IServerGuildHandlers serverGuildHandlers, int guildId, IPlayerCharacterData inviteeCharacter)
        {
            UITextKeys gameMessage;
            GuildData guild;
            if (!serverGuildHandlers.HasGuildInvitation(guildId, inviteeCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_INVITATION_NOT_FOUND;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_NOT_FOUND;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanKickMemberFromGuild(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter, string memberId)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guild.IsLeader(memberId))
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_KICK_GUILD_LEADER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.CanKick(playerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_KICK_GUILD_MEMBER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (playerCharacter.Id.Equals(memberId))
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_KICK_YOURSELF_FROM_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            byte role;
            if (!guild.TryGetMemberRole(memberId, out role) && playerCharacter.GuildRole < role)
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_KICK_HIGHER_GUILD_MEMBER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.ContainsMemberId(memberId))
            {
                gameMessage = UITextKeys.UI_ERROR_CHARACTER_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanLeaveGuild(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanIncreaseGuildSkillLevel(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter, int dataId)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (!GameInstance.GuildSkills.ContainsKey(dataId))
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_GUILD_SKILL_DATA;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (!guild.IsLeader(playerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_GUILD_LEADER;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guild.IsSkillReachedMaxLevel(dataId))
            {
                gameMessage = UITextKeys.UI_ERROR_GUILD_SKILL_REACHED_MAX_LEVEL;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            if (guild.skillPoint <= 0)
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_GUILD_SKILL_POINT;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }

        public static ValidateGuildRequestResult CanIncreaseGuildExp(this IServerGuildHandlers serverGuildHandlers, IPlayerCharacterData playerCharacter, int exp)
        {
            UITextKeys gameMessage;
            int guildId = playerCharacter.GuildId;
            GuildData guild;
            if (guildId <= 0 || !serverGuildHandlers.TryGetGuild(guildId, out guild))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_GUILD;
                return new ValidateGuildRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidateGuildRequestResult(true, gameMessage, guildId, guild);
        }
    }
}
