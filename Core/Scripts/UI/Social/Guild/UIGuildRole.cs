using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIGuildRole : UISelectionEntry<GuildRoleData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Role Name}")]
        public UILocaleKeySetting formatKeyRoleName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Share Exp Percentage}")]
        public UILocaleKeySetting formatKeyShareExpPercentage = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SHARE_EXP_PERCENTAGE);

        [Header("UI Elements")]
        public TextWrapper textRoleName;
        public TextWrapper textCanInvite;
        public TextWrapper textCanKick;
        public TextWrapper textCanUseStorage;
        public TextWrapper textShareExpPercentage;
        public GameObject[] canInviteObjects;
        public GameObject[] canKickObjects;
        public GameObject[] canUseStorageObjects;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            textRoleName = null;
            textCanInvite = null;
            textCanKick = null;
            textCanUseStorage = null;
            textShareExpPercentage = null;
            canInviteObjects.Nullify();
            canKickObjects.Nullify();
            canUseStorageObjects.Nullify();
        }

        protected override void UpdateData()
        {
            if (textRoleName != null)
            {
                textRoleName.text = ZString.Format(
                    LanguageManager.GetText(formatKeyRoleName), Data.roleName);
            }

            if (textCanInvite != null)
            {
                textCanInvite.text = Data.canInvite ?
                    LanguageManager.GetText(UITextKeys.UI_GUILD_ROLE_CAN_INVITE.ToString()) :
                    LanguageManager.GetText(UITextKeys.UI_GUILD_ROLE_CANNOT_INVITE.ToString());
            }

            if (textCanKick != null)
            {
                textCanKick.text = Data.canKick ?
                    LanguageManager.GetText(UITextKeys.UI_GUILD_ROLE_CAN_KICK.ToString()) :
                    LanguageManager.GetText(UITextKeys.UI_GUILD_ROLE_CANNOT_KICK.ToString());
            }

            if (textCanUseStorage != null)
            {
                textCanUseStorage.text = Data.canUseStorage ?
                    LanguageManager.GetText(UITextKeys.UI_GUILD_ROLE_CAN_USE_STORAGE.ToString()) :
                    LanguageManager.GetText(UITextKeys.UI_GUILD_ROLE_CANNOT_USE_STORAGE.ToString());
            }

            if (textShareExpPercentage != null)
            {
                textShareExpPercentage.text = ZString.Format(
                    LanguageManager.GetText(formatKeyShareExpPercentage),
                    Data.shareExpPercentage.ToString("N0"));
            }

            if (canInviteObjects != null)
            {
                foreach (GameObject canInviteObject in canInviteObjects)
                {
                    canInviteObject.SetActive(Data.canInvite);
                }
            }

            if (canKickObjects != null)
            {
                foreach (GameObject canKickObject in canKickObjects)
                {
                    canKickObject.SetActive(Data.canKick);
                }
            }

            if (canUseStorageObjects != null)
            {
                foreach (GameObject canUseStorageObject in canUseStorageObjects)
                {
                    canUseStorageObject.SetActive(Data.canUseStorage);
                }
            }
        }
    }
}
