using LiteNetLibManager;

namespace MultiplayerARPG
{
    public static class ClientPartyActions
    {
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSendPartyInvitationMessage> onResponseSendPartyInvitation;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseAcceptPartyInvitationMessage> onResponseAcceptPartyInvitation;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseDeclinePartyInvitationMessage> onResponseDeclinePartyInvitation;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseCreatePartyMessage> onResponseCreateParty;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangePartyLeaderMessage> onResponseChangePartyLeader;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseKickMemberFromPartyMessage> onResponseKickMemberFromParty;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseLeavePartyMessage> onResponseLeaveParty;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangePartySettingMessage> onResponseChangePartySetting;
        public static event System.Action<PartyInvitationData> onNotifyPartyInvitation;
        public static event System.Action<UpdatePartyMessage.UpdateType, PartyData> onNotifyPartyUpdated;
        public static event System.Action<UpdateSocialMemberMessage.UpdateType, int, SocialCharacterData> onNotifyPartyMemberUpdated;

        public static void Clean()
        {
            onResponseSendPartyInvitation = null;
            onResponseAcceptPartyInvitation = null;
            onResponseDeclinePartyInvitation = null;
            onResponseCreateParty = null;
            onResponseChangePartyLeader = null;
            onResponseKickMemberFromParty = null;
            onResponseLeaveParty = null;
            onResponseChangePartySetting = null;
            onNotifyPartyInvitation = null;
            onNotifyPartyUpdated = null;
            onNotifyPartyMemberUpdated = null;
        }

        public static void ResponseSendPartyInvitation(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSendPartyInvitationMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSendPartyInvitation != null)
                onResponseSendPartyInvitation.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseAcceptPartyInvitation(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAcceptPartyInvitationMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseAcceptPartyInvitation != null)
                onResponseAcceptPartyInvitation.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseDeclinePartyInvitation(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDeclinePartyInvitationMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseDeclinePartyInvitation != null)
                onResponseDeclinePartyInvitation.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseCreateParty(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCreatePartyMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseCreateParty != null)
                onResponseCreateParty.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangePartyLeader(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangePartyLeaderMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangePartyLeader != null)
                onResponseChangePartyLeader.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseKickMemberFromParty(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseKickMemberFromPartyMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseKickMemberFromParty != null)
                onResponseKickMemberFromParty.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseLeaveParty(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseLeavePartyMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseLeaveParty != null)
                onResponseLeaveParty.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangePartySetting(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangePartySettingMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangePartySetting != null)
                onResponseChangePartySetting.Invoke(requestHandler, responseCode, response);
        }

        public static void NotifyPartyInvitation(PartyInvitationData invitation)
        {
            if (onNotifyPartyInvitation != null)
                onNotifyPartyInvitation.Invoke(invitation);
        }

        public static void NotifyPartyUpdated(UpdatePartyMessage.UpdateType updateType, PartyData party)
        {
            if (onNotifyPartyUpdated != null)
                onNotifyPartyUpdated.Invoke(updateType, party);
        }

        public static void NotifyPartyMemberUpdated(UpdateSocialMemberMessage.UpdateType updateType, int socialId, SocialCharacterData character)
        {
            if (onNotifyPartyMemberUpdated != null)
                onNotifyPartyMemberUpdated.Invoke(updateType, socialId, character);
        }
    }
}
