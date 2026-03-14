using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public class UIGuildCreate : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD);
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGoldNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Target Amount}")]
        public UILocaleKeySetting formatKeySimpleRequireGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Current Cash Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireCash = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_CASH);
        [Tooltip("Format => {0} = {Current Cash Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireCashNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_CASH_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Target Amount}")]
        public UILocaleKeySetting formatKeySimpleRequireCash = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public InputFieldWrapper inputFieldGuildName;
        [FormerlySerializedAs("uiRequireItems")]
        public UIItemAmounts uiRequireItemAmounts;
        public UICurrencyAmounts uiRequireCurrencyAmounts;
        [FormerlySerializedAs("textRequireGold")]
        public TextWrapper uiTextRequireGold;
        public TextWrapper uiTextSimpleRequireGold;
        [FormerlySerializedAs("textRequireCash")]
        public TextWrapper uiTextRequireCash;
        public TextWrapper uiTextSimpleRequireCash;
        public UnityEvent onGuildCreate = new UnityEvent();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            inputFieldGuildName = null;
            uiRequireItemAmounts = null;
            uiRequireCurrencyAmounts = null;
            uiTextRequireGold = null;
            uiTextSimpleRequireGold = null;
            uiTextRequireCash = null;
            uiTextSimpleRequireCash = null;
            onGuildCreate?.RemoveAllListeners();
            onGuildCreate = null;
            GameInstance.OnSetGuildDataEvent -= GameInstance_OnSetGuildDataEvent;
        }

        protected virtual void OnEnable()
        {
            IPlayerCharacterData owningCharacter = GameInstance.PlayingCharacter;
            SocialSystemSetting systemSetting = GameInstance.Singleton.SocialSystemSetting;
            if (uiTextRequireGold != null)
            {
                int gold = owningCharacter.Gold;
                uiTextRequireGold.text = ZString.Format(
                    gold >= systemSetting.CreateGuildRequiredGold ?
                        LanguageManager.GetText(formatKeyRequireGold) :
                        LanguageManager.GetText(formatKeyRequireGoldNotEnough),
                    gold.ToString("N0"),
                    systemSetting.CreateGuildRequiredGold.ToString("N0"));
            }

            if (uiTextSimpleRequireGold != null)
                uiTextSimpleRequireGold.text = ZString.Format(LanguageManager.GetText(formatKeySimpleRequireGold), systemSetting.CreateGuildRequiredGold.ToString("N0"));

            if (uiTextRequireCash != null)
            {
                int cash = owningCharacter.UserCash;
                uiTextRequireCash.text = ZString.Format(
                    cash >= systemSetting.CreateGuildRequiredCash ?
                        LanguageManager.GetText(formatKeyRequireCash) :
                        LanguageManager.GetText(formatKeyRequireCashNotEnough),
                    cash.ToString("N0"),
                    systemSetting.CreateGuildRequiredCash.ToString("N0"));
            }

            if (uiTextSimpleRequireCash != null)
                uiTextSimpleRequireCash.text = ZString.Format(LanguageManager.GetText(formatKeySimpleRequireCash), systemSetting.CreateGuildRequiredCash.ToString("N0"));

            if (uiRequireItemAmounts != null)
            {
                uiRequireItemAmounts.displayType = UIItemAmounts.DisplayType.Requirement;
                uiRequireItemAmounts.Show();
                uiRequireItemAmounts.Data = systemSetting.CreateGuildRequireItems;
            }

            if (uiRequireCurrencyAmounts != null)
            {
                uiRequireCurrencyAmounts.displayType = UICurrencyAmounts.DisplayType.Requirement;
                uiRequireCurrencyAmounts.Show();
                uiRequireCurrencyAmounts.Data = systemSetting.CreateGuildRequireCurrencies;
            }
        }

        public void OnClickCreate()
        {
            inputFieldGuildName.interactable = false;
            GameInstance.ClientGuildHandlers.RequestCreateGuild(new RequestCreateGuildMessage()
            {
                guildName = inputFieldGuildName.text,
            }, CreateGuildCallback);
        }

        private void CreateGuildCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCreateGuildMessage response)
        {
            inputFieldGuildName.interactable = true;
            ClientGuildActions.ResponseCreateGuild(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            inputFieldGuildName.text = string.Empty;
            if (GameInstance.JoinedGuild == null)
            {
                GameInstance.OnSetGuildDataEvent += GameInstance_OnSetGuildDataEvent;
                return;
            }
            GameInstance_OnSetGuildDataEvent(GameInstance.JoinedGuild);
        }

        private void GameInstance_OnSetGuildDataEvent(GuildData guildData)
        {
            GameInstance.OnSetGuildDataEvent -= GameInstance_OnSetGuildDataEvent;
            onGuildCreate.Invoke();
            Hide();
        }
    }
}
