using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientGuildHandlers : MonoBehaviour, IClientGuildHandlers
    {
        public GuildData ClientGuild { get; set; }
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestCreateGuild(RequestCreateGuildMessage data, ResponseDelegate<ResponseCreateGuildMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.CreateGuild, data, responseDelegate: callback);
        }

        public bool RequestChangeGuildLeader(RequestChangeGuildLeaderMessage data, ResponseDelegate<ResponseChangeGuildLeaderMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangeGuildLeader, data, responseDelegate: callback);
        }

        public bool RequestChangeGuildMessage(RequestChangeGuildMessageMessage data, ResponseDelegate<ResponseChangeGuildMessageMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangeGuildMessage, data, responseDelegate: callback);
        }

        public bool RequestChangeGuildMessage2(RequestChangeGuildMessageMessage data, ResponseDelegate<ResponseChangeGuildMessageMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangeGuildMessage2, data, responseDelegate: callback);
        }

        public bool RequestChangeGuildOptions(RequestChangeGuildOptionsMessage data, ResponseDelegate<ResponseChangeGuildOptionsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangeGuildOptions, data, responseDelegate: callback);
        }

        public bool RequestChangeGuildAutoAcceptRequests(RequestChangeGuildAutoAcceptRequestsMessage data, ResponseDelegate<ResponseChangeGuildAutoAcceptRequestsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangeGuildAutoAcceptRequests, data, responseDelegate: callback);
        }

        public bool RequestChangeGuildRole(RequestChangeGuildRoleMessage data, ResponseDelegate<ResponseChangeGuildRoleMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangeGuildRole, data, responseDelegate: callback);
        }

        public bool RequestChangeMemberGuildRole(RequestChangeMemberGuildRoleMessage data, ResponseDelegate<ResponseChangeMemberGuildRoleMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangeMemberGuildRole, data, responseDelegate: callback);
        }

        public bool RequestSendGuildInvitation(RequestSendGuildInvitationMessage data, ResponseDelegate<ResponseSendGuildInvitationMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SendGuildInvitation, data, responseDelegate: callback);
        }

        public bool RequestAcceptGuildInvitation(RequestAcceptGuildInvitationMessage data, ResponseDelegate<ResponseAcceptGuildInvitationMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.AcceptGuildInvitation, data, responseDelegate: callback);
        }

        public bool RequestDeclineGuildInvitation(RequestDeclineGuildInvitationMessage data, ResponseDelegate<ResponseDeclineGuildInvitationMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DeclineGuildInvitation, data, responseDelegate: callback);
        }

        public bool RequestKickMemberFromGuild(RequestKickMemberFromGuildMessage data, ResponseDelegate<ResponseKickMemberFromGuildMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.KickMemberFromGuild, data, responseDelegate: callback);
        }

        public bool RequestLeaveGuild(ResponseDelegate<ResponseLeaveGuildMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.LeaveGuild, EmptyMessage.Value, responseDelegate: callback);
        }

        public bool RequestIncreaseGuildSkillLevel(RequestIncreaseGuildSkillLevelMessage data, ResponseDelegate<ResponseIncreaseGuildSkillLevelMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.IncreaseGuildSkillLevel, data, responseDelegate: callback);
        }

        public bool RequestSendGuildRequest(RequestSendGuildRequestMessage data, ResponseDelegate<ResponseSendGuildRequestMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SendGuildRequest, data, responseDelegate: callback);
        }

        public bool RequestAcceptGuildRequest(RequestAcceptGuildRequestMessage data, ResponseDelegate<ResponseAcceptGuildRequestMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.AcceptGuildRequest, data, responseDelegate: callback);
        }

        public bool RequestDeclineGuildRequest(RequestDeclineGuildRequestMessage data, ResponseDelegate<ResponseDeclineGuildRequestMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DeclineGuildRequest, data, responseDelegate: callback);
        }

        public bool RequestGetGuildRequests(RequestGetGuildRequestsMessage data, ResponseDelegate<ResponseGetGuildRequestsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.GetGuildRequests, data, responseDelegate: callback);
        }

        public bool RequestFindGuilds(RequestFindGuildsMessage data, ResponseDelegate<ResponseFindGuildsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.FindGuilds, data, responseDelegate: callback);
        }

        public bool RequestGetGuildInfo(RequestGetGuildInfoMessage data, ResponseDelegate<ResponseGetGuildInfoMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.GetGuildInfo, data, responseDelegate: callback);
        }

        public bool RequestGuildRequestNotification(ResponseDelegate<ResponseGuildRequestNotificationMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.GuildRequestNotification, EmptyMessage.Value, responseDelegate: callback);
        }
    }
}
