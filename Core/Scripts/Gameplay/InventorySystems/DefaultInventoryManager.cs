using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultInventoryManager : BaseInventoryManager
    {
        public override void FillEmptySlots(IList<CharacterItem> itemList, bool isLimitSlot, int slotLimit)
        {
            int i;
            if (!isLimitSlot || GameInstance.Singleton.doNotFillEmptySlots)
            {
                // If it is not limit slots, don't fill it, and also remove empty slots
                for (i = itemList.Count - 1; i >= 0; --i)
                {
                    if (itemList[i].IsEmpty() || itemList[i].IsEmptySlot())
                        itemList.RemoveAt(i);
                }
                return;
            }

            // Place empty slots
            for (i = 0; i < itemList.Count; ++i)
            {
                if (itemList[i].IsEmpty())
                    itemList[i] = CharacterItem.CreateEmptySlot();
            }

            // Fill empty slots
            for (i = itemList.Count; i < slotLimit; ++i)
            {
                itemList.Add(CharacterItem.CreateEmptySlot());
            }

            // Remove empty slots if it's over limit
            for (i = itemList.Count - 1; itemList.Count > slotLimit && i >= 0; --i)
            {
                if (itemList[i].IsEmptySlot())
                    itemList.RemoveAt(i);
            }
        }

        public override bool UnEquipItemWillOverwhelming(ICharacterData data, int unEquipCount)
        {
            if (!GameInstance.Singleton.IsLimitInventorySlot)
                return false;
            return data.GetCaches().TotalItemSlot + unEquipCount > data.GetCaches().LimitItemSlot;
        }

        public override bool IncreasingItemsWillOverwhelming(IList<CharacterItem> itemList, int dataId, int amount, bool isLimitWeight, float weightLimit, float totalItemWeight, bool isLimitSlot, int slotLimit)
        {
            BaseItem itemData;
            if (amount <= 0 || !GameInstance.Items.TryGetValue(dataId, out itemData))
            {
                // If item not valid
                return false;
            }

            if (isLimitWeight && totalItemWeight > weightLimit)
            {
                // If overwhelming
                return true;
            }

            if (!isLimitSlot)
            {
                // If not limit slot then don't checking for slot amount
                return false;
            }

            int maxStack = itemData.MaxStack;
            int slotCount = 0;
            // Loop to all slots to add amount to any slots that item amount not max in stack
            CharacterItem tempItem;
            for (int i = 0; i < itemList.Count; ++i)
            {
                tempItem = itemList[i];
                if (tempItem.IsEmptySlot())
                {
                    // If current entry is not valid, assume that it is empty slot, so reduce amount of adding item here
                    if (amount <= maxStack)
                    {
                        // Can add all items, so assume that it is not overwhelming 
                        return false;
                    }
                    else
                        amount -= maxStack;
                }
                else
                {
                    if (!tempItem.GetItem().NoSlotUsage)
                        slotCount++;
                    if (tempItem.dataId == itemData.DataId)
                    {
                        // If same item id, increase its amount
                        if (tempItem.amount + amount <= maxStack)
                        {
                            // Can add all items, so assume that it is not overwhelming 
                            return false;
                        }
                        else if (maxStack - tempItem.amount >= 0)
                            amount -= maxStack - tempItem.amount;
                    }
                }
            }

            bool noSlotUsage = itemData.NoSlotUsage;
            // Count adding slot here
            while (amount > 0)
            {
                if (!noSlotUsage)
                {
                    if (slotCount + 1 > slotLimit)
                    {
                        // If adding slot is more than slot limit, assume that it is overwhelming 
                        return true;
                    }
                    ++slotCount;
                }
                if (amount <= maxStack)
                {
                    // Can add all items, so assume that it is not overwhelming 
                    return false;
                }
                else
                    amount -= maxStack;
            }

            return true;
        }

        public override bool IncreaseItems(IList<CharacterItem> itemList, CharacterItem increasingItem)
        {
            // If item not valid
            if (increasingItem.IsEmptySlot())
                return false;

            BaseItem itemData = increasingItem.GetItem();
            if (GameInstance.Singleton.IsRepresentItem(itemData))
                return false;

            int amount = increasingItem.amount;

            int maxStack = itemData.MaxStack;
            Dictionary<int, CharacterItem> emptySlots = new Dictionary<int, CharacterItem>();
            Dictionary<int, CharacterItem> changes = new Dictionary<int, CharacterItem>();
            // Loop to all slots to add amount to any slots that item amount not max in stack
            CharacterItem item;
            for (int i = 0; i < itemList.Count; ++i)
            {
                item = itemList[i];
                if (item.IsEmptySlot())
                {
                    // If current entry is not valid, add it to empty list, going to replacing it later
                    emptySlots[i] = item;
                }
                else if (item.dataId == increasingItem.dataId && item.level == increasingItem.level)
                {
                    // If same item id, increase its amount
                    if (item.amount + amount <= maxStack)
                    {
                        item.amount += amount;
                        changes[i] = item;
                        amount = 0;
                        break;
                    }
                    else if (maxStack - item.amount >= 0)
                    {
                        amount -= maxStack - item.amount;
                        item.amount = maxStack;
                        changes[i] = item;
                    }
                }
            }

            // Adding item to new slots or empty slots if needed
            CharacterItem tempNewItem;
            if (changes.Count == 0 && emptySlots.Count > 0)
            {
                // If there are no changes and there are an empty entries, fill them
                foreach (int emptySlotIndex in emptySlots.Keys)
                {
                    tempNewItem = increasingItem.Clone(true);
                    int addAmount = 0;
                    if (amount - maxStack >= 0)
                    {
                        addAmount = maxStack;
                        amount -= maxStack;
                    }
                    else
                    {
                        addAmount = amount;
                        amount = 0;
                    }
                    tempNewItem.amount = addAmount;
                    changes[emptySlotIndex] = tempNewItem;
                    if (amount == 0)
                        break;
                }
            }

            // Apply all changes
            foreach (KeyValuePair<int, CharacterItem> change in changes)
            {
                itemList[change.Key] = change.Value;
            }

            // Add new items to new slots
            while (amount > 0)
            {
                tempNewItem = increasingItem.Clone(true);
                int addAmount;
                if (amount - maxStack >= 0)
                {
                    addAmount = maxStack;
                    amount -= maxStack;
                }
                else
                {
                    addAmount = amount;
                    amount = 0;
                }
                tempNewItem.amount = addAmount;
                itemList.AddOrSetItems(tempNewItem);
                if (amount == 0)
                    break;
            }
            return true;
        }

        public override bool DecreaseItems(IList<CharacterItem> itemList, int dataId, int amount, bool isLimitInventorySlot, out Dictionary<int, int> decreaseItems)
        {
            decreaseItems = new Dictionary<int, int>();
            Dictionary<int, int> decreasingItemIndexes = new Dictionary<int, int>();
            int tempDecresingAmount;
            CharacterItem tempItem;
            for (int i = 0; i < itemList.Count; ++i)
            {
                tempItem = itemList[i];
                if (tempItem.dataId == dataId)
                {
                    if (amount - tempItem.amount > 0)
                        tempDecresingAmount = tempItem.amount;
                    else
                        tempDecresingAmount = amount;
                    amount -= tempDecresingAmount;
                    decreasingItemIndexes[i] = tempDecresingAmount;
                }
                if (amount == 0)
                    break;
            }
            if (amount > 0)
                return false;
            foreach (KeyValuePair<int, int> decreasingItem in decreasingItemIndexes)
            {
                decreaseItems.Add(decreasingItem.Key, decreasingItem.Value);
                itemList.DecreaseItemsByIndex(decreasingItem.Key, decreasingItem.Value, isLimitInventorySlot, true);
            }
            return true;
        }

        public override bool DecreaseItemsByIndex(IList<CharacterItem> itemList, int index, int amount, bool isLimitInventorySlot, bool adjustMaxAmount)
        {
            if (index < 0 || index >= itemList.Count)
                return false;
            CharacterItem item = itemList[index];
            if (item.IsEmptySlot())
                return false;
            if (amount > item.amount)
            {
                if (!adjustMaxAmount)
                    return false;
                amount = item.amount;
            }
            if (item.amount - amount == 0)
            {
                if (isLimitInventorySlot)
                    itemList[index] = CharacterItem.Empty;
                else
                    itemList.RemoveAt(index);
            }
            else
            {
                item.amount -= amount;
                itemList[index] = item;
            }
            return true;
        }

        public override void RemoveOrPlaceEmptySlot(IList<CharacterItem> itemList, int index)
        {
            if (GameInstance.Singleton.IsLimitInventorySlot)
                itemList[index] = CharacterItem.Empty;
            else
                itemList.RemoveAt(index);
        }

        public override void MoveItemToEmptySlot(IList<CharacterItem> itemList, int movingIndex)
        {
            itemList[itemList.IndexOfEmptyItemSlot()] = itemList[movingIndex];
            itemList[movingIndex] = CharacterItem.Empty;
        }

        public override void AddOrSetItems(IList<CharacterItem> itemList, CharacterItem characterItem, out int index, int expectedIndex)
        {
            index = expectedIndex;
            if (index < 0 || index >= itemList.Count || !itemList[index].IsEmptySlot())
                index = itemList.IndexOfEmptyItemSlot();
            if (index >= 0)
            {
                // Insert to empty slot
                itemList[index] = characterItem;
            }
            else
            {
                // Add to last index
                itemList.Add(characterItem);
                index = itemList.Count - 1;
            }
        }
    }
}
