using Insthync.AudioManager;
using Insthync.ManagedUpdating;
using LiteNetLibManager;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class CharacterFootstepSoundComponent : BaseGameEntityComponent<BaseGameEntity>, IManagedUpdate
    {
        public AudioSource audioSource;
        public AudioComponentSettingType settingType = AudioComponentSettingType.Sfx;
        public string otherSettingId;
        public FootstepSettings walkFootstepSettings;
        public FootstepSettings moveFootstepSettings;
        public FootstepSettings sprintFootstepSettings;
        public FootstepSettings crouchFootstepSettings;
        public FootstepSettings crawlFootstepSettings;
        public FootstepSettings swimFootstepSettings;

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

        #region Deprecated settings
        [HideInInspector]
        public FootstepSoundData soundData;
        [HideInInspector]
        [Tooltip("This is delay to play future footstep sounds")]
        public float stepDelay = 0.35f;
        [HideInInspector]
        [Tooltip("This is threshold to play footstep sounds, for example if this value is 0.1 and velocity.magnitude more or equals to 0.1 it will play sounds")]
        public float stepThreshold = 0.1f;
        [HideInInspector]
        [Range(0f, 1f)]
        public float randomVolumeMin = 0.75f;
        [HideInInspector]
        [Range(0f, 1f)]
        public float randomVolumeMax = 1f;
        [HideInInspector]
        [Range(-3f, 3f)]
        public float randomPitchMin = 0.75f;
        [HideInInspector]
        [Range(-3f, 3f)]
        public float randomPitchMax = 1f;
        #endregion

        private FootstepSettings currentFootstepSettings;
        private float delayCounter = 0f;

        private void Start()
        {
            if (!Entity.IsClient)
            {
                enabled = false;
                return;
            }
            MigrateSettings();
            if (audioSource == null)
            {
                GameObject audioSourceObject = new GameObject("_FootstepAudioSource");
                audioSourceObject.transform.parent = CacheTransform;
                audioSourceObject.transform.localPosition = Vector3.zero;
                audioSourceObject.transform.localRotation = Quaternion.identity;
                audioSourceObject.transform.localScale = Vector3.one;
                audioSource = audioSourceObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (MigrateSettings())
                EditorUtility.SetDirty(this);
        }
#endif

        private bool MigrateSettings()
        {
            if (soundData.randomAudioClips != null && soundData.randomAudioClips.Length > 0 &&
                (moveFootstepSettings == null ||
                moveFootstepSettings.soundData.randomAudioClips == null ||
                moveFootstepSettings.soundData.randomAudioClips.Length == 0))
            {
                Logging.LogWarning(ToString(), "Migration run to setup old footstep settings to new footstep settings due to codes structure changes");
                moveFootstepSettings = new FootstepSettings()
                {
                    soundData = soundData,
                    stepDelay = stepDelay,
                    stepThreshold = stepThreshold,
                    randomVolumeMin = randomVolumeMin,
                    randomVolumeMax = randomVolumeMax,
                    randomPitchMin = randomPitchMin,
                    randomPitchMax = randomPitchMax,
                };
                return true;
            }
            return false;
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
            audioSource.mute = !AudioManager.Singleton.sfxVolumeSetting.IsOn;

            if (Entity.MovementState.Has(MovementState.IsUnderWater))
            {
                currentFootstepSettings = swimFootstepSettings;
            }
            else
            {
                switch (Entity.ExtraMovementState)
                {
                    case ExtraMovementState.IsWalking:
                        currentFootstepSettings = walkFootstepSettings;
                        break;
                    case ExtraMovementState.IsSprinting:
                        currentFootstepSettings = sprintFootstepSettings;
                        break;
                    case ExtraMovementState.IsCrouching:
                        currentFootstepSettings = crouchFootstepSettings;
                        break;
                    case ExtraMovementState.IsCrawling:
                        currentFootstepSettings = crawlFootstepSettings;
                        break;
                    default:
                        currentFootstepSettings = moveFootstepSettings;
                        break;
                }
            }

            if (!Entity.MovementState.Has(MovementState.Forward) &&
                !Entity.MovementState.Has(MovementState.Backward) &&
                !Entity.MovementState.Has(MovementState.Right) &&
                !Entity.MovementState.Has(MovementState.Left))
            {
                // No movement
                delayCounter = 0f;
                return;
            }

            delayCounter += Time.deltaTime;
            if (delayCounter >= currentFootstepSettings.stepDelay / Entity.MoveAnimationSpeedMultiplier)
            {
                if (Entity.MovementState.Has(MovementState.IsUnderWater) || Entity.MovementState.Has(MovementState.IsGrounded))
                    PlaySound();
                delayCounter = 0f;
            }
        }

        public void PlaySound()
        {
            if (Application.isBatchMode || AudioListener.pause)
                return;

            // Don't play sound while muting footstep sound
            if (Entity.MuteFootstepSound)
                return;

            // Don't play sound while passenging vehicle
            if (Entity.PassengingVehicleEntity != null)
                return;

            audioSource.pitch = Random.Range(currentFootstepSettings.randomPitchMin, currentFootstepSettings.randomPitchMax);
            audioSource.PlayOneShot(currentFootstepSettings.soundData.GetRandomedAudioClip(), Random.Range(currentFootstepSettings.randomVolumeMin, currentFootstepSettings.randomVolumeMax) * AudioManager.Singleton.GetVolumeLevel(SettingId));
        }
    }

    [System.Serializable]
    public struct FootstepSoundData
    {
        public AudioClip[] randomAudioClips;

        public AudioClip GetRandomedAudioClip()
        {
            if (randomAudioClips == null || randomAudioClips.Length == 0)
                return null;
            return randomAudioClips[Random.Range(0, randomAudioClips.Length)];
        }
    }

    [System.Serializable]
    public class FootstepSettings
    {
        public FootstepSoundData soundData;
        [Tooltip("This is delay to play next footstep sounds")]
        public float stepDelay = 0.35f;
        [Tooltip("This is threshold to play footstep sounds, for example if this value is 0.1 and velocity.magnitude more or equals to 0.1 it will play sounds")]
        public float stepThreshold = 0.1f;
        [Range(0f, 1f)]
        public float randomVolumeMin = 0.75f;
        [Range(0f, 1f)]
        public float randomVolumeMax = 1f;
        [Range(-3f, 3f)]
        public float randomPitchMin = 0.75f;
        [Range(-3f, 3f)]
        public float randomPitchMax = 1f;
    }
}
