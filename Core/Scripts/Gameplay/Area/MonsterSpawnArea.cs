using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class MonsterSpawnArea : GameSpawnArea<BaseMonsterCharacterEntity>
    {
        [Tooltip("This is deprecated, might be removed in future version, set your asset to `Asset` instead.")]
        [ReadOnlyField]
        public BaseMonsterCharacterEntity monsterCharacterEntity;
        public Faction faction;

        protected override void Awake()
        {
            base.Awake();
            MigrateAsset();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            MigrateAsset();
        }
#endif

        private void MigrateAsset()
        {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            if (prefab == null && monsterCharacterEntity != null)
            {
                prefab = monsterCharacterEntity;
                monsterCharacterEntity = null;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
            if (prefab != null && monsterCharacterEntity != null)
            {
                monsterCharacterEntity = null;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
#endif
        }

        public override void RegisterPrefabs()
        {
            base.RegisterPrefabs();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddMonsterCharacterEntities(prefab);
            foreach (SpawnPrefabData spawnData in spawningPrefabs)
            {
                GameInstance.AddMonsterCharacterEntities(spawnData.prefab);
            }
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferenceMonsterCharacterEntities(addressablePrefab);
            foreach (SpawnPrefabData spawnData in spawningPrefabs)
            {
                GameInstance.AddAssetReferenceMonsterCharacterEntities(spawnData.addressablePrefab);
            }
#endif
        }

        protected override BaseMonsterCharacterEntity SpawnInternal(BaseMonsterCharacterEntity prefab
#if !DISABLE_ADDRESSABLES
            , AddressablePrefab addressablePrefab
#endif
            , int level, float destroyRespawnDelay)
        {
            if (!GetRandomPosition(out Vector3 spawnPosition))
            {
#if UNITY_EDITOR || DEBUG_SPAWN_AREA
                Logging.LogWarning(ToString(), $"Cannot spawn monster, it cannot find grounded position, pending monster amount {_pending.Count}");
#endif
                return null;
            }

            Quaternion spawnRotation = GetRandomRotation();
            LiteNetLibIdentity spawnObj = null;
            BaseMonsterCharacterEntity entity = null;
            if (prefab != null)
            {
                spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                    prefab.Identity.HashAssetId,
                    spawnPosition, spawnRotation);
                if (spawnObj == null)
                    return null;
                spawnObj.SubChannelId = subChannelId;
                entity = spawnObj.GetComponent<BaseMonsterCharacterEntity>();
                entity.SetSpawnArea(this, prefab, level, spawnPosition);
                if (destroyRespawnDelay > 0f)
                    entity.DestroyRespawnDelay = destroyRespawnDelay;
            }
#if !DISABLE_ADDRESSABLES
            else if (addressablePrefab.IsDataValid())
            {
                spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                    addressablePrefab.HashAssetId,
                    spawnPosition, spawnRotation);
                if (spawnObj == null)
                    return null;
                spawnObj.SubChannelId = subChannelId;
                entity = spawnObj.GetComponent<BaseMonsterCharacterEntity>();
                entity.SetSpawnArea(this, addressablePrefab, level, spawnPosition);
                if (destroyRespawnDelay > 0f)
                    entity.DestroyRespawnDelay = destroyRespawnDelay;
            }
#endif

            if (entity == null)
            {
                Logging.LogWarning(ToString(), $"Cannot spawn monster, entity is null");
                return null;
            }

            if (!entity.FindGroundedPosition(spawnPosition, groundDetectionOffsets, out spawnPosition))
            {
                // Destroy the entity (because it can't find ground position)
                BaseGameNetworkManager.Singleton.Assets.DestroyObjectInstance(spawnObj);
#if UNITY_EDITOR || DEBUG_SPAWN_AREA
                Logging.LogWarning(ToString(), $"Cannot spawn monster, it cannot find grounded position, pending monster amount {_pending.Count}");
#endif
                return null;
            }

            entity.Level = level;
            entity.Faction = faction;
            entity.Teleport(spawnPosition, spawnRotation, false);
            entity.InitStats();
            BaseGameNetworkManager.Singleton.Assets.NetworkSpawn(spawnObj);
            entity.CallRpcOnSpawned();
            return entity;
        }

        public override int GroundLayerMask
        {
            get { return CurrentGameInstance.GetGameEntityGroundDetectionLayerMask(); }
        }

#if UNITY_EDITOR
        [ContextMenu("Count Spawning Objects")]
        public override void CountSpawningObjects()
        {
            base.CountSpawningObjects();
        }
#endif
    }
}
