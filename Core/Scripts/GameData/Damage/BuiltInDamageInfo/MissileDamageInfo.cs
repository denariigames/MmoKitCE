using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class MissileDamageInfo : BaseCustomDamageInfo
    {
        [Tooltip("If this is TRUE, it will hit only selected target, if no selected target it will hit 1 random target")]
        public bool hitOnlySelectedTarget;
        public float missileDistance;
        public float missileSpeed;
        public bool isHeadshotInstantDeath;
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableMissileDamageEntity))]
#endif
        public MissileDamageEntity missileDamageEntity;
#endif
        public MissileDamageEntity MissileDamageEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return missileDamageEntity;
#else
                return null;
#endif
            }
        }
#if !DISABLE_ADDRESSABLES
        public AssetReferenceMissileDamageEntity addressableMissileDamageEntity;
        public AssetReferenceMissileDamageEntity AddressableMissileDamageEntity
        {
            get => addressableMissileDamageEntity;
        }
#endif
        private MissileDamageEntity.HitDetectionMode _hitDetectionMode;
        private float _sphereCastRadius;
        private Vector3 _boxCastSize;
        private float _explodeDistance;

        public override async void PrepareRelatesData()
        {
            MissileDamageEntity loadedDamageEntity;
#if !DISABLE_ADDRESSABLES
            loadedDamageEntity = await AddressableMissileDamageEntity
                .GetOrLoadAssetAsyncOrUsePrefab(MissileDamageEntity);
#else
            loadedDamageEntity = MissileDamageEntity;
#endif
            PrepareHitValidationData(loadedDamageEntity);
#if !DISABLE_ADDRESSABLES
            if (AddressableMissileDamageEntity.IsDataValid())
                AddressableAssetsManager.Release(AddressableMissileDamageEntity.RuntimeKey);
#endif
            await UniTask.Yield();
        }

        protected void PrepareHitValidationData(MissileDamageEntity damageEntity)
        {
            if (damageEntity == null)
                return;
            _hitDetectionMode = damageEntity.hitDetectionMode;
            _sphereCastRadius = damageEntity.sphereCastRadius;
            _boxCastSize = damageEntity.boxCastSize;
            _explodeDistance = damageEntity.explodeDistance;
        }

        public override Transform GetDamageTransform(BaseCharacterEntity attacker, bool isLeftHand)
        {
            Transform transform = null;
            if (attacker.ModelManager.IsFps)
            {
                if (attacker.FpsModel && attacker.FpsModel.gameObject.activeSelf)
                {
                    // Spawn bullets from fps model
                    transform = isLeftHand ? attacker.FpsModel.GetLeftHandMissileDamageTransform() : attacker.FpsModel.GetRightHandMissileDamageTransform();
                }
            }
            else
            {
                // Spawn bullets from tps model
                transform = isLeftHand ? attacker.CharacterModel.GetLeftHandMissileDamageTransform() : attacker.CharacterModel.GetRightHandMissileDamageTransform();
            }

            if (transform == null)
            {
                // Still no missile transform, use default missile transform
                transform = attacker.MissileDamageTransform;
            }
            return transform;
        }

        public override float GetDistance()
        {
            return missileDistance;
        }

        public override float GetFov()
        {
            return 10f;
        }

        public override bool IsHitValid(HitValidateData hitValidateData, HitRegisterData hitData, DamageableHitBox hitBox)
        {
            float hitBoxMaxExtents = Mathf.Max(hitBox.Bounds.extents.x, hitBox.Bounds.extents.y, hitBox.Bounds.extents.z);
            float missileHitDist = 0f;
            switch (_hitDetectionMode)
            {
                case MissileDamageEntity.HitDetectionMode.SphereCast:
                    missileHitDist = _sphereCastRadius;
                    break;
                case MissileDamageEntity.HitDetectionMode.BoxCast:
                    missileHitDist = Mathf.Max(_boxCastSize.x * 0.5f, _boxCastSize.y * 0.5f, _boxCastSize.z * 0.5f);
                    break;
            }
            // Not in hit distance?
            float dist = Vector3.Distance(hitData.Origin, hitData.HitOrigin);
            Vector3 dir = hitData.Direction;
            if (hitData.HitDestination.HasValue)
            {
                float distFromDest = Vector3.Distance(hitData.Origin, hitData.HitDestination.Value);
                Vector3 dirFromDest = (hitData.HitDestination.Value - hitData.Origin).normalized;
                // If it almost in the same direction then use `distFromDest` instead of `dist`
                if (distFromDest < dist && Vector3.Dot(dir, dirFromDest) > 0.75f)
                {
                    dist = distFromDest;
                    if (distFromDest > missileDistance + hitBoxMaxExtents + missileHitDist)
                    {
                        return false;
                    }
                }
                else if (dist > missileDistance + hitBoxMaxExtents + missileHitDist)
                {
                    return false;
                }
            }
            else if (dist > missileDistance + hitBoxMaxExtents + missileHitDist)
            {
                return false;
            }
            float explodeDistance = _explodeDistance;
            if (explodeDistance > 0f && hitData.HitDestination.HasValue)
            {
                float distHitPoints = Vector3.Distance(hitData.HitOrigin, hitData.HitDestination.Value);
                if (distHitPoints > explodeDistance + hitBoxMaxExtents)
                {
                    return false;
                }
            }
            // Missile speed validation, accept if speed <= 105% (5% for lagging)
            return IsAcceptHitBetweenTime(dist, hitData.LaunchTimestamp, hitData.HitTimestamp, 1.05);
        }

        public override bool IsHeadshotInstantDeath()
        {
            return isHeadshotInstantDeath;
        }

        private bool IsAcceptHitBetweenTime(float dist, long launchTimestamp, long hitTimestamp, double acceptableRate)
        {
            double duration = hitTimestamp - launchTimestamp;
            double distInProperTimeUnit = dist * 1000;
            double calculatedSpeed = distInProperTimeUnit / duration;
            if (calculatedSpeed / missileSpeed > acceptableRate)
                return false;
            return true;
        }

        public override async UniTask LaunchDamageEntity(BaseCharacterEntity attacker, bool isLeftHand, CharacterItem weapon, int simulateSeed, byte triggerIndex, byte spreadIndex, Vector3 fireSpreadRange, List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts, BaseSkill skill, int skillLevel, AimPosition aimPosition)
        {
            MissileDamageEntity loadedDamageEntity;
#if !DISABLE_ADDRESSABLES
            loadedDamageEntity = await AddressableMissileDamageEntity
                .GetOrLoadAssetAsyncOrUsePrefab(MissileDamageEntity);
#else
            loadedDamageEntity = MissileDamageEntity;
#endif

            // Spawn missile damage entity, it will move to target then apply damage when hit
            // Instantiates on both client and server (damage applies at server)
            if (loadedDamageEntity == null)
                return;

#if UNITY_EDITOR
            // Store data for testing
            PrepareHitValidationData(loadedDamageEntity);
#endif

            // Get generic attack data
            EntityInfo instigator = attacker.GetInfo();
            System.Random random = new System.Random(unchecked(simulateSeed + ((triggerIndex + 1) * (spreadIndex + 1) * 16)));
            Vector3 spreadRange = new Vector3(GenericUtils.RandomFloat(random.Next(), -fireSpreadRange.x, fireSpreadRange.x), GenericUtils.RandomFloat(random.Next(), -fireSpreadRange.y, fireSpreadRange.y));
            this.GetDamagePositionAndRotation(attacker, isLeftHand, aimPosition, spreadRange, out Vector3 damagePosition, out Vector3 damageDirection, out Quaternion damageRotation);
            // Prepare hit reg data
            HitRegisterData hitRegData = new HitRegisterData()
            {
                SimulateSeed = simulateSeed,
                TriggerIndex = triggerIndex,
                SpreadIndex = spreadIndex,
                LaunchTimestamp = BaseGameNetworkManager.Singleton.ServerTimestamp,
                Origin = damagePosition,
                Direction = damageDirection,
            };

            DamageableEntity lockingTarget;
            if (!hitOnlySelectedTarget || !attacker.TryGetTargetEntity(out lockingTarget))
                lockingTarget = null;

            // Instantiate missile damage entity
            float missileDistance = this.missileDistance;
            float missileSpeed = this.missileSpeed;
            PoolSystem.GetInstance(loadedDamageEntity, damagePosition, damageRotation)
                .Setup(instigator, weapon, simulateSeed, triggerIndex, spreadIndex, damageAmounts[triggerIndex], skill, skillLevel, hitRegData, missileDistance, missileSpeed, lockingTarget);
            await UniTask.Yield();
        }
    }
}