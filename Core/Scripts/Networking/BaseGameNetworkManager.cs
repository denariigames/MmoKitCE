using ConcurrentCollections;
using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.DevExtension;
using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLibManager;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace MultiplayerARPG
{
    public abstract partial class BaseGameNetworkManager : LiteNetLibGameManager
    {
        public const string CHAT_SYSTEM_ANNOUNCER_SENDER = "SYSTEM_ANNOUNCER";
        public const float UPDATE_ONLINE_CHARACTER_DURATION = 1f;
        public const float UPDATE_TIME_OF_DAY_DURATION = 5f;
        public const string INSTANTIATES_OBJECTS_DELAY_STATE_KEY = "INSTANTIATES_OBJECTS_DELAY";
        public const float INSTANTIATES_OBJECTS_DELAY = 1f;

        protected static readonly NetDataWriter s_Writer = new NetDataWriter();

        public static BaseGameNetworkManager Singleton { get; protected set; }
        protected GameInstance CurrentGameInstance { get { return GameInstance.Singleton; } }
        // Others
        public ILagCompensationManager LagCompensationManager { get; protected set; }
        public IHitRegistrationManager HitRegistrationManager { get; protected set; }
        public BaseGameNetworkManagerComponent[] ManagerComponents { get; private set; }

        public bool IsTemporarilyClose { get; set; } = false;
        public string ChannelId { get; set; } = string.Empty;
        public string ChannelTitle { get; set; } = string.Empty;
        public string ChannelDescription { get; set; } = string.Empty;
        public string ChannelTag { get; set; } = string.Empty;
        public string ChannelPassword { get; set; } = string.Empty;
        public BaseMapInfo MapInfo { get; protected set; } = null;
        public static BaseMapInfo CurrentMapInfo => Singleton == null ? null : Singleton.MapInfo;
        public bool ShouldPhysicSyncTransforms { get; set; }
        public bool ShouldPhysicSyncTransforms2D { get; set; }

        public bool useUnityAutoPhysicSyncTransform = true;
        // Spawn entities events
        public LiteNetLibLoadSceneEvent onSpawnEntitiesStart = new LiteNetLibLoadSceneEvent();
        public LiteNetLibLoadSceneEvent onSpawnEntitiesProgress = new LiteNetLibLoadSceneEvent();
        public LiteNetLibLoadSceneEvent onSpawnEntitiesFinish = new LiteNetLibLoadSceneEvent();
        // Other events
        /// <summary>
        /// ConnectionID, PlayerCharacterEntity
        /// </summary>
        public event System.Action<long, BasePlayerCharacterEntity> onRegisterCharacter;
        /// <summary>
        /// ConnectionID, CharacterID, UserID
        /// </summary>
        public event System.Action<long, string, string> onUnregisterCharacter;
        /// <summary>
        /// ConnectionID, UserID
        /// </summary>
        public event System.Action<long, string> onRegisterUser;
        /// <summary>
        /// ConnectionID, UserID
        /// </summary>
        public event System.Action<long, string> onUnregisterUser;
        // Private variables
        protected float _updateOnlineCharactersCountDown;
        protected float _updateTimeOfDayCountDown;
        protected float _serverSceneLoadedTime;
        protected float _clientSceneLoadedTime;

        internal readonly ConcurrentHashSet<int> proceedingGuildIds = new ConcurrentHashSet<int>();
        internal readonly ConcurrentHashSet<int> proceedingPartyIds = new ConcurrentHashSet<int>();
        internal readonly ConcurrentHashSet<long> proceedingConnectionIds = new ConcurrentHashSet<long>();

        // Instantiate object allowing status
        /// <summary>
        /// For backward compatibility, should use `_serverReadyToInstantiateObjectsStates` instead.
        /// </summary>
        protected ConcurrentDictionary<string, bool> _readyToInstantiateObjectsStates { get { return _serverReadyToInstantiateObjectsStates; } set { _serverReadyToInstantiateObjectsStates = value; } }
        protected ConcurrentDictionary<string, bool> _serverReadyToInstantiateObjectsStates = new ConcurrentDictionary<string, bool>();
        public ConcurrentDictionary<string, bool> ServerReadyToInstantiateObjectsStates => _serverReadyToInstantiateObjectsStates;
        protected ConcurrentDictionary<string, bool> _clientReadyToInstantiateObjectsStates = new ConcurrentDictionary<string, bool>();
        public ConcurrentDictionary<string, bool> ClientReadyToInstantiateObjectsStates => _clientReadyToInstantiateObjectsStates;

        /// <summary>
        /// For backward compatibility, should use `_isServerReadyToInstantiateObjects` instead.
        /// </summary>
        protected bool _isReadyToInstantiateObjects { get { return _isServerReadyToInstantiateObjects; } set { _isServerReadyToInstantiateObjects = value; } }
        protected bool _isServerReadyToInstantiateObjects;
        protected bool _isClientReadyToInstantiateObjects;

        /// <summary>
        /// For backward compatibility, should use `_isServerReadyToInstantiatePlayers` instead.
        /// </summary>
        protected bool _isReadyToInstantiatePlayers { get { return _isServerReadyToInstantiatePlayers; } set { _isServerReadyToInstantiatePlayers = value; } }
        protected bool _isServerReadyToInstantiatePlayers;

        protected ConcurrentDictionary<uint, UITextKeys> _enterGameRequestResponseMessages = new ConcurrentDictionary<uint, UITextKeys>();
        protected ConcurrentDictionary<uint, UITextKeys> _clientReadyRequestResponseMessages = new ConcurrentDictionary<uint, UITextKeys>();

        protected override void Awake()
        {
            Singleton = this;
            doNotEnterGameOnConnect = false;
            doNotReadyOnSceneLoaded = true;
            doNotDestroyOnSceneChanges = true;
            LagCompensationManager = gameObject.GetOrAddComponent<ILagCompensationManager, DefaultLagCompensationManager>();
            HitRegistrationManager = gameObject.GetOrAddComponent<IHitRegistrationManager, DefaultHitRegistrationManager>();
            _defaultInterestManager = GetComponent<BaseInterestManager>();
            if (_defaultInterestManager == null)
                _defaultInterestManager = gameObject.AddComponent<JobifiedGridSpatialPartitioningAOI>();
            ManagerComponents = GetComponents<BaseGameNetworkManagerComponent>();
            // Force change physic auto sync transforms mode to manual
            Physics.autoSyncTransforms = useUnityAutoPhysicSyncTransform;
            Physics2D.autoSyncTransforms = useUnityAutoPhysicSyncTransform;
            // Setup character hidding condition
            LiteNetLibIdentity.ForceHideFunctions.Add(IsHideEntity);
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // Remove character hidding condition
            LiteNetLibIdentity.ForceHideFunctions.Remove(IsHideEntity);
        }

        protected bool IsHideEntity(LiteNetLibIdentity mustHideThis, LiteNetLibIdentity fromThis)
        {
            if (!mustHideThis.TryGetComponent(out BaseGameEntity mustHideThisEntity) ||
                !fromThis.TryGetComponent(out BaseGameEntity fromThisEntity))
                return false;
            return mustHideThisEntity.IsHideFrom(fromThisEntity);
        }

        protected override void Update()
        {
            // Network messages will be handled before update game entities (in base.Update())
            base.Update();
            float tempDeltaTime = Time.unscaledDeltaTime;
            if (IsServer)
            {
                _updateOnlineCharactersCountDown -= tempDeltaTime;
                if (_updateOnlineCharactersCountDown <= 0f)
                {
                    _updateOnlineCharactersCountDown = UPDATE_ONLINE_CHARACTER_DURATION;
                    UpdateOnlineCharacters();
                }
                _updateTimeOfDayCountDown -= tempDeltaTime;
                if (_updateTimeOfDayCountDown <= 0f)
                {
                    _updateTimeOfDayCountDown = UPDATE_TIME_OF_DAY_DURATION;
                    SendTimeOfDay();
                }
            }
            // Network messages were handled (in base.Update()), enity movement proceeded, it may have transform changing manually, and need to sync tranforms before update physic movement
            if (ShouldPhysicSyncTransforms && !Physics.autoSyncTransforms)
                Physics.SyncTransforms();
            ShouldPhysicSyncTransforms = false;
            if (ShouldPhysicSyncTransforms2D && !Physics2D.autoSyncTransforms)
                Physics2D.SyncTransforms();
            ShouldPhysicSyncTransforms2D = false;

            // Update game entity, it may update entities movement
            if (IsNetworkActive)
            {
                // Update day-night time on both client and server. It will sync from server some time to make sure that clients time of day won't very difference
                CurrentGameInstance.DayNightTimeUpdater.UpdateTimeOfDay(tempDeltaTime);
            }
        }

        protected override void RegisterMessages()
        {
            base.RegisterMessages();
            RegisterHandlerMessages();
            RegisterRequestToServer<EmptyMessage, EmptyMessage>(GameNetworkingConsts.SafeDisconnect, HandleSafeDisconnectRequest, HandleSafeDisconnectResponse);
            // Keeping `RegisterClientMessages` and `RegisterServerMessages` for backward compatibility, can use any of below dev extension methods
            this.InvokeInstanceDevExtMethods("RegisterClientMessages");
            this.InvokeInstanceDevExtMethods("RegisterServerMessages");
            this.InvokeInstanceDevExtMethods("RegisterMessages");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.RegisterMessages(this);
            }
        }

        protected virtual void Clean()
        {
            CleanHandlers();
            // Other components
            HitRegistrationManager.ClearData();
            MapInfo = null;
            proceedingGuildIds.Clear();
            proceedingPartyIds.Clear();
            proceedingConnectionIds.Clear();
            _serverReadyToInstantiateObjectsStates.Clear();
            _clientReadyToInstantiateObjectsStates.Clear();
            _enterGameRequestResponseMessages.Clear();
            _clientReadyRequestResponseMessages.Clear();
            _isServerReadyToInstantiateObjects = false;
            _isClientReadyToInstantiateObjects = false;
            _isServerReadyToInstantiatePlayers = false;
            GameInstance.JoinedGuild = null;
            GameInstance.JoinedGuild = null;
            PoolSystem.Clear();
            ClientBankActions.Clean();
            ClientCashShopActions.Clean();
            ClientCharacterActions.Clean();
            ClientFriendActions.Clean();
            ClientGachaActions.Clean();
            ClientGenericActions.Clean();
            ClientGuildActions.Clean();
            ClientInventoryActions.Clean();
            ClientMailActions.Clean();
            ClientPartyActions.Clean();
            ClientStorageActions.Clean();
            // Extensions
            this.InvokeInstanceDevExtMethods("Clean");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.Clean(this);
            }
        }

        public override bool StartServer()
        {
            InitPrefabs();
            return base.StartServer();
        }

        public override void OnStartServer()
        {
            SetServerHandlersRef();
            this.InvokeInstanceDevExtMethods("OnStartServer");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnStartServer(this);
            }
            CurrentGameInstance.DayNightTimeUpdater.InitTimeOfDay(this);
            base.OnStartServer();
        }

        public override void OnStopServer()
        {
            this.InvokeInstanceDevExtMethods("OnStopServer");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnStopServer(this);
            }
            Clean();
            base.OnStopServer();
        }

        public override bool StartClient(string networkAddress, int networkPort)
        {
            // Server will call init prefabs function too, so don't call it again
            if (!IsServer)
                InitPrefabs();
            return base.StartClient(networkAddress, networkPort);
        }

        public override void OnStartClient(LiteNetLibClient client)
        {
            SetClientHandlersRef();
            this.InvokeInstanceDevExtMethods("OnStartClient", client);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnStartClient(this, client);
            }
            base.OnStartClient(client);
        }

        public override void OnStopClient()
        {
            this.InvokeInstanceDevExtMethods("OnStopClient");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnStopClient(this);
            }
            ClientGenericActions.ClientStopped();
            if (!IsServer)
                Clean();
            base.OnStopClient();
        }

        public override void OnClientConnected()
        {
            this.InvokeInstanceDevExtMethods("OnClientConnected");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnClientConnected(this);
            }
            ClientGenericActions.ClientConnected();
            base.OnClientConnected();
        }

        public override void OnClientDisconnected(DisconnectReason reason, SocketError socketError, byte[] data)
        {
            UITextKeys message = UITextKeys.NONE;
            if (data != null && data.Length > 0)
            {
                NetDataReader reader = new NetDataReader(data);
                message = (UITextKeys)reader.GetPackedUShort();
            }
            UISceneGlobal.Singleton.ShowDisconnectDialog(reason, socketError, message, "OnClientDisconnected");
            this.InvokeInstanceDevExtMethods("OnClientDisconnected", reason, socketError, data);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnClientDisconnected(this, reason, socketError, data);
            }
            ClientGenericActions.ClientDisconnected(reason, socketError, message);
        }

        public override void OnPeerConnected(long connectionId)
        {
            if (IsTemporarilyClose)
            {
                KickClient(connectionId, UITextKeys.UI_ERROR_SERVER_CLOSE);
                return;
            }
            this.InvokeInstanceDevExtMethods("OnPeerConnected", connectionId);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnPeerConnected(this, connectionId);
            }
            base.OnPeerConnected(connectionId);
        }

        public override void OnPeerDisconnected(long connectionId, DisconnectReason reason, SocketError socketError)
        {
            this.InvokeInstanceDevExtMethods("OnPeerDisconnected", connectionId, reason, socketError);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnPeerDisconnected(this, connectionId, reason, socketError);
            }
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.UpdateClientReadyToInstantiateObjectsStates(this, _clientReadyToInstantiateObjectsStates);
            }
            base.OnPeerDisconnected(connectionId, reason, socketError);
        }

        public override void SendClientEnterGame()
        {
            if (!IsClientConnected)
                return;
            this.InvokeInstanceDevExtMethods("SendClientEnterGame");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.SendClientEnterGame(this);
            }
            base.SendClientEnterGame();
        }

        public override void SendClientReady()
        {
            if (!IsClientConnected)
                return;
            this.InvokeInstanceDevExtMethods("SendClientReady");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.SendClientReady(this);
            }
            base.SendClientReady();
        }

        public override void SendClientNotReady()
        {
            if (!IsClientConnected)
                return;
            this.InvokeInstanceDevExtMethods("SendClientNotReady");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.SendClientNotReady(this);
            }
            base.SendClientNotReady();
        }

        protected override void WriteExtraEnterGameResponse(uint requestId, long connectionId, EnterGameRequestMessage request, AckResponseCode responseCode, NetDataWriter writer)
        {
            if (!_enterGameRequestResponseMessages.TryRemove(requestId, out UITextKeys responseMessage))
                responseMessage = UITextKeys.NONE;
            writer.PutPackedUShort((ushort)responseMessage);
            if (responseMessage == UITextKeys.NONE)
            {
                WriteServerInfo(writer);
                WriteMapInfo(writer);
                WriteTimeOfDay(writer);
            }
        }

        protected override void ReadExtraEnterGameResponse(AckResponseCode responseCode, EnterGameResponseMessage response, NetDataReader reader)
        {
            UITextKeys responseMessage = (UITextKeys)reader.GetPackedUShort();
            if (responseMessage == UITextKeys.NONE)
            {
                ReadServerInfo(reader);
                ReadMapInfo(reader);
                ReadTimeOfDay(reader);
                return;
            }
            UISceneGlobal.Singleton.ShowDisconnectDialog(DisconnectReason.ConnectionRejected, SocketError.Success, responseMessage, "ReadExtraEnterGameResponse");
            StopClient();
        }

        protected override void WriteExtraClientReadyResponse(uint requestId, long connectionId, AckResponseCode responseCode, NetDataWriter writer)
        {
            if (!_clientReadyRequestResponseMessages.TryRemove(requestId, out UITextKeys responseMessage))
                responseMessage = UITextKeys.NONE;
            writer.PutPackedUShort((ushort)responseMessage);
        }

        protected override void ReadExtraClientReadyResponse(AckResponseCode responseCode, EmptyMessage response, NetDataReader reader)
        {
            UITextKeys responseMessage = (UITextKeys)reader.GetPackedUShort();
            if (responseMessage == UITextKeys.NONE)
                return;
            UISceneGlobal.Singleton.ShowDisconnectDialog(DisconnectReason.ConnectionRejected, SocketError.Success, responseMessage, "ReadExtraClientReadyResponse");
            StopClient();
        }

        protected virtual void UpdateOnlineCharacter(BasePlayerCharacterEntity playerCharacterEntity)
        {
            ServerCharacterHandlers.MarkOnlineCharacter(playerCharacterEntity.Id);
        }

        protected virtual void UpdateOnlineCharacters()
        {
            Dictionary<long, PartyData> updatingPartyMembers = new Dictionary<long, PartyData>();
            Dictionary<long, GuildData> updatingGuildMembers = new Dictionary<long, GuildData>();

            PartyData tempParty;
            GuildData tempGuild;
            foreach (BasePlayerCharacterEntity playerCharacter in ServerUserHandlers.GetPlayerCharacters())
            {
                if (playerCharacter == null)
                    continue;

                UpdateOnlineCharacter(playerCharacter);

                if (playerCharacter.PartyId > 0 && ServerPartyHandlers.TryGetParty(playerCharacter.PartyId, out tempParty) && tempParty != null)
                {
                    tempParty.UpdateMember(playerCharacter);
                    if (!updatingPartyMembers.ContainsKey(playerCharacter.ConnectionId))
                        updatingPartyMembers.Add(playerCharacter.ConnectionId, tempParty);
                }

                if (playerCharacter.GuildId > 0 && ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out tempGuild) && tempGuild != null)
                {
                    tempGuild.UpdateMember(playerCharacter);
                    if (!updatingGuildMembers.ContainsKey(playerCharacter.ConnectionId))
                        updatingGuildMembers.Add(playerCharacter.ConnectionId, tempGuild);
                }
            }

            foreach (long connectionId in updatingPartyMembers.Keys)
            {
                ServerGameMessageHandlers.SendUpdatePartyMembersToOne(connectionId, updatingPartyMembers[connectionId]);
            }

            foreach (long connectionId in updatingGuildMembers.Keys)
            {
                ServerGameMessageHandlers.SendUpdateGuildMembersToOne(connectionId, updatingGuildMembers[connectionId]);
            }
        }

        protected virtual void HandleWarpAtClient(MessageHandlerData messageHandler)
        {
            ClientGenericActions.ClientWarp();
        }

        protected virtual void HandleChatAtClient(MessageHandlerData messageHandler)
        {
            ClientGenericActions.ClientReceiveChatMessage(messageHandler.ReadMessage<ChatMessage>());
        }

        protected void HandleUpdateDayNightTimeAtClient(MessageHandlerData messageHandler)
        {
            // Don't set time of day again at server
            if (IsServer)
                return;
            UpdateTimeOfDayMessage message = messageHandler.ReadMessage<UpdateTimeOfDayMessage>();
            CurrentGameInstance.DayNightTimeUpdater.SetTimeOfDay(message.timeOfDay);
        }

        protected void HandleUpdateMapInfoAtClient(MessageHandlerData messageHandler)
        {
            // Don't set map info again at server
            if (IsServer)
                return;
            UpdateMapInfoMessage message = messageHandler.ReadMessage<UpdateMapInfoMessage>();
            SetMapInfo(message.mapName);
            if (MapInfo == null)
            {
                Logging.LogError(LogTag, $"Cannot find map info: {message.mapName}, it will create new map info to use, it can affect players' experience.");
                MapInfo = ScriptableObject.CreateInstance<MapInfo>();
                MapInfo.Id = message.mapName;
                return;
            }
            if (!MapInfo.GetType().FullName.Equals(message.className))
            {
                Logging.LogError(LogTag, $"Invalid map info expect: {message.className}, found {MapInfo.GetType().FullName}, it can affect players' experience.");
                return;
            }
            MapInfo.Deserialize(messageHandler.Reader);
            this.InvokeInstanceDevExtMethods("ReadMapInfoExtra", messageHandler.Reader);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.ReadMapInfoExtra(this, messageHandler.Reader);
            }
        }

        protected virtual void HandleChatAtServer(MessageHandlerData messageHandler)
        {
            ChatMessage message = messageHandler.ReadMessage<ChatMessage>().FillChannelId();
            // Get character
            IPlayerCharacterData playerCharacter = null;
            if (!string.IsNullOrEmpty(message.senderName))
                ServerUserHandlers.TryGetPlayerCharacterByName(message.senderName, out playerCharacter);
            // Set guild data
            if (playerCharacter != null)
            {
                message.senderId = playerCharacter.Id;
                message.senderUserId = playerCharacter.UserId;
                message.senderName = playerCharacter.CharacterName;
                if (ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out GuildData guildData))
                {
                    message.guildId = playerCharacter.GuildId;
                    message.guildName = guildData.guildName;
                }
            }
            // Character muted?
            if (!message.sendByServer && playerCharacter != null)
            {
                if (playerCharacter.IsMuting())
                {
                    if (ServerUserHandlers.TryGetConnectionIdById(playerCharacter.Id, out long connectionId))
                        ServerGameMessageHandlers.SendGameMessage(connectionId, UITextKeys.UI_ERROR_CHAT_MUTED);
                    return;
                }
                if (ServerChatHandlers.ChatTooFast(playerCharacter.Id))
                {
                    ServerChatHandlers.ChatFlooded(playerCharacter.Id);
                    if (ServerUserHandlers.TryGetConnectionIdById(playerCharacter.Id, out long connectionId))
                        ServerGameMessageHandlers.SendGameMessage(connectionId, UITextKeys.UI_ERROR_CHAT_ENTER_TOO_FAST);
                    return;
                }
            }
            if (message.channel != ChatChannel.System || ServerChatHandlers.CanSendSystemAnnounce(message.senderName))
            {
                ServerChatHandlers.OnChatMessage(message);
                ServerLogHandlers.LogEnterChat(message);
            }
        }

        protected void HandleClientEntityStateAtServer(MessageHandlerData messageHandler)
        {
            uint objectId = messageHandler.Reader.GetPackedUInt();
            long peerTimestamp = messageHandler.Reader.GetPackedLong();
            if (Assets.TryGetSpawnedObject(objectId, out BaseGameEntity gameEntity) && gameEntity.Identity.ConnectionId == messageHandler.ConnectionId)
                gameEntity.ReadClientStateAtServer(peerTimestamp, messageHandler.Reader);
        }

        protected void HandleServerEntityStateAtClient(MessageHandlerData messageHandler)
        {
            uint objectId = messageHandler.Reader.GetPackedUInt();
            long peerTimestamp = messageHandler.Reader.GetPackedLong();
            if (Assets.TryGetSpawnedObject(objectId, out BaseGameEntity gameEntity))
                gameEntity.ReadServerStateAtClient(peerTimestamp, messageHandler.Reader);
        }

        public virtual void InitPrefabs()
        {
#if !DISABLE_ADDRESSABLES
            Assets.addressableOfflineScene = null;
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            Assets.offlineScene = default;
#endif
            if (CurrentGameInstance.GetHomeScene(out SceneField scene
#if !DISABLE_ADDRESSABLES
                , out AssetReferenceScene addressableScene
#endif
                ))
            {
#if !DISABLE_ADDRESSABLES
                Assets.addressableOfflineScene = addressableScene;
#endif
            }
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            else
            {
                Assets.offlineScene = scene;
            }
#endif
            // Prepare networking prefabs
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            Assets.playerPrefab = null;
            HashSet<LiteNetLibIdentity> spawnablePrefabs = new HashSet<LiteNetLibIdentity>(Assets.spawnablePrefabs);
            if (CurrentGameInstance.ItemDropEntityPrefab != null)
                spawnablePrefabs.Add(CurrentGameInstance.ItemDropEntityPrefab.Identity);
            if (CurrentGameInstance.ExpDropEntityPrefab != null)
                spawnablePrefabs.Add(CurrentGameInstance.ExpDropEntityPrefab.Identity);
            if (CurrentGameInstance.GoldDropEntityPrefab != null)
                spawnablePrefabs.Add(CurrentGameInstance.GoldDropEntityPrefab.Identity);
            if (CurrentGameInstance.CurrencyDropEntityPrefab != null)
                spawnablePrefabs.Add(CurrentGameInstance.CurrencyDropEntityPrefab.Identity);
            if (CurrentGameInstance.WarpPortalEntityPrefab != null)
                spawnablePrefabs.Add(CurrentGameInstance.WarpPortalEntityPrefab.Identity);
            if (CurrentGameInstance.PlayerCorpsePrefab != null)
                spawnablePrefabs.Add(CurrentGameInstance.PlayerCorpsePrefab.Identity);
            if (CurrentGameInstance.MonsterCorpsePrefab != null)
                spawnablePrefabs.Add(CurrentGameInstance.MonsterCorpsePrefab.Identity);
            foreach (PlayerCharacterEntityMetaData entry in GameInstance.PlayerCharacterEntityMetaDataList.Values)
            {
                if (entry.EntityPrefab != null)
                    spawnablePrefabs.Add(entry.EntityPrefab.Identity);
            }
            foreach (BasePlayerCharacterEntity entry in GameInstance.PlayerCharacterEntities.Values)
            {
                spawnablePrefabs.Add(entry.Identity);
            }
            foreach (BaseMonsterCharacterEntity entry in GameInstance.MonsterCharacterEntities.Values)
            {
                spawnablePrefabs.Add(entry.Identity);
            }
            foreach (VehicleEntity entry in GameInstance.VehicleEntities.Values)
            {
                spawnablePrefabs.Add(entry.Identity);
            }
            foreach (WarpPortalEntity entry in GameInstance.WarpPortalEntities.Values)
            {
                spawnablePrefabs.Add(entry.Identity);
            }
            foreach (NpcEntity entry in GameInstance.NpcEntities.Values)
            {
                spawnablePrefabs.Add(entry.Identity);
            }
            foreach (BuildingEntity entry in GameInstance.BuildingEntities.Values)
            {
                spawnablePrefabs.Add(entry.Identity);
            }
            foreach (LiteNetLibIdentity identity in GameInstance.OtherNetworkObjectPrefabs.Values)
            {
                spawnablePrefabs.Add(identity);
            }
            Assets.spawnablePrefabs = new LiteNetLibIdentity[spawnablePrefabs.Count];
            spawnablePrefabs.CopyTo(Assets.spawnablePrefabs);
#endif

#if !DISABLE_ADDRESSABLES
            Assets.addressablePlayerPrefab = null;
            HashSet<AssetReferenceLiteNetLibIdentity> addressableSpawnablePrefabs = new HashSet<AssetReferenceLiteNetLibIdentity>(Assets.addressableSpawnablePrefabs);
            if (CurrentGameInstance.AddressableItemDropEntityPrefab.IsDataValid())
                addressableSpawnablePrefabs.Add(CurrentGameInstance.AddressableItemDropEntityPrefab);
            if (CurrentGameInstance.AddressableExpDropEntityPrefab.IsDataValid())
                addressableSpawnablePrefabs.Add(CurrentGameInstance.AddressableExpDropEntityPrefab);
            if (CurrentGameInstance.AddressableGoldDropEntityPrefab.IsDataValid())
                addressableSpawnablePrefabs.Add(CurrentGameInstance.AddressableGoldDropEntityPrefab);
            if (CurrentGameInstance.AddressableCurrencyDropEntityPrefab.IsDataValid())
                addressableSpawnablePrefabs.Add(CurrentGameInstance.AddressableCurrencyDropEntityPrefab);
            if (CurrentGameInstance.AddressableWarpPortalEntityPrefab.IsDataValid())
                addressableSpawnablePrefabs.Add(CurrentGameInstance.AddressableWarpPortalEntityPrefab);
            if (CurrentGameInstance.AddressablePlayerCorpsePrefab.IsDataValid())
                addressableSpawnablePrefabs.Add(CurrentGameInstance.AddressablePlayerCorpsePrefab);
            if (CurrentGameInstance.AddressableMonsterCorpsePrefab.IsDataValid())
                addressableSpawnablePrefabs.Add(CurrentGameInstance.AddressableMonsterCorpsePrefab);
            foreach (PlayerCharacterEntityMetaData entry in GameInstance.PlayerCharacterEntityMetaDataList.Values)
            {
                if (entry.AddressableEntityPrefab.IsDataValid())
                    addressableSpawnablePrefabs.Add(entry.AddressableEntityPrefab);
            }
            foreach (AssetReferenceLiteNetLibBehaviour<BasePlayerCharacterEntity> entry in GameInstance.AddressablePlayerCharacterEntities.Values)
            {
                addressableSpawnablePrefabs.Add(entry);
            }
            foreach (AssetReferenceLiteNetLibBehaviour<BaseMonsterCharacterEntity> entry in GameInstance.AddressableMonsterCharacterEntities.Values)
            {
                addressableSpawnablePrefabs.Add(entry);
            }
            foreach (AssetReferenceLiteNetLibBehaviour<VehicleEntity> entry in GameInstance.AddressableVehicleEntities.Values)
            {
                addressableSpawnablePrefabs.Add(entry);
            }
            foreach (AssetReferenceLiteNetLibBehaviour<WarpPortalEntity> entry in GameInstance.AddressableWarpPortalEntities.Values)
            {
                addressableSpawnablePrefabs.Add(entry);
            }
            foreach (AssetReferenceLiteNetLibBehaviour<NpcEntity> entry in GameInstance.AddressableNpcEntities.Values)
            {
                addressableSpawnablePrefabs.Add(entry);
            }
            foreach (AssetReferenceLiteNetLibBehaviour<BuildingEntity> entry in GameInstance.AddressableBuildingEntities.Values)
            {
                addressableSpawnablePrefabs.Add(entry);
            }
            foreach (AssetReferenceLiteNetLibIdentity identity in GameInstance.AddressableOtherNetworkObjectPrefabs.Values)
            {
                addressableSpawnablePrefabs.Add(identity);
            }
            Assets.addressableSpawnablePrefabs = new AssetReferenceLiteNetLibIdentity[addressableSpawnablePrefabs.Count];
            addressableSpawnablePrefabs.CopyTo(Assets.addressableSpawnablePrefabs);
#endif

            this.InvokeInstanceDevExtMethods("InitPrefabs");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.InitPrefabs(this);
            }
        }

        public void Quit()
        {
            Application.Quit();
        }

        private void RegisterEntities()
        {
            MonsterSpawnArea[] monsterSpawnAreas = FindObjectsOfType<MonsterSpawnArea>();
            foreach (MonsterSpawnArea monsterSpawnArea in monsterSpawnAreas)
            {
                monsterSpawnArea.RegisterPrefabs();
            }

            HarvestableSpawnArea[] harvestableSpawnAreas = FindObjectsOfType<HarvestableSpawnArea>();
            foreach (HarvestableSpawnArea harvestableSpawnArea in harvestableSpawnAreas)
            {
                harvestableSpawnArea.RegisterPrefabs();
            }

            ItemDropSpawnArea[] itemDropSpawnAreas = FindObjectsOfType<ItemDropSpawnArea>();
            foreach (ItemDropSpawnArea itemDropSpawnArea in itemDropSpawnAreas)
            {
                itemDropSpawnArea.RegisterPrefabs();
            }

            // Register scene entities
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddMonsterCharacterEntities(FindObjectsOfType<BaseMonsterCharacterEntity>());
            GameInstance.AddHarvestableEntities(FindObjectsOfType<HarvestableEntity>());
            GameInstance.AddItemDropEntities(FindObjectsOfType<ItemDropEntity>());
#endif

            PoolSystem.Clear();
            System.GC.Collect();
        }

        protected override void HandleEnterGameResponse(ResponseHandlerData responseHandler, AckResponseCode responseCode, EnterGameResponseMessage response)
        {
            base.HandleEnterGameResponse(responseHandler, responseCode, response);
            this.InvokeInstanceDevExtMethods("HandleEnterGameResponse", responseHandler, responseCode, response);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.HandleEnterGameResponse(this, responseHandler, responseCode, response);
            }
            ClientGenericActions.OnEnterGameResponse(responseCode);
        }

        protected override void HandleClientReadyResponse(ResponseHandlerData responseHandler, AckResponseCode responseCode, EmptyMessage response)
        {
            base.HandleClientReadyResponse(responseHandler, responseCode, response);
            this.InvokeInstanceDevExtMethods("HandleClientReadyResponse", responseHandler, responseCode, response);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.HandleClientReadyResponse(this, responseHandler, responseCode, response);
            }
            ClientGenericActions.OnClientReadyResponse(responseCode);
            if (responseCode != AckResponseCode.Success)
                OnClientConnectionRefused();
        }

        public override void OnClientConnectionRefused()
        {
            base.OnClientConnectionRefused();
            UISceneGlobal.Singleton.ShowDisconnectDialog(DisconnectReason.ConnectionRejected, SocketError.Success, UITextKeys.NONE, "OnClientConnectionRefused");
        }

        public override void OnClientOnlineSceneLoaded()
        {
            this.InvokeInstanceDevExtMethods("OnClientOnlineSceneLoaded");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnClientOnlineSceneLoaded(this);
            }
            _clientSceneLoadedTime = Time.unscaledTime;
            // Server will register entities later, so don't register entities now
            if (!IsServer)
                RegisterEntities();
            ProceedUntilClientReady().Forget();
        }

        public override void OnServerOnlineSceneLoaded()
        {
            this.InvokeInstanceDevExtMethods("OnServerOnlineSceneLoaded");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.OnServerOnlineSceneLoaded(this);
            }
            _serverSceneLoadedTime = Time.unscaledTime;
            _serverReadyToInstantiateObjectsStates.Clear();
            _isServerReadyToInstantiateObjects = false;
            _isServerReadyToInstantiatePlayers = false;
            SpawnEntities().Forget();
        }

        public override void ServerSceneChange(ServerSceneInfo serverSceneInfo)
        {
            if (!IsServer)
                return;
            _serverReadyToInstantiateObjectsStates.Clear();
            _isServerReadyToInstantiateObjects = false;
            _isServerReadyToInstantiatePlayers = false;
            base.ServerSceneChange(serverSceneInfo);
        }

        protected virtual async UniTaskVoid SpawnEntities()
        {
            do { await UniTask.Delay(100); } while (!IsServerReadyToInstantiateObjects());
            float progress = 0f;
            string sceneName = SceneManager.GetActiveScene().name;
            await UniTask.NextFrame();
            onSpawnEntitiesStart.Invoke(sceneName, false, true, progress);
            await PreSpawnEntities();
            RegisterEntities();
            await UniTask.NextFrame();
            int i;
            LiteNetLibIdentity spawnObj;
            // Spawn Warp Portals
            if (LogInfo)
                Logging.Log(LogTag, "Spawning warp portals");
            if (GameInstance.MapWarpPortals.Count > 0)
            {
                if (GameInstance.MapWarpPortals.TryGetValue(MapInfo.Id, out List<WarpPortal> mapWarpPortals))
                {
                    WarpPortal warpPortal;
                    WarpPortalEntity warpPortalPrefab;
#if !DISABLE_ADDRESSABLES
                    AssetReferenceWarpPortalEntity addressableWarpPortalPrefab;
#endif
                    WarpPortalEntity warpPortalEntity;
                    for (i = 0; i < mapWarpPortals.Count; ++i)
                    {
                        warpPortal = mapWarpPortals[i];
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                        warpPortalPrefab = warpPortal.entityPrefab != null ? warpPortal.entityPrefab : CurrentGameInstance.WarpPortalEntityPrefab;
#else
                        warpPortalPrefab = null;
#endif
#if !DISABLE_ADDRESSABLES
                        addressableWarpPortalPrefab = warpPortal.addressableEntityPrefab.IsDataValid() ? warpPortal.addressableEntityPrefab : CurrentGameInstance.AddressableWarpPortalEntityPrefab;
#endif
                        spawnObj = null;
                        if (warpPortalPrefab != null)
                        {
                            spawnObj = Assets.GetObjectInstance(
                                warpPortalPrefab.Identity.HashAssetId, warpPortal.position,
                                Quaternion.Euler(warpPortal.rotation));
                        }
#if !DISABLE_ADDRESSABLES
                        else if (addressableWarpPortalPrefab.IsDataValid())
                        {
                            spawnObj = Assets.GetObjectInstance(
                                addressableWarpPortalPrefab.HashAssetId, warpPortal.position,
                                Quaternion.Euler(warpPortal.rotation));
                        }
#endif
                        if (spawnObj != null)
                        {
                            warpPortalEntity = spawnObj.GetComponent<WarpPortalEntity>();
                            warpPortalEntity.WarpPortalType = warpPortal.warpPortalType;
                            warpPortalEntity.WarpToMapInfo = warpPortal.warpToMapInfo;
                            warpPortalEntity.WarpToPosition = warpPortal.warpToPosition;
                            warpPortalEntity.WarpOverrideRotation = warpPortal.warpOverrideRotation;
                            warpPortalEntity.WarpToRotation = warpPortal.warpToRotation;
                            warpPortalEntity.WarpPointsByCondition = warpPortal.warpPointsByCondition;
                            Assets.NetworkSpawn(spawnObj);
                        }
                        await UniTask.NextFrame();
                        progress = 0f + ((float)i / (float)mapWarpPortals.Count * 0.25f);
                        onSpawnEntitiesProgress.Invoke(sceneName, false, true, progress);
                    }
                }
            }
            await UniTask.NextFrame();
            progress = 0.25f;
            onSpawnEntitiesProgress.Invoke(sceneName, false, true, progress);
            // Spawn Npcs
            if (LogInfo)
                Logging.Log(LogTag, "Spawning NPCs");
            if (GameInstance.MapNpcs.Count > 0)
            {
                if (GameInstance.MapNpcs.TryGetValue(MapInfo.Id, out List<Npc> mapNpcs))
                {
                    Npc npc;
                    NpcEntity npcPrefab;
#if !DISABLE_ADDRESSABLES
                    AssetReferenceNpcEntity addressableNpcPrefab;
#endif
                    NpcEntity npcEntity;
                    for (i = 0; i < mapNpcs.Count; ++i)
                    {
                        npc = mapNpcs[i];
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                        npcPrefab = npc.entityPrefab;
#else
                        npcPrefab = null;
#endif
#if !DISABLE_ADDRESSABLES
                        addressableNpcPrefab = npc.addressableEntityPrefab;
#endif
                        spawnObj = null;
                        if (npcPrefab != null)
                        {
                            spawnObj = Assets.GetObjectInstance(
                                npcPrefab.Identity.HashAssetId, npc.position,
                                Quaternion.Euler(npc.rotation));
                        }
#if !DISABLE_ADDRESSABLES
                        else if (addressableNpcPrefab.IsDataValid())
                        {
                            spawnObj = Assets.GetObjectInstance(
                                addressableNpcPrefab.HashAssetId, npc.position,
                                Quaternion.Euler(npc.rotation));
                        }
#endif
                        if (spawnObj != null)
                        {
                            npcEntity = spawnObj.GetComponent<NpcEntity>();
                            npcEntity.Title = npc.title;
                            npcEntity.StartDialog = npc.startDialog;
                            npcEntity.Graph = npc.graph;
                            Assets.NetworkSpawn(spawnObj);
                        }
                        await UniTask.NextFrame();
                        progress = 0.25f + ((float)i / (float)mapNpcs.Count * 0.25f);
                        onSpawnEntitiesProgress.Invoke(sceneName, false, true, progress);
                    }
                }
            }
            await UniTask.NextFrame();
            progress = 0.5f;
            onSpawnEntitiesProgress.Invoke(sceneName, false, true, progress);
            // Spawn entities
            if (LogInfo)
                Logging.Log(LogTag, "Spawning entities");
            List<GameSpawnArea> gameSpawnAreas = GenericUtils.GetComponentsFromAllLoadedScenes<GameSpawnArea>(false);
            for (i = 0; i < gameSpawnAreas.Count; ++i)
            {
                gameSpawnAreas[i].SpawnFirstTime();
                await UniTask.NextFrame();
                progress = 0.5f + ((float)i / (float)gameSpawnAreas.Count * 0.5f);
                onSpawnEntitiesProgress.Invoke(sceneName, false, true, progress);
            }
            await UniTask.NextFrame();
            progress = 1f;
            onSpawnEntitiesProgress.Invoke(sceneName, false, true, progress);
            // If it's server (not host) spawn simple camera controller
            if (!IsClient && GameInstance.Singleton.serverCharacterPrefab != null &&
                SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
            {
                if (LogInfo)
                    Logging.Log(LogTag, "Spawning server character");
                Instantiate(GameInstance.Singleton.serverCharacterPrefab, MapInfo.StartPosition, Quaternion.identity);
            }
            await UniTask.NextFrame();
            // Entities were spawned
            progress = 1f;
            onSpawnEntitiesFinish.Invoke(sceneName, false, true, progress);
            await PostSpawnEntities();
            _isServerReadyToInstantiatePlayers = true;
        }

        protected virtual UniTask PreSpawnEntities()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask PostSpawnEntities()
        {
            return UniTask.CompletedTask;
        }

        public bool IsServerReadyToInstantiateObjects()
        {
            if (!_isServerReadyToInstantiateObjects)
            {
                _serverReadyToInstantiateObjectsStates[INSTANTIATES_OBJECTS_DELAY_STATE_KEY] = Time.unscaledTime - _serverSceneLoadedTime >= INSTANTIATES_OBJECTS_DELAY;
                // NOTE: Make it works with old version 
                this.InvokeInstanceDevExtMethods("UpdateReadyToInstantiateObjectsStates", _serverReadyToInstantiateObjectsStates);
                this.InvokeInstanceDevExtMethods("UpdateServerReadyToInstantiateObjectsStates", _serverReadyToInstantiateObjectsStates);
                foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
                {
                    component.UpdateReadyToInstantiateObjectsStates(this, _serverReadyToInstantiateObjectsStates);
                    component.UpdateServerReadyToInstantiateObjectsStates(this, _serverReadyToInstantiateObjectsStates);
                }
                foreach (bool value in _serverReadyToInstantiateObjectsStates.Values)
                {
                    if (!value)
                        return false;
                }
                _isServerReadyToInstantiateObjects = true;
            }
            return true;
        }

        protected virtual async UniTaskVoid ProceedUntilClientReady()
        {
            do
            {
                await UniTask.Delay(100);
            }
            while (!IsClientReadyToInstantiateObjects());
            SendClientReady();
        }

        public bool IsClientReadyToInstantiateObjects()
        {
            if (!_isClientReadyToInstantiateObjects)
            {
                _clientReadyToInstantiateObjectsStates[INSTANTIATES_OBJECTS_DELAY_STATE_KEY] = Time.unscaledTime - _clientSceneLoadedTime >= INSTANTIATES_OBJECTS_DELAY;
                this.InvokeInstanceDevExtMethods("UpdateClientReadyToInstantiateObjectsStates", _clientReadyToInstantiateObjectsStates);
                foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
                {
                    component.UpdateClientReadyToInstantiateObjectsStates(this, _clientReadyToInstantiateObjectsStates);
                }
                foreach (bool value in _clientReadyToInstantiateObjectsStates.Values)
                {
                    if (!value)
                        return false;
                }
                _isClientReadyToInstantiateObjects = true;
            }
            return true;
        }

        public virtual void RegisterPlayerCharacter(long connectionId, BasePlayerCharacterEntity playerCharacter)
        {
            bool success = ServerUserHandlers.AddPlayerCharacter(connectionId, playerCharacter);
            if (success)
            {
                ServerLogHandlers.LogEnterGame(playerCharacter);
                onRegisterCharacter?.Invoke(connectionId, playerCharacter);
            }
        }

        public virtual void UnregisterPlayerCharacter(long connectionId)
        {
            ServerStorageHandlers.CloseAllStorages(connectionId);
            bool success = ServerUserHandlers.RemovePlayerCharacter(connectionId, out string characterId, out string userId);
            if (success)
            {
                ServerLogHandlers.LogExitGame(characterId, userId);
                onUnregisterCharacter?.Invoke(connectionId, characterId, userId);
            }
        }

        public virtual void UnregisterPlayerCharacterById(string characterId)
        {
            if (ServerUserHandlers.TryGetConnectionIdById(characterId, out long connectionId))
                UnregisterPlayerCharacter(connectionId);
        }

        public virtual void UnregisterPlayerCharacterByUserId(string userId)
        {
            if (ServerUserHandlers.TryGetConnectionIdByUserId(userId, out long connectionId))
                UnregisterPlayerCharacter(connectionId);
        }

        public virtual void RegisterUserIdAndAccessToken(long connectionId, string userId, string accessToken)
        {
            bool success = ServerUserHandlers.AddUserId(connectionId, userId);
            if (success)
            {
                ServerUserHandlers.AddAccessToken(connectionId, accessToken);
                onRegisterUser?.Invoke(connectionId, userId);
            }
        }

        public virtual void UnregisterUserIdAndAccessToken(long connectionId)
        {
            bool success = ServerUserHandlers.RemoveUserId(connectionId, out string userId);
            if (success)
            {
                ServerUserHandlers.RemoveAccessToken(connectionId, out _);
                onUnregisterUser?.Invoke(connectionId, userId);
            }
        }

        public virtual void UnregisterUserIdAndAccessTokenById(string characterId)
        {
            if (ServerUserHandlers.TryGetConnectionIdById(characterId, out long connectionId))
                UnregisterUserIdAndAccessToken(connectionId);
        }

        public virtual void UnregisterUserIdAndAccessTokenByUserId(string userId)
        {
            if (ServerUserHandlers.TryGetConnectionIdByUserId(userId, out long connectionId))
                UnregisterUserIdAndAccessToken(connectionId);
        }

        public virtual async UniTask<BuildingEntity> CreateBuildingEntity(BuildingSaveData saveData, bool initialize)
        {
            await UniTask.Yield();
            LiteNetLibIdentity spawnObj;
#if !DISABLE_ADDRESSABLES
            if (GameInstance.AddressableBuildingEntities.TryGetValue(saveData.EntityId, out AssetReferenceLiteNetLibBehaviour<BuildingEntity> addressablePrefab))
            {
                spawnObj = Assets.GetObjectInstance(
                    addressablePrefab.HashAssetId,
                    saveData.Position, Quaternion.Euler(saveData.Rotation));
            }
#else
            if (false) { }
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            else if (GameInstance.BuildingEntities.TryGetValue(saveData.EntityId, out BuildingEntity prefab))
            {
                spawnObj = Assets.GetObjectInstance(
                    prefab.Identity.HashAssetId,
                    saveData.Position, Quaternion.Euler(saveData.Rotation));
            }
#endif
            else
            {
                return null;
            }

            if (spawnObj == null)
            {
                return null;
            }

            BuildingEntity buildingEntity = spawnObj.GetComponent<BuildingEntity>();
            buildingEntity.Id = saveData.Id;
            buildingEntity.ParentId = saveData.ParentId;
            buildingEntity.CurrentHp = saveData.CurrentHp;
            buildingEntity.RemainsLifeTime = saveData.RemainsLifeTime;
            buildingEntity.IsLocked = saveData.IsLocked;
            buildingEntity.LockPassword = saveData.LockPassword;
            buildingEntity.CreatorId = saveData.CreatorId;
            buildingEntity.CreatorName = saveData.CreatorName;
            buildingEntity.ExtraData = saveData.ExtraData;
            Assets.NetworkSpawn(spawnObj);
            ServerBuildingHandlers.AddBuilding(buildingEntity.Id, buildingEntity);
            buildingEntity.CallRpcOnBuildingConstruct();
            return buildingEntity;
        }

        public virtual async UniTask DestroyBuildingEntity(string id, bool isSceneObject)
        {
            await UniTask.Yield();
            if (!isSceneObject)
                ServerBuildingHandlers.RemoveBuilding(id);
        }

        public void SetMapInfo(string mapName)
        {
            if (!GameInstance.MapInfos.TryGetValue(mapName, out BaseMapInfo mapInfo))
            {
                MapInfo = null;
                return;
            }
            SetMapInfo(mapInfo);
        }

        public void SetMapInfo(BaseMapInfo mapInfo)
        {
            if (mapInfo == null)
                return;
            MapInfo = mapInfo;
            SendMapInfo();
        }

        public void SendMapInfo()
        {
            if (!IsServer)
                return;
            foreach (long connectionId in Server.ConnectionIds)
            {
                SendMapInfo(connectionId);
            }
        }

        public void SendMapInfo(long connectionId)
        {
            if (!IsServer || MapInfo == null)
                return;
            ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableOrdered, GameNetworkingConsts.UpdateMapInfo, WriteMapInfo);
        }

        public void WriteMapInfo(NetDataWriter writer)
        {
            writer.Put(new UpdateMapInfoMessage()
            {
                mapName = MapInfo.Id,
                className = MapInfo.GetType().FullName,
            });
            MapInfo.Serialize(writer);
            this.InvokeInstanceDevExtMethods("WriteMapInfoExtra", writer);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.WriteMapInfoExtra(this, writer);
            }
        }

        public void ReadMapInfo(NetDataReader reader)
        {
            UpdateMapInfoMessage message = reader.Get<UpdateMapInfoMessage>();
            SetMapInfo(message.mapName);
            if (MapInfo == null)
            {
                Logging.LogError(LogTag, $"Cannot find map info: {message.mapName}, it will create new map info to use, it can affect players' experience.");
                MapInfo = ScriptableObject.CreateInstance<MapInfo>();
                MapInfo.Id = message.mapName;
                return;
            }
            if (!MapInfo.GetType().FullName.Equals(message.className))
            {
                Logging.LogError(LogTag, $"Invalid map info expect: {message.className}, found {MapInfo.GetType().FullName}, it can affect players' experience.");
                return;
            }
            MapInfo.Deserialize(reader);
            this.InvokeInstanceDevExtMethods("ReadMapInfoExtra", reader);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.ReadMapInfoExtra(this, reader);
            }
        }

        public void SendTimeOfDay()
        {
            if (!IsServer)
                return;
            foreach (long connectionId in Server.ConnectionIds)
            {
                SendTimeOfDay(connectionId);
            }
        }

        public void SendTimeOfDay(long connectionId)
        {
            if (!IsServer)
                return;
            ServerSendPacket(connectionId, 0, DeliveryMethod.Unreliable, GameNetworkingConsts.UpdateTimeOfDay, WriteTimeOfDay);
        }

        public void WriteTimeOfDay(NetDataWriter writer)
        {
            writer.Put(new UpdateTimeOfDayMessage()
            {
                timeOfDay = CurrentGameInstance.DayNightTimeUpdater.TimeOfDay,
            });
        }

        public void ReadTimeOfDay(NetDataReader reader)
        {
            UpdateTimeOfDayMessage message = reader.Get<UpdateTimeOfDayMessage>();
            CurrentGameInstance.DayNightTimeUpdater.SetTimeOfDay(message.timeOfDay);
        }

        public void SendServerInfo()
        {
            if (!IsServer)
                return;
            foreach (long connectionId in Server.ConnectionIds)
            {
                SendServerInfo(connectionId);
            }
        }

        public void SendServerInfo(long connectionId)
        {
            if (!IsServer)
                return;
            ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableOrdered, GameNetworkingConsts.UpdateServerInfo, WriteServerInfo);
        }

        public void WriteServerInfo(NetDataWriter writer)
        {
            writer.Put(new UpdateServerInfoMessage()
            {
                channelId = ChannelId,
                channelTitle = ChannelTitle,
                channelDescription = ChannelDescription,
            });
        }

        public void ReadServerInfo(NetDataReader reader)
        {
            UpdateServerInfoMessage message = reader.Get<UpdateServerInfoMessage>();
            ChannelId = message.channelId;
            ChannelTitle = message.channelTitle;
            ChannelDescription = message.channelDescription;
        }

        public void ServerSendSystemAnnounce(string message)
        {
            if (!IsServer)
                return;
            s_Writer.Reset();
            s_Writer.Put(new ChatMessage()
            {
                channel = ChatChannel.System,
                senderName = CHAT_SYSTEM_ANNOUNCER_SENDER,
                message = message,
                sendByServer = true,
            });
            HandleChatAtServer(new MessageHandlerData(GameNetworkingConsts.Chat, Server, -1, new NetDataReader(s_Writer.Data, 0, s_Writer.Length)));
        }

        public void ServerSendLocalMessage(string sender, string message)
        {
            if (!IsServer)
                return;
            s_Writer.Reset();
            s_Writer.Put(new ChatMessage()
            {
                channel = ChatChannel.Local,
                senderName = sender,
                message = message,
                sendByServer = true,
            });
            HandleChatAtServer(new MessageHandlerData(GameNetworkingConsts.Chat, Server, -1, new NetDataReader(s_Writer.Data, 0, s_Writer.Length)));
        }

        public async void KickClient(long connectionId, UITextKeys message)
        {
            if (!IsServer)
                return;
            s_Writer.Reset();
            s_Writer.PutPackedUShort((ushort)message);
            if (!await KickClient(connectionId, s_Writer.CopyData()))
            {
                // Force remove client data
                Logging.LogWarning(LogTag, $"Unable to kick client, Force removing client data for connectionId: {connectionId}");
                UnregisterPlayerCharacter(connectionId);
                UnregisterUserIdAndAccessToken(connectionId);
            }
        }

        public virtual async UniTask<AsyncResponseData<EmptyMessage>> SendClientSafeDisconnect()
        {
            this.InvokeInstanceDevExtMethods("SendClientSafeDisconnect");
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.SendClientSafeDisconnect(this);
            }
            return await ClientSendRequestAsync<EmptyMessage, EmptyMessage>(GameNetworkingConsts.SafeDisconnect, EmptyMessage.Value);
        }

        protected virtual UniTaskVoid HandleSafeDisconnectRequest(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<EmptyMessage> result)
        {
            result.InvokeSuccess(EmptyMessage.Value);
            return default;
        }

        protected virtual void HandleSafeDisconnectResponse(
            ResponseHandlerData responseHandler,
            AckResponseCode responseCode,
            EmptyMessage response)
        {
            this.InvokeInstanceDevExtMethods("HandleSafeDisconnectResponse", responseHandler, responseCode, response);
            foreach (BaseGameNetworkManagerComponent component in ManagerComponents)
            {
                component.HandleSafeDisconnectResponse(this, responseHandler, responseCode, response);
            }
        }
    }
}
