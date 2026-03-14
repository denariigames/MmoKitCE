using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public static partial class GameDataHelpers
    {
        #region Combine Dictionary with Array functions
        /// <summary>
        /// Combine damage effectiveness attribute amounts dictionary
        /// </summary>
        /// <param name="sourceEffectivesses"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static void CombineDamageEffectivenessAttributes(DamageEffectivenessAttribute[] sourceEffectivesses, Dictionary<Attribute, float> resultDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceEffectivesses == null)
                return;
            foreach (DamageEffectivenessAttribute sourceEffectivess in sourceEffectivesses)
            {
                if (sourceEffectivess.attribute == null)
                    continue;
                if (!resultDictionary.ContainsKey(sourceEffectivess.attribute))
                    resultDictionary[sourceEffectivess.attribute] = sourceEffectivess.effectiveness;
                else
                    resultDictionary[sourceEffectivess.attribute] += sourceEffectivess.effectiveness;
            }
            return;
        }

        /// <summary>
        /// Combine damage amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineDamages(DamageAmount[] sourceAmounts, Dictionary<DamageElement, MinMaxFloat> resultDictionary, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceAmounts == null)
                return;
            KeyValuePair<DamageElement, MinMaxFloat> pair;
            foreach (DamageAmount sourceAmount in sourceAmounts)
            {
                pair = ToKeyValuePair(sourceAmount, rate);
                CombineDamages(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine damage amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineDamages(DamageIncremental[] sourceIncrementals, Dictionary<DamageElement, MinMaxFloat> resultDictionary, int level, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceIncrementals == null)
                return;
            KeyValuePair<DamageElement, MinMaxFloat> pair;
            foreach (DamageIncremental sourceIncremental in sourceIncrementals)
            {
                pair = ToKeyValuePair(sourceIncremental, level, rate);
                CombineDamages(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine damage infliction amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static void CombineDamageInflictions(DamageInflictionIncremental[] sourceIncrementals, Dictionary<DamageElement, float> resultDictionary, int level)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceIncrementals == null)
                return;
            KeyValuePair<DamageElement, float> pair;
            foreach (DamageInflictionIncremental sourceIncremental in sourceIncrementals)
            {
                pair = ToKeyValuePair(sourceIncremental, level);
                CombineDamageInflictions(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine attribute amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineAttributes(AttributeAmount[] sourceAmounts, Dictionary<Attribute, float> resultDictionary, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceAmounts == null)
                return;
            KeyValuePair<Attribute, float> pair;
            foreach (AttributeAmount sourceAmount in sourceAmounts)
            {
                pair = ToKeyValuePair(sourceAmount, rate);
                CombineAttributes(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine attribute amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineAttributes(AttributeIncremental[] sourceIncrementals, Dictionary<Attribute, float> resultDictionary, int level, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceIncrementals == null)
                return;
            KeyValuePair<Attribute, float> pair;
            foreach (AttributeIncremental sourceIncremental in sourceIncrementals)
            {
                pair = ToKeyValuePair(sourceIncremental, level, rate);
                CombineAttributes(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine currency amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineCurrencies(CurrencyAmount[] sourceAmounts, Dictionary<Currency, int> resultDictionary, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceAmounts == null)
                return;
            KeyValuePair<Currency, int> pair;
            foreach (CurrencyAmount sourceAmount in sourceAmounts)
            {
                pair = ToKeyValuePair(sourceAmount, rate);
                CombineCurrencies(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine resistance amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineResistances(ResistanceAmount[] sourceAmounts, Dictionary<DamageElement, float> resultDictionary, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceAmounts == null)
                return;
            KeyValuePair<DamageElement, float> pair;
            foreach (ResistanceAmount sourceAmount in sourceAmounts)
            {
                pair = ToKeyValuePair(sourceAmount, rate);
                CombineResistances(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine resistance amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineResistances(ResistanceIncremental[] sourceIncrementals, Dictionary<DamageElement, float> resultDictionary, int level, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceIncrementals == null)
                return;
            KeyValuePair<DamageElement, float> pair;
            foreach (ResistanceIncremental sourceIncremental in sourceIncrementals)
            {
                pair = ToKeyValuePair(sourceIncremental, level, rate);
                CombineResistances(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine armor amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineArmors(ArmorAmount[] sourceAmounts, Dictionary<DamageElement, float> resultDictionary, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceAmounts == null)
                return;
            KeyValuePair<DamageElement, float> pair;
            foreach (ArmorAmount sourceAmount in sourceAmounts)
            {
                pair = ToKeyValuePair(sourceAmount, rate);
                CombineArmors(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine armor amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineArmors(ArmorIncremental[] sourceIncrementals, Dictionary<DamageElement, float> resultDictionary, int level, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceIncrementals == null)
                return;
            KeyValuePair<DamageElement, float> pair;
            foreach (ArmorIncremental sourceIncremental in sourceIncrementals)
            {
                pair = ToKeyValuePair(sourceIncremental, level, rate);
                CombineArmors(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine skill levels dictionary
        /// </summary>
        /// <param name="sourceLevels"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineSkills(SkillLevel[] sourceLevels, Dictionary<BaseSkill, int> resultDictionary, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceLevels == null)
                return;
            KeyValuePair<BaseSkill, int> pair;
            foreach (SkillLevel sourceLevel in sourceLevels)
            {
                pair = ToKeyValuePair(sourceLevel, rate);
                CombineSkills(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine skill level incrementals dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineSkills(SkillIncremental[] sourceIncrementals, Dictionary<BaseSkill, int> resultDictionary, int level, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceIncrementals == null)
                return;
            KeyValuePair<BaseSkill, int> pair;
            foreach (SkillIncremental sourceIncremental in sourceIncrementals)
            {
                pair = ToKeyValuePair(sourceIncremental, level, rate);
                CombineSkills(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine player skills dictionary
        /// </summary>
        /// <param name="sourcePlayerSkills"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static void CombineSkills(PlayerSkill[] sourcePlayerSkills, Dictionary<BaseSkill, int> resultDictionary, int characterLevel)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourcePlayerSkills == null)
                return;
            KeyValuePair<BaseSkill, int> pair;
            foreach (PlayerSkill sourcePlayerSkill in sourcePlayerSkills)
            {
                pair = ToKeyValuePair(sourcePlayerSkill, characterLevel);
                CombineSkills(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine monster skills dictionary
        /// </summary>
        /// <param name="sourceMonsterSkills"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static void CombineSkills(MonsterSkill[] sourceMonsterSkills, Dictionary<BaseSkill, int> resultDictionary, int characterLevel)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceMonsterSkills == null)
                return;
            KeyValuePair<BaseSkill, int> pair;
            foreach (MonsterSkill sourceMonsterSkill in sourceMonsterSkills)
            {
                pair = ToKeyValuePair(sourceMonsterSkill, characterLevel);
                CombineSkills(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine status effect resistance amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineStatusEffectResistances(StatusEffectResistanceAmount[] sourceAmounts, Dictionary<StatusEffect, float> resultDictionary, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceAmounts == null)
                return;
            KeyValuePair<StatusEffect, float> pair;
            foreach (StatusEffectResistanceAmount sourceAmount in sourceAmounts)
            {
                pair = ToKeyValuePair(sourceAmount, rate);
                CombineStatusEffectResistances(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine status effect resistance amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineStatusEffectResistances(StatusEffectResistanceIncremental[] sourceIncrementals, Dictionary<StatusEffect, float> resultDictionary, int level, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceIncrementals == null)
                return;
            KeyValuePair<StatusEffect, float> pair;
            foreach (StatusEffectResistanceIncremental sourceIncremental in sourceIncrementals)
            {
                pair = ToKeyValuePair(sourceIncremental, level, rate);
                CombineStatusEffectResistances(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine buff removals dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineBuffRemovals(BuffRemoval[] sourceAmounts, Dictionary<BuffRemoval, float> resultDictionary, int level, float rate)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceAmounts == null)
                return;
            KeyValuePair<BuffRemoval, float> pair;
            foreach (BuffRemoval sourceAmount in sourceAmounts)
            {
                pair = ToKeyValuePair(sourceAmount, level, rate);
                CombineBuffRemovals(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine item amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static void CombineItems(ItemAmount[] sourceAmounts, Dictionary<BaseItem, int> resultDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceAmounts == null)
                return;
            KeyValuePair<BaseItem, int> pair;
            foreach (ItemAmount sourceAmount in sourceAmounts)
            {
                pair = ToKeyValuePair(sourceAmount);
                CombineItems(resultDictionary, pair);
            }
            return;
        }

        /// <summary>
        /// Combine ammo type amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static void CombineAmmoTypes(AmmoTypeAmount[] sourceAmounts, Dictionary<AmmoType, int> resultDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (sourceAmounts == null)
                return;
            KeyValuePair<AmmoType, int> pair;
            foreach (AmmoTypeAmount sourceAmount in sourceAmounts)
            {
                pair = ToKeyValuePair(sourceAmount);
                CombineAmmoTypes(resultDictionary, pair);
            }
            return;
        }
        #endregion
    }
}
