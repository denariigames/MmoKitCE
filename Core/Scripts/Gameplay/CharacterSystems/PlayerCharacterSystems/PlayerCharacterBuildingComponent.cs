using Insthync.AddressableAssetTools;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public partial class PlayerCharacterBuildingComponent : BaseNetworkedGameEntityComponent<BasePlayerCharacterEntity>
    {
        private IPhysicFunctions _detectCostructionAreaPhysicFunctions;

        private void Awake()
        {
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                _detectCostructionAreaPhysicFunctions = new PhysicFunctions(8);
            else
                _detectCostructionAreaPhysicFunctions = new PhysicFunctions2D(8);
        }

        public bool CallCmdConstructBuilding(int itemIndex, Vector3 position, Vector3 rotation, uint parentObjectId)
        {
            if (!Entity.CanDoActions())
                return false;
            RPC(CmdConstructBuilding, itemIndex, position, rotation, parentObjectId);
            return true;
        }

        [ServerRpc]
        protected void CmdConstructBuilding(int itemIndex, Vector3 position, Vector3 rotation, uint parentObjectId)
        {
            CmdConstructBuildingBody(itemIndex, position, rotation, parentObjectId);
        }

        protected virtual async void CmdConstructBuildingBody(int itemIndex, Vector3 position, Vector3 rotation, uint parentObjectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
            {
                // Not allow to do it
                return;
            }

            if (itemIndex >= Entity.NonEquipItems.Count)
            {
                // Invalid data index
                return;
            }

            if (Entity.IsInSafeArea)
            {
                // Unable to build in safe area
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_IN_SAFE_AREA);
                return;
            }

            int overlapCount = _detectCostructionAreaPhysicFunctions.OverlapObjects(position, Vector3.Distance(position, EntityTransform.position), 1 << PhysicLayers.IgnoreRaycast, false, QueryTriggerInteraction.Collide);
            for (int i = 0; i < overlapCount; ++i)
            {
                if (_detectCostructionAreaPhysicFunctions.GetOverlapObject(i).TryGetComponent<SafeArea>(out _))
                {
                    // Unable to build in safe area
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_IN_SAFE_AREA);
                    return;
                }
            }

            CharacterItem nonEquipItem = Entity.NonEquipItems[itemIndex];
            if (nonEquipItem.IsEmptySlot())
            {
                // Invalid data
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_BUILDING_DATA);
                return;
            }

            IBuildingItem buildingItem = nonEquipItem.GetBuildingItem();
            if (buildingItem == null)
            {
                // Invalid data
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_BUILDING_DATA);
                return;
            }

            BuildingEntity tempPrefab = buildingItem.BuildingEntity;
#if !DISABLE_ADDRESSABLES
            AssetReferenceBuildingEntity tempAddressablePrefab = buildingItem.AddressableBuildingEntity;
#endif
            BuildingEntity loadedPrefab;
#if !DISABLE_ADDRESSABLES
            loadedPrefab = await tempAddressablePrefab.GetOrLoadAssetAsyncOrUsePrefab(tempPrefab);
#else
            loadedPrefab = tempPrefab;
#endif

            if (loadedPrefab == null)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_BUILDING_ENTITY);
                return;
            }

            if (loadedPrefab.BuildLimit > 0 && GameInstance.ServerBuildingHandlers.CountPlayerBuildings(Entity.Id, loadedPrefab.EntityId) >= loadedPrefab.BuildLimit)
            {
                // Reached limit amount
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_REACHED_BUILD_LIMIT);
                return;
            }

            if (!Entity.DecreaseItemsByIndex(itemIndex, 1, false))
            {
                // Not enough items?
                return;
            }

            Entity.FillEmptySlots();

            // Create the building
            BuildingSaveData buildingSaveData = new BuildingSaveData();
            buildingSaveData.Id = GenericUtils.GetUniqueId();
            buildingSaveData.ParentId = string.Empty;
            if (Manager.TryGetEntityByObjectId(parentObjectId, out BuildingEntity parentBuildingEntity))
                buildingSaveData.ParentId = parentBuildingEntity.Id;
            buildingSaveData.EntityId = loadedPrefab.EntityId;
            buildingSaveData.CurrentHp = loadedPrefab.MaxHp;
            buildingSaveData.RemainsLifeTime = loadedPrefab.LifeTime;
            buildingSaveData.Position = position;
            buildingSaveData.Rotation = rotation;
            buildingSaveData.CreatorId = Entity.Id;
            buildingSaveData.CreatorName = Entity.CharacterName;
            await CurrentGameManager.CreateBuildingEntity(buildingSaveData, false);
#endif
        }

        public bool CallCmdRepairBuilding(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdRepairBuilding, objectId);
            return true;
        }

        [ServerRpc]
        protected void CmdRepairBuilding(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out BuildingEntity buildingEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(buildingEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!buildingEntity.Repair(Entity, out UITextKeys errorMessage))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, errorMessage);
                return;
            }
