using UnityEngine;

namespace MultiplayerARPG
{
    public class OnClickChangeLanguage : MonoBehaviour
    {
        public string languageKey;
        public void OnClick()
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
