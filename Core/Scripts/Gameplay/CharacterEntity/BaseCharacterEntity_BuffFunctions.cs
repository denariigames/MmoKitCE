using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        private List<string> _restrictBuffTags = new List<string>();

        public virtual void ApplyBuff(int dataId, BuffType type, int level, EntityInfo buffApplier, CharacterItem buffApplierWeapon)
        {
            if (!IsServer || this.IsDead())
                return;

            Buff tempBuff = null;
            bool isExtendDuration = false;
            int maxStack = 0;
            switch (type)
            {
                case BuffType.SkillBuff:
                    if (!GameInstance.Skills.ContainsKey(dataId) || !GameInstance.Skills[dataId].TryGetBuff(out tempBuff))
                        return;
                    isExtendDuration = tempBuff.isExtendDuration;
                    maxStack = tempBuff.GetMaxStack(level);
                    break;
                case BuffType.SkillDebuff:
                    if (!GameInstance.Skills.ContainsKey(dataId) || !GameInstance.Skills[dataId].TryGetDebuff(out tempBuff))
                        return;
                    isExtendDuration = tempBuff.isExtendDuration;
                    maxStack = tempBuff.GetMaxStack(level);
                    break;
                case BuffType.PotionBuff:
                    if (!GameInstance.Items.ContainsKey(dataId) || !GameInstance.Items[dataId].IsPotion())
                        return;
                    tempBuff = (GameInstance.Items[dataId] as IPotionItem).BuffData;
                    isExtendDuration = tempBuff.isExtendDuration;
                    maxStack = tempBuff.GetMaxStack(level);
                    break;
                case BuffType.GuildSkillBuff:
                    if (!GameInstance.GuildSkills.ContainsKey(dataId))
                        return;
                    tempBuff = GameInstance.GuildSkills[dataId].Buff;
                    isExtendDuration = tempBuff.isExtendDuration;
                    maxStack = tempBuff.GetMaxStack(level);
                    break;
                case BuffType.StatusEffect:
                    if (!GameInstance.StatusEffects.ContainsKey(dataId))
                        return;
                    tempBuff = GameInstance.StatusEffects[dataId].Buff;
                    isExtendDuration = tempBuff.isExtendDuration;
                    maxStack = tempBuff.GetMaxStack(level);
                    break;
            }

            if (tempBuff.enableApplyChance && Random.value > tempBuff.applyChance.GetAmount(Level))
            {
                // Failed, so buff won't be applied
                return;
            }

            _restrictBuffTags.Clear();
            string[] tempRestrictTags;
            for (int i = 0; i < Buffs.Count; ++i)
            {
                tempRestrictTags = Buffs[i].GetBuff().GetBuff().restrictTags;
                _restrictBuffTags.AddRange(tempRestrictTags);
            }

            if (!string.IsNullOrEmpty(tempBuff.tag) && _restrictBuffTags.Contains(tempBuff.tag))
            {
                // Restricted, so don't applies the buff
                return;
            }

            if (isExtendDuration)
            {
                int buffIndex = this.IndexOfBuff(type, dataId);
                if (buffIndex >= 0)
                {
                    CharacterBuff characterBuff = Buffs[buffIndex];
                    characterBuff.level = level;
                    characterBuff.buffRemainsDuration += Buffs[buffIndex].GetBuff().GetDuration();
                    characterBuff.SetApplier(buffApplier, buffApplierWeapon);
                    Buffs[buffIndex] = characterBuff;
                    return;
                }
            }
            else
            {
                if (maxStack > 1)
                {
                    List<int> indexesOfBuff = this.IndexesOfBuff(type, dataId);
                    while (indexesOfBuff.Count + 1 > maxStack)
                    {
                        int buffIndex = indexesOfBuff[0];
                        if (buffIndex >= 0)
                        {
                            OnRemoveBuff(Buffs[buffIndex], BuffRemoveReasons.FullStack);
                            Buffs.RemoveAt(buffIndex);
                        }
                        indexesOfBuff.RemoveAt(0);
                    }
                }
                else
                {
                    // `maxStack` <= 0, assume that it's = `1`
                    int buffIndex = this.IndexOfBuff(type, dataId);
                    if (buffIndex >= 0)
                    {
                        OnRemoveBuff(Buffs[buffIndex], BuffRemoveReasons.FullStack);
                        Buffs.RemoveAt(buffIndex);
                    }
                }
            }

            CharacterBuff newBuff = CharacterBuff.Create(type, dataId, level);
            CalculatedBuff calculatedBuff = newBuff.GetBuff();

            Dictionary<BuffRemoval, float> buffRemovals = calculatedBuff.GetBuffRemovals();
            if (Buffs.Count > 0 && buffRemovals != null && buffRemovals.Count > 0)
            {
                foreach (KeyValuePair<BuffRemoval, float> buffRemoval in buffRemovals)
                {
                    if (!buffRemoval.Key.IsValid())
                    {
                        // Invalid buff data
                        continue;
                    }
                    for (int i = Buffs.Count - 1; i >= 0; --i)
                    {
                        CharacterBuff characterBuff = Buffs[i];
                        if (!string.Equals(buffRemoval.Key.GetId(), characterBuff.GetKey()))
                        {
                            // This is not a removing buff
                            continue;
                        }
                        if (!buffRemoval.Key.RandomRemoveOccurs(buffRemoval.Value, characterBuff.level))
                        {
                            // Buff removal is not occurring
                            continue;
                        }
                        // Buff removed
                        OnRemoveBuff(characterBuff, BuffRemoveReasons.RemoveByOtherBuffs);
                        Buffs.RemoveAt(i);
                    }
                    if (Buffs.Count == 0)
                        break;
                }
            }

            newBuff.Apply(buffApplier, buffApplierWeapon);
            OnApplyBuff(newBuff);
            Buffs.Add(newBuff);

            if (!calculatedBuff.NoDuration() && calculatedBuff.GetDuration() <= 0f)
            {
                CharacterRecoveryData recoveryData = new CharacterRecoveryData(this);
                recoveryData.SetupByBuff(newBuff, calculatedBuff);
                recoveryData.Apply(1f);
            }
        }

        public virtual void OnApplyBuff(CharacterBuff characterBuff)
        {
            if (onApplyBuff != null)
                onApplyBuff.Invoke(this, characterBuff);
        }

        public virtual void OnRemoveBuff(CharacterBuff characterBuff, BuffRemoveReasons reason)
        {
            if (onRemoveBuff != null)
                onRemoveBuff.Invoke(this, characterBuff, reason);
        }

        public virtual void OnBuffHpRecovery(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentHp += amount;
            CallRpcAppendCombatText(CombatAmountType.HpRecovery, HitEffectsSourceType.None, 0, amount);
            if (onBuffHpRecovery != null)
                onBuffHpRecovery.Invoke(this, causer, amount);
        }

        public virtual void OnBuffHpDecrease(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentHp -= amount;
            CallRpcAppendCombatText(CombatAmountType.HpDecrease, HitEffectsSourceType.None, 0, amount);
            if (onBuffHpDecrease != null)
                onBuffHpDecrease.Invoke(this, causer, amount);
        }

        public virtual void OnBuffMpRecovery(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentMp += amount;
            CallRpcAppendCombatText(CombatAmountType.MpRecovery, HitEffectsSourceType.None, 0, amount);
            if (onBuffMpRecovery != null)
                onBuffMpRecovery.Invoke(this, causer, amount);
        }

        public virtual void OnBuffMpDecrease(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentMp -= amount;
            CallRpcAppendCombatText(CombatAmountType.MpDecrease, HitEffectsSourceType.None, 0, amount);
            if (onBuffMpDecrease != null)
                onBuffMpDecrease.Invoke(this, causer, amount);
        }

        public virtual void OnBuffStaminaRecovery(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentStamina += amount;
            CallRpcAppendCombatText(CombatAmountType.StaminaRecovery, HitEffectsSourceType.None, 0, amount);
            if (onBuffStaminaRecovery != null)
                onBuffStaminaRecovery.Invoke(this, causer, amount);
        }

        public virtual void OnBuffStaminaDecrease(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentStamina -= amount;
            CallRpcAppendCombatText(CombatAmountType.StaminaDecrease, HitEffectsSourceType.None, 0, amount);
            if (onBuffStaminaDecrease != null)
                onBuffStaminaDecrease.Invoke(this, causer, amount);
        }

        public virtual void OnBuffFoodRecovery(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentFood += amount;
            CallRpcAppendCombatText(CombatAmountType.FoodRecovery, HitEffectsSourceType.None, 0, amount);
            if (onBuffFoodRecovery != null)
                onBuffFoodRecovery.Invoke(this, causer, amount);
        }

        public virtual void OnBuffFoodDecrease(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentFood -= amount;
            CallRpcAppendCombatText(CombatAmountType.FoodDecrease, HitEffectsSourceType.None, 0, amount);
            if (onBuffFoodDecrease != null)
                onBuffFoodDecrease.Invoke(this, causer, amount);
        }

        public virtual void OnBuffWaterRecovery(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentWater += amount;
            CallRpcAppendCombatText(CombatAmountType.WaterRecovery, HitEffectsSourceType.None, 0, amount);
            if (onBuffWaterRecovery != null)
                onBuffWaterRecovery.Invoke(this, causer, amount);
        }

        public virtual void OnBuffWaterDecrease(EntityInfo causer, int amount)
        {
            if (amount < 0)
                amount = 0;
            CurrentWater -= amount;
            CallRpcAppendCombatText(CombatAmountType.WaterDecrease, HitEffectsSourceType.None, 0, amount);
            if (onBuffWaterDecrease != null)
                onBuffWaterDecrease.Invoke(this, causer, amount);
        }
    }
}
