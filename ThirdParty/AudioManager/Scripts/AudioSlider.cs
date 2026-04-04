using UnityEngine;
using UnityEngine.UI;

namespace Insthync.AudioManager
{
    [RequireComponent(typeof(Slider))]
    public class AudioSlider : AudioComponent
    {
        public Slider slider { get; private set; }
        public float volumeChangeStepOnClick = 0.01f;
        public Button buttonIncrease;
        public Button buttonDecrease;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.onValueChanged.RemoveListener(OnValueChanged);
            slider.onValueChanged.AddListener(OnValueChanged);
            if (buttonIncrease != null)
                buttonIncrease.onClick.AddListener(OnClickIncrease);
            if (buttonDecrease != null)
                buttonDecrease.onClick.AddListener(OnClickDecrease);
        }

        private void OnDestroy()
        {
            if (buttonIncrease != null)
                buttonIncrease.onClick.RemoveListener(OnClickIncrease);
            if (buttonDecrease != null)
                buttonDecrease.onClick.RemoveListener(OnClickDecrease);
        }

        private void OnValueChanged(float level)
        {
            AudioManager.Singleton.SetVolumeLevelSetting(SettingId, level);
        }

        private void OnEnable()
        {
            slider.value = AudioManager.Singleton.GetVolumeLevelSetting(SettingId);
        }

        public void OnClickIncrease()
        {
            slider.value += volumeChangeStepOnClick;
        }

        public void OnClickDecrease()
        {
            slider.value -= volumeChangeStepOnClick;
        }
    }
}
