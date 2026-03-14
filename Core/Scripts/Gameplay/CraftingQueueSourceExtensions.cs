using UnityEngine;

namespace MultiplayerARPG
{
    public static class CraftingQueueSourceExtensions
    {
        public static bool IsInCraftDistance(this ICraftingQueueSource source, Vector3 crafterPosition)
        {
            return source.CraftingDistance <= 0f || Vector3.Distance(crafterPosition, source.transform.position) <= source.CraftingDistance;
        }

        public static void UpdateQueue(this ICraftingQueueSource craftingHandler)
        {
            if (!craftingHandler.CanCraft)
            {
                if (craftingHandler.QueueItems.Count > 0)
                    craftingHandler.QueueItems.Clear();
                return;
            }

            if (craftingHandler.QueueItems.Count == 0)
            {
                craftingHandler.TimeCounter = 0f;
                return;
            }

            CraftingQueueItem craftingItem = craftingHandler.QueueItems[0];
            ItemCraftFormula formula = GameInstance.ItemCraftFormulas[craftingItem.dataId];
            if (!BaseGameNetworkManager.Singleton.TryGetEntityByObjectId(craftingItem.crafterId, out BasePlayerCharacterEntity crafter))
            {
                // Crafter may left the game
                craftingHandler.QueueItems.RemoveAt(0);
                return;
            }

            BaseGameEntity craftingSourceEntity;
            if (!BaseGameNetworkManager.Singleton.TryGetEntityByObjectId(craftingItem.sourceObjectId, out craftingSourceEntity))
            {
                // Crafting source might be destroyed
                craftingHandler.QueueItems.RemoveAt(0);
                return;
            }

            if (!craftingSourceEntity.TryGetComponent(out ICraftingQueueSource craftingSourceEntityComponent) ||
                !craftingSourceEntityComponent.IsInCraftDistance(crafter.EntityTransform.position))
            {
                // Crafter too far from crafting source
                craftingHandler.QueueItems.RemoveAt(0);
                return;
            }

            if (!formula.ItemCraft.CanCraft(crafter, out UITextKeys errorMessage))
            {
                craftingHandler.TimeCounter = 0f;
                craftingHandler.QueueItems.RemoveAt(0);
                GameInstance.ServerGameMessageHandlers.SendGameMessage(crafter.ConnectionId, errorMessage);
                return;
            }

            craftingHandler.TimeCounter += Time.unscaledDeltaTime;
            if (craftingHandler.TimeCounter >= 1f)
            {
                craftingItem.craftRemainsDuration -= craftingHandler.TimeCounter;
                craftingHandler.TimeCounter = 0f;
                if (craftingItem.craftRemainsDuration <= 0f)
                {
                    // Reduce items and add crafting item
                    formula.ItemCraft.CraftItem(crafter);
                    // Reduce amount
                    if (craftingItem.amount > 1)
                    {
                        --craftingItem.amount;
                        craftingItem.craftRemainsDuration = formula.CraftDuration;
                        craftingHandler.QueueItems[0] = craftingItem;
                    }
                    else
                    {
                        craftingHandler.QueueItems.RemoveAt(0);
                    }
                }
                else
                {
                    // Update remains duration
                    craftingHandler.QueueItems[0] = craftingItem;
                }
            }
        }

        public static bool AppendCraftingQueueItem(this ICraftingQueueSource craftingHandler, BasePlayerCharacterEntity crafter, int dataId, int amount, out UITextKeys errorMessage)
        {
            return craftingHandler.AppendCraftingQueueItem(craftingHandler, crafter, dataId, amount, out errorMessage);
        }

        public static bool AppendCraftingQueueItem(this ICraftingQueueSource craftingHandler, ICraftingQueueSource craftingSourceEntity, BasePlayerCharacterEntity crafter, int dataId, int amount, out UITextKeys errorMessage)
        {
            if (craftingHandler.ObjectId != crafter.ObjectId && !craftingHandler.PublicQueue)
            {
                // Not public, so it will be updated by player's source
                return crafter.CraftingComponent.AppendCraftingQueueItem(craftingHandler, crafter, dataId, amount, out errorMessage);
            }
            errorMessage = UITextKeys.NONE;
            if (!craftingHandler.CanCraft)
                return false;
            ItemCraftFormula itemCraftFormula;
            if (!GameInstance.ItemCraftFormulas.TryGetValue(dataId, out itemCraftFormula))
                return false;
            if (!itemCraftFormula.ItemCraft.CanCraft(crafter, out errorMessage))
                return false;
            if (craftingHandler.QueueItems.Count >= craftingHandler.MaxQueueSize)
                return false;
            craftingHandler.QueueItems.Add(new CraftingQueueItem()
            {
                crafterId = crafter.ObjectId,
                dataId = dataId,
                amount = amount,
                craftRemainsDuration = itemCraftFormula.CraftDuration,
                sourceObjectId = craftingSourceEntity.ObjectId,
            });
            return true;
        }

        public static void ChangeCraftingQueueItem(this ICraftingQueueSource source, BasePlayerCharacterEntity crafter, int index, int amount)
        {
            if (source.ObjectId != crafter.ObjectId && !source.PublicQueue)
            {
                // Not public, so it will be updated by player's source
                crafter.CraftingComponent.ChangeCraftingQueueItem(crafter, index, amount);
                return;
            }
            if (!source.CanCraft)
                return;
            if (index < 0 || index >= source.QueueItems.Count)
                return;
            if (source.QueueItems[index].crafterId != crafter.ObjectId)
                return;
            if (amount <= 0)
            {
                source.QueueItems.RemoveAt(index);
                return;
            }
            CraftingQueueItem craftingItem = source.QueueItems[index];
            craftingItem.amount = amount;
            source.QueueItems[index] = craftingItem;
        }

        public static void CancelCraftingQueueItem(this ICraftingQueueSource source, BasePlayerCharacterEntity crafter, int index)
        {
            if (source.ObjectId != crafter.ObjectId && !source.PublicQueue)
            {
                // Not public, so it will be updated by player's source
                crafter.CraftingComponent.CancelCraftingQueueItem(crafter, index);
                return;
            }
            if (!source.CanCraft)
                return;
            if (index < 0 || index >= source.QueueItems.Count)
                return;
            if (source.QueueItems[index].crafterId != crafter.ObjectId)
                return;
            source.QueueItems.RemoveAt(index);
        }
    }
}
