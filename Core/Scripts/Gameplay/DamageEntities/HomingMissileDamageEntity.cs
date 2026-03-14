using UnityEngine;

namespace MultiplayerARPG
{
    public class HomingMissileDamageEntity : MissileDamageEntity
    {
        [Header("Homing Missile Settings")]
        public float seekDelay;
        public float seekTargetDetectDistance;
        [Range(0, 360)]
        public float seekAngle = 360f;
        public float seekDamping = 10f;

        private PhysicFunctions _physicFunctions = new PhysicFunctions(32);

        public override void ManagedUpdate()
        {
            if (Destroying)
                return;

            if (Time.unscaledTime - _launchTime >= _missileDuration)
            {
                Explode();
                PushBack(destroyDelay);
                Destroying = true;
            }

            HitDetect();

            if (Destroying)
                return;

            UpdateMovement();
        }
        public virtual float SeekDirection()
        {
            return Mathf.Cos(seekAngle / 2 * Mathf.Deg2Rad);
        }

        private void UpdateMovement()
        {
            if (Time.unscaledTime - _launchTime < seekDelay)
                return;

            if (_lockingTarget == null)
            {
                float minDistance = int.MaxValue;
                int hitCount = _physicFunctions.OverlapObjects(CacheTransform.position, seekTargetDetectDistance, GameInstance.Singleton.GetTargetLayerMask(), false, QueryTriggerInteraction.Collide);
                for (int i = 0; i < hitCount; ++i)
                {
                    GameObject hitObj = _physicFunctions.GetOverlapObject(i);
                    DamageableEntity hitEntity = hitObj.transform.root.GetComponent<DamageableEntity>();
                    if (hitEntity == null || hitEntity.IsDead() || hitEntity.GetObjectId() == _instigator.ObjectId || !hitEntity.CanReceiveDamageFrom(_instigator))
                        continue;
                    float currentDistance = Vector3.Distance(CacheTransform.position, hitEntity.EntityTransform.position);
                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        _lockingTarget = hitEntity;
                    }
                }
            }
            else
            {
                Vector3 targetPosition = _lockingTarget.EntityTransform.position;
                Vector3 dir = (targetPosition - CacheTransform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(dir);
                float direction = Vector3.Dot(dir, CacheTransform.forward);
                if (direction > SeekDirection())
                    CacheTransform.rotation = Quaternion.Slerp(CacheTransform.rotation, rotation, Time.deltaTime * seekDamping);
            }

            if (CurrentGameInstance.DimensionType == DimensionType.Dimension2D)
                CacheTransform.position += -CacheTransform.up * _missileSpeed * Time.deltaTime;
            else
                CacheTransform.position += CacheTransform.forward * _missileSpeed * Time.deltaTime;
        }
    }
}
