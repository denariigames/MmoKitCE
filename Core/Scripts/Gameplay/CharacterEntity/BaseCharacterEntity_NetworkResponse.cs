using LiteNetLibManager;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        /// <summary>
        /// This will be called at server to order character to pickup selected thing
        /// </summary>
        /// <param name="objectId"></param>
        [ServerRpc]
        protected virtual void CmdPickup(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!CanPickup())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out IPickupActivatableEntity itemDropEntity))
            {
                // Can't find the entity
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DATA);
                return;
            }

            if (!IsGameEntityInDistance(itemDropEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!itemDropEntity.ProceedPickingUpAtServer(this, out UITextKeys message))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, message);
                return;
            }

            // Do something with buffs when pickup something
            SkillAndBuffComponent.OnPickupItem();
#endif
        }

        /// <summary>
        /// This will be called at server to order character to pickup selected item from items container
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="itemsContainerIndex"></param>
        [ServerRpc]
        protected virtual void CmdPickupItemFromContainer(uint objectId, int itemsContainerIndex, int amount)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!CanPickup())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out ItemsContainerEntity itemsContainerEntity))
            {
                // Can't find the entity
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DATA);
                return;
            }

            if (!IsGameEntityInDistance(itemsContainerEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!itemsContainerEntity.IsAbleToLoot(this))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NOT_ABLE_TO_LOOT);
                return;
            }

            if (itemsContainerIndex < 0 || itemsContainerIndex >= itemsContainerEntity.Items.Count)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_ITEM_INDEX);
                return;
            }

            CharacterItem pickingItem = itemsContainerEntity.Items[itemsContainerIndex].Clone();
            if (amount < 0)
                amount = pickingItem.amount;
            pickingItem.amount = amount;
            if (this.IncreasingItemsWillOverwhelming(pickingItem.dataId, pickingItem.amount))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
                return;
            }

            this.IncreaseItems(pickingItem, characterItem => OnRewardItem(itemsContainerEntity.GivenType, characterItem));
            itemsContainerEntity.Items.DecreaseItemsByIndex(itemsContainerIndex, amount, false, true);
            itemsContainerEntity.PickedUp();
            this.FillEmptySlots();
#endif
        }

        /// <summary>
        /// This will be called at server to order character to pickup all items from items container
        /// </summary>
        /// <param name="objectId"></param>
        [ServerRpc]
        protected virtual void CmdPickupAllItemsFromContainer(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!CanPickup())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out ItemsContainerEntity itemsContainerEntity))
            {
                // Can't find the entity
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DATA);
                return;
            }

            if (!IsGameEntityInDistance(itemsContainerEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!itemsContainerEntity.IsAbleToLoot(this))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NOT_ABLE_TO_LOOT);
                return;
            }

            while (itemsContainerEntity.Items.Count > 0)
            {
                CharacterItem pickingItem = itemsContainerEntity.Items[0];
                if (this.IncreasingItemsWillOverwhelming(pickingItem.dataId, pickingItem.amount))
                {
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
                    break;
                }

                this.IncreaseItems(pickingItem, characterItem => OnRewardItem(itemsContainerEntity.GivenType, characterItem));
                itemsContainerEntity.Items.RemoveAt(0);
            }
            itemsContainerEntity.PickedUp();
            this.FillEmptySlots();
#endif
        }

        /// <summary>
        /// This will be called at server to order character to pickup nearby items
        /// </summary>
        [ServerRpc]
        protected virtual void CmdPickupNearbyItems()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!CanPickup())
                return;
            List<ItemDropEntity> itemDropEntities = FindGameEntitiesInDistance<ItemDropEntity>(CurrentGameInstance.pickUpItemDistance, CurrentGameInstance.itemDropLayer.Mask);
            foreach (ItemDropEntity itemDropEntity in itemDropEntities)
            {
                CmdPickup(itemDropEntity.ObjectId);
            }
