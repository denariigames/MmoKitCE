using Cysharp.Text;
using LiteNetLibManager;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public class UIGuildListEntry : UISelectionEntry<GuildListEntry>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Guild Name}")]
        public UILocaleKeySetting formatKeyGuildName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_LEVEL);
        [Tooltip("Format => {0} = {Message}")]
        public UILocaleKeySetting formatKeyMessage = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Score}")]
        public UILocaleKeySetting formatKeyScore = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Rank}")]
        public UILocaleKeySetting formatKeyRank = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Current Amount}, {1} = {Max Amount}")]
        public UILocaleKeySetting formatKeyMemberAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SOCIAL_MEMBER_AMOUNT);

        [Header("UI Elements")]
        public UIGuildIcon uiGuildIcon;
        public TextWrapper textGuildName;
        public TextWrapper textLevel;
        public TextWrapper textMessage;
        public TextWrapper textScore;
        public TextWrapper textRank;
        public TextWrapper textMemberAmount;
        public GameObject[] autoAcceptRequestsObjects;
        public GameObject[] notAutoAcceptRequestsObjects;

        [Header("Events")]
        public UnityEvent onGuildRequested = new UnityEvent();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiGuildIcon = null;
            textGuildName = null;
            textLevel = null;
            textMessage = null;
            textScore = null;
            textRank = null;
            textMemberAmount = null;
            autoAcceptRequestsObjects.Nullify();
            notAutoAcceptRequestsObjects.Nullify();
            onGuildRequested?.RemoveAllListeners();
            onGuildRequested = null;
        }

        protected override void UpdateData()
        {
            GuildOptions options = new GuildOptions();
            if (!string.IsNullOrEmpty(Data.Options))
                options = JsonConvert.DeserializeObject<GuildOptions>(Data.Options);

            if (uiGuildIcon != null)
                uiGuildIcon.SetDataByDataId(options.iconDataId);

            if (textGuildName != null)
                textGuildName.text = ZString.Format(LanguageManager.GetText(formatKeyGuildName), Data.GuildName);

            if (textLevel != null)
                textLevel.text = ZString.Format(LanguageManager.GetText(formatKeyLevel), Data.Level.ToString("N0"));

            if (textScore != null)
                textScore.text = ZString.Format(LanguageManager.GetText(formatKeyScore), Data.Score.ToString("N0"));

            if (textMessage != null)
                textMessage.text = ZString.Format(LanguageManager.GetText(formatKeyMessage), Data.GuildMessage);

            if (textRank != null)
                textRank.text = ZString.Format(LanguageManager.GetText(formatKeyRank), Data.Rank.ToString("N0"));

            if (textMemberAmount != null)
            {
                textMemberAmount.text = ZString.Format(
                    LanguageManager.GetText(formatKeyMemberAmount),
                    Data.CurrentMembers.ToString("N0"),
                    Data.MaxMembers.ToString("N0"));
            }

            if (autoAcceptRequestsObjects != null && autoAcceptRequestsObjects.Length > 0)
            {
                foreach (GameObject autoAcceptRequestsObject in autoAcceptRequestsObjects)
                {
                    autoAcceptRequestsObject.SetActive(Data.AutoAcceptRequests);
                }
            }

            if (notAutoAcceptRequestsObjects != null && notAutoAcceptRequestsObjects.Length > 0)
            {
                foreach (GameObject notAutoAcceptRequestsObject in notAutoAcceptRequestsObjects)
                {
                    notAutoAcceptRequestsObject.SetActive(!Data.AutoAcceptRequests);
                }
            }
        }

        public void OnClickSendGuildRequest()
        {
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_GUILD_REQUEST.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_GUILD_REQUEST_DESCRIPTION.ToString()), Data.GuildName), false, true, true, false, null, () =>
            {
                GameInstance.ClientGuildHandlers.RequestSendGuildRequest(new RequestSendGuildRequestMessage()
                {
                    guildId = Data.Id,
                }, SendGuildRequestCallback);
            });
        }

        private void SendGuildRequestCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseSendGuildRequestMessage response)
        {
            ClientGuildActions.ResponseSendGuildRequest(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onGuildRequested.Invoke();
        }
    }
}
