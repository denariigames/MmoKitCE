using System.Collections.Generic;
using UnityEngine;

namespace Insthync.AudioManager
{
    public class AudioSourcesSetterByTags : MonoBehaviour
    {
        public enum NoTagSetting
        {
            Bgm,
            Sfx,
            Ambient,
        }

        [System.Serializable]
        public struct CustomIdTag
        {
            public string tag;
            public string id;
        }

        public NoTagSetting noTagSetting = NoTagSetting.Sfx;
        public string bgmTag = "BGM";
        public string sfxTag = "SFX";
        public string ambientTag = "AMBIENT";
        public CustomIdTag[] customIdTags = new CustomIdTag[0];
        public bool setOnStart = false;

        private void Start()
        {
            if (setOnStart)
                Set();
        }

        [ContextMenu("Set")]
        public void Set()
        {
            Dictionary<string, string> tempCustomIdTags = new Dictionary<string, string>();
            foreach (CustomIdTag customIdTag in customIdTags)
            {
                tempCustomIdTags[customIdTag.tag] = customIdTag.id;
            }
            AudioSourceSetterWithoutControls tempComp;
            AudioSource[] sources = FindObjectsOfType<AudioSource>(true);
            foreach (AudioSource source in sources)
            {
                tempComp = source.gameObject.GetComponent<AudioSourceSetterWithoutControls>();
                if (tempComp) continue;
                tempComp = source.gameObject.AddComponent<AudioSourceSetterWithoutControls>();
                if (source.gameObject.CompareTag(bgmTag))
                {
                    tempComp.type = AudioComponentSettingType.Bgm;
                }
                else if (source.gameObject.CompareTag(sfxTag))
                {
                    tempComp.type = AudioComponentSettingType.Sfx;
                }
                else if (source.gameObject.CompareTag(ambientTag))
                {
                    tempComp.type = AudioComponentSettingType.Ambient;
                }
                else if (tempCustomIdTags.ContainsKey(source.gameObject.tag))
                {
                    tempComp.type = AudioComponentSettingType.Other;
                    tempComp.otherSettingId = tempCustomIdTags[tempComp.gameObject.tag];
                }
                else
                {
                    switch (noTagSetting)
                    {
                        case NoTagSetting.Bgm:
                            tempComp.type = AudioComponentSettingType.Bgm;
                            break;
                        case NoTagSetting.Sfx:
                            tempComp.type = AudioComponentSettingType.Sfx;
                            break;
                        case NoTagSetting.Ambient:
                            tempComp.type = AudioComponentSettingType.Ambient;
                            break;
                    }
                }
            }
        }
    }
}
