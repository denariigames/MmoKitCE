using System.Collections.Generic;
using UnityEngine;

namespace Insthync.AudioManager
{
    [DefaultExecutionOrder(100)]
    public class AudioSettingManager : MonoBehaviour
    {
        private Dictionary<string, AudioSetting> _defaultVolumeSettings = new Dictionary<string, AudioSetting>();
        private Dictionary<string, AudioSlider> _sliders = new Dictionary<string, AudioSlider>();
        private Dictionary<string, AudioToggle> _toggles = new Dictionary<string, AudioToggle>();

        private void OnEnable()
        {
            _defaultVolumeSettings.Clear();
            foreach (KeyValuePair<string, AudioSetting> kv in AudioManager.Singleton.VolumeSettings)
            {
                _defaultVolumeSettings[kv.Key] = kv.Value.Clone();
            }
            AudioSlider[] sliderComps = GetComponentsInChildren<AudioSlider>(true);
            foreach (AudioSlider sliderComp in sliderComps)
            {
                _sliders[sliderComp.SettingId] = sliderComp;
            }
            AudioToggle[] toggleComps = GetComponentsInChildren<AudioToggle>(true);
            foreach (AudioToggle toggleComp in toggleComps)
            {
                _toggles[toggleComp.SettingId] = toggleComp;
            }
        }

        public void ResetAudioSetting()
        {
            foreach (var kv in _defaultVolumeSettings)
            {
                if (_sliders.ContainsKey(kv.Key))
                    _sliders[kv.Key].slider.SetValueWithoutNotify(kv.Value.LevelSetting);
                if (_toggles.ContainsKey(kv.Key))
                    _toggles[kv.Key].toggle.SetIsOnWithoutNotify(kv.Value.IsOn);
                AudioManager.Singleton.VolumeSettings[kv.Key] = kv.Value.Clone();
            }
        }
    }
}
