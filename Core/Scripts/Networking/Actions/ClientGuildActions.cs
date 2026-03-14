using LiteNetLibManager;

namespace MultiplayerARPG
{
    public static class ClientGuildActions
    {
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSendGuildInvitationMessage> onResponseSendGuildInvitation;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseAcceptGuildInvitationMessage> onResponseAcceptGuildInvitation;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseDeclineGuildInvitationMessage> onResponseDeclineGuildInvitation;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseCreateGuildMessage> onResponseCreateGuild;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangeGuildLeaderMessage> onResponseChangeGuildLeader;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseKickMemberFromGuildMessage> onResponseKickMemberFromGuild;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseLeaveGuildMessage> onResponseLeaveGuild;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangeGuildMessageMessage> onResponseChangeGuildMessage;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangeGuildMessageMessage> onResponseChangeGuildMessage2;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangeGuildOptionsMessage> onResponseChangeGuildOptions;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangeGuildAutoAcceptRequestsMessage> onResponseChangeGuildAutoAcceptRequests;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangeGuildRoleMessage> onResponseChangeGuildRole;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangeMemberGuildRoleMessage> onResponseChangeMemberGuildRole;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseIncreaseGuildSkillLevelMessage> onResponseIncreaseGuildSkillLevel;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSendGuildRequestMessage> onResponseSendGuildRequest;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseAcceptGuildRequestMessage> onResponseAcceptGuildRequest;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseDeclineGuildRequestMessage> onResponseDeclineGuildRequest;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseGetGuildRequestsMessage> onResponseGetGuildRequests;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseFindGuildsMessage> onResponseFindGuilds;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseGetGuildInfoMessage> onResponseGetGuildInfo;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseGuildRequestNotificationMessage> onResponseGuildRequestNotification;
        public static event System.Action<GuildInvitationData> onNotifyGuildInvitation;
        public static event System.Action<UpdateGuildMessage.UpdateType, GuildData> onNotifyGuildUpdated;
        public static event System.Action<UpdateSocialMemberMessage.UpdateType, int, SocialCharacterData> onNotifyGuildMemberUpdated;

        public static void Clean()
        {
            onResponseSendGuildInvitation = null;
            onResponseAcceptGuildInvitation = null;
            onResponseDeclineGuildInvitation = null;
            onResponseCreateGuild = null;
            onResponseChangeGuildLeader = null;
            onResponseKickMemberFromGuild = null;
            onResponseLeaveGuild = null;
            onResponseChangeGuildMessage = null;
            onResponseChangeGuildMessage2 = null;
            onResponseChangeGuildOptions = null;
            onResponseChangeGuildAutoAcceptRequests = null;
            onResponseChangeGuildRole = null;
            onResponseChangeMemberGuildRole = null;
            onResponseIncreaseGuildSkillLevel = null;
            onResponseSendGuildRequest = null;
            onResponseAcceptGuildRequest = null;
            onResponseDeclineGuildRequest = null;
            onResponseGetGuildRequests = null;
            onResponseFindGuilds = null;
            onResponseGetGuildInfo = null;
            onResponseGuildRequestNotification = null;
            onNotifyGuildInvitation = null;
            onNotifyGuildUpdated = null;
            onNotifyGuildMemberUpdated = null;
            GuildRequestNotificationCacheManager.ClearCache();
        }

        public static void ResponseSendGuildInvitation(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSendGuildInvitationMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSendGuildInvitation != null)
                onResponseSendGuildInvitation.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseAcceptGuildInvitation(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAcceptGuildInvitationMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseAcceptGuildInvitation != null)
                onResponseAcceptGuildInvitation.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseDeclineGuildInvitation(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDeclineGuildInvitationMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseDeclineGuildInvitation != null)
                onResponseDeclineGuildInvitation.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseCreateGuild(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCreateGuildMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseCreateGuild != null)
                onResponseCreateGuild.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangeGuildLeader(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeGuildLeaderMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangeGuildLeader != null)
                onResponseChangeGuildLeader.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseKickMemberFromGuild(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseKickMemberFromGuildMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseKickMemberFromGuild != null)
                onResponseKickMemberFromGuild.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseLeaveGuild(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseLeaveGuildMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseLeaveGuild != null)
                onResponseLeaveGuild.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangeGuildMessage(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeGuildMessageMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangeGuildMessage != null)
                onResponseChangeGuildMessage.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangeGuildMessage2(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeGuildMessageMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangeGuildMessage2 != null)
                onResponseChangeGuildMessage2.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangeGuildOptions(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeGuildOptionsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangeGuildOptions != null)
                onResponseChangeGuildOptions.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangeGuildAutoAcceptRequests(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeGuildAutoAcceptRequestsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangeGuildAutoAcceptRequests != null)
                onResponseChangeGuildAutoAcceptRequests.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangeGuildRole(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeGuildRoleMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangeGuildRole != null)
                onResponseChangeGuildRole.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangeMemberGuildRole(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeMemberGuildRoleMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangeMemberGuildRole != null)
                onResponseChangeMemberGuildRole.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseIncreaseGuildSkillLevel(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseIncreaseGuildSkillLevelMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseIncreaseGuildSkillLevel != null)
                onResponseIncreaseGuildSkillLevel.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseSendGuildRequest(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSendGuildRequestMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSendGuildRequest != null)
                onResponseSendGuildRequest.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseAcceptGuildRequest(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAcceptGuildRequestMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseAcceptGuildRequest != null)
                onResponseAcceptGuildRequest.Invoke(requestHandler, responseCode, response);
            if (responseCode == AckResponseCode.Success)
                GuildRequestNotificationCacheManager.LoadOrGetGuildRequestNotificationFromCache(null, true);
        }

