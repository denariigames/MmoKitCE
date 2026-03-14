using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using NotifiableCollection;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        #region Sync data
        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldInt dataId = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt factionId = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldFloat statPoint = new SyncFieldFloat();
        [SerializeField]
        protected SyncFieldFloat skillPoint = new SyncFieldFloat();
        [SerializeField]
        protected SyncFieldInt gold = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt userGold = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt userCash = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt partyId = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt guildId = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldString persistChannel = new SyncFieldString();
        [SerializeField]
        protected SyncFieldString persistMapName = new SyncFieldString();
        [SerializeField]
        protected SyncFieldVector3 persistPosition = new SyncFieldVector3();
        [SerializeField]
        protected SyncFieldString persistSafeArea = new SyncFieldString();
#if !DISABLE_DIFFER_MAP_RESPAWNING
        [SerializeField]
        protected SyncFieldString respawnMapName = new SyncFieldString();
        [SerializeField]
        protected SyncFieldVector3 respawnPosition = new SyncFieldVector3();
#endif
        [SerializeField]
        protected SyncFieldInt iconDataId = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt frameDataId = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt backgroundDataId = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt titleDataId = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldLong lastDeadTime = new SyncFieldLong();
        [SerializeField]
        protected SyncFieldLong unmuteTime = new SyncFieldLong();
#if !DISABLE_CLASSIC_PK
        [SerializeField]
        protected SyncFieldBool isPkOn = new SyncFieldBool();
        [SerializeField]
        protected SyncFieldInt pkPoint = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt consecutivePkKills = new SyncFieldInt();
#endif
        [SerializeField]
        protected SyncFieldInt reputation = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldBool isWarping = new SyncFieldBool();

        [Category("Sync Lists")]
        [SerializeField]
        protected SyncListCharacterHotkey hotkeys = new SyncListCharacterHotkey();
        [SerializeField]
        protected SyncListCharacterQuest quests = new SyncListCharacterQuest();
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
        [SerializeField]
        protected SyncListCharacterCurrency currencies = new SyncListCharacterCurrency();
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
        [SerializeField]
        private NotifiableList<CharacterDataBoolean> serverBools = new NotifiableList<CharacterDataBoolean>();
        [SerializeField]
        private NotifiableList<CharacterDataInt32> serverInts = new NotifiableList<CharacterDataInt32>();
        [SerializeField]
        private NotifiableList<CharacterDataFloat32> serverFloats = new NotifiableList<CharacterDataFloat32>();
        [SerializeField]
        protected SyncListCharacterDataBoolean privateBools = new SyncListCharacterDataBoolean();
        [SerializeField]
        protected SyncListCharacterDataInt32 privateInts = new SyncListCharacterDataInt32();
        [SerializeField]
        protected SyncListCharacterDataFloat32 privateFloats = new SyncListCharacterDataFloat32();
        [SerializeField]
        protected SyncListCharacterDataBoolean publicBools = new SyncListCharacterDataBoolean();
        [SerializeField]
        protected SyncListCharacterDataInt32 publicInts = new SyncListCharacterDataInt32();
        [SerializeField]
        protected SyncListCharacterDataFloat32 publicFloats = new SyncListCharacterDataFloat32();
