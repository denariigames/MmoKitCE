using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIBlendshapeOption : UISelectionEntry<PlayerCharacterBlendshapeComponent.BlendshapeOption>
    {
        public delegate void SetBlendshapeValueDelegate(int hashedSettingId, float value);

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public Image imageIcon;
        public Slider sliderValue;

        public UIBlendshapeManager Manager { get; set; }
        public PlayerCharacterBlendshapeComponent Component { get; set; }
        public int HashedSettingId { get; set; }
        public int Index { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            imageIcon = null;
            sliderValue = null;
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

            if (sliderValue != null)
            {
                sliderValue.onValueChanged.RemoveListener(OnSliderValueChanged);
                sliderValue.onValueChanged.AddListener(OnSliderValueChanged);
                sliderValue.minValue = Data.minValue;
                sliderValue.maxValue = Data.maxValue;
                sliderValue.value = Data.defaultValue;
            }
        }

        private void OnSliderValueChanged(float value)
        {
            Manager.onSetBlendshapeValue?.Invoke(HashedSettingId, value);
            Component.SetData(Index, value);
        }
    }
}