using Insthync.AudioManager;
using Insthync.ManagedUpdating;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class GameEffect : PoolDescriptor, IManagedUpdate
    {
        public enum PlayMode
        {
            PlayClipAtPoint,
            PlayClipAtAudioSource
        }

        [Header("Generic Settings")]
        public string effectSocket;
        public bool isLoop;
        public bool stayInPlace = false;
        public float lifeTime;

        [Header("Sound Effect Playing")]
        public PlayMode playMode = PlayMode.PlayClipAtPoint;

        private AudioSource _cacheAudioSource;
        public AudioComponentSettingType settingType = AudioComponentSettingType.Sfx;
        public string otherSettingId;
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
        public AudioClip[] randomSoundEffects = new AudioClip[0];

        private bool _intendToFollowTarget;
        private Transform _followingTarget;
        public Transform FollowingTarget
        {
            get { return _followingTarget; }
            set
            {
                if (stayInPlace || value == null)
                    return;

                _followingTarget = value;
                _intendToFollowTarget = true;
            }
        }

        public Transform CacheTransform { get; private set; }

        private FxCollection _fxCollection;
        public FxCollection FxCollection
        {
            get
            {
                if (_fxCollection == null)
                    _fxCollection = new FxCollection(gameObject);
                return _fxCollection;
            }
        }

        protected float _destroyTime;

        protected virtual void Awake()
        {
            CacheTransform = transform;

            if (playMode == PlayMode.PlayClipAtAudioSource)
            {
                _cacheAudioSource = GetComponent<AudioSource>();
                if (_cacheAudioSource == null)
                {
                    _cacheAudioSource = gameObject.AddComponent<AudioSource>();
                    _cacheAudioSource.spatialBlend = 1f;
                    _cacheAudioSource.playOnAwake = false;
                }
            }
        }

        protected virtual void OnEnable()
        {
            UpdateManager.Register(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.Unregister(this);
        }

        public override void PushBack()
        {
            if (ObjectPrefab != null)
            {
                PoolSystem.PushBack(this);
            }
            else if (gameObject.activeSelf)
            {
                OnPushBack();
                gameObject.SetActive(false);
            }
        }

        public virtual void ManagedUpdate()
        {
            if (_destroyTime >= 0 && _destroyTime - Time.time <= 0)
            {
                PushBack();
                return;
            }
            if (FollowingTarget != null)
            {
                // Following target is not destroyed, follow its position
                CacheTransform.position = FollowingTarget.position;
                CacheTransform.rotation = FollowingTarget.rotation;
            }
            else if (_intendToFollowTarget)
            {
                // Following target destroyed, don't push back immediately, destroy it after some delay
                DestroyEffect();
            }
        }

        public virtual void DestroyEffect()
        {
            FxCollection.SetLoop(false);
            _destroyTime = Time.time + lifeTime;
        }

        public override void InitPrefab()
        {
            if (this == null)
            {
                Debug.LogWarning("The Game Effect is null, this should not happens");
                return;
            }
            FxCollection.InitPrefab();
            base.InitPrefab();
        }

        public override void OnGetInstance()
        {
            Play();
            base.OnGetInstance();
        }

        /// <summary>
        /// Play particle effects and an audio
        /// </summary>
        public virtual void Play()
        {
            if (Application.isBatchMode)
            {
                // Will be destroyed in next frame
                _destroyTime = Time.time;
                return;
            }

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            // Prepare destroy time
            _destroyTime = isLoop ? -1 : Time.time + lifeTime;

            if (!Application.isBatchMode && !AudioListener.pause && randomSoundEffects.Length > 0)
            {
                switch (playMode)
                {
                    case PlayMode.PlayClipAtAudioSource:
                        _cacheAudioSource.clip = randomSoundEffects[Random.Range(0, randomSoundEffects.Length)];
                        _cacheAudioSource.volume = AudioManager.Singleton.GetVolumeLevel(SettingId);
                        _cacheAudioSource.loop = isLoop;
                        _cacheAudioSource.Play();
                        break;
                    case PlayMode.PlayClipAtPoint:
                        AudioSource.PlayClipAtPoint(randomSoundEffects[Random.Range(0, randomSoundEffects.Length)], CacheTransform.position, AudioManager.Singleton.GetVolumeLevel(SettingId));
                        break;
                }
            }

            FxCollection.Play();
        }
    }
}
