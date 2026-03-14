using ConcurrentCollections;
using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class LanRpgServerStorageHandlers : MonoBehaviour, IServerStorageHandlers
    {
        private readonly ConcurrentDictionary<StorageId, List<CharacterItem>> storageItems = new ConcurrentDictionary<StorageId, List<CharacterItem>>();
        private readonly ConcurrentDictionary<StorageId, HashSet<long>> usingStorageClients = new ConcurrentDictionary<StorageId, HashSet<long>>();
        private readonly ConcurrentHashSet<StorageId> convertingStorages = new ConcurrentHashSet<StorageId>();
        private readonly ConcurrentDictionary<long, List<UserUsingStorageData>> userUsingStorages = new ConcurrentDictionary<long, List<UserUsingStorageData>>();
        private float _lastUpdateTime = 0f;

        public void OpenStorage(long connectionId, IPlayerCharacterData playerCharacter, IActivatableEntity storageEntity, StorageId storageId)
        {
            if (!CanAccessStorage(playerCharacter, storageEntity, storageId, out UITextKeys errorMessage))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(connectionId, UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE);
                return;
            }
            // Store storage usage states
            if (!usingStorageClients.ContainsKey(storageId))
                usingStorageClients.TryAdd(storageId, new HashSet<long>());
            usingStorageClients[storageId].Add(connectionId);
            // Using storage data
            if (!userUsingStorages.TryGetValue(connectionId, out List<UserUsingStorageData> oneUserUsingStorages))
            {
                oneUserUsingStorages = new List<UserUsingStorageData>();
                userUsingStorages.TryAdd(connectionId, oneUserUsingStorages);
            }
            UserUsingStorageData usingStorage = new UserUsingStorageData()
            {
                Id = storageId,
                RequireEntity = !storageEntity.IsNull(),
                Entity = storageEntity,
            };
            if (!oneUserUsingStorages.Contains(usingStorage))
                oneUserUsingStorages.Add(usingStorage);
            // Notify storage items to client
            Storage storage = GetStorage(storageId, out uint storageObjectId);
            GameInstance.ServerGameMessageHandlers.NotifyStorageOpened(connectionId, storageId.storageType, storageId.storageOwnerId, storageObjectId, storage.weightLimit, storage.slotLimit);
            List<CharacterItem> storageItems = GetStorageItems(storageId);
            storageItems.FillEmptySlots(storage.slotLimit > 0, storage.slotLimit);
            GameInstance.ServerGameMessageHandlers.NotifyStorageItems(connectionId, storageId.storageType, storageId.storageOwnerId, storageItems);
        }

        public void CloseStorage(long connectionId, StorageId storageId)
        {
            if (!usingStorageClients.ContainsKey(storageId))
                return;
            usingStorageClients[storageId].Remove(connectionId);
            if (userUsingStorages.TryGetValue(connectionId, out List<UserUsingStorageData> oneUserUsingStorages))
            {
                for (int i = oneUserUsingStorages.Count - 1; i >= 0; --i)
                {
                    if (oneUserUsingStorages[i].Id.Equals(storageId))
                    {
                        oneUserUsingStorages.RemoveAt(i);
                        break;
                    }
                }
            }
            GameInstance.ServerGameMessageHandlers.NotifyStorageClosed(connectionId, storageId.storageType, storageId.storageOwnerId);
        }

        public void CloseAllStorages(long connectionId)
        {
            if (!userUsingStorages.TryGetValue(connectionId, out List<UserUsingStorageData> oneUserUsingStorages))
                return;
            for (int i = oneUserUsingStorages.Count - 1; i >= 0; --i)
            {
                CloseStorage(connectionId, oneUserUsingStorages[i].Id);
            }
        }

        private void Update()
        {
            float time = Time.unscaledTime;
            // Update every seconds
            if (time - _lastUpdateTime < 1f)
                return;
            _lastUpdateTime = time;
            List<long> connectionIds = new List<long>(userUsingStorages.Keys);
            foreach (long connectionId in connectionIds)
            {
                if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(connectionId, out IPlayerCharacterData playerCharacter))
                {
                    CloseAllStorages(connectionId);
                    continue;
                }
                Vector3 currentPosition = playerCharacter.CurrentPosition;
                if (playerCharacter is BasePlayerCharacterEntity entity)
                    currentPosition = entity.EntityTransform.position;
                if (userUsingStorages.TryGetValue(connectionId, out List<UserUsingStorageData> oneUserUsingStorages))
                {
                    // Looking for far entities and close the storage
                    for (int i = oneUserUsingStorages.Count - 1; i >= 0; --i)
                    {
                        UserUsingStorageData oneUserUsingStorage = oneUserUsingStorages[i];
                        if (!oneUserUsingStorage.RequireEntity)
                            continue;
                        if (oneUserUsingStorage.Entity.IsNull() || Vector3.Distance(playerCharacter.CurrentPosition, oneUserUsingStorage.Entity.EntityTransform.position) > oneUserUsingStorage.Entity.GetActivatableDistance())
                            CloseStorage(connectionId, oneUserUsingStorage.Id);
                    }
                }
            }
        }

        public UniTask<bool> ConvertStorageItems(StorageId storageId, List<StorageConvertItemsEntry> convertItems, List<CharacterItem> droppingItems)
        {
            if (convertingStorages.Contains(storageId))
            {
                return new UniTask<bool>(false);
            }
            convertingStorages.Add(storageId);
            // Prepare storage data
            Storage storage = GetStorage(storageId, out _);
            bool isLimitWeight = storage.weightLimit > 0;
            bool isLimitSlot = storage.slotLimit > 0;
            int weightLimit = storage.weightLimit;
            int slotLimit = storage.slotLimit;
            // Prepare storage items
            List<CharacterItem> storageItems = new List<CharacterItem>(GetStorageItems(storageId));
            for (int i = 0; i < convertItems.Count; ++i)
            {
                int dataId = convertItems[i].dataId;
                int amount = convertItems[i].amount;
                int convertedDataId = convertItems[i].convertedDataId;
                int convertedAmount = convertItems[i].convertedAmount;
                // Decrease item from storage
                if (!storageItems.DecreaseItems(dataId, amount, isLimitSlot, out _))
                    continue;
                // Increase item to storage
                if (GameInstance.Items.ContainsKey(convertedDataId) && convertedAmount > 0)
                {
                    // Increase item to storage
                    CharacterItem droppingItem = CharacterItem.Create(convertedDataId, 1, convertedAmount);
                    if (!storageItems.IncreasingItemsWillOverwhelming(convertedDataId, convertedAmount, isLimitWeight, weightLimit, storageItems.GetTotalItemWeight(), isLimitSlot, slotLimit))
                    {
                        storageItems.IncreaseItems(droppingItem);
                    }
                    else
                    {
                        droppingItems.Add(droppingItem);
                    }
                }
            }
            // Update slots
            storageItems.FillEmptySlots(isLimitSlot, slotLimit);
            SetStorageItems(storageId, storageItems);
            NotifyStorageItemsUpdated(storageId.storageType, storageId.storageOwnerId);
            return new UniTask<bool>(true);
        }

        public List<CharacterItem> GetStorageItems(StorageId storageId)
        {
            if (!storageItems.ContainsKey(storageId))
                storageItems.TryAdd(storageId, new List<CharacterItem>());
            return storageItems[storageId];
        }

        public void SetStorageItems(StorageId storageId, List<CharacterItem> items)
        {
            if (!storageItems.ContainsKey(storageId))
                storageItems.TryAdd(storageId, new List<CharacterItem>());
            storageItems[storageId] = items;
        }

        public Storage GetStorage(StorageId storageId, out uint objectId)
        {
            objectId = 0;
            Storage storage = default;
            switch (storageId.storageType)
            {
                case StorageType.Player:
                    storage = GameInstance.Singleton.playerStorage;
                    break;
                case StorageType.Guild:
                    storage = GameInstance.Singleton.guildStorage;
                    break;
                case StorageType.Building:
                    if (GameInstance.ServerBuildingHandlers.TryGetBuilding(storageId.storageOwnerId, out StorageEntity buildingEntity))
                    {
                        objectId = buildingEntity.ObjectId;
                        storage = buildingEntity.Storage;
                    }
                    break;
            }
            return storage;
        }

        public bool CanAccessStorage(IPlayerCharacterData playerCharacter, IActivatableEntity storageEntity, StorageId storageId, out UITextKeys uiTextKeys)
        {
            if (!playerCharacter.CanAccessStorage(storageId))
            {
                uiTextKeys = UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE;
                return false;
            }

            if (storageEntity.IsNull())
            {
                // TODO: May add an options or rules to allow to open storage without storage entity needed
                uiTextKeys = UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR;
                return false;
            }

            Vector3 currentPosition = playerCharacter.CurrentPosition;
            if (playerCharacter is BasePlayerCharacterEntity entity)
                currentPosition = entity.EntityTransform.position;
            if (Vector3.Distance(currentPosition, storageEntity.EntityTransform.position) > storageEntity.GetActivatableDistance())
            {
                uiTextKeys = UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR;
                return false;
            }

            uiTextKeys = UITextKeys.NONE;
            return true;
        }

        public bool IsStorageEntityOpen(StorageEntity storageEntity)
        {
            if (storageEntity == null)
                return false;
            StorageId id = new StorageId(StorageType.Building, storageEntity.Id);
            return usingStorageClients.ContainsKey(id) && usingStorageClients[id].Count > 0;
        }

        public List<CharacterItem> GetStorageEntityItems(StorageEntity storageEntity)
        {
            if (storageEntity == null)
                return new List<CharacterItem>();
            return GetStorageItems(new StorageId(StorageType.Building, storageEntity.Id));
        }

        public void ClearStorage()
        {
            foreach (var collection in storageItems.Values)
            {
                collection.Clear();
            }
            storageItems.Clear();

            foreach (var collection in usingStorageClients.Values)
            {
                collection.Clear();
            }
            usingStorageClients.Clear();

            foreach (var collection in userUsingStorages.Values)
            {
                collection.Clear();
            }
            userUsingStorages.Clear();
        }

        public void NotifyStorageItemsUpdated(StorageType storageType, string storageOwnerId)
        {
            StorageId storageId = new StorageId(storageType, storageOwnerId);
            if (!usingStorageClients.ContainsKey(storageId))
                return;
            GameInstance.ServerGameMessageHandlers.NotifyStorageItemsToClients(usingStorageClients[storageId], storageType, storageOwnerId, GetStorageItems(storageId));
        }

        public IDictionary<StorageId, List<CharacterItem>> GetAllStorageItems()
        {
            return storageItems;
        }

        public bool WillProceedStorageSaving(StorageType storageType, string storageOwnerId)
        {
            if (storageType == StorageType.Building && BaseGameNetworkManager.Singleton.IsInstanceMap())
                return false;
            return true;
        }
    }
}
