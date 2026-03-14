using Cysharp.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLibManager;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Profiling;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class LanRpgNetworkManager : BaseGameNetworkManager
    {
        public enum GameStartType
        {
            Client,
            Host,
            SinglePlayer,
        }

        public enum EnableGmCommandType
        {
            Everyone,
            HostOnly,
        }

        protected static readonly ProfilerMarker s_SaveProfilerMarker = new ProfilerMarker("LanRpgNetworkManager - Save");

        [Header("Lan RPG")]
        public float autoSaveDuration = 2f;
        public GameStartType startType;
        public PlayerCharacterData selectedCharacter;
        public List<CharacterBuff> selectedCharacterSummonBuffs;
        public List<CharacterItem> selectedCharacterStorageItems;
        public EnableGmCommandType enableGmCommands;
        private float _lastSaveTime;
        private Vector3? _teleportPosition;
        private readonly ConcurrentDictionary<long, PlayerCharacterData> _pendingSpawnPlayerCharacters = new ConcurrentDictionary<long, PlayerCharacterData>();
        private readonly ConcurrentDictionary<long, List<CharacterBuff>> _pendingSpawnPlayerCharacterSummonBuffs = new ConcurrentDictionary<long, List<CharacterBuff>>();
        private readonly HashSet<string> _teleportingPlayerCharacterIds = new HashSet<string>();

        public LiteNetLibDiscovery CacheDiscovery { get; private set; }
        public BaseGameSaveSystem SaveSystem { get { return GameInstance.Singleton.SaveSystem; } }

        protected override void Awake()
        {
            CacheDiscovery = gameObject.GetOrAddComponent<LiteNetLibDiscovery>();
            PrepareLanRpgHandlers();
            base.Awake();
        }

        protected override void Clean()
        {
            base.Clean();
            _pendingSpawnPlayerCharacters.Clear();
            _pendingSpawnPlayerCharacterSummonBuffs.Clear();
            _teleportingPlayerCharacterIds.Clear();
        }

        public void StartGame()
        {
            NetworkSetting gameServiceConnection = CurrentGameInstance.NetworkSetting;
            switch (startType)
            {
                case GameStartType.Host:
                    SetMapInfo(selectedCharacter.CurrentMapName);
#if !DISABLE_ADDRESSABLES
                    Assets.addressableOnlineScene = CurrentMapInfo.AddressableScene;
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                    Assets.onlineScene = CurrentMapInfo.Scene;
#endif
                    networkPort = gameServiceConnection.networkPort;
                    maxConnections = gameServiceConnection.maxConnections;
                    StartHost(false);
                    // Set discovery data by selected character
                    CacheDiscovery.data = JsonConvert.SerializeObject(new DiscoveryData()
                    {
                        id = selectedCharacter.Id,
                        characterName = selectedCharacter.CharacterName,
                        level = selectedCharacter.Level
                    });
                    // Stop discovery client because game started
                    CacheDiscovery.StopClient();
                    // Start discovery server to allow clients to connect
                    CacheDiscovery.StartServer();
                    break;
                case GameStartType.SinglePlayer:
                    SetMapInfo(selectedCharacter.CurrentMapName);
#if !DISABLE_ADDRESSABLES
                    Assets.addressableOnlineScene = CurrentMapInfo.AddressableScene;
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                    Assets.onlineScene = CurrentMapInfo.Scene;
#endif
                    StartHost(true);
                    // Stop discovery client because game started
                    CacheDiscovery.StopClient();
                    break;
                case GameStartType.Client:
                    networkPort = gameServiceConnection.networkPort;
                    StartClient();
                    // Stop discovery client because game started
                    CacheDiscovery.StopClient();
                    break;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SaveSystem.OnServerStart();
        }

        public override void OnClientConnected()
        {
            base.OnClientConnected();
            ClientStorageActions.onNotifyStorageItemsUpdated += NotifyStorageItemsUpdated;
        }

        public override void OnStopHost()
        {
            // Stop both client and server
            CacheDiscovery.StopClient();
            CacheDiscovery.StopServer();
            base.OnStopHost();
        }

        public override void OnStopClient()
        {
            Save();
            base.OnStopClient();
            ClientStorageActions.onNotifyStorageItemsUpdated -= NotifyStorageItemsUpdated;
        }

        private void NotifyStorageItemsUpdated(StorageType storageType, string storageOwnerId, List<CharacterItem> storageItems)
        {
            if (storageType != StorageType.Player || !string.Equals(storageOwnerId, selectedCharacter.Id))
                return;
            selectedCharacterStorageItems = storageItems;
        }

        protected override void HandleServerSceneChange(MessageHandlerData messageHandler)
        {
            if (!IsServer)
            {
                Save();
                SaveSystem.OnSceneChanging();
            }
            base.HandleServerSceneChange(messageHandler);
        }

        protected override async UniTask PreSpawnEntities()
        {
            await SaveSystem.PreSpawnEntities(selectedCharacter, ServerStorageHandlers.GetAllStorageItems());
        }

        public void Save(System.Action<IPlayerCharacterData> onBeforeSaveCharacter = null, bool saveWorld = true, bool saveStorage = true)
        {
            using (s_SaveProfilerMarker.Auto())
            {
                BasePlayerCharacterEntity playingCharacter = GameInstance.PlayingCharacterEntity;
                if (playingCharacter != null)
                {
                    selectedCharacter = playingCharacter.CloneTo(selectedCharacter);
                    if (onBeforeSaveCharacter != null)
                        onBeforeSaveCharacter.Invoke(selectedCharacter);
                    SaveSystem.SaveCharacter(selectedCharacter);
                    SaveSystem.SaveSummonBuffs(selectedCharacter, new List<CharacterSummon>(selectedCharacter.Summons));
                    if (IsServer)
                    {
                        if (saveWorld)
                            SaveSystem.SaveWorld(selectedCharacter, ServerBuildingHandlers.GetBuildings());
                        if (saveStorage)
                            SaveSystem.SaveStorage(selectedCharacter, ServerStorageHandlers.GetAllStorageItems());
                    }
                    if (saveStorage)
                        SaveSystem.SavePlayerStorage(selectedCharacter, selectedCharacterStorageItems);
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            float tempTime = Time.unscaledTime;
            if (tempTime - _lastSaveTime > autoSaveDuration)
            {
                if (IsClientConnected || IsServer)
                    Save();
                _lastSaveTime = tempTime;
            }
        }

        protected override void OnServerUpdate(LogicUpdater updater)
        {
            base.OnServerUpdate(updater);
            if (_pendingSpawnPlayerCharacters.Count > 0 && _isServerReadyToInstantiatePlayers)
            {
                // Spawn pending player characters
                LiteNetLibPlayer player;
                foreach (KeyValuePair<long, PlayerCharacterData> spawnPlayerCharacter in _pendingSpawnPlayerCharacters)
                {
                    if (!Players.TryGetValue(spawnPlayerCharacter.Key, out player))
                        continue;
                    player.IsReady = true;
                    SpawnPlayerCharacter(spawnPlayerCharacter.Key, spawnPlayerCharacter.Value, _pendingSpawnPlayerCharacterSummonBuffs[spawnPlayerCharacter.Key]);
                }
                _pendingSpawnPlayerCharacters.Clear();
                _pendingSpawnPlayerCharacterSummonBuffs.Clear();
            }
        }

        public override void OnPeerDisconnected(long connectionId, DisconnectReason reason, SocketError socketError)
        {
            base.OnPeerDisconnected(connectionId, reason, socketError);
            UnregisterPlayerCharacter(connectionId);
        }


        public override void SerializeClientReadyData(NetDataWriter writer)
        {
            GameInstance.PlayingCharacter = selectedCharacter;
            selectedCharacterSummonBuffs = SaveSystem.LoadSummonBuffs(selectedCharacter);
            selectedCharacterStorageItems = SaveSystem.LoadPlayerStorage(selectedCharacter);
            selectedCharacter.SerializeCharacterData(writer);
            writer.PutList(selectedCharacterSummonBuffs);
            writer.PutList(selectedCharacterStorageItems);
        }

        public override UniTask<bool> DeserializeClientReadyData(uint requestId, long connectionId, NetDataReader reader, LiteNetLibIdentity playerIdentity)
        {
            PlayerCharacterData playerCharacterData = new PlayerCharacterData().DeserializeCharacterData(reader);
            List<CharacterBuff> playerSummonBuffs = reader.GetList<CharacterBuff>();
            List<CharacterItem> playerStorageItems = reader.GetList<CharacterItem>();
            ServerStorageHandlers.SetStorageItems(new StorageId(StorageType.Player, playerCharacterData.Id), playerStorageItems);
            if (!_isServerReadyToInstantiatePlayers || (IsClient && !_isClientReadyToInstantiateObjects))
            {
                // Not ready to instantiate objects, add spawning player character to pending dictionary
                if (LogDev) Logging.Log(LogTag, "Not ready to deserializing client ready extra");
                _pendingSpawnPlayerCharacters[connectionId] = playerCharacterData;
                _pendingSpawnPlayerCharacterSummonBuffs[connectionId] = playerSummonBuffs;
                return UniTask.FromResult(true);
            }
            if (LogDev) Logging.Log(LogTag, "Deserializing client ready extra");
            SpawnPlayerCharacter(connectionId, playerCharacterData, playerSummonBuffs);
            return UniTask.FromResult(true);
        }

        private void SpawnPlayerCharacter(long connectionId, PlayerCharacterData playerCharacterData, List<CharacterBuff> summonBuffs)
        {
            // If it is not allow this character data, disconnect user
            if (!playerCharacterData.TryGetEntityPrefab(out _, out _)
#if !DISABLE_ADDRESSABLES
                && !playerCharacterData.TryGetEntityAddressablePrefab(out _, out _)
#endif
                )
            {
                Logging.LogError(LogTag, "Cannot find player character with entity Id: " + playerCharacterData.EntityId);
                return;
            }

            if (!CurrentMapInfo.Id.Equals(playerCharacterData.CurrentMapName) ||
                _teleportingPlayerCharacterIds.Contains(playerCharacterData.Id))
            {
                Vector3 targetPosition = _teleportPosition.HasValue ? _teleportPosition.Value : CurrentMapInfo.StartPosition;
                playerCharacterData.CurrentPosition = targetPosition;
            }
            _teleportingPlayerCharacterIds.Remove(playerCharacterData.Id);

            // Set proper spawn position
            if (!IsInstanceMap())
            {
                CurrentMapInfo.GetEnterMapPoint(playerCharacterData, out string mapName, out Vector3 position, out Vector3 rotation);
                playerCharacterData.CurrentMapName = mapName;
                playerCharacterData.CurrentPosition = position;
                playerCharacterData.CurrentRotation = rotation;
            }

            // Spawn character entity and set its data
            Quaternion characterRotation = Quaternion.identity;
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                characterRotation = Quaternion.Euler(playerCharacterData.CurrentRotation);
            // NOTE: entity ID is a hash asset ID :)
            int metaDataId;
            LiteNetLibIdentity spawnObj = Assets.GetObjectInstance(
                GameInstance.GetPlayerCharacterEntityHashAssetId(playerCharacterData.EntityId, out metaDataId),
                playerCharacterData.CurrentPosition,
                characterRotation);

            BasePlayerCharacterEntity playerCharacterEntity = spawnObj.GetComponent<BasePlayerCharacterEntity>();
            GameInstance.SetupByMetaData(playerCharacterEntity, metaDataId);
            playerCharacterData.CloneTo(playerCharacterEntity);

            Assets.NetworkSpawn(spawnObj, 0, connectionId);

            // Set user Id
            playerCharacterEntity.UserId = playerCharacterEntity.Id;

            // Enable GM commands in Singleplayer / LAN mode
            // TODO: Don't use fixed user level
            if (enableGmCommands == EnableGmCommandType.Everyone)
                playerCharacterEntity.UserLevel = 1;

            // Load data for first character (host)
            if (ServerUserHandlers.PlayerCharactersCount == 0)
            {
                if (enableGmCommands == EnableGmCommandType.HostOnly)
                    playerCharacterEntity.UserLevel = 1;
            }

            // Force make caches, to calculate current stats to fill empty slots items
            playerCharacterEntity.ForceMakeCaches();
            playerCharacterEntity.FillEmptySlots();

            // Notify clients that this character is spawn or dead
            if (!playerCharacterEntity.IsDead())
            {
                playerCharacterEntity.CallRpcOnRespawn();
                // Summon mount
                playerCharacterEntity.SpawnMount(
                    playerCharacterEntity.Mount.type,
                    playerCharacterEntity.Mount.sourceId,
                    playerCharacterEntity.Mount.mountRemainsDuration,
                    playerCharacterEntity.Mount.level,
                    playerCharacterEntity.Mount.currentHp);
                // Summon monsters
                for (int i = 0; i < playerCharacterEntity.Summons.Count; ++i)
                {
                    CharacterSummon summon = playerCharacterEntity.Summons[i];
                    summon.Summon(playerCharacterEntity, summon.level, summon.summonRemainsDuration, summon.exp, summon.currentHp, summon.currentMp);
                    for (int j = 0; j < summonBuffs.Count; ++j)
                    {
                        if (summonBuffs[j].id.StartsWith(i.ToString()))
                        {
                            summon.CacheEntity.Buffs.Add(summonBuffs[j]);
                            summonBuffs.RemoveAt(j);
                            j--;
                        }
                    }
                    playerCharacterEntity.Summons[i] = summon;
                }
            }
            else
            {
                playerCharacterEntity.CallRpcOnDead();
            }

            // Register player, will use registered player to send chat / player messages
            RegisterPlayerCharacter(connectionId, playerCharacterEntity);

            SocialCharacterData[] members;
            // Set guild id
            if (ServerGuildHandlers.GuildsCount > 0)
            {
                foreach (GuildData guild in ServerGuildHandlers.GetGuilds())
                {
                    members = guild.GetMembers();
                    for (int i = 0; i < members.Length; ++i)
                    {
                        if (members[i].id.Equals(playerCharacterEntity.Id))
                        {
                            playerCharacterEntity.GuildId = guild.id;
                            break;
                        }
                    }
                    if (playerCharacterEntity.GuildId > 0)
                        break;
                }
            }
            // Set party id
            if (ServerPartyHandlers.PartiesCount > 0)
            {
                foreach (PartyData party in ServerPartyHandlers.GetParties())
                {
                    members = party.GetMembers();
                    for (int i = 0; i < members.Length; ++i)
                    {
                        if (members[i].id.Equals(playerCharacterEntity.Id))
                        {
                            playerCharacterEntity.PartyId = party.id;
                            break;
                        }
                    }
                    if (playerCharacterEntity.PartyId > 0)
                        break;
                }
            }
        }

        public override async UniTask WarpCharacter(BasePlayerCharacterEntity playerCharacterEntity, string mapName, Vector3 position, bool overrideRotation, Vector3 rotation)
        {
            if (!CanWarpCharacter(playerCharacterEntity))
                return;

            // If map name is empty, just teleport character to target position
            if (string.IsNullOrEmpty(mapName) || (mapName.Equals(CurrentMapInfo.Id) && !IsInstanceMap()))
            {
                if (overrideRotation)
                    playerCharacterEntity.CurrentRotation = rotation;
                playerCharacterEntity.Teleport(position, Quaternion.Euler(playerCharacterEntity.CurrentRotation), false);
                await playerCharacterEntity.WaitClientTeleportConfirm();
                return;
            }

            if (!string.IsNullOrEmpty(mapName) && playerCharacterEntity.IsServer && playerCharacterEntity.IsOwnerClient &&
                GameInstance.MapInfos.TryGetValue(mapName, out BaseMapInfo mapInfo) && (mapInfo.IsAddressableSceneValid() || mapInfo.IsSceneValid()))
            {
                // Save data before warp
                BasePlayerCharacterEntity owningCharacter = GameInstance.PlayingCharacterEntity;
                SaveSystem.SaveWorld(owningCharacter, ServerBuildingHandlers.GetBuildings());
                SaveSystem.SaveStorage(owningCharacter, ServerStorageHandlers.GetAllStorageItems());
                ServerBuildingHandlers.ClearBuildings();
                ServerStorageHandlers.ClearStorage();
                SetMapInfo(mapInfo);
                _teleportPosition = position;
                foreach (IPlayerCharacterData playerCharacter in GameInstance.ServerUserHandlers.GetPlayerCharacters())
                {
                    _teleportingPlayerCharacterIds.Add(playerCharacter.Id);
                }
                Save((savingCharacter) =>
                {
                    savingCharacter.CurrentMapName = mapInfo.Id;
                    savingCharacter.CurrentPosition = position;
                    if (overrideRotation)
                        savingCharacter.CurrentRotation = rotation;
                }, false, false);
                SaveSystem.OnSceneChanging();
                // Unregister all players characters to register later after map changed
                foreach (LiteNetLibPlayer player in GetPlayers())
                {
                    UnregisterPlayerCharacter(player.ConnectionId);
                }
                if (owningCharacter != null)
                {
                    // Destroy owning character to avoid save while warp
                    owningCharacter.NetworkDestroy();
                }
                ServerSceneChange(mapInfo.GetSceneInfo());
            }
        }

        public override async UniTask WarpCharacterToInstance(BasePlayerCharacterEntity playerCharacterEntity, string mapName, Vector3 position, bool overrideRotation, Vector3 rotation)
        {
            // For now just warp follow host
            // TODO: May add instance by load scene additive and offsets for LAN mode
            await WarpCharacter(playerCharacterEntity, mapName, position, overrideRotation, rotation);
        }

        public override bool IsInstanceMap()
        {
            return false;
        }

        public override UniTask<ResponsePlayerCharacterTransformMessage> RequestPlayerCharacterTransform(long connectionId)
        {
            if (_pendingSpawnPlayerCharacters.TryGetValue(connectionId, out PlayerCharacterData playerCharacterData))
            {
                return UniTask.FromResult(new ResponsePlayerCharacterTransformMessage()
                {
                    message = UITextKeys.NONE,
                    position = playerCharacterData.CurrentPosition,
                    rotation = playerCharacterData.CurrentRotation,
                });
            }
            return UniTask.FromResult(new ResponsePlayerCharacterTransformMessage()
            {
                message = UITextKeys.UI_ERROR_CHARACTER_NOT_FOUND,
            });
        }
    }
}
