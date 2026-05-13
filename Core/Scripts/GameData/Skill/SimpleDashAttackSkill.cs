using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.SIMPLE_DASH_ATTACK_SKILL_FILE, menuName = GameDataMenuConsts.SIMPLE_DASH_ATTACK_SKILL_MENU, order = GameDataMenuConsts.SIMPLE_DASH_ATTACK_SKILL_ORDER)]
    public class SimpleDashAttackSkill : BaseSkill, IDashAttackEventListener
    {
        [Category("Skill Casting")]
        public IncrementalFloat castDistance;

        [Category("Force Applying")]
        public ApplyMovementForceMode forceMode = ApplyMovementForceMode.Dash;
        public EntityMovementForceApplierData forceApplierData = EntityMovementForceApplierData.CreateDefault();
        [Tooltip("If turn this on it will dash to where the enemy is")]
        public bool dashToEnemyPosition;
        [Tooltip("If turn this on it will dash to where the enemy is by duration, otherwise by speed")]
        public bool dashToEnemyByDuration;
        public float dashToEnemyStoppingDistance = 0.25f;

        [Category("Damage Applying - Pre Jump")]
        public float preDashEnemyLookupRadius = -1f;
        public DamageIncremental[] preDashDamageAmounts = new DamageIncremental[0];
        public SkillKnockback preDashKnockbackEffect = new SkillKnockback()
        {
            force = 0f,
            deceleration = 0f,
            duration = 1f,
        };

        [Category("Damage Applying - Jump Moving")]
        public float dashMovingEnemyLookupRadius = -1f;
        public DamageIncremental[] dashMovingDamageAmounts = new DamageIncremental[0];
        public float dashMovingTriggerInterval = 0.25f;
        public SkillKnockback dashMovingKnockbackEffect = new SkillKnockback()
        {
            force = 0f,
            deceleration = 0f,
            duration = 1f,
        };

        [Category("Damage Applying - Post Jump")]
        public float postDashEnemyLookupRadius = -1f;
        public DamageIncremental[] postDashDamageAmounts = new DamageIncremental[0];

        public float DashMovingTriggerInterval => dashMovingTriggerInterval;
        public SkillKnockback postDashKnockbackEffect = new SkillKnockback()
        {
            force = 0f,
            deceleration = 0f,
            duration = 1f,
        };

        [Category("Attackings")]
        public StatusEffectApplying[] attackStatusEffects;

        public override SkillType SkillType
        {
            get { return SkillType.Active; }
        }

        public override float GetCastDistance(BaseCharacterEntity skillUser, int skillLevel, bool isLeftHand)
        {
            return castDistance.GetAmount(skillLevel);
        }

        public void OnPreDashAttack(BaseCharacterEntity user, int skillLevel)
        {
            if (preDashEnemyLookupRadius <= 0f)
                return;
            // Apply damage to nearby enemies
            using (CollectionPool<List<BaseCharacterEntity>, BaseCharacterEntity>.Get(out List<BaseCharacterEntity> entities))
            {
                user.FindAliveEntities(entities, user.EntityTransform.position, preDashEnemyLookupRadius, false, true, true, GameInstance.Singleton.playerLayer.Mask | GameInstance.Singleton.monsterLayer.Mask);
                if (entities == null || entities.Count == 0)
                    return;
                Dictionary<DamageElement, MinMaxFloat> damageAmounts = new Dictionary<DamageElement, MinMaxFloat>();
                GameDataHelpers.CombineDamages(preDashDamageAmounts, damageAmounts, skillLevel, 1f);
                for (int i = 0; i < entities.Count; ++i)
                {
                    entities[i].ApplyDamage(HitBoxPosition.Body, user.EntityTransform.position, user.GetInfo(), damageAmounts, CharacterItem.Empty, this, skillLevel, Random.Range(0, 255));

                    if (preDashKnockbackEffect.force > 0)
                    {
                        preDashKnockbackEffect.ApplyKnockback(user, entities[i]);
                    }
                }
            }
        }

        public void OnDashMovingToAttack(BaseCharacterEntity user, int skillLevel)
        {
            if (dashMovingEnemyLookupRadius <= 0f)
                return;
            // Check if hit something and apply damage?
            using (CollectionPool<List<BaseCharacterEntity>, BaseCharacterEntity>.Get(out List<BaseCharacterEntity> entities))
            {
                user.FindAliveEntities(entities, user.EntityTransform.position, dashMovingEnemyLookupRadius, false, true, true, GameInstance.Singleton.playerLayer.Mask | GameInstance.Singleton.monsterLayer.Mask);
                if (entities == null || entities.Count == 0)
                    return;
                Dictionary<DamageElement, MinMaxFloat> damageAmounts = new Dictionary<DamageElement, MinMaxFloat>();
                GameDataHelpers.CombineDamages(dashMovingDamageAmounts, damageAmounts, skillLevel, 1f);
                for (int i = 0; i < entities.Count; ++i)
                {
                    entities[i].ApplyDamage(HitBoxPosition.Body, user.EntityTransform.position, user.GetInfo(), damageAmounts, CharacterItem.Empty, this, skillLevel, Random.Range(0, 255));

                    if (dashMovingKnockbackEffect.force > 0)
                    {
                        dashMovingKnockbackEffect.ApplyKnockback(user, entities[i]);
                    }
                }
            }
        }

        public void OnPostDashAttack(BaseCharacterEntity user, int skillLevel)
        {
            if (postDashEnemyLookupRadius <= 0f)
                return;
            // Apply damage to nearby enemies
            using (CollectionPool<List<BaseCharacterEntity>, BaseCharacterEntity>.Get(out List<BaseCharacterEntity> entities))
            {
                user.FindAliveEntities(entities, user.EntityTransform.position, postDashEnemyLookupRadius, false, true, true, GameInstance.Singleton.playerLayer.Mask | GameInstance.Singleton.monsterLayer.Mask);
                if (entities == null || entities.Count == 0)
                    return;
                Dictionary<DamageElement, MinMaxFloat> damageAmounts = new Dictionary<DamageElement, MinMaxFloat>();
                GameDataHelpers.CombineDamages(postDashDamageAmounts, damageAmounts, skillLevel, 1f);
                for (int i = 0; i < entities.Count; ++i)
                {
                    entities[i].ApplyDamage(HitBoxPosition.Body, user.EntityTransform.position, user.GetInfo(), damageAmounts, CharacterItem.Empty, this, skillLevel, Random.Range(0, 255));

                    if (postDashKnockbackEffect.force > 0)
                    {
                        postDashKnockbackEffect.ApplyKnockback(user, entities[i]);
                    }
                }
            }
        }

        public override bool TryGetAttackStatusEffectApplyings(out StatusEffectApplying[] statusEffectApplyings)
        {
            if (IsAttack)
            {
                statusEffectApplyings = attackStatusEffects;
                return true;
            }
            return base.TryGetAttackStatusEffectApplyings(out statusEffectApplyings);
        }

        protected override void ApplySkillImplement(
            BaseCharacterEntity skillUser,
            int skillLevel,
            WeaponHandlingState weaponHandlingState,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            uint targetObjectId,
            AimPosition aimPosition)
        {
            // Teleport to aim position
            if (!skillUser.CurrentGameManager.TryGetEntityByObjectId(targetObjectId, out DamageableEntity targetEntity))
                targetEntity = null;
            targetEntity = skillUser.GetTargetEntity() as DamageableEntity;
            Vector3 direction = skillUser.EntityTransform.forward;
            float distance = 0f;
            if (targetEntity != null)
            {
                Vector3 heading = targetEntity.EntityTransform.position - skillUser.EntityTransform.position;
                distance = heading.magnitude - dashToEnemyStoppingDistance;
                direction = heading / heading.magnitude;
            }
            // Calculate data
            float speed = forceApplierData.speed;
            float deceleration = forceApplierData.deceleration;
            float duration = forceApplierData.duration;
            if (dashToEnemyPosition && distance > 0f)
            {
                if (dashToEnemyByDuration && forceApplierData.duration > 0)
                    speed = EntityMovementForceApplierData.CalculateSpeed(distance, duration, deceleration);
                else
                    duration = EntityMovementForceApplierData.CalculateDuration(distance, speed, deceleration);
            }
            skillUser.ApplyForce(forceMode, direction, ApplyMovementForceSourceType.Skill, DataId, skillLevel, speed, deceleration, duration);
        }
    }
}
