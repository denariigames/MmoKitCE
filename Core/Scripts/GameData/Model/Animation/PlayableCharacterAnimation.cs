using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG.GameData.Model.Playables
{
    [System.Serializable]
    public class AnimState
    {
        public AnimationClip clip;
        [Tooltip("If this <= 0, it will not be used to calculate with animation speed multiplier")]
        public float animSpeedRate;
        [Tooltip("If this <= 0, it will use default transition duration setting from model component")]
        public float transitionDuration;
        public bool isAdditive;
        public bool applyFootIk;
        public bool applyPlayableIk;

        public float GetSpeed(float rate)
        {
            return (animSpeedRate > 0 ? animSpeedRate : 1) * rate;
        }

        public float GetClipLength(float rate)
        {
            return clip.length / GetSpeed(rate);
        }
    }

    [System.Serializable]
    public class AnimWithMaskState : AnimState
    {
        [Tooltip("If this is `null`, it will use default avatar mask setting from model component")]
        public AvatarMask avatarMask;
    }

    [System.Serializable]
    public class ActionState : AnimWithMaskState
    {
        [Tooltip("If this is `null`, it will use `clip`")]
        public AnimationClip clipWhileMoving;
        [Tooltip("If this is `null`, it will use `avatarMask`")]
        public AvatarMask avatarMaskWhileMoving;
        [Tooltip("If this is `null`, it will use `clipWhileMoving`")]
        public AnimationClip clipWhileSprinting;
        [Tooltip("If this is `null`, it will use `avatarMaskWhileMoving`")]
        public AvatarMask avatarMaskWhileSprinting;
        [Tooltip("If this is `null`, it will use `clip`")]
        public AnimationClip clipWhileAirbourne;
        [Tooltip("If this is `null`, it will use `avatarMask`")]
        public AvatarMask avatarMaskWhileAirbourne;
        [Tooltip("Turn this on to skip movement validation while playing this animation")]
        public bool skipMovementValidation;
        [Tooltip("Turn this on to use root motion while playing this animation")]
        public bool shouldUseRootMotion;
    }

    [System.Serializable]
    public class HolsterAnimation
    {
        [Header("Sheath")]
        [FormerlySerializedAs("holsterState")]
        public ActionState sheathState = new ActionState();
        [Range(0f, 1f)]
        [FormerlySerializedAs("holsteredDurationRate")]
        public float sheathedDurationRate;

        [Header("Unsheath")]
        [FormerlySerializedAs("drawState")]
        public ActionState unsheathState = new ActionState();
        [Range(0f, 1f)]
        [FormerlySerializedAs("drawnDurationRate")]
        public float unsheathedDurationRate;
    }

    [System.Serializable]
    public class MoveStates
    {
        public AnimState forwardState = new AnimState();
        public AnimState backwardState = new AnimState();
        public AnimState leftState = new AnimState();
        public AnimState rightState = new AnimState();
        public AnimState upState = new AnimState();
        public AnimState downState = new AnimState();
        public AnimState forwardLeftState = new AnimState();
        public AnimState forwardRightState = new AnimState();
        public AnimState backwardLeftState = new AnimState();
        public AnimState backwardRightState = new AnimState();
    }

    [System.Serializable]
    public class WieldMoveStates
    {
        public AnimWithMaskState forwardState = new AnimWithMaskState();
        public AnimWithMaskState backwardState = new AnimWithMaskState();
        public AnimWithMaskState leftState = new AnimWithMaskState();
        public AnimWithMaskState rightState = new AnimWithMaskState();
        public AnimWithMaskState upState = new AnimWithMaskState();
        public AnimWithMaskState downState = new AnimWithMaskState();
        public AnimWithMaskState forwardLeftState = new AnimWithMaskState();
        public AnimWithMaskState forwardRightState = new AnimWithMaskState();
        public AnimWithMaskState backwardLeftState = new AnimWithMaskState();
        public AnimWithMaskState backwardRightState = new AnimWithMaskState();
    }

    [System.Serializable]
    public class ActionAnimation
    {
        public ActionState state = new ActionState();
        [Tooltip("This will be in used with attacking/skill animations, This is rate of total animation duration at when it should hit enemy or apply skill")]
        [Range(0f, 1f)]
        public float[] triggerDurationRates = new float[0];
        [Tooltip("How animation duration defined")]
        public AnimationDurationType durationType;
        [StringShowConditional(nameof(durationType), nameof(AnimationDurationType.ByFixedDuration))]
        [Tooltip("This will be used when `durationType` equals to `ByFixValue` to define animation duration")]
        public float fixedDuration;
        [Tooltip("This will be in use with attacking/skill animations, This is duration after action animation clip played to add some delay before next animation")]
        public float extendDuration;
        [Tooltip("This will be in use with attacking/skill animations, These audio clips will be played randomly while play this animation (not loop). PS. You actually can use animation event instead :P")]
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public AudioClip[] audioClips = new AudioClip[0];
#endif
#if !DISABLE_ADDRESSABLES
        public AssetReferenceAudioClip[] addressableAudioClips = new AssetReferenceAudioClip[0];
#endif

        public async UniTask<AudioClip> GetRandomAudioClip()
        {
#if !UNITY_SERVER
            AudioClip[] tempAudioClips = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            tempAudioClips = audioClips;
#endif
            if (tempAudioClips != null && tempAudioClips.Length > 0)
            {
                await UniTask.Yield();
                return tempAudioClips[Random.Range(0, tempAudioClips.Length)];
            }
#if !DISABLE_ADDRESSABLES
            else if (addressableAudioClips != null && addressableAudioClips.Length > 0)
            {
                AssetReferenceAudioClip clip = addressableAudioClips.GetRandomObjectInArray(out int index);
                return await clip.GetOrLoadObjectAsync<AudioClip>();
            }
#endif
#endif
            return null;
        }

        public float GetClipLength()
        {
            switch (durationType)
            {
                case AnimationDurationType.ByClipLength:
                    if (state.clip == null)
                        return 0f;
                    return state.clip.length;
                case AnimationDurationType.ByFixedDuration:
                    return fixedDuration;
            }
            return 0f;
        }

        public float GetExtendDuration()
        {
            return extendDuration;
        }

        public float GetAnimSpeedRate()
        {
            return state.animSpeedRate > 0 ? state.animSpeedRate : 1f;
        }

        public float[] GetTriggerDurations()
        {
            float clipLength = GetClipLength();
            if (triggerDurationRates == null || triggerDurationRates.Length == 0)
                return new float[] { clipLength * 0.5f };
            float previousRate = 0f;
            float[] durations = new float[triggerDurationRates.Length];
            for (int i = 0; i < durations.Length; ++i)
            {
                durations[i] = clipLength * (triggerDurationRates[i] - previousRate);
                previousRate = triggerDurationRates[i];
            }
            return durations;
        }

        public float GetTotalDuration()
        {
            return GetClipLength() + extendDuration;
        }
    }

    [System.Serializable]
    public class EnterExitStates
    {
        public ActionState enterState = new ActionState();
        public ActionState exitState = new ActionState();
    }

    [System.Serializable]
    public class WeaponAnimations : IWeaponAnims
    {
        public WeaponType weaponType;

        [Header("Movements while standing")]
        public AnimState idleState = new AnimState();
        public MoveStates moveStates = new MoveStates();
        public MoveStates sprintStates = new MoveStates();
        public MoveStates walkStates = new MoveStates();

        [Header("Movements while crouching")]
        public AnimState crouchIdleState = new AnimState();
        public MoveStates crouchMoveStates = new MoveStates();

        [Header("Movements while crawling")]
        public AnimState crawlIdleState = new AnimState();
        public MoveStates crawlMoveStates = new MoveStates();

        [Header("Movements while swimming")]
        public AnimState swimIdleState = new AnimState();
        public MoveStates swimMoveStates = new MoveStates();

        [Header("Airborne")]
        public AnimState jumpState = new AnimState();
        public AnimState fallState = new AnimState();
        public AnimState landedState = new AnimState();

        [Header("Hurt")]
        public ActionState hurtState = new ActionState();

        [Header("Dead")]
        public AnimState deadState = new AnimState();

        [Header("Dash")]
        public AnimState dashStartState = new AnimState();
        public AnimState dashLoopState = new AnimState();
        public AnimState dashEndState = new AnimState();

        [Header("Pickup")]
        public ActionState pickupState = new ActionState();

        [Header("Attack animations")]
        public ActionState rightHandChargeState = new ActionState();
        public ActionState leftHandChargeState = new ActionState();
        [ArrayElementTitle("clip")]
        public ActionAnimation[] rightHandAttackAnimations = new ActionAnimation[0];
        [ArrayElementTitle("clip")]
        public ActionAnimation[] leftHandAttackAnimations = new ActionAnimation[0];

        [Header("Reload(Gun) animations")]
        public ActionAnimation rightHandReloadAnimation = new ActionAnimation();
        public ActionAnimation leftHandReloadAnimation = new ActionAnimation();

        [Header("Sheath/Unsheath Animations")]
        [FormerlySerializedAs("rightHandHolsterAnimation")]
        public HolsterAnimation rightHandWeaponSheathingAnimation = new HolsterAnimation();
        [FormerlySerializedAs("leftHandHolsterAnimation")]
        public HolsterAnimation leftHandWeaponSheathingAnimation = new HolsterAnimation();

        public WeaponType Data { get { return weaponType; } }
    }

    [System.Serializable]
    public class WieldWeaponAnimations : IWeaponAnims
    {
        public WeaponType weaponType;

        [Header("Movements while standing")]
        public ActionState idleState = new ActionState();
        public WieldMoveStates moveStates = new WieldMoveStates();
        public WieldMoveStates sprintStates = new WieldMoveStates();
        public WieldMoveStates walkStates = new WieldMoveStates();

        [Header("Movements while crouching")]
        public ActionState crouchIdleState = new ActionState();
        public WieldMoveStates crouchMoveStates = new WieldMoveStates();

        [Header("Movements while crawling")]
        public ActionState crawlIdleState = new ActionState();
        public WieldMoveStates crawlMoveStates = new WieldMoveStates();

        [Header("Movements while swimming")]
        public ActionState swimIdleState = new ActionState();
        public WieldMoveStates swimMoveStates = new WieldMoveStates();

        [Header("Airborne")]
        public ActionState jumpState = new ActionState();
        public ActionState fallState = new ActionState();
        public ActionState landedState = new ActionState();

        [Header("Dead")]
        public ActionState deadState = new ActionState();

        [Header("Dash")]
        public ActionState dashStartState = new ActionState();
        public ActionState dashLoopState = new ActionState();
        public ActionState dashEndState = new ActionState();

        public WeaponType Data { get { return weaponType; } }
    }

    [System.Serializable]
    public class WeaponActionState : ActionState
    {
        public WeaponType weaponType;
    }

    [System.Serializable]
    public class WeaponActionAnimation : ActionAnimation
    {
        public WeaponType weaponType;
    }

    [System.Serializable]
    public class SkillAnimations : ISkillAnims
    {
        public BaseSkill skill;
        public ActionState castState = new ActionState();
        public WeaponActionState[] castStatesByWeaponTypes = new WeaponActionState[0];
        public SkillActivateAnimationType activateAnimationType;
        [StringShowConditional(nameof(activateAnimationType), nameof(SkillActivateAnimationType.UseActivateAnimation))]
        public ActionAnimation activateAnimation = new ActionAnimation();
        public WeaponActionAnimation[] activateAnimationsByWeaponTypes = new WeaponActionAnimation[0];
        public BaseSkill Data { get { return skill; } }
        private Dictionary<int, WeaponActionState> _cacheCastStatesByWeaponTypes = null;
        public Dictionary<int, WeaponActionState> CastStatesByWeaponTypes
        {
            get
            {
                if (_cacheCastStatesByWeaponTypes == null)
                {
                    _cacheCastStatesByWeaponTypes = new Dictionary<int, WeaponActionState>();
                    WeaponActionState actionState;
                    for (int i = 0; i < castStatesByWeaponTypes.Length; ++i)
                    {
                        actionState = castStatesByWeaponTypes[i];
                        if (actionState == null ||
                            actionState.weaponType == null ||
                            actionState.clip == null)
                            continue;
                        _cacheCastStatesByWeaponTypes[actionState.weaponType.DataId] = actionState;
                    }
                }
                return _cacheCastStatesByWeaponTypes;
            }
        }
        private Dictionary<int, WeaponActionAnimation> _cacheActivateAnimationsByWeaponTypes = null;
        public Dictionary<int, WeaponActionAnimation> ActivateAnimationsByWeaponTypes
        {
            get
            {
                if (_cacheActivateAnimationsByWeaponTypes == null)
                {
                    _cacheActivateAnimationsByWeaponTypes = new Dictionary<int, WeaponActionAnimation>();
                    WeaponActionAnimation actionState;
                    for (int i = 0; i < activateAnimationsByWeaponTypes.Length; ++i)
                    {
                        actionState = activateAnimationsByWeaponTypes[i];
                        if (actionState == null ||
                            actionState.weaponType == null)
                            continue;
                        _cacheActivateAnimationsByWeaponTypes[actionState.weaponType.DataId] = actionState;
                    }
                }
                return _cacheActivateAnimationsByWeaponTypes;
            }
        }

        public ActionState GetCastState(int weaponTypeDataId)
        {
            if (CastStatesByWeaponTypes.TryGetValue(weaponTypeDataId, out WeaponActionState weaponCastState))
                return weaponCastState;
            return castState;
        }

        public ActionAnimation GetActivateAnimation(int weaponTypeDataId)
        {
            if (ActivateAnimationsByWeaponTypes.TryGetValue(weaponTypeDataId, out WeaponActionAnimation weaponActivateAnimation))
                return weaponActivateAnimation;
            return activateAnimation;
        }
    }

    [System.Serializable]
    public class DefaultAnimations
    {
        [Header("Movements while standing")]
        public AnimState idleState = new AnimState();
        public MoveStates moveStates = new MoveStates();
        public MoveStates sprintStates = new MoveStates();
        public MoveStates walkStates = new MoveStates();

        [Header("Movements while crouching")]
        public AnimState crouchIdleState = new AnimState();
        public MoveStates crouchMoveStates = new MoveStates();

        [Header("Movements while crawling")]
        public AnimState crawlIdleState = new AnimState();
        public MoveStates crawlMoveStates = new MoveStates();

        [Header("Movements while swimming")]
        public AnimState swimIdleState = new AnimState();
        public MoveStates swimMoveStates = new MoveStates();

        [Header("Airborne")]
        public AnimState jumpState = new AnimState();
        public AnimState fallState = new AnimState();
        public AnimState landedState = new AnimState();

        [Header("Vehicle Animation")]
        public EnterExitStates vehicleEnterExitStates;

        [Header("Ladder Animation")]
        public EnterExitStates climbBottomEnterExitStates;
        public EnterExitStates climbTopEnterExitStates;
        public AnimState climbIdleState = new AnimState();
        public MoveStates climbMoveStates = new MoveStates();

        [Header("Hurt")]
        public ActionState hurtState = new ActionState();

        [Header("Dead")]
        public AnimState deadState = new AnimState();

        [Header("Dash")]
        public AnimState dashStartState = new AnimState();
        public AnimState dashLoopState = new AnimState();
        public AnimState dashEndState = new AnimState();

        [Header("Pickup")]
        public ActionState pickupState = new ActionState();

        [Header("Attack animations")]
        public ActionState rightHandChargeState = new ActionState();
        public ActionState leftHandChargeState = new ActionState();
        [ArrayElementTitle("clip")]
        public ActionAnimation[] rightHandAttackAnimations = new ActionAnimation[0];
        [ArrayElementTitle("clip")]
        public ActionAnimation[] leftHandAttackAnimations = new ActionAnimation[0];

        [Header("Reload(Gun) animations")]
        public ActionAnimation rightHandReloadAnimation = new ActionAnimation();
        public ActionAnimation leftHandReloadAnimation = new ActionAnimation();

        [Header("Skill animations")]
        public ActionState skillCastState = new ActionState();
        public ActionAnimation skillActivateAnimation = new ActionAnimation();

        [Header("Sheath/Unsheath Animations")]
        [FormerlySerializedAs("rightHandHolsterAnimation")]
        public HolsterAnimation rightHandWeaponSheathingAnimation = new HolsterAnimation();
        [FormerlySerializedAs("leftHandHolsterAnimation")]
        public HolsterAnimation leftHandWeaponSheathingAnimation = new HolsterAnimation();
        public HolsterAnimation leftHandShieldSheathingAnimation = new HolsterAnimation();
    }
}
