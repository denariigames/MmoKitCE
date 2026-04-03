using UnityEngine;

namespace Insthync.AudioManager
{
    public class AudioSourceSetter : AudioComponent
    {
        public enum PlayMode
        {
            PlayClipAtAudioSource,
            PlayClipAtPoint,
        }
        public PlayMode playMode = PlayMode.PlayClipAtAudioSource;
        public bool playOnAwake = true;
        public bool playOnEnable = false;
        public AudioClip[] randomClips = new AudioClip[0];
        private AudioSource _cacheAudioSource;
        private bool _isAwaken;
        private int _dirtyVolume;

        private void Awake()
        {
            _dirtyVolume = -1;
            if (playMode == PlayMode.PlayClipAtAudioSource)
            {
                _cacheAudioSource = GetComponent<AudioSource>();
                if (_cacheAudioSource == null)
                {
                    _cacheAudioSource = gameObject.AddComponent<AudioSource>();
                    _cacheAudioSource.spatialBlend = 1f;
                }
                _cacheAudioSource.Stop();
            }

            if (playOnAwake)
                Play();

            _isAwaken = true;
        }

        private void OnEnable()
        {
            if (playOnEnable && _isAwaken)
                Play();
        }

        public void Play()
        {
            if (Application.isBatchMode || AudioListener.pause)
                return;

            AudioClip clip = null;
            if (randomClips.Length > 0)
                clip = randomClips[Random.Range(0, randomClips.Length)];

            // No random clips, try to use clip from audio source
            if (clip == null)
            {
                if (_cacheAudioSource.clip == null)
                    return;
                clip = _cacheAudioSource.clip;
            }

            float volume = AudioManager.Singleton.GetVolumeLevel(SettingId);
            switch (playMode)
            {
                case PlayMode.PlayClipAtAudioSource:
                    _cacheAudioSource.clip = clip;
                    _cacheAudioSource.volume = volume;
                    _cacheAudioSource.Play();
                    break;
                case PlayMode.PlayClipAtPoint:
                    AudioSource.PlayClipAtPoint(clip, transform.position, volume);
                    break;
            }
        }

        public void Pause()
        {
            if (playMode != PlayMode.PlayClipAtAudioSource)
                return;
            _cacheAudioSource.Pause();
        }

        public void UnPause()
        {
            if (playMode != PlayMode.PlayClipAtAudioSource)
                return;
            _cacheAudioSource.UnPause();
        }

        public void Stop()
        {
            if (playMode != PlayMode.PlayClipAtAudioSource)
                return;
            _cacheAudioSource.Stop();
        }

        private void Update()
        {
            float volume = AudioManager.Singleton.GetVolumeLevel(SettingId);
            int intVolume = (int)(volume * 100);
            if (playMode == PlayMode.PlayClipAtAudioSource && _dirtyVolume != intVolume)
            {
                _dirtyVolume = intVolume;
                _cacheAudioSource.volume = volume;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.playOnAwake = false;
                audioSource.volume = 0;
            }
        }
#endif
    }
}