#endif
        }
        
        public bool CallCmdDestroyBuilding(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdDestroyBuilding, objectId);
            return true;
        }

        [ServerRpc]
        protected void CmdDestroyBuilding(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out BuildingEntity buildingEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(buildingEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!buildingEntity.IsCreator(Entity))
            {
                // Character is not the creator
                return;
            }

            buildingEntity.Destroy();
#endif
        }

        public bool CallCmdOpenStorage(uint objectId, string password)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdOpenStorage, objectId, password);
            return true;
        }

        [ServerRpc]
        protected void CmdOpenStorage(uint objectId, string password)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out StorageEntity storageEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(storageEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (storageEntity.Lockable && storageEntity.IsLocked && !string.IsNullOrEmpty(storageEntity.LockPassword) && !string.Equals(storageEntity.LockPassword, password))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_WRONG_BUILDING_PASSWORD);
                return;
            }

            StorageId storageId;
            if (!Entity.GetStorageId(StorageType.Building, objectId, out storageId))
            {
                // Wrong storage type or relative data
                return;
            }

            GameInstance.ServerStorageHandlers.OpenStorage(ConnectionId, Entity, storageEntity, storageId);
#endif
        }

        public bool CallCmdOpenDoor(uint objectId, string password)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdOpenDoor, objectId, password);
            return true;
        }

        [ServerRpc]
        protected void CmdOpenDoor(uint objectId, string password)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out DoorEntity doorEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(doorEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (doorEntity.Lockable && doorEntity.IsLocked && !string.IsNullOrEmpty(doorEntity.LockPassword) && !string.Equals(doorEntity.LockPassword, password))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_WRONG_BUILDING_PASSWORD);
                return;
            }

            doorEntity.IsOpen = true;
#endif
        }

        public bool CallCmdCloseDoor(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdCloseDoor, objectId);
            return true;
        }

        [ServerRpc]
        protected void CmdCloseDoor(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out DoorEntity doorEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(doorEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            doorEntity.IsOpen = false;
#endif
        }

        public bool CallCmdTurnOnCampFire(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdTurnOnCampFire, objectId);
            return true;
        }

        [ServerRpc]
        protected void CmdTurnOnCampFire(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out CampFireEntity campfireEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(campfireEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            campfireEntity.TurnOn();
#endif
        }

        public bool CallCmdTurnOffCampFire(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdTurnOffCampFire, objectId);
            return true;
        }

        [ServerRpc]
        protected void CmdTurnOffCampFire(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out CampFireEntity campfireEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(campfireEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            campfireEntity.TurnOff();
#endif
        }

        public bool CallCmdCraftItemByWorkbench(uint objectId, int dataId)
        {
            RPC(CmdCraftItemByWorkbench, objectId, dataId);
            return true;
        }

        [ServerRpc]
        protected void CmdCraftItemByWorkbench(uint objectId, int dataId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out WorkbenchEntity workbenchEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(workbenchEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            workbenchEntity.CraftItem(Entity, dataId);
#endif
        }

        public bool CallCmdSetBuildingPassword(uint objectId, string password)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdSetBuildingPassword, objectId, password);
            return true;
        }

        [ServerRpc]
        protected void CmdSetBuildingPassword(uint objectId, string password)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out BuildingEntity buildingEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(buildingEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!buildingEntity.IsCreator(Entity))
            {
                // Character is not the creator
                return;
            }

            if (!buildingEntity.Lockable)
            {
                // It's not lockable building
                return;
            }

            buildingEntity.LockPassword = password;
            buildingEntity.IsLocked = true;
#endif
        }

        public bool CallCmdLockBuilding(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdLockBuilding, objectId);
            return true;
        }

        [ServerRpc]
        protected void CmdLockBuilding(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out BuildingEntity buildingEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(buildingEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!buildingEntity.IsCreator(Entity))
            {
                // Character is not the creator
                return;
            }

            if (!buildingEntity.Lockable)
            {
                // It's not lockable building
                return;
            }

            buildingEntity.IsLocked = true;
#endif
        }

        public bool CallCmdUnlockBuilding(uint objectId)
        {
            if (!CurrentGameplayRule.CanInteractEntity(Entity, objectId))
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return false;
            }
            RPC(CmdUnlockBuilding, objectId);
            return true;
        }

        [ServerRpc]
        protected void CmdUnlockBuilding(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out BuildingEntity buildingEntity))
            {
                // Can't find the entity
                return;
            }

            if (!Entity.IsGameEntityInDistance(buildingEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            if (!buildingEntity.IsCreator(Entity))
            {
                // Character is not the creator
                return;
            }

            if (!buildingEntity.Lockable)
            {
                // It's not lockable building
                return;
            }

            buildingEntity.IsLocked = false;
#endif
        }
    }
}
