using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public enum DamageType : byte
    {
        Melee,
        Missile,
        Raycast,
        Throwable,
        Custom = 254
    }

    [System.Serializable]
    public class DamageInfo : IDamageInfo, IAddressableAssetConversable
    {
        public DamageType damageType;

        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Melee), nameof(DamageType.Missile) })]
        [Tooltip("If this is TRUE, it will hit only selected target, if no selected target it will hit 1 random target")]
        public bool hitOnlySelectedTarget;

        [Tooltip("Distance to start an attack, this is NOT distance to hit and apply damage, this value should be less than `hitDistance` or `missileDistance` to make sure it will hit the enemy properly. If this value <= 0 or > `hitDistance` or `missileDistance` it will re-calculate by `hitDistance` or `missileDistance`")]
        public float startAttackDistance;

        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Melee) })]
        public float hitDistance;
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Melee) })]
        [Min(10f)]
        public float hitFov;

        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Missile), nameof(DamageType.Raycast) })]
        public float missileDistance;
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Missile), nameof(DamageType.Raycast) })]
        public float missileSpeed;
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Missile) })]
        public MissileDamageEntity missileDamageEntity;
#endif
#if !DISABLE_ADDRESSABLES
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Missile) })]
        public AssetReferenceMissileDamageEntity addressableMissileDamageEntity;
#endif

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Raycast) })]
        public ProjectileEffect projectileEffect;
#endif
#if !DISABLE_ADDRESSABLES
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Raycast) })]
        public AssetReferenceProjectileEffect addressableProjectEffect;
#endif
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Raycast) })]
        public byte pierceThroughEntities;
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Melee), nameof(DamageType.Raycast) })]
        public ImpactEffects impactEffects;

        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Throwable) })]
        public float throwForce;
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Throwable) })]
        public float throwableLifeTime;
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Throwable) })]
        public ThrowableDamageEntity throwableDamageEntity;
#endif
#if !DISABLE_ADDRESSABLES
        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Throwable) })]
        public AssetReferenceThrowableDamageEntity addressableThrowableDamageEntity;
