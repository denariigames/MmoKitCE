using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Insthync.AudioManager
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager s_singleton;
        public static AudioManager Singleton
        {
            get
            {
                if (s_singleton != null)
                    return s_singleton;
                return new GameObject("_AudioManager").AddComponent<AudioManager>();
            }
            private set
            {
                s_singleton = value;
            }
        }

        public AudioSetting masterVolumeSetting = new AudioSetting() { id = "MASTER" };
        public AudioSetting bgmVolumeSetting = new AudioSetting() { id = "BGM" };
        public AudioSetting sfxVolumeSetting = new AudioSetting() { id = "SFX" };
        public AudioSetting ambientVolumeSetting = new AudioSetting() { id = "AMBIENT" };
        public AudioSetting[] otherVolumeSettings;
    	public UnityEvent<string, float> onSetVolumeLevel;

        public Dictionary<string, AudioSetting> VolumeSettings { get; private set; } = new Dictionary<string, AudioSetting>();

        private void Awake()
        {
            if (s_singleton != null)
            {
                Destroy(gameObject);
                return;
            }
            s_singleton = this;
            DontDestroyOnLoad(gameObject);

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                AudioListener.pause = true;

            VolumeSettings[masterVolumeSetting.id] = masterVolumeSetting;
            VolumeSettings[bgmVolumeSetting.id] = bgmVolumeSetting;
            VolumeSettings[sfxVolumeSetting.id] = sfxVolumeSetting;
            VolumeSettings[ambientVolumeSetting.id] = ambientVolumeSetting;
            if (otherVolumeSettings != null)
            {
                foreach (AudioSetting otherVolumeSetting in otherVolumeSettings)
                {
                    VolumeSettings[otherVolumeSetting.id] = otherVolumeSetting;
                }
            }
        }

        public void SetVolumeIsOn(string id, bool isOn)
        {
            if (VolumeSettings.ContainsKey(id))
                VolumeSettings[id].IsOn = isOn;
        }

        public bool GetVolumeIsOn(string id)
        {
            if (VolumeSettings.ContainsKey(id))
                return VolumeSettings[id].IsOn;
            return false;
        }

        public float GetVolumeLevel(string id)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                return 0;

            if (VolumeSettings.ContainsKey(id))
                return VolumeSettings[id].Level * masterVolumeSetting.Level;
            return 0;
        }

        public void SetVolumeLevelSetting(string id, float level)
        {
            if (VolumeSettings.ContainsKey(id))
            {
                VolumeSettings[id].LevelSetting = level;
            	onSetVolumeLevel?.Invoke(id, level);
            }
        }

        public float GetVolumeLevelSetting(string id)
        {
            if (VolumeSettings.ContainsKey(id))
                return VolumeSettings[id].LevelSetting;
            return 0;
        }

		public float GetBgmVolume()
		{
			return bgmVolumeSetting.Level * masterVolumeSetting.Level;
        }

		public float GetSfxVolume()
		{
			return sfxVolumeSetting.Level * masterVolumeSetting.Level;
        }

		public float GetAmbientVolume()
		{
			return ambientVolumeSetting.Level * masterVolumeSetting.Level;
        }

        public static void PlaySfxClipAtPoint(AudioClip audioClip, Vector3 position, float volumeScale = 1f)
        {
            if (Application.isBatchMode || AudioListener.pause || audioClip == null) return;
            AudioSource.PlayClipAtPoint(audioClip, position, (Singleton == null ? 1f : Singleton.GetSfxVolume()) * volumeScale);
        }

	    public static void PlaySfxClipAtPoint(AudioClip audioClip, Vector3 position, float minDistance, float maxDistance, float volumeScale = 1f, float spatialBlend = 1f, AudioRolloffMode audioRolloffMode = AudioRolloffMode.Linear)
	    {
	        float volume = (Singleton == null ? 1f : Singleton.GetSfxVolume()) * volumeScale;

	        GameObject gameObject = new GameObject("_OneShotAudio");
	        gameObject.transform.position = position;
	        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
	        audioSource.clip = audioClip;
	        audioSource.volume = volume;
	        audioSource.spatialBlend = spatialBlend;
	        audioSource.rolloffMode = audioRolloffMode;
	        audioSource.minDistance = minDistance;
	        audioSource.maxDistance = maxDistance;
	        audioSource.Play();
	        Object.Destroy(gameObject, audioClip.length);
	    }

        public static void PlaySfxClipAtAudioSource(AudioClip audioClip, AudioSource audioSource, float volumeScale = 1f)
        {
            if (Application.isBatchMode || AudioListener.pause || audioClip == null || audioSource == null) return;
            audioSource.PlayOneShot(audioClip, (Singleton == null ? 1f : Singleton.GetSfxVolume()) * volumeScale);
        }
    }
}