#endif
        [SerializeField]
        protected SyncListCharacterSkill guildSkills = new SyncListCharacterSkill();
        #endregion

        #region Fields/Interface/Getter/Setter implementation
        public override int DataId { get { return dataId.Value; } set { dataId.Value = value; } }
        public override int FactionId { get { return factionId.Value; } set { factionId.Value = value; } }
        public float StatPoint { get { return statPoint.Value; } set { statPoint.Value = value; } }
        public float SkillPoint { get { return skillPoint.Value; } set { skillPoint.Value = value; } }
        public int Gold
        {
            get
            {
                if (CurrentGameInstance.goldStoreMode == GoldStoreMode.UserGoldOnly)
                    return UserGold;
                return gold.Value;
            }
            set
            {
                if (IsServer && CurrentGameInstance.goldStoreMode == GoldStoreMode.UserGoldOnly)
                {
                    GameInstance.ServerUserHandlers.ChangeUserGold(UserId, value - UserGold);
                    return;
                }
                gold.Value = value;
            }
        }
        public int UserGold { get { return userGold.Value; } set { userGold.Value = value; } }
        public int UserCash { get { return userCash.Value; } set { userCash.Value = value; } }
        public int PartyId { get { return partyId.Value; } set { partyId.Value = value; } }
        public int GuildId { get { return guildId.Value; } set { guildId.Value = value; } }
        public byte GuildRole { get; set; }
        public string UserId { get; set; }
        public byte UserLevel { get; set; }
        public string CurrentChannel
        {
            get
            {
                if (CurrentGameManager.IsInstanceMap())
                    return persistChannel.Value;
                return CurrentGameManager.ChannelId;
            }
            set
            {
                persistChannel.Value = value;
            }
        }
        public string CurrentMapName
        {
            get
            {
                if (CurrentGameManager.IsInstanceMap())
                    return persistMapName.Value;
#if !DISABLE_DIFFER_MAP_RESPAWNING
                if (CurrentGameInstance.currentPositionSaveMode == CurrentPositionSaveMode.UseRespawnPosition || !CurrentMapInfo.SaveCurrentMapPosition)
                    return RespawnMapName;
#endif
                return CurrentMapInfo.Id;
            }
            set
            {
                persistMapName.Value = value;
            }
        }
        public Vec3 CurrentPosition
        {
            get
            {
                if (CurrentGameManager.IsInstanceMap())
                    return persistPosition.Value;
#if !DISABLE_DIFFER_MAP_RESPAWNING
                if (CurrentGameInstance.currentPositionSaveMode == CurrentPositionSaveMode.UseRespawnPosition || !CurrentMapInfo.SaveCurrentMapPosition)
                    return RespawnPosition;
#endif
                Vector3 currentPosition = EntityTransform.position;
                if (!PassengingVehicleEntity.IsNull())
                    currentPosition.y = PassengingVehicleEntity.Entity.EntityTransform.position.y;
                return currentPosition;
            }
            set
            {
                persistPosition.Value = value;
            }
        }
        public Vec3 CurrentRotation
        {
            get
            {
                if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                    return EntityTransform.eulerAngles;
                return Quaternion.LookRotation(Direction2D).eulerAngles;
            }
            set
            {
                if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                {
                    EntityTransform.eulerAngles = value;
                    return;
                }
                Direction2D = Quaternion.Euler(value) * Vector3.forward;
            }
        }
        public string CurrentSafeArea
        {
            get
            {
                if (CurrentGameManager.IsInstanceMap())
                    return persistSafeArea.Value;
                return SafeArea != null ? SafeArea.name : string.Empty;
            }
            set
            {
                persistSafeArea.Value = value;
            }
        }
#if !DISABLE_DIFFER_MAP_RESPAWNING
        public string RespawnMapName
        {
            get { return respawnMapName.Value; }
            set { respawnMapName.Value = value; }
        }
        public Vec3 RespawnPosition
        {
            get { return respawnPosition.Value; }
            set { respawnPosition.Value = value; }
        }
#endif
        public override int IconDataId
        {
            get { return iconDataId.Value; }
            set { iconDataId.Value = value; }
        }
        public override int FrameDataId
        {
            get { return frameDataId.Value; }
            set { frameDataId.Value = value; }
        }
        public override int BackgroundDataId
        {
            get { return backgroundDataId.Value; }
            set { backgroundDataId.Value = value; }
        }
        public override int TitleDataId
        {
            get { return titleDataId.Value; }
            set { titleDataId.Value = value; }
        }
        public long LastDeadTime
        {
            get { return lastDeadTime.Value; }
            set { lastDeadTime.Value = value; }
        }
        public long UnmuteTime
        {
            get { return unmuteTime.Value; }
            set { unmuteTime.Value = value; }
        }
        public long LastUpdate { get; set; }
#if !DISABLE_CLASSIC_PK
        public bool IsPkOn
        {
            get { return isPkOn.Value; }
            set { isPkOn.Value = value; }
        }
        public long LastPkOnTime { get; set; }
        public int PkPoint
        {
            get { return pkPoint.Value; }
            set { pkPoint.Value = value; }
        }
        public int ConsecutivePkKills
        {
            get { return consecutivePkKills.Value; }
            set { consecutivePkKills.Value = value; }
        }
        public int HighestPkPoint { get; set; }
        public int HighestConsecutivePkKills { get; set; }
