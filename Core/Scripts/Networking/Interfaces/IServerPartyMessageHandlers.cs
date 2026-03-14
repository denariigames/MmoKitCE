using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IServerPartyMessageHandlers
    {
        UniTaskVoid HandleRequestCreateParty(
            RequestHandlerData requestHandler, RequestCreatePartyMessage request,
            RequestProceedResultDelegate<ResponseCreatePartyMessage> result);

        UniTaskVoid HandleRequestChangePartyLeader(
            RequestHandlerData requestHandler, RequestChangePartyLeaderMessage request,
            RequestProceedResultDelegate<ResponseChangePartyLeaderMessage> result);

        UniTaskVoid HandleRequestChangePartySetting(
            RequestHandlerData requestHandler, RequestChangePartySettingMessage request,
            RequestProceedResultDelegate<ResponseChangePartySettingMessage> result);

        UniTaskVoid HandleRequestSendPartyInvitation(
            RequestHandlerData requestHandler, RequestSendPartyInvitationMessage request,
            RequestProceedResultDelegate<ResponseSendPartyInvitationMessage> result);

        UniTaskVoid HandleRequestAcceptPartyInvitation(
            RequestHandlerData requestHandler, RequestAcceptPartyInvitationMessage request,
            RequestProceedResultDelegate<ResponseAcceptPartyInvitationMessage> result);

        UniTaskVoid HandleRequestDeclinePartyInvitation(
            RequestHandlerData requestHandler, RequestDeclinePartyInvitationMessage request,
            RequestProceedResultDelegate<ResponseDeclinePartyInvitationMessage> result);

        UniTaskVoid HandleRequestKickMemberFromParty(
            RequestHandlerData requestHandler, RequestKickMemberFromPartyMessage request,
            RequestProceedResultDelegate<ResponseKickMemberFromPartyMessage> result);

        UniTaskVoid HandleRequestLeaveParty(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseLeavePartyMessage> result);
    }
}
