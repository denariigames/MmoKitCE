using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IClientGuildHandlers
    {
        bool RequestCreateGuild(RequestCreateGuildMessage data, ResponseDelegate<ResponseCreateGuildMessage> callback);
        bool RequestChangeGuildLeader(RequestChangeGuildLeaderMessage data, ResponseDelegate<ResponseChangeGuildLeaderMessage> callback);
        bool RequestChangeGuildMessage(RequestChangeGuildMessageMessage data, ResponseDelegate<ResponseChangeGuildMessageMessage> callback);
        bool RequestChangeGuildMessage2(RequestChangeGuildMessageMessage data, ResponseDelegate<ResponseChangeGuildMessageMessage> callback);
        bool RequestChangeGuildOptions(RequestChangeGuildOptionsMessage data, ResponseDelegate<ResponseChangeGuildOptionsMessage> callback);
        bool RequestChangeGuildAutoAcceptRequests(RequestChangeGuildAutoAcceptRequestsMessage data, ResponseDelegate<ResponseChangeGuildAutoAcceptRequestsMessage> callback);
        bool RequestChangeGuildRole(RequestChangeGuildRoleMessage data, ResponseDelegate<ResponseChangeGuildRoleMessage> callback);
        bool RequestChangeMemberGuildRole(RequestChangeMemberGuildRoleMessage data, ResponseDelegate<ResponseChangeMemberGuildRoleMessage> callback);
        bool RequestSendGuildInvitation(RequestSendGuildInvitationMessage data, ResponseDelegate<ResponseSendGuildInvitationMessage> callback);
        bool RequestAcceptGuildInvitation(RequestAcceptGuildInvitationMessage data, ResponseDelegate<ResponseAcceptGuildInvitationMessage> callback);
        bool RequestDeclineGuildInvitation(RequestDeclineGuildInvitationMessage data, ResponseDelegate<ResponseDeclineGuildInvitationMessage> callback);
        bool RequestKickMemberFromGuild(RequestKickMemberFromGuildMessage data, ResponseDelegate<ResponseKickMemberFromGuildMessage> callback);
        bool RequestLeaveGuild(ResponseDelegate<ResponseLeaveGuildMessage> callback);
        bool RequestIncreaseGuildSkillLevel(RequestIncreaseGuildSkillLevelMessage data, ResponseDelegate<ResponseIncreaseGuildSkillLevelMessage> callback);
        bool RequestSendGuildRequest(RequestSendGuildRequestMessage data, ResponseDelegate<ResponseSendGuildRequestMessage> callback);
        bool RequestAcceptGuildRequest(RequestAcceptGuildRequestMessage data, ResponseDelegate<ResponseAcceptGuildRequestMessage> callback);
        bool RequestDeclineGuildRequest(RequestDeclineGuildRequestMessage data, ResponseDelegate<ResponseDeclineGuildRequestMessage> callback);
        bool RequestGetGuildRequests(RequestGetGuildRequestsMessage data, ResponseDelegate<ResponseGetGuildRequestsMessage> callback);
        bool RequestFindGuilds(RequestFindGuildsMessage data, ResponseDelegate<ResponseFindGuildsMessage> callback);
        bool RequestGetGuildInfo(RequestGetGuildInfoMessage data, ResponseDelegate<ResponseGetGuildInfoMessage> callback);
        bool RequestGuildRequestNotification(ResponseDelegate<ResponseGuildRequestNotificationMessage> callback);
    }
}
