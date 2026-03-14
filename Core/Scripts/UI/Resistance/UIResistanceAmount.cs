using Cysharp.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UIResistanceAmount : UISelectionEntry<UIResistanceAmountData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyAmountOnly = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Resistance Title}, {1} = {Amount * 100}")]
        [FormerlySerializedAs("formatKeyAmount")]
        public UILocaleKeySetting formatKeyTitleWithAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RESISTANCE_AMOUNT);

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
                    (Data.amount * 100).ToString("N2"));
            }

            if (uiTextTitleWithAmount != null)
            {
                uiTextTitleWithAmount.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitleWithAmount),
                    Data.damageElement.Title,
                    (Data.amount * 100).ToString("N2"));
            }
        }
    }
}
