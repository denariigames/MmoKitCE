using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        public void ValidateRecovery(EntityInfo instigator)
        {
            if (!IsServer)
                return;

            // Validate Hp
            if (CurrentHp < 0)
                CurrentHp = 0;
            if (CurrentHp > CachedData.MaxHp)
                CurrentHp = CachedData.MaxHp;
            // Validate Mp
            if (CurrentMp < 0)
                CurrentMp = 0;
            if (CurrentMp > CachedData.MaxMp)
                CurrentMp = CachedData.MaxMp;
            // Validate Stamina
            if (CurrentStamina < 0)
                CurrentStamina = 0;
            if (CurrentStamina > CachedData.MaxStamina)
                CurrentStamina = CachedData.MaxStamina;
            // Validate Food
            if (CurrentFood < 0)
                CurrentFood = 0;
            if (CurrentFood > CachedData.MaxFood)
                CurrentFood = CachedData.MaxFood;
            // Validate Water
            if (CurrentWater < 0)
                CurrentWater = 0;
            if (CurrentWater > CachedData.MaxWater)
                CurrentWater = CachedData.MaxWater;

            if (this.IsDead())
                Killed(instigator);
        }

        public virtual void Killed(EntityInfo lastAttacker)
        {
            StopAllCoroutines();
            for (int i = buffs.Count - 1; i >= 0; --i)
            {
                if (buffs[i].GetBuff().GetBuff().doNotRemoveOnDead)
                    continue;
                OnRemoveBuff(buffs[i], BuffRemoveReasons.CharacterDead);
                buffs.RemoveAt(i);
            }
            for (int i = summons.Count - 1; i >= 0; --i)
            {
                summons[i].UnSummon(this);
                summons.RemoveAt(i);
            }
            if (CurrentGameInstance.clearSkillCooldownOnDead)
            {
                skillUsages.Clear();
            }
            CallRpcOnDead();
        }

        public virtual void OnRespawn()
        {
            if (!IsServer)
                return;
            _lastGrounded = true;
            _lastGroundedPosition = EntityTransform.position;
            RespawnGroundedCheckCountDown = RESPAWN_GROUNDED_CHECK_DURATION;
            RespawnInvincibleCountDown = RESPAWN_INVINCIBLE_DURATION;
            CallRpcOnRespawn();
        }

        public void RewardExp(int exp, float multiplier, RewardGivenType rewardGivenType, int giverLevel, int sourceLevel)
        {
            if (!IsServer || exp <= 0)
                return;
            if (!CurrentGameplayRule.RewardExp(this, exp, multiplier, rewardGivenType, giverLevel, sourceLevel, out int rewardedExp))
            {
                OnRewardExp(rewardGivenType, rewardedExp, false);
                return;
            }
            OnRewardExp(rewardGivenType, rewardedExp, true);
            CallRpcOnLevelUp();
        }

        public void RewardGold(int gold, float multiplier, RewardGivenType rewardGivenType, int giverLevel, int sourceLevel)
        {
            if (!IsServer || gold <= 0)
                return;
            CurrentGameplayRule.RewardGold(this, gold, multiplier, rewardGivenType, giverLevel, sourceLevel, out int rewardedGold);
            OnRewardGold(rewardGivenType, rewardedGold);
        }

        public void RewardCurrencies(IEnumerable<CurrencyAmount> currencies, float multiplier, RewardGivenType rewardGivenType, int giverLevel, int sourceLevel)
        {
            if (!IsServer || currencies == null)
                return;
            CurrentGameplayRule.RewardCurrencies(this, currencies, multiplier, rewardGivenType, giverLevel, sourceLevel);
            foreach (CurrencyAmount currency in currencies)
            {
                if (currency.currency == null || currency.amount <= 0)
                    continue;
                OnRewardCurrency(rewardGivenType, currency.currency, currency.amount);
            }
        }

        protected override void ApplyReceiveDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed, out CombatAmountType combatAmountType, out int totalDamage)
        {
            if (damageAmounts == null)
            {
                Logging.LogWarning($"{name}({nameof(BaseCharacterEntity)}) damage amounts dictionary is null, this should not occurring.");
                combatAmountType = CombatAmountType.Miss;
                totalDamage = 0;
                return;
            }

            if (instigator.TryGetEntity(out BaseCharacterEntity attackerCharacter))
            {
                // Notify enemy spotted when received damage from enemy
                NotifyEnemySpotted(attackerCharacter);

                // Notify enemy spotted when damage taken to enemy
                attackerCharacter.NotifyEnemySpotted(this);
            }

            float decreaseRate = 0f;
            switch (position)
            {
                case HitBoxPosition.Head:
                    decreaseRate = CachedData.HeadDamageAbsorbs;
                    break;
                case HitBoxPosition.Body:
                    decreaseRate = CachedData.BodyDamageAbsorbs;
                    break;
            }

            if (decreaseRate > 0f)
            {
                List<DamageElement> keys = new List<DamageElement>(damageAmounts.Keys);
                foreach (DamageElement key in keys)
                {
                    damageAmounts[key] = damageAmounts[key] - (damageAmounts[key] * decreaseRate);
                }
            }

            if (!CurrentGameInstance.GameplayRule.RandomAttackHitOccurs(fromPosition, attackerCharacter, this, damageAmounts, weapon, skill, skillLevel, randomSeed, out bool isCritical, out bool isBlocked))
            {
                // Don't hit (Miss)
                combatAmountType = CombatAmountType.Miss;
                totalDamage = 0;
                return;
            }

            // Calculate damages
            combatAmountType = CombatAmountType.NormalDamage;
            float calculatingTotalDamage = 0f;
            foreach (DamageElement damageElement in damageAmounts.Keys)
            {
                calculatingTotalDamage += damageElement.GetDamageReducedByResistance(CachedData.Resistances, CachedData.Armors,
                    CurrentGameInstance.GameplayRule.RandomAttackDamage(fromPosition, attackerCharacter, this, damageElement, damageAmounts[damageElement], weapon, skill, skillLevel, randomSeed));
            }

            if (attackerCharacter != null)
            {
                // If critical occurs
                if (isCritical)
                {
                    calculatingTotalDamage = CurrentGameInstance.GameplayRule.GetCriticalDamage(attackerCharacter, this, calculatingTotalDamage);
                    combatAmountType = CombatAmountType.CriticalDamage;
                }
                // If block occurs
                if (isBlocked)
                {
                    calculatingTotalDamage = CurrentGameInstance.GameplayRule.GetBlockDamage(attackerCharacter, this, calculatingTotalDamage);
                    combatAmountType = CombatAmountType.BlockedDamage;
                }
            }

            // Apply damages
            totalDamage = CurrentGameInstance.GameplayRule.GetTotalDamage(fromPosition, instigator, this, calculatingTotalDamage, weapon, skill, skillLevel);
            if (totalDamage < 0)
                totalDamage = 0;
            CurrentHp -= totalDamage;
        }

        public override void ReceivedDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CombatAmountType combatAmountType, int totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime = false)
        {
            base.ReceivedDamage(position, fromPosition, instigator, damageAmounts, combatAmountType, totalDamage, weapon, skill, skillLevel, buff, isDamageOverTime);
            instigator.TryGetEntity(out BaseCharacterEntity attackerCharacter);
            CurrentGameInstance.GameplayRule.OnCharacterReceivedDamage(attackerCharacter, this, combatAmountType, totalDamage, weapon, skill, skillLevel, buff, isDamageOverTime);

            if (combatAmountType == CombatAmountType.Miss)
                return;

            // Interrupt casting skill when receive damage
            UseSkillComponent.InterruptCastingSkill();

            // Do something when character dead
            if (this.IsDead())
            {
                AttackComponent.CancelAttack();
                UseSkillComponent.CancelSkill();
                ReloadComponent.CancelReload();

                // Call killed function, this should be called only once when dead
                ValidateRecovery(instigator);
            }
            else
            {
                // Do something with buffs when attacked
                SkillAndBuffComponent.OnAttacked();
                // Do something when skill hit target
                if (skill != null && buff.IsEmpty())
                    skill.OnSkillAttackHit(skillLevel, instigator, weapon, this);
            }
        }
    }
}
