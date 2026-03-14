using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IClientPartyHandlers
    {
        bool RequestCreateParty(RequestCreatePartyMessage data, ResponseDelegate<ResponseCreatePartyMessage> callback);
        bool RequestChangePartyLeader(RequestChangePartyLeaderMessage data, ResponseDelegate<ResponseChangePartyLeaderMessage> callback);
        bool RequestChangePartySetting(RequestChangePartySettingMessage data, ResponseDelegate<ResponseChangePartySettingMessage> callback);
        bool RequestSendPartyInvitation(RequestSendPartyInvitationMessage data, ResponseDelegate<ResponseSendPartyInvitationMessage> callback);
        bool RequestAcceptPartyInvitation(RequestAcceptPartyInvitationMessage data, ResponseDelegate<ResponseAcceptPartyInvitationMessage> callback);
        bool RequestDeclinePartyInvitation(RequestDeclinePartyInvitationMessage data, ResponseDelegate<ResponseDeclinePartyInvitationMessage> callback);
        bool RequestKickMemberFromParty(RequestKickMemberFromPartyMessage data, ResponseDelegate<ResponseKickMemberFromPartyMessage> callback);
        bool RequestLeaveParty(ResponseDelegate<ResponseLeavePartyMessage> callback);
    }
}
