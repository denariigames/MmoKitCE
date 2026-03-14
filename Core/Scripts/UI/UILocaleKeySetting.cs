using Cysharp.Text;
using Insthync.UnityEditorUtils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UILocaleKeySetting
    {
        public bool useCustomValue;
        [BoolShowConditional(nameof(useCustomValue), false)]
        public UIFormatKeys localeKey;
        [StringShowConditional(nameof(localeKey), nameof(UIFormatKeys.UI_CUSTOM))]
        public string customKey;
        [BoolShowConditional(nameof(useCustomValue), true)]
        public string customValue;
        public UILocaleKeySetting(UIFormatKeys localeKey)
        {
            this.localeKey = localeKey;
            useCustomValue = false;
            customKey = string.Empty;
            customValue = string.Empty;
        }

        public string ToFormat()
        {
            if (useCustomValue)
                return customValue;
            return LanguageManager.GetText(ToString());
        }

        public override string ToString()
        {
            if (useCustomValue)
                return ZString.Concat(LanguageManager.RETURN_KEY_AS_DEFAULT_VALUE_PREFIX, customValue);
            if (localeKey == UIFormatKeys.UI_CUSTOM)
                return customKey;
            return localeKey.ToString();
        }

        public string GetText()
        {
            return LanguageManager.GetText(this);
        }

        public static implicit operator string(UILocaleKeySetting keySetting)
        {
            return keySetting.ToString();
        }
    }
}
