using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static partial class CharacterInventoryExtensions
    {
        public static UITextKeys ValidateDismantleItem(CharacterItem targetItem)
        {
            if (!GameInstance.Singleton.dismantleFilter.Filter(targetItem))
            {
                return UITextKeys.UI_ERROR_INVALID_ITEM_DATA;
            }
            return UITextKeys.NONE;
        }

        public static bool VerifyDismantleItem(this IPlayerCharacterData simulateCharacter, InventoryType inventoryType, int index, byte equipSlotIndex, int amount, out UITextKeys gameMessage, out ItemAmount dismantleItem, out int returningGold, out List<ItemAmount> returningItems, out List<CurrencyAmount> returningCurrencies)
        {
            gameMessage = UITextKeys.NONE;
            dismantleItem = new ItemAmount();
            returningGold = 0;
            returningItems = null;
            returningCurrencies = null;

            if (!DecreaseItem(simulateCharacter, inventoryType, index, equipSlotIndex, amount, out gameMessage, out CharacterItem targetItem, ValidateDismantleItem))
            {
                return false;
            }

            // Character can receives all items or not?
            BaseItem targetItemData = targetItem.GetItem();
            CharacterDataCache cacheData = simulateCharacter.GetCaches();
            targetItem.GetDismantleReturnItems(amount, out returningItems, out returningCurrencies);
            if (simulateCharacter.NonEquipItems.IncreasingItemsWillOverwhelming(
                returningItems,
                GameInstance.Singleton.IsLimitInventoryWeight,
                cacheData.LimitItemWeight,
                cacheData.TotalItemWeight,
                GameInstance.Singleton.IsLimitInventorySlot,
                cacheData.LimitItemSlot))
            {
                returningItems.Clear();
                gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                return false;
            }
            dismantleItem = new ItemAmount()
            {
                item = targetItemData,
                amount = amount,
            };
            simulateCharacter.NonEquipItems.IncreaseItems(returningItems);
            returningGold = targetItemData.DismantleReturnGold * amount;
            return true;
        }

        public static bool DismantleItem(this IPlayerCharacterData character, InventoryType inventoryType, int index, byte equipSlotIndex, int amount, out UITextKeys gameMessage)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            PlayerCharacterData simulateCharacter = character.CloneTo(new PlayerCharacterData(), true, true, true, false, true, true, true, false, false, false, false, false, false, false, false);
            // Proceed dismantle
            ItemAmount dismantleItem;
            int returningGold;
            List<ItemAmount> returningItems;
            List<CurrencyAmount> returningCurrencies;
            if (!VerifyDismantleItem(simulateCharacter, inventoryType, index, equipSlotIndex, amount, out gameMessage, out dismantleItem, out returningGold, out returningItems, out returningCurrencies))
                return false;
            List<ItemAmount> dismantleItems = new List<ItemAmount>() { dismantleItem };
            List<CharacterItem> increasedItems = new List<CharacterItem>();
            List<CharacterItem> droppedItems = new List<CharacterItem>();
            // Apply changes
            character.Gold = character.Gold.Increase(returningGold);
            character.NonEquipItems = simulateCharacter.NonEquipItems;
            switch (inventoryType)
            {
                case InventoryType.EquipItems:
                    character.EquipItems = simulateCharacter.EquipItems;
                    break;
                case InventoryType.EquipWeaponRight:
                case InventoryType.EquipWeaponLeft:
                    character.SelectableWeaponSets = simulateCharacter.SelectableWeaponSets;
                    break;
            }
            character.IncreaseCurrencies(returningCurrencies);
            character.FillEmptySlots();

            var entity = character as BasePlayerCharacterEntity;
            if (entity != null)
            {
                if (returningGold > 0)
                    entity.OnRewardGold(RewardGivenType.Crafting, returningGold);

                foreach (var currencyAmount in returningCurrencies)
                    entity.OnRewardCurrency(RewardGivenType.Crafting, currencyAmount.currency, currencyAmount.amount);

                foreach (var itemAmount in returningItems)
                    entity.OnRewardItem(RewardGivenType.Crafting, itemAmount.item, itemAmount.amount);
            }

            GameInstance.ServerLogHandlers.LogDismantleItems(character, dismantleItems);
            return true;
#else
            gameMessage = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE;
            return false;
#endif
        }

        public static bool DismantleItems(this IPlayerCharacterData character, int[] selectedIndexes, out UITextKeys gameMessage)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            gameMessage = UITextKeys.NONE;
            PlayerCharacterData simulateCharacter = character.CloneTo(new PlayerCharacterData(), true, true, true, false, true, true, true, false, false, false, false, false, false, false, false);
            List<int> indexes = new List<int>(selectedIndexes);
            indexes.Sort();
            // Proceed dismantle
            Dictionary<int, int> indexAmountPairs = new Dictionary<int, int>();
            List<ItemAmount> dismantleItems = new List<ItemAmount>();
            int returningGold = 0;
            List<ItemAmount> returningItems = new List<ItemAmount>();
            List<CurrencyAmount> returningCurrencies = new List<CurrencyAmount>();
            // Dismantle all items
            int tempIndex;
            int tempAmount;
            ItemAmount tempDismantleItem;
            int tempReturningGold;
            List<ItemAmount> tempReturningItems;
            List<CurrencyAmount> tempReturningCurrencies;
            for (int i = indexes.Count - 1; i >= 0; --i)
            {
                tempIndex = indexes[i];
                if (indexAmountPairs.ContainsKey(tempIndex))
                    continue;
                if (tempIndex >= character.NonEquipItems.Count)
                    continue;
                tempAmount = character.NonEquipItems[tempIndex].amount;
                if (!VerifyDismantleItem(simulateCharacter, InventoryType.NonEquipItems, tempIndex, 0, tempAmount, out gameMessage, out tempDismantleItem, out tempReturningGold, out tempReturningItems, out tempReturningCurrencies))
                {
                    return false;
                }
                dismantleItems.Add(tempDismantleItem);
                returningGold += tempReturningGold;
                returningItems.AddRange(tempReturningItems);
                returningCurrencies.AddRange(tempReturningCurrencies);
                indexAmountPairs.Add(tempIndex, tempAmount);
            }
            // Apply changes
            character.Gold = character.Gold.Increase(returningGold);
            character.NonEquipItems = simulateCharacter.NonEquipItems;
            character.IncreaseCurrencies(returningCurrencies);
            character.FillEmptySlots();

            var entity = character as BasePlayerCharacterEntity;
            if (entity != null)
            {
                if (returningGold > 0)
                    entity.OnRewardGold(RewardGivenType.Crafting, returningGold);

                foreach (var currencyAmount in returningCurrencies)
                    entity.OnRewardCurrency(RewardGivenType.Crafting, currencyAmount.currency, currencyAmount.amount);

                foreach (var itemAmount in returningItems)
                    entity.OnRewardItem(RewardGivenType.Crafting, itemAmount.item, itemAmount.amount);
            }

            GameInstance.ServerLogHandlers.LogDismantleItems(character, dismantleItems);
            return true;
