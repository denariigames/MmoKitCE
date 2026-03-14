using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class LanRpgServerStorageMessageHandlers : MonoBehaviour, IServerStorageMessageHandlers
    {
        public UniTaskVoid HandleRequestOpenStorage(RequestHandlerData requestHandler, RequestOpenStorageMessage request, RequestProceedResultDelegate<ResponseOpenStorageMessage> result)
        {
            if (request.storageType == StorageType.None)
            {
                result.InvokeError(new ResponseOpenStorageMessage()
                {
                    message = UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE,
                });
                return default;
            }
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseOpenStorageMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (request.storageType == StorageType.Guild)
            {
                if (!GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out GuildData guildData) || !guildData.CanUseStorage(playerCharacter.Id))
                {
                    result.InvokeError(new ResponseOpenStorageMessage()
                    {
                        message = UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE,
                    });
                    return default;
                }
            }
            if (!playerCharacter.GetStorageId(request.storageType, 0, out StorageId storageId))
            {
                result.InvokeError(new ResponseOpenStorageMessage()
                {
                    message = UITextKeys.UI_ERROR_STORAGE_NOT_FOUND,
                });
                return default;
            }
            GameInstance.ServerStorageHandlers.OpenStorage(requestHandler.ConnectionId, playerCharacter, null, storageId);
            result.InvokeSuccess(new ResponseOpenStorageMessage());
            return default;
        }

        public UniTaskVoid HandleRequestCloseStorage(RequestHandlerData requestHandler, EmptyMessage request, RequestProceedResultDelegate<ResponseCloseStorageMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseCloseStorageMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            GameInstance.ServerStorageHandlers.CloseAllStorages(requestHandler.ConnectionId);
            result.InvokeSuccess(new ResponseCloseStorageMessage());
            return default;
        }

        public UniTaskVoid HandleRequestMoveItemFromStorage(RequestHandlerData requestHandler, RequestMoveItemFromStorageMessage request, RequestProceedResultDelegate<ResponseMoveItemFromStorageMessage> result)
        {
            StorageId storageId = new StorageId(request.storageType, request.storageOwnerId);
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseMoveItemFromStorageMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (!playerCharacter.CanAccessStorage(storageId))
            {
                result.InvokeError(new ResponseMoveItemFromStorageMessage()
                {
                    message = UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE,
                });
                return default;
            }

            // Get items from storage
            List<CharacterItem> storageItems = GameInstance.ServerStorageHandlers.GetStorageItems(storageId);

            // Prepare storage data
            Storage storage = GameInstance.ServerStorageHandlers.GetStorage(storageId, out _);
            bool isLimitWeight = storage.weightLimit > 0;
            bool isLimitSlot = storage.slotLimit > 0;
            int weightLimit = storage.weightLimit;
            int slotLimit = storage.slotLimit;
            if (!playerCharacter.MoveItemFromStorage(storageId, isLimitWeight, weightLimit, isLimitSlot, slotLimit, storageItems, request.storageItemIndex, request.storageItemAmount, request.inventoryType, request.inventoryItemIndex, request.equipSlotIndexOrWeaponSet, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseMoveItemFromStorageMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            GameInstance.ServerStorageHandlers.SetStorageItems(storageId, storageItems);
            GameInstance.ServerStorageHandlers.NotifyStorageItemsUpdated(request.storageType, request.storageOwnerId);
            // Success
            result.InvokeSuccess(new ResponseMoveItemFromStorageMessage());
            return default;
        }

        public UniTaskVoid HandleRequestMoveItemToStorage(RequestHandlerData requestHandler, RequestMoveItemToStorageMessage request, RequestProceedResultDelegate<ResponseMoveItemToStorageMessage> result)
        {
            StorageId storageId = new StorageId(request.storageType, request.storageOwnerId);
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseMoveItemToStorageMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (!playerCharacter.CanAccessStorage(storageId))
            {
                result.InvokeError(new ResponseMoveItemToStorageMessage()
                {
                    message = UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE,
                });
                return default;
            }

            // Get items from storage
            List<CharacterItem> storageItems = GameInstance.ServerStorageHandlers.GetStorageItems(storageId);

            // Prepare storage data
            Storage storage = GameInstance.ServerStorageHandlers.GetStorage(storageId, out _);
            bool isLimitWeight = storage.weightLimit > 0;
            bool isLimitSlot = storage.slotLimit > 0;
            int weightLimit = storage.weightLimit;
            int slotLimit = storage.slotLimit;
            if (!playerCharacter.MoveItemToStorage(storageId, isLimitWeight, weightLimit, isLimitSlot, slotLimit, storageItems, request.storageItemIndex, request.inventoryType, request.inventoryItemIndex, request.inventoryItemAmount, request.equipSlotIndexOrWeaponSet, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseMoveItemToStorageMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            GameInstance.ServerStorageHandlers.SetStorageItems(storageId, storageItems);
            GameInstance.ServerStorageHandlers.NotifyStorageItemsUpdated(request.storageType, request.storageOwnerId);
            // Success
            result.InvokeSuccess(new ResponseMoveItemToStorageMessage());
            return default;
        }

        public UniTaskVoid HandleRequestSwapOrMergeStorageItem(RequestHandlerData requestHandler, RequestSwapOrMergeStorageItemMessage request, RequestProceedResultDelegate<ResponseSwapOrMergeStorageItemMessage> result)
        {
            StorageId storageId = new StorageId(request.storageType, request.storageOwnerId);
            int fromIndex = request.fromIndex;
            int toIndex = request.toIndex;
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSwapOrMergeStorageItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (!playerCharacter.CanAccessStorage(storageId))
            {
                result.InvokeError(new ResponseSwapOrMergeStorageItemMessage()
                {
                    message = UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE,
                });
                return default;
            }
            // Check that the character can move items or not
            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanMoveItem())
            {
                result.InvokeError(new ResponseSwapOrMergeStorageItemMessage());
                return default;
            }

            // Get items from storage
            List<CharacterItem> storageItems = GameInstance.ServerStorageHandlers.GetStorageItems(storageId);

            // Prepare storage data
            Storage storage = GameInstance.ServerStorageHandlers.GetStorage(storageId, out _);
            bool isLimitSlot = storage.slotLimit > 0;
            int slotLimit = storage.slotLimit;
            if (!playerCharacter.SwapOrMergeStorageItem(storageId, isLimitSlot, slotLimit, storageItems, fromIndex, toIndex, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseSwapOrMergeStorageItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            GameInstance.ServerStorageHandlers.SetStorageItems(storageId, storageItems);
            GameInstance.ServerStorageHandlers.NotifyStorageItemsUpdated(request.storageType, request.storageOwnerId);
            // Success
            result.InvokeSuccess(new ResponseSwapOrMergeStorageItemMessage());
            return default;
        }
    }
}
