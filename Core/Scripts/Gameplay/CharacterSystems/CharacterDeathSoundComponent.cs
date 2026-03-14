using Insthync.AudioManager;
using Insthync.ManagedUpdating;
using UnityEngine;

namespace MultiplayerARPG
{
    public class CharacterDeathSoundComponent : BaseGameEntityComponent<BaseCharacterEntity>, IManagedUpdate
    {
        [System.Serializable]
        public struct DeathSoundData
        {
            public AudioClip[] randomAudioClips;

            public AudioClip GetRandomedAudioClip()
            {
                if (randomAudioClips == null || randomAudioClips.Length == 0)
                    return null;
                return randomAudioClips[Random.Range(0, randomAudioClips.Length)];
            }
        }

        public AudioSource audioSource;
        public AudioComponentSettingType settingType = AudioComponentSettingType.Sfx;
        public string otherSettingId;
        public DeathSoundData soundData;
        private bool _dirtyIsDead;

        public string SettingId
        {
            get
            {
                switch (settingType)
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

        private void Start()
        {
            if (!Entity.IsClient)
            {
                enabled = false;
                return;
            }
            if (audioSource == null)
            {
                GameObject audioSourceObject = new GameObject("_DeathAudioSource");
                audioSourceObject.transform.parent = CacheTransform;
                audioSourceObject.transform.localPosition = Vector3.zero;
                audioSourceObject.transform.localRotation = Quaternion.identity;
                audioSourceObject.transform.localScale = Vector3.one;
                audioSource = audioSourceObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f;
            }
        }

        private void OnEnable()
        {
            UpdateManager.Register(this);
        }

        private void OnDisable()
        {
            UpdateManager.Unregister(this);
        }

        public void ManagedUpdate()
        {
            if (_dirtyIsDead != Entity.IsDead())
            {
                _dirtyIsDead = Entity.IsDead();
                if (_dirtyIsDead)
                    PlaySound();
            }
        }

        public void PlaySound()
        {
            if (Application.isBatchMode || AudioListener.pause)
                return;
            audioSource.PlayOneShot(soundData.GetRandomedAudioClip(), AudioManager.Singleton.GetVolumeLevel(SettingId));
        }
    }
}
