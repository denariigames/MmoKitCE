using TMPro;
using UnityEngine.UI;

namespace Insthync.AudioManager
{
    public class AudioText : AudioComponent
    {
        public Text text;
        public TMP_Text tmpText;
        public string format = "{0}%";

        private void Awake()
        {
            if (text == null)
                text = GetComponent<Text>();
            if (tmpText == null)
                tmpText = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            string textVal = string.Format(format, (AudioManager.Singleton.GetVolumeLevelSetting(SettingId) * 100f).ToString("N0"));
            if (text != null)
                text.text = textVal;
            if (tmpText != null)
                tmpText.text = textVal;
        }
    }
}
