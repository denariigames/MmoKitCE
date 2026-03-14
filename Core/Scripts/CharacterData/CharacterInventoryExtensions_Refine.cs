using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public static partial class CharacterInventoryExtensions
    {
        public static bool RefineItem(this IPlayerCharacterData character, InventoryType inventoryType, int index, byte equipSlotIndex, int[] enhancerDataIds, out UITextKeys gameMessage)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            switch (inventoryType)
            {
                case InventoryType.NonEquipItems:
                    return character.RefineNonEquipItem(index, enhancerDataIds, out gameMessage);
                case InventoryType.EquipItems:
                    return character.RefineEquipItem(index, enhancerDataIds, out gameMessage);
                case InventoryType.EquipWeaponRight:
                    return character.RefineRightHandItem(equipSlotIndex, enhancerDataIds, out gameMessage);
                case InventoryType.EquipWeaponLeft:
                    return character.RefineLeftHandItem(equipSlotIndex, enhancerDataIds, out gameMessage);
            }
            gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_DATA;
            return false;
#else
            gameMessage = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE;
            return false;
#endif
        }

        public static bool RefineRightHandItem(this IPlayerCharacterData character, byte equipSlotIndex, int[] enhancerDataIds, out UITextKeys gameMessageType)
        {
            return RefineItem(character, character.SelectableWeaponSets[equipSlotIndex].rightHand, enhancerDataIds, (refinedItem) =>
            {
                EquipWeapons equipWeapon = character.SelectableWeaponSets[equipSlotIndex];
                equipWeapon.rightHand = refinedItem;
                character.SelectableWeaponSets[equipSlotIndex] = equipWeapon;
            }, () =>
            {
                EquipWeapons equipWeapon = character.SelectableWeaponSets[equipSlotIndex];
                equipWeapon.rightHand = CharacterItem.Empty;
                character.SelectableWeaponSets[equipSlotIndex] = equipWeapon;
            }, out gameMessageType);
        }

        public static bool RefineLeftHandItem(this IPlayerCharacterData character, byte equipSlotIndex, int[] enhancerDataIds, out UITextKeys gameMessageType)
        {
            return RefineItem(character, character.SelectableWeaponSets[equipSlotIndex].leftHand, enhancerDataIds, (refinedItem) =>
            {
                EquipWeapons equipWeapon = character.SelectableWeaponSets[equipSlotIndex];
                equipWeapon.leftHand = refinedItem;
                character.SelectableWeaponSets[equipSlotIndex] = equipWeapon;
            }, () =>
            {
                EquipWeapons equipWeapon = character.SelectableWeaponSets[equipSlotIndex];
                equipWeapon.leftHand = CharacterItem.Empty;
                character.SelectableWeaponSets[equipSlotIndex] = equipWeapon;
            }, out gameMessageType);
        }

        public static bool RefineEquipItem(this IPlayerCharacterData character, int itemIndex, int[] enhancerDataIds, out UITextKeys gameMessageType)
        {
            return RefineItemByList(character, character.EquipItems, itemIndex, enhancerDataIds, out gameMessageType);
        }

        public static bool RefineNonEquipItem(this IPlayerCharacterData character, int itemIndex, int[] enhancerDataIds, out UITextKeys gameMessageType)
        {
            return RefineItemByList(character, character.NonEquipItems, itemIndex, enhancerDataIds, out gameMessageType);
        }

        private static bool RefineItemByList(IPlayerCharacterData character, IList<CharacterItem> list, int itemIndex, int[] enhancerDataIds, out UITextKeys gameMessageType)
        {
            return RefineItem(character, list[itemIndex], enhancerDataIds, (refinedItem) =>
            {
                list[itemIndex] = refinedItem;
            }, () =>
            {
                list.RemoveOrPlaceEmptySlot(itemIndex);
            }, out gameMessageType);
        }

        private static bool RefineItem(IPlayerCharacterData character, CharacterItem refiningItem, int[] enhancerDataIds, System.Action<CharacterItem> onRefine, System.Action onDestroy, out UITextKeys gameMessage)
        {
            refiningItem = refiningItem.Clone();
            if (refiningItem.IsEmptySlot())
            {
                // Cannot refine because character item is empty
                gameMessage = UITextKeys.UI_ERROR_ITEM_NOT_FOUND;
                return false;
            }
            BaseItem equipmentItem = refiningItem.GetEquipmentItem() as BaseItem;
            if (equipmentItem == null)
            {
                // Cannot refine because it's not equipment item
                gameMessage = UITextKeys.UI_ERROR_ITEM_NOT_EQUIPMENT;
                return false;
            }
            if (!equipmentItem.CanRefine(character, refiningItem.level, enhancerDataIds, out gameMessage))
            {
                // Cannot refine because of some reasons
                return false;
            }
            ItemRefineLevel refineLevel = equipmentItem.ItemRefine.Levels[refiningItem.level - 1];
            bool inventoryChanged = false;
            List<BaseItem> enhancerItems = new List<BaseItem>();
            float increaseSuccessRate = 0f;
            float decreaseRequireGoldRate = 0f;
            float chanceToNotDecreaseLevels = 0f;
            float chanceToNotDestroyItem = 0f;
            for (int i = 0; i < enhancerDataIds.Length; ++i)
            {
                int materialDataId = enhancerDataIds[i];
                int indexOfMaterial = character.IndexOfNonEquipItem(materialDataId);
                if (indexOfMaterial < 0 || character.NonEquipItems[indexOfMaterial].IsEmptySlot())
                {
                    // Material not found
                    continue;
                }
                for (int j = 0; j < refineLevel.AvailableEnhancers.Length; ++j)
                {
                    if (refineLevel.AvailableEnhancers[j].item == null || refineLevel.AvailableEnhancers[j].item.DataId != materialDataId)
                    {
                        // Not a material we're looking for
                        continue;
                    }
                    // Found the material, enhance
                    increaseSuccessRate += refineLevel.AvailableEnhancers[j].increaseSuccessRate;
                    decreaseRequireGoldRate += refineLevel.AvailableEnhancers[j].decreaseRequireGoldRate;
                    chanceToNotDecreaseLevels += refineLevel.AvailableEnhancers[j].chanceToNotDecreaseLevels;
                    chanceToNotDestroyItem += refineLevel.AvailableEnhancers[j].chanceToNotDestroyItem;
                    break;
                }
                enhancerItems.Add(character.NonEquipItems[indexOfMaterial].GetItem());
                character.DecreaseItems(materialDataId, 1);
                inventoryChanged = true;
            }
            bool isSuccess = false;
            bool isDestroy = false;
            int decreaseLevels = 0;
            bool isReturning = false;
            ItemRefineFailReturning returning = null;
            if (Random.value <= refineLevel.SuccessRate + increaseSuccessRate)
            {
                // If success, increase item level
                isSuccess = true;
                gameMessage = UITextKeys.UI_REFINE_SUCCESS;
                ++refiningItem.level;
                onRefine.Invoke(refiningItem);
            }
            else
            {
                // Fail
                gameMessage = UITextKeys.UI_REFINE_FAIL;
                if (refineLevel.RefineFailDestroyItem)
                {
                    // If condition when fail is it has to be destroyed
                    if (Random.value > chanceToNotDestroyItem)
                    {
                        isDestroy = true;
                        onDestroy.Invoke();
                    }
                }
                if (!isDestroy)
                {
                    // If condition when fail is reduce its level
                    if (Random.value > chanceToNotDecreaseLevels)
                    {
                        decreaseLevels = refineLevel.RefineFailDecreaseLevels;
                        while (refiningItem.level - decreaseLevels < 1)
                            --decreaseLevels;
                        refiningItem.level -= decreaseLevels;
                        onRefine.Invoke(refiningItem);
                    }
                }
                // Return items/currencies
                if (refineLevel.FailReturnings != null && refineLevel.FailReturnings.Length > 0)
                {
                    List<WeightedRandomizerItem<ItemRefineFailReturning>> randomItems = new List<WeightedRandomizerItem<ItemRefineFailReturning>>();
                    foreach (ItemRefineFailReturning item in refineLevel.FailReturnings)
                    {
                        randomItems.Add(new WeightedRandomizerItem<ItemRefineFailReturning>()
                        {
                            item = item,
                            weight = item.randomWeight,
                        });
                    }
                    returning = WeightedRandomizer.From(randomItems).TakeOne();
                    isReturning = true;
                    character.Gold = character.Gold.Increase(returning.returnGold);
                    character.IncreaseItems(returning.returnItems, onFail: dropData => ItemDropEntity.Drop(null, RewardGivenType.None, dropData, new string[] { character.Id }).Forget());
                    character.IncreaseCurrencies(returning.returnCurrencies);
                    inventoryChanged = true;
                }
            }
            if (refineLevel.RequireItems != null)
            {
                // Decrease required items
                character.DecreaseItems(refineLevel.RequireItems);
                inventoryChanged = true;
            }
            // Fill empty slots
            if (inventoryChanged)
                character.FillEmptySlots();
            // Decrease required gold
            GameInstance.Singleton.GameplayRule.DecreaseCurrenciesWhenRefineItem(character, refineLevel, decreaseRequireGoldRate);
            GameInstance.ServerLogHandlers.LogRefine(character, refiningItem, enhancerItems, increaseSuccessRate, decreaseRequireGoldRate, chanceToNotDecreaseLevels, chanceToNotDestroyItem, isSuccess, isDestroy, refineLevel, isReturning, returning);
            return true;
        }
    }
}
