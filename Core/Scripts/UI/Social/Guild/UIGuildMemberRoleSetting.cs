using System.Collections.Generic;
using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIGuildMemberRoleSetting : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Character Name}")]
        public UILocaleKeySetting formatKeyName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_LEVEL);

        [Header("UI Elements")]
        public TextWrapper uiTextName;
        public TextWrapper uiTextLevel;
        public DropdownWrapper dropdownRoles;

        private string _characterId;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextName = null;
            uiTextLevel = null;
            dropdownRoles = null;
        }

        public void Show(GuildRoleData[] roles, SocialCharacterData member, byte guildRole)
        {
            base.Show();

            _characterId = member.id;

            if (uiTextName != null)
            {
                uiTextName.text = ZString.Format(
                    LanguageManager.GetText(formatKeyName),
                    string.IsNullOrEmpty(member.characterName) ? LanguageManager.GetUnknowTitle() : member.characterName);
            }

            if (uiTextLevel != null)
            {
                uiTextLevel.text = ZString.Format(
                    LanguageManager.GetText(formatKeyLevel),
                    member.level.ToString("N0"));
            }

            if (dropdownRoles != null)
            {
                List<DropdownWrapper.OptionData> options = new List<DropdownWrapper.OptionData>();
                options.Add(new DropdownWrapper.OptionData(LanguageManager.GetText(UITextKeys.UI_LABEL_NONE.ToString())));
                for (int i = 1; i < roles.Length; ++i)
                {
                    options.Add(new DropdownWrapper.OptionData(roles[i].roleName));
                }
                dropdownRoles.options = options;
                dropdownRoles.value = guildRole;
            }
        }

        public void OnClickSetting()
        {
            byte role = (byte)(dropdownRoles != null ? dropdownRoles.value : 0);
            if (role == 0)
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_INVALID_GUILD_ROLE.ToString()));
                return;
            }
            GameInstance.ClientGuildHandlers.RequestChangeMemberGuildRole(new RequestChangeMemberGuildRoleMessage()
            {
                guildRole = (byte)dropdownRoles.value,
                memberId = _characterId,
            }, ChangeMemberGuildRoleCallback);
        }

        private void ChangeMemberGuildRoleCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeMemberGuildRoleMessage response)
        {
            ClientGuildActions.ResponseChangeMemberGuildRole(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            Hide();
        }
    }
}
