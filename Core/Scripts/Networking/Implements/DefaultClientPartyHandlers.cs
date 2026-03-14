using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientPartyHandlers : MonoBehaviour, IClientPartyHandlers
    {
        public PartyData ClientParty { get; set; }
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestCreateParty(RequestCreatePartyMessage data, ResponseDelegate<ResponseCreatePartyMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.CreateParty, data, responseDelegate: callback);
        }

        public bool RequestChangePartyLeader(RequestChangePartyLeaderMessage data, ResponseDelegate<ResponseChangePartyLeaderMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangePartyLeader, data, responseDelegate: callback);
        }

        public bool RequestChangePartySetting(RequestChangePartySettingMessage data, ResponseDelegate<ResponseChangePartySettingMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangePartySetting, data, responseDelegate: callback);
        }

        public bool RequestSendPartyInvitation(RequestSendPartyInvitationMessage data, ResponseDelegate<ResponseSendPartyInvitationMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SendPartyInvitation, data, responseDelegate: callback);
        }

        public bool RequestAcceptPartyInvitation(RequestAcceptPartyInvitationMessage data, ResponseDelegate<ResponseAcceptPartyInvitationMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.AcceptPartyInvitation, data, responseDelegate: callback);
        }

        public bool RequestDeclinePartyInvitation(RequestDeclinePartyInvitationMessage data, ResponseDelegate<ResponseDeclinePartyInvitationMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DeclinePartyInvitation, data, responseDelegate: callback);
        }

        public bool RequestKickMemberFromParty(RequestKickMemberFromPartyMessage data, ResponseDelegate<ResponseKickMemberFromPartyMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.KickMemberFromParty, data, responseDelegate: callback);
        }

        public bool RequestLeaveParty(ResponseDelegate<ResponseLeavePartyMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.LeaveParty, EmptyMessage.Value, responseDelegate: callback);
        }
    }
}