#endif

        public override int Reputation
        {
            get { return reputation.Value; }
            set { reputation.Value = value; }
        }

        public bool IsWarping
        {
            get { return isWarping.Value; }
            set { isWarping.Value = value; }
        }

        public IList<CharacterHotkey> Hotkeys
        {
            get { return hotkeys; }
            set
            {
                hotkeys.Clear();
                hotkeys.AddRange(value);
            }
        }

        public IList<CharacterQuest> Quests
        {
            get { return quests; }
            set
            {
                quests.Clear();
                quests.AddRange(value);
            }
        }

#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
        public IList<CharacterCurrency> Currencies
        {
            get { return currencies; }
            set
            {
                currencies.Clear();
                currencies.AddRange(value);
            }
        }
#endif

#if !DISABLE_CUSTOM_CHARACTER_DATA
        public IList<CharacterDataBoolean> ServerBools
        {
            get { return serverBools; }
            set
            {
                serverBools.Clear();
                serverBools.AddRange(value);
            }
        }

        public IList<CharacterDataInt32> ServerInts
        {
            get { return serverInts; }
            set
            {
                serverInts.Clear();
                serverInts.AddRange(value);
            }
        }

        public IList<CharacterDataFloat32> ServerFloats
        {
            get { return serverFloats; }
            set
            {
                serverFloats.Clear();
                serverFloats.AddRange(value);
            }
        }

        public IList<CharacterDataBoolean> PrivateBools
        {
            get { return privateBools; }
            set
            {
                privateBools.Clear();
                privateBools.AddRange(value);
            }
        }

        public IList<CharacterDataInt32> PrivateInts
        {
            get { return privateInts; }
            set
            {
                privateInts.Clear();
                privateInts.AddRange(value);
            }
        }

        public IList<CharacterDataFloat32> PrivateFloats
        {
            get { return privateFloats; }
            set
            {
                privateFloats.Clear();
                privateFloats.AddRange(value);
            }
        }

        public IList<CharacterDataBoolean> PublicBools
        {
            get { return publicBools; }
            set
            {
                publicBools.Clear();
                publicBools.AddRange(value);
            }
        }

        public IList<CharacterDataInt32> PublicInts
        {
            get { return publicInts; }
            set
            {
                publicInts.Clear();
                publicInts.AddRange(value);
            }
        }

        public IList<CharacterDataFloat32> PublicFloats
        {
            get { return publicFloats; }
            set
            {
                publicFloats.Clear();
                publicFloats.AddRange(value);
            }
        }
#endif

        public IList<CharacterSkill> GuildSkills
        {
            get { return guildSkills; }
            set
            {
                guildSkills.Clear();
                guildSkills.AddRange(value);
            }
        }
        #endregion

        #region Network setup functions
        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            // Sync fields
            syncMetaDataId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            dataId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            factionId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            statPoint.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
            skillPoint.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
            gold.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
            userGold.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
            userCash.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
            partyId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            guildId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
#if !DISABLE_DIFFER_MAP_RESPAWNING
            respawnMapName.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
            respawnPosition.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
#endif
            iconDataId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            frameDataId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            backgroundDataId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            titleDataId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            lastDeadTime.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
#if !DISABLE_CLASSIC_PK
            isPkOn.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            pkPoint.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            consecutivePkKills.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
#endif
            reputation.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            isWarping.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
            pitch.syncMode = LiteNetLibSyncFieldMode.ClientMulticast;
            lookPosition.syncMode = LiteNetLibSyncFieldMode.ClientMulticast;
            targetEntityId.syncMode = LiteNetLibSyncFieldMode.ClientMulticast;
            // Sync lists
            hotkeys.forOwnerOnly = true;
            quests.forOwnerOnly = true;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            currencies.forOwnerOnly = true;
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            privateBools.forOwnerOnly = true;
            privateInts.forOwnerOnly = true;
            privateFloats.forOwnerOnly = true;
            publicBools.forOwnerOnly = false;
            publicInts.forOwnerOnly = false;
            publicFloats.forOwnerOnly = false;
