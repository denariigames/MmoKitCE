using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class Language
    {
        public string languageKey;
        public List<LanguageData> dataList = new List<LanguageData>();

        public bool ContainKey(string key)
        {
            foreach (LanguageData entry in dataList)
            {
                if (string.IsNullOrEmpty(entry.key))
                    continue;
                if (entry.key.Equals(key))
                    return true;
            }
            return false;
        }

        public static string GetText(IEnumerable<LanguageData> langs, string defaultValue)
        {
            if (langs != null)
            {
                foreach (LanguageData entry in langs)
                {
                    if (string.IsNullOrEmpty(entry.key))
                        continue;
                    if (entry.key.Equals(LanguageManager.CurrentLanguageKey))
                        return entry.value;
                }
            }
            return defaultValue;
        }

        public static string GetTextByLanguageKey(IEnumerable<LanguageData> langs, string languageKey, string defaultValue)
        {
            if (langs != null)
            {
                foreach (LanguageData entry in langs)
                {
                    if (string.IsNullOrEmpty(entry.key))
                        continue;
                    if (entry.key.Equals(languageKey))
                        return entry.value;
                }
            }
            return defaultValue;
        }
    }

    [System.Serializable]
    public struct LanguageData
    {
        public string key;
        [TextArea]
        public string value;
    }

    [System.Serializable]
    public struct LanguageTextSetting
    {
        [Tooltip("Priority: get text from `LanguageManager` by `localKeySetting` then if it's not exists, get text from `languageSpecificTexts`, and then `defaultText`")]
        [TextArea]
        public string defaultText;
        [Tooltip("Priority: get text from `LanguageManager` by `localKeySetting` then if it's not exists, get text from `languageSpecificTexts`, and then `defaultText`")]
        public LanguageData[] languageSpecificTexts;
        [Tooltip("Priority: get text from `LanguageManager` by `localKeySetting` then if it's not exists, get text from `languageSpecificTexts`, and then `defaultText`")]
        public string localeKeySetting;

        public string Text
        {
            get
            {
                if (!string.IsNullOrEmpty(localeKeySetting) && LanguageManager.Texts.ContainsKey(localeKeySetting))
                    return LanguageManager.GetText(localeKeySetting, defaultText);
                return Language.GetText(languageSpecificTexts, defaultText);
            }
        }

        public void Update(Text unityText)
        {
            if (unityText != null)
                unityText.text = Text;
        }

        public void Update(TextMeshProUGUI textMeshText)
        {
            if (textMeshText != null)
                textMeshText.text = Text;
        }
    }
}
