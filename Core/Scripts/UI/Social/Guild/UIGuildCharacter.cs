using Cysharp.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public class UIGuildCharacter : UISocialCharacter
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Role Name}")]
        public UILocaleKeySetting formatKeyGuildRole = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Share Exp Percentage}")]
        public UILocaleKeySetting formatKeyShareExpPercentage = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SHARE_EXP_PERCENTAGE);

        [Header("UI Elements")]
        [FormerlySerializedAs("uiGuildRole")]
        public TextWrapper uiTextGuildRole;
        public TextWrapper uiTextShareExpPercentage;

        public byte GuildRole { get; private set; }
        public GuildRoleData GuildRoleData { get; private set; }

        public override void CloneTo(UISelectionEntry<SocialCharacterData> target)
        {
            base.CloneTo(target);
            if (target is UIGuildCharacter uiGuildCharacter)
                uiGuildCharacter.Setup(Data, GuildRole, GuildRoleData);
        }

        public void Setup(SocialCharacterData data, byte guildRole, GuildRoleData guildRoleData)
        {
            GuildRoleData = guildRoleData;
            GuildRole = guildRole;
            Data = data;

            if (uiTextGuildRole != null)
            {
                uiTextGuildRole.text = ZString.Format(
                    LanguageManager.GetText(formatKeyGuildRole),
                    guildRoleData.roleName);
            }

            if (uiTextShareExpPercentage != null)
            {
                uiTextShareExpPercentage.text = ZString.Format(
                    LanguageManager.GetText(formatKeyShareExpPercentage),
                    guildRoleData.shareExpPercentage.ToString("N0"));
            }
        }
    }
}