        public static void ResponseDeclineGuildRequest(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDeclineGuildRequestMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseDeclineGuildRequest != null)
                onResponseDeclineGuildRequest.Invoke(requestHandler, responseCode, response);
            if (responseCode == AckResponseCode.Success)
                GuildRequestNotificationCacheManager.LoadOrGetGuildRequestNotificationFromCache(null, true);
        }

        public static void ResponseGetGuildRequests(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseGetGuildRequestsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseGetGuildRequests != null)
                onResponseGetGuildRequests.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseFindGuilds(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseFindGuildsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseFindGuilds != null)
                onResponseFindGuilds.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseGetGuildInfo(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseGetGuildInfoMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseGetGuildInfo != null)
                onResponseGetGuildInfo.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseGuildRequestNotification(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseGuildRequestNotificationMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseGuildRequestNotification != null)
                onResponseGuildRequestNotification.Invoke(requestHandler, responseCode, response);
        }

        public static void NotifyGuildInvitation(GuildInvitationData invitation)
        {
            if (onNotifyGuildInvitation != null)
                onNotifyGuildInvitation.Invoke(invitation);
        }

        public static void NotifyGuildUpdated(UpdateGuildMessage.UpdateType updateType, GuildData guild)
        {
            if (onNotifyGuildUpdated != null)
                onNotifyGuildUpdated.Invoke(updateType, guild);
            if (guild == null)
                return;
            GuildListEntry cachedData;
            switch (updateType)
            {
                case UpdateGuildMessage.UpdateType.Create:
                    cachedData = new GuildListEntry()
                    {
                        Id = guild.id,
                        GuildName = guild.guildName,
                        Level = guild.level,
                        FieldOptions = GuildListFieldOptions.All,
                        GuildMessage = guild.guildMessage,
                        GuildMessage2 = guild.guildMessage2,
                        Score = guild.score,
                        Options = guild.options,
                        AutoAcceptRequests = guild.autoAcceptRequests,
                        Rank = guild.rank,
                        CurrentMembers = guild.CountMember(),
                        MaxMembers = guild.MaxMember(),
                    };
                    GuildInfoCacheManager.SetCache(cachedData);
                    break;
                case UpdateGuildMessage.UpdateType.SetGuildMessage:
                    if (GuildInfoCacheManager.TryGetFromCache(guild.id, out cachedData))
                    {
                        cachedData.GuildMessage = guild.guildMessage;
                        GuildInfoCacheManager.SetCache(cachedData);
                    }
                    break;
                case UpdateGuildMessage.UpdateType.SetGuildMessage2:
                    if (GuildInfoCacheManager.TryGetFromCache(guild.id, out cachedData))
                    {
                        cachedData.GuildMessage2 = guild.guildMessage2;
                        GuildInfoCacheManager.SetCache(cachedData);
                    }
                    break;
                case UpdateGuildMessage.UpdateType.LevelExpSkillPoint:
                    if (GuildInfoCacheManager.TryGetFromCache(guild.id, out cachedData))
                    {
                        cachedData.Level = guild.level;
                        GuildInfoCacheManager.SetCache(cachedData);
                    }
                    break;
                case UpdateGuildMessage.UpdateType.SetScore:
                    if (GuildInfoCacheManager.TryGetFromCache(guild.id, out cachedData))
                    {
                        cachedData.Score = guild.score;
                        GuildInfoCacheManager.SetCache(cachedData);
                    }
                    break;
                case UpdateGuildMessage.UpdateType.SetOptions:
                    if (GuildInfoCacheManager.TryGetFromCache(guild.id, out cachedData))
                    {
                        cachedData.Options = guild.options;
                        GuildInfoCacheManager.SetCache(cachedData);
                    }
                    break;
                case UpdateGuildMessage.UpdateType.SetAutoAcceptRequests:
                    if (GuildInfoCacheManager.TryGetFromCache(guild.id, out cachedData))
                    {
                        cachedData.AutoAcceptRequests = guild.autoAcceptRequests;
                        GuildInfoCacheManager.SetCache(cachedData);
                    }
                    break;
                case UpdateGuildMessage.UpdateType.SetRank:
                    if (GuildInfoCacheManager.TryGetFromCache(guild.id, out cachedData))
                    {
                        cachedData.Rank = guild.rank;
                        GuildInfoCacheManager.SetCache(cachedData);
                    }
                    break;
                case UpdateGuildMessage.UpdateType.Member:
                    if (GuildInfoCacheManager.TryGetFromCache(guild.id, out cachedData))
                    {
                        cachedData.CurrentMembers = guild.CountMember();
                        cachedData.MaxMembers = guild.MaxMember();
                        GuildInfoCacheManager.SetCache(cachedData);
                    }
                    break;
            }
        }

        public static void NotifyGuildMemberUpdated(UpdateSocialMemberMessage.UpdateType updateType, int socialId, SocialCharacterData character)
        {
            if (onNotifyGuildMemberUpdated != null)
                onNotifyGuildMemberUpdated.Invoke(updateType, socialId, character);
        }
    }
}
