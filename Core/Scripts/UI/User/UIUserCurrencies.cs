using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIUserCurrencies : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Gold Amount}")]
        public UILocaleKeySetting formatKeyUserGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);
        [Tooltip("Format => {0} = {Gold Amount}")]
        public UILocaleKeySetting formatKeyTotalGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);
        [Tooltip("Format => {0} = {Cash Amount}")]
        public UILocaleKeySetting formatKeyUserCash = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CASH);

        [Header("UI Elements")]
        public TextWrapper uiTextUserGold;
        public TextWrapper uiTextTotalGold;
        public TextWrapper uiTextUserCash;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextUserGold = null;
            uiTextTotalGold = null;
            uiTextUserCash = null;
        }

        private void Update()
        {
            int amount = 0;
            if (uiTextUserGold != null)
            {
                if (GameInstance.PlayingCharacter != null)
                    amount = GameInstance.PlayingCharacter.UserGold;
                uiTextUserGold.text = ZString.Format(
                    LanguageManager.GetText(formatKeyUserGold),
                    amount.ToString("N0"));
            }

            if (uiTextTotalGold != null)
            {
                if (GameInstance.PlayingCharacter != null)
                {
                    switch (GameInstance.Singleton.goldStoreMode)
                    {
                        case GoldStoreMode.UserGoldOnly:
                            amount = GameInstance.PlayingCharacter.UserGold;
                            break;
                        default:
                            amount = GameInstance.PlayingCharacter.UserGold + GameInstance.PlayingCharacter.Gold;
                            break;
                    }
                }
                uiTextTotalGold.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTotalGold),
                    amount.ToString("N0"));
            }

            if (uiTextUserCash != null)
            {
                if (GameInstance.PlayingCharacter != null)
                    amount = GameInstance.PlayingCharacter.UserCash;
                uiTextUserCash.text = ZString.Format(
                    LanguageManager.GetText(formatKeyUserCash),
                    amount.ToString("N0"));
            }
        }
    }
}