#endif
            guildSkills.forOwnerOnly = false;
        }

        public override void OnSetup()
        {
            base.OnSetup();
            // On data changes events
            id.onChange += OnPlayerIdChange;
            syncTitle.onChange += OnPlayerCharacterNameChange;
            dataId.onChange += OnDataIdChange;
            factionId.onChange += OnFactionIdChange;
            statPoint.onChange += OnStatPointChange;
            skillPoint.onChange += OnSkillPointChange;
            gold.onChange += OnGoldChange;
            userGold.onChange += OnUserGoldChange;
            userCash.onChange += OnUserCashChange;
            partyId.onChange += OnPartyIdChange;
            guildId.onChange += OnGuildIdChange;
            iconDataId.onChange += OnIconDataIdChange;
            frameDataId.onChange += OnFrameDataIdChange;
            backgroundDataId.onChange += OnBackgroundDataIdChange;
            titleDataId.onChange += OnTitleDataIdChange;
#if !DISABLE_CLASSIC_PK
            isPkOn.onChange += OnIsPkOnChange;
            pkPoint.onChange += OnPkPointChange;
            consecutivePkKills.onChange += OnConsecutivePkKillsChange;
#endif
            reputation.onChange += OnReputationChange;
            isWarping.onChange += OnIsWarpingChange;
            // On list changes events
            hotkeys.onOperation += OnHotkeysOperation;
            quests.onOperation += OnQuestsOperation;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            currencies.onOperation += OnCurrenciesOperation;
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            serverBools.ListChanged += OnServerBoolsOperation;
            serverInts.ListChanged += OnServerIntsOperation;
            serverFloats.ListChanged += OnServerFloatsOperation;
            privateBools.onOperation += OnPrivateBoolsOperation;
            privateInts.onOperation += OnPrivateIntsOperation;
            privateFloats.onOperation += OnPrivateFloatsOperation;
            publicBools.onOperation += OnPublicBoolsOperation;
            publicInts.onOperation += OnPublicIntsOperation;
            publicFloats.onOperation += OnPublicFloatsOperation;
#endif
            guildSkills.onOperation += OnGuildSkillsOperation;
        }

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            // On data changes events
            id.onChange -= OnPlayerIdChange;
            syncTitle.onChange -= OnPlayerCharacterNameChange;
            dataId.onChange -= OnDataIdChange;
            factionId.onChange -= OnFactionIdChange;
            statPoint.onChange -= OnStatPointChange;
            skillPoint.onChange -= OnSkillPointChange;
            gold.onChange -= OnGoldChange;
            userGold.onChange -= OnUserGoldChange;
            userCash.onChange -= OnUserCashChange;
            partyId.onChange -= OnPartyIdChange;
            guildId.onChange -= OnGuildIdChange;
            iconDataId.onChange -= OnIconDataIdChange;
            frameDataId.onChange -= OnFrameDataIdChange;
            backgroundDataId.onChange += OnBackgroundDataIdChange;
            titleDataId.onChange -= OnTitleDataIdChange;
#if !DISABLE_CLASSIC_PK
            isPkOn.onChange -= OnIsPkOnChange;
            pkPoint.onChange -= OnPkPointChange;
            consecutivePkKills.onChange -= OnConsecutivePkKillsChange;
#endif
            reputation.onChange -= OnReputationChange;
            isWarping.onChange -= OnIsWarpingChange;
            // On list changes events
            hotkeys.onOperation -= OnHotkeysOperation;
            quests.onOperation -= OnQuestsOperation;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            currencies.onOperation -= OnCurrenciesOperation;
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            serverBools.ListChanged -= OnServerBoolsOperation;
            serverInts.ListChanged -= OnServerIntsOperation;
            serverFloats.ListChanged -= OnServerFloatsOperation;
            privateBools.onOperation -= OnPrivateBoolsOperation;
            privateInts.onOperation -= OnPrivateIntsOperation;
            privateFloats.onOperation -= OnPrivateFloatsOperation;
            publicBools.onOperation -= OnPublicBoolsOperation;
            publicInts.onOperation -= OnPublicIntsOperation;
            publicFloats.onOperation -= OnPublicFloatsOperation;
