using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static partial class CharacterInventoryExtensions
    {
        public static bool MoveItemFromStorage(
            this IPlayerCharacterData playerCharacter,
            StorageId storageId,
            bool storageIsLimitWeight,
            float storageWeightLimit,
            bool storageIsLimitSlot,
            int storageSlotLimit,
            IList<CharacterItem> storageItems,
            int storageItemIndex,
            int storageItemAmount,
            InventoryType inventoryType,
            int inventoryItemIndex,
            byte equipSlotIndexOrWeaponSet,
            out UITextKeys gameMessage)
        {
            // Prepare item data
            switch (inventoryType)
            {
                case InventoryType.EquipWeaponLeft:
                case InventoryType.EquipWeaponRight:
                case InventoryType.EquipItems:
                    if (!playerCharacter.SwapStorageItemWithEquipmentItem(storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, equipSlotIndexOrWeaponSet, out gameMessage))
                    {
                        GameInstance.ServerLogHandlers.LogMoveItemFromStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, storageItemAmount, inventoryType, inventoryItemIndex, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    break;
                default:
                    if (storageItems[storageItemIndex].IsEmptySlot())
                    {
                        gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                        GameInstance.ServerLogHandlers.LogMoveItemFromStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, storageItemAmount, inventoryType, inventoryItemIndex, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    if (storageItems[storageItemIndex].amount < storageItemAmount)
                    {
                        gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                        GameInstance.ServerLogHandlers.LogMoveItemFromStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, storageItemAmount, inventoryType, inventoryItemIndex, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    CharacterItem movingItem = storageItems[storageItemIndex];
                    movingItem = movingItem.Clone(true);
                    movingItem.amount = storageItemAmount;
                    if (inventoryItemIndex < 0 || inventoryItemIndex >= playerCharacter.NonEquipItems.Count ||
                        playerCharacter.NonEquipItems[inventoryItemIndex].dataId == movingItem.dataId)
                    {
                        // Add to inventory or merge
                        bool isOverwhelming = playerCharacter.IncreasingItemsWillOverwhelming(movingItem.dataId, movingItem.amount);
                        if (isOverwhelming || !playerCharacter.IncreaseItems(movingItem))
                        {
                            gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                            GameInstance.ServerLogHandlers.LogMoveItemFromStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, storageItemAmount, inventoryType, inventoryItemIndex, equipSlotIndexOrWeaponSet, false, gameMessage);
                            return false;
                        }
                        // Remove from storage
                        storageItems.DecreaseItemsByIndex(storageItemIndex, storageItemAmount, storageIsLimitSlot, true);
                    }
                    else
                    {
                        // Already check for the storage index, so don't do it again
                        if (playerCharacter.NonEquipItems[inventoryItemIndex].IsEmptySlot())
                        {
                            // Replace empty slot
                            playerCharacter.NonEquipItems[inventoryItemIndex] = movingItem;
                            // Remove from storage
                            storageItems.DecreaseItemsByIndex(storageItemIndex, storageItemAmount, storageIsLimitSlot, true);
                        }
                        else
                        {
                            // Swapping
                            CharacterItem nonEquipItem = playerCharacter.NonEquipItems[inventoryItemIndex];

                            // Prevent item dealing by using storage
                            if (storageId.storageType != StorageType.Player)
                            {
                                if (nonEquipItem.GetItem().RestrictDealing)
                                {
                                    gameMessage = UITextKeys.UI_ERROR_ITEM_DEALING_RESTRICTED;
                                    GameInstance.ServerLogHandlers.LogMoveItemFromStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, storageItemAmount, inventoryType, inventoryItemIndex, equipSlotIndexOrWeaponSet, false, gameMessage);
                                    return false;
                                }
                            }

                            CharacterItem storageItem = storageItems[storageItemIndex].Clone(true);
                            nonEquipItem = nonEquipItem.Clone(true);
                            storageItems[storageItemIndex] = nonEquipItem;
                            playerCharacter.NonEquipItems[inventoryItemIndex] = storageItem;
                        }
                    }
                    break;
            }
            storageItems.FillEmptySlots(storageIsLimitSlot, storageSlotLimit);
            playerCharacter.FillEmptySlots();
            gameMessage = UITextKeys.NONE;
            GameInstance.ServerLogHandlers.LogMoveItemFromStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, storageItemAmount, inventoryType, inventoryItemIndex, equipSlotIndexOrWeaponSet, true, gameMessage);
            return true;
        }

        public static bool MoveItemToStorage(
            this IPlayerCharacterData playerCharacter,
            StorageId storageId,
            bool storageIsLimitWeight,
            float storageWeightLimit,
            bool storageIsLimitSlot,
            int storageSlotLimit,
            IList<CharacterItem> storageItems,
            int storageItemIndex,
            InventoryType inventoryType,
            int inventoryItemIndex,
            int inventoryItemAmount,
            byte equipSlotIndexOrWeaponSet,
            out UITextKeys gameMessage)
        {
            // Get and validate inventory item
            if (equipSlotIndexOrWeaponSet < 0 || equipSlotIndexOrWeaponSet >= GameInstance.Singleton.maxEquipWeaponSet)
                equipSlotIndexOrWeaponSet = playerCharacter.EquipWeaponSet;
            playerCharacter.FillWeaponSetsIfNeeded(equipSlotIndexOrWeaponSet);
            EquipWeapons equipWeapons = playerCharacter.SelectableWeaponSets[equipSlotIndexOrWeaponSet];
            CharacterItem movingItem;

            // Validate item index
            switch (inventoryType)
            {
                case InventoryType.EquipWeaponLeft:
                    if (equipWeapons.leftHand.IsEmptySlot())
                    {
                        gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                        GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    movingItem = equipWeapons.leftHand.Clone(true);
                    break;
                case InventoryType.EquipWeaponRight:
                    if (equipWeapons.rightHand.IsEmptySlot())
                    {
                        gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                        GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    movingItem = equipWeapons.rightHand.Clone(true);
                    break;
                case InventoryType.EquipItems:
                    if (inventoryItemIndex < 0 || inventoryItemIndex >= playerCharacter.EquipItems.Count)
                    {
                        gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                        GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    if (playerCharacter.EquipItems[inventoryItemIndex].IsEmptySlot())
                    {
                        gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                        GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    movingItem = playerCharacter.EquipItems[inventoryItemIndex].Clone(true);
                    break;
                default:
                    if (inventoryItemIndex < 0 || inventoryItemIndex >= playerCharacter.NonEquipItems.Count)
                    {
                        gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                        GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    if (playerCharacter.NonEquipItems[inventoryItemIndex].IsEmptySlot())
                    {
                        gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                        GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    if (playerCharacter.NonEquipItems[inventoryItemIndex].amount < inventoryItemAmount)
                    {
                        gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                        GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                        return false;
                    }
                    movingItem = playerCharacter.NonEquipItems[inventoryItemIndex].Clone(true);
                    movingItem.amount = inventoryItemAmount;
                    break;
            }

            // Prevent item dealing by using storage
            if (storageId.storageType != StorageType.Player)
            {
                if (movingItem.GetItem().RestrictDealing)
                {
                    gameMessage = UITextKeys.UI_ERROR_ITEM_DEALING_RESTRICTED;
                    GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                    return false;
                }
            }

            switch (inventoryType)
            {
                case InventoryType.EquipWeaponLeft:
                case InventoryType.EquipWeaponRight:
                case InventoryType.EquipItems:
                    if (storageItemIndex < 0 || storageItemIndex >= storageItems.Count ||
                        storageItems[storageItemIndex].IsEmptySlot())
                    {
                        // Add to storage
                        bool isOverwhelming = storageItems.IncreasingItemsWillOverwhelming(
                            movingItem.dataId, movingItem.amount, storageIsLimitWeight, storageWeightLimit,
                            storageItems.GetTotalItemWeight(), storageIsLimitSlot, storageSlotLimit);
                        if (isOverwhelming || !storageItems.IncreaseItems(movingItem))
                        {
                            gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                            GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                            return false;
                        }
                        // Remove from inventory
                        switch (inventoryType)
                        {
                            case InventoryType.EquipWeaponLeft:
                                equipWeapons.leftHand = CharacterItem.Empty;
                                playerCharacter.SelectableWeaponSets[equipSlotIndexOrWeaponSet] = equipWeapons;
                                break;
                            case InventoryType.EquipWeaponRight:
                                equipWeapons.rightHand = CharacterItem.Empty;
                                playerCharacter.SelectableWeaponSets[equipSlotIndexOrWeaponSet] = equipWeapons;
                                break;
                            case InventoryType.EquipItems:
                                playerCharacter.EquipItems.RemoveAt(inventoryItemIndex);
                                break;
                        }
                    }
                    else
                    {
                        // Swapping
                        if (!playerCharacter.SwapStorageItemWithEquipmentItem(storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, equipSlotIndexOrWeaponSet, out gameMessage))
                        {
                            GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                            return false;
                        }
                    }
                    break;
                default:
                    if (storageItemIndex < 0 || storageItemIndex >= storageItems.Count ||
                        storageItems[storageItemIndex].dataId == movingItem.dataId)
                    {
                        // Add to storage or merge
                        bool isOverwhelming = storageItems.IncreasingItemsWillOverwhelming(
                            movingItem.dataId, movingItem.amount, storageIsLimitWeight, storageWeightLimit,
                            storageItems.GetTotalItemWeight(), storageIsLimitSlot, storageSlotLimit);
                        if (isOverwhelming || !storageItems.IncreaseItems(movingItem))
                        {
                            gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                            GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, false, gameMessage);
                            return false;
                        }
                        // Remove from inventory
                        playerCharacter.DecreaseItemsByIndex(inventoryItemIndex, inventoryItemAmount, true);
                    }
                    else
                    {
                        // Already check for the storage index, so don't do it again
                        if (storageItems[storageItemIndex].IsEmptySlot())
                        {
                            // Replace empty slot
                            storageItems[storageItemIndex] = movingItem;
                            // Remove from inventory
                            playerCharacter.DecreaseItemsByIndex(inventoryItemIndex, inventoryItemAmount, true);
                        }
                        else
                        {
                            // Swapping
                            CharacterItem storageItem = storageItems[storageItemIndex].Clone(true);
                            CharacterItem nonEquipItem = playerCharacter.NonEquipItems[inventoryItemIndex].Clone(true);
                            storageItems[storageItemIndex] = nonEquipItem;
                            playerCharacter.NonEquipItems[inventoryItemIndex] = storageItem;
                        }
                    }
                    break;
            }
            storageItems.FillEmptySlots(storageIsLimitSlot, storageSlotLimit);
            playerCharacter.FillEmptySlots();
            gameMessage = UITextKeys.NONE;
            GameInstance.ServerLogHandlers.LogMoveItemToStorage(playerCharacter, storageId, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit, storageItems, storageItemIndex, inventoryType, inventoryItemIndex, inventoryItemAmount, equipSlotIndexOrWeaponSet, true, gameMessage);
            return true;
        }

        public static bool SwapStorageItemWithEquipmentItem(
            this IPlayerCharacterData playerCharacter,
            StorageId storageId,
            bool storageIsLimitWeight,
            float storageWeightLimit,
            bool storageIsLimitSlot,
            int storageSlotLimit,
            IList<CharacterItem> storageItems,
            int storageItemIndex,
            InventoryType inventoryType,
            byte equipSlotIndexOrWeaponSet,
            out UITextKeys gameMessage)
        {
            if (storageItemIndex >= storageItems.Count)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                return false;
            }
            CharacterItem storageItem = storageItems[storageItemIndex].Clone(true);
            if (storageItem.IsEmptySlot())
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                return false;
            }
            int storageItemAmount = storageItem.amount;
            // Can equip only one item, not stackable
            storageItem.amount = 1;
            // Prepare variables that being used in switch scope
            if (equipSlotIndexOrWeaponSet < 0 || equipSlotIndexOrWeaponSet >= GameInstance.Singleton.maxEquipWeaponSet)
                equipSlotIndexOrWeaponSet = playerCharacter.EquipWeaponSet;
            playerCharacter.FillWeaponSetsIfNeeded(equipSlotIndexOrWeaponSet);
            EquipWeapons equipWeapons = playerCharacter.SelectableWeaponSets[equipSlotIndexOrWeaponSet];
            bool shouldUnequipRightHand;
            bool shouldUnequipLeftHand;
            int unequippingIndex;
            switch (inventoryType)
            {
                case InventoryType.EquipWeaponLeft:
                    if (!playerCharacter.CanEquipWeapon(storageItem, equipSlotIndexOrWeaponSet, true, out gameMessage, out shouldUnequipRightHand, out shouldUnequipLeftHand))
                        return false;
                    // Validate unequipping right-hand item only, for the left one it will be swapped with storage item
                    if (shouldUnequipRightHand)
                    {
                        if (!playerCharacter.UnEquipWeapon(equipSlotIndexOrWeaponSet, false, false, out gameMessage, out _))
                            return false;
                    }
                    // Just equip or swapping
                    if (equipWeapons.IsEmptyLeftHandSlot())
                    {
                        // Just equip
                        equipWeapons.leftHand = storageItem;
                        playerCharacter.SelectableWeaponSets[equipSlotIndexOrWeaponSet] = equipWeapons;
                        // Remove from storage
                        storageItems.DecreaseItemsByIndex(storageItemIndex, storageItem.amount, storageIsLimitSlot, true);
                    }
                    else
                    {
                        // Swapping
                        CharacterItem equipItem = equipWeapons.leftHand;

                        // Prevent item dealing by using storage
                        if (storageId.storageType != StorageType.Player)
                        {
                            if (equipItem.GetItem().RestrictDealing)
                            {
                                gameMessage = UITextKeys.UI_ERROR_ITEM_DEALING_RESTRICTED;
                                return false;
                            }
                        }

                        if (storageItemAmount > 1)
                        {
                            if (!storageItems.AbleToDecreaseThenIncreaseItem(storageItem.dataId, storageItem.amount, equipItem.dataId, equipItem.amount, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit))
                                return false;
                            equipItem.equipSlotIndex = 0;
                            storageItems.DecreaseItemsByIndex(storageItemIndex, storageItem.amount, storageIsLimitSlot, true);
                            storageItems.IncreaseItems(equipItem);
                            storageItems.FillEmptySlots(storageIsLimitSlot, storageSlotLimit);
                        }
                        else
                        {
                            // Just swap items
                            equipItem.equipSlotIndex = 0;
                            storageItems[storageItemIndex] = equipItem;
                        }
                        equipWeapons.leftHand = storageItem;
                        playerCharacter.SelectableWeaponSets[equipSlotIndexOrWeaponSet] = equipWeapons;
                    }
                    return true;
                case InventoryType.EquipWeaponRight:
                    if (!playerCharacter.CanEquipWeapon(storageItem, equipSlotIndexOrWeaponSet, false, out gameMessage, out shouldUnequipRightHand, out shouldUnequipLeftHand))
                        return false;
                    // Validate unequipping left-hand item only, for the right one it will be swapped with storage item
                    if (shouldUnequipLeftHand)
                    {
                        if (!playerCharacter.UnEquipWeapon(equipSlotIndexOrWeaponSet, true, false, out gameMessage, out _))
                            return false;
                    }
                    // Just equip or swapping
                    if (equipWeapons.IsEmptyRightHandSlot())
                    {
                        // Just equip
                        equipWeapons.rightHand = storageItem;
                        playerCharacter.SelectableWeaponSets[equipSlotIndexOrWeaponSet] = equipWeapons;
                        // Remove from storage
                        storageItems.DecreaseItemsByIndex(storageItemIndex, storageItem.amount, storageIsLimitSlot, true);
                    }
                    else
                    {
                        // Swapping
                        CharacterItem equipItem = equipWeapons.rightHand;

                        // Prevent item dealing by using storage
                        if (storageId.storageType != StorageType.Player)
                        {
                            if (equipItem.GetItem().RestrictDealing)
                            {
                                gameMessage = UITextKeys.UI_ERROR_ITEM_DEALING_RESTRICTED;
                                return false;
                            }
                        }

                        if (storageItemAmount > 1)
                        {
                            if (!storageItems.AbleToDecreaseThenIncreaseItem(storageItem.dataId, storageItem.amount, equipItem.dataId, equipItem.amount, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit))
                                return false;
                            equipItem.equipSlotIndex = 0;
                            storageItems.DecreaseItemsByIndex(storageItemIndex, storageItem.amount, storageIsLimitSlot, true);
                            storageItems.IncreaseItems(equipItem);
                            storageItems.FillEmptySlots(storageIsLimitSlot, storageSlotLimit);
                        }
                        else
                        {
                            // Just swap items
                            equipItem.equipSlotIndex = 0;
                            storageItems[storageItemIndex] = equipItem;
                        }
                        equipWeapons.rightHand = storageItem;
                        playerCharacter.SelectableWeaponSets[equipSlotIndexOrWeaponSet] = equipWeapons;
                    }
                    return true;
                case InventoryType.EquipItems:
                    if (!playerCharacter.CanEquipItem(storageItem, equipSlotIndexOrWeaponSet, out gameMessage, out unequippingIndex))
                        return false;
                    // Just equip or swapping
                    if (unequippingIndex < 0)
                    {
                        // Just equip
                        storageItem.equipSlotIndex = equipSlotIndexOrWeaponSet;
                        playerCharacter.EquipItems.Add(storageItem);
                        // Remove from storage
                        storageItems.DecreaseItemsByIndex(storageItemIndex, storageItem.amount, storageIsLimitSlot, true);
                    }
                    else
                    {
                        // Swapping
                        CharacterItem equipItem = playerCharacter.EquipItems[unequippingIndex].Clone(true);

                        // Prevent item dealing by using storage
                        if (storageId.storageType != StorageType.Player)
                        {
                            if (equipItem.GetItem().RestrictDealing)
                            {
                                gameMessage = UITextKeys.UI_ERROR_ITEM_DEALING_RESTRICTED;
                                return false;
                            }
                        }

                        if (storageItemAmount > 1)
                        {
                            if (!storageItems.AbleToDecreaseThenIncreaseItem(storageItem.dataId, storageItem.amount, equipItem.dataId, equipItem.amount, storageIsLimitWeight, storageWeightLimit, storageIsLimitSlot, storageSlotLimit))
                                return false;
                            equipItem.equipSlotIndex = 0;
                            storageItems.DecreaseItemsByIndex(storageItemIndex, storageItem.amount, storageIsLimitSlot, true);
                            storageItems.IncreaseItems(equipItem);
                            storageItems.FillEmptySlots(storageIsLimitSlot, storageSlotLimit);
                        }
                        else
                        {
                            // Just swap items
                            equipItem.equipSlotIndex = 0;
                            storageItems[storageItemIndex] = equipItem;
                        }
                        storageItem.equipSlotIndex = equipSlotIndexOrWeaponSet;
                        playerCharacter.EquipItems[unequippingIndex] = storageItem;
                    }
                    return true;
                default:
                    gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_DATA;
                    return false;
            }
        }

        public static bool SwapOrMergeStorageItem(
            this IPlayerCharacterData playerCharacter,
            StorageId storageId,
            bool storageIsLimitSlot,
            int storageSlotLimit,
            IList<CharacterItem> storageItems,
            int fromIndex,
            int toIndex,
            out UITextKeys gameMessage)
        {
            CharacterItem fromItem = storageItems[fromIndex];
            CharacterItem toItem = storageItems[toIndex];
            if (fromIndex < 0 || fromIndex >= storageItems.Count ||
                toIndex < 0 || toIndex >= storageItems.Count)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                GameInstance.ServerLogHandlers.LogSwapOrMergeStorageItem(playerCharacter, storageId, storageIsLimitSlot, storageSlotLimit, storageItems, fromIndex, toIndex, false, gameMessage);
                return false;
            }

            if (fromItem.dataId == toItem.dataId && !fromItem.IsFull() && !toItem.IsFull() && fromItem.level == toItem.level)
            {
                // Merge if same id and not full
                int maxStack = toItem.GetMaxStack();
                if (toItem.amount + fromItem.amount <= maxStack)
                {
                    toItem.amount += fromItem.amount;
                    storageItems[fromIndex] = CharacterItem.Empty;
                    storageItems[toIndex] = toItem;
                }
                else
                {
                    int remains = toItem.amount + fromItem.amount - maxStack;
                    toItem.amount = maxStack;
                    fromItem.amount = remains;
                    storageItems[fromIndex] = fromItem;
                    storageItems[toIndex] = toItem;
                }
            }
            else
            {
                // Swap
                storageItems[fromIndex] = toItem;
                storageItems[toIndex] = fromItem;
            }
            storageItems.FillEmptySlots(storageIsLimitSlot, storageSlotLimit);
            gameMessage = UITextKeys.NONE;
            GameInstance.ServerLogHandlers.LogSwapOrMergeStorageItem(playerCharacter, storageId, storageIsLimitSlot, storageSlotLimit, storageItems, fromIndex, toIndex, true, gameMessage);
            return true;
        }

        public static bool CanEquipWeapon(this ICharacterData character, CharacterItem equippingItem, byte equipWeaponSet, bool isLeftHand, out UITextKeys gameMessage, out bool shouldUnequipRightHand, out bool shouldUnequipLeftHand)
        {
            shouldUnequipRightHand = false;
            shouldUnequipLeftHand = false;

            if (equippingItem.GetWeaponItem() == null && equippingItem.GetShieldItem() == null)
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_EQUIP;
                return false;
            }

            if (equipWeaponSet >= GameInstance.Singleton.maxEquipWeaponSet)
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_EQUIP;
                return false;
            }

            if (!equippingItem.GetEquipmentItem().CanEquip(character, equippingItem.level, out gameMessage))
                return false;

            character.FillWeaponSetsIfNeeded(equipWeaponSet);
            EquipWeapons tempEquipWeapons = character.SelectableWeaponSets[equipWeaponSet];

            WeaponItemEquipType rightHandEquipType;
            bool hasRightHandItem =
                tempEquipWeapons.GetRightHandWeaponItem().TryGetWeaponItemEquipType(out rightHandEquipType);
            WeaponItemEquipType leftHandEquipType;
            bool hasLeftHandItem =
                tempEquipWeapons.GetLeftHandWeaponItem().TryGetWeaponItemEquipType(out leftHandEquipType) ||
                tempEquipWeapons.GetLeftHandShieldItem() != null;

            // Equipping item is weapon
            IWeaponItem equippingWeaponItem = equippingItem.GetWeaponItem();
            if (equippingWeaponItem != null)
            {
                List<byte> equippableSlotIndexes = equippingWeaponItem.GetEquippableSetIndexes();
                if (equippableSlotIndexes?.Count > 0 && !equippableSlotIndexes.Contains(equipWeaponSet))
                {
                    gameMessage = UITextKeys.UI_ERROR_CANNOT_EQUIP;
                    return false;
                }

                switch (equippingWeaponItem.GetEquipType())
                {
                    case WeaponItemEquipType.MainHandOnly:
                        // If weapon is main-hand only its equip position must be right hand
                        if (isLeftHand)
                        {
                            gameMessage = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_RIGHT_HAND;
                            return false;
                        }
                        // One hand can equip with shield only 
                        // if there are weapons on left hand it should unequip
                        if (hasRightHandItem)
                            shouldUnequipRightHand = true;
                        // Unequip left-hand weapon, don't unequip shield
                        if (hasLeftHandItem && tempEquipWeapons.GetLeftHandWeaponItem() != null)
                            shouldUnequipLeftHand = true;
                        break;
                    case WeaponItemEquipType.DualWieldable:
                        DualWieldRestriction dualWieldRestriction = equippingWeaponItem.GetDualWieldRestriction();
                        if (dualWieldRestriction == DualWieldRestriction.MainHandRestricted && !isLeftHand)
                        {
                            gameMessage = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_LEFT_HAND;
                            return false;
                        }
                        if (dualWieldRestriction == DualWieldRestriction.OffHandRestricted && isLeftHand)
                        {
                            gameMessage = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_RIGHT_HAND;
                            return false;
                        }
                        // If weapon is one hand can dual its equip position must be right or left hand
                        if (!isLeftHand && hasRightHandItem)
                        {
                            shouldUnequipRightHand = true;
                        }
                        if (isLeftHand && hasLeftHandItem)
                        {
                            shouldUnequipLeftHand = true;
                        }
                        // Unequip item if right hand weapon is main-hand only or two-hand when equipping at left-hand
                        if (isLeftHand && hasRightHandItem)
                        {
                            if (rightHandEquipType == WeaponItemEquipType.MainHandOnly ||
                                rightHandEquipType == WeaponItemEquipType.TwoHand)
                                shouldUnequipRightHand = true;
                        }
                        // Unequip item if left hand weapon is off-hand only when equipping at right-hand
                        if (!isLeftHand && hasLeftHandItem)
                        {
                            if (leftHandEquipType == WeaponItemEquipType.OffHandOnly)
                                shouldUnequipLeftHand = true;
                        }
                        break;
                    case WeaponItemEquipType.TwoHand:
                        // If weapon is one hand its equip position must be right hand
                        if (isLeftHand)
                        {
                            gameMessage = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_RIGHT_HAND;
                            return false;
                        }
                        // Unequip both left and right hand
                        if (hasRightHandItem)
                            shouldUnequipRightHand = true;
                        if (hasLeftHandItem)
                            shouldUnequipLeftHand = true;
                        break;
                    case WeaponItemEquipType.OffHandOnly:
                        // If weapon is off-hand only its equip position must be left hand
                        if (!isLeftHand)
                        {
                            gameMessage = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_LEFT_HAND;
                            return false;
                        }
                        // Unequip both left and right hand (there is no shield for main-hand)
                        if (hasRightHandItem)
                            shouldUnequipRightHand = true;
                        if (hasLeftHandItem)
                            shouldUnequipLeftHand = true;
                        break;
                }
                return true;
            }

            // Equipping item is shield
            IShieldItem equippingShieldItem = equippingItem.GetShieldItem();
            if (equippingShieldItem != null)
            {
                // If it is shield, its equip position must be left hand
                if (!isLeftHand)
                {
                    gameMessage = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_LEFT_HAND;
                    return false;
                }
                if (hasRightHandItem && rightHandEquipType == WeaponItemEquipType.TwoHand)
                    shouldUnequipRightHand = true;
                if (hasLeftHandItem)
                    shouldUnequipLeftHand = true;
                return true;
            }
            gameMessage = UITextKeys.UI_ERROR_CANNOT_EQUIP;
            return false;
        }

        public static bool CanEquipItem(this ICharacterData character, CharacterItem equippingItem, byte equipSlotIndex, out UITextKeys gameMessage, out int unEquippingIndex)
        {
            unEquippingIndex = -1;

            if (equippingItem.GetArmorItem() == null)
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_EQUIP;
                return false;
            }

            if (!equippingItem.GetEquipmentItem().CanEquip(character, equippingItem.level, out gameMessage))
                return false;

            // Equipping item is armor
            IArmorItem equippingArmorItem = equippingItem.GetArmorItem();
            if (equippingArmorItem != null)
            {
                unEquippingIndex = character.IndexOfEquipItemByEquipPosition(equippingArmorItem.GetEquipPosition(), equipSlotIndex);
                return true;
            }
            gameMessage = UITextKeys.UI_ERROR_CANNOT_EQUIP;
            return false;
        }

        public static bool EquipWeapon(this ICharacterData character, int nonEquipIndex, byte equipWeaponSet, bool isLeftHand, out UITextKeys gameMessage)
        {
            if (nonEquipIndex < 0 || nonEquipIndex >= character.NonEquipItems.Count)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                return false;
            }

            CharacterItem equippingItem = character.NonEquipItems[nonEquipIndex];
            bool shouldUnequipRightHand;
            bool shouldUnequipLeftHand;
            if (!character.CanEquipWeapon(equippingItem, equipWeaponSet, isLeftHand, out gameMessage, out shouldUnequipRightHand, out shouldUnequipLeftHand))
                return false;

            int unEquipCount = -1;
            if (shouldUnequipRightHand)
                ++unEquipCount;
            if (shouldUnequipLeftHand)
                ++unEquipCount;

            if (character.UnEquipItemWillOverwhelming(unEquipCount))
            {
                gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                return false;
            }

            int unEquippedIndexRightHand = -1;
            if (shouldUnequipRightHand)
            {
                if (!character.UnEquipWeapon(equipWeaponSet, false, true, out gameMessage, out unEquippedIndexRightHand, fillEmptySlots: false))
                    return false;
            }
            int unEquippedIndexLeftHand = -1;
            if (shouldUnequipLeftHand)
            {
                if (!character.UnEquipWeapon(equipWeaponSet, true, true, out gameMessage, out unEquippedIndexLeftHand, fillEmptySlots: false))
                    return false;
            }

            // Equipping items
            character.FillWeaponSetsIfNeeded(equipWeaponSet);
            EquipWeapons tempEquipWeapons = character.SelectableWeaponSets[equipWeaponSet];
            if (isLeftHand)
            {
                equippingItem.equipSlotIndex = equipWeaponSet;
                tempEquipWeapons.leftHand = equippingItem;
                character.SelectableWeaponSets[equipWeaponSet] = tempEquipWeapons;
            }
            else
            {
                equippingItem.equipSlotIndex = equipWeaponSet;
                tempEquipWeapons.rightHand = equippingItem;
                character.SelectableWeaponSets[equipWeaponSet] = tempEquipWeapons;
            }
            // Update inventory
            if (unEquippedIndexRightHand >= 0 && unEquippedIndexLeftHand >= 0)
            {
                // Swap with equipped item
                character.NonEquipItems[nonEquipIndex] = character.NonEquipItems[unEquippedIndexRightHand];
                character.NonEquipItems.RemoveOrPlaceEmptySlot(unEquippedIndexRightHand);
                // Find empty slot for unequipped left-hand weapon to swap with empty slot
                if (GameInstance.Singleton.IsLimitInventorySlot)
                    character.NonEquipItems.MoveItemToEmptySlot(unEquippedIndexLeftHand);
            }
            else if (unEquippedIndexRightHand >= 0)
            {
                // Swap with equipped item
                character.NonEquipItems[nonEquipIndex] = character.NonEquipItems[unEquippedIndexRightHand];
                character.NonEquipItems.RemoveOrPlaceEmptySlot(unEquippedIndexRightHand);
            }
            else if (unEquippedIndexLeftHand >= 0)
            {
                // Swap with equipped item
                character.NonEquipItems[nonEquipIndex] = character.NonEquipItems[unEquippedIndexLeftHand];
                character.NonEquipItems.RemoveOrPlaceEmptySlot(unEquippedIndexLeftHand);
            }
            else
            {
                // Remove equipped item
                character.NonEquipItems.RemoveOrPlaceEmptySlot(nonEquipIndex);
            }
            character.FillEmptySlots(true);
            gameMessage = UITextKeys.NONE;
            return true;
        }

        public static bool UnEquipWeapon(this ICharacterData character, byte equipWeaponSet, bool isLeftHand, bool doNotValidate, out UITextKeys gameMessage, out int unEquippedIndex, int expectedUnequippedIndex = -1, bool fillEmptySlots = true)
        {
            unEquippedIndex = -1;
            character.FillWeaponSetsIfNeeded(equipWeaponSet);
            EquipWeapons tempEquipWeapons = character.SelectableWeaponSets[equipWeaponSet];
            CharacterItem unEquipItem;

            if (isLeftHand)
            {
                // Unequip left-hand weapon
                unEquipItem = tempEquipWeapons.leftHand;
                if (!doNotValidate && !unEquipItem.IsEmptySlot() &&
                    character.UnEquipItemWillOverwhelming())
                {
                    gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                    return false;
                }
                tempEquipWeapons.leftHand = CharacterItem.Empty;
                character.SelectableWeaponSets[equipWeaponSet] = tempEquipWeapons;
            }
            else
            {
                // Unequip right-hand weapon
                unEquipItem = tempEquipWeapons.rightHand;
                if (!doNotValidate && !unEquipItem.IsEmptySlot() &&
                    character.UnEquipItemWillOverwhelming())
                {
                    gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                    return false;
                }
                tempEquipWeapons.rightHand = CharacterItem.Empty;
                character.SelectableWeaponSets[equipWeaponSet] = tempEquipWeapons;
            }

            if (!unEquipItem.IsEmptySlot())
            {
                character.AddOrSetNonEquipItems(unEquipItem, out unEquippedIndex, expectedUnequippedIndex);
                if (fillEmptySlots)
                    character.FillEmptySlots(true);
            }
            gameMessage = UITextKeys.NONE;
            return true;
        }

        public static bool EquipArmor(this ICharacterData character, int nonEquipIndex, byte equipSlotIndex, out UITextKeys gameMessage)
        {
            if (nonEquipIndex < 0 || nonEquipIndex >= character.NonEquipItems.Count)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                return false;
            }

            CharacterItem equippingItem = character.NonEquipItems[nonEquipIndex];
            int unEquippingIndex;
            if (!character.CanEquipItem(equippingItem, equipSlotIndex, out gameMessage, out unEquippingIndex))
                return false;

            int unEquippedIndex = -1;
            if (unEquippingIndex >= 0 && !character.UnEquipArmor(unEquippingIndex, true, out gameMessage, out unEquippedIndex, fillEmptySlots: false))
                return false;

            // Can equip the item when there is no equipped item or able to unequip the equipped item
            equippingItem.equipSlotIndex = equipSlotIndex;
            character.EquipItems.Add(equippingItem);
            // Update inventory
            if (unEquippedIndex >= 0)
            {
                // Swap with equipped item
                character.NonEquipItems[nonEquipIndex] = character.NonEquipItems[unEquippedIndex];
                character.NonEquipItems.RemoveOrPlaceEmptySlot(unEquippedIndex);
            }
            else
            {
                // Remove equipped item
                character.NonEquipItems.RemoveOrPlaceEmptySlot(nonEquipIndex);
            }
            character.FillEmptySlots(true);
            gameMessage = UITextKeys.NONE;
            return true;
        }

        public static bool UnEquipArmor(this ICharacterData character, int index, bool doNotValidate, out UITextKeys gameMessage, out int unEquippedIndex, int expectedUnequippedIndex = -1, bool fillEmptySlots = true)
        {
            unEquippedIndex = -1;
            if (index < 0 || index >= character.EquipItems.Count)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                return false;
            }
            CharacterItem unEquipItem = character.EquipItems[index];
            if (!doNotValidate && !unEquipItem.IsEmptySlot() &&
                character.UnEquipItemWillOverwhelming())
            {
                gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                return false;
            }
            character.EquipItems.RemoveAt(index);

            if (!unEquipItem.IsEmptySlot())
            {
                character.AddOrSetNonEquipItems(unEquipItem, out unEquippedIndex, expectedUnequippedIndex);
                if (fillEmptySlots)
                    character.FillEmptySlots(true);
            }
            gameMessage = UITextKeys.NONE;
            return true;
        }

        public static bool SwapOrMergeItem(this ICharacterData character, int fromIndex, int toIndex, out UITextKeys gameMessage)
        {
            if (fromIndex < 0 || fromIndex >= character.NonEquipItems.Count ||
                toIndex < 0 || toIndex >= character.NonEquipItems.Count ||
                fromIndex == toIndex)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
                return false;
            }

            CharacterItem fromItem = character.NonEquipItems[fromIndex];
            CharacterItem toItem = character.NonEquipItems[toIndex];
            if (fromItem.dataId == toItem.dataId && !fromItem.IsFull() && !toItem.IsFull() && fromItem.level == toItem.level)
            {
                // Merge if same id and not full
                int maxStack = toItem.GetMaxStack();
                if (toItem.amount + fromItem.amount <= maxStack)
                {
                    toItem.amount += fromItem.amount;
                    character.NonEquipItems[toIndex] = toItem;
                    character.NonEquipItems.RemoveOrPlaceEmptySlot(fromIndex);
                    character.FillEmptySlots();
                }
                else
                {
                    int remains = toItem.amount + fromItem.amount - maxStack;
                    toItem.amount = maxStack;
                    fromItem.amount = remains;
                    character.NonEquipItems[fromIndex] = fromItem;
                    character.NonEquipItems[toIndex] = toItem;
                }
            }
            else
            {
                // Swap
                character.NonEquipItems[fromIndex] = toItem;
                character.NonEquipItems[toIndex] = fromItem;
            }
            gameMessage = UITextKeys.NONE;
            return true;
        }

        public static bool DecreaseItem(this IPlayerCharacterData character, InventoryType inventoryType, int index, byte equipSlotIndex, int amount, out UITextKeys gameMessage, out CharacterItem targetItem, System.Func<CharacterItem, UITextKeys> validateTargetItem)
        {
            targetItem = CharacterItem.Empty;
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_INDEX;
            if (amount <= 0)
                return false;

            bool canUnEquipItem = true;
            if (character is BasePlayerCharacterEntity castedCharacter && !castedCharacter.CanUnEquipItem())
                canUnEquipItem = false;

            switch (inventoryType)
            {
                case InventoryType.NonEquipItems:
                    if (index >= character.NonEquipItems.Count)
                        return false;
                    targetItem = character.NonEquipItems[index].Clone();
                    break;
                case InventoryType.EquipItems:
                    if (index >= character.EquipItems.Count || !canUnEquipItem)
                        return false;
                    targetItem = character.EquipItems[index].Clone();
                    break;
                case InventoryType.EquipWeaponRight:
                    if (index >= character.SelectableWeaponSets.Count || !canUnEquipItem)
                        return false;
                    targetItem = character.SelectableWeaponSets[equipSlotIndex].rightHand.Clone();
                    break;
                case InventoryType.EquipWeaponLeft:
                    if (index >= character.SelectableWeaponSets.Count || !canUnEquipItem)
                        return false;
                    targetItem = character.SelectableWeaponSets[equipSlotIndex].leftHand.Clone();
                    break;
                default:
                    return false;
            }
            if (targetItem.IsEmptySlot())
                return false;

            if (amount > targetItem.amount)
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                return false;
            }

            gameMessage = validateTargetItem.Invoke(targetItem);
            if (gameMessage != UITextKeys.NONE)
            {
                return false;
            }

            switch (inventoryType)
            {
                case InventoryType.NonEquipItems:
                    if (!character.NonEquipItems.DecreaseItemsByIndex(index, amount, GameInstance.Singleton.IsLimitInventorySlot, false))
                    {
                        gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                        return false;
                    }
                    break;
                case InventoryType.EquipItems:
                    if (!character.EquipItems.DecreaseItemsByIndex(index, amount, GameInstance.Singleton.IsLimitInventorySlot, false))
                    {
                        gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                        return false;
                    }
                    break;
                case InventoryType.EquipWeaponRight:
                    if (amount == targetItem.amount)
                    {
                        EquipWeapons equipWeapons = character.SelectableWeaponSets[index];
                        equipWeapons.rightHand = CharacterItem.Empty;
                        character.SelectableWeaponSets[equipSlotIndex] = equipWeapons;
                    }
                    else
                    {
                        EquipWeapons equipWeapons = character.SelectableWeaponSets[index];
                        CharacterItem equipWeapon = equipWeapons.rightHand;
                        equipWeapon.amount -= amount;
                        equipWeapons.rightHand = equipWeapon;
                        character.SelectableWeaponSets[equipSlotIndex] = equipWeapons;
                    }
                    break;
                case InventoryType.EquipWeaponLeft:
                    if (amount == targetItem.amount)
                    {
                        EquipWeapons equipWeapons = character.SelectableWeaponSets[index];
                        equipWeapons.leftHand = CharacterItem.Empty;
                        character.SelectableWeaponSets[equipSlotIndex] = equipWeapons;
                    }
                    else
                    {
                        EquipWeapons equipWeapons = character.SelectableWeaponSets[index];
                        CharacterItem equipWeapon = equipWeapons.leftHand;
                        equipWeapon.amount -= amount;
                        equipWeapons.leftHand = equipWeapon;
                        character.SelectableWeaponSets[equipSlotIndex] = equipWeapons;
                    }
                    break;
            }

            character.FillEmptySlots();
            return true;
