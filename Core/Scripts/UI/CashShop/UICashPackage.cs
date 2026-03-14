using Cysharp.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UICashPackage : UISelectionEntry<CashPackage>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Sell Price}")]
        public UILocaleKeySetting formatKeySellPrice = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SELL_PRICE);
        [Tooltip("Format => {0} = {Cash Amount}")]
        [FormerlySerializedAs("formatKeyRewardCash")]
        public UILocaleKeySetting formatKeyCashAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REWARD_CASH);

        [Header("UI Elements")]
        public UICashPackages uiCashPackages;
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public Image imageIcon;
        public RawImage rawImageExternalIcon;
        public TextWrapper uiTextSellPrice;
        public TextWrapper uiTextCashAmount;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiCashPackages = null;
            uiTextTitle = null;
            uiTextDescription = null;
            imageIcon = null;
            rawImageExternalIcon = null;
            uiTextSellPrice = null;
            uiTextCashAmount = null;
            _data = null;
        }

        protected override void UpdateData()
        {
            if (uiTextTitle != null)
            {
                uiTextTitle.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.Title);
            }

            if (uiTextDescription != null)
            {
                uiTextDescription.text = ZString.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    Data == null ? LanguageManager.GetUnknowDescription() : Data.Description);
            }

            imageIcon.SetImageGameDataIcon(Data);
            rawImageExternalIcon.SetRawImageExternalTexture(Data.ExternalIconUrl);

            if (uiTextSellPrice != null)
            {
                uiTextSellPrice.text = ZString.Format(
                    LanguageManager.GetText(formatKeySellPrice),
                    Data == null ? "0" : Data.GetSellPrice());
            }

            if (uiTextCashAmount != null)
            {
                uiTextCashAmount.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCashAmount),
                    Data == null ? "0" : Data.CashAmount.ToString("N0"));
            }
        }

        public void OnClickBuy()
        {
            if (uiCashPackages != null)
                uiCashPackages.Buy(Data.Id);
        }
    }
}
