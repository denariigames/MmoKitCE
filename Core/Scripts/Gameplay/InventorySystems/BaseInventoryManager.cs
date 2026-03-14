using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseInventoryManager : ScriptableObject
    {
        public abstract void FillEmptySlots(IList<CharacterItem> itemList, bool isLimitSlot, int slotLimit);
        public abstract bool UnEquipItemWillOverwhelming(ICharacterData data, int unEquipCount);
        public abstract bool IncreasingItemsWillOverwhelming(IList<CharacterItem> itemList, int dataId, int amount, bool isLimitWeight, float weightLimit, float totalItemWeight, bool isLimitSlot, int slotLimit);
        public abstract bool IncreaseItems(IList<CharacterItem> itemList, CharacterItem increasingItem);
        public abstract bool DecreaseItems(IList<CharacterItem> itemList, int dataId, int amount, bool isLimitInventorySlot, out Dictionary<int, int> decreaseItems);
        public abstract bool DecreaseItemsByIndex(IList<CharacterItem> itemList, int index, int amount, bool isLimitInventorySlot, bool adjustMaxAmount);
        public abstract void RemoveOrPlaceEmptySlot(IList<CharacterItem> itemList, int index);
        public abstract void MoveItemToEmptySlot(IList<CharacterItem> itemList, int movingIndex);
        public abstract void AddOrSetItems(IList<CharacterItem> itemList, CharacterItem characterItem, out int index, int expectedIndex);
    }
}