#endif
            guildSkills.onOperation -= OnGuildSkillsOperation;

            if (IsOwnerClient && BasePlayerCharacterController.Singleton != null)
                Destroy(BasePlayerCharacterController.Singleton.gameObject);

            // Unsubscribe this entity
            if (GameInstance.ClientCharacterHandlers != null)
                GameInstance.ClientCharacterHandlers.UnsubscribePlayerCharacter(this);
        }

        protected override void EntityOnSetOwnerClient()
        {
            base.EntityOnSetOwnerClient();
            InstantiatePlayerCharacterObjects();
        }

        private async void InstantiatePlayerCharacterObjects()
        {
            // Setup relates elements
            if (IsOwnerClient)
            {
                BasePlayerCharacterController controllerPrefab = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                if (ControllerPrefab != null)
                {
                    controllerPrefab = ControllerPrefab;
                }
                else if (CurrentGameInstance.DefaultControllerPrefab != null)
                {
                    controllerPrefab = CurrentGameInstance.DefaultControllerPrefab;
                }
#endif
                if (controllerPrefab != null)
                {
                    // Do nothing, just have it to make it able to compile properly (it have compile condition above)
                }
#if !DISABLE_ADDRESSABLES
                else if (AddressableControllerPrefab.IsDataValid())
                {
                    controllerPrefab = await AddressableControllerPrefab.GetOrLoadAssetAsync<BasePlayerCharacterController>();
                }
                else if (CurrentGameInstance.AddressableDefaultControllerPrefab.IsDataValid())
                {
                    controllerPrefab = await CurrentGameInstance.AddressableDefaultControllerPrefab.GetOrLoadAssetAsync<BasePlayerCharacterController>();
                }
#endif
                else if (BasePlayerCharacterController.Singleton != null)
                {
                    controllerPrefab = BasePlayerCharacterController.LastPrefab;
                }
                else
                {
                    Logging.LogWarning(ToString(), "`Controller Prefab` is empty so it cannot be instantiated");
                    controllerPrefab = null;
                }
                if (controllerPrefab != null)
                {
                    BasePlayerCharacterController.LastPrefab = controllerPrefab;
                    BasePlayerCharacterController controller = Instantiate(controllerPrefab);
                    controller.PlayingCharacterEntity = this;
                }
#if !DISABLE_ADDRESSABLES
                // Instantiates owning objects
                await CurrentGameInstance.AddressableOwningCharacterObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.OwningCharacterObjects, EntityTransform);
#else
                foreach (var prefab in CurrentGameInstance.OwningCharacterObjects)
                {
                    if (prefab == null) continue;
                    Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform);
                }
#endif
#if !DISABLE_ADDRESSABLES
                // Instantiates owning minimap objects
                await CurrentGameInstance.AddressableOwningCharacterMiniMapObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.OwningCharacterMiniMapObjects, EntityTransform);
#else
                foreach (var prefab in CurrentGameInstance.OwningCharacterMiniMapObjects)
                {
                    if (prefab == null) continue;
                    Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform);
                }
#endif
                // Instantiates owning character UI
                InstantiateUI(await CurrentGameInstance.GetLoadedOwningCharacterUIPrefab());
            }
            else if (IsClient)
            {
#if !DISABLE_ADDRESSABLES
                // Instantiates non-owning objects
                await CurrentGameInstance.AddressableNonOwningCharacterObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.NonOwningCharacterObjects, EntityTransform);
#else
                foreach (var prefab in CurrentGameInstance.NonOwningCharacterObjects)
                {
                    if (prefab == null) continue;
                    Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform);
                }
#endif
#if !DISABLE_ADDRESSABLES
                // Instantiates non-owning minimap objects
                await CurrentGameInstance.AddressableNonOwningCharacterMiniMapObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.NonOwningCharacterMiniMapObjects, EntityTransform);
#else
                foreach (var prefab in CurrentGameInstance.NonOwningCharacterMiniMapObjects)
                {
                    if (prefab == null) continue;
                    Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform);
                }
