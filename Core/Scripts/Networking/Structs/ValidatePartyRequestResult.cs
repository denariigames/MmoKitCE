namespace MultiplayerARPG
{
    public struct ValidatePartyRequestResult
    {
        public bool IsSuccess { get; set; }
        public UITextKeys GameMessage { get; set; }
        public int PartyId { get; set; }
        public PartyData Party { get; set; }

        public ValidatePartyRequestResult(bool isSuccess, UITextKeys gameMessageType)
        {
            IsSuccess = isSuccess;
            GameMessage = gameMessageType;
            PartyId = 0;
            Party = null;
        }

        public ValidatePartyRequestResult(bool isSuccess, UITextKeys gameMessageType, int partyId, PartyData party)
        {
            IsSuccess = isSuccess;
            GameMessage = gameMessageType;
            PartyId = partyId;
            Party = party;
        }
    }
}
