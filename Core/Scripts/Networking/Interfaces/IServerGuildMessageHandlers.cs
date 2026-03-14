using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IServerGuildMessageHandlers
    {
        UniTaskVoid HandleRequestCreateGuild(
            RequestHandlerData requestHandler, RequestCreateGuildMessage request,
            RequestProceedResultDelegate<ResponseCreateGuildMessage> result);

        UniTaskVoid HandleRequestChangeGuildLeader(
            RequestHandlerData requestHandler, RequestChangeGuildLeaderMessage request,
            RequestProceedResultDelegate<ResponseChangeGuildLeaderMessage> result);

        UniTaskVoid HandleRequestChangeGuildMessage(
            RequestHandlerData requestHandler, RequestChangeGuildMessageMessage request,
            RequestProceedResultDelegate<ResponseChangeGuildMessageMessage> result);

        UniTaskVoid HandleRequestChangeGuildMessage2(
            RequestHandlerData requestHandler, RequestChangeGuildMessageMessage request,
            RequestProceedResultDelegate<ResponseChangeGuildMessageMessage> result);

        UniTaskVoid HandleRequestChangeGuildOptions(
            RequestHandlerData requestHandler, RequestChangeGuildOptionsMessage request,
            RequestProceedResultDelegate<ResponseChangeGuildOptionsMessage> result);

        UniTaskVoid HandleRequestChangeGuildAutoAcceptRequests(
            RequestHandlerData requestHandler, RequestChangeGuildAutoAcceptRequestsMessage request,
            RequestProceedResultDelegate<ResponseChangeGuildAutoAcceptRequestsMessage> result);

        UniTaskVoid HandleRequestChangeGuildRole(
            RequestHandlerData requestHandler, RequestChangeGuildRoleMessage request,
            RequestProceedResultDelegate<ResponseChangeGuildRoleMessage> result);

        UniTaskVoid HandleRequestChangeMemberGuildRole(
            RequestHandlerData requestHandler, RequestChangeMemberGuildRoleMessage request,
            RequestProceedResultDelegate<ResponseChangeMemberGuildRoleMessage> result);

        UniTaskVoid HandleRequestSendGuildInvitation(
            RequestHandlerData requestHandler, RequestSendGuildInvitationMessage request,
            RequestProceedResultDelegate<ResponseSendGuildInvitationMessage> result);

        UniTaskVoid HandleRequestAcceptGuildInvitation(
            RequestHandlerData requestHandler, RequestAcceptGuildInvitationMessage request,
            RequestProceedResultDelegate<ResponseAcceptGuildInvitationMessage> result);

        UniTaskVoid HandleRequestDeclineGuildInvitation(
            RequestHandlerData requestHandler, RequestDeclineGuildInvitationMessage request,
            RequestProceedResultDelegate<ResponseDeclineGuildInvitationMessage> result);

        UniTaskVoid HandleRequestKickMemberFromGuild(
            RequestHandlerData requestHandler, RequestKickMemberFromGuildMessage request,
            RequestProceedResultDelegate<ResponseKickMemberFromGuildMessage> result);

        UniTaskVoid HandleRequestLeaveGuild(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseLeaveGuildMessage> result);

        UniTaskVoid HandleRequestIncreaseGuildSkillLevel(
            RequestHandlerData requestHandler, RequestIncreaseGuildSkillLevelMessage request,
            RequestProceedResultDelegate<ResponseIncreaseGuildSkillLevelMessage> result);

        UniTaskVoid HandleRequestSendGuildRequest(
            RequestHandlerData requestHandler, RequestSendGuildRequestMessage request,
            RequestProceedResultDelegate<ResponseSendGuildRequestMessage> result);

        UniTaskVoid HandleRequestAcceptGuildRequest(
            RequestHandlerData requestHandler, RequestAcceptGuildRequestMessage request,
            RequestProceedResultDelegate<ResponseAcceptGuildRequestMessage> result);

        UniTaskVoid HandleRequestDeclineGuildRequest(
            RequestHandlerData requestHandler, RequestDeclineGuildRequestMessage request,
            RequestProceedResultDelegate<ResponseDeclineGuildRequestMessage> result);

        UniTaskVoid HandleRequestGetGuildRequests(
            RequestHandlerData requestHandler, RequestGetGuildRequestsMessage request,
            RequestProceedResultDelegate<ResponseGetGuildRequestsMessage> result);

        UniTaskVoid HandleRequestFindGuilds(
            RequestHandlerData requestHandler, RequestFindGuildsMessage request,
            RequestProceedResultDelegate<ResponseFindGuildsMessage> result);

        UniTaskVoid HandleRequestGetGuildInfo(
            RequestHandlerData requestHandler, RequestGetGuildInfoMessage request,
            RequestProceedResultDelegate<ResponseGetGuildInfoMessage> result);

        UniTaskVoid HandleRequestGuildRequestNotification(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseGuildRequestNotificationMessage> result);
    }
}
