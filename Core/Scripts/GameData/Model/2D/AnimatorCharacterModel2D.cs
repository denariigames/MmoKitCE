using Insthync.AudioManager;
using Insthync.DevExtension;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public partial class AnimatorCharacterModel2D : BaseCharacterModel
    {
        public enum AnimatorControllerType
        {
            FourDirections,
            EightDirections,
            Custom,
        }
        // Clip name variables
        // Idle
        public const string CLIP_IDLE_DOWN = "__IdleDown";
        public const string CLIP_IDLE_UP = "__IdleUp";
        public const string CLIP_IDLE_LEFT = "__IdleLeft";
        public const string CLIP_IDLE_RIGHT = "__IdleRight";
        public const string CLIP_IDLE_DOWN_LEFT = "__IdleDownLeft";
        public const string CLIP_IDLE_DOWN_RIGHT = "__IdleDownRight";
        public const string CLIP_IDLE_UP_LEFT = "__IdleUpLeft";
        public const string CLIP_IDLE_UP_RIGHT = "__IdleUpRight";
        // Move
        public const string CLIP_MOVE_DOWN = "__MoveDown";
        public const string CLIP_MOVE_UP = "__MoveUp";
        public const string CLIP_MOVE_LEFT = "__MoveLeft";
        public const string CLIP_MOVE_RIGHT = "__MoveRight";
        public const string CLIP_MOVE_DOWN_LEFT = "__MoveDownLeft";
        public const string CLIP_MOVE_DOWN_RIGHT = "__MoveDownRight";
        public const string CLIP_MOVE_UP_LEFT = "__MoveUpLeft";
        public const string CLIP_MOVE_UP_RIGHT = "__MoveUpRight";
        // Run
        public const string CLIP_SPRINT_DOWN = "__SprintDown";
        public const string CLIP_SPRINT_UP = "__SprintUp";
        public const string CLIP_SPRINT_LEFT = "__SprintLeft";
        public const string CLIP_SPRINT_RIGHT = "__SprintRight";
        public const string CLIP_SPRINT_DOWN_LEFT = "__SprintDownLeft";
        public const string CLIP_SPRINT_DOWN_RIGHT = "__SprintDownRight";
        public const string CLIP_SPRINT_UP_LEFT = "__SprintUpLeft";
        public const string CLIP_SPRINT_UP_RIGHT = "__SprintUpRight";
        // Dead
        public const string CLIP_DEAD_DOWN = "__DeadDown";
        public const string CLIP_DEAD_UP = "__DeadUp";
        public const string CLIP_DEAD_LEFT = "__DeadLeft";
        public const string CLIP_DEAD_RIGHT = "__DeadRight";
        public const string CLIP_DEAD_DOWN_LEFT = "__DeadDownLeft";
        public const string CLIP_DEAD_DOWN_RIGHT = "__DeadDownRight";
        public const string CLIP_DEAD_UP_LEFT = "__DeadUpLeft";
        public const string CLIP_DEAD_UP_RIGHT = "__DeadUpRight";
        // Action
        public const string CLIP_ACTION_DOWN = "__ActionDown";
        public const string CLIP_ACTION_UP = "__ActionUp";
        public const string CLIP_ACTION_LEFT = "__ActionLeft";
        public const string CLIP_ACTION_RIGHT = "__ActionRight";
        public const string CLIP_ACTION_DOWN_LEFT = "__ActionDownLeft";
        public const string CLIP_ACTION_DOWN_RIGHT = "__ActionDownRight";
        public const string CLIP_ACTION_UP_LEFT = "__ActionUpLeft";
        public const string CLIP_ACTION_UP_RIGHT = "__ActionUpRight";
        // Cast Skill
        public const string CLIP_CAST_SKILL_DOWN = "__CastSkillDown";
        public const string CLIP_CAST_SKILL_UP = "__CastSkillUp";
        public const string CLIP_CAST_SKILL_LEFT = "__CastSkillLeft";
        public const string CLIP_CAST_SKILL_RIGHT = "__CastSkillRight";
        public const string CLIP_CAST_SKILL_DOWN_LEFT = "__CastSkillDownLeft";
        public const string CLIP_CAST_SKILL_DOWN_RIGHT = "__CastSkillDownRight";
        public const string CLIP_CAST_SKILL_UP_LEFT = "__CastSkillUpLeft";
        public const string CLIP_CAST_SKILL_UP_RIGHT = "__CastSkillUpRight";
        // Animator variables
        public static readonly int ANIM_DIRECTION_X = Animator.StringToHash("DirectionX");
        public static readonly int ANIM_DIRECTION_Y = Animator.StringToHash("DirectionY");
        public static readonly int ANIM_IS_DEAD = Animator.StringToHash("IsDead");
        public static readonly int ANIM_MOVE_SPEED = Animator.StringToHash("MoveSpeed");
        public static readonly int ANIM_DO_ACTION = Animator.StringToHash("DoAction");
        public static readonly int ANIM_IS_CASTING_SKILL = Animator.StringToHash("IsCastingSkill");
        public static readonly int ANIM_MOVE_CLIP_MULTIPLIER = Animator.StringToHash("MoveSpeedMultiplier");
        public static readonly int ANIM_ACTION_CLIP_MULTIPLIER = Animator.StringToHash("ActionSpeedMultiplier");

        [Header("2D Animations")]
        public AnimatorCharacterAnimation2D idleAnimation2D;
        public AnimatorCharacterAnimation2D moveAnimation2D;
        public AnimatorCharacterAnimation2D runAnimation2D;
        public AnimatorCharacterAnimation2D deadAnimation2D;
        public AnimatorActionAnimation2D defaultAttackAnimation2D;
        [FormerlySerializedAs("defaultSkillCastClip2D")]
        public AnimatorCharacterAnimation2D defaultSkillCastAnimation2D;
        public AnimatorActionAnimation2D defaultSkillActivateAnimation2D;
        public AnimatorActionAnimation2D defaultReloadAnimation2D;
        [ArrayElementTitle("weaponType")]
        public AnimatorWeaponAnimations2D[] weaponAnimations2D;
        [ArrayElementTitle("skill")]
        public AnimatorSkillAnimations2D[] skillAnimations2D;

        [Header("Settings")]
        public AnimatorControllerType controllerType;

        [Header("Relates Components")]
        [Tooltip("It will find `Animator` component on automatically if this is NULL")]
        public Animator animator;
        [Tooltip("You can set this when animator controller type is `Custom`")]
        public RuntimeAnimatorController animatorController;

        [Header("Action State Settings")]
        public string actionStateName = "Action";

        [Header("Cast Skill State Settings")]
        public string castSkillStateName = "CastSkill";

#if UNITY_EDITOR
        [Header("Animation Test Tool")]
        public AnimActionType testAnimActionType;
        public WeaponType testWeaponType;
        public BaseSkill testSkill;
        [InspectorButton(nameof(SetAnimatorClipsForTest))]
        public bool setAnimatorClipsForTest;
#endif

        public AnimatorOverrideController CacheAnimatorController { get; private set; }
        public DirectionType2D DirectionType2D { get { return GameplayUtils.GetDirectionTypeByVector2(Direction2D); } }

        private Coroutine actionCoroutine;
        private bool isSetupComponent;
        private int actionStateNameHash;
        private int castSkillStateNameHash;
        private float awakenTime;

        protected override void Awake()
        {
            base.Awake();
            awakenTime = Time.unscaledTime;
            SetupComponent();
        }

        public bool TryGetWeaponAnimations(int dataId, out AnimatorWeaponAnimations2D anims)
        {
            return CacheAnimationsManager.SetAndTryGetCacheWeaponAnimations(Id, weaponAnimations2D, skillAnimations2D, dataId, out anims);
        }

        public bool TryGetSkillAnimations(int dataId, out AnimatorSkillAnimations2D anims)
        {
            return CacheAnimationsManager.SetAndTryGetCacheSkillAnimations(Id, weaponAnimations2D, skillAnimations2D, dataId, out anims);
        }

        protected Coroutine StartActionCoroutine(IEnumerator routine)
        {
            StopActionCoroutine();
            actionCoroutine = StartCoroutine(routine);
            return actionCoroutine;
        }

        protected void StopActionCoroutine()
        {
            if (actionCoroutine != null)
                StopCoroutine(actionCoroutine);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
#if UNITY_EDITOR
            bool hasChanges = false;
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
                if (animator != null)
                    hasChanges = true;
            }

            RuntimeAnimatorController changingAnimatorController;
            switch (controllerType)
            {
                case AnimatorControllerType.FourDirections:
                    changingAnimatorController = Resources.Load("__Animator/__2DFourDirectionsCharacter") as RuntimeAnimatorController;
                    if (changingAnimatorController != null &&
                        changingAnimatorController != animatorController)
                    {
                        animatorController = changingAnimatorController;
                        hasChanges = true;
                    }
                    break;
                case AnimatorControllerType.EightDirections:
                    changingAnimatorController = Resources.Load("__Animator/__2DEightDirectionsCharacter") as RuntimeAnimatorController;
                    if (changingAnimatorController != null &&
                        changingAnimatorController != animatorController)
                    {
                        animatorController = changingAnimatorController;
                        hasChanges = true;
                    }
                    break;
            }
            if (animator == null)
                Debug.LogError($"{name}(AnimatorCharacterModel2D) -> `Animator` is empty, please set reference of `Animator` component to `AnimatorCharacterModel2D` -> `Animator` field.");
            if (animatorController == null)
                Debug.LogError($"{name}(AnimatorCharacterModel2D) -> `AnimatorController` is empty, please set reference of `AnimatorController` data to `AnimatorCharacterModel2D` -> `AnimatorController` field.");
            if (hasChanges)
            {
                isSetupComponent = false;
                SetupComponent();
                EditorUtility.SetDirty(this);
            }
#endif
        }

        private void SetupComponent()
        {
            if (isSetupComponent)
                return;
            isSetupComponent = true;
            if (CacheAnimatorController == null)
                CacheAnimatorController = new AnimatorOverrideController(animatorController);
            // Use override controller as animator
            if (animator != null && animator.runtimeAnimatorController != CacheAnimatorController)
                animator.runtimeAnimatorController = CacheAnimatorController;
            // Setup action state name hashes
            actionStateNameHash = Animator.StringToHash(actionStateName);
            // Setup cast skill state name hashes
            castSkillStateNameHash = Animator.StringToHash(castSkillStateName);
            SetDefaultAnimations();
        }

        public override void SetDefaultAnimations()
        {
            // Set default clips
            if (CacheAnimatorController != null)
            {
                // Idle
                CacheAnimatorController[CLIP_IDLE_DOWN] = idleAnimation2D.down;
                CacheAnimatorController[CLIP_IDLE_UP] = idleAnimation2D.up;
                CacheAnimatorController[CLIP_IDLE_LEFT] = idleAnimation2D.left;
                CacheAnimatorController[CLIP_IDLE_RIGHT] = idleAnimation2D.right;
                CacheAnimatorController[CLIP_IDLE_DOWN_LEFT] = idleAnimation2D.downLeft;
                CacheAnimatorController[CLIP_IDLE_DOWN_RIGHT] = idleAnimation2D.downRight;
                CacheAnimatorController[CLIP_IDLE_UP_LEFT] = idleAnimation2D.upLeft;
                CacheAnimatorController[CLIP_IDLE_UP_RIGHT] = idleAnimation2D.upRight;
                // Move
                CacheAnimatorController[CLIP_MOVE_DOWN] = moveAnimation2D.down;
                CacheAnimatorController[CLIP_MOVE_UP] = moveAnimation2D.up;
                CacheAnimatorController[CLIP_MOVE_LEFT] = moveAnimation2D.left;
                CacheAnimatorController[CLIP_MOVE_RIGHT] = moveAnimation2D.right;
                CacheAnimatorController[CLIP_MOVE_DOWN_LEFT] = moveAnimation2D.downLeft;
                CacheAnimatorController[CLIP_MOVE_DOWN_RIGHT] = moveAnimation2D.downRight;
                CacheAnimatorController[CLIP_MOVE_UP_LEFT] = moveAnimation2D.upLeft;
                CacheAnimatorController[CLIP_MOVE_UP_RIGHT] = moveAnimation2D.upRight;
                // Run
                CacheAnimatorController[CLIP_SPRINT_DOWN] = runAnimation2D.down;
                CacheAnimatorController[CLIP_SPRINT_UP] = runAnimation2D.up;
                CacheAnimatorController[CLIP_SPRINT_LEFT] = runAnimation2D.left;
                CacheAnimatorController[CLIP_SPRINT_RIGHT] = runAnimation2D.right;
                CacheAnimatorController[CLIP_SPRINT_DOWN_LEFT] = runAnimation2D.downLeft;
                CacheAnimatorController[CLIP_SPRINT_DOWN_RIGHT] = runAnimation2D.downRight;
                CacheAnimatorController[CLIP_SPRINT_UP_LEFT] = runAnimation2D.upLeft;
                CacheAnimatorController[CLIP_SPRINT_UP_RIGHT] = runAnimation2D.upRight;
                // Dead
                CacheAnimatorController[CLIP_DEAD_DOWN] = deadAnimation2D.down;
                CacheAnimatorController[CLIP_DEAD_UP] = deadAnimation2D.up;
                CacheAnimatorController[CLIP_DEAD_LEFT] = deadAnimation2D.left;
                CacheAnimatorController[CLIP_DEAD_RIGHT] = deadAnimation2D.right;
                CacheAnimatorController[CLIP_DEAD_DOWN_LEFT] = deadAnimation2D.downLeft;
                CacheAnimatorController[CLIP_DEAD_DOWN_RIGHT] = deadAnimation2D.downRight;
                CacheAnimatorController[CLIP_DEAD_UP_LEFT] = deadAnimation2D.upLeft;
                CacheAnimatorController[CLIP_DEAD_UP_RIGHT] = deadAnimation2D.upRight;
            }
            base.SetDefaultAnimations();
        }

        public override void PlayMoveAnimation()
        {
            if (!animator.gameObject.activeInHierarchy)
                return;

            if (animator.runtimeAnimatorController != CacheAnimatorController)
                animator.runtimeAnimatorController = CacheAnimatorController;

            if (IsDead)
            {
                // Clear action animations when dead
                if (animator.GetBool(ANIM_DO_ACTION))
                    animator.SetBool(ANIM_DO_ACTION, false);
                if (animator.GetBool(ANIM_IS_CASTING_SKILL))
                    animator.SetBool(ANIM_IS_CASTING_SKILL, false);
            }

            float moveSpeed = 0f;
            if (MovementState.Has(MovementState.Forward) ||
                MovementState.Has(MovementState.Backward) ||
                MovementState.Has(MovementState.Right) ||
                MovementState.Has(MovementState.Left))
            {
                if (ExtraMovementState == ExtraMovementState.IsSprinting)
                    moveSpeed = 2;
                else
                    moveSpeed = 1;
            }
            // Set animator parameters
            animator.SetFloat(ANIM_MOVE_SPEED, IsDead ? 0 : moveSpeed);
            animator.SetFloat(ANIM_MOVE_CLIP_MULTIPLIER, MoveAnimationSpeedMultiplier);
            // Fix stutter turning
            if (controllerType == AnimatorControllerType.FourDirections)
            {
                if (Mathf.Abs(Mathf.Abs(Direction2D.x) - Mathf.Abs(Direction2D.y)) < 0.01f)
                {
                    // Up, Down is higher priority
                    Vector2 applyDirection2D = Direction2D;
                    applyDirection2D.x = 0;
                    Direction2D = applyDirection2D.normalized;
                }
            }
            animator.SetFloat(ANIM_DIRECTION_X, Direction2D.x);
            animator.SetFloat(ANIM_DIRECTION_Y, Direction2D.y);
            animator.SetBool(ANIM_IS_DEAD, IsDead);
            if (IsDead && Time.unscaledTime - awakenTime < 1f)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                animator.Play(stateInfo.fullPathHash, 0, 1f);
            }
        }

        private AnimatorActionAnimation2D GetActionAnimation(AnimActionType animActionType, int dataId)
        {
            AnimatorActionAnimation2D animation2D = null;
            AnimatorWeaponAnimations2D weaponAnimations2D;
            AnimatorSkillAnimations2D skillAnimations2D;
            switch (animActionType)
            {
                case AnimActionType.AttackRightHand:
                    if (!TryGetWeaponAnimations(dataId, out weaponAnimations2D))
                        animation2D = defaultAttackAnimation2D;
                    else
                        animation2D = weaponAnimations2D.rightHandAttackAnimation;
                    break;
                case AnimActionType.AttackLeftHand:
                    if (!TryGetWeaponAnimations(dataId, out weaponAnimations2D))
                        animation2D = defaultAttackAnimation2D;
                    else
                        animation2D = weaponAnimations2D.leftHandAttackAnimation;
                    break;
                case AnimActionType.SkillRightHand:
                case AnimActionType.SkillLeftHand:
                    if (!TryGetSkillAnimations(dataId, out skillAnimations2D))
                        animation2D = defaultSkillActivateAnimation2D;
                    else
                        animation2D = skillAnimations2D.activateAnimation;
                    break;
                case AnimActionType.ReloadRightHand:
                    if (!TryGetWeaponAnimations(dataId, out weaponAnimations2D))
                        animation2D = defaultReloadAnimation2D;
                    else
                        animation2D = weaponAnimations2D.rightHandReloadAnimation;
                    break;
                case AnimActionType.ReloadLeftHand:
                    if (!TryGetWeaponAnimations(dataId, out weaponAnimations2D))
                        animation2D = defaultReloadAnimation2D;
                    else
                        animation2D = weaponAnimations2D.leftHandReloadAnimation;
                    break;
            }
            return animation2D;
        }

        public override void PlayActionAnimation(AnimActionType animActionType, int dataId, int index, out bool skipMovementValidation, out bool shouldUseRootMotion, float playSpeedMultiplier, float changeClipLength, float overrideClipLength)
        {
            skipMovementValidation = false;
            shouldUseRootMotion = false;
            StopActionAnimation();
            StopSkillCastAnimation();
            StopWeaponChargeAnimation();
            StartActionCoroutine(PlayActionAnimation_Animator(animActionType, dataId, index, playSpeedMultiplier, changeClipLength, overrideClipLength));
        }

        private IEnumerator PlayActionAnimation_Animator(AnimActionType animActionType, int dataId, int index, float playSpeedMultiplier, float changeClipLength, float overrideClipLength)
        {
            // If animator is not null, play the action animation
            AnimatorActionAnimation2D animation2D = GetActionAnimation(animActionType, dataId);
            // Action
            CacheAnimatorController[CLIP_ACTION_DOWN] = animation2D.down;
            CacheAnimatorController[CLIP_ACTION_UP] = animation2D.up;
            CacheAnimatorController[CLIP_ACTION_LEFT] = animation2D.left;
            CacheAnimatorController[CLIP_ACTION_RIGHT] = animation2D.right;
            CacheAnimatorController[CLIP_ACTION_DOWN_LEFT] = animation2D.downLeft;
            CacheAnimatorController[CLIP_ACTION_DOWN_RIGHT] = animation2D.downRight;
            CacheAnimatorController[CLIP_ACTION_UP_LEFT] = animation2D.upLeft;
            CacheAnimatorController[CLIP_ACTION_UP_RIGHT] = animation2D.upRight;
            AnimationClip clip = animation2D.GetClipByDirection(DirectionType2D);
            AudioManager.PlaySfxClipAtAudioSource(animation2D.GetRandomAudioClip(), GenericAudioSource);
            float clipLength = overrideClipLength > 0f ? overrideClipLength : clip.length;
            float clipSpeedRate = GetAnimationSpeedRate(clipLength, playSpeedMultiplier, changeClipLength);
            animator.SetFloat(ANIM_ACTION_CLIP_MULTIPLIER, clipSpeedRate);
            animator.SetBool(ANIM_DO_ACTION, true);
            animator.Play(actionStateNameHash, 0, 0f);
            // Waits by current transition + clip duration before end animation
            yield return new WaitForSecondsRealtime(clipLength / clipSpeedRate);
            animator.SetBool(ANIM_DO_ACTION, false);
            // Waits by current transition + extra duration before end playing animation state
            yield return new WaitForSecondsRealtime(animation2D.extraDuration / playSpeedMultiplier);
        }

        public override void PlaySkillCastClip(int dataId, float duration, out bool skipMovementValidation, out bool shouldUseRootMotion)
        {
            skipMovementValidation = false;
            shouldUseRootMotion = false;
            StopActionAnimation();
            StopSkillCastAnimation();
            StopWeaponChargeAnimation();
            StartActionCoroutine(PlaySkillCastClip_Animator(dataId, duration));
        }

        private IEnumerator PlaySkillCastClip_Animator(int dataId, float duration)
        {
            AnimatorCharacterAnimation2D animation2D;
            AnimatorSkillAnimations2D skillAnimations2D;
            if (!TryGetSkillAnimations(dataId, out skillAnimations2D))
                animation2D = defaultSkillActivateAnimation2D;
            else
                animation2D = skillAnimations2D.castClip;

            if (animation2D != null)
            {
                // Cast Skill
                CacheAnimatorController[CLIP_CAST_SKILL_DOWN] = animation2D.down;
                CacheAnimatorController[CLIP_CAST_SKILL_UP] = animation2D.up;
                CacheAnimatorController[CLIP_CAST_SKILL_LEFT] = animation2D.left;
                CacheAnimatorController[CLIP_CAST_SKILL_RIGHT] = animation2D.right;
                CacheAnimatorController[CLIP_CAST_SKILL_DOWN_LEFT] = animation2D.downLeft;
                CacheAnimatorController[CLIP_CAST_SKILL_DOWN_RIGHT] = animation2D.downRight;
                CacheAnimatorController[CLIP_CAST_SKILL_UP_LEFT] = animation2D.upLeft;
                CacheAnimatorController[CLIP_CAST_SKILL_UP_RIGHT] = animation2D.upRight;
                animator.SetBool(ANIM_IS_CASTING_SKILL, true);
                animator.Play(castSkillStateNameHash, 0, 0f);
                yield return new WaitForSecondsRealtime(duration);
                animator.SetBool(ANIM_IS_CASTING_SKILL, false);
            }
        }

        public override void PlayWeaponChargeClip(int dataId, bool isLeftHand, out bool skipMovementValidation, out bool shouldUseRootMotion)
        {
            // TODO: May implement pulling animation for 2D models
            skipMovementValidation = false;
            shouldUseRootMotion = false;
        }

        public override void StopActionAnimation()
        {
            animator.SetBool(ANIM_DO_ACTION, false);
        }

        public override void StopSkillCastAnimation()
        {
            animator.SetBool(ANIM_IS_CASTING_SKILL, false);
        }

        public override void StopWeaponChargeAnimation()
        {
            // TODO: May implement pulling animation for 2D models
        }

        public override int GetRightHandAttackRandomMax(int dataId)
        {
            return 1;
        }

        public override int GetLeftHandAttackRandomMax(int dataId)
        {
            return 1;
        }

        public override bool GetRandomRightHandAttackAnimation(int dataId, int randomSeed, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            animationIndex = 0;
            return GetRightHandAttackAnimation(dataId, animationIndex, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public override bool GetRandomLeftHandAttackAnimation(int dataId, int randomSeed, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            animationIndex = 0;
            return GetLeftHandAttackAnimation(dataId, animationIndex, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public override bool GetRightHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            AnimatorActionAnimation2D animation2D = defaultAttackAnimation2D;
            AnimatorWeaponAnimations2D weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation2D = weaponAnims.rightHandAttackAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override bool GetLeftHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            AnimatorActionAnimation2D animation2D = defaultAttackAnimation2D;
            AnimatorWeaponAnimations2D weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation2D = weaponAnims.leftHandAttackAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override bool GetSkillActivateAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            AnimatorActionAnimation2D animation2D = defaultSkillActivateAnimation2D;
            AnimatorSkillAnimations2D skillAnims;
            if (TryGetSkillAnimations(dataId, out skillAnims))
                animation2D = skillAnims.activateAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override bool GetRightHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            AnimatorActionAnimation2D animation2D = defaultReloadAnimation2D;
            AnimatorWeaponAnimations2D weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation2D = weaponAnims.rightHandReloadAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override bool GetLeftHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            AnimatorActionAnimation2D animation2D = defaultReloadAnimation2D;
            AnimatorWeaponAnimations2D weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation2D = weaponAnims.leftHandReloadAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override SkillActivateAnimationType GetSkillActivateAnimationType(int dataId)
        {
            AnimatorSkillAnimations2D anims;
            if (!TryGetSkillAnimations(dataId, out anims))
                return SkillActivateAnimationType.UseActivateAnimation;
            return anims.activateAnimationType;
        }

#if UNITY_EDITOR
        [ContextMenu("Set Animator Clips For Test", false, 1000501)]
        public void SetAnimatorClipsForTest()
        {
            SetupComponent();

            int testActionAnimDataId = 0;
            int testCastSkillAnimDataId = 0;
            switch (testAnimActionType)
            {
                case AnimActionType.AttackRightHand:
                case AnimActionType.AttackLeftHand:
                    if (testWeaponType != null)
                        testActionAnimDataId = testWeaponType.DataId;
                    break;
                case AnimActionType.SkillRightHand:
                case AnimActionType.SkillLeftHand:
                    if (testSkill != null)
                    {
                        testActionAnimDataId = testSkill.DataId;
                        testCastSkillAnimDataId = testSkill.DataId;
                    }
                    break;
                case AnimActionType.ReloadRightHand:
                case AnimActionType.ReloadLeftHand:
                    if (testWeaponType != null)
                        testActionAnimDataId = testWeaponType.DataId;
                    break;
            }

            // Movement animation clips
            SetDefaultAnimations();

            // Set animation clip
            AnimatorCharacterAnimation2D animation2D;
            AnimatorSkillAnimations2D skillAnimations2D;

            // Action animation clips
            animation2D = GetActionAnimation(testAnimActionType, testActionAnimDataId);
            CacheAnimatorController[CLIP_ACTION_DOWN] = animation2D.down;
            CacheAnimatorController[CLIP_ACTION_UP] = animation2D.up;
            CacheAnimatorController[CLIP_ACTION_LEFT] = animation2D.left;
            CacheAnimatorController[CLIP_ACTION_RIGHT] = animation2D.right;
            CacheAnimatorController[CLIP_ACTION_DOWN_LEFT] = animation2D.downLeft;
            CacheAnimatorController[CLIP_ACTION_DOWN_RIGHT] = animation2D.downRight;
            CacheAnimatorController[CLIP_ACTION_UP_LEFT] = animation2D.upLeft;
            CacheAnimatorController[CLIP_ACTION_UP_RIGHT] = animation2D.upRight;

            // Skill animation clips
            if (!TryGetSkillAnimations(testCastSkillAnimDataId, out skillAnimations2D))
                animation2D = defaultSkillActivateAnimation2D;
            else
                animation2D = skillAnimations2D.castClip;
            CacheAnimatorController[CLIP_CAST_SKILL_DOWN] = animation2D.down;
            CacheAnimatorController[CLIP_CAST_SKILL_UP] = animation2D.up;
            CacheAnimatorController[CLIP_CAST_SKILL_LEFT] = animation2D.left;
            CacheAnimatorController[CLIP_CAST_SKILL_RIGHT] = animation2D.right;
            CacheAnimatorController[CLIP_CAST_SKILL_DOWN_LEFT] = animation2D.downLeft;
            CacheAnimatorController[CLIP_CAST_SKILL_DOWN_RIGHT] = animation2D.downRight;
            CacheAnimatorController[CLIP_CAST_SKILL_UP_LEFT] = animation2D.upLeft;
            CacheAnimatorController[CLIP_CAST_SKILL_UP_RIGHT] = animation2D.upRight;

            Logging.Log(ToString(), "Animation Clips already set to animator controller, you can test an animations in Animation tab");

            this.InvokeInstanceDevExtMethods("SetAnimatorClipsForTest");
        }
#endif
    }
}