#endif
                // Instantiates non-owning character UI
                InstantiateUI(await CurrentGameInstance.GetLoadedNonOwningCharacterUIPrefab());
            }
        }
#endregion

            #region Sync data changes callback
        private void OnPlayerIdChange(bool isInitial, string oldId, string id)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(CharacterName) && GameInstance.ClientCharacterHandlers != null)
                GameInstance.ClientCharacterHandlers.SubscribePlayerCharacter(this);
        }

        private void OnPlayerCharacterNameChange(bool isInitial, string oldCharacterName, string characterName)
        {
            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(characterName) && GameInstance.ClientCharacterHandlers != null)
                GameInstance.ClientCharacterHandlers.SubscribePlayerCharacter(this);
        }

        private void OnDataIdChange(bool isInitial, int oldDataId, int dataId)
        {
            IsRecaching = true;
            if (onDataIdChange != null)
                onDataIdChange.Invoke(dataId);
        }

        private void OnFactionIdChange(bool isInitial, int oldFactionId, int factionId)
        {
            IsRecaching = true;
            if (onFactionIdChange != null)
                onFactionIdChange.Invoke(factionId);
        }

        private void OnStatPointChange(bool isInitial, float oldStatPoint, float statPoint)
        {
            if (onStatPointChange != null)
                onStatPointChange.Invoke(statPoint);
        }

        private void OnSkillPointChange(bool isInitial, float oldSkillPoint, float skillPoint)
        {
            if (onSkillPointChange != null)
                onSkillPointChange.Invoke(skillPoint);
        }

        private void OnGoldChange(bool isInitial, int oldGold, int gold)
        {
            if (onGoldChange != null)
                onGoldChange.Invoke(gold);
        }

        private void OnUserGoldChange(bool isInitial, int oldUserGold, int userGold)
        {
            if (onUserGoldChange != null)
                onUserGoldChange.Invoke(userGold);
        }

        private void OnUserCashChange(bool isInitial, int oldUserCash, int userCash)
        {
            if (onUserCashChange != null)
                onUserCashChange.Invoke(userCash);
        }

        private void OnPartyIdChange(bool isInitial, int oldPartyId, int partyId)
        {
            IsRecaching = true;
            if (onPartyIdChange != null)
                onPartyIdChange.Invoke(partyId);
        }

        private void OnGuildIdChange(bool isInitial, int oldGuildId, int guildId)
        {
            IsRecaching = true;
            if (onGuildIdChange != null)
                onGuildIdChange.Invoke(guildId);
        }

        private void OnIconDataIdChange(bool isInitial, int oldIconDataId, int iconDataId)
        {
            IsRecaching = true;
            if (onIconDataIdChange != null)
                onIconDataIdChange.Invoke(iconDataId);
        }

        private void OnFrameDataIdChange(bool isInitial, int oldFrameDataId, int frameDataId)
        {
            IsRecaching = true;
            if (onFrameDataIdChange != null)
                onFrameDataIdChange.Invoke(frameDataId);
        }

        private void OnBackgroundDataIdChange(bool isInitial, int oldBackgroundDataId, int backgroundDataId)
        {
            IsRecaching = true;
            if (onBackgroundDataIdChange != null)
                onBackgroundDataIdChange.Invoke(backgroundDataId);
        }

        private void OnTitleDataIdChange(bool isInitial, int oldTitleDataId, int titleDataId)
        {
            IsRecaching = true;
            if (onTitleDataIdChange != null)
                onTitleDataIdChange.Invoke(titleDataId);
        }

#if !DISABLE_CLASSIC_PK
        private void OnIsPkOnChange(bool isInitial, bool oldIsOn, bool isPkOn)
        {
            if (onIsPkOnChange != null)
                onIsPkOnChange.Invoke(isPkOn);
        }

        private void OnPkPointChange(bool isInitial, int oldPkPoint, int pkPoint)
        {
            if (onPkPointChange != null)
                onPkPointChange.Invoke(pkPoint);
        }

        private void OnConsecutivePkKillsChange(bool isInitial, int oldConsecutivePkKills, int consecutivePkKills)
        {
            if (onConsecutivePkKillsChange != null)
                onConsecutivePkKillsChange.Invoke(consecutivePkKills);
        }
