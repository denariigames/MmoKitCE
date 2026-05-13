using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct SkillKnockback
    {
        [Tooltip("Raw force to apply, not recommend to over 100 because it will make player likely fly away :P")]
        public float force;
        [Tooltip("Deceleration after apply force, more value will make knockback looks smooth, but force will decrease faster")]
        public float deceleration;
        public float duration;

        public void ApplyKnockback(BaseCharacterEntity user, BaseCharacterEntity target)
        {
            Vector3 directionFromAttacker = (target.EntityTransform.position - user.EntityTransform.position).normalized;
            Vector3 eulerAngle = Quaternion.LookRotation(directionFromAttacker).eulerAngles;
            Vector3 knockbackDirection = new Vector3(Mathf.Sin(eulerAngle.y * Mathf.Deg2Rad), 0, Mathf.Cos(eulerAngle.y * Mathf.Deg2Rad));
            target.ApplyForce(ApplyMovementForceMode.ReplaceMovement, knockbackDirection, ApplyMovementForceSourceType.None, 0, 0, force, deceleration, duration);
        }
    }
}