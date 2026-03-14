using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static partial class CharacterInventoryExtensions
    {
        public static bool EnhanceSocketItem(this IPlayerCharacterData character, InventoryType inventoryType, int index, byte equipSlotIndex, int enhancerId, int socketIndex, out UITextKeys gameMessage)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            switch (inventoryType)
            {
                case InventoryType.NonEquipItems:
                    return character.EnhanceSocketNonEquipItem(index, enhancerId, socketIndex, out gameMessage);
                case InventoryType.EquipItems:
                    return character.EnhanceSocketEquipItem(index, enhancerId, socketIndex, out gameMessage);
                case InventoryType.EquipWeaponRight:
                    return character.EnhanceSocketRightHandItem(equipSlotIndex, enhancerId, socketIndex, out gameMessage);
                case InventoryType.EquipWeaponLeft:
                    return character.EnhanceSocketLeftHandItem(equipSlotIndex, enhancerId, socketIndex, out gameMessage);
            }
            gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_DATA;
            return false;
#else
            gameMessage = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE;
            return false;
#endif
        }

        public static bool RemoveEnhancerFromItem(this IPlayerCharacterData character, InventoryType inventoryType, int index, byte equipSlotIndex, int socketIndex, out UITextKeys gameMessage)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            bool returnEnhancer = GameInstance.Singleton.enhancerRemoval.ReturnEnhancerItem;
            switch (inventoryType)
            {
                case InventoryType.NonEquipItems:
                    return character.RemoveEnhancerFromNonEquipItem(index, socketIndex, returnEnhancer, out gameMessage);
                case InventoryType.EquipItems:
                    return character.RemoveEnhancerFromEquipItem(index, socketIndex, returnEnhancer, out gameMessage);
                case InventoryType.EquipWeaponRight:
                    return character.RemoveEnhancerFromRightHandItem(equipSlotIndex, socketIndex, returnEnhancer, out gameMessage);
                case InventoryType.EquipWeaponLeft:
                    return character.RemoveEnhancerFromLeftHandItem(equipSlotIndex, socketIndex, returnEnhancer, out gameMessage);
            }
            gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_DATA;
            return false;
#else
            gameMessage = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE;
            return false;
