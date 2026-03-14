using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIBodyPartModelOption : UISelectionEntry<PlayerCharacterBodyPartComponent.ModelOption>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public Image imageIcon;

        public UIBodyPartManager Manager { get; set; }
        public PlayerCharacterBodyPartComponent Component { get; set; }
        public int HashedSettingId { get; set; }
        public int Index { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            imageIcon = null;
            Manager = null;
            Component = null;
            _data = null;
        }

        protected override void UpdateData()
        {
            if (uiTextTitle != null)
            {
                string str = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.Title);
                uiTextTitle.text = str;
            }

            if (imageIcon != null)
            {
                Sprite iconSprite = Data == null ? null : Data.icon;
                imageIcon.gameObject.SetActive(iconSprite != null);
                imageIcon.sprite = iconSprite;
            }
        }
    }
}