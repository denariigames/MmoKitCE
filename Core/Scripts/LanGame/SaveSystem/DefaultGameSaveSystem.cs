using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.DEFAULT_GAME_SAVE_SYSTEM_FILE, menuName = GameDataMenuConsts.DEFAULT_GAME_SAVE_SYSTEM_MENU, order = GameDataMenuConsts.DEFAULT_GAME_SAVE_SYSTEM_ORDER)]
    public class DefaultGameSaveSystem : BaseGameSaveSystem
    {
        private readonly WorldSaveData worldSaveData = new WorldSaveData();
        private readonly SummonBuffsSaveData summonBuffsSaveData = new SummonBuffsSaveData();
        private readonly StorageSaveData worldStorageSaveData = new StorageSaveData();
        private readonly StorageSaveData playerStorageSaveData = new StorageSaveData();
        private readonly Dictionary<StorageId, List<CharacterItem>> playerStorageItems = new Dictionary<StorageId, List<CharacterItem>>();
        private bool isReadyToSave;

        public override void OnServerStart()
        {
            isReadyToSave = false;
        }

        public override async UniTask PreSpawnEntities(IPlayerCharacterData hostPlayerCharacterData, IDictionary<StorageId, List<CharacterItem>> storageItems)
        {
            isReadyToSave = false;
            // Remove all building storage items, will fill with the new ones
            List<StorageId> keys = new List<StorageId>(storageItems.Keys);
            foreach (StorageId key in keys)
            {
                switch (key.storageType)
                {
                    case StorageType.Building:
                        storageItems.Remove(key);
                        break;
                }
            }
            if (hostPlayerCharacterData != null && !string.IsNullOrEmpty(hostPlayerCharacterData.Id))
            {
                // Load and Spawn buildings
                BuildingEntity tempBuildingEntity;
                BuildingEntity[] inSceneBuildings = FindObjectsByType<BuildingEntity>(FindObjectsSortMode.None);
                Dictionary<string, BuildingEntity> initializingBuildingDicts = new Dictionary<string, BuildingEntity>();
                for (int i = 0; i < inSceneBuildings.Length; ++i)
                {
                    tempBuildingEntity = inSceneBuildings[i];
                    initializingBuildingDicts.Add(tempBuildingEntity.Id, tempBuildingEntity);
                }
                worldSaveData.LoadPersistentData(hostPlayerCharacterData.Id, BaseGameNetworkManager.CurrentMapInfo.Id);
                foreach (BuildingSaveData building in worldSaveData.buildings)
                {
                    if (building.IsSceneObject && initializingBuildingDicts.TryGetValue(building.Id, out BuildingEntity inSceneBuilding))
                    {
                        if (building.CurrentHp <= 0)
                        {
                            initializingBuildingDicts.Remove(building.Id);
                            inSceneBuilding.NetworkDestroy();
                            continue;
                        }
                        initializingBuildingDicts.Remove(building.Id);
                        inSceneBuilding.CurrentHp = building.CurrentHp;
                        GameInstance.ServerBuildingHandlers.AddBuilding(inSceneBuilding.Id, inSceneBuilding);
                    }
                    else
                    {
                        await BaseGameNetworkManager.Singleton.CreateBuildingEntity(building, true);
                    }
                }
                // Load storage data
                worldStorageSaveData.LoadPersistentData($"{hostPlayerCharacterData.Id}_world");
                StorageId storageId;
                foreach (StorageCharacterItem storageItem in worldStorageSaveData.storageItems)
                {
                    storageId = new StorageId(storageItem.storageType, storageItem.storageOwnerId);
                    if (!storageItems.ContainsKey(storageId))
                        storageItems[storageId] = new List<CharacterItem>();
                    storageItems[storageId].Add(storageItem.characterItem);
                }
                // Setup building
                foreach (BuildingEntity initializingBuilding in initializingBuildingDicts.Values)
                {
                    initializingBuilding.InitSceneObject();
                    GameInstance.ServerBuildingHandlers.AddBuilding(initializingBuilding.Id, initializingBuilding);
                }
            }
            isReadyToSave = true;
        }

        public override void SaveCharacter(IPlayerCharacterData playerCharacterData)
        {
            playerCharacterData.SavePersistentCharacterData();
        }

        public override List<PlayerCharacterData> LoadCharacters()
        {
            return PlayerCharacterDataExtensions.LoadAllPersistentCharacterData();
        }

        public override List<CharacterBuff> LoadSummonBuffs(IPlayerCharacterData playerCharacterData)
        {
            summonBuffsSaveData.LoadPersistentData(playerCharacterData.Id);
            return summonBuffsSaveData.summonBuffs;
        }

        public override List<CharacterItem> LoadPlayerStorage(IPlayerCharacterData playerCharacterData)
        {
            List<CharacterItem> result = new List<CharacterItem>();
            playerStorageItems.Clear();
            if (playerCharacterData != null && !string.IsNullOrEmpty(playerCharacterData.Id))
            {
                // Load storage data
                playerStorageSaveData.LoadPersistentData(playerCharacterData.Id);
                StorageId storageId;
                foreach (StorageCharacterItem storageItem in playerStorageSaveData.storageItems)
                {
                    storageId = new StorageId(storageItem.storageType, storageItem.storageOwnerId);
                    if (!playerStorageItems.ContainsKey(storageId))
                        playerStorageItems[storageId] = new List<CharacterItem>();
                    playerStorageItems[storageId].Add(storageItem.characterItem);
                }
                storageId = new StorageId(StorageType.Player, playerCharacterData.Id);
                if (playerStorageItems.ContainsKey(storageId))
                {
                    // Result is storage items for the character only
                    result = playerStorageItems[storageId];
                }
            }
            return result;
        }

        public override void SaveStorage(IPlayerCharacterData hostPlayerCharacterData, IDictionary<StorageId, List<CharacterItem>> storageItems)
        {
            if (!isReadyToSave)
                return;

            worldStorageSaveData.storageItems.Clear();
            foreach (StorageId storageId in storageItems.Keys)
            {
                if (storageId.storageType == StorageType.Player)
                {
                    // Player's storage will be saved in `SavePlayerStorage` function
                    continue;
                }
                foreach (CharacterItem storageItem in storageItems[storageId])
                {
                    worldStorageSaveData.storageItems.Add(new StorageCharacterItem()
                    {
                        storageType = storageId.storageType,
                        storageOwnerId = storageId.storageOwnerId,
                        characterItem = storageItem,
                    });
                }
            }
            worldStorageSaveData.SavePersistentData($"{hostPlayerCharacterData.Id}_world");
        }

        public override void SavePlayerStorage(IPlayerCharacterData playerCharacterData, List<CharacterItem> storageItems)
        {
            for (int i = playerStorageSaveData.storageItems.Count - 1; i >= 0; --i)
            {
                if (playerStorageSaveData.storageItems[i].storageType == StorageType.Player &&
                    playerStorageSaveData.storageItems[i].storageOwnerId.Equals(playerCharacterData.Id))
                    playerStorageSaveData.storageItems.RemoveAt(i);
            }
            foreach (CharacterItem storageItem in storageItems)
            {
                playerStorageSaveData.storageItems.Add(new StorageCharacterItem()
                {
                    storageType = StorageType.Player,
                    storageOwnerId = playerCharacterData.Id,
                    characterItem = storageItem,
                });
            }
            playerStorageSaveData.SavePersistentData(playerCharacterData.Id);
        }

        public override void SaveWorld(IPlayerCharacterData hostPlayerCharacterData, IEnumerable<IBuildingSaveData> buildings)
        {
            if (!isReadyToSave || BaseGameNetworkManager.CurrentMapInfo == null)
                return;

            // Save building entities / Tree / Rocks
            worldSaveData.buildings.Clear();
            foreach (IBuildingSaveData buildingEntity in buildings)
            {
                if (buildingEntity == null) continue;
                worldSaveData.buildings.Add(buildingEntity.CloneTo(new BuildingSaveData()));
            }
            worldSaveData.SavePersistentData(hostPlayerCharacterData.Id, BaseGameNetworkManager.CurrentMapInfo.Id);
        }

        public override void SaveSummonBuffs(IPlayerCharacterData playerCharacterData, List<CharacterSummon> summons)
        {
            if (!isReadyToSave)
                return;

            // Save buffs from all summons
            summonBuffsSaveData.summonBuffs.Clear();
            CharacterSummon tempSummon;
            CharacterBuff tempBuff;
            for (int i = 0; i < summons.Count; ++i)
            {
                tempSummon = summons[i];
                if (tempSummon.CacheEntity == null || tempSummon.CacheEntity.Buffs == null || tempSummon.CacheEntity.Buffs.Count == 0) continue;
                for (int j = 0; j < tempSummon.CacheEntity.Buffs.Count; ++j)
                {
                    tempBuff = tempSummon.CacheEntity.Buffs[j];
                    summonBuffsSaveData.summonBuffs.Add(new CharacterBuff()
                    {
                        id = i + "_" + j,
                        type = tempBuff.type,
                        dataId = tempBuff.dataId,
                        level = tempBuff.level,
                        buffRemainsDuration = tempBuff.buffRemainsDuration,
                    });
                }
            }
            summonBuffsSaveData.SavePersistentData(playerCharacterData.Id);
        }

        public override void OnSceneChanging()
        {
            isReadyToSave = false;
        }
    }
}
