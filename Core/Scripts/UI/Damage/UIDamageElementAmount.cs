using Cysharp.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UIDamageElementAmount : UISelectionEntry<UIDamageElementAmountData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Min Damage}, {1} = {Max Damage}")]
        public UILocaleKeySetting formatKeyAmountOnly = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_AMOUNT);
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Min Damage}, {2} = {Max Damage}")]
        public UILocaleKeySetting formatKeyAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_WITH_ELEMENTAL);

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
            if (uiTextTitleWithAmount != null)
            {
                uiTextTitleWithAmount.text = ZString.Format(
                    LanguageManager.GetText(formatKeyAmount),
                    Data.damageElement.Title,
                    Data.amount.min.ToString("N0"),
                    Data.amount.max.ToString("N0"));
            }
        }
    }
}
