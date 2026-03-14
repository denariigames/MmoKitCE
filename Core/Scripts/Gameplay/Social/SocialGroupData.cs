namespace MultiplayerARPG
{
    public partial class SocialGroupData
    {
        public static SocialSystemSetting SystemSetting { get { return GameInstance.Singleton.SocialSystemSetting; } }

        public SocialCharacterData CreateMemberData(BasePlayerCharacterEntity playerCharacter)
        {
            return SocialCharacterData.Create(playerCharacter);
        }

        public void AddMember(BasePlayerCharacterEntity playerCharacter)
        {
            AddMember(CreateMemberData(playerCharacter));
        }

        public void UpdateMember(BasePlayerCharacterEntity playerCharacter)
        {
            UpdateMember(CreateMemberData(playerCharacter));
        }

        public bool UpdateSocialGroupMember(UpdateSocialMemberMessage message)
        {
            if (id != message.socialId)
                return false;

            switch (message.type)
            {
                case UpdateSocialMemberMessage.UpdateType.Add:
                    AddMember(message.character);
                    break;
                case UpdateSocialMemberMessage.UpdateType.Update:
                    UpdateMember(message.character);
                    break;
                case UpdateSocialMemberMessage.UpdateType.Remove:
                    RemoveMember(message.character.id);
                    break;
                case UpdateSocialMemberMessage.UpdateType.Clear:
                    ClearMembers();
                    break;
            }
            return true;
        }
    }
}