#else
            gameMessage = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE;
            return false;
#endif
        }

        public static UITextKeys ValidateSellItem(CharacterItem targetItem)
        {
            BaseItem targetItemData = targetItem.GetItem();
            if (targetItemData.RestrictSelling)
            {
                return UITextKeys.UI_ERROR_ITEM_SELLING_RESTRICTED;
            }
            return UITextKeys.NONE;
        }

        public static bool SellItem(this IPlayerCharacterData character, InventoryType inventoryType, int index, byte equipSlotIndex, int amount, out UITextKeys gameMessage)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!DecreaseItem(character, inventoryType, index, equipSlotIndex, amount, out gameMessage, out CharacterItem targetItem, ValidateSellItem))
            {
                return false;
            }

            // Increase currencies
            BaseItem targetItemData = targetItem.GetItem();
            GameInstance.Singleton.GameplayRule.IncreaseCurrenciesWhenSellItem(character, targetItemData, amount);
            gameMessage = UITextKeys.NONE;

            GameInstance.ServerLogHandlers.LogSellNpcItem(character, targetItem, amount);
            return true;
#else
            gameMessage = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE;
            return false;
#endif
        }

        public static bool SellItems(this IPlayerCharacterData character, int[] selectedIndexes, out UITextKeys gameMessage)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            List<int> indexes = new List<int>(selectedIndexes);
            indexes.Sort();
            int tempIndex;
            for (int i = indexes.Count - 1; i >= 0; --i)
            {
                tempIndex = indexes[i];
                if (tempIndex >= character.NonEquipItems.Count)
                    continue;
                character.SellItem(InventoryType.NonEquipItems, tempIndex, 0, character.NonEquipItems[tempIndex].amount, out _);
            }
            gameMessage = UITextKeys.NONE;
            return true;
#else
            gameMessage = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE;
            return false;
#endif
        }
    }
}
