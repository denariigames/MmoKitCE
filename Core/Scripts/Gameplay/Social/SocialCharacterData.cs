namespace MultiplayerARPG
{
    public partial struct SocialCharacterData
    {
        public static SocialCharacterData Create(BasePlayerCharacterEntity character)
        {
            return new SocialCharacterData()
            {
                id = character.Id,
                characterName = character.CharacterName,
                dataId = character.DataId,
                level = character.Level,
                factionId = character.FactionId,
                partyId = character.PartyId,
                guildId = character.GuildId,
                guildRole = character.GuildRole,
                currentHp = character.CurrentHp,
                maxHp = character.MaxHp,
                currentMp = character.CurrentMp,
                maxMp = character.MaxMp,
                iconDataId = character.IconDataId,
                frameDataId = character.FrameDataId,
                backgroundDataId = character.BackgroundDataId,
                titleDataId = character.TitleDataId,
            };
        }
    }
}
