using UnityEngine;

namespace Insthync.AudioManager
{
    public class AudioComponent : MonoBehaviour
    {
        public AudioComponentSettingType type;
        public string otherSettingId;

        public string SettingId
        {
            get
            {
                switch (type)
                {
                    case AudioComponentSettingType.Master:
                        return AudioManager.Singleton.masterVolumeSetting.id;
                    case AudioComponentSettingType.Bgm:
                        return AudioManager.Singleton.bgmVolumeSetting.id;
                    case AudioComponentSettingType.Sfx:
                        return AudioManager.Singleton.sfxVolumeSetting.id;
                    case AudioComponentSettingType.Ambient:
                        return AudioManager.Singleton.ambientVolumeSetting.id;
                }
                return otherSettingId;
            }
        }
    }
}
