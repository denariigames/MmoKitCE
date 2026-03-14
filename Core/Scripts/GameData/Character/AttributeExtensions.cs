using System.Collections.Generic;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    public static class AttributeExtensions
    {
        public static CharacterStats GetStats(this Dictionary<Attribute, float> entries)
        {
            CharacterStats result = new CharacterStats();
            if (entries == null || entries.Count == 0)
                return result;
            foreach (KeyValuePair<Attribute, float> entry in entries)
            {
                result += entry.Key.GetStats(entry.Value);
            }
            return result;
        }

        public static CharacterStats GetStats(this Attribute attribute, float level)
        {
            if (attribute == null)
                return new CharacterStats();
            return attribute.GetStatsByLevel(level);
        }

        public static CharacterStats GetStats(this AttributeAmount attributeAmount)
        {
            if (attributeAmount.attribute == null)
                return new CharacterStats();
            Attribute attribute = attributeAmount.attribute;
            return attribute.GetStats(attributeAmount.amount);
        }

        public static CharacterStats GetStats(this AttributeIncremental attributeIncremental, int level)
        {
            if (attributeIncremental.attribute == null)
                return new CharacterStats();
            Attribute attribute = attributeIncremental.attribute;
            return attribute.GetStats(attributeIncremental.amount.GetAmount(level));
        }

        public static void GetIncreaseResistances(this Dictionary<Attribute, float> entries, Dictionary<DamageElement, float> result)
        {
            result.Clear();
            if (entries == null || entries.Count == 0)
                return;
            foreach (KeyValuePair<Attribute, float> entry in entries)
            {
                if (entry.Key == null)
                    continue;
                using (CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get(out Dictionary<DamageElement, float> tempData))
                {
                    entry.Key.GetIncreaseResistances(entry.Value, tempData);
                    GameDataHelpers.CombineResistances(result, tempData);
                }
            }
        }

        public static void GetIncreaseResistances(this Attribute attribute, float amount, Dictionary<DamageElement, float> result)
        {
            result.Clear();
            if (attribute != null)
                attribute.GetIncreaseResistancesByLevel(amount, result);
        }

        public static void GetIncreaseArmors(this Dictionary<Attribute, float> entries, Dictionary<DamageElement, float> result)
        {
            result.Clear();
            if (entries == null || entries.Count == 0)
                return;
            foreach (KeyValuePair<Attribute, float> entry in entries)
            {
                if (entry.Key == null)
                    continue;
                using (CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get(out Dictionary<DamageElement, float> tempData))
                {
                    entry.Key.GetIncreaseArmors(entry.Value, tempData);
                    GameDataHelpers.CombineArmors(result, tempData);
                }
            }
        }

        public static void GetIncreaseArmors(this Attribute attribute, float amount, Dictionary<DamageElement, float> result)
        {
            result.Clear();
            if (attribute != null)
                attribute.GetIncreaseArmorsByLevel(amount, result);
        }

        public static void GetIncreaseDamages(this Dictionary<Attribute, float> entries, Dictionary<DamageElement, MinMaxFloat> result)
        {
            result.Clear();
            if (entries == null || entries.Count == 0)
                return;
            foreach (KeyValuePair<Attribute, float> entry in entries)
            {
                if (entry.Key == null)
                    continue;
                using (CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get(out Dictionary<DamageElement, MinMaxFloat> tempData))
                {
                    entry.Key.GetIncreaseDamages(entry.Value, tempData);
                    GameDataHelpers.CombineDamages(result, tempData);
                }
            }
        }

        public static void GetIncreaseDamages(this Attribute attribute, float amount, Dictionary<DamageElement, MinMaxFloat> result)
        {
            result.Clear();
            if (attribute != null)
                attribute.GetIncreaseDamagesByLevel(amount, result);
        }

        public static void GetIncreaseStatusEffectResistances(this Dictionary<Attribute, float> entries, Dictionary<StatusEffect, float> result)
        {
            result.Clear();
            if (entries == null || entries.Count == 0)
                return;
            foreach (KeyValuePair<Attribute, float> entry in entries)
            {
                if (entry.Key == null)
                    continue;
                using (CollectionPool<Dictionary<StatusEffect, float>, KeyValuePair<StatusEffect, float>>.Get(out Dictionary<StatusEffect, float> tempData))
                {
                    entry.Key.GetIncreaseStatusEffectResistances(entry.Value, tempData);
                    GameDataHelpers.CombineStatusEffectResistances(result, tempData);
                }
            }
        }

        public static void GetIncreaseStatusEffectResistances(this Attribute attribute, float amount, Dictionary<StatusEffect, float> result)
        {
            result.Clear();
            if (attribute != null)
                attribute.GetIncreaseStatusEffectResistancesByLevel(amount, result);
        }
    }
}