#endif
        }

        /// <summary>
        /// This will be called at server to order character to drop items
        /// </summary>
        /// <param name="inventoryType"></param>
        /// <param name="index"></param>
        /// <param name="equipSlotIndex"></param>
        /// <param name="amount"></param>
        [ServerRpc]
        protected virtual void CmdDropItem(InventoryType inventoryType, int index, byte equipSlotIndex, int amount)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (amount <= 0 || !CanDoActions())
                return;

            bool canUnEquipItem = true;
            if (!CanUnEquipItem())
                canUnEquipItem = false;

            CharacterItem droppingItem;
            switch (inventoryType)
            {
                case InventoryType.NonEquipItems:
                    if (index >= NonEquipItems.Count)
                        return;
                    droppingItem = NonEquipItems[index].Clone();
                    break;
                case InventoryType.EquipItems:
                    if (index >= EquipItems.Count || !canUnEquipItem)
                        return;
                    droppingItem = EquipItems[index].Clone();
                    break;
                case InventoryType.EquipWeaponRight:
                    if (index >= SelectableWeaponSets.Count || !canUnEquipItem)
                        return;
                    droppingItem = SelectableWeaponSets[equipSlotIndex].rightHand.Clone();
                    break;
                case InventoryType.EquipWeaponLeft:
                    if (index >= SelectableWeaponSets.Count || !canUnEquipItem)
                        return;
                    droppingItem = SelectableWeaponSets[equipSlotIndex].leftHand.Clone();
                    break;
                default:
                    return;
            }
            if (droppingItem.IsEmptySlot() || amount > droppingItem.amount)
                return;

            if (droppingItem.GetItem().RestrictDropping)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_ITEM_DROPPING_RESTRICTED);
                return;
            }

            switch (inventoryType)
            {
                case InventoryType.NonEquipItems:
                    if (!NonEquipItems.DecreaseItemsByIndex(index, amount, GameInstance.Singleton.IsLimitInventorySlot, false))
                        return;
                    break;
                case InventoryType.EquipItems:
                    if (!EquipItems.DecreaseItemsByIndex(index, amount, GameInstance.Singleton.IsLimitInventorySlot, false))
                        return;
                    break;
                case InventoryType.EquipWeaponRight:
                    if (amount == droppingItem.amount)
                    {
                        EquipWeapons equipWeapons = SelectableWeaponSets[index];
                        equipWeapons.rightHand = CharacterItem.Empty;
                        SelectableWeaponSets[equipSlotIndex] = equipWeapons;
                    }
                    else
                    {
                        EquipWeapons equipWeapons = SelectableWeaponSets[index];
                        CharacterItem equipWeapon = equipWeapons.rightHand;
                        equipWeapon.amount -= amount;
                        equipWeapons.rightHand = equipWeapon;
                        SelectableWeaponSets[equipSlotIndex] = equipWeapons;
                    }
                    break;
                case InventoryType.EquipWeaponLeft:
                    if (amount == droppingItem.amount)
                    {
                        EquipWeapons equipWeapons = SelectableWeaponSets[index];
                        equipWeapons.leftHand = CharacterItem.Empty;
                        SelectableWeaponSets[equipSlotIndex] = equipWeapons;
                    }
                    else
                    {
                        EquipWeapons equipWeapons = SelectableWeaponSets[index];
                        CharacterItem equipWeapon = equipWeapons.leftHand;
                        equipWeapon.amount -= amount;
                        equipWeapons.leftHand = equipWeapon;
                        SelectableWeaponSets[equipSlotIndex] = equipWeapons;
                    }
                    break;
            }

            this.FillEmptySlots();

            switch (CurrentGameInstance.playerDropItemMode)
            {
                case PlayerDropItemMode.DropOnGround:
                    // Drop item to the ground
                    droppingItem.amount = amount;
                    if (CurrentGameInstance.canPickupItemsWhichDropsByPlayersImmediately)
                        ItemDropEntity.Drop(this, RewardGivenType.PlayerDrop, droppingItem, System.Array.Empty<string>()).Forget();
                    else
                        ItemDropEntity.Drop(this, RewardGivenType.PlayerDrop, droppingItem, new string[] { Id }).Forget();
                    break;
            }
#endif
        }

        [AllRpc]
        protected virtual void RpcOnDead()
        {
            if (IsOwnerClient)
            {
                AttackComponent.CancelAttack();
                UseSkillComponent.CancelSkill();
                ReloadComponent.CancelReload();
                ClearActionStates();
            }
            if (onDead != null)
                onDead.Invoke();
        }

        [AllRpc]
        protected virtual void RpcOnRespawn()
        {
            if (IsOwnerClient)
                ClearActionStates();
            if (onRespawn != null)
                onRespawn.Invoke();
        }

        [AllRpc]
        protected virtual void RpcOnLevelUp()
        {
            PlayLevelUpEffects();
            if (onLevelUp != null)
                onLevelUp.Invoke();
        }

        protected virtual void PlayLevelUpEffects()
        {
            CharacterModel.InstantiateEffect(CurrentGameInstance.LevelUpEffects);
#if !DISABLE_ADDRESSABLES
            CharacterModel.InstantiateEffect(CurrentGameInstance.AddressableLevelUpEffects).Forget();
#endif
        }

        [ServerRpc]
        protected virtual void CmdUnSummon(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            int index = this.IndexOfSummon(objectId);
            if (index < 0)
                return;

            CharacterSummon summon = Summons[index];
            if (summon.type != SummonType.PetItem &&
                summon.type != SummonType.Custom)
                return;

            Summons.RemoveAt(index);
            summon.UnSummon(this);
#endif
        }
    }
}
