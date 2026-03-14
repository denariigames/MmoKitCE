using UnityEngine;

namespace MultiplayerARPG {
    public class TextChangingLoop : MonoBehaviour
    {
        [System.Serializable]
        public struct Setting
        {
            public LanguageTextSetting text;
            public float showDuration;
        }

        public TextWrapper uiText;
        public Setting[] settings = new Setting[0];

        public int currentIndex = -1;
        private float _lastShowTime;

        private void OnEnable()
        {
            currentIndex = -1;
        }

        private void Update()
        {
            float currentTime = Time.time;
            if (currentIndex < 0 || currentTime - _lastShowTime >= settings[currentIndex].showDuration)
            {
                _lastShowTime = currentTime;
                if (currentIndex < 0)
                    currentIndex = 0;
                else
                    currentIndex++;
                if (currentIndex >= settings.Length)
                    currentIndex = 0;
                uiText.text = settings[currentIndex].text.Text;
            }
        }
    }
}