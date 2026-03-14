using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    public class TextSetterByGameDataDescription : MonoBehaviour
    {
        public BaseGameData gameData;
        public TextWrapper textWrapper;
        [InspectorButton(nameof(UpdateUI))]
        public bool updateUI;
        private string _currentLanguageKey;

        private void Update()
        {
            if (!textWrapper || LanguageManager.CurrentLanguageKey.Equals(_currentLanguageKey))
                return;
            _currentLanguageKey = LanguageManager.CurrentLanguageKey;
            textWrapper.text = gameData.Description;
        }

        private void UpdateUI()
        {
            textWrapper.text = gameData.Description;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
