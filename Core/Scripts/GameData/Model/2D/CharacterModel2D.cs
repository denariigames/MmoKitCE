using Insthync.AudioManager;
using Insthync.UnityEditorUtils;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [ExecuteInEditMode]
    public partial class CharacterModel2D : BaseCharacterModel
    {
        public enum SampleAnimation
        {
            Idle,
            Move,
            Sprint,
            Walk,
            Dead,
            DefaultAttack,
            DefaultSkillCast,
        }

        [Header("2D Animations")]
        public SpriteRenderer spriteRenderer;
        public CharacterAnimation2D idleAnimation2D;
        public CharacterAnimation2D moveAnimation2D;
        public CharacterAnimation2D sprintAnimation2D;
        public CharacterAnimation2D walkAnimation2D;
        public CharacterAnimation2D deadAnimation2D;
        public ActionAnimation2D defaultAttackAnimation2D;
        [FormerlySerializedAs("defaultSkillCastClip2D")]
        public CharacterAnimation2D defaultSkillCastAnimation2D;
        public ActionAnimation2D defaultSkillActivateAnimation2D;
        public ActionAnimation2D defaultReloadAnimation2D;
        [ArrayElementTitle("weaponType")]
        public WeaponAnimations2D[] weaponAnimations2D;
        [ArrayElementTitle("skill")]
        public SkillAnimations2D[] skillAnimations2D;

        [Header("Sample 2D Animations")]
        public SampleAnimation sampleAnimation = SampleAnimation.Idle;
        public DirectionType2D sampleDirection = DirectionType2D.Down;
        public bool sampleAlwaysLoop = true;

        private AnimationClip2D playingAnim = null;
        private int currentFrame = 0;
        private bool playing = false;
        private bool playingAction = false;
        private float transitionToMoveAnim = 0f;
        private float secsPerFrame = 0;
        private float nextFrameTime = 0;
        private SampleAnimation? dirtySampleAnimation;
        private DirectionType2D? dirtySampleType;

        public DirectionType2D DirectionType2D { get { return GameplayUtils.GetDirectionTypeByVector2(Direction2D); } }

        protected override void Awake()
        {
            base.Awake();
            if (sprintAnimation2D == null)
            {
                sprintAnimation2D = moveAnimation2D;
            }
            else
            {
                if (sprintAnimation2D.down == null)
                    sprintAnimation2D.down = moveAnimation2D.down;
                if (sprintAnimation2D.up == null)
                    sprintAnimation2D.up = moveAnimation2D.up;
                if (sprintAnimation2D.left == null)
                    sprintAnimation2D.left = moveAnimation2D.left;
                if (sprintAnimation2D.right == null)
                    sprintAnimation2D.right = moveAnimation2D.right;
            }
            if (walkAnimation2D == null)
            {
                walkAnimation2D = moveAnimation2D;
            }
            else
            {
                if (walkAnimation2D.down == null)
                    walkAnimation2D.down = moveAnimation2D.down;
                if (walkAnimation2D.up == null)
                    walkAnimation2D.up = moveAnimation2D.up;
                if (walkAnimation2D.left == null)
                    walkAnimation2D.left = moveAnimation2D.left;
                if (walkAnimation2D.right == null)
                    walkAnimation2D.right = moveAnimation2D.right;
            }
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

        public bool TryGetWeaponAnimations(int dataId, out WeaponAnimations2D anims)
        {
            return CacheAnimationsManager.SetAndTryGetCacheWeaponAnimations(Id, weaponAnimations2D, skillAnimations2D, dataId, out anims);
        }

        public bool TryGetSkillAnimations(int dataId, out SkillAnimations2D anims)
        {
            return CacheAnimationsManager.SetAndTryGetCacheSkillAnimations(Id, weaponAnimations2D, skillAnimations2D, dataId, out anims);
        }

        void EditorUpdate()
        {
            if (!Application.isPlaying)
            {
                UpdateSample();
                Update();
            }
        }

        void Update()
        {
            if (spriteRenderer == null)
                return;
#if UNITY_EDITOR
            if (!Application.isPlaying && sampleAlwaysLoop)
                playing = true;
#endif
            // Increase next frame time while pause
            if (!playing)
            {
                nextFrameTime += Time.deltaTime;
                return;
            }
            // Is is time to play next frame?
            float time = Time.realtimeSinceStartup;
            if (time < nextFrameTime)
                return;
            // Play next frame
            currentFrame++;
            if (currentFrame >= playingAnim.frames.Length)
            {
                if (!playingAnim.loop)
                {
                    playing = false;
                    return;
                }
                currentFrame = 0;
            }
            spriteRenderer.sprite = playingAnim.frames[currentFrame];
            nextFrameTime = time + secsPerFrame;
        }

        private void UpdateSample()
        {
            if (dirtySampleAnimation.HasValue &&
                dirtySampleAnimation.Value == sampleAnimation &&
                dirtySampleType.HasValue &&
                dirtySampleType.Value == sampleDirection)
                return;
            dirtySampleAnimation = sampleAnimation;
            dirtySampleType = sampleDirection;
            switch (sampleAnimation)
            {
                case SampleAnimation.Idle:
                    Play(idleAnimation2D, sampleDirection);
                    break;
                case SampleAnimation.Move:
                    Play(moveAnimation2D, sampleDirection);
                    break;
                case SampleAnimation.Walk:
                    Play(walkAnimation2D, sampleDirection);
                    break;
                case SampleAnimation.Sprint:
                    Play(sprintAnimation2D, sampleDirection);
                    break;
                case SampleAnimation.Dead:
                    Play(deadAnimation2D, sampleDirection);
                    break;
                case SampleAnimation.DefaultAttack:
                    Play(defaultAttackAnimation2D, sampleDirection);
                    break;
                case SampleAnimation.DefaultSkillCast:
                    Play(defaultSkillCastAnimation2D, sampleDirection);
                    break;
            }
        }

        public void Play(CharacterAnimation2D animation, DirectionType2D directionType)
        {
            if (animation == null)
                return;
            Play(animation.GetClipByDirection(directionType));
        }

        public void Play(AnimationClip2D anim)
        {
            if (anim == playingAnim)
                return;

            playingAnim = anim;
            spriteRenderer.flipX = anim.flipX;
            spriteRenderer.flipY = anim.flipY;
            secsPerFrame = 1f / anim.framesPerSec;
            currentFrame = -1;
            playing = true;
            nextFrameTime = Time.realtimeSinceStartup + 0.025f;   // Add some delay to avoid glitch
        }

        public void Stop()
        {
            playing = false;
        }

        public void Resume()
        {
            playing = true;
        }

        public override void PlayMoveAnimation()
        {
            if (IsDead)
            {
                Play(deadAnimation2D, DirectionType2D);
                playingAction = false;
                transitionToMoveAnim = 0f;
            }

            if (playingAction)
                return;

            if (transitionToMoveAnim >= 0f)
            {
                transitionToMoveAnim -= Time.deltaTime;
                return;
            }

            if (MovementState.Has(MovementState.Forward) ||
                MovementState.Has(MovementState.Backward) ||
                MovementState.Has(MovementState.Right) ||
                MovementState.Has(MovementState.Left))
            {
                switch (ExtraMovementState)
                {
                    case ExtraMovementState.IsSprinting:
                        Play(sprintAnimation2D, DirectionType2D);
                        break;
                    case ExtraMovementState.IsWalking:
                        Play(walkAnimation2D, DirectionType2D);
                        break;
                    default:
                        Play(moveAnimation2D, DirectionType2D);
                        break;
                }
            }
            else
            {
                Play(idleAnimation2D, DirectionType2D);
            }
        }

        private ActionAnimation2D GetActionAnimation(AnimActionType animActionType, int dataId)
        {
            ActionAnimation2D animation2D = null;
            WeaponAnimations2D weaponAnimations2D;
            SkillAnimations2D skillAnimations2D;
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
            StartCoroutine(PlayActionAnimationRoutine(animActionType, dataId, index, playSpeedMultiplier, changeClipLength, overrideClipLength));
        }

        IEnumerator PlayActionAnimationRoutine(AnimActionType animActionType, int dataId, int index, float playSpeedMultiplier, float changeClipLength, float overrideClipLength)
        {
            playingAction = true;
            // If animator is not null, play the action animation
            ActionAnimation2D animation2D = GetActionAnimation(animActionType, dataId);
            if (animation2D != null)
            {
                AnimationClip2D anim = animation2D.GetClipByDirection(DirectionType2D);
                if (anim != null)
                {
                    AudioManager.PlaySfxClipAtAudioSource(animation2D.GetRandomAudioClip(), GenericAudioSource);
                    // Waits by current transition + clip duration before end animation
                    Play(anim);
                    float clipLength = overrideClipLength > 0f ? overrideClipLength : anim.length;
                    float clipSpeedRate = GetAnimationSpeedRate(clipLength, playSpeedMultiplier, changeClipLength);
                    yield return new WaitForSecondsRealtime(clipLength / clipSpeedRate);
                    Play(idleAnimation2D, DirectionType2D);
                    yield return new WaitForSecondsRealtime(animation2D.extraDuration / playSpeedMultiplier);
                    transitionToMoveAnim = 0.25f;
                }
            }
            playingAction = false;
        }

        public override void PlaySkillCastClip(int dataId, float duration, out bool skipMovementValidation, out bool shouldUseRootMotion)
        {
            skipMovementValidation = false;
            shouldUseRootMotion = false;
            StartCoroutine(PlaySkillCastClipRoutine(dataId, duration));
        }

        IEnumerator PlaySkillCastClipRoutine(int dataId, float duration)
        {
            playingAction = true;
            CharacterAnimation2D animation2D = defaultSkillActivateAnimation2D;
            SkillAnimations2D skillAnims;
            if (TryGetSkillAnimations(dataId, out skillAnims))
                animation2D = skillAnims.castAnimation;

            if (animation2D != null)
            {
                AnimationClip2D anim = animation2D.GetClipByDirection(DirectionType2D);
                if (anim != null)
                {
                    Play(anim);
                    yield return new WaitForSecondsRealtime(duration);
                    Play(idleAnimation2D, DirectionType2D);
                    transitionToMoveAnim = 0.25f;
                }
            }
            playingAction = false;
        }

        public override void PlayWeaponChargeClip(int dataId, bool isLeftHand, out bool skipMovementValidation, out bool shouldUseRootMotion)
        {
            // TODO: May implement pulling animation for 2D models
            skipMovementValidation = false;
            shouldUseRootMotion = false;
        }

        public override void StopActionAnimation()
        {
            playingAction = false;
        }

        public override void StopSkillCastAnimation()
        {
            playingAction = false;
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
            ActionAnimation2D animation2D = defaultAttackAnimation2D;
            WeaponAnimations2D weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation2D = weaponAnims.rightHandAttackAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip2D clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override bool GetLeftHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            ActionAnimation2D animation2D = defaultAttackAnimation2D;
            WeaponAnimations2D weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation2D = weaponAnims.leftHandAttackAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip2D clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override bool GetSkillActivateAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            ActionAnimation2D animation2D = defaultSkillActivateAnimation2D;
            SkillAnimations2D skillAnims;
            if (TryGetSkillAnimations(dataId, out skillAnims))
                animation2D = skillAnims.activateAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip2D clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override bool GetRightHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            ActionAnimation2D animation2D = defaultReloadAnimation2D;
            WeaponAnimations2D weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation2D = weaponAnims.rightHandReloadAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip2D clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override bool GetLeftHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            ActionAnimation2D animation2D = defaultReloadAnimation2D;
            WeaponAnimations2D weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation2D = weaponAnims.leftHandReloadAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            if (animation2D == null) return false;
            AnimationClip2D clip = animation2D.GetClipByDirection(DirectionType2D);
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations(clip.length);
            totalDuration = animation2D.GetTotalDuration(clip.length);
            return true;
        }

        public override SkillActivateAnimationType GetSkillActivateAnimationType(int dataId)
        {
            SkillAnimations2D anims;
            if (!TryGetSkillAnimations(dataId, out anims))
                return SkillActivateAnimationType.UseActivateAnimation;
            return anims.activateAnimationType;
        }
    }
}
