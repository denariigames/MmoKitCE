namespace MultiplayerARPG
{
    public static partial class ServerPartyHandlersExtensions
    {
        public static ValidatePartyRequestResult CanCreateParty(this IPlayerCharacterData playerCharacter)
        {
            UITextKeys gameMessageType;
            if (playerCharacter.PartyId > 0)
            {
                gameMessageType = UITextKeys.UI_ERROR_JOINED_ANOTHER_PARTY;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            gameMessageType = UITextKeys.NONE;
            return new ValidatePartyRequestResult(true, gameMessageType);
        }

        public static ValidatePartyRequestResult CanChangePartyLeader(this IServerPartyHandlers serverPartyHandlers, IPlayerCharacterData playerCharacter, string memberId)
        {
            UITextKeys gameMessage;
            int partyId = playerCharacter.PartyId;
            PartyData party;
            if (partyId <= 0 || !serverPartyHandlers.TryGetParty(partyId, out party))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_JOINED_PARTY;
                return new ValidatePartyRequestResult(false, gameMessage);
            }
            if (!party.IsLeader(playerCharacter.Id))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_PARTY_LEADER;
                return new ValidatePartyRequestResult(false, gameMessage);
            }
            if (party.IsLeader(memberId))
            {
                gameMessage = UITextKeys.UI_ERROR_ALREADY_IS_PARTY_LEADER;
                return new ValidatePartyRequestResult(false, gameMessage);
            }
            if (!party.ContainsMemberId(memberId))
            {
                gameMessage = UITextKeys.UI_ERROR_CHARACTER_NOT_JOINED_PARTY;
                return new ValidatePartyRequestResult(false, gameMessage);
            }
            gameMessage = UITextKeys.NONE;
            return new ValidatePartyRequestResult(true, gameMessage, partyId, party);
        }

        public static ValidatePartyRequestResult CanChangePartySetting(this IServerPartyHandlers serverPartyHandlers, IPlayerCharacterData playerCharacter)
        {
            UITextKeys gameMessageType;
            int partyId = playerCharacter.PartyId;
            PartyData party;
            if (partyId <= 0 || !serverPartyHandlers.TryGetParty(partyId, out party))
            {
                gameMessageType = UITextKeys.UI_ERROR_NOT_JOINED_PARTY;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            if (!party.IsLeader(playerCharacter.Id))
            {
                gameMessageType = UITextKeys.UI_ERROR_NOT_PARTY_LEADER;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            gameMessageType = UITextKeys.NONE;
            return new ValidatePartyRequestResult(true, gameMessageType, partyId, party);
        }

        public static ValidatePartyRequestResult CanSendPartyInvitation(this IServerPartyHandlers serverPartyHandlers, IPlayerCharacterData inviterCharacter, IPlayerCharacterData inviteeCharacter)
        {
            UITextKeys gameMessageType;
            int partyId = inviterCharacter.PartyId;
            PartyData party;
            if (partyId <= 0 || !serverPartyHandlers.TryGetParty(partyId, out party))
            {
                gameMessageType = UITextKeys.UI_ERROR_NOT_JOINED_PARTY;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            if (!party.CanInvite(inviterCharacter.Id))
            {
                gameMessageType = UITextKeys.UI_ERROR_CANNOT_SEND_PARTY_INVITATION;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            if (inviteeCharacter.PartyId > 0)
            {
                gameMessageType = UITextKeys.UI_ERROR_CHARACTER_JOINED_ANOTHER_PARTY;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            gameMessageType = UITextKeys.NONE;
            return new ValidatePartyRequestResult(true, gameMessageType, partyId, party);
        }

        public static ValidatePartyRequestResult CanAcceptPartyInvitation(this IServerPartyHandlers serverPartyHandlers, int partyId, IPlayerCharacterData inviteeCharacter)
        {
            UITextKeys gameMessageType;
            PartyData party;
            if (partyId <= 0 || !serverPartyHandlers.TryGetParty(partyId, out party))
            {
                gameMessageType = UITextKeys.UI_ERROR_PARTY_NOT_FOUND;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            return serverPartyHandlers.CanAcceptPartyInvitation(party, inviteeCharacter);
        }

        public static ValidatePartyRequestResult CanAcceptPartyInvitation(this IServerPartyHandlers serverPartyHandlers, PartyData party, IPlayerCharacterData inviteeCharacter)
        {
            UITextKeys gameMessageType;
            if (!serverPartyHandlers.HasPartyInvitation(party.id, inviteeCharacter.Id))
            {
                gameMessageType = UITextKeys.UI_ERROR_PARTY_INVITATION_NOT_FOUND;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            if (inviteeCharacter.PartyId > 0)
            {
                gameMessageType = UITextKeys.UI_ERROR_JOINED_ANOTHER_PARTY;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            if (party.CountMember() >= party.MaxMember())
            {
                gameMessageType = UITextKeys.UI_ERROR_PARTY_MEMBER_REACHED_LIMIT;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            gameMessageType = UITextKeys.NONE;
            return new ValidatePartyRequestResult(true, gameMessageType, party.id, party);
        }

        public static ValidatePartyRequestResult CanDeclinePartyInvitation(this IServerPartyHandlers serverPartyHandlers, int partyId, IPlayerCharacterData inviteeCharacter)
        {
            UITextKeys gameMessageType;
            PartyData party;
            if (partyId <= 0 || !serverPartyHandlers.TryGetParty(partyId, out party))
            {
                gameMessageType = UITextKeys.UI_ERROR_PARTY_NOT_FOUND;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            return serverPartyHandlers.CanDeclinePartyInvitation(party, inviteeCharacter);
        }

        public static ValidatePartyRequestResult CanDeclinePartyInvitation(this IServerPartyHandlers serverPartyHandlers, PartyData party, IPlayerCharacterData inviteeCharacter)
        {
            UITextKeys gameMessageType;
            if (!serverPartyHandlers.HasPartyInvitation(party.id, inviteeCharacter.Id))
            {
                gameMessageType = UITextKeys.UI_ERROR_PARTY_INVITATION_NOT_FOUND;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            gameMessageType = UITextKeys.NONE;
            return new ValidatePartyRequestResult(true, gameMessageType, party.id, party);
        }

        public static ValidatePartyRequestResult CanKickMemberFromParty(this IServerPartyHandlers serverPartyHandlers, IPlayerCharacterData playerCharacter, string memberId)
        {
            UITextKeys gameMessageType;
            int partyId = playerCharacter.PartyId;
            PartyData party;
            if (partyId <= 0 || !serverPartyHandlers.TryGetParty(partyId, out party))
            {
                gameMessageType = UITextKeys.UI_ERROR_NOT_JOINED_PARTY;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            if (party.IsLeader(memberId))
            {
                gameMessageType = UITextKeys.UI_ERROR_CANNOT_KICK_PARTY_LEADER;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            if (!party.CanKick(playerCharacter.Id))
            {
                gameMessageType = UITextKeys.UI_ERROR_CANNOT_KICK_PARTY_MEMBER;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            if (playerCharacter.Id.Equals(memberId))
            {
                gameMessageType = UITextKeys.UI_ERROR_CANNOT_KICK_YOURSELF_FROM_PARTY;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            if (!party.ContainsMemberId(memberId))
            {
                gameMessageType = UITextKeys.UI_ERROR_CHARACTER_NOT_JOINED_PARTY;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            gameMessageType = UITextKeys.NONE;
            return new ValidatePartyRequestResult(true, gameMessageType, partyId, party);
        }

        public static ValidatePartyRequestResult CanLeaveParty(this IServerPartyHandlers serverPartyHandlers, IPlayerCharacterData playerCharacter)
        {
            UITextKeys gameMessageType;
            int partyId = playerCharacter.PartyId;
            PartyData party;
            if (partyId <= 0 || !serverPartyHandlers.TryGetParty(partyId, out party))
            {
                gameMessageType = UITextKeys.UI_ERROR_NOT_JOINED_PARTY;
                return new ValidatePartyRequestResult(false, gameMessageType);
            }
            gameMessageType = UITextKeys.NONE;
            return new ValidatePartyRequestResult(true, gameMessageType, partyId, party);
        }
    }
}
