using Insthync.UnityEditorUtils;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AnimatorCharacterAnimation2D
    {
        [Header("4-Directions")]
        public AnimationClip down;
        public AnimationClip up;
        public AnimationClip left;
        public AnimationClip right;
        [Header("8-Directions")]
        public AnimationClip downLeft;
        public AnimationClip downRight;
        public AnimationClip upLeft;
        public AnimationClip upRight;

        public AnimationClip GetClipByDirection(DirectionType2D directionType)
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
    public class AnimatorActionAnimation2D : AnimatorCharacterAnimation2D
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
    public struct AnimatorWeaponAnimations2D : IWeaponAnims
    {
        public WeaponType weaponType;
        public AnimatorActionAnimation2D rightHandAttackAnimation;
        public AnimatorActionAnimation2D leftHandAttackAnimation;
        public AnimatorActionAnimation2D rightHandReloadAnimation;
        public AnimatorActionAnimation2D leftHandReloadAnimation;
        public WeaponType Data { get { return weaponType; } }
    }

    [System.Serializable]
    public struct AnimatorSkillAnimations2D : ISkillAnims
    {
        public BaseSkill skill;
        [FormerlySerializedAs("castAnimation")]
        public AnimatorCharacterAnimation2D castClip;
        public SkillActivateAnimationType activateAnimationType;
        [StringShowConditional(nameof(activateAnimationType), nameof(SkillActivateAnimationType.UseActivateAnimation))]
        public AnimatorActionAnimation2D activateAnimation;
        public BaseSkill Data { get { return skill; } }
    }
}
