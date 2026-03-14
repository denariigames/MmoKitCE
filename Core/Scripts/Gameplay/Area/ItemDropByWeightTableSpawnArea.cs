using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public class ItemDropByWeightTableSpawnArea : GameSpawnArea
    {

        [System.Serializable]
        public class SpawnPendingData
        {
            public CharacterItem item;
            public float countdown = 0f;
        }

        [Header("Drop settings")]
        public ItemRandomByWeightTable weightTable;
        [FormerlySerializedAs("respawnPickedupDelay")]
        public float respawnPickedupDelayMin = 10f;
        public float respawnPickedupDelayMax = 10f;
        public float droppedItemDestroyDelay = 300f;
        public RewardGivenType rewardGivenType = RewardGivenType.None;

        protected float secondsCounter = 0f;
        protected List<SpawnPendingData> _pending = new List<SpawnPendingData>();
        protected List<SpawnPendingData> _unableToSpawns = new List<SpawnPendingData>();
        protected List<float> _failedRespawnCountdowns = new List<float>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            weightTable = null;
            _pending?.Clear();
            _pending = null;
            _unableToSpawns?.Clear();
            _unableToSpawns = null;
            _failedRespawnCountdowns?.Clear();
            _failedRespawnCountdowns = null;
        }

        protected override void LateUpdate()
        {
            if (!BaseGameNetworkManager.Singleton.IsServer)
                return;

            base.LateUpdate();

            float deltaTime = Time.deltaTime;
            secondsCounter += deltaTime;
            if (secondsCounter < SPAWN_UPDATE_DELAY)
                return;
            float decreaseCountdown = secondsCounter;
            secondsCounter -= SPAWN_UPDATE_DELAY;

            if (_pending.Count > 0)
            {
                for (int i = _pending.Count - 1; i >= 0; --i)
                {
                    SpawnPendingData pendingEntry = _pending[i];
                    if (pendingEntry == null)
                    {
                        _pending.RemoveAt(i);
                        continue;
                    }

                    pendingEntry.countdown -= decreaseCountdown;
                    if (pendingEntry.countdown > 0f)
                    {
                        // Not ready to spawn yet
                        _pending[i] = pendingEntry;
                        continue;
                    }

                    if (!AbleToSpawn())
                    {
                        // Not able to spawn yet
                        continue;
                    }

                    SpawnPending(i);
                }
            }

            if (_unableToSpawns.Count > 0)
            {
                // Add unable spawns to pending list
                for (int i = 0; i < _unableToSpawns.Count; ++i)
                {
                    _pending.Add(_unableToSpawns[i]);
                }
                _unableToSpawns.Clear();
            }

            if (_failedRespawnCountdowns.Count > 0)
            {
                for (int i = _failedRespawnCountdowns.Count - 1; i >= 0; --i)
                {
                    float countdown = _failedRespawnCountdowns[i];
                    countdown -= decreaseCountdown;
                    if (countdown > 0f)
                    {
                        // Not ready to spawn yet
                        _failedRespawnCountdowns[i] = countdown;
                        continue;
                    }
                    _failedRespawnCountdowns.RemoveAt(i);
                    RandomItem(0f);
                }
            }
        }

        public override void OnDestroyBySubscribeHandler(GameSpawnAreaEntityHandler handler)
        {
            SpawnPendingData pendingData = handler.SpawnData as SpawnPendingData;
            pendingData.countdown = 1f;
            AddPending(pendingData);
        }

#if UNITY_EDITOR
        [ContextMenu("Spawn All")]
