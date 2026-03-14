using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public static partial class GameDataHelpers
    {
        #region Combine Dictionary with KeyValuePair functions
        /// <summary>
        /// Combine damage amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineDamages(Dictionary<DamageElement, MinMaxFloat> resultDictionary, KeyValuePair<DamageElement, MinMaxFloat> newEntry, float rate = 1f)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            DamageElement damageElement = newEntry.Key;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            if (!resultDictionary.ContainsKey(damageElement))
                resultDictionary[damageElement] = newEntry.Value * rate;
            else
                resultDictionary[damageElement] += newEntry.Value * rate;
            return;
        }

        /// <summary>
        /// Multiply damage amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="multiplyEntry"></param>
        /// <returns></returns>
        public static void MultiplyDamages(Dictionary<DamageElement, MinMaxFloat> resultDictionary, KeyValuePair<DamageElement, MinMaxFloat> multiplyEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (multiplyEntry.Key != null && resultDictionary.ContainsKey(multiplyEntry.Key))
                resultDictionary[multiplyEntry.Key] *= multiplyEntry.Value;
            return;
        }

        /// <summary>
        /// Combine damage infliction amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineDamageInflictions(Dictionary<DamageElement, float> resultDictionary, KeyValuePair<DamageElement, float> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            DamageElement damageElement = newEntry.Key;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            if (!resultDictionary.ContainsKey(damageElement))
                resultDictionary[damageElement] = newEntry.Value;
            else
                resultDictionary[damageElement] += newEntry.Value;
            return;
        }

        /// <summary>
        /// Combine attribute amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineAttributes(Dictionary<Attribute, float> resultDictionary, KeyValuePair<Attribute, float> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (newEntry.Key == null)
                return;
            if (!resultDictionary.ContainsKey(newEntry.Key))
                resultDictionary[newEntry.Key] = newEntry.Value;
            else
                resultDictionary[newEntry.Key] += newEntry.Value;
            return;
        }

        /// <summary>
        /// Multiply attribute amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="multiplyEntry"></param>
        /// <returns></returns>
        public static void MultiplyAttributes(Dictionary<Attribute, float> resultDictionary, KeyValuePair<Attribute, float> multiplyEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (multiplyEntry.Key != null && resultDictionary.ContainsKey(multiplyEntry.Key))
                resultDictionary[multiplyEntry.Key] *= multiplyEntry.Value;
            return;
        }

        /// <summary>
        /// Combine currency amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineCurrencies(Dictionary<Currency, int> resultDictionary, KeyValuePair<Currency, int> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (newEntry.Key == null)
                return;
            if (!resultDictionary.ContainsKey(newEntry.Key))
                resultDictionary[newEntry.Key] = newEntry.Value;
            else
                resultDictionary[newEntry.Key] += newEntry.Value;
            return;
        }

        /// <summary>
        /// Combine resistance amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineResistances(Dictionary<DamageElement, float> resultDictionary, KeyValuePair<DamageElement, float> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (newEntry.Key == null)
                return;
            if (!resultDictionary.ContainsKey(newEntry.Key))
                resultDictionary[newEntry.Key] = newEntry.Value;
            else
                resultDictionary[newEntry.Key] += newEntry.Value;
            return;
        }

        /// <summary>
        /// Combine armor amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineArmors(Dictionary<DamageElement, float> resultDictionary, KeyValuePair<DamageElement, float> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (newEntry.Key == null)
                return;
            if (!resultDictionary.ContainsKey(newEntry.Key))
                resultDictionary[newEntry.Key] = newEntry.Value;
            else
                resultDictionary[newEntry.Key] += newEntry.Value;
            return;
        }

        /// <summary>
        /// Multiply armors amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="multiplyEntry"></param>
        /// <returns></returns>
        public static void MultiplyArmors(Dictionary<DamageElement, float> resultDictionary, KeyValuePair<DamageElement, float> multiplyEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (multiplyEntry.Key != null && resultDictionary.ContainsKey(multiplyEntry.Key))
                resultDictionary[multiplyEntry.Key] *= multiplyEntry.Value;
            return;
        }

        /// <summary>
        /// Combine skill levels dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineSkills(Dictionary<BaseSkill, int> resultDictionary, KeyValuePair<BaseSkill, int> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (newEntry.Key == null)
                return;
            if (!resultDictionary.ContainsKey(newEntry.Key))
                resultDictionary[newEntry.Key] = newEntry.Value;
            else
                resultDictionary[newEntry.Key] += newEntry.Value;
            return;
        }

        /// <summary>
        /// Combine status effect resistance amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineStatusEffectResistances(Dictionary<StatusEffect, float> resultDictionary, KeyValuePair<StatusEffect, float> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (newEntry.Key == null)
                return;
            if (!resultDictionary.ContainsKey(newEntry.Key))
                resultDictionary[newEntry.Key] = newEntry.Value;
            else
                resultDictionary[newEntry.Key] += newEntry.Value;
            return;
        }

        /// <summary>
        /// Combine buff removals dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineBuffRemovals(Dictionary<BuffRemoval, float> resultDictionary, KeyValuePair<BuffRemoval, float> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (newEntry.Key == null)
                return;
            if (!resultDictionary.ContainsKey(newEntry.Key))
                resultDictionary[newEntry.Key] = newEntry.Value;
            else
                resultDictionary[newEntry.Key] += newEntry.Value;
            return;
        }

        /// <summary>
        /// Combine item amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineItems(Dictionary<BaseItem, int> resultDictionary, KeyValuePair<BaseItem, int> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (newEntry.Key == null)
                return;
            if (!resultDictionary.ContainsKey(newEntry.Key))
                resultDictionary[newEntry.Key] = newEntry.Value;
            else
                resultDictionary[newEntry.Key] += newEntry.Value;
            return;
        }

        /// <summary>
        /// Combine ammo type amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static void CombineAmmoTypes(Dictionary<AmmoType, int> resultDictionary, KeyValuePair<AmmoType, int> newEntry)
        {
            if (resultDictionary == null)
            {
                Debug.LogError("Collecton is null");
                return;
            }
            if (newEntry.Key == null)
                return;
            if (!resultDictionary.ContainsKey(newEntry.Key))
                resultDictionary[newEntry.Key] = newEntry.Value;
            else
                resultDictionary[newEntry.Key] += newEntry.Value;
            return;
        }
        #endregion
    }
}
