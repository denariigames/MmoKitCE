using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class MeleeDamageInfo : BaseCustomDamageInfo
    {
        [Tooltip("If this is TRUE, it will hit only selected target, if no selected target it will hit 1 random target")]
        public bool hitOnlySelectedTarget;
        public float hitDistance;
        private float originOffsets = 1f;
        [Min(10f)]
        public float hitFov;
        public ImpactEffects impactEffects;

        public override void PrepareRelatesData()
        {
            if (impactEffects != null)
                impactEffects.PrepareRelatesData();
        }

        public override Transform GetDamageTransform(BaseCharacterEntity attacker, bool isLeftHand)
        {
            return attacker.MeleeDamageTransform;
        }

        public override float GetDistance()
        {
            return hitDistance;
        }

        public override float GetFov()
        {
            return hitFov;
        }

        public override bool IsHitValid(HitValidateData hitValidateData, HitRegisterData hitData, DamageableHitBox hitBox)
        {
            if (!hitValidateData.HitsCount.TryGetValue(hitData.GetHitId(), out int hitCount))
            {
                // Set hit count to 0, if it is not in collection
                hitCount = 0;
            }
            if (hitOnlySelectedTarget && hitCount > 0)
                return false;
            return true;
        }

        public override UniTask LaunchDamageEntity(BaseCharacterEntity attacker, bool isLeftHand, CharacterItem weapon, int simulateSeed, byte triggerIndex, byte spreadIndex, Vector3 fireSpreadRange, List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts, BaseSkill skill, int skillLevel, AimPosition aimPosition)
        {
            bool isClient = attacker.IsClient;
            bool isServer = attacker.IsServer;
            bool isOwnerClient = attacker.IsOwnerClient;
            bool isOwnedByServer = attacker.IsOwnedByServer;
            bool willProceedHitRegByClient = !attacker.IsOwnedByServer && !attacker.IsOwnerHost;

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
            Vector3 damagePositionWithOffsets = damagePosition - (damageDirection * originOffsets);
            float hitDistanceWithOffsets = hitDistance + originOffsets;
#if UNITY_EDITOR
            attacker.SetDebugDamage(new BaseCharacterEntity.DebugDamageLaunch()
            {
                position = damagePositionWithOffsets,
                rotation = damageRotation,
                direction = damageDirection,
                isLeftHand = isLeftHand,
                fov = hitFov,
                distance = hitDistanceWithOffsets,
            });
#endif

            if (!isOwnedByServer && !isClient)
            {
                // Only server entities (such as monsters) and clients will launch raycast damage
                // clients do it for game effects playing, server do it to apply damage
                return default;
            }

            // Find hitting objects
            int layerMask = GameInstance.Singleton.GetDamageableLayerMask();
            int tempHitCount = attacker.AttackPhysicFunctions.OverlapObjects(damagePositionWithOffsets, hitDistanceWithOffsets, layerMask, true, QueryTriggerInteraction.Collide);
            if (tempHitCount <= 0)
                return default;

            HashSet<uint> hitObjects = new HashSet<uint>();
#if !UNITY_SERVER
            bool isPlayImpactEffects = isClient && impactEffects != null;
#endif
            DamageableHitBox tempDamageableHitBox;
            GameObject tempGameObject;
            Object tempCollider;
            bool tempIsTrigger;
            string tempTag;
            DamageableHitBox tempDamageTakenTarget = null;
            DamageableEntity tempSelectedTarget = null;
            bool hasSelectedTarget = hitOnlySelectedTarget && attacker.TryGetTargetEntity(out tempSelectedTarget);

            // Find characters that receiving damages
            for (int i = 0; i < tempHitCount; ++i)
            {
                tempGameObject = attacker.AttackPhysicFunctions.GetOverlapObject(i);
                if (!tempGameObject.GetComponent<IUnHittable>().IsNull())
                    continue;

                // Hit obstacles before hit target object
                if (attacker.AttackPhysicFunctions.SingleRaycast(damagePositionWithOffsets, damageDirection, out _, hitDistanceWithOffsets, GameInstance.Singleton.GetAttackObstacleLayerMask(), QueryTriggerInteraction.Ignore))
                    continue;

                tempCollider = attacker.AttackPhysicFunctions.GetOverlapCollider(i);
                tempIsTrigger = attacker.AttackPhysicFunctions.GetOverlapIsTrigger(i);

                Vector3 hitPoint = attacker.AttackPhysicFunctions.GetOverlapColliderClosestPoint(i, damagePosition);
                if (!tempGameObject.TryGetComponent(out tempDamageableHitBox))
                    continue;

                // Target position is not in hit fov?
                if (!attacker.IsPositionInFov(damagePositionWithOffsets, hitFov, hitPoint))
                    continue;

                if (tempDamageableHitBox.GetObjectId() == attacker.ObjectId)
                    continue;

                if (hitObjects.Contains(tempDamageableHitBox.GetObjectId()))
                    continue;

                // Add entity to table, if it found entity in the table next time it will skip. 
                // So it won't applies damage to entity repeatly.
                hitObjects.Add(tempDamageableHitBox.GetObjectId());

                // Target won't receive damage if dead or can't receive damage from this character
                if (tempDamageableHitBox.IsDead() || !tempDamageableHitBox.CanReceiveDamageFrom(instigator))
                    continue;

                if (hitOnlySelectedTarget)
                {
                    // Check with selected target
                    // Set damage taken target, it will be used in-case it can't find selected target
                    tempDamageTakenTarget = tempDamageableHitBox;
                    // The hitting entity is the selected target so break the loop to apply damage later (outside this loop)
                    if (hasSelectedTarget && tempSelectedTarget.GetObjectId() == tempDamageableHitBox.GetObjectId())
                        break;
                    continue;
                }

                // Target receives damages
                if (isServer && !willProceedHitRegByClient)
                    tempDamageableHitBox.ReceiveDamage(attacker.EntityTransform.position, instigator, damageAmounts[triggerIndex], weapon, skill, skillLevel, simulateSeed);

                // Prepare hit reg because it is hitting
                if (isOwnerClient && willProceedHitRegByClient)
                {
                    hitRegData.HitTimestamp = BaseGameNetworkManager.Singleton.ServerTimestamp;
                    hitRegData.HitObjectId = tempDamageableHitBox.GetObjectId();
                    hitRegData.HitBoxIndex = tempDamageableHitBox.Index;
                    hitRegData.HitOrigin = tempDamageableHitBox.CacheTransform.position;
                    attacker.CallCmdPerformHitRegValidation(hitRegData);
                }
#if !UNITY_SERVER
                // Instantiate impact effects
                if (isPlayImpactEffects)
                {
                    tempTag = tempDamageableHitBox.EntityGameObject.tag;
                    PlayMeleeImpactEffect(attacker, tempTag, damagePosition, tempDamageableHitBox);
                }
#endif
            }

            if (hitOnlySelectedTarget && tempDamageTakenTarget != null)
            {
                // Only 1 target will receives damages
                // Pass all receive damage condition, then apply damages
                if (isServer && !willProceedHitRegByClient)
                    tempDamageTakenTarget.ReceiveDamage(attacker.EntityTransform.position, instigator, damageAmounts[triggerIndex], weapon, skill, skillLevel, simulateSeed);

                // Prepare hit reg because it is hitting
                if (isOwnerClient && willProceedHitRegByClient)
                {
                    hitRegData.HitTimestamp = BaseGameNetworkManager.Singleton.ServerTimestamp;
                    hitRegData.HitObjectId = tempDamageTakenTarget.GetObjectId();
                    hitRegData.HitBoxIndex = tempDamageTakenTarget.Index;
                    hitRegData.HitOrigin = tempDamageTakenTarget.CacheTransform.position;
                    attacker.CallCmdPerformHitRegValidation(hitRegData);
                }
#if !UNITY_SERVER
                // Instantiate impact effects
                if (isPlayImpactEffects)
                {
                    tempTag = tempDamageTakenTarget.EntityGameObject.tag;
                    PlayMeleeImpactEffect(attacker, tempTag, damagePosition, tempDamageTakenTarget);
                }
#endif
            }

            return default;
        }

#if !UNITY_SERVER
        private void PlayMeleeImpactEffect(BaseCharacterEntity attacker, string tag, Vector3 damagePosition, DamageableHitBox hitBox)
        {
            if (impactEffects == null)
                return;
            Vector3 position = hitBox.Bounds.center;
            Vector3 targetPosition = hitBox.Bounds.center;
            targetPosition.y = damagePosition.y;
            float dist = (targetPosition - damagePosition).magnitude;
            Vector3 dir = (targetPosition - damagePosition).normalized;
            Quaternion rotation = Quaternion.LookRotation(Vector3.up, -dir);
            impactEffects.PlayEffect(tag, damagePosition + (dir * dist), rotation);
        }
#endif

#if !UNITY_SERVER
        private void PlayMeleeImpactEffect(BaseCharacterEntity attacker, string tag, Vector3 damagePosition, Vector3 hitPoint)
        {
            if (impactEffects == null)
                return;
            Vector3 dir = (hitPoint - damagePosition).normalized;
            Quaternion rotation = Quaternion.LookRotation(Vector3.up, -dir);
            impactEffects.PlayEffect(tag, hitPoint, rotation);
        }
#endif
    }
}