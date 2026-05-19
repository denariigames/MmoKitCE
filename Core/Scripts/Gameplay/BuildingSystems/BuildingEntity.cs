using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using LiteNetLib;
using LiteNetLibManager;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public partial class BuildingEntity : DamageableEntity, IBuildingSaveData, IActivatableEntity, IHoldActivatableEntity
    {
        public const float BUILD_DISTANCE_BUFFER = 0.1f;
        protected static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("BuildingEntity - Update");

        [Category(5, "Building Settings")]
        [SerializeField]
        [Tooltip("Set it more than `0` to make it uses this value instead of `GameInstance` -> `conversationDistance` as its activatable distance")]
        protected float activatableDistance = 0f;

        [SerializeField]
        [Tooltip("If this is `TRUE` this entity will be able to be attacked")]
        protected bool canBeAttacked = true;

        [SerializeField]
        [Tooltip("If this is `TRUE` this building entity will be able to build on any surface. But when constructing, if player aimming on building area it will place on building area")]
        protected bool canBuildOnAnySurface = false;

        [SerializeField]
        [Tooltip("If this is `TRUE` this building entity will be able to build on limited surface hit normal angle (default up angle is 90)")]
        protected bool limitSurfaceHitNormalAngle = false;

        [SerializeField]
        protected float limitSurfaceHitNormalAngleMin = 80f;

        [SerializeField]
        protected float limitSurfaceHitNormalAngleMax = 100f;

        [HideInInspector]
        [SerializeField]
        [Tooltip("Type of building you can set it as Foundation, Wall, Door anything as you wish. This is a part of `buildingTypes`, just keep it for backward compatibility.")]
        protected string buildingType = string.Empty;

        [SerializeField]
        [Tooltip("Type of building you can set it as Foundation, Wall, Door anything as you wish.")]
        protected List<string> buildingTypes = new List<string>();

        [SerializeField]
        [Tooltip("This is a distance that allows a player to build the building")]
        protected float buildDistance = 5f;

        [SerializeField]
        [Tooltip("If this is `TRUE`, this entity will be destroyed when its parent building entity was destroyed")]
        protected bool destroyWhenParentDestroyed = false;

        [SerializeField]
        [Tooltip("If this is `TRUE`, character will move on it when click on it, not select or set it as target")]
        protected bool notBeingSelectedOnClick = true;

        [SerializeField]
        [Tooltip("Building's max HP. If its HP <= 0, it will be destroyed")]
        protected int maxHp = 100;

        [SerializeField]
        [Tooltip("If life time is <= 0, it's unlimit lifetime")]
        protected float lifeTime = 0f;

        [SerializeField]
        [Tooltip("Maximum number of buildings per player in a map, if it is <= 0, it's unlimit")]
        protected int buildLimit = 0;

        [SerializeField]
        [Tooltip("Items which will be dropped when building destroyed")]
        protected List<ItemAmount> droppingItems = new List<ItemAmount>();

        [SerializeField]
        [Tooltip("List of repair data")]
        protected List<BuildingRepairData> repairs = new List<BuildingRepairData>();

        [SerializeField]
        [Tooltip("Delay before the entity destroyed, you may set some delay to play destroyed animation by `onBuildingDestroy` event before it's going to be destroyed from the game.")]
        protected float destroyDelay = 2f;

        [SerializeField]
        [Tooltip("Interval in seconds to batch/coalesce segmented part HP updates.")]
        protected float segmentedHpDeltaSendInterval = 0.2f;

        [SerializeField]
        protected InputField.ContentType passwordContentType = InputField.ContentType.Pin;
        public InputField.ContentType PasswordContentType { get { return passwordContentType; } }

        [SerializeField]
        protected int passwordLength = 6;
        public int PasswordLength { get { return passwordLength; } }

        [Category("Events")]
        [SerializeField]
        protected UnityEvent onBuildingDestroy = new UnityEvent();
        [SerializeField]
        protected UnityEvent onBuildingConstruct = new UnityEvent();

        public bool CanBuildOnAnySurface { get { return canBuildOnAnySurface; } }
        public bool LimitSurfaceHitNormalAngle { get { return limitSurfaceHitNormalAngle; } }
        public float LimitSurfaceHitNormalAngleMin { get { return limitSurfaceHitNormalAngleMin; } }
        public float LimitSurfaceHitNormalAngleMax { get { return limitSurfaceHitNormalAngleMax; } }
        public List<string> BuildingTypes { get { return buildingTypes; } }
        public float BuildDistance { get { return buildDistance; } }
        public float BuildYRotation { get; set; }
        public override bool IsInvincible { get { return base.IsInvincible || !canBeAttacked; } set { base.IsInvincible = value; } }
        public override int MaxHp { get { return maxHp; } }
        public float LifeTime { get { return lifeTime; } }
        public int BuildLimit { get { return buildLimit; } }
        public bool HasFinalizedChildren { get { return _finalizedChildVisualData.Count > 0; } }

        /// <summary>
        /// Use this as reference for area to build this object while in build mode
        /// </summary>
        public BuildingArea BuildingArea { get; set; }

        /// <summary>
        /// Use this as reference for hit surface state while in build mode
        /// </summary>
        public bool HitSurface { get; set; }

        /// <summary>
        /// Use this as reference for hit surface normal while in build mode
        /// </summary>
        public Vector3 HitSurfaceNormal { get; set; }

        [Category("Sync Fields")]
        [SerializeField]
        private SyncFieldString id = new SyncFieldString();
        [SerializeField]
        private SyncFieldString parentId = new SyncFieldString();
        [SerializeField]
        private SyncFieldFloat remainsLifeTime = new SyncFieldFloat();
        [SerializeField]
        private SyncFieldBool isLocked = new SyncFieldBool();
        [SerializeField]
        private SyncFieldString creatorId = new SyncFieldString();
        [SerializeField]
        private SyncFieldString creatorName = new SyncFieldString();
        [SerializeField]
        private SyncFieldString finalizedChildrenPayload = new SyncFieldString();

        public string Id
        {
            get
            {
                if (IsSceneObject)
                    return ZString.Concat(CurrentGameManager.ChannelId, '_', CurrentGameManager.MapInfo.Id, '_', Identity.SceneObjectId);
                else
                    return id;
            }
            set
            {
                if (CanSetSpawnMetadata(id, value))
                    id.Value = value;
            }
        }

        public string ParentId
        {
            get { return parentId; }
            set
            {
                if (CanSetSpawnMetadata(parentId, value))
                    parentId.Value = value;
            }
        }

        public float RemainsLifeTime
        {
            get { return remainsLifeTime; }
            set { remainsLifeTime.Value = value; }
        }

        public bool IsLocked
        {
            get { return isLocked; }
            set { isLocked.Value = value; }
        }

        public string LockPassword
        {
            get;
            set;
        }

        public Vec3 Position
        {
            get { return EntityTransform.position; }
            set { EntityTransform.position = value; }
        }

        public Vec3 Rotation
        {
            get { return EntityTransform.eulerAngles; }
            set { EntityTransform.eulerAngles = value; }
        }

        public string CreatorId
        {
            get { return creatorId; }
            set { creatorId.Value = value; }
        }

        public string CreatorName
        {
            get { return creatorName; }
            set { creatorName.Value = value; }
        }

        public virtual string ExtraData
        {
            get
            {
                if (_finalizedChildVisualData.Count == 0)
                    return string.Empty;
                FinalizedBuildingExtraData extraData = new FinalizedBuildingExtraData();
                extraData.finalizedChildren = new List<FinalizedChildVisualData>(_finalizedChildVisualData);
                return JsonUtility.ToJson(extraData);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _pendingLoadedFinalizedChildVisualData.Clear();
                    _finalizedChildVisualData.Clear();
                    _finalizedPartCurrentHp.Clear();
                    _finalizedPartMaxHp.Clear();
                    _pendingFinalizedPartHpDeltas.Clear();
                    ClearFinalizedChildVisuals();
                    return;
                }

                FinalizedBuildingExtraData extraData;
                try
                {
                    extraData = JsonUtility.FromJson<FinalizedBuildingExtraData>(value);
                }
                catch
                {
                    return;
                }

                if (extraData == null || extraData.finalizedChildren == null)
                    return;

                _pendingLoadedFinalizedChildVisualData.Clear();
                _pendingLoadedFinalizedChildVisualData.AddRange(extraData.finalizedChildren);

                // During map bootstrap this can be called before network spawn is finished.
                // Defer visual rebuild until OnSetup to avoid early identity initialization.
                if (!IsSpawned)
                    return;

                RebuildFinalizedChildVisuals(extraData.finalizedChildren);
            }
        }

        bool IBuildingSaveData.IsSceneObject
        {
            get { return Identity.IsSceneObject; }
            set { }
        }

        public virtual bool Lockable { get { return false; } }
        public bool IsBuildMode { get; private set; }
        public BasePlayerCharacterEntity Builder { get; private set; }

        private BuildingRepairData? _repairDataForMenu;
        private Dictionary<BaseItem, BuildingRepairData> _cacheRepairs;
        public Dictionary<BaseItem, BuildingRepairData> CacheRepairs
        {
            get
            {
                if (_cacheRepairs == null)
                {
                    _cacheRepairs = new Dictionary<BaseItem, BuildingRepairData>();
                    if (repairs != null && repairs.Count > 0)
                    {
                        for (int i = 0; i < repairs.Count; ++i)
                        {
                            if (repairs[i].canRepairFromMenu && !_repairDataForMenu.HasValue)
                            {
                                _repairDataForMenu = repairs[i];
                                continue;
                            }
                            BaseItem weaponItem = repairs[i].weaponItem;
                            if (weaponItem == null)
                                weaponItem = CurrentGameInstance.DefaultWeaponItem as BaseItem;
                            if (!weaponItem.IsWeapon())
                                continue;
                            if (_cacheRepairs.ContainsKey(weaponItem))
                                continue;
                            _cacheRepairs[weaponItem] = repairs[i];
                        }
                    }
                }
                return _cacheRepairs;
            }
        }

        protected readonly HashSet<GameObject> _triggerObjects = new HashSet<GameObject>();
        protected readonly HashSet<BuildingEntity> _children = new HashSet<BuildingEntity>();
        protected readonly HashSet<BuildingMaterial> _buildingMaterials = new HashSet<BuildingMaterial>();
        protected readonly List<GameObject> _finalizedChildVisualObjects = new List<GameObject>();
        private readonly List<FinalizedChildVisualData> _finalizedChildVisualData = new List<FinalizedChildVisualData>();
        private readonly Dictionary<string, int> _finalizedPartCurrentHp = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _finalizedPartMaxHp = new Dictionary<string, int>();
        private readonly Dictionary<string, List<GameObject>> _finalizedPartVisualObjects = new Dictionary<string, List<GameObject>>();
        private readonly Dictionary<string, int> _pendingFinalizedPartHpDeltas = new Dictionary<string, int>();
        private readonly List<FinalizedChildVisualData> _pendingLoadedFinalizedChildVisualData = new List<FinalizedChildVisualData>();
        protected float _serverRemainsLifeTime;
        protected float _lastSegmentedHpDeltaSendTime;
        protected int _lastSyncedRemainsLifeTimeSeconds = -1;
        protected int _lastAddedTriggerObjectFrame;
        protected bool _didPostNetSetupInitialize;
        protected bool _parentFound;
        protected bool _isDestroyed;

        protected override void EntityAwake()
        {
            base.EntityAwake();
            gameObject.tag = CurrentGameInstance.buildingTag;
            gameObject.layer = CurrentGameInstance.buildingLayer;
            isStaticHitBoxes = true;
            _isDestroyed = false;
            MigrateBuildingType();
        }

        public override void InitialRequiredComponents()
        {
            CurrentGameInstance.EntitySetting.InitialBuildingEntityComponents(this);
            base.InitialRequiredComponents();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (MigrateBuildingType())
                EditorUtility.SetDirty(this);
        }
