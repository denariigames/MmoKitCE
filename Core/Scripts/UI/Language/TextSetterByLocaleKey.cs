using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    public class TextSetterByLocaleKey : MonoBehaviour
    {
        [Tooltip("Priority: get text from `LanguageManager` by `localKeySetting` then if it's not exists, get text from `textByLanguageKeys`, and then `defaultText`")]
        [TextArea]
        public string defaultText;
        [Tooltip("Priority: get text from `LanguageManager` by `localKeySetting` then if it's not exists, get text from `textByLanguageKeys`, and then `defaultText`")]
        public LanguageData[] textByLanguageKeys;
        [Tooltip("Priority: get text from `LanguageManager` by `localKeySetting` then if it's not exists, get text from `textByLanguageKeys`, and then `defaultText`")]
        public UILocaleKeySetting localeKeySetting;
        public TextWrapper textWrapper;
        [InspectorButton(nameof(UpdateUI))]
        public bool updateUI;
        private string _currentLanguageKey;

        public string Text
        {
            get
            {
                if (!string.IsNullOrEmpty(localeKeySetting) && LanguageManager.Texts.ContainsKey(localeKeySetting))
                    return LanguageManager.GetText(localeKeySetting, defaultText);
                return Language.GetText(textByLanguageKeys, defaultText);
            }
        }

        private void Update()
        {
            if (!textWrapper || LanguageManager.CurrentLanguageKey.Equals(_currentLanguageKey))
                return;
            _currentLanguageKey = LanguageManager.CurrentLanguageKey;
            textWrapper.text = Text;
        }

        private void UpdateUI()
        {
            textWrapper.text = defaultText;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [ContextMenu("Set Attached TextWrapper Component To Field")]
        public void SetAttachedTextWrapperComponentToField()
        {
            textWrapper = GetComponent<TextWrapper>();
        }

        [ContextMenu("Set Current Text Value As Default Text")]
        public void SetCurrentTextValueAsDefaultText()
        {
            defaultText = textWrapper.text;
        }
    }
}
