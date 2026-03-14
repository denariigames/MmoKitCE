using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MultiplayerARPG
{
    public class UILanguageText : MonoBehaviour
    {
        public string dataKey;
        [TextArea(1, 10)]
        public string defaultText;
        public Text unityText;
        public TextMeshProUGUI textMeshText;
        private string _currentLanguageKey;

        private void Awake()
        {
            if (unityText == null)
                unityText = GetComponent<Text>();
            if (textMeshText == null)
                textMeshText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (_currentLanguageKey != LanguageManager.CurrentLanguageKey)
            {
                if (LanguageManager.Texts.TryGetValue(dataKey, out string text))
                {
                    if (unityText != null)
                        unityText.text = text;
                    if (textMeshText != null)
                        textMeshText.text = text;
                }
                else
                {
                    if (unityText != null)
                        unityText.text = defaultText;
                    if (textMeshText != null)
                        textMeshText.text = defaultText;
                }
                _currentLanguageKey = LanguageManager.CurrentLanguageKey;
            }
        }

        void OnValidate()
        {
            if (unityText != null)
                unityText.text = defaultText;
            if (textMeshText != null)
                textMeshText.text = defaultText;
        }
    }
}