#endif

        protected bool MigrateBuildingType()
        {
            if (!string.IsNullOrEmpty(buildingType) && !buildingTypes.Contains(buildingType))
            {
                buildingTypes.Add(buildingType);
                buildingType = string.Empty;
                return true;
            }
            return false;
        }

        public void UpdateBuildingAreaSnapping()
        {
            if (BuildingArea != null && BuildingArea.snapBuildingObject)
            {
                EntityTransform.position = BuildingArea.transform.position;
                EntityTransform.rotation = BuildingArea.transform.rotation;
                if (BuildingArea.allowRotateInSocket)
                {
                    EntityTransform.localEulerAngles = new Vector3(
                        EntityTransform.localEulerAngles.x,
                        EntityTransform.localEulerAngles.y + BuildYRotation,
                        EntityTransform.localEulerAngles.z);
                }
            }
        }

        protected override void EntityUpdate()
        {
            base.EntityUpdate();
            using (s_UpdateProfilerMarker.Auto())
            {
                if (IsServer && lifeTime > 0f)
                {
                    // Keep server-authoritative timer with frame precision.
                    _serverRemainsLifeTime -= Time.deltaTime;
                    if (_serverRemainsLifeTime < 0f)
                    {
                        // Destroy building
                        _serverRemainsLifeTime = 0f;
                        SyncRemainsLifeTimeToClients(true);
                        Destroy();
                    }
                    else
                    {
                        // Sync only coarse lifetime steps to reduce network/DB churn.
                        SyncRemainsLifeTimeToClients();
                    }
                }
                // Flush pending finalized part HP deltas
                if (IsServer)
                    FlushPendingFinalizedPartHpDeltas();
            }
        }

        protected override void EntityLateUpdate()
        {
            base.EntityLateUpdate();
            if (IsBuildMode)
            {
                UpdateBuildingAreaSnapping();
                bool canBuild = CanBuild();
                foreach (BuildingMaterial buildingMaterial in _buildingMaterials)
                {
                    if (!buildingMaterial) continue;
                    buildingMaterial.CurrentState = canBuild ? BuildingMaterial.State.CanBuild : BuildingMaterial.State.CannotBuild;
                }
                // Clear all triggered, `BuildingMaterialBuildModeHandler` will try to add them later
                if (Time.frameCount > _lastAddedTriggerObjectFrame + 1)
                    _triggerObjects.Clear();
            }
            // Setup parent which when it's destroying it will destroy children (chain destroy)
            if (IsServer && !_parentFound)
            {
                BuildingEntity parent;
                if (GameInstance.ServerBuildingHandlers.TryGetBuilding(ParentId, out parent))
                {
                    _parentFound = true;
                    parent.AddChildren(this);
                }
            }
        }

        public void RegisterMaterial(BuildingMaterial material)
        {
            _buildingMaterials.Add(material);
        }

        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            id.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            //id.sendInterval = 60f;
            parentId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            //parentId.sendInterval = 60f;
            UpdateRemainsLifeTimeSyncSettings();
            isLocked.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            // Keep lock state reasonably responsive for interaction flow.
            //isLocked.sendInterval = 1f;
            creatorId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            //creatorId.sendInterval = 1f;
            creatorName.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            //creatorName.sendInterval = 1f;
            finalizedChildrenPayload.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            //finalizedChildrenPayload.sendInterval = 1f;
            PostNetSetupInitialize();
        }

        /// <summary>
        /// Initialize the building entity after the network setup is completed
        /// </summary>
        private void PostNetSetupInitialize()
        {
            if (_didPostNetSetupInitialize)
                return;
            _didPostNetSetupInitialize = true;
            if (_pendingLoadedFinalizedChildVisualData.Count > 0)
            {
                RebuildFinalizedChildVisuals(_pendingLoadedFinalizedChildVisualData);
                _pendingLoadedFinalizedChildVisualData.Clear();
            }
            // Preserve current save state, fallback to configured lifetime for freshly spawned buildings.
            _serverRemainsLifeTime = RemainsLifeTime;
            if (IsServer)
            {
                if (lifeTime > 0f && _serverRemainsLifeTime <= 0f)
                    _serverRemainsLifeTime = lifeTime;
                SyncRemainsLifeTimeToClients(true);
                SyncFinalizedChildrenPayloadToClients();
            }
            parentId.onChange += OnParentIdChange;
            finalizedChildrenPayload.onChange += OnFinalizedChildrenPayloadChange;
        }

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            parentId.onChange -= OnParentIdChange;
            finalizedChildrenPayload.onChange -= OnFinalizedChildrenPayloadChange;
        }

        public void CallRpcOnBuildingDestroy()
        {
            RPC(RpcOnBuildingDestroy);
        }

        [AllRpc]
        private void RpcOnBuildingDestroy()
        {
            if (onBuildingDestroy != null)
                onBuildingDestroy.Invoke();
        }

        public void CallRpcOnBuildingConstruct()
        {
            RPC(RpcOnBuildingConstruct);
        }

        [AllRpc]
        private void RpcOnBuildingConstruct()
        {
            if (onBuildingConstruct != null)
                onBuildingConstruct.Invoke();
        }

        private void OnParentIdChange(bool isInitial, string oldParentId, string parentId)
        {
            _parentFound = false;
            UpdateRemainsLifeTimeSyncSettings();
        }

        private void SyncRemainsLifeTimeToClients(bool force = false)
        {
            int currentRemainsLifeTimeSeconds = Mathf.CeilToInt(Mathf.Max(0f, _serverRemainsLifeTime));
            if (!force && currentRemainsLifeTimeSeconds == _lastSyncedRemainsLifeTimeSeconds)
                return;
            _lastSyncedRemainsLifeTimeSeconds = currentRemainsLifeTimeSeconds;
            RemainsLifeTime = currentRemainsLifeTimeSeconds;
        }

        private void UpdateRemainsLifeTimeSyncSettings()
        {
            // Child parts don't need to broadcast lifetime to everyone.
            remainsLifeTime.syncMode = string.IsNullOrEmpty(ParentId)
                ? LiteNetLibSyncFieldMode.ServerToClients
                : LiteNetLibSyncFieldMode.ServerToOwnerClient;
            //remainsLifeTime.sendInterval = 60f;
        }

        private bool CanSetSpawnMetadata(SyncFieldString field, string value)
        {
            // Treat persistent identity metadata as spawn-time immutable.
            if (!IsSpawned)
                return true;
            if (string.IsNullOrEmpty(field.Value))
                return true;
            return field.Value.Equals(value);
        }

        public void AddChildren(BuildingEntity buildingEntity)
        {
            _children.Add(buildingEntity);
        }

        /// <summary>
        /// Finalized child visual data
        /// </summary>
        [Serializable]
        private struct FinalizedChildVisualData
        {
            // Logical segmented child identifier after finalization.
            // It is not a live BuildingEntity ID because finalized children become visuals.
            public string partId;
            public int entityId;
            public int currentHp;
            public int maxHp;
            public Vector3 localPosition;
            public Vector3 localEulerAngles;
            public Vector3 localScale;
        }

        /// <summary>
        /// List of finalized child visual data
        /// </summary>
        [Serializable]
        private class FinalizedChildVisualDataList
        {
            public List<FinalizedChildVisualData> values = new List<FinalizedChildVisualData>();
        }

        /// <summary>
        /// Persisted extra data for finalized building children
        /// </summary>
        [Serializable]
        private class FinalizedBuildingExtraData
        {
            public List<FinalizedChildVisualData> finalizedChildren = new List<FinalizedChildVisualData>();
        }

        /// <summary>
        /// Finalized piece HP delta
        /// </summary>
        [Serializable]
        private struct FinalizedPartHpDelta
        {
            public string partId;
            public int currentHp;
        }

        /// <summary>
        /// List of finalized part HP deltas
        /// </summary>
        [Serializable]
        private class FinalizedPartHpDeltaList
        {
            public List<FinalizedPartHpDelta> values = new List<FinalizedPartHpDelta>();
        }

        /// <summary>
        /// Finalize children as visuals
        /// </summary>
        public void FinalizeChildrenAsVisuals()
        {
            if (!IsServer)
                return;
            List<BuildingEntity> descendants = CollectDescendantBuildings();
            if (descendants.Count == 0)
                return;

            foreach (BuildingEntity child in descendants)
            {
                if (child == null || child == this)
                    continue;
                UpsertFinalizedChildVisualData(child);
            }

            FinalizedChildVisualDataList payload = new FinalizedChildVisualDataList();
            for (int i = 0; i < _finalizedChildVisualData.Count; ++i)
            {
                FinalizedChildVisualData data = _finalizedChildVisualData[i];
                if (_finalizedPartCurrentHp.TryGetValue(data.partId, out int currentHp))
                    data.currentHp = currentHp;
                if (_finalizedPartMaxHp.TryGetValue(data.partId, out int maxHp))
                    data.maxHp = maxHp;
                payload.values.Add(data);
            }
            string payloadJson = JsonUtility.ToJson(payload);
            finalizedChildrenPayload.Value = payloadJson;
            RPC(RpcFinalizeChildrenAsVisuals, payloadJson);

            for (int i = 0; i < descendants.Count; ++i)
            {
                BuildingEntity child = descendants[i];
                if (child == null || child == this || child.IsDestroyed)
                    continue;
                child.NetworkDestroy(0f);
            }
        }

        /// <summary>
        /// RPC to finalize children as visuals
        /// </summary>
        [AllRpc]
        private void RpcFinalizeChildrenAsVisuals(string payloadJson)
        {
            ApplyFinalizedChildrenPayload(payloadJson);
        }

        /// <summary>
        /// Rebuild finalized child visuals
        /// </summary>
        private void RebuildFinalizedChildVisuals(List<FinalizedChildVisualData> dataList)
        {
            ClearFinalizedChildVisuals();
            _finalizedChildVisualData.Clear();
            _finalizedPartCurrentHp.Clear();
            _finalizedPartMaxHp.Clear();
            _finalizedChildVisualData.AddRange(dataList);
            for (int i = 0; i < dataList.Count; ++i)
            {
                if (!string.IsNullOrEmpty(dataList[i].partId))
                {
                    _finalizedPartCurrentHp[dataList[i].partId] = dataList[i].currentHp;
                    _finalizedPartMaxHp[dataList[i].partId] = dataList[i].maxHp > 0 ? dataList[i].maxHp : MaxHp;
                }
                if (dataList[i].currentHp <= 0)
                    continue;
                if (!GameInstance.BuildingEntities.TryGetValue(dataList[i].entityId, out BuildingEntity prefab) || prefab == null)
                    continue;
                GameObject visual = Instantiate(prefab.gameObject, EntityTransform);
                visual.transform.localPosition = dataList[i].localPosition;
                visual.transform.localEulerAngles = dataList[i].localEulerAngles;
                visual.transform.localScale = dataList[i].localScale;
                SetLayerAndTagRecursively(visual, gameObject.layer, gameObject.tag);
                // Route socket ownership checks through root building after finalization.
                BuildingArea[] areas = visual.GetComponentsInChildren<BuildingArea>(true);
                for (int j = 0; j < areas.Length; ++j)
                {
                    areas[j].entity = this;
                }
                DamageableHitBox[] visualHitBoxes = GetOrCreateFinalizedVisualHitBoxes(visual);
                for (int j = 0; j < visualHitBoxes.Length; ++j)
                {
                    visualHitBoxes[j].OverrideDamageableEntity(this);
                    visualHitBoxes[j].OverrideSegmentedDamageTarget(this, dataList[i].partId);
                    visualHitBoxes[j].Setup((byte)j);
                }
                // Prevent finalized visual copy from running its own BuildingEntity logic.
                BuildingEntity visualEntity = visual.GetComponent<BuildingEntity>();
                if (visualEntity != null)
                    visualEntity.enabled = false;
                LiteNetLibIdentity visualIdentity = visual.GetComponent<LiteNetLibIdentity>();
                if (visualIdentity != null)
                    visualIdentity.enabled = false;
                LiteNetLibBehaviour[] visualNetBehaviours = visual.GetComponentsInChildren<LiteNetLibBehaviour>(true);
                for (int j = 0; j < visualNetBehaviours.Length; ++j)
                {
                    if (visualNetBehaviours[j] == null)
                        continue;
                    visualNetBehaviours[j].enabled = false;
                }
                _finalizedChildVisualObjects.Add(visual);
                if (!_finalizedPartVisualObjects.TryGetValue(dataList[i].partId, out List<GameObject> pieceVisuals))
                {
                    pieceVisuals = new List<GameObject>();
                    _finalizedPartVisualObjects[dataList[i].partId] = pieceVisuals;
                }
                pieceVisuals.Add(visual);
            }
            RefreshFinalizedRootHitBoxes();
        }

        private void OnFinalizedChildrenPayloadChange(bool isInitial, string oldPayload, string newPayload)
        {
            // Server already has authoritative finalized state.
            if (IsServer)
                return;
            ApplyFinalizedChildrenPayload(newPayload);
        }

        private void ApplyFinalizedChildrenPayload(string payloadJson)
        {
            if (string.IsNullOrEmpty(payloadJson))
            {
                ClearFinalizedChildVisuals();
                _finalizedChildVisualData.Clear();
                _finalizedPartCurrentHp.Clear();
                _finalizedPartMaxHp.Clear();
                return;
            }

            FinalizedChildVisualDataList payload = JsonUtility.FromJson<FinalizedChildVisualDataList>(payloadJson);
            if (payload == null || payload.values == null)
                return;
            RebuildFinalizedChildVisuals(payload.values);
        }

        private string BuildFinalizedChildrenPayloadJson()
        {
            FinalizedChildVisualDataList payload = new FinalizedChildVisualDataList();
            for (int i = 0; i < _finalizedChildVisualData.Count; ++i)
            {
                FinalizedChildVisualData data = _finalizedChildVisualData[i];
                if (_finalizedPartCurrentHp.TryGetValue(data.partId, out int currentHp))
                    data.currentHp = currentHp;
                if (_finalizedPartMaxHp.TryGetValue(data.partId, out int maxHp))
                    data.maxHp = maxHp;
                payload.values.Add(data);
            }
            return JsonUtility.ToJson(payload);
        }

        private void SyncFinalizedChildrenPayloadToClients()
        {
            if (!IsServer)
                return;
            finalizedChildrenPayload.Value = BuildFinalizedChildrenPayloadJson();
        }

        /// <summary>
        /// Get or create finalized visual hit boxes
        /// </summary>
        private DamageableHitBox[] GetOrCreateFinalizedVisualHitBoxes(GameObject visual)
        {
            List<DamageableHitBox> hitBoxes = new List<DamageableHitBox>(visual.GetComponentsInChildren<DamageableHitBox>(true));
            Collider[] colliders = visual.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; ++i)
            {
                if (colliders[i] == null)
                    continue;
                if (!colliders[i].GetComponent<IUnHittable>().IsNull() || colliders[i].GetComponent<BuildingArea>() != null)
                    continue;
                DamageableHitBox hitBox = colliders[i].GetComponent<DamageableHitBox>();
                if (hitBox == null)
                {
                    hitBox = colliders[i].gameObject.AddComponent<DamageableHitBox>();
                    hitBoxes.Add(hitBox);
                }
            }

            Collider2D[] colliders2D = visual.GetComponentsInChildren<Collider2D>(true);
            for (int i = 0; i < colliders2D.Length; ++i)
            {
                if (colliders2D[i] == null)
                    continue;
                if (!colliders2D[i].GetComponent<IUnHittable>().IsNull() || colliders2D[i].GetComponent<BuildingArea>() != null)
                    continue;
                DamageableHitBox hitBox = colliders2D[i].GetComponent<DamageableHitBox>();
                if (hitBox == null)
                {
                    hitBox = colliders2D[i].gameObject.AddComponent<DamageableHitBox>();
                    hitBoxes.Add(hitBox);
                }
            }

            if (hitBoxes.Count > 0)
            {
                return hitBoxes.ToArray();
            }

            if (colliders != null && colliders.Length > 0)
            {
                Bounds bounds = colliders[0].bounds;
                for (int i = 1; i < colliders.Length; ++i)
                {
                    bounds.Encapsulate(colliders[i].bounds);
                }

                GameObject hitBoxObject = new GameObject("_FinalizedHitBox3D");
                hitBoxObject.transform.SetParent(visual.transform, true);
                hitBoxObject.transform.position = bounds.center;
                hitBoxObject.transform.rotation = Quaternion.identity;

                BoxCollider boxCollider = hitBoxObject.AddComponent<BoxCollider>();
                boxCollider.center = Vector3.zero;
                boxCollider.size = bounds.size;
                boxCollider.isTrigger = false;
                DamageableHitBox created = hitBoxObject.AddComponent<DamageableHitBox>();
                return new DamageableHitBox[] { created };
            }

            if (colliders2D != null && colliders2D.Length > 0)
            {
                Bounds bounds = colliders2D[0].bounds;
                for (int i = 1; i < colliders2D.Length; ++i)
                {
                    bounds.Encapsulate(colliders2D[i].bounds);
                }

                GameObject hitBoxObject = new GameObject("_FinalizedHitBox2D");
                hitBoxObject.transform.SetParent(visual.transform, true);
                hitBoxObject.transform.position = bounds.center;
                hitBoxObject.transform.rotation = Quaternion.identity;

                BoxCollider2D boxCollider = hitBoxObject.AddComponent<BoxCollider2D>();
                boxCollider.offset = Vector2.zero;
                boxCollider.size = new Vector2(bounds.size.x, bounds.size.y);
                boxCollider.isTrigger = false;
                DamageableHitBox created = hitBoxObject.AddComponent<DamageableHitBox>();
                return new DamageableHitBox[] { created };
            }

            Logging.LogWarning(LogTag, $"No hitboxes/colliders found for finalized visual `{visual.name}`");
            return Array.Empty<DamageableHitBox>();
        }
        /// <summary>
        /// Set layer and tag recursively
        /// </summary>
        private void SetLayerAndTagRecursively(GameObject root, int layer, string tag)
        {
            if (root == null)
                return;
            root.layer = layer;
            root.tag = tag;
            Transform[] children = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; ++i)
            {
                if (children[i] == null)
                    continue;
                children[i].gameObject.layer = layer;
                children[i].gameObject.tag = tag;
            }
        }

        /// <summary>
        /// Clear finalized child visuals
        /// </summary>
        private void ClearFinalizedChildVisuals()
        {
            for (int i = 0; i < _finalizedChildVisualObjects.Count; ++i)
            {
                if (_finalizedChildVisualObjects[i] == null)
                    continue;
                Destroy(_finalizedChildVisualObjects[i]);
            }
            _finalizedChildVisualObjects.Clear();
            _finalizedPartVisualObjects.Clear();
            RefreshFinalizedRootHitBoxes();
        }

        /// <summary>
        /// Rebuild root hit box cache so hit registration can resolve finalized child hit boxes.
        /// </summary>
        private void RefreshFinalizedRootHitBoxes()
        {
            DamageableHitBox[] refreshedHitBoxes = GetComponentsInChildren<DamageableHitBox>(true);
            if (refreshedHitBoxes == null || refreshedHitBoxes.Length == 0)
            {
                HitBoxes = Array.Empty<DamageableHitBox>();
                return;
            }
            Array.Sort(refreshedHitBoxes, CompareHitBoxesDeterministicOrder);
            HitBoxes = refreshedHitBoxes;

            int maxHitBoxCount = byte.MaxValue + 1;
            int setupCount = Mathf.Min(HitBoxes.Length, maxHitBoxCount);
            for (int i = 0; i < setupCount; ++i)
            {
                HitBoxes[i].Setup((byte)i);
            }

            if (HitBoxes.Length > maxHitBoxCount)
            {
                Logging.LogWarning(LogTag, $"Hitbox count ({HitBoxes.Length}) exceeds supported max ({maxHitBoxCount}). Extra hitboxes won't be hit-reg addressable.");
            }

        }

        private static int CompareHitBoxesDeterministicOrder(DamageableHitBox a, DamageableHitBox b)
        {
            if (ReferenceEquals(a, b))
                return 0;
            if (a == null)
                return -1;
            if (b == null)
                return 1;
            string pathA = GetTransformPathWithSiblingIndex(a.transform);
            string pathB = GetTransformPathWithSiblingIndex(b.transform);
            return string.CompareOrdinal(pathA, pathB);
        }

        private static string GetTransformPathWithSiblingIndex(Transform transform)
        {
            if (transform == null)
                return string.Empty;
            string path = $"{transform.GetSiblingIndex()}:{transform.name}";
            Transform current = transform.parent;
            while (current != null)
            {
                path = $"{current.GetSiblingIndex()}:{current.name}/{path}";
                current = current.parent;
            }
            return path;
        }

        /// <summary>
        /// Upsert finalized child visual data
        /// </summary>
        private void UpsertFinalizedChildVisualData(BuildingEntity child)
        {
            string partId = GenerateUniqueFinalizedPartId(child);
            int index = _finalizedChildVisualData.FindIndex(data => data.partId == partId);
            if (index >= 0)
                return;

            _finalizedPartCurrentHp[partId] = child.CurrentHp;
            _finalizedPartMaxHp[partId] = child.MaxHp;
            _finalizedChildVisualData.Add(new FinalizedChildVisualData()
            {
                partId = partId,
                entityId = child.EntityId,
                currentHp = child.CurrentHp,
                maxHp = child.MaxHp,
                localPosition = EntityTransform.InverseTransformPoint(child.EntityTransform.position),
                localEulerAngles = (Quaternion.Inverse(EntityTransform.rotation) * child.EntityTransform.rotation).eulerAngles,
                localScale = child.EntityTransform.localScale,
            });
        }

        /// <summary>
        /// Generate a unique finalized part id
        /// </summary>
        private string GenerateUniqueFinalizedPartId(BuildingEntity child)
        {
            // Reuse child.Id when available, then suffix for collisions.
            string basePartId = child != null && !string.IsNullOrEmpty(child.Id)
                ? child.Id
                : $"{child.EntityId}_{child.ObjectId}_{child.EntityTransform.position.x:0.###}_{child.EntityTransform.position.y:0.###}_{child.EntityTransform.position.z:0.###}";
            if (_finalizedPartCurrentHp.ContainsKey(basePartId))
            {
                int suffix = 1;
                string uniqueId = $"{basePartId}_{suffix}";
                while (_finalizedPartCurrentHp.ContainsKey(uniqueId))
                {
                    ++suffix;
                    uniqueId = $"{basePartId}_{suffix}";
                }
                return uniqueId;
            }
            return basePartId;
        }

        /// <summary>
        /// Receive damage to a segmented part
        /// </summary>
        public void ReceiveSegmentedPartDamage(string partId, HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed, float hitBoxDamageRate)
        {
            if (!IsServer || string.IsNullOrEmpty(partId))
                return;
            if (!_finalizedPartCurrentHp.TryGetValue(partId, out int currentHp))
                return;
            if (currentHp <= 0)
                return;

            float calculatingTotalDamage = 0f;
            if (damageAmounts != null)
            {
                foreach (DamageElement damageElement in damageAmounts.Keys)
                {
                    calculatingTotalDamage += damageAmounts[damageElement].Random(randomSeed) * hitBoxDamageRate;
                }
            }

            int totalDamage = CurrentGameInstance.GameplayRule.GetTotalDamage(fromPosition, instigator, this, calculatingTotalDamage, weapon, skill, skillLevel);
            if (totalDamage < 0)
                totalDamage = 0;
            if (totalDamage == 0)
                return;

            currentHp -= totalDamage;
            if (currentHp < 0)
                currentHp = 0;
            _finalizedPartCurrentHp[partId] = currentHp;
            int dataIndex = _finalizedChildVisualData.FindIndex(data => data.partId == partId);
            if (dataIndex >= 0)
            {
                FinalizedChildVisualData data = _finalizedChildVisualData[dataIndex];
                data.currentHp = currentHp;
                _finalizedChildVisualData[dataIndex] = data;
            }
            CallRpcAppendCombatText(CombatAmountType.NormalDamage, HitEffectsSourceType.None, 0, totalDamage);
            EnqueueFinalizedPartHpDelta(partId, currentHp);
        }

        /// <summary>
        /// Try to get the current HP and max HP of a segmented part
        /// </summary>
        public bool TryGetSegmentedPartHp(string partId, out int currentHp, out int maxHp)
        {
            currentHp = 0;
            maxHp = 0;
            if (string.IsNullOrEmpty(partId))
                return false;
            if (!_finalizedPartCurrentHp.TryGetValue(partId, out currentHp))
                return false;
            if (_finalizedPartMaxHp.TryGetValue(partId, out int tempMaxHp))
                maxHp = tempMaxHp;
            else
                maxHp = MaxHp;
            return true;
        }

        /// <summary>
        /// Enqueue a finalized part HP delta
        /// </summary>
        private void EnqueueFinalizedPartHpDelta(string partId, int currentHp)
        {
            if (!IsServer || string.IsNullOrEmpty(partId))
                return;
            _pendingFinalizedPartHpDeltas[partId] = currentHp;
        }

        /// <summary>
        /// Flush pending finalized part HP deltas
        /// </summary>
        private void FlushPendingFinalizedPartHpDeltas(bool force = false)
        {
            if (!IsServer || _pendingFinalizedPartHpDeltas.Count == 0)
                return;
            float now = Time.unscaledTime;
            float interval = Mathf.Max(0.05f, segmentedHpDeltaSendInterval);
            if (!force && now - _lastSegmentedHpDeltaSendTime < interval)
                return;
            _lastSegmentedHpDeltaSendTime = now;

            FinalizedPartHpDeltaList payload = new FinalizedPartHpDeltaList();
            foreach (KeyValuePair<string, int> entry in _pendingFinalizedPartHpDeltas)
            {
                payload.values.Add(new FinalizedPartHpDelta()
                {
                    partId = entry.Key,
                    currentHp = entry.Value,
                });
            }
            _pendingFinalizedPartHpDeltas.Clear();
            string payloadJson = JsonUtility.ToJson(payload);
            RPC(RpcApplyFinalizedPieceHpDeltas, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, payloadJson);
            // Keep full snapshot in sync so relogging/late-join clients receive current segmented HP.
            SyncFinalizedChildrenPayloadToClients();
        }

        /// <summary>
        /// RPC to apply finalized part HP deltas
        /// </summary>
        // IMPORTANT: Keep RPC method name stable for network hash compatibility.
        [AllRpc]
        private void RpcApplyFinalizedPieceHpDeltas(string payloadJson)
        {
            FinalizedPartHpDeltaList payload = JsonUtility.FromJson<FinalizedPartHpDeltaList>(payloadJson);
            if (payload == null || payload.values == null)
                return;
            for (int i = 0; i < payload.values.Count; ++i)
            {
                ApplyFinalizedPartHp(payload.values[i].partId, payload.values[i].currentHp);
            }
        }

        /// <summary>
        /// Apply a finalized part HP delta
        /// </summary>
        private void ApplyFinalizedPartHp(string partId, int currentHp)
        {
            if (string.IsNullOrEmpty(partId))
                return;
            _finalizedPartCurrentHp[partId] = currentHp;
            int dataIndex = _finalizedChildVisualData.FindIndex(data => data.partId == partId);
            if (dataIndex >= 0)
            {
                FinalizedChildVisualData data = _finalizedChildVisualData[dataIndex];
                data.currentHp = currentHp;
                _finalizedChildVisualData[dataIndex] = data;
            }

            bool isActive = currentHp > 0;
            if (!_finalizedPartVisualObjects.TryGetValue(partId, out List<GameObject> visuals) || visuals == null)
                return;
            for (int i = 0; i < visuals.Count; ++i)
            {
                if (visuals[i] == null)
                    continue;
                visuals[i].SetActive(isActive);
            }
        }


        /// <summary>
        /// RPC to set a finalized part active
        /// </summary>
        // IMPORTANT: Keep RPC method name stable for network hash compatibility.
        [AllRpc]
        private void RpcSetFinalizedPieceActive(string partId, bool isActive)
        {
            if (string.IsNullOrEmpty(partId))
                return;
            if (!_finalizedPartVisualObjects.TryGetValue(partId, out List<GameObject> visuals) || visuals == null)
                return;
            for (int i = 0; i < visuals.Count; ++i)
            {
                if (visuals[i] == null)
                    continue;
                visuals[i].SetActive(isActive);
            }
        }

        /// <summary>
        /// Collect descendant buildings
        /// </summary>
        private List<BuildingEntity> CollectDescendantBuildings()
        {
            Dictionary<string, List<BuildingEntity>> childrenByParentId = new Dictionary<string, List<BuildingEntity>>();
            foreach (IBuildingSaveData building in GameInstance.ServerBuildingHandlers.GetBuildings())
            {
                BuildingEntity child = building as BuildingEntity;
                if (child == null || child == this || string.IsNullOrEmpty(child.ParentId))
                    continue;
                if (!childrenByParentId.TryGetValue(child.ParentId, out List<BuildingEntity> list))
                {
                    list = new List<BuildingEntity>();
                    childrenByParentId[child.ParentId] = list;
                }
                list.Add(child);
            }

            List<BuildingEntity> result = new List<BuildingEntity>();
            HashSet<string> visitedIds = new HashSet<string>();
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(Id);
            visitedIds.Add(Id);

            while (queue.Count > 0)
            {
                string parent = queue.Dequeue();
                if (!childrenByParentId.TryGetValue(parent, out List<BuildingEntity> children))
                    continue;
                for (int i = 0; i < children.Count; ++i)
                {
                    BuildingEntity child = children[i];
                    if (child == null || !visitedIds.Add(child.Id))
                        continue;
                    result.Add(child);
                    queue.Enqueue(child.Id);
                }
            }

            return result;
        }

        public bool IsPositionInBuildDistance(Vector3 builderPosition, Vector3 placePosition)
        {
            return Vector3.Distance(builderPosition, placePosition) <= BuildDistance;
        }

        public bool CanBuild()
        {
            if (Builder == null)
            {
                // Builder destroyed?
                return false;
            }
            if (!IsPositionInBuildDistance(Builder.EntityTransform.position, EntityTransform.position))
            {
                // Too far from builder?
                return false;
            }
            if (_triggerObjects.Count > 0)
            {
                // Triggered something?
                return false;
            }
            if (LimitSurfaceHitNormalAngle)
            {
                float angle = GameplayUtils.GetPitchByDirection(HitSurfaceNormal);
                if (angle < LimitSurfaceHitNormalAngleMin || angle > LimitSurfaceHitNormalAngleMax)
                    return false;
            }
            if (BuildingArea != null)
            {
                // Must build on building area
                return BuildingArea.AllowToBuild(this);
            }
            else
            {
                // Can build on any surface and it hit surface?
                return canBuildOnAnySurface && HitSurface;
            }
        }

        public bool CanRepairByMenu()
        {
            // Ensure repair cache/menu flag is initialized from serialized `repairs`.
            _ = CacheRepairs;
            return _repairDataForMenu.HasValue;
        }

        public bool TryGetRepairAmount(BasePlayerCharacterEntity repairPlayer, out int repairAmount, out UITextKeys errorMessage)
        {
            repairAmount = 0;
            errorMessage = UITextKeys.NONE;
            if (!CanRepairByMenu())
                return false;
            return TryGetRepairAmountByMissingHp(repairPlayer, Mathf.Max(0, MaxHp - CurrentHp), _repairDataForMenu.Value, out repairAmount, out errorMessage);
        }

        public bool TryGetRepairAmount(BasePlayerCharacterEntity repairPlayer, string partId, out int repairAmount, out UITextKeys errorMessage)
        {
            repairAmount = 0;
            errorMessage = UITextKeys.NONE;
            if (string.IsNullOrEmpty(partId))
                return TryGetRepairAmount(repairPlayer, out repairAmount, out errorMessage);
            if (!CanRepairByMenu())
                return false;
            if (!TryGetSegmentedPartHp(partId, out int currentHp, out int maxHp))
                return TryGetRepairAmount(repairPlayer, out repairAmount, out errorMessage);
            return TryGetRepairAmountByMissingHp(repairPlayer, Mathf.Max(0, maxHp - currentHp), _repairDataForMenu.Value, out repairAmount, out errorMessage);
        }

        public bool Repair(BasePlayerCharacterEntity repairPlayer, out UITextKeys errorMessage)
        {
            if (!TryGetRepairAmount(repairPlayer, out int repairAmount, out errorMessage))
                return false;
            CurrentHp += repairAmount;
            return true;
        }

        public bool Repair(BasePlayerCharacterEntity repairPlayer, string partId, out UITextKeys errorMessage)
        {
            if (string.IsNullOrEmpty(partId))
                return Repair(repairPlayer, out errorMessage);
            if (!TryGetRepairAmount(repairPlayer, partId, out int repairAmount, out errorMessage))
                return false;
            if (!_finalizedPartCurrentHp.TryGetValue(partId, out int currentHp))
                return Repair(repairPlayer, out errorMessage);
            int maxHp = _finalizedPartMaxHp.TryGetValue(partId, out int tempMaxHp) ? tempMaxHp : MaxHp;
            currentHp += repairAmount;
            if (currentHp > maxHp)
                currentHp = maxHp;
            _finalizedPartCurrentHp[partId] = currentHp;
            int dataIndex = _finalizedChildVisualData.FindIndex(data => data.partId == partId);
            if (dataIndex >= 0)
            {
                FinalizedChildVisualData data = _finalizedChildVisualData[dataIndex];
                data.currentHp = currentHp;
                _finalizedChildVisualData[dataIndex] = data;
            }
            ApplyFinalizedPartHp(partId, currentHp);
            EnqueueFinalizedPartHpDelta(partId, currentHp);
            SyncFinalizedChildrenPayloadToClients();
            return true;
        }

        public bool DestroySegmentedPart(string partId)
        {
            if (!IsServer || string.IsNullOrEmpty(partId))
                return false;
            if (!_finalizedPartCurrentHp.TryGetValue(partId, out int currentHp))
                return false;
            if (currentHp <= 0)
                return true;

            currentHp = 0;
            _finalizedPartCurrentHp[partId] = currentHp;
            int dataIndex = _finalizedChildVisualData.FindIndex(data => data.partId == partId);
            if (dataIndex >= 0)
            {
                FinalizedChildVisualData data = _finalizedChildVisualData[dataIndex];
                data.currentHp = currentHp;
                _finalizedChildVisualData[dataIndex] = data;
            }
            ApplyFinalizedPartHp(partId, currentHp);
            EnqueueFinalizedPartHpDelta(partId, currentHp);
            SyncFinalizedChildrenPayloadToClients();
            return true;
        }

        private bool TryGetRepairAmountByMissingHp(BasePlayerCharacterEntity repairPlayer, int missingHp, BuildingRepairData buildingRepairData, out int repairAmount, out UITextKeys errorMessage)
        {
            repairAmount = Mathf.Min(missingHp, buildingRepairData.maxRecoveryHp);
            errorMessage = UITextKeys.NONE;
            if (repairAmount <= 0)
            {
                // No repairing
                return false;
            }
            // Calculate repairable amount
            // Gold
            int requireGold = buildingRepairData.requireGold * repairAmount;
            while (requireGold > repairPlayer.Gold)
            {
                requireGold -= buildingRepairData.requireGold;
                repairAmount--;
                if (repairAmount <= 0)
                {
                    errorMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD;
                    return false;
                }
            }
            // Items
            int i;
            if (buildingRepairData.requireItems != null)
            {
                for (i = 0; i < buildingRepairData.requireItems.Length; ++i)
                {
                    if (buildingRepairData.requireItems[i].item == null || buildingRepairData.requireItems[i].amount == 0)
                        continue;
                    int requireAmount = buildingRepairData.requireItems[i].amount * repairAmount;
                    int currentAmount = repairPlayer.CountNonEquipItems(buildingRepairData.requireItems[i].item.DataId);
                    while (requireAmount > currentAmount)
                    {
                        requireAmount -= buildingRepairData.requireItems[i].amount;
                        repairAmount--;
                        if (repairAmount <= 0)
                        {
                            errorMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                            return false;
                        }
                    }
                }
            }
            // Currencies
            if (buildingRepairData.requireCurrencies != null)
            {
                Dictionary<Currency, int> playerCurrencies = repairPlayer.GetCurrencies();
                for (i = 0; i < buildingRepairData.requireCurrencies.Length; ++i)
                {
                    if (buildingRepairData.requireCurrencies[i].currency == null || buildingRepairData.requireCurrencies[i].amount == 0)
                        continue;
                    if (!playerCurrencies.TryGetValue(buildingRepairData.requireCurrencies[i].currency, out int currentAmount))
                    {
                        repairAmount = 0;
                        errorMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                        return false;
                    }
                    int requireAmount = buildingRepairData.requireCurrencies[i].amount * repairAmount;
                    while (requireAmount > currentAmount)
                    {
                        requireAmount -= buildingRepairData.requireCurrencies[i].amount;
                        repairAmount--;
                        if (repairAmount <= 0)
                        {
                            errorMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool TryGetRepairAmount(BasePlayerCharacterEntity repairPlayer, BuildingRepairData buildingRepairData, out int repairAmount, out UITextKeys errorMessage)
        {
            return TryGetRepairAmountByMissingHp(repairPlayer, Mathf.Max(0, MaxHp - CurrentHp), buildingRepairData, out repairAmount, out errorMessage);
        }

        protected override void ApplyReceiveDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed, out CombatAmountType combatAmountType, out int totalDamage)
        {
            // Repairing
            if (instigator.TryGetEntity(out BasePlayerCharacterEntity attackPlayer) && !weapon.IsEmptySlot() && CacheRepairs.TryGetValue(weapon.GetItem(), out BuildingRepairData buildingRepairData))
            {
                combatAmountType = CombatAmountType.HpRecovery;
                totalDamage = 0;
                if (!TryGetRepairAmount(attackPlayer, buildingRepairData, out int repairAmount, out UITextKeys errorMessage))
                {
                    // Can't repair
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(attackPlayer.ConnectionId, errorMessage);
                    return;
                }
                // Decrease currency
                attackPlayer.Gold -= buildingRepairData.requireGold * repairAmount;
                attackPlayer.DecreaseItems(buildingRepairData.requireItems, repairAmount);
                attackPlayer.DecreaseCurrencies(buildingRepairData.requireCurrencies, repairAmount);
                totalDamage = repairAmount;
                CurrentHp += totalDamage;
                return;
            }

            // Calculate damages
            float calculatingTotalDamage = 0f;
            foreach (DamageElement damageElement in damageAmounts.Keys)
            {
                calculatingTotalDamage += damageAmounts[damageElement].Random(randomSeed);
            }
            // Apply damages
            combatAmountType = CombatAmountType.NormalDamage;
            totalDamage = CurrentGameInstance.GameplayRule.GetTotalDamage(fromPosition, instigator, this, calculatingTotalDamage, weapon, skill, skillLevel);
            if (totalDamage < 0)
                totalDamage = 0;
            CurrentHp -= totalDamage;
        }

        public override void ReceivedDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CombatAmountType combatAmountType, int totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime = false)
        {
            base.ReceivedDamage(position, fromPosition, instigator, damageAmounts, combatAmountType, totalDamage, weapon, skill, skillLevel, buff, isDamageOverTime);
            if (this.IsDead())
                Destroy();
        }

        public virtual void Destroy()
        {
            if (!IsServer)
                return;
            CurrentHp = 0;
            if (_isDestroyed)
                return;
            _isDestroyed = true;
            // Tell clients that the building destroy to play animation at client
            CallRpcOnBuildingDestroy();
            // Drop items
            if (droppingItems != null && droppingItems.Count > 0)
            {
                foreach (ItemAmount droppingItem in droppingItems)
                {
                    if (droppingItem.item == null || droppingItem.amount == 0)
                        continue;
                    ItemDropEntity.Drop(this, RewardGivenType.BuildingDestroyed, CharacterItem.Create(droppingItem.item, 1, droppingItem.amount), System.Array.Empty<string>()).Forget();
                }
            }
            // Destroy this entity
            NetworkDestroy(destroyDelay);
        }

        public void SetupAsBuildMode(BasePlayerCharacterEntity builder)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (Collider collider in colliders)
            {
                collider.isTrigger = true;
                // Use rigidbody to detect trigger events
                Rigidbody rigidbody = collider.gameObject.GetOrAddComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            Collider2D[] colliders2D = GetComponentsInChildren<Collider2D>(true);
            foreach (Collider2D collider in colliders2D)
            {
                collider.isTrigger = true;
                // Use rigidbody to detect trigger events
                Rigidbody2D rigidbody = collider.gameObject.GetOrAddComponent<Rigidbody2D>();
                rigidbody.gravityScale = 0;
                rigidbody.bodyType = RigidbodyType2D.Kinematic;
                rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            IsBuildMode = true;
            Builder = builder;
        }

        public bool AddTriggeredEntity(BaseGameEntity entity)
        {
            if (entity == null || entity.EntityGameObject == EntityGameObject)
                return false;
            _triggerObjects.Add(entity.EntityGameObject);
            _lastAddedTriggerObjectFrame = Time.frameCount;
            return true;
        }

        public bool AddTriggeredComponent(Component component)
        {
            if (component == null)
                return false;
            _triggerObjects.Add(component.gameObject);
            _lastAddedTriggerObjectFrame = Time.frameCount;
            return true;
        }

        public bool AddTriggeredGameObject(GameObject other)
        {
            if (other == null)
                return false;
            _triggerObjects.Add(other);
            _lastAddedTriggerObjectFrame = Time.frameCount;
            return true;
        }

        public override async void OnNetworkDestroy(byte reasons)
        {
            base.OnNetworkDestroy(reasons);
            if (!IsServer)
                return;
            if (reasons == DestroyObjectReasons.RequestedToDestroy)
            {
                // Chain destroy
                foreach (BuildingEntity child in _children)
                {
                    if (child == null || !child.destroyWhenParentDestroyed) continue;
                    child.Destroy();
                }
                _children.Clear();
                await CurrentGameManager.DestroyBuildingEntity(Id, IsSceneObject);
            }
        }

        public bool IsCreator(IPlayerCharacterData playerCharacter)
        {
            return playerCharacter != null && IsCreator(playerCharacter.Id);
        }

        public bool IsCreator(string playerCharacterId)
        {
            return !string.IsNullOrEmpty(CreatorId) &&
                !string.IsNullOrEmpty(playerCharacterId) &&
                CreatorId.Equals(playerCharacterId);
        }

        public virtual void InitSceneObject()
        {
            CurrentHp = MaxHp;
        }

        public override bool NotBeingSelectedOnClick()
        {
            return notBeingSelectedOnClick;
        }

        public virtual float GetActivatableDistance()
        {
            if (activatableDistance > 0f)
                return activatableDistance;
            else
                return GameInstance.Singleton.conversationDistance;
        }

        public virtual bool ShouldClearTargetAfterActivated()
        {
            return false;
        }

        public virtual bool ShouldBeAttackTarget()
        {
            return false;
        }

        public virtual bool ShouldNotActivateAfterFollowed()
        {
            return false;
        }

        public virtual bool CanActivate()
        {
            return false;
        }

        public virtual void OnActivate()
        {
            // Do nothing, override this function to do something
        }

        public virtual bool CanHoldActivate()
        {
            return !this.IsDead();
        }

        public virtual void OnHoldActivate()
        {
            BaseUISceneGameplay.Singleton.ShowCurrentBuildingDialog(this);
        }
    }
}