#endif
        }

        public static bool EnhanceSocketRightHandItem(this IPlayerCharacterData character, byte equipSlotIndex, int enhancerId, int socketIndex, out UITextKeys gameMessage)
        {
            return EnhanceSocketItem(character, character.SelectableWeaponSets[equipSlotIndex].rightHand, enhancerId, socketIndex, (enhancedSocketItem) =>
            {
                EquipWeapons equipWeapon = character.SelectableWeaponSets[equipSlotIndex];
                equipWeapon.rightHand = enhancedSocketItem;
                character.SelectableWeaponSets[equipSlotIndex] = equipWeapon;
            }, out gameMessage);
        }

        public static bool EnhanceSocketLeftHandItem(this IPlayerCharacterData character, byte equipSlotIndex, int enhancerId, int socketIndex, out UITextKeys gameMessage)
        {
            return EnhanceSocketItem(character, character.SelectableWeaponSets[equipSlotIndex].leftHand, enhancerId, socketIndex, (enhancedSocketItem) =>
            {
                EquipWeapons equipWeapon = character.SelectableWeaponSets[equipSlotIndex];
                equipWeapon.leftHand = enhancedSocketItem;
                character.SelectableWeaponSets[equipSlotIndex] = equipWeapon;
            }, out gameMessage);
        }

        public static bool EnhanceSocketEquipItem(this IPlayerCharacterData character, int index, int enhancerId, int socketIndex, out UITextKeys gameMessage)
        {
            return EnhanceSocketItemByList(character, character.EquipItems, index, enhancerId, socketIndex, out gameMessage);
        }

        public static bool EnhanceSocketNonEquipItem(this IPlayerCharacterData character, int index, int enhancerId, int socketIndex, out UITextKeys gameMessage)
        {
            return EnhanceSocketItemByList(character, character.NonEquipItems, index, enhancerId, socketIndex, out gameMessage);
        }

        private static bool EnhanceSocketItemByList(IPlayerCharacterData character, IList<CharacterItem> list, int index, int enhancerId, int socketIndex, out UITextKeys gameMessage)
        {
            return EnhanceSocketItem(character, list[index], enhancerId, socketIndex, (enhancedSocketItem) =>
            {
                list[index] = enhancedSocketItem;
            }, out gameMessage);
        }

        private static bool EnhanceSocketItem(IPlayerCharacterData character, CharacterItem enhancingItem, int enhancerId, int socketIndex, System.Action<CharacterItem> onEnhanceSocket, out UITextKeys gameMessage)
        {
            gameMessage = UITextKeys.NONE;
            if (enhancingItem.IsEmptySlot())
            {
                // Cannot enhance socket because character item is empty
                gameMessage = UITextKeys.UI_ERROR_ITEM_NOT_FOUND;
                return false;
            }
            IEquipmentItem equipmentItem = enhancingItem.GetEquipmentItem();
            if (equipmentItem == null)
            {
                // Cannot enhance socket because it's not equipment item
                gameMessage = UITextKeys.UI_ERROR_ITEM_NOT_EQUIPMENT;
                return false;
            }
            int maxSocket = GameInstance.Singleton.GameplayRule.GetItemMaxSocket(character, enhancingItem);
            if (maxSocket <= 0)
            {
                // Cannot enhance socket because equipment has no socket(s)
                gameMessage = UITextKeys.UI_ERROR_NO_EMPTY_SOCKET;
                return false;
            }
            while (enhancingItem.sockets.Count < maxSocket)
            {
                // Add empty slots
                enhancingItem.sockets.Add(0);
            }
            if (socketIndex >= 0)
            {
                // Put enhancer to target socket
                if (socketIndex >= enhancingItem.sockets.Count || enhancingItem.sockets[socketIndex] != 0)
                {
                    gameMessage = UITextKeys.UI_ERROR_SOCKET_NOT_EMPTY;
                    return false;
                }
            }
            else
            {
                // Put enhancer to any empty socket
                for (int index = 0; index < enhancingItem.sockets.Count; ++index)
                {
                    if (enhancingItem.sockets[index] == 0)
                    {
                        socketIndex = index;
                        break;
                    }
                    if (index == enhancingItem.sockets.Count - 1)
                    {
                        gameMessage = UITextKeys.UI_ERROR_NO_EMPTY_SOCKET;
                        return false;
                    }
                }
            }
            BaseItem enhancerItem;
            if (!GameInstance.Items.TryGetValue(enhancerId, out enhancerItem) || !enhancerItem.IsSocketEnhancer())
            {
                // Cannot enhance socket because enhancer id is invalid
                gameMessage = UITextKeys.UI_ERROR_CANNOT_ENHANCE_SOCKET;
                return false;
            }
            ISocketEnhancerItem castedEnhancerItem = enhancerItem as ISocketEnhancerItem;
            if (castedEnhancerItem.SocketEnhancerType != equipmentItem.AvailableSocketEnhancerTypes[socketIndex])
            {
                // Cannot enhance socket because it is not a valid socket
                gameMessage = UITextKeys.UI_ERROR_CANNOT_ENHANCE_SOCKET;
                return false;
            }
            if (!character.HasOneInNonEquipItems(enhancerId))
            {
                // Cannot enhance socket because there is no item
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_SOCKET_ENCHANER;
                return false;
            }
            character.DecreaseItems(enhancerId, 1);
            // Fill empty slots
            character.FillEmptySlots();
            // Update enhanced item
            enhancingItem.sockets[socketIndex] = enhancerId;
            onEnhanceSocket.Invoke(enhancingItem);
            GameInstance.ServerLogHandlers.LogEnhanceSocketItem(character, enhancingItem, enhancerItem);
            return true;
        }

        public static bool RemoveEnhancerFromRightHandItem(this IPlayerCharacterData character, byte equipSlotIndex, int socketIndex, bool returnEnhancer, out UITextKeys gameMessage)
        {
            return RemoveEnhancerFromItem(character, character.SelectableWeaponSets[equipSlotIndex].rightHand, socketIndex, returnEnhancer, (enhancedSocketItem) =>
            {
                EquipWeapons equipWeapon = character.SelectableWeaponSets[equipSlotIndex];
                equipWeapon.rightHand = enhancedSocketItem;
                character.SelectableWeaponSets[equipSlotIndex] = equipWeapon;
            }, out gameMessage);
        }

        public static bool RemoveEnhancerFromLeftHandItem(this IPlayerCharacterData character, byte equipSlotIndex, int socketIndex, bool returnEnhancer, out UITextKeys gameMessage)
        {
            return RemoveEnhancerFromItem(character, character.SelectableWeaponSets[equipSlotIndex].leftHand, socketIndex, returnEnhancer, (enhancedSocketItem) =>
            {
                EquipWeapons equipWeapon = character.SelectableWeaponSets[equipSlotIndex];
                equipWeapon.leftHand = enhancedSocketItem;
                character.SelectableWeaponSets[equipSlotIndex] = equipWeapon;
            }, out gameMessage);
        }

        public static bool RemoveEnhancerFromEquipItem(this IPlayerCharacterData character, int index, int socketIndex, bool returnEnhancer, out UITextKeys gameMessage)
        {
            return RemoveEnhancerFromItemByList(character, character.EquipItems, index, socketIndex, returnEnhancer, out gameMessage);
        }

        public static bool RemoveEnhancerFromNonEquipItem(this IPlayerCharacterData character, int index, int socketIndex, bool returnEnhancer, out UITextKeys gameMessage)
        {
            return RemoveEnhancerFromItemByList(character, character.NonEquipItems, index, socketIndex, returnEnhancer, out gameMessage);
        }

        private static bool RemoveEnhancerFromItemByList(IPlayerCharacterData character, IList<CharacterItem> list, int index, int socketIndex, bool returnEnhancer, out UITextKeys gameMessage)
        {
            return RemoveEnhancerFromItem(character, list[index], socketIndex, returnEnhancer, (enhancedSocketItem) =>
            {
                list[index] = enhancedSocketItem;
            }, out gameMessage);
        }

        private static bool RemoveEnhancerFromItem(IPlayerCharacterData character, CharacterItem enhancedItem, int socketIndex, bool returnEnhancer, System.Action<CharacterItem> onRemoveEnhancer, out UITextKeys gameMessage)
        {
            gameMessage = UITextKeys.NONE;
            if (enhancedItem.IsEmptySlot())
            {
                gameMessage = UITextKeys.UI_ERROR_ITEM_NOT_FOUND;
                return false;
            }
            if (enhancedItem.sockets.Count == 0 || socketIndex >= enhancedItem.sockets.Count)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ENHANCER_ITEM_INDEX;
                return false;
            }
            if (enhancedItem.sockets[socketIndex] == 0)
            {
                gameMessage = UITextKeys.UI_ERROR_NO_ENHANCER;
                return false;
            }
            if (!GameInstance.Singleton.enhancerRemoval.CanRemove(character, out gameMessage))
                return false;
            int enhancerId = enhancedItem.sockets[socketIndex];
            BaseItem enhancerItem;
            if (!GameInstance.Items.TryGetValue(enhancerId, out enhancerItem))
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_DATA;
                return false;
            }
            bool inventoryChanged = false;
            if (returnEnhancer)
            {
                if (character.IncreasingItemsWillOverwhelming(enhancerId, 1))
                {
                    gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                    return false;
                }
                // Update enhanced item, before make changes to other items
                enhancedItem.sockets[socketIndex] = 0;
                onRemoveEnhancer.Invoke(enhancedItem);
                character.IncreaseItems(CharacterItem.Create(enhancerId));
                inventoryChanged = true;
            }
            else
            {
                // Update enhanced item, before make changes to other items
                enhancedItem.sockets[socketIndex] = 0;
                onRemoveEnhancer.Invoke(enhancedItem);
            }
            if (GameInstance.Singleton.enhancerRemoval.RequireItems != null)
            {
                // Decrease required items
                character.DecreaseItems(GameInstance.Singleton.enhancerRemoval.RequireItems);
                inventoryChanged = true;
            }
            // Fill empty slots
            if (inventoryChanged)
                character.FillEmptySlots();
            // Decrease required gold
            GameInstance.Singleton.GameplayRule.DecreaseCurrenciesWhenRemoveEnhancer(character);
            GameInstance.ServerLogHandlers.LogRemoveEnhancerFromItem(character, enhancedItem, enhancerItem);
            return true;
        }
    }
}
