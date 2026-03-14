using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class LanRpgServerGuildMessageHandlers : MonoBehaviour, IServerGuildMessageHandlers
    {
        public static int Id { get; set; }

        public UniTaskVoid HandleRequestAcceptGuildInvitation(RequestHandlerData requestHandler, RequestAcceptGuildInvitationMessage request, RequestProceedResultDelegate<ResponseAcceptGuildInvitationMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out BasePlayerCharacterEntity playerCharacter))
            {
                result.InvokeError(new ResponseAcceptGuildInvitationMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanAcceptGuildInvitation(request.guildId, playerCharacter);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseAcceptGuildInvitationMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            playerCharacter.GuildId = request.guildId;
            validateResult.Guild.AddMember(playerCharacter);
            GameInstance.ServerGuildHandlers.SetGuild(request.guildId, validateResult.Guild);
            GameInstance.ServerGuildHandlers.RemoveGuildInvitation(request.guildId, playerCharacter.Id);
            GameInstance.ServerGameMessageHandlers.SendSetFullGuildData(requestHandler.ConnectionId, validateResult.Guild);
            GameInstance.ServerGameMessageHandlers.SendAddGuildMemberToMembers(validateResult.Guild, SocialCharacterData.Create(playerCharacter));
            // Send message to inviter
            GameInstance.ServerGameMessageHandlers.SendGameMessageByCharacterId(request.inviterId, UITextKeys.UI_GUILD_INVITATION_ACCEPTED);
            // Response to invitee
            result.InvokeSuccess(new ResponseAcceptGuildInvitationMessage()
            {
                message = UITextKeys.UI_GUILD_INVITATION_ACCEPTED,
            });
            return default;
        }

        public UniTaskVoid HandleRequestDeclineGuildInvitation(RequestHandlerData requestHandler, RequestDeclineGuildInvitationMessage request, RequestProceedResultDelegate<ResponseDeclineGuildInvitationMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out BasePlayerCharacterEntity playerCharacter))
            {
                result.InvokeError(new ResponseDeclineGuildInvitationMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanDeclineGuildInvitation(request.guildId, playerCharacter);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseDeclineGuildInvitationMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            GameInstance.ServerGuildHandlers.RemoveGuildInvitation(request.guildId, playerCharacter.Id);
            // Send message to inviter
            GameInstance.ServerGameMessageHandlers.SendGameMessageByCharacterId(request.inviterId, UITextKeys.UI_GUILD_INVITATION_DECLINED);
            // Response to invitee
            result.InvokeSuccess(new ResponseDeclineGuildInvitationMessage()
            {
                message = UITextKeys.UI_GUILD_INVITATION_DECLINED,
            });
            return default;
        }

        public UniTaskVoid HandleRequestSendGuildInvitation(RequestHandlerData requestHandler, RequestSendGuildInvitationMessage request, RequestProceedResultDelegate<ResponseSendGuildInvitationMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out BasePlayerCharacterEntity playerCharacter))
            {
                result.InvokeError(new ResponseSendGuildInvitationMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacterById(request.inviteeId, out BasePlayerCharacterEntity inviteeCharacter))
            {
                result.InvokeError(new ResponseSendGuildInvitationMessage()
                {
                    message = UITextKeys.UI_ERROR_CHARACTER_NOT_FOUND,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanSendGuildInvitation(playerCharacter, inviteeCharacter);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseSendGuildInvitationMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            GameInstance.ServerGuildHandlers.AppendGuildInvitation(playerCharacter.GuildId, request.inviteeId);
            GameInstance.ServerGameMessageHandlers.SendNotifyGuildInvitation(inviteeCharacter.ConnectionId, new GuildInvitationData()
            {
                InviterId = playerCharacter.Id,
                InviterName = playerCharacter.CharacterName,
                InviterLevel = playerCharacter.Level,
                GuildId = validateResult.GuildId,
                GuildName = validateResult.Guild.guildName,
                GuildLevel = validateResult.Guild.level,
            });
            result.InvokeSuccess(new ResponseSendGuildInvitationMessage());
            return default;
        }

        public UniTaskVoid HandleRequestCreateGuild(RequestHandlerData requestHandler, RequestCreateGuildMessage request, RequestProceedResultDelegate<ResponseCreateGuildMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseCreateGuildMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = playerCharacter.CanCreateGuild(request.guildName);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseCreateGuildMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            GuildData guild = new GuildData(++Id, request.guildName, SocialCharacterData.Create(playerCharacter), GameInstance.Singleton.SocialSystemSetting.GuildMemberRoles);
            GameInstance.Singleton.SocialSystemSetting.DecreaseCreateGuildResource(playerCharacter);
            GameInstance.ServerGuildHandlers.SetGuild(guild.id, guild);
            playerCharacter.GuildId = guild.id;
            playerCharacter.GuildRole = guild.GetMemberRole(playerCharacter.Id);
            GameInstance.ServerGameMessageHandlers.SendSetFullGuildData(requestHandler.ConnectionId, guild);
            result.InvokeSuccess(new ResponseCreateGuildMessage());
            return default;
        }

        public UniTaskVoid HandleRequestChangeGuildLeader(RequestHandlerData requestHandler, RequestChangeGuildLeaderMessage request, RequestProceedResultDelegate<ResponseChangeGuildLeaderMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseChangeGuildLeaderMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanChangeGuildLeader(playerCharacter, request.memberId);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseChangeGuildLeaderMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            validateResult.Guild.SetLeader(request.memberId);
            GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
            GameInstance.ServerGameMessageHandlers.SendSetGuildLeaderToMembers(validateResult.Guild);
            GameInstance.ServerGameMessageHandlers.SendSetGuildMemberRoleToMembers(validateResult.Guild, request.memberId, 0);
            result.InvokeSuccess(new ResponseChangeGuildLeaderMessage());
            return default;
        }

        public UniTaskVoid HandleRequestKickMemberFromGuild(RequestHandlerData requestHandler, RequestKickMemberFromGuildMessage request, RequestProceedResultDelegate<ResponseKickMemberFromGuildMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseKickMemberFromGuildMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanKickMemberFromGuild(playerCharacter, request.memberId);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseKickMemberFromGuildMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterById(request.memberId, out IPlayerCharacterData memberCharacter) &&
                GameInstance.ServerUserHandlers.TryGetConnectionIdById(request.memberId, out long memberConnectionId))
            {
                memberCharacter.ClearGuild();
                GameInstance.ServerGameMessageHandlers.SendClearGuildData(memberConnectionId, validateResult.GuildId);
            }
            validateResult.Guild.RemoveMember(request.memberId);
            GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
            GameInstance.ServerGameMessageHandlers.SendRemoveGuildMemberToMembers(validateResult.Guild, request.memberId);
            result.InvokeSuccess(new ResponseKickMemberFromGuildMessage());
            return default;
        }

        public UniTaskVoid HandleRequestLeaveGuild(RequestHandlerData requestHandler, EmptyMessage request, RequestProceedResultDelegate<ResponseLeaveGuildMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseLeaveGuildMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanLeaveGuild(playerCharacter);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseLeaveGuildMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            if (validateResult.Guild.IsLeader(playerCharacter.Id))
            {
                IPlayerCharacterData memberCharacter;
                long memberConnectionId;
                foreach (string memberId in validateResult.Guild.GetMemberIds())
                {
                    if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterById(memberId, out memberCharacter) &&
                        GameInstance.ServerUserHandlers.TryGetConnectionIdById(memberId, out memberConnectionId))
                    {
                        memberCharacter.ClearGuild();
                        GameInstance.ServerGameMessageHandlers.SendClearGuildData(memberConnectionId, validateResult.GuildId);
                    }
                }
                GameInstance.ServerGuildHandlers.RemoveGuild(validateResult.GuildId);
            }
            else
            {
                playerCharacter.ClearGuild();
                validateResult.Guild.RemoveMember(playerCharacter.Id);
                GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
                GameInstance.ServerGameMessageHandlers.SendRemoveGuildMemberToMembers(validateResult.Guild, playerCharacter.Id);
                GameInstance.ServerGameMessageHandlers.SendClearGuildData(requestHandler.ConnectionId, validateResult.GuildId);
            }
            result.InvokeSuccess(new ResponseLeaveGuildMessage());
            return default;
        }

        public UniTaskVoid HandleRequestChangeGuildMessage(RequestHandlerData requestHandler, RequestChangeGuildMessageMessage request, RequestProceedResultDelegate<ResponseChangeGuildMessageMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseChangeGuildMessageMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanChangeGuildMessage(playerCharacter, request.message);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseChangeGuildMessageMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            validateResult.Guild.guildMessage = request.message;
            GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
            GameInstance.ServerGameMessageHandlers.SendSetGuildMessageToMembers(validateResult.Guild);
            result.InvokeSuccess(new ResponseChangeGuildMessageMessage());
            return default;
        }

        public UniTaskVoid HandleRequestChangeGuildMessage2(RequestHandlerData requestHandler, RequestChangeGuildMessageMessage request, RequestProceedResultDelegate<ResponseChangeGuildMessageMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseChangeGuildMessageMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanChangeGuildMessage2(playerCharacter, request.message);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseChangeGuildMessageMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            validateResult.Guild.guildMessage2 = request.message;
            GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
            GameInstance.ServerGameMessageHandlers.SendSetGuildMessageToMembers(validateResult.Guild);
            result.InvokeSuccess(new ResponseChangeGuildMessageMessage());
            return default;
        }

        public UniTaskVoid HandleRequestChangeGuildOptions(RequestHandlerData requestHandler, RequestChangeGuildOptionsMessage request, RequestProceedResultDelegate<ResponseChangeGuildOptionsMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseChangeGuildOptionsMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanChangeGuildOptions(playerCharacter);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseChangeGuildOptionsMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            validateResult.Guild.options = request.options;
            GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
            GameInstance.ServerGameMessageHandlers.SendSetGuildOptionsToMembers(validateResult.Guild);
            result.InvokeSuccess(new ResponseChangeGuildOptionsMessage());
            return default;
        }

        public UniTaskVoid HandleRequestChangeGuildAutoAcceptRequests(RequestHandlerData requestHandler, RequestChangeGuildAutoAcceptRequestsMessage request, RequestProceedResultDelegate<ResponseChangeGuildAutoAcceptRequestsMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseChangeGuildAutoAcceptRequestsMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanChangeGuildOptions(playerCharacter);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseChangeGuildAutoAcceptRequestsMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            validateResult.Guild.autoAcceptRequests = request.autoAcceptRequests;
            GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
            GameInstance.ServerGameMessageHandlers.SendSetGuildAutoAcceptRequestsToMembers(validateResult.Guild);
            result.InvokeSuccess(new ResponseChangeGuildAutoAcceptRequestsMessage());
            return default;
        }

        public UniTaskVoid HandleRequestChangeGuildRole(RequestHandlerData requestHandler, RequestChangeGuildRoleMessage request, RequestProceedResultDelegate<ResponseChangeGuildRoleMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseChangeGuildRoleMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (request.guildRoleData.shareExpPercentage > GameInstance.Singleton.SocialSystemSetting.MaxShareExpPercentage)
                request.guildRoleData.shareExpPercentage = GameInstance.Singleton.SocialSystemSetting.MaxShareExpPercentage;
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanChangeGuildRole(playerCharacter, request.guildRole, request.guildRoleData.roleName);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseChangeGuildRoleMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            validateResult.Guild.SetRole(request.guildRole, request.guildRoleData);
            GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
            // Change characters guild role
            IPlayerCharacterData memberCharacter;
            foreach (string memberId in validateResult.Guild.GetMemberIds())
            {
                if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterById(memberId, out memberCharacter))
                    memberCharacter.GuildRole = validateResult.Guild.GetMemberRole(memberCharacter.Id);
            }
            GameInstance.ServerGameMessageHandlers.SendSetGuildRoleToMembers(validateResult.Guild, request.guildRole, request.guildRoleData);
            result.InvokeSuccess(new ResponseChangeGuildRoleMessage());
            return default;
        }

        public UniTaskVoid HandleRequestChangeMemberGuildRole(RequestHandlerData requestHandler, RequestChangeMemberGuildRoleMessage request, RequestProceedResultDelegate<ResponseChangeMemberGuildRoleMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseChangeMemberGuildRoleMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanChangeGuildMemberRole(playerCharacter, request.memberId);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseChangeMemberGuildRoleMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            validateResult.Guild.SetMemberRole(request.memberId, request.guildRole);
            GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
            if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterById(request.memberId, out IPlayerCharacterData memberCharacter))
                memberCharacter.GuildRole = validateResult.Guild.GetMemberRole(memberCharacter.Id);
            GameInstance.ServerGameMessageHandlers.SendSetGuildMemberRoleToMembers(validateResult.Guild, request.memberId, request.guildRole);
            result.InvokeSuccess(new ResponseChangeMemberGuildRoleMessage());
            return default;
        }

        public UniTaskVoid HandleRequestIncreaseGuildSkillLevel(RequestHandlerData requestHandler, RequestIncreaseGuildSkillLevelMessage request, RequestProceedResultDelegate<ResponseIncreaseGuildSkillLevelMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseIncreaseGuildSkillLevelMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            ValidateGuildRequestResult validateResult = GameInstance.ServerGuildHandlers.CanIncreaseGuildSkillLevel(playerCharacter, request.dataId);
            if (!validateResult.IsSuccess)
            {
                result.InvokeError(new ResponseIncreaseGuildSkillLevelMessage()
                {
                    message = validateResult.GameMessage,
                });
                return default;
            }
            if (!validateResult.Guild.AddSkillLevel(request.dataId))
            {
                result.InvokeError(new ResponseIncreaseGuildSkillLevelMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_ENOUGH_SKILL_POINT,
                });
                return default;
            }
            GameInstance.ServerGuildHandlers.SetGuild(validateResult.GuildId, validateResult.Guild);
            GameInstance.ServerGameMessageHandlers.SendSetGuildSkillLevelToMembers(validateResult.Guild, request.dataId);
            GameInstance.ServerGameMessageHandlers.SendSetGuildLevelExpSkillPointToMembers(validateResult.Guild);
            result.InvokeSuccess(new ResponseIncreaseGuildSkillLevelMessage());
            return default;
        }

        public UniTaskVoid HandleRequestSendGuildRequest(RequestHandlerData requestHandler, RequestSendGuildRequestMessage request, RequestProceedResultDelegate<ResponseSendGuildRequestMessage> result)
        {
            result.Invoke(AckResponseCode.Unimplemented, new ResponseSendGuildRequestMessage());
            return default;
        }

        public UniTaskVoid HandleRequestAcceptGuildRequest(RequestHandlerData requestHandler, RequestAcceptGuildRequestMessage request, RequestProceedResultDelegate<ResponseAcceptGuildRequestMessage> result)
        {
            result.Invoke(AckResponseCode.Unimplemented, new ResponseAcceptGuildRequestMessage());
            return default;
        }

        public UniTaskVoid HandleRequestDeclineGuildRequest(RequestHandlerData requestHandler, RequestDeclineGuildRequestMessage request, RequestProceedResultDelegate<ResponseDeclineGuildRequestMessage> result)
        {
            result.Invoke(AckResponseCode.Unimplemented, new ResponseDeclineGuildRequestMessage());
            return default;
        }

        public UniTaskVoid HandleRequestGetGuildRequests(RequestHandlerData requestHandler, RequestGetGuildRequestsMessage request, RequestProceedResultDelegate<ResponseGetGuildRequestsMessage> result)
        {
            result.Invoke(AckResponseCode.Unimplemented, new ResponseGetGuildRequestsMessage());
            return default;
        }

        public UniTaskVoid HandleRequestFindGuilds(RequestHandlerData requestHandler, RequestFindGuildsMessage request, RequestProceedResultDelegate<ResponseFindGuildsMessage> result)
        {
            result.Invoke(AckResponseCode.Unimplemented, new ResponseFindGuildsMessage());
            return default;
        }

        public UniTaskVoid HandleRequestGetGuildInfo(RequestHandlerData requestHandler, RequestGetGuildInfoMessage request, RequestProceedResultDelegate<ResponseGetGuildInfoMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseGetGuildInfoMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            if (!GameInstance.ServerGuildHandlers.TryGetGuild(request.guildId, out GuildData guild))
            {
                result.InvokeError(new ResponseGetGuildInfoMessage()
                {
                    message = UITextKeys.UI_ERROR_GUILD_NOT_FOUND,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseGetGuildInfoMessage()
            {
                guild = new GuildListEntry()
                {
                    Id = guild.id,
                    GuildName = guild.guildName,
                    Level = guild.level,
                    FieldOptions = GuildListFieldOptions.Options,
                    Options = guild.options,
                }
            });
            return default;
        }

        public UniTaskVoid HandleRequestGuildRequestNotification(RequestHandlerData requestHandler, EmptyMessage request, RequestProceedResultDelegate<ResponseGuildRequestNotificationMessage> result)
        {
            result.Invoke(AckResponseCode.Unimplemented, new ResponseGuildRequestNotificationMessage());
            return default;
        }
    }
}