#endif

        private void OnReputationChange(bool isInitial, int oldReputation, int reputation)
        {
            if (onReputationChange != null)
                onReputationChange.Invoke(reputation);
        }

        private void OnIsWarpingChange(bool isInitial, bool isOldWarping, bool isWarping)
        {
            if (onIsWarpingChange != null)
                onIsWarpingChange.Invoke(isWarping);
        }
        #endregion

        #region Net functions operation callback
        private void OnHotkeysOperation(LiteNetLibSyncListOp operation, int index, CharacterHotkey oldItem, CharacterHotkey newItem)
        {
            if (onHotkeysOperation != null)
                onHotkeysOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnQuestsOperation(LiteNetLibSyncListOp operation, int index, CharacterQuest oldItem, CharacterQuest newItem)
        {
            if (onQuestsOperation != null)
                onQuestsOperation.Invoke(operation, index, oldItem, newItem);
        }

#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
        private void OnCurrenciesOperation(LiteNetLibSyncListOp operation, int index, CharacterCurrency oldItem, CharacterCurrency newItem)
        {
            if (onCurrenciesOperation != null)
                onCurrenciesOperation.Invoke(operation, index, oldItem, newItem);
        }
#endif

#if !DISABLE_CUSTOM_CHARACTER_DATA
        private void OnServerBoolsOperation(NotifiableListAction action, int index, CharacterDataBoolean oldItem, CharacterDataBoolean newItem)
        {
            if (onServerBoolsOperation != null)
                onServerBoolsOperation.Invoke(action, index, oldItem, newItem);
        }

        private void OnServerIntsOperation(NotifiableListAction action, int index, CharacterDataInt32 oldItem, CharacterDataInt32 newItem)
        {
            if (onServerIntsOperation != null)
                onServerIntsOperation.Invoke(action, index, oldItem, newItem);
        }

        private void OnServerFloatsOperation(NotifiableListAction action, int index, CharacterDataFloat32 oldItem, CharacterDataFloat32 newItem)
        {
            if (onServerFloatsOperation != null)
                onServerFloatsOperation.Invoke(action, index, oldItem, newItem);
        }

        private void OnPrivateBoolsOperation(LiteNetLibSyncListOp operation, int index, CharacterDataBoolean oldItem, CharacterDataBoolean newItem)
        {
            if (onPrivateBoolsOperation != null)
                onPrivateBoolsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnPrivateIntsOperation(LiteNetLibSyncListOp operation, int index, CharacterDataInt32 oldItem, CharacterDataInt32 newItem)
        {
            if (onPrivateIntsOperation != null)
                onPrivateIntsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnPrivateFloatsOperation(LiteNetLibSyncListOp operation, int index, CharacterDataFloat32 oldItem, CharacterDataFloat32 newItem)
        {
            if (onPrivateFloatsOperation != null)
                onPrivateFloatsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnPublicBoolsOperation(LiteNetLibSyncListOp operation, int index, CharacterDataBoolean oldItem, CharacterDataBoolean newItem)
        {
            if (onPublicBoolsOperation != null)
                onPublicBoolsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnPublicIntsOperation(LiteNetLibSyncListOp operation, int index, CharacterDataInt32 oldItem, CharacterDataInt32 newItem)
        {
            if (onPublicIntsOperation != null)
                onPublicIntsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnPublicFloatsOperation(LiteNetLibSyncListOp operation, int index, CharacterDataFloat32 oldItem, CharacterDataFloat32 newItem)
        {
            if (onPublicFloatsOperation != null)
                onPublicFloatsOperation.Invoke(operation, index, oldItem, newItem);
        }
#endif

        private void OnGuildSkillsOperation(LiteNetLibSyncListOp operation, int index, CharacterSkill oldItem, CharacterSkill newItem)
        {
            switch (operation)
            {
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    if (oldItem.dataId != newItem.dataId ||
                        oldItem.level != newItem.level)
                        IsRecaching = true;
                    break;
                default:
                    IsRecaching = true;
                    break;
            }
            if (onGuildSkillsOperation != null)
                onGuildSkillsOperation.Invoke(operation, index, oldItem, newItem);
        }
        #endregion
    }
}
