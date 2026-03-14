using UnityEngine;

namespace Insthync.AudioManager
{
    public class AudioSourceSetterWithoutControls : AudioComponent
    {
        private AudioSource _cacheAudioSource;
        private int _dirtyVolume;

        private void Awake()
        {
            _dirtyVolume = -1;
            _cacheAudioSource = GetComponent<AudioSource>();
            if (_cacheAudioSource == null)
            {
                _cacheAudioSource = gameObject.AddComponent<AudioSource>();
                _cacheAudioSource.spatialBlend = 1f;
            }
        }

        private void Update()
        {
            float volume = AudioManager.Singleton.GetVolumeLevel(SettingId);
            int intVolume = (int)(volume * 100);
            if (_dirtyVolume != intVolume)
            {
                _dirtyVolume = intVolume;
                _cacheAudioSource.volume = volume;
            }
        }
    }
}
