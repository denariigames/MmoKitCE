using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class CharacterAnimation2D
    {
        [Header("4-Directions")]
        public AnimationClip2D down;
        public AnimationClip2D up;
        public AnimationClip2D left;
        public AnimationClip2D right;
        [Header("8-Directions")]
        public AnimationClip2D downLeft;
        public AnimationClip2D downRight;
        public AnimationClip2D upLeft;
        public AnimationClip2D upRight;

        public AnimationClip2D GetClipByDirection(DirectionType2D directionType)
        {
            switch (directionType)
            {
                case DirectionType2D.Down:
                    return down;
                case DirectionType2D.Up:
                    return up;
                case DirectionType2D.Left:
                    return left;
                case DirectionType2D.Right:
                    return right;
                case DirectionType2D.DownLeft:
                    // Return down if it is support 4-direction
                    if (downLeft == null)
                        return down;
                    return downLeft;
                case DirectionType2D.DownRight:
                    // Return down if it is support 4-direction
                    if (downRight == null)
                        return down;
                    return downRight;
                case DirectionType2D.UpLeft:
                    // Return up if it is support 4-direction
                    if (upLeft == null)
                        return up;
                    return upLeft;
                case DirectionType2D.UpRight:
                    // Return up if it is support 4-direction
                    if (upRight == null)
                        return up;
                    return upRight;
            }
            // Default direction is down
            return down;
        }
    }

    [System.Serializable]
    public class ActionAnimation2D : CharacterAnimation2D
    {
        [Tooltip("This will be in use with attack/skill animations, This is rate of total animation duration at when it should hit enemy or apply skill")]
        [Range(0f, 1f)]
        public float triggerDurationRate;
        [Tooltip("If this length more than 1, will use each entry for trigger duration rate")]
        [Range(0f, 1f)]
        public float[] multiHitTriggerDurationRates;
        [Tooltip("This will be in use with attack/skill animations, This is duration after played animation clip to add delay before next animation")]
        public float extraDuration;
        [Tooltip("This will be in use with attack/skill animations, These audio clips playing randomly while play this animation (not loop)")]
        public AudioClip[] audioClips;

        public AudioClip GetRandomAudioClip()
        {
            AudioClip clip = null;
            if (audioClips != null && audioClips.Length > 0)
                clip = audioClips[Random.Range(0, audioClips.Length)];
            return clip;
        }

        public float GetExtraDuration()
        {
            return extraDuration;
        }

        public float[] GetTriggerDurations(float clipLength)
        {
            if (multiHitTriggerDurationRates != null &&
                multiHitTriggerDurationRates.Length > 0)
            {
                float[] durations = new float[multiHitTriggerDurationRates.Length];
                for (int i = 0; i < durations.Length; ++i)
                {
                    durations[i] = clipLength * multiHitTriggerDurationRates[i];
                }
                return durations;
            }
            return new float[] { clipLength * triggerDurationRate };
        }

        public float GetTotalDuration(float clipLength)
        {
            return clipLength + extraDuration;
        }
    }

    [System.Serializable]
    public struct WeaponAnimations2D : IWeaponAnims
    {
        public WeaponType weaponType;
        public ActionAnimation2D rightHandAttackAnimation;
        public ActionAnimation2D leftHandAttackAnimation;
        public ActionAnimation2D rightHandReloadAnimation;
        public ActionAnimation2D leftHandReloadAnimation;
        public WeaponType Data { get { return weaponType; } }
    }

    [System.Serializable]
    public struct SkillAnimations2D : ISkillAnims
    {
        public BaseSkill skill;
        public CharacterAnimation2D castAnimation;
        public SkillActivateAnimationType activateAnimationType;
        [StringShowConditional(nameof(activateAnimationType), nameof(SkillActivateAnimationType.UseActivateAnimation))]
        public ActionAnimation2D activateAnimation;
        public BaseSkill Data { get { return skill; } }
    }
}
