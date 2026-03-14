using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    public static partial class GameDataHelpers
    {
        #region Combine Dictionary with Dictionary functions
        /// <summary>
        /// Combine damage amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static void CombineDamages(Dictionary<DamageElement, MinMaxFloat> resultDictionary, Dictionary<DamageElement, MinMaxFloat> combineDictionary, float rate = 1f)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (combineDictionary == null || combineDictionary.Count <= 0)
                return;
            foreach (KeyValuePair<DamageElement, MinMaxFloat> entry in combineDictionary)
            {
                CombineDamages(resultDictionary, entry, rate);
            }
            return;
        }

        /// <summary>
        /// Multiply damage amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="multiplyDictionary"></param>
        /// <returns></returns>
        public static void MultiplyDamages(Dictionary<DamageElement, MinMaxFloat> resultDictionary, Dictionary<DamageElement, MinMaxFloat> multiplyDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (multiplyDictionary != null && multiplyDictionary.Count > 0)
            {
                // Remove attributes that are not multiplying
                using (CollectionPool<List<DamageElement>, DamageElement>.Get(out List<DamageElement> availableDamages))
                {
                    availableDamages.AddRange(resultDictionary.Keys);
                    foreach (DamageElement damage in availableDamages)
                    {
                        if (!multiplyDictionary.ContainsKey(damage))
                            resultDictionary.Remove(damage);
                    }
                }
                foreach (KeyValuePair<DamageElement, MinMaxFloat> entry in multiplyDictionary)
                {
                    MultiplyDamages(resultDictionary, entry);
                }
            }
            else
            {
                resultDictionary.Clear();
            }
            return;
        }

        /// <summary>
        /// Combine damage infliction amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static void CombineDamageInflictions(Dictionary<DamageElement, float> resultDictionary, Dictionary<DamageElement, float> combineDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (combineDictionary == null || combineDictionary.Count <= 0)
                return;
            foreach (KeyValuePair<DamageElement, float> entry in combineDictionary)
            {
                CombineDamageInflictions(resultDictionary, entry);
            }
            return;
        }

        /// <summary>
        /// Combine attribute amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static void CombineAttributes(Dictionary<Attribute, float> resultDictionary, Dictionary<Attribute, float> combineDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (combineDictionary == null || combineDictionary.Count <= 0)
                return;
            foreach (KeyValuePair<Attribute, float> entry in combineDictionary)
            {
                CombineAttributes(resultDictionary, entry);
            }
            return;
        }

        /// <summary>
        /// Multiply attribute amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="multiplyDictionary"></param>
        /// <returns></returns>
        public static void MultiplyAttributes(Dictionary<Attribute, float> resultDictionary, Dictionary<Attribute, float> multiplyDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (multiplyDictionary != null && multiplyDictionary.Count > 0)
            {
                // Remove attributes that are not multiplying
                using (CollectionPool<List<Attribute>, Attribute>.Get(out List<Attribute> availableAttributes))
                {
                    availableAttributes.AddRange(resultDictionary.Keys);
                    foreach (Attribute attribute in availableAttributes)
                    {
                        if (!multiplyDictionary.ContainsKey(attribute))
                            resultDictionary.Remove(attribute);
                    }
                }
                foreach (KeyValuePair<Attribute, float> entry in multiplyDictionary)
                {
                    MultiplyAttributes(resultDictionary, entry);
                }
            }
            else
            {
                resultDictionary.Clear();
            }
            return;
        }

        /// <summary>
        /// Combine resistance amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static void CombineResistances(Dictionary<DamageElement, float> resultDictionary, Dictionary<DamageElement, float> combineDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (combineDictionary == null || combineDictionary.Count <= 0)
                return;
            foreach (KeyValuePair<DamageElement, float> entry in combineDictionary)
            {
                CombineResistances(resultDictionary, entry);
            }
            return;
        }

        /// <summary>
        /// Combine defend amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static void CombineArmors(Dictionary<DamageElement, float> resultDictionary, Dictionary<DamageElement, float> combineDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (combineDictionary == null || combineDictionary.Count <= 0)
                return;
            foreach (KeyValuePair<DamageElement, float> entry in combineDictionary)
            {
                CombineArmors(resultDictionary, entry);
            }
            return;
        }

        /// <summary>
        /// Multiply armors amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="multiplyDictionary"></param>
        /// <returns></returns>
        public static void MultiplyArmors(Dictionary<DamageElement, float> resultDictionary, Dictionary<DamageElement, float> multiplyDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (multiplyDictionary != null && multiplyDictionary.Count > 0)
            {
                // Remove attributes that are not multiplying
                using (CollectionPool<List<DamageElement>, DamageElement>.Get(out List<DamageElement> availableArmors))
                {
                    availableArmors.AddRange(resultDictionary.Keys);
                    foreach (DamageElement armor in availableArmors)
                    {
                        if (!multiplyDictionary.ContainsKey(armor))
                            resultDictionary.Remove(armor);
                    }
                }
                foreach (KeyValuePair<DamageElement, float> entry in multiplyDictionary)
                {
                    MultiplyArmors(resultDictionary, entry);
                }
            }
            else
            {
                resultDictionary.Clear();
            }
            return;
        }

        /// <summary>
        /// Combine skill levels dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static void CombineSkills(Dictionary<BaseSkill, int> resultDictionary, Dictionary<BaseSkill, int> combineDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (combineDictionary == null || combineDictionary.Count <= 0)
                return;
            foreach (KeyValuePair<BaseSkill, int> entry in combineDictionary)
            {
                CombineSkills(resultDictionary, entry);
            }
            return;
        }

        /// <summary>
        /// Combine status effect resistance amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static void CombineStatusEffectResistances(Dictionary<StatusEffect, float> resultDictionary, Dictionary<StatusEffect, float> combineDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (combineDictionary == null || combineDictionary.Count <= 0)
                return;
            foreach (KeyValuePair<StatusEffect, float> entry in combineDictionary)
            {
                CombineStatusEffectResistances(resultDictionary, entry);
            }
            return;
        }

        /// <summary>
        /// Combine item amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static void CombineItems(Dictionary<BaseItem, int> resultDictionary, Dictionary<BaseItem, int> combineDictionary)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (combineDictionary == null || combineDictionary.Count <= 0)
                return;
            foreach (KeyValuePair<BaseItem, int> entry in combineDictionary)
            {
                CombineItems(resultDictionary, entry);
            }
            return;
        }
        #endregion
    }
}