#endif
        public override void SpawnAll()
        {
            int amount = GetRandomedSpawnAmount();
            for (int i = 0; i < amount; ++i)
            {
                RandomItem(0f);
            }
        }

        public virtual void Spawn(CharacterItem item, float delay)
        {
            if (item.IsEmptySlot())
                return;

            AddPending(new SpawnPendingData()
            {
                item = item,
                countdown = delay,
            });
        }

        private async void SpawnPending(int pendingIndex)
        {
            if (!AbleToSpawn())
            {
                Debug.Log($"Not able to spawn, Spawn Type={spawnType}, Spawn State={_subscribeHandler.CurrentSpawnState}");
                return;
            }
            SpawnPendingData pendingEntry = _pending[pendingIndex];
            _pending.RemoveAt(pendingIndex);
            CharacterItem item = pendingEntry.item;

            if (GetRandomPosition(out Vector3 dropPosition))
            {
                Quaternion dropRotation = Quaternion.identity;
                if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                {
                    dropRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
                }

                BaseItem itemData = item.GetItem();
                if (GameInstance.Singleton.IsExpDropRepresentItem(itemData))
                {
                    ExpDropEntity prefab = await CurrentGameInstance.GetLoadedExpDropEntityPrefab();
                    if (prefab != null)
                    {
                        ExpDropEntity newEntity = BaseRewardDropEntity.Drop(prefab, dropPosition, dropRotation, 1f, rewardGivenType, 1, 1, item.amount, System.Array.Empty<string>(), -1);
#if UNITY_EDITOR || DEBUG_SPAWN_AREA
                        newEntity.name = $"ExpDropEntity_{name}_{item.dataId}_{item.amount}";
#else
                        newEntity.name = string.Empty;
#endif
                        newEntity.Identity.SubChannelId = subChannelId;
                        newEntity.CallRpcOnSpawned();
                        newEntity.onNetworkDestroy -= NewEntity_onNetworkDestroy;
                        newEntity.onNetworkDestroy += NewEntity_onNetworkDestroy;
                        _subscribeHandler.AddEntity(newEntity, pendingEntry);
                    }
                }
                else if (GameInstance.Singleton.IsGoldDropRepresentItem(itemData))
                {
                    GoldDropEntity prefab = await CurrentGameInstance.GetLoadedGoldDropEntityPrefab();
                    if (prefab != null)
                    {
                        GoldDropEntity newEntity = BaseRewardDropEntity.Drop(prefab, dropPosition, dropRotation, 1f, rewardGivenType, 1, 1, item.amount, System.Array.Empty<string>(), -1);
#if UNITY_EDITOR || DEBUG_SPAWN_AREA
                        newEntity.name = $"GoldDropEntity_{name}_{item.dataId}_{item.amount}";
#else
                        newEntity.name = string.Empty;
#endif
                        newEntity.Identity.SubChannelId = subChannelId;
                        newEntity.CallRpcOnSpawned();
                        newEntity.onNetworkDestroy -= NewEntity_onNetworkDestroy;
                        newEntity.onNetworkDestroy += NewEntity_onNetworkDestroy;
                        _subscribeHandler.AddEntity(newEntity, pendingEntry);
                    }
                }
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
                else if (GameInstance.Singleton.IsCurrencyDropRepresentItem(itemData, out Currency currency))
                {
                    CurrencyDropEntity prefab = await CurrentGameInstance.GetLoadedCurrencyDropEntityPrefab();
                    if (prefab != null)
                    {
                        CurrencyDropEntity newEntity = BaseRewardDropEntity.Drop(prefab, dropPosition, dropRotation, 1f, rewardGivenType, 1, 1, item.amount, System.Array.Empty<string>(), -1);
#if UNITY_EDITOR || DEBUG_SPAWN_AREA
                        newEntity.name = $"CurrencyDropEntity_{name}_{item.dataId}_{item.amount}";
#else
                        newEntity.name = string.Empty;
#endif
                        newEntity.Currency = currency;
                        newEntity.Identity.SubChannelId = subChannelId;
                        newEntity.CallRpcOnSpawned();
                        newEntity.onNetworkDestroy -= NewEntity_onNetworkDestroy;
                        newEntity.onNetworkDestroy += NewEntity_onNetworkDestroy;
                        _subscribeHandler.AddEntity(newEntity, pendingEntry);
                    }
                }
#endif
                else
                {
                    ItemDropEntity prefab = await CurrentGameInstance.GetLoadedItemDropEntityPrefab();
                    if (prefab != null)
                    {
                        ItemDropEntity newEntity = ItemDropEntity.Drop(prefab, dropPosition, dropRotation, rewardGivenType, item, System.Array.Empty<string>(), -1);
#if UNITY_EDITOR || DEBUG_SPAWN_AREA
                        newEntity.name = $"ItemDropEntity_{name}_{item.dataId}_{item.amount}";
#else
                        newEntity.name = string.Empty;
#endif
                        newEntity.Identity.SubChannelId = subChannelId;
                        newEntity.CallRpcOnSpawned();
                        newEntity.onNetworkDestroy -= NewEntity_onNetworkDestroy;
                        newEntity.onNetworkDestroy += NewEntity_onNetworkDestroy;
                        _subscribeHandler.AddEntity(newEntity, pendingEntry);
                    }
                }
            }
            else
            {
                // Unable to spawn yet, will add to spawn pending later
                pendingEntry.countdown = respawnPendingEntitiesDelay;
                _unableToSpawns.Add(pendingEntry);
            }
        }

        protected virtual void AddPending(SpawnPendingData data)
        {
            _pending.Add(data);
        }

        protected virtual void NewEntity_onNetworkDestroy(byte reasons)
        {
            if (!AbleToSpawn())
            {
                return;
            }
            RandomItem(Random.Range(respawnPickedupDelayMin, respawnPickedupDelayMax));
        }

        protected virtual void RandomItem(float respawnDelay)
        {
            if (weightTable == null)
            {
#if UNITY_EDITOR || DEBUG_SPAWN_AREA
                Logging.LogWarning(ToString(), $"Unable to spawn item, table is empty.");
#endif
                return;
            }
            weightTable.RandomItem((item, level, amount) =>
            {
                if (amount > item.MaxStack)
                    amount = item.MaxStack;
                Spawn(CharacterItem.Create(item, level, amount), respawnDelay);
            }, onFailed: () =>
            {
                _failedRespawnCountdowns.Add(Random.Range(respawnPickedupDelayMin, respawnPickedupDelayMax));
            });
        }
    }
}