#else
            gameMessage = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE;
            return false;
#endif
        }

        public static void GetDismantleReturnItems(this CharacterItem dismantlingItem, int amount, out List<ItemAmount> items, out List<CurrencyAmount> currencies)
        {
            items = new List<ItemAmount>();
            currencies = new List<CurrencyAmount>();
            if (dismantlingItem.IsEmptySlot() || amount == 0)
                return;

            if (amount < 0 || amount > dismantlingItem.amount)
                amount = dismantlingItem.amount;

            // Returning items
            ItemAmount[] dismantleReturnItems = dismantlingItem.GetItem().DismantleReturnItems;
            for (int i = 0; i < dismantleReturnItems.Length; ++i)
            {
                items.Add(new ItemAmount()
                {
                    item = dismantleReturnItems[i].item,
                    amount = dismantleReturnItems[i].amount * amount,
                });
            }
            if (dismantlingItem.sockets.Count > 0)
            {
                BaseItem socketItem;
                for (int i = 0; i < dismantlingItem.sockets.Count; ++i)
                {
                    if (!GameInstance.Items.TryGetValue(dismantlingItem.sockets[i], out socketItem))
                        continue;
                    items.Add(new ItemAmount()
                    {
                        item = socketItem,
                        amount = 1,
                    });
                }
            }

            // Returning currencies
            CurrencyAmount[] dismantleReturnCurrencies = dismantlingItem.GetItem().DismantleReturnCurrencies;
            for (int i = 0; i < dismantleReturnCurrencies.Length; ++i)
            {
                currencies.Add(new CurrencyAmount()
                {
                    currency = dismantleReturnCurrencies[i].currency,
                    amount = dismantleReturnCurrencies[i].amount * amount,
                });
            }
        }
    }
}
