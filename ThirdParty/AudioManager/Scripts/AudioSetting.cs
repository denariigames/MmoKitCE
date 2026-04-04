using UnityEngine;
using UnityEngine.Serialization;

namespace Insthync.AudioManager
{
    [System.Serializable]
    public class AudioSetting
    {
        public const string KeyVolumeOnPrefix = "SETTING_VOLUME_ON_";
        public const string KeyVolumeLevelPrefix = "SETTING_VOLUME_LEVEL_";
        public string id;
        [FormerlySerializedAs("maxVolumeRate")]
        [Range(0.01f, 1f)]
        public float volumeScale = 1f;

        private bool? _isOn;
        private float? _levelSetting;

        public bool IsOn
        {
            get
            {
                if (!_isOn.HasValue)
                    _isOn = PlayerPrefs.GetInt(KeyVolumeOnPrefix + id, 1) == 0 ? false : true;
                return _isOn.Value;
            }
            set
            {
                _isOn = value;
                PlayerPrefs.SetInt(KeyVolumeOnPrefix + id, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public float LevelSetting
        {
            get
            {
                if (!_levelSetting.HasValue)
                    _levelSetting = PlayerPrefs.GetFloat(KeyVolumeLevelPrefix + id, 1);
                return _levelSetting.Value;
            }
            set
            {
                _levelSetting = value;
                PlayerPrefs.SetFloat(KeyVolumeLevelPrefix + id, value);
                PlayerPrefs.Save();
            }
        }

        public float Level
        {
            get
            {
                if (!IsOn)
                    return 0;
                return LevelSetting * volumeScale;
            }
        }

        public AudioSetting Clone()
        {
            return new AudioSetting()
            {
                id = id,
                IsOn = IsOn,
                LevelSetting = LevelSetting,
            };
        }
    }
}
