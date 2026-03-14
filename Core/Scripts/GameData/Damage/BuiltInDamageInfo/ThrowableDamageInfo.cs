using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class ThrowableDamageInfo : BaseCustomDamageInfo
    {
        public float throwForce;
        public float throwableLifeTime;
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableThrowableDamageEntity))]
#endif
        public ThrowableDamageEntity throwableDamageEntity;
#endif
        public ThrowableDamageEntity ThrowableDamageEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return throwableDamageEntity;
#else
                return null;
#endif
            }
        }
#if !DISABLE_ADDRESSABLES
        public AssetReferenceThrowableDamageEntity addressableThrowableDamageEntity;
        public AssetReferenceThrowableDamageEntity AddressableThrowableDamageEntity
        {
            get => addressableThrowableDamageEntity;
        }
#endif
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
            // NOTE: It is actually can't find actual distance by simple math because it has many factors,
            // Such as thrown position, distance from ground, gravity. 
            // So all throwable weapons are suited for shooter games only.
            return throwForce * 0.5f;
        }

        public override float GetFov()
        {
            return 10f;
        }

        public override bool IsHitValid(HitValidateData hitValidateData, HitRegisterData hitData, DamageableHitBox hitBox)
        {
            return true;
        }

        public override async UniTask LaunchDamageEntity(BaseCharacterEntity attacker, bool isLeftHand, CharacterItem weapon, int simulateSeed, byte triggerIndex, byte spreadIndex, Vector3 fireSpreadRange, List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts, BaseSkill skill, int skillLevel, AimPosition aimPosition)
        {
            ThrowableDamageEntity loadedDamageEntity;
#if !DISABLE_ADDRESSABLES
            loadedDamageEntity = await AddressableThrowableDamageEntity
                .GetOrLoadAssetAsyncOrUsePrefab(ThrowableDamageEntity);
#else
            loadedDamageEntity = ThrowableDamageEntity;
#endif

            if (loadedDamageEntity == null)
                return;

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

            // Instantiate throwable damage entity
            // TODO: May predict and move missile ahead of time based on client's RTT
            float throwForce = this.throwForce;
            float throwableLifeTime = this.throwableLifeTime;
            PoolSystem.GetInstance(loadedDamageEntity, damagePosition, damageRotation)
                .Setup(instigator, weapon, simulateSeed, triggerIndex, spreadIndex, damageAmounts[triggerIndex], skill, skillLevel, hitRegData, throwForce, throwableLifeTime);
            await UniTask.Yield();
        }
    }
}