#endif

        [StringShowConditional(nameof(damageType), new string[] { nameof(DamageType.Custom) })]
        public BaseCustomDamageInfo customDamageInfo;

        private BaseCustomDamageInfo _builtInDamageInfo;

        private bool TryGetDamageInfo(out BaseCustomDamageInfo damageInfo)
        {
            damageInfo = GetDamageInfo();
            return damageInfo != null;
        }

        private BaseCustomDamageInfo GetDamageInfo()
        {
            switch (damageType)
            {
                case DamageType.Custom:
                    return customDamageInfo;
                case DamageType.Throwable:
                    if (_builtInDamageInfo == null)
                    {
                        ThrowableDamageInfo tempThrowableDamageInfo = ScriptableObject.CreateInstance<ThrowableDamageInfo>();
                        tempThrowableDamageInfo.throwForce = throwForce;
                        tempThrowableDamageInfo.throwableLifeTime = throwableLifeTime;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                        tempThrowableDamageInfo.throwableDamageEntity = throwableDamageEntity;
#endif
#if !DISABLE_ADDRESSABLES
                        tempThrowableDamageInfo.addressableThrowableDamageEntity = addressableThrowableDamageEntity;
#endif
                        _builtInDamageInfo = tempThrowableDamageInfo;
                    }
                    break;
                case DamageType.Raycast:
                    if (_builtInDamageInfo == null)
                    {
                        RaycastDamageInfo tempRaycastDamageInfo = ScriptableObject.CreateInstance<RaycastDamageInfo>();
                        tempRaycastDamageInfo.missileDistance = missileDistance;
                        tempRaycastDamageInfo.missileSpeed = missileSpeed;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                        tempRaycastDamageInfo.projectileEffect = projectileEffect;
#endif
#if !DISABLE_ADDRESSABLES
                        tempRaycastDamageInfo.addressableProjectEffect = addressableProjectEffect;
#endif
                        tempRaycastDamageInfo.pierceThroughEntities = pierceThroughEntities;
                        tempRaycastDamageInfo.impactEffects = impactEffects;
                        _builtInDamageInfo = tempRaycastDamageInfo;
                    }
                    break;
                case DamageType.Missile:
                    if (_builtInDamageInfo == null)
                    {
                        MissileDamageInfo tempMissileDamageInfo = ScriptableObject.CreateInstance<MissileDamageInfo>();
                        tempMissileDamageInfo.hitOnlySelectedTarget = hitOnlySelectedTarget;
                        tempMissileDamageInfo.missileDistance = missileDistance;
                        tempMissileDamageInfo.missileSpeed = missileSpeed;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                        tempMissileDamageInfo.missileDamageEntity = missileDamageEntity;
#endif
#if !DISABLE_ADDRESSABLES
                        tempMissileDamageInfo.addressableMissileDamageEntity = addressableMissileDamageEntity;
#endif
                        _builtInDamageInfo = tempMissileDamageInfo;
                    }
                    break;
                default:
                    if (_builtInDamageInfo == null)
                    {
                        MeleeDamageInfo tempMeleeDamageInfo = ScriptableObject.CreateInstance<MeleeDamageInfo>();
                        tempMeleeDamageInfo.hitOnlySelectedTarget = hitOnlySelectedTarget;
                        tempMeleeDamageInfo.hitDistance = hitDistance;
                        tempMeleeDamageInfo.hitFov = hitFov;
                        tempMeleeDamageInfo.impactEffects = impactEffects;
                        _builtInDamageInfo = tempMeleeDamageInfo;
                    }
                    break;
            }
            return _builtInDamageInfo;
        }

        public float GetDistance()
        {
            float dist = TryGetDamageInfo(out BaseCustomDamageInfo dmgInfo) ? dmgInfo.GetDistance() : 0f;
            if (startAttackDistance > 0 && startAttackDistance < dist)
                dist = startAttackDistance;
            return dist;
        }

        public float GetFov()
        {
            return TryGetDamageInfo(out BaseCustomDamageInfo dmgInfo) ? dmgInfo.GetFov() : 0f;
        }

        public Transform GetDamageTransform(BaseCharacterEntity attacker, bool isLeftHand)
        {
            return TryGetDamageInfo(out BaseCustomDamageInfo dmgInfo) ? dmgInfo.GetDamageTransform(attacker, isLeftHand) : null;
        }

        public async UniTask LaunchDamageEntity(
            BaseCharacterEntity attacker,
            bool isLeftHand,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            Vector3 fireSpreadRange,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            BaseSkill skill,
            int skillLevel,
            AimPosition aimPosition)
        {
            // No attacker
            if (attacker == null)
                return;

            // Don't launch if character dead
            if (attacker.IsServer && attacker.IsDead())
                return;

            if (TryGetDamageInfo(out BaseCustomDamageInfo dmgInfo))
            {
                await dmgInfo.LaunchDamageEntity(
                    attacker,
                    isLeftHand,
                    weapon,
                    simulateSeed,
                    triggerIndex,
                    spreadIndex,
                    fireSpreadRange,
                    damageAmounts,
                    skill,
                    skillLevel,
                    aimPosition);
            }

            // Trigger attacker's on launch damage entity event
            attacker.OnLaunchDamageEntity(
                isLeftHand,
                weapon,
                simulateSeed,
                triggerIndex,
                spreadIndex,
                damageAmounts,
                skill,
                skillLevel,
                aimPosition);
        }

        public void PrepareRelatesData()
        {
            if (TryGetDamageInfo(out BaseCustomDamageInfo dmgInfo))
                dmgInfo.PrepareRelatesData();
        }

        public bool IsHitValid(HitValidateData hitValidateData, HitRegisterData hitData, DamageableHitBox hitBox)
        {
            return TryGetDamageInfo(out BaseCustomDamageInfo dmgInfo) ? dmgInfo.IsHitValid(hitValidateData, hitData, hitBox) : false;
        }

        public bool IsHeadshotInstantDeath()
        {
            return TryGetDamageInfo(out BaseCustomDamageInfo dmgInfo) ? dmgInfo.IsHeadshotInstantDeath() : false;
        }

        public void ProceedAddressableAssetConversion(string groupName)
        {
#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
            AddressableEditorUtils.ConvertObjectRefToAddressable(ref missileDamageEntity, ref addressableMissileDamageEntity, groupName);
            AddressableEditorUtils.ConvertObjectRefToAddressable(ref projectileEffect, ref addressableProjectEffect, groupName);
            AddressableEditorUtils.ConvertObjectRefToAddressable(ref throwableDamageEntity, ref addressableThrowableDamageEntity, groupName);
#endif
        }
    }

    [System.Serializable]
    public struct DamageAmount
    {
        [Tooltip("If `damageElement` is empty it will use default damage element from game instance")]
        public DamageElement damageElement;
        public MinMaxFloat amount;
    }

    [System.Serializable]
    public struct DamageRandomAmount
    {
        [Tooltip("If `damageElement` is empty it will use default damage element from game instance")]
        public DamageElement damageElement;
        public MinMaxFloat minAmount;
        public MinMaxFloat maxAmount;
        [Range(0, 1f)]
        public float applyRate;

        public bool Apply(System.Random random)
        {
            return random.NextDouble() <= applyRate;
        }

        public DamageAmount GetRandomedAmount(System.Random random)
        {
            return new DamageAmount()
            {
                damageElement = damageElement,
                amount = new MinMaxFloat()
                {
                    min = random.RandomFloat(minAmount.min, minAmount.max),
                    max = random.RandomFloat(maxAmount.min, maxAmount.max),
                },
            };
        }
    }

    [System.Serializable]
    public struct DamageIncremental
    {
        [Tooltip("If `damageElement` is empty it will use default damage element from game instance")]
        public DamageElement damageElement;
        public IncrementalMinMaxFloat amount;
    }

    [System.Serializable]
    public struct DamageEffectivenessAttribute
    {
        public Attribute attribute;
        public float effectiveness;
    }

    [System.Serializable]
    public struct DamageInflictionAmount
    {
        public DamageElement damageElement;
        public float rate;
    }

    [System.Serializable]
    public struct DamageInflictionIncremental
    {
        public DamageElement damageElement;
        public IncrementalFloat rate;
    }
}
