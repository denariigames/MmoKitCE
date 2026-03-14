using LiteNetLibManager;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIGuildRoleSetting : UIBase
    {
        [Header("UI Elements")]
        public InputFieldWrapper inputFieldRoleName;
        public Toggle toggleCanInvite;
        public Toggle toggleCanKick;
        public Toggle toggleCanUseStorage;
        public InputFieldWrapper inputFieldShareExpPercentage;

        private byte _guildRole;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            inputFieldRoleName = null;
            toggleCanInvite = null;
            toggleCanKick = null;
            toggleCanUseStorage = null;
            inputFieldShareExpPercentage = null;
        }

        public void Show(byte guildRole, GuildRoleData guildRoleData)
        {
            base.Show();

            this._guildRole = guildRole;
            if (inputFieldRoleName != null)
            {
                inputFieldRoleName.contentType = InputField.ContentType.Standard;
                inputFieldRoleName.text = guildRoleData.roleName;
            }

            if (toggleCanInvite != null)
                toggleCanInvite.isOn = guildRoleData.canInvite;

            if (toggleCanKick != null)
                toggleCanKick.isOn = guildRoleData.canKick;

            if (toggleCanUseStorage != null)
                toggleCanUseStorage.isOn = guildRoleData.canUseStorage;

            if (inputFieldShareExpPercentage != null)
            {
                inputFieldShareExpPercentage.contentType = InputField.ContentType.IntegerNumber;
                inputFieldShareExpPercentage.text = guildRoleData.shareExpPercentage.ToString("N0");
            }
        }

        public void OnClickSetting()
        {
            byte shareExpPercentage;
            if (inputFieldRoleName == null ||
                string.IsNullOrEmpty(inputFieldRoleName.text))
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_GUILD_ROLE_NAME_TOO_SHORT.ToString()));
                return;
            }
            if (inputFieldShareExpPercentage == null ||
                !byte.TryParse(inputFieldShareExpPercentage.text, out shareExpPercentage))
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_GUILD_ROLE_SHARE_EXP_NOT_NUMBER.ToString()));
                return;
            }

            if (shareExpPercentage > GameInstance.Singleton.SocialSystemSetting.MaxShareExpPercentage)
                shareExpPercentage = GameInstance.Singleton.SocialSystemSetting.MaxShareExpPercentage;

            GameInstance.ClientGuildHandlers.RequestChangeGuildRole(new RequestChangeGuildRoleMessage()
            {
                guildRole = _guildRole,
                guildRoleData = new GuildRoleData()
                {
                    roleName = inputFieldRoleName.text,
                    canInvite = toggleCanInvite != null && toggleCanInvite.isOn,
                    canKick = toggleCanKick != null && toggleCanKick.isOn,
                    canUseStorage = toggleCanUseStorage != null && toggleCanUseStorage.isOn,
                    shareExpPercentage = shareExpPercentage,
                },
            }, ChangeGuildRoleCallback);
        }

        private void ChangeGuildRoleCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeGuildRoleMessage response)
        {
            ClientGuildActions.ResponseChangeGuildRole(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            Hide();
        }
    }
}
