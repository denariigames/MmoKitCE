using Cysharp.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UIArmorAmount : UISelectionEntry<UIArmorAmountData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyAmountOnly = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Armor Title}, {1} = {Amount}")]
        [FormerlySerializedAs("formatKeyAmount")]
        public UILocaleKeySetting formatKeyTitleWithAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ARMOR_AMOUNT);

        [Header("UI Elements")]
        public UIGameDataElements uiGameDataElements;
        public TextWrapper uiTextAmountOnly;
        [FormerlySerializedAs("uiTextAmount")]
        public TextWrapper uiTextTitleWithAmount;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiGameDataElements = null;
            uiTextAmountOnly = null;
            uiTextTitleWithAmount = null;
        }

        protected override void UpdateData()
        {
            if (uiGameDataElements != null)
            {
                uiGameDataElements.Update(Data.damageElement);
            }

            if (uiTextAmountOnly != null)
            {
                uiTextAmountOnly.text = ZString.Format(
                    LanguageManager.GetText(formatKeyAmountOnly),
                    Data.amount.ToString("N0"));
            }

            if (uiTextTitleWithAmount != null)
            {
                uiTextTitleWithAmount.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitleWithAmount),
                    Data.damageElement.Title,
                    Data.amount.ToString("N0"));
            }
        }
    }
}
