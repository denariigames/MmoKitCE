using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
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

        public string Id
        {
            get
            {
                if (IsSceneObject)
                    return ZString.Concat(CurrentGameManager.ChannelId, '_', CurrentGameManager.MapInfo.Id, '_', Identity.SceneObjectId);
                else
                    return id;
            }
            set { id.Value = value; }
        }

        public string ParentId
        {
            get { return parentId; }
            set { parentId.Value = value; }
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
            get { return string.Empty; }
            set { }
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
        protected int _lastAddedTriggerObjectFrame;
        protected bool _parentFound = true; // No parent by default
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
                    // Reduce remains life time
                    RemainsLifeTime -= Time.deltaTime;
                    if (RemainsLifeTime < 0)
                    {
                        // Destroy building
                        RemainsLifeTime = 0f;
                        Destroy();
                    }
                }
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
            parentId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            parentId.onChange += OnParentIdChange;
            parentId.redundancyCount = 0;
        }

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            parentId.onChange -= OnParentIdChange;
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
            _parentFound = false; // Set `_parentFound` to `false` to find parent (in `LateUpdate)
            if (string.IsNullOrEmpty(parentId))
            {
                // No parent
                _parentFound = true;
            }
        }

        public void AddChildren(BuildingEntity buildingEntity)
        {
            _children.Add(buildingEntity);
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
            if (LimitSurfaceHitNormalAngle && CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
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
            if (CacheRepairs.Count <= 0)
                return false;
            return _repairDataForMenu.HasValue;
        }

        public bool TryGetRepairAmount(BasePlayerCharacterEntity repairPlayer, out int repairAmount, out UITextKeys errorMessage)
        {
            repairAmount = 0;
            errorMessage = UITextKeys.NONE;
            if (!CanRepairByMenu())
                return false;
            return TryGetRepairAmount(repairPlayer, _repairDataForMenu.Value, out repairAmount, out errorMessage);
        }

        public bool Repair(BasePlayerCharacterEntity repairPlayer, out UITextKeys errorMessage)
        {
            if (!TryGetRepairAmount(repairPlayer, out int repairAmount, out errorMessage))
                return false;
            CurrentHp += repairAmount;
            return true;
        }

        public bool TryGetRepairAmount(BasePlayerCharacterEntity repairPlayer, BuildingRepairData buildingRepairData, out int repairAmount, out UITextKeys errorMessage)
        {
            repairAmount = Mathf.Min(MaxHp - CurrentHp, buildingRepairData.maxRecoveryHp);
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
                rigidbody.isKinematic = true;
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
            return CreatorId.Equals(playerCharacterId);
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
            if (Identity != null && Identity.IsServer && GameInstance.PlayingCharacterEntity != null && Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                return false;
            return !this.IsDead();
        }

        public virtual void OnHoldActivate()
        {
            BaseUISceneGameplay.Singleton.ShowCurrentBuildingDialog(this);
        }
    }
}
