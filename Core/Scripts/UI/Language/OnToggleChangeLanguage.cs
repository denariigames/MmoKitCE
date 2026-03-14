using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(Toggle))]
    public class OnToggleChangeLanguage : MonoBehaviour
    {
        public string languageKey;
        
        private void Start()
        {
            Toggle toggle = GetComponent<Toggle>();
            toggle.isOn = LanguageManager.CurrentLanguageKey.Equals(languageKey);
            toggle.onValueChanged.AddListener(OnToggle);
        }

        public void OnToggle(bool selected)
        {
            if (selected)
            {
                LanguageManager.ChangeLanguage(languageKey);
                UIBase[] uis = FindObjectsOfType<UIBase>();
                for (int i = 0; i < uis.Length; ++i)
                {
                    if (!uis[i].IsVisible())
                        continue;
                    if (uis[i] is IUISelectionEntry selectionEntry)
                        selectionEntry.ForceUpdate();
                        
                }
            }
        }
    }
}
