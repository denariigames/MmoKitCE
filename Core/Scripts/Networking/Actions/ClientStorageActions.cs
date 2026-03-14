using LiteNetLibManager;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static class ClientStorageActions
    {
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseOpenStorageMessage> onResponseOpenStorage;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseCloseStorageMessage> onResponseCloseStorage;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseMoveItemFromStorageMessage> onResponseMoveItemFromStorage;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseMoveItemToStorageMessage> onResponseMoveItemToStorage;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSwapOrMergeStorageItemMessage> onResponseSwapOrMergeStorageItem;
        public static event System.Action<StorageType, string, uint, int, int> onNotifyStorageOpened;
        public static event System.Action<StorageType, string> onNotifyStorageClosed;
        public static event System.Action<StorageType, string, List<CharacterItem>> onNotifyStorageItemsUpdated;

        public static void Clean()
        {
            onResponseOpenStorage = null;
            onResponseCloseStorage = null;
            onResponseMoveItemFromStorage = null;
            onResponseMoveItemToStorage = null;
            onResponseSwapOrMergeStorageItem = null;
            onNotifyStorageOpened = null;
            onNotifyStorageClosed = null;
            onNotifyStorageItemsUpdated = null;
        }

        public static void ResponseOpenStorage(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseOpenStorageMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseOpenStorage != null)
                onResponseOpenStorage.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseCloseStorage(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCloseStorageMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseCloseStorage != null)
                onResponseCloseStorage.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseMoveItemFromStorage(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseMoveItemFromStorageMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseMoveItemFromStorage != null)
                onResponseMoveItemFromStorage.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseMoveItemToStorage(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseMoveItemToStorageMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseMoveItemToStorage != null)
                onResponseMoveItemToStorage.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseSwapOrMergeStorageItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSwapOrMergeStorageItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSwapOrMergeStorageItem != null)
                onResponseSwapOrMergeStorageItem.Invoke(requestHandler, responseCode, response);
        }

        public static void NotifyStorageOpened(StorageType type, string ownerId, uint objectId, int weightLimit, int slotLimit)
        {
            StorageId storageId = new StorageId(type, ownerId);
            if (!GameInstance.OpenedStorages.ContainsKey(storageId))
                GameInstance.OpenedStorages.Add(storageId, new List<CharacterItem>());
            GameInstance.ItemUIVisibilityManager.ShowStorageDialog(type, ownerId, objectId, weightLimit, slotLimit);
            if (onNotifyStorageOpened != null)
                onNotifyStorageOpened.Invoke(type, ownerId, objectId, weightLimit, slotLimit);
        }

        public static void NotifyStorageClosed(StorageType type, string ownerId)
        {
            StorageId storageId = new StorageId(type, ownerId);
            GameInstance.OpenedStorages.Remove(storageId);
            GameInstance.ItemUIVisibilityManager.HideStorageDialog(type, ownerId);
            if (onNotifyStorageClosed != null)
                onNotifyStorageClosed.Invoke(type, ownerId);
        }

        public static void NotifyStorageItemsUpdated(StorageType type, string ownerId, List<CharacterItem> storageItems)
        {
            StorageId storageId = new StorageId(type, ownerId);
            if (onNotifyStorageItemsUpdated != null)
                onNotifyStorageItemsUpdated.Invoke(type, ownerId, storageItems);
            if (GameInstance.OpenedStorages.ContainsKey(storageId))
                GameInstance.OpenedStorages[storageId] = storageItems;
        }
    }
}
