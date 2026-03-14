using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIFaction : UISelectionEntry<Faction>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public Image imageIcon;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            uiTextDescription = null;
            imageIcon = null;
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
        }

        public void SetDataByDataId(int dataId)
        {
            Faction faction;
            if (GameInstance.Factions.TryGetValue(dataId, out faction))
                Data = faction;
            else
                Data = null;
        }
    }
}
