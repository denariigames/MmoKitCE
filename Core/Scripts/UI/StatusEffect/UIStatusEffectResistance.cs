using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIStatusEffectResistance : UISelectionEntry<UIStatusEffectResistanceData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Level}, {1} = {Chance * 100}")]
        public UILocaleKeySetting formatKeyEntry = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_RESISTANCE_ENTRY);
        [Tooltip("Format => {0} = {Entries}")]
        public UILocaleKeySetting formatKeyEntriesOnly = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Status Effect Title}, {1} = {Entries}")]
        public UILocaleKeySetting formatKeyTitleWithEntries = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_RESISTANCE_ENTRIES);
        public string entriesSeparator = ", ";

        [Header("UI Elements")]
        public UIGameDataElements uiGameDataElements;
        public TextWrapper uiTextEntriesOnly;
        public TextWrapper uiTextTitleWithEntries;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiGameDataElements = null;
            uiTextEntriesOnly = null;
            uiTextTitleWithEntries = null;
        }

        protected override void UpdateData()
        {
            if (uiGameDataElements != null)
            {
                uiGameDataElements.Update(Data.statusEffect);
            }

            if (uiTextEntriesOnly != null)
            {
                uiTextEntriesOnly.text = ZString.Format(
                    LanguageManager.GetText(formatKeyEntriesOnly),
                    Data.statusEffect.GetResistanceEntriesText(Data.amount, LanguageManager.GetText(formatKeyEntry), entriesSeparator));
            }

            if (uiTextTitleWithEntries != null)
            {
                uiTextTitleWithEntries.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitleWithEntries),
                    Data.statusEffect.Title,
                    Data.statusEffect.GetResistanceEntriesText(Data.amount, LanguageManager.GetText(formatKeyEntry), entriesSeparator));
            }
        }
    }
}
