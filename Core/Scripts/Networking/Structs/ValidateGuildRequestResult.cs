namespace MultiplayerARPG
{
    public struct ValidateGuildRequestResult
    {
        public bool IsSuccess { get; set; }
        public UITextKeys GameMessage { get; set; }
        public int GuildId { get; set; }
        public GuildData Guild { get; set; }

        public ValidateGuildRequestResult(bool isSuccess, UITextKeys gameMessageType)
        {
            IsSuccess = isSuccess;
            GameMessage = gameMessageType;
            GuildId = 0;
            Guild = null;
        }

        public ValidateGuildRequestResult(bool isSuccess, UITextKeys gameMessageType, int guildId, GuildData guild)
        {
            IsSuccess = isSuccess;
            GameMessage = gameMessageType;
            GuildId = guildId;
            Guild = guild;
        }
    }
}
