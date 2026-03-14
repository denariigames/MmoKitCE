using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public static partial class GameDataHelpers
    {
        #region Make KeyValuePair functions
        /// <summary>
        /// Make damage - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, MinMaxFloat> ToKeyValuePair(this DamageAmount source, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, MinMaxFloat>(damageElement, source.amount * rate);
        }

        /// <summary>
        /// Make damage amount
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, MinMaxFloat> ToKeyValuePair(this DamageIncremental source, int level, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, MinMaxFloat>(damageElement, source.amount.GetAmount(level) * rate);
        }

        /// <summary>
        /// Make damage infliction - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this DamageInflictionAmount source)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.rate);
        }

        /// <summary>
        /// Make damage infliction - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this DamageInflictionIncremental source, int level)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.rate.GetAmount(level));
        }

        /// <summary>
        /// Make attribute - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<Attribute, float> ToKeyValuePair(this AttributeAmount source, float rate)
        {
            if (source.attribute == null)
                return new KeyValuePair<Attribute, float>();
            return new KeyValuePair<Attribute, float>(source.attribute, source.amount * rate);
        }

        /// <summary>
        /// Make attribute - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<Attribute, float> ToKeyValuePair(this AttributeIncremental source, int level, float rate)
        {
            if (source.attribute == null)
                return new KeyValuePair<Attribute, float>();
            return new KeyValuePair<Attribute, float>(source.attribute, source.amount.GetAmount(level) * rate);
        }

        /// <summary>
        /// Make currency - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<Currency, int> ToKeyValuePair(this CurrencyAmount source, float rate)
        {
            return new KeyValuePair<Currency, int>(source.currency, Mathf.CeilToInt(source.amount * rate));
        }

        /// <summary>
        /// Make resistance - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this ResistanceAmount source, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.amount * rate);
        }

        /// <summary>
        /// Make resistance - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this ResistanceIncremental source, int level, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.amount.GetAmount(level) * rate);
        }

        /// <summary>
        /// Make armor - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this ArmorAmount source, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.amount * rate);
        }

        /// <summary>
        /// Make armor - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this ArmorIncremental source, int level, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.amount.GetAmount(level) * rate);
        }

        /// <summary>
        /// Make skill - level key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<BaseSkill, int> ToKeyValuePair(this SkillLevel source, float rate)
        {
            if (source.skill == null)
                return new KeyValuePair<BaseSkill, int>();
            return new KeyValuePair<BaseSkill, int>(source.skill, Mathf.CeilToInt(source.level * rate));
        }

        /// <summary>
        /// Make skill - level key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<BaseSkill, int> ToKeyValuePair(this SkillIncremental source, int level, float rate)
        {
            if (source.skill == null)
                return new KeyValuePair<BaseSkill, int>();
            return new KeyValuePair<BaseSkill, int>(source.skill, Mathf.CeilToInt(source.level.GetAmount(level) * rate));
        }

        /// <summary>
        /// Make skill - level key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="characterLevel"></param>
        /// <returns></returns>
        public static KeyValuePair<BaseSkill, int> ToKeyValuePair(this PlayerSkill source, int characterLevel)
        {
            if (source.skill == null)
                return new KeyValuePair<BaseSkill, int>();
            return new KeyValuePair<BaseSkill, int>(source.skill, source.skillLevel.GetAmount(characterLevel));
        }

        /// <summary>
        /// Make skill - level key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="characterLevel"></param>
        /// <returns></returns>
        public static KeyValuePair<BaseSkill, int> ToKeyValuePair(this MonsterSkill source, int characterLevel)
        {
            if (source.skill == null)
                return new KeyValuePair<BaseSkill, int>();
            return new KeyValuePair<BaseSkill, int>(source.skill, source.skillLevel.GetAmount(characterLevel));
        }

        /// <summary>
        /// Make status effect resistance - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<StatusEffect, float> ToKeyValuePair(this StatusEffectResistanceAmount source, float rate)
        {
            if (source.statusEffect == null)
                return new KeyValuePair<StatusEffect, float>();
            return new KeyValuePair<StatusEffect, float>(source.statusEffect, source.amount * rate);
        }

        /// <summary>
        /// Make status effect resistance - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<StatusEffect, float> ToKeyValuePair(this StatusEffectResistanceIncremental source, int level, float rate)
        {
            if (source.statusEffect == null)
                return new KeyValuePair<StatusEffect, float>();
            return new KeyValuePair<StatusEffect, float>(source.statusEffect, source.amount.GetAmount(level) * rate);
        }

        /// <summary>
        /// Make buff removal - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<BuffRemoval, float> ToKeyValuePair(this BuffRemoval source, int level, float rate)
        {
            if (source == null)
                return new KeyValuePair<BuffRemoval, float>();
            return new KeyValuePair<BuffRemoval, float>(source, source.removalChance.GetAmount(level) * rate);
        }

        /// <summary>
        /// Make item - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<BaseItem, int> ToKeyValuePair(this ItemAmount source)
        {
            if (source.item == null)
                return new KeyValuePair<BaseItem, int>();
            return new KeyValuePair<BaseItem, int>(source.item, source.amount);
        }

        /// <summary>
        /// Make ammo type - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<AmmoType, int> ToKeyValuePair(this AmmoTypeAmount source)
        {
            if (source.ammoType == null)
                return new KeyValuePair<AmmoType, int>();
            return new KeyValuePair<AmmoType, int>(source.ammoType, source.amount);
        }
        #endregion

        #region Calculate functions
        public static float GetEffectivenessDamage(Dictionary<Attribute, float> effectivenessAttributes, Dictionary<Attribute, float> characterAttributes)
        {
            float damageEffectiveness = 0f;
            if (effectivenessAttributes == null || characterAttributes == null)
                return damageEffectiveness;
            foreach (Attribute attribute in characterAttributes.Keys)
            {
                if (attribute != null && effectivenessAttributes.ContainsKey(attribute))
                    damageEffectiveness += effectivenessAttributes[attribute] * characterAttributes[attribute];
            }
            return damageEffectiveness;
        }

        public static KeyValuePair<DamageElement, MinMaxFloat> GetDamageWithEffectiveness(Dictionary<Attribute, float> effectivenessAttributes, Dictionary<Attribute, float> characterAttributes, KeyValuePair<DamageElement, MinMaxFloat> pureDamage)
        {
            float damageEffectiveness = GetEffectivenessDamage(effectivenessAttributes, characterAttributes);
            DamageElement damageElement = pureDamage.Key;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, MinMaxFloat>(damageElement, pureDamage.Value + damageEffectiveness);
        }

        public static MinMaxFloat GetSumDamages(Dictionary<DamageElement, MinMaxFloat> damages)
        {
            MinMaxFloat totalDamageAmount = new MinMaxFloat()
            {
                min = 0,
                max = 0,
            };
            if (damages == null || damages.Count <= 0)
                return totalDamageAmount;
            foreach (MinMaxFloat damageAmount in damages.Values)
            {
                totalDamageAmount += damageAmount;
            }
            return totalDamageAmount;
        }
        #endregion
    }
}