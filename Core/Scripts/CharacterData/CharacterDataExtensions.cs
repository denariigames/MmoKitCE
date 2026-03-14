using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    public static partial class CharacterDataExtensions
    {
        public static BaseCharacter GetDatabase(this ICharacterData data)
        {
            if (data == null)
            {
                // Data has not been set
                return null;
            }

            if (data is Object unityObject && unityObject == null)
            {
                // Data has not been set
                return null;
            }

            if (data.DataId == 0)
            {
                // Data has not been set
                return null;
            }

            BaseCharacter database;
            if (!GameInstance.Characters.TryGetValue(data.DataId, out database))
            {
                Logging.LogWarning($"[GetDatabase] Cannot find character database with id: {data.DataId}");
                return null;
            }

            return database;
        }

        public static int GetNextLevelExp(this ICharacterData data)
        {
            if (data == null)
                return 0;
            BaseCharacter characterData = data.GetDatabase();
            if (characterData == null || characterData.ExpTable == null)
                return 0;
            return characterData.ExpTable.GetNextLevelExp(data.Level);
        }

        public static void GetProperCurrentByNextLevelExp(this ICharacterData data, out int properCurrentExp, out int properNextLevelExp)
        {
            properCurrentExp = 0;
            properNextLevelExp = 0;
            if (data == null)
                return;
            BaseCharacter characterData = data.GetDatabase();
            if (characterData == null || characterData.ExpTable == null)
                return;
            characterData.ExpTable.GetProperCurrentByNextLevelExp(data.Level, data.Exp, out properCurrentExp, out properNextLevelExp);
        }

        public static void GetProperCurrentByNextLevelExp(this ICharacterData data, int currentLevel, int currentExp, out int properCurrentExp, out int properNextLevelExp)
        {
            properCurrentExp = 0;
            properNextLevelExp = 0;
            if (data == null)
                return;
            BaseCharacter characterData = data.GetDatabase();
            if (characterData == null || characterData.ExpTable == null)
                return;
            characterData.ExpTable.GetProperCurrentByNextLevelExp(currentLevel, currentExp, out properCurrentExp, out properNextLevelExp);
        }

        public static void GetProperCurrentByNextLevelExp(this ExpTable expTable, int currentLevel, int currentExp, out int properCurrentExp, out int properNextLevelExp)
        {
            properCurrentExp = 0;
            properNextLevelExp = 0;
            if (expTable == null)
                return;
            expTable.GetProperCurrentByNextLevelExp(currentLevel, currentExp, out properCurrentExp, out properNextLevelExp);
        }

        #region Stats calculation, make saperate stats for buffs calculation
        public static float GetTotalItemWeight(this IList<CharacterItem> itemList)
        {
            float result = 0f;
            foreach (CharacterItem item in itemList)
            {
                if (item.IsEmptySlot()) continue;
                result += item.GetItem().Weight * item.amount;
            }
            return result;
        }

        public static int GetTotalItemSlot(this IList<CharacterItem> itemList)
        {
            int result = 0;
            foreach (CharacterItem item in itemList)
            {
                if (item.IsEmptySlot()) continue;
                if (item.GetItem().NoSlotUsage) continue;
                result++;
            }
            return result;
        }
        #endregion

        #region Fill Empty Slots
        public static void FillEmptySlots(this IList<CharacterItem> itemList, bool isLimitSlot, int slotLimit)
        {
            GameInstance.Singleton.InventoryManager.FillEmptySlots(itemList, isLimitSlot, slotLimit);
        }

        public static void FillEmptySlots(this ICharacterData data, bool recacheStats = false)
        {
            if (recacheStats)
                data.MarkToMakeCaches();
            FillEmptySlots(data.NonEquipItems, GameInstance.Singleton.IsLimitInventorySlot, data.GetCaches().LimitItemSlot);
        }
        #endregion

        #region Increasing Items Will Overwhelming
        public static bool UnEquipItemWillOverwhelming(this ICharacterData data, int unEquipCount = 1)
        {
            return GameInstance.Singleton.InventoryManager.UnEquipItemWillOverwhelming(data, unEquipCount);
        }

        public static bool IncreasingItemsWillOverwhelming(this IList<CharacterItem> itemList, int dataId, int amount, bool isLimitWeight, float weightLimit, float totalItemWeight, bool isLimitSlot, int slotLimit)
        {
            return GameInstance.Singleton.InventoryManager.IncreasingItemsWillOverwhelming(itemList, dataId, amount, isLimitWeight, weightLimit, totalItemWeight, isLimitSlot, slotLimit);
        }

        public static bool IncreasingItemsWillOverwhelming(this IList<CharacterItem> itemList, IEnumerable<ItemAmount> increasingItems, bool isLimitWeight, float weightLimit, float totalItemWeight, bool isLimitSlot, int slotLimit)
        {
            if (itemList == null || increasingItems == null)
                return false;
            List<CharacterItem> simulatingItemList = new List<CharacterItem>(itemList);
            foreach (ItemAmount receiveItem in increasingItems)
            {
                if (receiveItem.item == null || receiveItem.amount <= 0) continue;
                if (simulatingItemList.IncreasingItemsWillOverwhelming(
                    receiveItem.item.DataId,
                    receiveItem.amount,
                    isLimitWeight,
                    weightLimit,
                    totalItemWeight,
                    isLimitSlot,
                    slotLimit))
                {
                    // Overwhelming
                    return true;
                }
                else
                {
                    // Add item to temp list to check it will overwhelming or not later
                    simulatingItemList.AddOrSetItems(CharacterItem.Create(receiveItem.item, 1, receiveItem.amount));
                }
            }
            return false;
        }

        public static bool IncreasingItemsWillOverwhelming(this IList<CharacterItem> itemList, IEnumerable<RewardedItem> increasingItems, bool isLimitWeight, float weightLimit, float totalItemWeight, bool isLimitSlot, int slotLimit)
        {
            if (itemList == null || increasingItems == null)
                return false;
            List<CharacterItem> simulatingItemList = new List<CharacterItem>(itemList);
            foreach (RewardedItem receiveItem in increasingItems)
            {
                if (receiveItem.item == null || receiveItem.amount <= 0) continue;
                if (simulatingItemList.IncreasingItemsWillOverwhelming(
                    receiveItem.item.DataId,
                    receiveItem.amount,
                    isLimitWeight,
                    weightLimit,
                    totalItemWeight,
                    isLimitSlot,
                    slotLimit))
                {
                    // Overwhelming
                    return true;
                }
                else
                {
                    // Add item to temp list to check it will overwhelming or not later
                    simulatingItemList.AddOrSetItems(CharacterItem.Create(receiveItem.item, receiveItem.level, receiveItem.amount, receiveItem.randomSeed));
                }
            }
            return false;
        }

        public static bool IncreasingItemsWillOverwhelming(this IList<CharacterItem> itemList, IEnumerable<CharacterItem> increasingItems, bool isLimitWeight, float weightLimit, float totalItemWeight, bool isLimitSlot, int slotLimit)
        {
            if (itemList == null || increasingItems == null)
                return false;
            List<CharacterItem> simulatingItemList = new List<CharacterItem>(itemList);
            foreach (CharacterItem receiveItem in increasingItems)
            {
                if (receiveItem.IsEmptySlot()) continue;
                if (simulatingItemList.IncreasingItemsWillOverwhelming(
                    receiveItem.dataId,
                    receiveItem.amount,
                    isLimitWeight,
                    weightLimit,
                    totalItemWeight,
                    isLimitSlot,
                    slotLimit))
                {
                    // Overwhelming
                    return true;
                }
                else
                {
                    // Add item to temp list to check it will overwhelming or not later
                    simulatingItemList.AddOrSetItems(CharacterItem.Create(receiveItem.dataId, receiveItem.level, receiveItem.amount));
                }
            }
            return false;
        }

        public static bool IncreasingItemsWillOverwhelming(this ICharacterData data, int dataId, int amount)
        {
            return IncreasingItemsWillOverwhelming(
                data.NonEquipItems,
                dataId,
                amount,
                GameInstance.Singleton.IsLimitInventoryWeight,
                data.GetCaches().LimitItemWeight,
                data.GetCaches().TotalItemWeight,
                GameInstance.Singleton.IsLimitInventorySlot,
                data.GetCaches().LimitItemSlot);
        }

        public static bool IncreasingItemsWillOverwhelming(this ICharacterData data, IEnumerable<ItemAmount> increasingItems)
        {
            return IncreasingItemsWillOverwhelming(
                data.NonEquipItems,
                increasingItems,
                GameInstance.Singleton.IsLimitInventoryWeight,
                data.GetCaches().LimitItemWeight,
                data.GetCaches().TotalItemWeight,
                GameInstance.Singleton.IsLimitInventorySlot,
                data.GetCaches().LimitItemSlot);
        }

        public static bool IncreasingItemsWillOverwhelming(this ICharacterData data, IEnumerable<RewardedItem> increasingItems)
        {
            return IncreasingItemsWillOverwhelming(
                data.NonEquipItems,
                increasingItems,
                GameInstance.Singleton.IsLimitInventoryWeight,
                data.GetCaches().LimitItemWeight,
                data.GetCaches().TotalItemWeight,
                GameInstance.Singleton.IsLimitInventorySlot,
                data.GetCaches().LimitItemSlot);
        }

        public static bool IncreasingItemsWillOverwhelming(this ICharacterData data, IEnumerable<CharacterItem> increasingItems)
        {
            return IncreasingItemsWillOverwhelming(
                data.NonEquipItems,
                increasingItems,
                GameInstance.Singleton.IsLimitInventoryWeight,
                data.GetCaches().LimitItemWeight,
                data.GetCaches().TotalItemWeight,
                GameInstance.Singleton.IsLimitInventorySlot,
                data.GetCaches().LimitItemSlot);
        }
        #endregion

        #region Decrease Then Increase Items
        public static bool AbleToDecreaseThenIncreaseItem(this IList<CharacterItem> itemList, int decreasingItemDataId, int decreasingItemAmount, int increasingItemDataId, int increasingItemAmount, bool isLimitWeight, float weightLimit, bool isLimitSlot, int slotLimit)
        {
            List<CharacterItem> tempItemList = new List<CharacterItem>(itemList);
            if (!tempItemList.DecreaseItems(decreasingItemDataId, decreasingItemAmount, isLimitSlot))
            {
                tempItemList.Clear();
                tempItemList = null;
                return false;
            }
            if (tempItemList.IncreasingItemsWillOverwhelming(increasingItemDataId, increasingItemAmount, isLimitWeight, weightLimit, tempItemList.GetTotalItemWeight(), isLimitSlot, slotLimit))
            {
                tempItemList.Clear();
                tempItemList = null;
                return false;
            }
            tempItemList.Clear();
            tempItemList = null;
            return true;
        }
        #endregion

        #region Increase Items
        public static bool IncreaseItems(this IList<CharacterItem> itemList, CharacterItem increasingItem)
        {
            return GameInstance.Singleton.InventoryManager.IncreaseItems(itemList, increasingItem);
        }

        public static void IncreaseItems(this IList<CharacterItem> itemList, IEnumerable<ItemAmount> increasingItems, System.Action<CharacterItem> onIncrease = null, System.Action<CharacterItem> onFail = null)
        {
            CharacterItem increasedItem;
            foreach (ItemAmount increasingItem in increasingItems)
            {
                if (increasingItem.item == null || increasingItem.amount <= 0) continue;
                increasedItem = CharacterItem.Create(increasingItem.item.DataId, increasingItem.level, increasingItem.amount);
                if (IncreaseItems(itemList, increasedItem))
                {
                    if (onIncrease != null)
                        onIncrease.Invoke(increasedItem);
                }
                else
                {
                    if (onFail != null)
                        onFail.Invoke(increasedItem);
                }
            }
        }

        public static void IncreaseItems(this IList<CharacterItem> itemList, IEnumerable<RewardedItem> increasingItems, System.Action<CharacterItem> onIncrease = null, System.Action<CharacterItem> onFail = null)
        {
            CharacterItem increasedItem;
            foreach (RewardedItem increasingItem in increasingItems)
            {
                if (increasingItem.item == null || increasingItem.amount <= 0) continue;
                increasedItem = CharacterItem.Create(increasingItem.item.DataId, increasingItem.level, increasingItem.amount, increasingItem.randomSeed);
                if (IncreaseItems(itemList, increasedItem))
                {
                    if (onIncrease != null)
                        onIncrease.Invoke(increasedItem);
                }
                else
                {
                    if (onFail != null)
                        onFail.Invoke(increasedItem);
                }
            }
        }

        public static void IncreaseItems(this IList<CharacterItem> itemList, IEnumerable<CharacterItem> increasingItems, System.Action<CharacterItem> onIncrease = null, System.Action<CharacterItem> onFail = null)
        {
            CharacterItem increasedItem;
            foreach (CharacterItem increasingItem in increasingItems)
            {
                if (increasingItem.IsEmptySlot()) continue;
                increasedItem = increasingItem.Clone();
                if (IncreaseItems(itemList, increasedItem))
                {
                    if (onIncrease != null)
                        onIncrease.Invoke(increasedItem);
                }
                else
                {
                    if (onFail != null)
                        onFail.Invoke(increasedItem);
                }
            }
        }

        public static bool IncreaseItems(this ICharacterData data, CharacterItem increasingItem, System.Action<CharacterItem> onIncrease = null, System.Action<CharacterItem> onFail = null)
        {
            if (IncreaseItems(data.NonEquipItems, increasingItem))
            {
                if (onIncrease != null)
                    onIncrease.Invoke(increasingItem);
                return true;
            }
            if (onFail != null)
                onFail.Invoke(increasingItem);
            return false;
        }

        public static void IncreaseItems(this ICharacterData data, IEnumerable<ItemAmount> increasingItems, System.Action<CharacterItem> onIncrease = null, System.Action<CharacterItem> onFail = null)
        {
            CharacterItem increasedItem;
            foreach (ItemAmount increasingItem in increasingItems)
            {
                if (increasingItem.item == null || increasingItem.amount <= 0) continue;
                increasedItem = CharacterItem.Create(increasingItem.item.DataId, increasingItem.level, increasingItem.amount);
                if (IncreaseItems(data.NonEquipItems, increasedItem))
                {
                    if (onIncrease != null)
                        onIncrease.Invoke(increasedItem);
                }
                else
                {
                    if (onFail != null)
                        onFail.Invoke(increasedItem);
                }
            }
        }

        public static void IncreaseItems(this ICharacterData data, IEnumerable<RewardedItem> increasingItems, System.Action<CharacterItem> onIncrease = null, System.Action<CharacterItem> onFail = null)
        {
            CharacterItem increasedItem;
            foreach (RewardedItem increasingItem in increasingItems)
            {
                if (increasingItem.item == null || increasingItem.amount <= 0) continue;
                increasedItem = CharacterItem.Create(increasingItem.item.DataId, increasingItem.level, increasingItem.amount, increasingItem.randomSeed);
                if (IncreaseItems(data.NonEquipItems, increasedItem))
                {
                    if (onIncrease != null)
                        onIncrease.Invoke(increasedItem);
                }
                else
                {
                    if (onFail != null)
                        onFail.Invoke(increasedItem);
                }
            }
        }

        public static void IncreaseItems(this ICharacterData data, IEnumerable<CharacterItem> increasingItems, System.Action<CharacterItem> onIncrease = null, System.Action<CharacterItem> onFail = null)
        {
            foreach (CharacterItem increasingItem in increasingItems)
            {
                if (increasingItem.IsEmptySlot()) continue;
                if (IncreaseItems(data.NonEquipItems, increasingItem))
                {
                    if (onIncrease != null)
                        onIncrease.Invoke(increasingItem);
                }
                else
                {
                    if (onFail != null)
                        onFail.Invoke(increasingItem);
                }
            }
        }
        #endregion

        #region Decrease Items
        public static bool DecreaseItems(this IList<CharacterItem> itemList, int dataId, int amount, bool isLimitInventorySlot, out Dictionary<int, int> decreaseItems)
        {
            return GameInstance.Singleton.InventoryManager.DecreaseItems(itemList, dataId, amount, isLimitInventorySlot, out decreaseItems);
        }

        public static bool DecreaseItems(this IList<CharacterItem> itemList, int dataId, int amount, bool isLimitInventorySlot)
        {
            return DecreaseItems(itemList, dataId, amount, isLimitInventorySlot, out _);
        }

        public static void DecreaseItems(this IList<CharacterItem> itemList, Dictionary<BaseItem, int> itemAmounts, bool isLimitInventorySlot, float multiplier = 1)
        {
            if (itemAmounts == null)
                return;
            foreach (KeyValuePair<BaseItem, int> itemAmount in itemAmounts)
            {
                DecreaseItems(itemList, itemAmount.Key.DataId, Mathf.CeilToInt(itemAmount.Value * multiplier), isLimitInventorySlot, out _);
            }
        }

        public static void DecreaseItems(this IList<CharacterItem> itemList, IEnumerable<ItemAmount> itemAmounts, bool isLimitInventorySlot, float multiplier = 1)
        {
            if (itemAmounts == null)
                return;
            foreach (ItemAmount characterItem in itemAmounts)
            {
                DecreaseItems(itemList, characterItem.item.DataId, Mathf.CeilToInt(characterItem.amount * multiplier), isLimitInventorySlot, out _);
            }
        }

        public static void DecreaseItems(this IList<CharacterItem> itemList, IEnumerable<CharacterItem> characterItems, bool isLimitInventorySlot, float multiplier = 1)
        {
            if (characterItems == null)
                return;
            foreach (CharacterItem characterItem in characterItems)
            {
                DecreaseItems(itemList, characterItem.dataId, Mathf.CeilToInt(characterItem.amount * multiplier), isLimitInventorySlot, out _);
            }
        }

        public static bool DecreaseItems(this ICharacterData data, int dataId, int amount, out Dictionary<int, int> decreaseItems)
        {
            if (DecreaseItems(data.NonEquipItems, dataId, amount, GameInstance.Singleton.IsLimitInventorySlot, out decreaseItems))
                return true;
            return false;
        }

        public static bool DecreaseItems(this ICharacterData data, int dataId, int amount)
        {
            return DecreaseItems(data, dataId, amount, out _);
        }

        public static void DecreaseItems(this ICharacterData character, Dictionary<BaseItem, int> itemAmounts, float multiplier = 1)
        {
            if (itemAmounts == null)
                return;
            foreach (KeyValuePair<BaseItem, int> itemAmount in itemAmounts)
            {
                DecreaseItems(character, itemAmount.Key.DataId, Mathf.CeilToInt(itemAmount.Value * multiplier), out _);
            }
        }

        public static void DecreaseItems(this ICharacterData character, IEnumerable<ItemAmount> itemAmounts, float multiplier = 1)
        {
            if (itemAmounts == null)
                return;
            foreach (ItemAmount itemAmount in itemAmounts)
            {
                DecreaseItems(character, itemAmount.item.DataId, Mathf.CeilToInt(itemAmount.amount * multiplier), out _);
            }
        }

        public static void DecreaseItems(this ICharacterData character, IEnumerable<CharacterItem> characterItems, float multiplier = 1)
        {
            if (characterItems == null)
                return;
            foreach (CharacterItem characterItem in characterItems)
            {
                DecreaseItems(character, characterItem.dataId, Mathf.CeilToInt(characterItem.amount * multiplier), out _);
            }
        }
        #endregion

        #region Decrease Items By Index
        public static bool DecreaseItemsByIndex(this IList<CharacterItem> itemList, int index, int amount, bool isLimitInventorySlot, bool adjustMaxAmount)
        {
            return GameInstance.Singleton.InventoryManager.DecreaseItemsByIndex(itemList, index, amount, isLimitInventorySlot, adjustMaxAmount);
        }

        public static bool DecreaseItemsByIndex(this ICharacterData data, int index, int amount, bool adjustMaxAmount)
        {
            if (DecreaseItemsByIndex(data.NonEquipItems, index, amount, GameInstance.Singleton.IsLimitInventorySlot, adjustMaxAmount))
                return true;
            return false;
        }
        #endregion

        #region Decrease Items By Indexes
        public static void DecreaseItemsByIndexes(this IList<CharacterItem> itemList, Dictionary<int, int> indexAndAmounts, bool isLimitInventorySlot)
        {
            foreach (KeyValuePair<int, int> kv in indexAndAmounts)
            {
                GameInstance.Singleton.InventoryManager.DecreaseItemsByIndex(itemList, kv.Key, kv.Value, isLimitInventorySlot, true);
            }
        }

        public static void DecreaseItemsByIndexes(this ICharacterData data, Dictionary<int, int> indexAndAmounts)
        {
            DecreaseItemsByIndexes(data.NonEquipItems, indexAndAmounts, GameInstance.Singleton.IsLimitInventorySlot);
        }
        #endregion

        #region Decrease Items By Id
        public static bool DecreaseItemsById(this IList<CharacterItem> itemList, string id, int amount, bool isLimitInventorySlot, bool adjustMaxAmount)
        {
            return GameInstance.Singleton.InventoryManager.DecreaseItemsByIndex(itemList, itemList.IndexOf(id), amount, isLimitInventorySlot, adjustMaxAmount);
        }

        public static bool DecreaseItemsById(this ICharacterData data, string id, int amount, bool adjustMaxAmount)
        {
            if (DecreaseItemsById(data.NonEquipItems, id, amount, GameInstance.Singleton.IsLimitInventorySlot, adjustMaxAmount))
                return true;
            return false;
        }
        #endregion

        #region Decrease Items By Ids
        public static void DecreaseItemsByIds(this IList<CharacterItem> itemList, Dictionary<string, int> idAndAmounts, bool isLimitInventorySlot, bool adjustMaxAmount)
        {
            foreach (KeyValuePair<string, int> kv in idAndAmounts)
            {
                GameInstance.Singleton.InventoryManager.DecreaseItemsByIndex(itemList, itemList.IndexOf(kv.Key), kv.Value, isLimitInventorySlot, adjustMaxAmount);
            }
        }

        public static void DecreaseItemsByIds(this ICharacterData data, Dictionary<string, int> idAndAmounts, bool adjustMaxAmount)
        {
            DecreaseItemsByIds(data.NonEquipItems, idAndAmounts, GameInstance.Singleton.IsLimitInventorySlot, adjustMaxAmount);
        }
        #endregion

        #region Ammo Functions
        public static bool DecreaseAmmos(this IList<CharacterItem> nonEquipItems, AmmoType ammoType, int amount, out Dictionary<DamageElement, MinMaxFloat> increaseDamageAmounts, Dictionary<CharacterItem, int> decreaseItems)
        {
            increaseDamageAmounts = new Dictionary<DamageElement, MinMaxFloat>();
            if (ammoType == null || amount < 0)
                return false;
            Dictionary<int, Dictionary<DamageElement, MinMaxFloat>> calculatingDamageAmounts = new Dictionary<int, Dictionary<DamageElement, MinMaxFloat>>();
            List<int> decreasingItemIndexes = new List<int>();
            List<int> decreasingItemAmounts = new List<int>();
            CharacterItem tempNonEquipItem;
            IAmmoItem tempAmmoItemData;
            int tempDecresingAmount;
            int i;
            for (i = 0; i < nonEquipItems.Count; ++i)
            {
                tempNonEquipItem = nonEquipItems[i];
                tempAmmoItemData = tempNonEquipItem.GetAmmoItem();
                if (tempAmmoItemData == null)
                    continue;

                if (ammoType == tempAmmoItemData.AmmoType)
                {
                    if (amount - tempNonEquipItem.amount > 0)
                        tempDecresingAmount = tempNonEquipItem.amount;
                    else
                        tempDecresingAmount = amount;
                    if (tempDecresingAmount > 0 && !calculatingDamageAmounts.ContainsKey(tempAmmoItemData.DataId))
                    {
                        using (CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get(out Dictionary<DamageElement, MinMaxFloat> tempIncreaseDamages))
                        {
                            tempAmmoItemData.GetIncreaseDamages(tempIncreaseDamages);
                            calculatingDamageAmounts.Add(tempAmmoItemData.DataId, tempIncreaseDamages);
                        }
                    }
                    amount -= tempDecresingAmount;
                    decreasingItemIndexes.Add(i);
                    decreasingItemAmounts.Add(tempDecresingAmount);
                }

                if (amount == 0)
                    break;
            }

            if (amount > 0)
                return false;

            float entryRate = 1f / calculatingDamageAmounts.Count;
            foreach (Dictionary<DamageElement, MinMaxFloat> damageAmounts in calculatingDamageAmounts.Values)
            {
                GameDataHelpers.CombineDamages(increaseDamageAmounts, damageAmounts, entryRate);
            }

            for (i = decreasingItemIndexes.Count - 1; i >= 0; --i)
            {
                if (decreaseItems != null)
                    decreaseItems.Add(nonEquipItems[decreasingItemIndexes[i]], decreasingItemAmounts[i]);
                DecreaseItemsByIndex(nonEquipItems, decreasingItemIndexes[i], decreasingItemAmounts[i], GameInstance.Singleton.IsLimitInventorySlot, true);
            }

            return true;
        }

        public static bool DecreaseAmmos(this ICharacterData data, AmmoType ammoType, int amount, out Dictionary<DamageElement, MinMaxFloat> increaseDamageAmounts, bool applyChanges = true)
        {
            if (data.CurrentHp <= 0)
            {
                increaseDamageAmounts = new Dictionary<DamageElement, MinMaxFloat>();
                return false;
            }
            IList<CharacterItem> nonEquipItems = applyChanges ? data.NonEquipItems : new List<CharacterItem>(data.NonEquipItems);
            return nonEquipItems.DecreaseAmmos(ammoType, amount, out increaseDamageAmounts, null);
        }

        public static bool DecreaseAmmos(ref EquipWeapons equipWeapons, IList<CharacterItem> nonEquipItems, bool isLimitSlot, int slotLimit, CharacterItem weapon, bool isLeftHand, int amount, out Dictionary<DamageElement, MinMaxFloat> increaseDamages, bool validIfNoRequireAmmoType = true)
        {
            increaseDamages = null;

            // Avoid null data
            if (weapon.IsEmptySlot())
                return validIfNoRequireAmmoType;

            IWeaponItem weaponItem = weapon.GetWeaponItem();
            bool hasAmmoType = weaponItem.WeaponType.AmmoType != null;
            bool hasAmmoItems = weaponItem.AmmoItemIds.Count > 0;
            if (weaponItem.AmmoCapacity > 0 && (hasAmmoType || hasAmmoItems))
            {
                // Ammo capacity >= `amount` reduce loaded ammo
                if (weapon.ammo >= amount)
                {
                    increaseDamages = new Dictionary<DamageElement, MinMaxFloat>();
                    if (GameInstance.Items.TryGetValue(weapon.ammoDataId, out BaseItem tempItemData) && tempItemData is IAmmoItem tempAmmoItem)
                        tempAmmoItem.GetIncreaseDamages(increaseDamages);
                    weapon.ammo -= amount;
                    if (isLeftHand)
                        equipWeapons.leftHand = weapon;
                    else
                        equipWeapons.rightHand = weapon;
                    return true;
                }
                // Not enough ammo
                return false;
            }
            else if (weaponItem.AmmoCapacity <= 0 && hasAmmoType)
            {
                // Ammo capacity is 0 so reduce ammo from inventory
                if (nonEquipItems.DecreaseAmmos(weaponItem.WeaponType.AmmoType, amount, out increaseDamages, null))
                {
                    nonEquipItems.FillEmptySlots(isLimitSlot, slotLimit);
                    return true;
                }
                // Not enough ammo
                return false;
            }
            else if (weaponItem.AmmoCapacity <= 0 && hasAmmoItems)
            {
                // Ammo capacity is 0 so reduce ammo from inventory
                BaseItem tempItemData;
                foreach (int tempAmmoDataId in weaponItem.AmmoItemIds)
                {
                    if (!GameInstance.Items.TryGetValue(tempAmmoDataId, out tempItemData))
                        continue;
                    if (nonEquipItems.DecreaseItems(tempAmmoDataId, amount, isLimitSlot))
                    {
                        nonEquipItems.FillEmptySlots(isLimitSlot, slotLimit);
                        increaseDamages = new Dictionary<DamageElement, MinMaxFloat>();
                        if (tempItemData is IAmmoItem tempAmmoItem)
                            tempAmmoItem.GetIncreaseDamages(increaseDamages);
                        return true;
                    }
                }
                // Not enough ammo
                return false;
            }

            return validIfNoRequireAmmoType;
        }

        public static bool DecreaseAmmos(this ICharacterData data, bool isLeftHand, int amount, out Dictionary<DamageElement, MinMaxFloat> increaseDamages, bool validIfNoRequireAmmoType = true, bool applyChanges = true)
        {
            CharacterItem weapon = isLeftHand ? data.EquipWeapons.leftHand : data.EquipWeapons.rightHand;
            if (data.CurrentHp <= 0)
            {
                increaseDamages = new Dictionary<DamageElement, MinMaxFloat>();
                return false;
            }
            IWeaponItem weaponItem = weapon.GetWeaponItem();
            if (weaponItem == null)
            {
                increaseDamages = new Dictionary<DamageElement, MinMaxFloat>();
                return true;
            }
            bool hasAmmoType = weaponItem.WeaponType.AmmoType != null;
            bool hasAmmoItems = weaponItem.AmmoItemIds.Count > 0;
            if (!hasAmmoType && !hasAmmoItems)
            {
                increaseDamages = new Dictionary<DamageElement, MinMaxFloat>();
                return true;
            }
            if (isLeftHand && data.EquipWeapons.leftHand.IsDiffer(weapon))
            {
                increaseDamages = new Dictionary<DamageElement, MinMaxFloat>();
                return false;
            }
            if (!isLeftHand && data.EquipWeapons.rightHand.IsDiffer(weapon))
            {
                increaseDamages = new Dictionary<DamageElement, MinMaxFloat>();
                return false;
            }
            EquipWeapons equipWeapons = data.EquipWeapons.Clone();
            IList<CharacterItem> nonEquipItems = applyChanges ? data.NonEquipItems : new List<CharacterItem>(data.NonEquipItems);
            if (!DecreaseAmmos(ref equipWeapons, nonEquipItems, GameInstance.Singleton.IsLimitInventorySlot, data.GetCaches().LimitItemSlot, weapon, isLeftHand, amount, out increaseDamages, validIfNoRequireAmmoType))
                return false;
            if (applyChanges)
                data.EquipWeapons = equipWeapons;
            return true;
        }
        #endregion

        public static void RemoveOrPlaceEmptySlot(this IList<CharacterItem> nonEquipItems, int index)
        {
            GameInstance.Singleton.InventoryManager.RemoveOrPlaceEmptySlot(nonEquipItems, index);
        }

        public static bool HasOneInNonEquipItems(this ICharacterData data, int dataId)
        {
            if (data != null && data.NonEquipItems.Count > 0)
            {
                IList<CharacterItem> nonEquipItems = data.NonEquipItems;
                foreach (CharacterItem nonEquipItem in nonEquipItems)
                {
                    if (nonEquipItem.dataId == dataId && nonEquipItem.amount > 0)
                        return true;
                }
            }
            return false;
        }

        public static int CountNonEquipItems(this ICharacterData data, int dataId)
        {
            int count = 0;
            if (data != null && data.NonEquipItems.Count > 0)
            {
                IList<CharacterItem> nonEquipItems = data.NonEquipItems;
                foreach (CharacterItem nonEquipItem in nonEquipItems)
                {
                    if (nonEquipItem.dataId == dataId)
                        count += nonEquipItem.amount;
                }
            }
            return count;
        }

        public static int CountAmmos(this ICharacterData data, AmmoType ammoType, out int dataId)
        {
            dataId = 0;
            if (ammoType == null)
                return 0;
            int count = 0;
            if (data != null && data.NonEquipItems.Count > 0)
            {
                CharacterItem tempNonEquipItem;
                IAmmoItem tempAmmoItemData;
                int i;
                for (i = 0; i < data.NonEquipItems.Count; ++i)
                {
                    tempNonEquipItem = data.NonEquipItems[i];
                    tempAmmoItemData = tempNonEquipItem.GetAmmoItem();
                    if (tempAmmoItemData == null)
                        continue;
                    if (tempAmmoItemData.AmmoType == null)
                        continue;
                    if (dataId != 0 && dataId != tempAmmoItemData.DataId)
                        continue;
                    if (ammoType == tempAmmoItemData.AmmoType)
                    {
                        dataId = tempAmmoItemData.DataId;
                        count += tempNonEquipItem.amount;
                    }
                }
            }
            return count;
        }

        public static int CountAllAmmos(this ICharacterData data, AmmoType ammoType)
        {
            if (data == null || ammoType == null)
                return 0;
            int count = 0;
            if (data.NonEquipItems.Count > 0)
            {
                CharacterItem tempNonEquipItem;
                IAmmoItem tempAmmoItemData;
                int i;
                for (i = 0; i < data.NonEquipItems.Count; ++i)
                {
                    tempNonEquipItem = data.NonEquipItems[i];
                    tempAmmoItemData = tempNonEquipItem.GetAmmoItem();
                    if (tempAmmoItemData == null)
                        continue;
                    if (tempAmmoItemData.AmmoType == null)
                        continue;
                    if (ammoType == tempAmmoItemData.AmmoType)
                        count += tempNonEquipItem.amount;
                }
            }
            return count;
        }

        public static int CountAllAmmos(this ICharacterData data, IWeaponItem weaponItem)
        {
            if (data == null || weaponItem == null)
                return 0;
            int count = 0;
            if (weaponItem.WeaponType != null)
                count += CountAllAmmos(data, weaponItem.WeaponType.AmmoType);
            if (weaponItem.AmmoItemIds.Count > 0 &&
                data.NonEquipItems.Count > 0)
            {
                CharacterItem tempNonEquipItem;
                IItem tempAmmoItemData;
                int i;
                for (i = 0; i < data.NonEquipItems.Count; ++i)
                {
                    tempNonEquipItem = data.NonEquipItems[i];
                    if (!weaponItem.AmmoItemIds.Contains(tempNonEquipItem.dataId))
                        continue;
                    tempAmmoItemData = tempNonEquipItem.GetItem();
                    if (tempAmmoItemData == null)
                        continue;
                    count += tempNonEquipItem.amount;
                }
            }
            return count;
        }

        public static void GetAvailableWeapon(
            this ICharacterData data, ref bool isLeftHand,
            out CharacterItem characterItem,
            out DamageInfo damageInfo)
        {
            CharacterDataCache cache = data.GetCaches();
            if (!isLeftHand && !cache.IsRightHandItemAvailable)
                isLeftHand = true;
            if (isLeftHand && !cache.IsLeftHandItemAvailable)
                isLeftHand = false;
            if (isLeftHand)
            {
                characterItem = cache.LeftHandItem;
                damageInfo = cache.LeftHandDamageInfo;
            }
            else
            {
                characterItem = cache.RightHandItem;
                damageInfo = cache.RightHandDamageInfo;
            }
        }

        public static CharacterItem GetAvailableWeapon(this ICharacterData data, ref bool isLeftHand)
        {
            CharacterDataCache cache = data.GetCaches();
            if (!isLeftHand && !cache.IsRightHandItemAvailable)
                isLeftHand = true;
            if (isLeftHand && !cache.IsLeftHandItemAvailable)
                isLeftHand = false;
            if (isLeftHand)
                return cache.LeftHandItem;
            else
                return cache.RightHandItem;
        }

        public static DamageInfo GetAvailableWeaponDamageInfo(this ICharacterData data, ref bool isLeftHand)
        {
            CharacterDataCache cache = data.GetCaches();
            if (!isLeftHand && !cache.IsRightHandItemAvailable)
                isLeftHand = true;
            if (isLeftHand && !cache.IsLeftHandItemAvailable)
                isLeftHand = false;
            if (isLeftHand)
                return cache.LeftHandDamageInfo;
            else
                return cache.RightHandDamageInfo;
        }

        public static float GetAttackDistance(this ICharacterData data, bool isLeftHand)
        {
            if (data == null)
                return 0f;
            return data.GetAvailableWeaponDamageInfo(ref isLeftHand).GetDistance();
        }

        public static float GetAttackFov(this ICharacterData data, bool isLeftHand)
        {
            if (data == null)
                return 0f;
            return data.GetAvailableWeaponDamageInfo(ref isLeftHand).GetDistance();
        }

        public static float GetMoveSpeedRateWhileEquipped(this ICharacterData data, IWeaponItem weaponItem)
        {
            if (data is BaseMonsterCharacterEntity)
                return 1f;
            if (weaponItem == null)
                return 1f;
            return weaponItem.MoveSpeedRateWhileEquipped;
        }

        public static float GetMoveSpeedRateWhileReloading(this ICharacterData data, IWeaponItem weaponItem)
        {
            if (data is BaseMonsterCharacterEntity)
                return 1f;
            if (weaponItem == null)
                return 1f;
            return weaponItem.MoveSpeedRateWhileReloading;
        }

        public static MovementRestriction GetMovementRestrictionWhileReloading(this ICharacterData data, IWeaponItem weaponItem)
        {
            if (data is BaseMonsterCharacterEntity)
                return MovementRestriction.None;
            if (weaponItem == null)
                return MovementRestriction.None;
            return weaponItem.MovementRestrictionWhileReloading;
        }

        public static float GetMoveSpeedRateWhileCharging(this ICharacterData data, IWeaponItem weaponItem)
        {
            if (data is BaseMonsterCharacterEntity)
                return 1f;
            if (weaponItem == null)
                return 1f;
            return weaponItem.MoveSpeedRateWhileCharging;
        }

        public static MovementRestriction GetMovementRestrictionWhileCharging(this ICharacterData data, IWeaponItem weaponItem)
        {
            if (data is BaseMonsterCharacterEntity)
                return MovementRestriction.None;
            if (weaponItem == null)
                return MovementRestriction.None;
            return weaponItem.MovementRestrictionWhileCharging;
        }

        public static float GetMoveSpeedRateWhileAttacking(this ICharacterData data, IWeaponItem weaponItem)
        {
            if (data is BaseMonsterCharacterEntity monsterCharacterEntity)
                return monsterCharacterEntity.CharacterDatabase.MoveSpeedRateWhileAttacking;
            if (weaponItem == null)
                return 1f;
            return weaponItem.MoveSpeedRateWhileAttacking;
        }

        public static MovementRestriction GetMovementRestrictionWhileAttacking(this ICharacterData data, IWeaponItem weaponItem)
        {
            if (data is BaseMonsterCharacterEntity)
                return MovementRestriction.None;
            if (weaponItem == null)
                return MovementRestriction.None;
            return weaponItem.MovementRestrictionWhileAttacking;
        }

        public static int IndexOfEquipItemByEquipPosition(this ICharacterData data, string equipPosition, byte equipSlotIndex)
        {
            if (string.IsNullOrEmpty(equipPosition))
                return -1;

            for (int i = 0; i < data.EquipItems.Count; ++i)
            {
                if (data.EquipItems[i].GetEquipmentItem() == null)
                    continue;

                if (data.EquipItems[i].equipSlotIndex == equipSlotIndex &&
                    equipPosition.Equals(data.EquipItems[i].GetArmorItem().GetEquipPosition()))
                    return i;
            }
            return -1;
        }

        public static bool HasEnoughAttributeAmounts(this ICharacterData data, Dictionary<Attribute, float> requiredAttributeAmounts, bool sumWithEquipments, out UITextKeys gameMessage, out Dictionary<Attribute, float> currentAttributeAmounts, float multiplier = 1, bool willReleaseAttributes = false)
        {
            gameMessage = UITextKeys.NONE;
            Dictionary<Attribute, float> tempAttributeAmounts = null;
            data.GetAllStats(sumWithEquipments, false, true,
                onGetAttributes: attributeAmounts => tempAttributeAmounts = attributeAmounts,
                willReleaseAttributes: false);
            currentAttributeAmounts = tempAttributeAmounts;
            foreach (Attribute requireAttribute in requiredAttributeAmounts.Keys)
            {
                if (!currentAttributeAmounts.ContainsKey(requireAttribute) ||
                    currentAttributeAmounts[requireAttribute] < Mathf.CeilToInt(requiredAttributeAmounts[requireAttribute] * multiplier))
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ATTRIBUTE_AMOUNTS;
                    if (willReleaseAttributes)
                        CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Release(currentAttributeAmounts);
                    return false;
                }
            }
            if (willReleaseAttributes)
                CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Release(currentAttributeAmounts);
            return true;
        }

        public static bool HasEnoughSkillLevels(this ICharacterData data, Dictionary<BaseSkill, int> requiredSkillLevels, bool sumWithEquipments, out UITextKeys gameMessage, out Dictionary<BaseSkill, int> currentSkillLevels, float multiplier = 1, bool willReleaseSkills = false)
        {
            gameMessage = UITextKeys.NONE;
            Dictionary<BaseSkill, int> tempSkillLevels = null;
            data.GetAllStats(sumWithEquipments, false, true,
                onGetSkills: skillLevels => tempSkillLevels = skillLevels,
                willReleaseSkills: false);
            currentSkillLevels = tempSkillLevels;
            foreach (BaseSkill requireSkill in requiredSkillLevels.Keys)
            {
                if (!currentSkillLevels.ContainsKey(requireSkill) ||
                    currentSkillLevels[requireSkill] < Mathf.CeilToInt(requiredSkillLevels[requireSkill] * multiplier))
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_SKILL_LEVELS;
                    if (willReleaseSkills)
                        CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Release(currentSkillLevels);
                    return false;
                }
            }
            if (willReleaseSkills)
                CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Release(currentSkillLevels);
            return true;
        }

        public static Dictionary<BaseItem, int> GetNonEquipItems(this ICharacterData data)
        {
            if (data == null)
                return new Dictionary<BaseItem, int>();
            Dictionary<BaseItem, int> result = new Dictionary<BaseItem, int>();
            foreach (CharacterItem characterItem in data.NonEquipItems)
            {
                BaseItem key = characterItem.GetItem();
                int value = characterItem.amount;
                if (key == null)
                    continue;
                if (!result.ContainsKey(key))
                    result[key] = value;
                else
                    result[key] += value;
            }

            return result;
        }

        public static bool HasEnoughNonEquipItemAmounts(this ICharacterData data, Dictionary<BaseItem, int> requiredItemAmounts, out UITextKeys gameMessage, out Dictionary<BaseItem, int> currentItemAmounts, float multiplier = 1)
        {
            gameMessage = UITextKeys.NONE;
            currentItemAmounts = data.GetNonEquipItems();
            foreach (BaseItem requireItem in requiredItemAmounts.Keys)
            {
                if (!currentItemAmounts.ContainsKey(requireItem) ||
                    currentItemAmounts[requireItem] < Mathf.CeilToInt(requiredItemAmounts[requireItem] * multiplier))
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                    return false;
                }
            }
            return true;
        }

        public static bool FindItemById(
            this ICharacterData data,
            string id)
        {
            return data.FindItemById(id, out _, out _, out _, out _);
        }

        public static bool FindItemById(
            this ICharacterData data,
            string id,
            out InventoryType inventoryType,
            out int itemIndex,
            out byte equipWeaponSet,
            out CharacterItem characterItem)
        {
            inventoryType = InventoryType.NonEquipItems;
            itemIndex = -1;
            equipWeaponSet = 0;
            characterItem = CharacterItem.Empty;

            EquipWeapons tempEquipWeapons;
            for (byte i = 0; i < data.SelectableWeaponSets.Count; ++i)
            {
                tempEquipWeapons = data.SelectableWeaponSets[i];
                if (!string.IsNullOrEmpty(tempEquipWeapons.rightHand.id) &&
                    tempEquipWeapons.rightHand.id.Equals(id))
                {
                    equipWeaponSet = i;
                    characterItem = tempEquipWeapons.rightHand;
                    inventoryType = InventoryType.EquipWeaponRight;
                    return true;
                }

                if (!string.IsNullOrEmpty(tempEquipWeapons.leftHand.id) &&
                    tempEquipWeapons.leftHand.id.Equals(id))
                {
                    equipWeaponSet = i;
                    characterItem = tempEquipWeapons.leftHand;
                    inventoryType = InventoryType.EquipWeaponLeft;
                    return true;
                }
            }

            itemIndex = data.IndexOfNonEquipItem(id);
            if (itemIndex >= 0)
            {
                characterItem = data.NonEquipItems[itemIndex];
                inventoryType = InventoryType.NonEquipItems;
                return true;
            }

            itemIndex = data.IndexOfEquipItem(id);
            if (itemIndex >= 0)
            {
                characterItem = data.EquipItems[itemIndex];
                inventoryType = InventoryType.EquipItems;
                return true;
            }

            return false;
        }

        public static bool IsEquipped(
            this ICharacterData data,
            string id,
            out InventoryType inventoryType,
            out int itemIndex,
            out byte equipWeaponSet,
            out CharacterItem characterItem)
        {
            if (data.FindItemById(id, out inventoryType, out itemIndex, out equipWeaponSet, out characterItem))
            {
                return inventoryType == InventoryType.EquipItems ||
                    inventoryType == InventoryType.EquipWeaponRight ||
                    inventoryType == InventoryType.EquipWeaponLeft;
            }
            return false;
        }

        public static void AddOrSetItems(this IList<CharacterItem> itemList, CharacterItem characterItem, out int index, int expectedIndex = -1)
        {
            GameInstance.Singleton.InventoryManager.AddOrSetItems(itemList, characterItem, out index, expectedIndex);
        }

        public static void AddOrSetItems(this IList<CharacterItem> itemList, CharacterItem characterItem, int expectedIndex = -1)
        {
            itemList.AddOrSetItems(characterItem, out _, expectedIndex);
        }

        public static void AddOrSetNonEquipItems(this ICharacterData data, CharacterItem characterItem, out int index, int expectedIndex = -1)
        {
            data.NonEquipItems.AddOrSetItems(characterItem, out index, expectedIndex);
        }

        public static void AddOrSetNonEquipItems(this ICharacterData data, CharacterItem characterItem, int expectedIndex = -1)
        {
            data.AddOrSetNonEquipItems(characterItem, out _, expectedIndex);
        }

        public static void MoveItemToEmptySlot(this IList<CharacterItem> itemList, int movingIndex)
        {
            GameInstance.Singleton.InventoryManager.MoveItemToEmptySlot(itemList, movingIndex);
        }

        public static int IndexOfAmmoItem(this ICharacterData data, AmmoType ammoType)
        {
            for (int i = 0; i < data.NonEquipItems.Count; ++i)
            {
                if (data.NonEquipItems[i].GetAmmoItem() != null && data.NonEquipItems[i].GetAmmoItem().AmmoType == ammoType)
                    return i;
            }
            return -1;
        }

        public static void ApplyStatusEffect(this IEnumerable<StatusEffectApplying> statusEffects, int level, EntityInfo applier, CharacterItem weapon, BaseCharacterEntity target)
        {
            if (level <= 0 || target == null || statusEffects == null)
                return;
            foreach (StatusEffectApplying effect in statusEffects)
            {
                if (effect.statusEffect == null)
                {
                    // Invalid status effect data
                    continue;
                }
                if (effect.statusEffect.RandomResistOccurs(target.GetCaches().GetStatusEffectResistance(effect.statusEffect.DataId), level))
                {
                    // Resisted, no status effect being applied
                    continue;
                }
                int buffLevel = effect.buffLevel.GetAmount(level);
                target.ApplyBuff(effect.statusEffect.DataId, BuffType.StatusEffect, buffLevel, applier, weapon);
                effect.statusEffect.OnApply(target, applier, weapon, level, buffLevel);
            }
        }

        public static List<RewardedItem> ToRewardedItems(this IEnumerable<CharacterItem> characterItems)
        {
            List<RewardedItem> result = new List<RewardedItem>();
            foreach (CharacterItem characterItem in characterItems)
            {
                result.Add(new RewardedItem()
                {
                    item = characterItem.GetItem(),
                    level = characterItem.level,
                    amount = characterItem.amount,
                    randomSeed = characterItem.randomSeed,
                });
            }
            return result;
        }

        public static List<ItemAmount> ToItemAmounts(this IEnumerable<CharacterItem> characterItems)
        {
            List<ItemAmount> result = new List<ItemAmount>();
            foreach (CharacterItem characterItem in characterItems)
            {
                result.Add(new ItemAmount()
                {
                    item = characterItem.GetItem(),
                    level = characterItem.level,
                    amount = characterItem.amount,
                });
            }
            return result;
        }

        public static List<CurrencyAmount> ToCurrencyAmounts(this IEnumerable<CharacterCurrency> characterCurrencies)
        {
            List<CurrencyAmount> result = new List<CurrencyAmount>();
            foreach (CharacterCurrency characterCurrency in characterCurrencies)
            {
                result.Add(new CurrencyAmount()
                {
                    currency = characterCurrency.GetCurrency(),
                    amount = characterCurrency.amount,
                });
            }
            return result;
        }

        public static Dictionary<int, int> ToAttributeAmountDictionary(this IEnumerable<CharacterAttribute> list)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            foreach (CharacterAttribute entry in list)
            {
                result[entry.dataId] = entry.amount;
            }
            return result;
        }

        public static List<CharacterAttribute> ToCharacterAttributes(this Dictionary<int, int> dict)
        {
            List<CharacterAttribute> result = new List<CharacterAttribute>();
            foreach (KeyValuePair<int, int> entry in dict)
            {
                result.Add(CharacterAttribute.Create(entry.Key, entry.Value));
            }
            return result;
        }

        public static Dictionary<int, int> ToSkillLevelDictionary(this IEnumerable<CharacterSkill> list)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            foreach (CharacterSkill entry in list)
            {
                result[entry.dataId] = entry.level;
            }
            return result;
        }

        public static List<CharacterSkill> ToCharacterSkills(this Dictionary<int, int> dict)
        {
            List<CharacterSkill> result = new List<CharacterSkill>();
            foreach (KeyValuePair<int, int> entry in dict)
            {
                result.Add(CharacterSkill.Create(entry.Key, entry.Value));
            }
            return result;
        }

        public static bool ValidateSkillToUse(this BaseCharacterEntity character, int dataId, bool isLeftHand, uint targetObjectId, out BaseSkill skill, out int skillLevel, out UITextKeys gameMessage)
        {
            skillLevel = 0;
            gameMessage = UITextKeys.NONE;

            if (!GameInstance.Skills.TryGetValue(dataId, out skill))
                return false;

            if (!character.GetCaches().Skills.TryGetValue(skill, out skillLevel) ||
                !skill.CanUse(character, skillLevel, isLeftHand, targetObjectId, out gameMessage))
                return false;

            return true;
        }

        public static bool ValidateSkillItemToUse(this BaseCharacterEntity character, int itemIndex, bool isLeftHand, uint targetObjectId, out ISkillItem skillItem, out BaseSkill skill, out int skillLevel, out UITextKeys gameMessage)
        {
            skillItem = null;
            skill = null;
            skillLevel = 0;

            if (!ValidateUsableItemToUse(character, itemIndex, out IUsableItem usableItem, out gameMessage))
            {
                return false;
            }

            skillItem = usableItem as ISkillItem;
            if (skillItem == null || skillItem.SkillData == null ||
                !skillItem.SkillData.CanUse(character, skillItem.SkillLevel, isLeftHand, targetObjectId, out gameMessage, true))
            {
                return false;
            }
            skill = skillItem.SkillData;
            skillLevel = skillItem.SkillLevel;

            return true;
        }

        public static bool ValidateUsableItemToUse(this BaseCharacterEntity character, int itemIndex, out IUsableItem usableItem, out UITextKeys gameMessage)
        {
            usableItem = null;

            if (itemIndex < 0 || itemIndex >= character.NonEquipItems.Count)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                return false;
            }

            if (character.NonEquipItems[itemIndex].IsLocked())
            {
                gameMessage = UITextKeys.UI_ERROR_ITEM_IS_LOCKED;
                return false;
            }

            usableItem = character.NonEquipItems[itemIndex].GetUsableItem();
            if (usableItem == null)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_DATA;
                return false;
            }

            if (character.IndexOfSkillUsage(SkillUsageType.UsableItem, character.NonEquipItems[itemIndex].dataId) >= 0)
            {
                gameMessage = UITextKeys.UI_ERROR_ITEM_IS_COOLING_DOWN;
                return false;
            }

            if (character.Level < usableItem.Requirement.level)
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_LEVEL;
                return false;
            }

            if (!usableItem.Requirement.ClassIsAvailable(character.DataId))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_MATCH_CHARACTER_CLASS;
                return false;
            }

            if (!character.HasEnoughAttributeAmounts(usableItem.RequireAttributeAmounts, true, out gameMessage, out _, willReleaseAttributes: true))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ATTRIBUTE_AMOUNTS;
                return false;
            }

            gameMessage = UITextKeys.NONE;
            return true;
        }

        public static bool ValidateAmmo(this ICharacterData data, CharacterItem weapon, int amount, bool validIfNoRequireAmmoType = true)
        {
            // Avoid null data
            if (weapon.IsEmptySlot())
                return validIfNoRequireAmmoType;

            IWeaponItem weaponItem = weapon.GetWeaponItem();
            bool hasAmmoType = weaponItem.WeaponType.AmmoType != null;
            bool hasAmmoItems = weaponItem.AmmoItemIds.Count > 0;
            if (weaponItem.AmmoCapacity > 0 && (hasAmmoType || hasAmmoItems))
            {
                // Ammo capacity more than 0 reduce loaded ammo
                if (weapon.ammo < amount)
                    return false;
            }
            else if (weaponItem.AmmoCapacity <= 0 && hasAmmoType)
            {
                // Ammo capacity is 0 so reduce ammo from inventory
                if (data.CountAmmos(weaponItem.WeaponType.AmmoType, out _) >= amount)
                    return true;
                return false;
            }
            else if (weaponItem.AmmoCapacity <= 0 && hasAmmoItems)
            {
                // Ammo capacity is 0 so reduce ammo from inventory
                foreach (int tempAmmoDataId in weaponItem.AmmoItemIds)
                {
                    if (data.CountNonEquipItems(tempAmmoDataId) >= amount)
                        return true;
                }
                return false;
            }

            return validIfNoRequireAmmoType;
        }

        public static List<Dictionary<DamageElement, MinMaxFloat>> PrepareDamageAmounts(this ICharacterData data, bool isLeftHand, Dictionary<DamageElement, MinMaxFloat> baseDamageAmounts, int triggerCount, int ammoAmountEachTrigger, bool validIfNoRequireAmmoType = true)
        {
            List<Dictionary<DamageElement, MinMaxFloat>> result = new List<Dictionary<DamageElement, MinMaxFloat>>();
            Dictionary<DamageElement, MinMaxFloat> tempCombinedDamageAmounts;
            Dictionary<DamageElement, MinMaxFloat> tempIncreaseDamageAmounts;
            for (int i = 0; i < triggerCount; ++i)
            {
                if (!DecreaseAmmos(data, isLeftHand, ammoAmountEachTrigger, out tempIncreaseDamageAmounts, validIfNoRequireAmmoType, false))
                    break;
                tempCombinedDamageAmounts = new Dictionary<DamageElement, MinMaxFloat>(baseDamageAmounts);
                GameDataHelpers.CombineDamages(tempCombinedDamageAmounts, tempIncreaseDamageAmounts);
                result.Add(tempCombinedDamageAmounts);
            }
            return result;
        }

        public static void AddOrUpdateSkillUsage(this BaseCharacterEntity character, SkillUsageType type, int dataId, int skillLevel)
        {
            int index = character.IndexOfSkillUsage(type, dataId);
            if (index >= 0)
            {
                CharacterSkillUsage newSkillUsage = character.SkillUsages[index];
                newSkillUsage.Use(character, skillLevel);
                character.SkillUsages[index] = newSkillUsage;
            }
            else
            {
                CharacterSkillUsage newSkillUsage = CharacterSkillUsage.Create(type, dataId);
                newSkillUsage.Use(character, skillLevel);
                character.SkillUsages.Add(newSkillUsage);
            }
        }

        public static bool IsDifferMount(this ICharacterData data, MountType mountType, string sourceId, int level)
        {
            if (data == null)
                return false;

            if (data.Mount.type == MountType.None && mountType == MountType.None)
                return false;

            if (data.Mount.type != mountType)
                return true;

            if (!string.Equals(data.Mount.sourceId, sourceId))
                return true;

            if (data.Mount.level != level)
                return true;

            return false;
        }
    }
}
