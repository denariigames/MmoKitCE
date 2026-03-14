using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.DevExtension;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UtilsComponents;
using Insthync.UnityEditorUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public abstract partial class BaseCharacterModel : GameEntityModel, IMoveableModel, IHittableModel, IJumppableModel, IPickupableModel, IDeadableModel
    {
        public static readonly object DefaultIKDisabler = new object();

        public int Id { get; protected set; }
        public CharacterModelManager Manager { get; set; }
        public BaseCharacterModel MainModel { get; set; }
        public bool IsMainModel { get { return MainModel == this; } }
        public bool IsActiveModel { get; protected set; } = false;
        public bool IsTpsModel { get; internal set; }
        public bool IsFpsModel { get; internal set; }
        public readonly HashSet<object> IKsDisablers = new HashSet<object>();
        public bool DisableIKs => IKsDisablers.Count > 0;
        public bool UpdateEquipmentImmediately { get; set; }

        [Header("Model Switching Settings")]
        [SerializeField]
        protected GameObject[] activateObjectsWhenSwitchModel = new GameObject[0];
        public GameObject[] ActivateObjectsWhenSwitchModel
        {
            get { return activateObjectsWhenSwitchModel; }
            set { activateObjectsWhenSwitchModel = value; }
        }

        [SerializeField]
        protected GameObject[] deactivateObjectsWhenSwitchModel = new GameObject[0];
        public GameObject[] DeactivateObjectsWhenSwitchModel
        {
            get { return deactivateObjectsWhenSwitchModel; }
            set { deactivateObjectsWhenSwitchModel = value; }
        }

        [SerializeField]
        protected VehicleCharacterModel[] vehicleModels = new VehicleCharacterModel[0];
        public VehicleCharacterModel[] VehicleModels
        {
            get { return vehicleModels; }
            set { vehicleModels = value; }
        }

        [Header("Equipment Containers")]
        [SerializeField]
        protected EquipmentContainer[] equipmentContainers = new EquipmentContainer[0];
        public EquipmentContainer[] EquipmentContainers
        {
            get { return equipmentContainers; }
            set { equipmentContainers = value; }
        }

        [Header("Equipment Layer Settings")]
        [SerializeField]
        protected bool setEquipmentLayerFollowEntity = true;
        public bool SetEquipmentLayerFollowEntity
        {
            get { return setEquipmentLayerFollowEntity; }
            set { setEquipmentLayerFollowEntity = value; }
        }

        [SerializeField]
        protected UnityLayer equipmentLayer;
        public int EquipmentLayer
        {
            get { return equipmentLayer.LayerIndex; }
            set { equipmentLayer = new UnityLayer(value); }
        }

#if UNITY_EDITOR
        [InspectorButton(nameof(SetEquipmentContainersBySetters))]
        public bool setEquipmentContainersBySetters = false;
        [InspectorButton(nameof(DeactivateInstantiatedObjects))]
        public bool deactivateInstantiatedObjects = false;
        [InspectorButton(nameof(ActivateInstantiatedObject))]
        public bool activateInstantiatedObject = false;
#endif


        protected Dictionary<string, EquipmentModel> _equippedModels = new Dictionary<string, EquipmentModel>();
        /// <summary>
        /// { equipPosition(String), model(EquipmentModel) }
        /// </summary>
        public Dictionary<string, EquipmentModel> EquippedModels
        {
            get { return IsMainModel ? _equippedModels : MainModel._equippedModels; }
            set { MainModel._equippedModels = value; }
        }

        protected Dictionary<string, GameObject> _equippedModelObjects = new Dictionary<string, GameObject>();
        /// <summary>
        /// { equipSocket(String), modelObject(GameObject) }
        /// </summary>
        public Dictionary<string, GameObject> EquippedModelObjects
        {
            get { return IsMainModel ? _equippedModelObjects : MainModel._equippedModelObjects; }
            set { MainModel._equippedModelObjects = value; }
        }

        public override Dictionary<string, EffectContainer> CacheEffectContainers
        {
            get { return IsMainModel ? base.CacheEffectContainers : MainModel.CacheEffectContainers; }
        }

        protected Dictionary<int, VehicleCharacterModel> _cacheVehicleModels;
        /// <summary>
        /// { vehicleType(Int32), vehicleCharacterModel(VehicleCharacterModel) }
        /// </summary>
        public Dictionary<int, VehicleCharacterModel> CacheVehicleModels
        {
            get { return IsMainModel ? _cacheVehicleModels : MainModel._cacheVehicleModels; }
        }

        protected Dictionary<string, EquipmentContainer> _cacheEquipmentModelContainers;
        /// <summary>
        /// { equipSocket(String), container(EquipmentModelContainer) }
        /// </summary>
        public Dictionary<string, EquipmentContainer> CacheEquipmentModelContainers
        {
            get { return IsMainModel ? _cacheEquipmentModelContainers : MainModel._cacheEquipmentModelContainers; }
        }

        protected Dictionary<string, List<GameEffect>> _cacheEffects = new Dictionary<string, List<GameEffect>>();
        /// <summary>
        /// { equipPosition(String), [ effect(GameEffect) ] }
        /// </summary>
        public Dictionary<string, List<GameEffect>> CacheEffects
        {
            get { return IsMainModel ? _cacheEffects : MainModel._cacheEffects; }
        }

        protected BaseEquipmentEntity _cacheRightHandEquipmentEntity;
        public BaseEquipmentEntity CacheRightHandEquipmentEntity
        {
            get { return IsMainModel ? _cacheRightHandEquipmentEntity : MainModel._cacheRightHandEquipmentEntity; }
            set { MainModel._cacheRightHandEquipmentEntity = value; }
        }

        protected BaseEquipmentEntity _cacheLeftHandEquipmentEntity;
        public BaseEquipmentEntity CacheLeftHandEquipmentEntity
        {
            get { return IsMainModel ? _cacheLeftHandEquipmentEntity : MainModel._cacheLeftHandEquipmentEntity; }
            set { MainModel._cacheLeftHandEquipmentEntity = value; }
        }

        protected IAttackRecoiler _cacheAttackRecoiler;
        public IAttackRecoiler CacheAttackRecoiler
        {
            get { return IsMainModel ? _cacheAttackRecoiler : MainModel._cacheAttackRecoiler; }
            set { MainModel._cacheAttackRecoiler = value; }
        }

        public IList<EquipWeapons> SelectableWeaponSets { get; protected set; }
        public byte EquipWeaponSet { get; protected set; }
        public bool IsWeaponsSheathed { get; protected set; }
        public IList<CharacterItem> EquipItems { get; protected set; }
        public IList<CharacterBuff> Buffs { get; protected set; }
        public bool IsDead { get; protected set; }
        public float MoveAnimationSpeedMultiplier { get; protected set; }
        public MovementState MovementState { get; protected set; }
        public ExtraMovementState ExtraMovementState { get; protected set; }
        public Vector2 Direction2D { get; protected set; }
        public bool IsFreezeAnimation { get; protected set; }

        // Public events
        public event UpdateEquipmentModelsDelegate onBeforeUpdateEquipmentModels;
        public event OnInstantiatedEquipmentDelegate onInstantiatedEquipment;

        // Optimize garbage collector
        protected readonly List<string> _tempAddingKeys = new List<string>();
        protected readonly List<string> _tempCachedKeys = new List<string>();
        protected CancellationTokenSource _updateEquipmentModelsCancellationTokenSource;

        protected override void Awake()
        {
            Manager = GetComponent<CharacterModelManager>();
            if (Manager == null)
                Manager = GetComponentInParent<CharacterModelManager>(true);
            Entity = GetComponent<BaseGameEntity>();
            if (Entity == null)
                Entity = GetComponentInParent<BaseGameEntity>();
            Id = GetInstanceID();
            // Can't find manager, this component may attached to non-character entities, so assume that this character model is main model
            if (Manager == null)
            {
                if (Entity != null && Entity.Identity != null)
                    Id = Entity.Identity.HashAssetId;
                MainModel = this;
                InitCacheData();
                SwitchModel(null);
            }
            else
            {
                byte id = Manager.InitTpsModel(this);
                unchecked
                {
                    Id = Entity.Identity.HashAssetId + id;
                }
            }
        }

        internal override void InitCacheData()
        {
            if (_isCacheDataInitialized)
                return;

            base.InitCacheData();
            // Prepare vehicle models
            _cacheVehicleModels = new Dictionary<int, VehicleCharacterModel>();
            if (vehicleModels != null && vehicleModels.Length > 0)
            {
                foreach (VehicleCharacterModel vehicleModel in vehicleModels)
                {
                    if (!vehicleModel.vehicleType) continue;
                    for (int i = 0; i < vehicleModel.modelsForEachSeats.Length; ++i)
                    {
                        vehicleModel.modelsForEachSeats[i].MainModel = this;
                        vehicleModel.modelsForEachSeats[i].IsTpsModel = IsTpsModel;
                        vehicleModel.modelsForEachSeats[i].IsFpsModel = IsFpsModel;
                    }
                    _cacheVehicleModels[vehicleModel.vehicleType.DataId] = vehicleModel;
                }
            }
            // Prepare equipment model containers
            _cacheEquipmentModelContainers = new Dictionary<string, EquipmentContainer>();
            if (equipmentContainers != null && equipmentContainers.Length > 0)
            {
                foreach (EquipmentContainer equipmentContainer in equipmentContainers)
                {
                    if (SetEquipmentLayerFollowEntity)
                        equipmentContainer.defaultModel?.GetOrAddComponent<SetLayerFollowGameObject>((comp) => comp.source = Entity.gameObject);
                    else
                        equipmentContainer.defaultModel?.SetLayerRecursively(EquipmentLayer, true);

                    if (!_cacheEquipmentModelContainers.ContainsKey(equipmentContainer.equipSocket))
                        _cacheEquipmentModelContainers[equipmentContainer.equipSocket] = equipmentContainer;
                }
            }
            // Prepare game effects collection (for buff effects)
            _cacheEffects = new Dictionary<string, List<GameEffect>>();
            // Prepare recoiler
            _cacheAttackRecoiler = GetComponentInChildren<IAttackRecoiler>();
        }

        protected void UpdateObjectsWhenSwitch()
        {
            if (activateObjectsWhenSwitchModel != null &&
                activateObjectsWhenSwitchModel.Length > 0)
            {
                foreach (GameObject obj in activateObjectsWhenSwitchModel)
                {
                    if (!obj.activeSelf)
                        obj.SetActive(true);
                }
            }
            if (deactivateObjectsWhenSwitchModel != null &&
                deactivateObjectsWhenSwitchModel.Length > 0)
            {
                foreach (GameObject obj in deactivateObjectsWhenSwitchModel)
                {
                    if (obj.activeSelf)
                        obj.SetActive(false);
                }
            }
        }

        protected void RevertObjectsWhenSwitch()
        {
            if (activateObjectsWhenSwitchModel != null &&
                activateObjectsWhenSwitchModel.Length > 0)
            {
                foreach (GameObject obj in activateObjectsWhenSwitchModel)
                {
                    if (obj.activeSelf)
                        obj.SetActive(false);
                }
            }
            if (deactivateObjectsWhenSwitchModel != null &&
                deactivateObjectsWhenSwitchModel.Length > 0)
            {
                foreach (GameObject obj in deactivateObjectsWhenSwitchModel)
                {
                    if (!obj.activeSelf)
                        obj.SetActive(true);
                }
            }
        }

        internal virtual void SwitchModel(BaseCharacterModel previousModel)
        {
            if (previousModel != null)
            {
                previousModel.IsActiveModel = false;
                previousModel.OnSwitchingToAnotherModel();
                previousModel.RevertObjectsWhenSwitch();
                OnSwitchingToThisModel();
                SetDefaultAnimations();
                SetIsDead(previousModel.IsDead);
                SetMoveAnimationSpeedMultiplier(previousModel.MoveAnimationSpeedMultiplier);
                SetMovementState(previousModel.MovementState, previousModel.ExtraMovementState, previousModel.Direction2D, previousModel.IsFreezeAnimation);
                SetEquipItems(previousModel.EquipItems, previousModel.SelectableWeaponSets, previousModel.EquipWeaponSet, previousModel.IsWeaponsSheathed);
                SetBuffs(previousModel.Buffs);
                IsActiveModel = true;
                UpdateObjectsWhenSwitch();
                OnSwitchedToThisModel();
                previousModel.OnSwitchedToAnotherModel();
            }
            else
            {
                OnSwitchingToThisModel();
                SetDefaultAnimations();
                SetEquipItems(EquipItems, SelectableWeaponSets, EquipWeaponSet, IsWeaponsSheathed);
                SetBuffs(Buffs);
                IsActiveModel = true;
                UpdateObjectsWhenSwitch();
                OnSwitchedToThisModel();
            }
        }

        internal virtual void OnSwitchingToAnotherModel()
        {

        }

        internal virtual void OnSwitchedToAnotherModel()
        {

        }

        internal virtual void OnSwitchingToThisModel()
        {

        }

        internal virtual void OnSwitchedToThisModel()
        {

        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (equipmentContainers != null)
            {
                foreach (EquipmentContainer equipmentContainer in equipmentContainers)
                {
                    if (equipmentContainer.transform == null) continue;
                    Gizmos.color = new Color(0, 1, 0, 0.5f);
                    Gizmos.DrawWireSphere(equipmentContainer.transform.position, 0.1f);
                    Gizmos.DrawSphere(equipmentContainer.transform.position, 0.03f);
                    Handles.Label(equipmentContainer.transform.position, equipmentContainer.equipSocket + "(Equipment)");
                    Gizmos.color = new Color(0, 0, 1, 0.5f);
                    DrawArrow.ForGizmo(equipmentContainer.transform.position, equipmentContainer.transform.forward, 0.5f, 0.1f);
                    Gizmos.color = new Color(0, 1, 0, 0.5f);
                    DrawArrow.ForGizmo(equipmentContainer.transform.position, -equipmentContainer.transform.up, 0.5f, 0.1f);
                }
            }
        }
#endif

#if UNITY_EDITOR
        [ContextMenu("Set Equipment Containers By Setters", false, 1000301)]
        public void SetEquipmentContainersBySetters()
        {
            EquipmentContainerSetter[] setters = GetComponentsInChildren<EquipmentContainerSetter>();
            if (setters != null && setters.Length > 0)
            {
                foreach (EquipmentContainerSetter setter in setters)
                {
                    setter.ApplyToCharacterModel(this);
                }
            }
            this.InvokeInstanceDevExtMethods("SetEquipmentContainersBySetters");
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Deactivate Instantiated Objects", false, 1000302)]
        public void DeactivateInstantiatedObjects()
        {
            if (EquipmentContainers != null && EquipmentContainers.Length > 0)
            {
                for (int i = 0; i < EquipmentContainers.Length; ++i)
                {
                    EquipmentContainers[i].DeactivateInstantiatedObjects();
                    EquipmentContainers[i].SetActiveDefaultModel(true);
                    EquipmentContainers[i].DeactivateInstantiatedObjectGroups();
                    EquipmentContainers[i].SetActiveDefaultModelGroup(true);
                }
            }
        }

        [ContextMenu("Activate Instantiated Object", false, 1000303)]
        public void ActivateInstantiatedObject()
        {
            if (EquipmentContainers != null && EquipmentContainers.Length > 0)
            {
                for (int i = 0; i < EquipmentContainers.Length; ++i)
                {
                    EquipmentContainers[i].SetActiveDefaultModel(false);
                    EquipmentContainers[i].ActivateInstantiatedObject(EquipmentContainers[i].activatingInstantiateObjectIndex);
                    EquipmentContainers[i].SetActiveDefaultModelGroup(false);
                    EquipmentContainers[i].ActivateInstantiatedObjectGroup(EquipmentContainers[i].activatingInstantiateObjectIndex);
                }
            }
        }
#endif

        public virtual async void SetEquipItems(IList<CharacterItem> equipItems, IList<EquipWeapons> selectableWeaponSets, byte equipWeaponSet, bool isWeaponsSheathed)
        {
            EquipItems = equipItems;
            SelectableWeaponSets = selectableWeaponSets;
            EquipWeaponSet = equipWeaponSet;
            IsWeaponsSheathed = isWeaponsSheathed;
            await UpdateEquipmentModels(equipItems, selectableWeaponSets, equipWeaponSet, isWeaponsSheathed);
        }

        public void SetEquipItemsImmediately(IList<CharacterItem> equipItems, IList<EquipWeapons> selectableWeaponSets, byte equipWeaponSet, bool isWeaponsSheathed)
        {
            UpdateEquipmentImmediately = true;
            SetEquipItems(equipItems, selectableWeaponSets, equipWeaponSet, isWeaponsSheathed);
            UpdateEquipmentImmediately = false;
        }

        public async UniTask UpdateEquipmentModels(IList<CharacterItem> equipItems, IList<EquipWeapons> selectableWeaponSets, byte equipWeaponSet, bool isWeaponsSheathed)
        {
            if (_updateEquipmentModelsCancellationTokenSource != null &&
                !_updateEquipmentModelsCancellationTokenSource.IsCancellationRequested)
            {
                _updateEquipmentModelsCancellationTokenSource.Cancel();
                _updateEquipmentModelsCancellationTokenSource = null;
            }
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            _updateEquipmentModelsCancellationTokenSource = cancellationTokenSource;

            // Prepared data
            EquipmentContainer tempContainer;
            EquipmentModel tempEquipmentModel;
            BaseEquipmentEntity tempEquipmentEntity;
            GameObject tempEquipmentObject;
            EquipmentInstantiatedObjectGroup tempEquipmentObjectGroup;
            Dictionary<string, EquipmentModel> showingModels = new Dictionary<string, EquipmentModel>();
            Dictionary<string, EquipmentModel> storingModels = new Dictionary<string, EquipmentModel>();
            HashSet<string> unequippingSockets = new HashSet<string>(EquippedModels.Keys);

            // Setup appearances before equip items
            if (onBeforeUpdateEquipmentModels != null)
                onBeforeUpdateEquipmentModels.Invoke(cancellationTokenSource, this, showingModels, storingModels, unequippingSockets);

            // Setup equipping models from equip items
            if (equipItems != null && equipItems.Count > 0)
            {
                foreach (CharacterItem equipItem in equipItems)
                {
                    IArmorItem armorItem = equipItem.GetArmorItem();
                    if (armorItem == null)
                        continue;
                    await SetupEquippingModels(cancellationTokenSource, showingModels, storingModels, unequippingSockets, armorItem.EquipmentModels, armorItem.GetEquipPosition(), equipItem);
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        cancellationTokenSource.Dispose();
                        return;
                    }
                }
            }

            // Setup equipping models from equip weapons
            EquipWeapons equipWeapons;
            if (isWeaponsSheathed || selectableWeaponSets == null || selectableWeaponSets.Count == 0)
            {
                equipWeapons = new EquipWeapons();
            }
            else
            {
                if (equipWeaponSet >= selectableWeaponSets.Count)
                {
                    // Issues occuring, so try to simulate data
                    // Create a new list to make sure that changes won't be applied to the source list (the source list must be readonly)
                    selectableWeaponSets = new List<EquipWeapons>(selectableWeaponSets);
                    while (equipWeaponSet >= selectableWeaponSets.Count)
                    {
                        selectableWeaponSets.Add(new EquipWeapons());
                    }
                }
                equipWeapons = selectableWeaponSets[equipWeaponSet];
            }
            IEquipmentItem rightHandItem = equipWeapons.GetRightHandEquipmentItem();
            IEquipmentItem leftHandItem = equipWeapons.GetLeftHandEquipmentItem();
            if (rightHandItem != null && rightHandItem.IsWeapon())
            {
                await SetupEquippingModels(cancellationTokenSource, showingModels, storingModels, unequippingSockets, (rightHandItem as IWeaponItem).EquipmentModels, GameDataConst.EQUIP_POSITION_RIGHT_HAND, equipWeapons.rightHand);
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Dispose();
                    return;
                }
            }

            if (leftHandItem != null && leftHandItem.IsWeapon())
            {
                await SetupEquippingModels(cancellationTokenSource, showingModels, storingModels, unequippingSockets, (leftHandItem as IWeaponItem).OffHandEquipmentModels, GameDataConst.EQUIP_POSITION_LEFT_HAND, equipWeapons.leftHand);
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Dispose();
                    return;
                }
            }

            if (leftHandItem != null && leftHandItem.IsShield())
            {
                await SetupEquippingModels(cancellationTokenSource, showingModels, storingModels, unequippingSockets, (leftHandItem as IShieldItem).EquipmentModels, GameDataConst.EQUIP_POSITION_LEFT_HAND, equipWeapons.leftHand);
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Dispose();
                    return;
                }
            }

            if (selectableWeaponSets != null && selectableWeaponSets.Count > 0)
            {
                for (byte i = 0; i < selectableWeaponSets.Count; ++i)
                {
                    if (isWeaponsSheathed || i != equipWeaponSet)
                    {
                        equipWeapons = selectableWeaponSets[i];
                        rightHandItem = equipWeapons.GetRightHandEquipmentItem();
                        leftHandItem = equipWeapons.GetLeftHandEquipmentItem();

                        if (rightHandItem != null && rightHandItem.IsWeapon())
                        {
                            await SetupEquippingModels(cancellationTokenSource, showingModels, storingModels, unequippingSockets, (rightHandItem as IWeaponItem).SheathModels, ZString.Concat(GameDataConst.EQUIP_POSITION_RIGHT_HAND, "_", i), equipWeapons.rightHand, true, i);
                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                cancellationTokenSource.Dispose();
                                return;
                            }
                        }

                        if (leftHandItem != null && leftHandItem.IsWeapon())
                        {
                            await SetupEquippingModels(cancellationTokenSource, showingModels, storingModels, unequippingSockets, (leftHandItem as IWeaponItem).OffHandSheathModels, ZString.Concat(GameDataConst.EQUIP_POSITION_LEFT_HAND, "_", i), equipWeapons.leftHand, true, i);
                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                cancellationTokenSource.Dispose();
                                return;
                            }
                        }

                        if (leftHandItem != null && leftHandItem.IsShield())
                        {
                            await SetupEquippingModels(cancellationTokenSource, showingModels, storingModels, unequippingSockets, (leftHandItem as IShieldItem).SheathModels, ZString.Concat(GameDataConst.EQUIP_POSITION_LEFT_HAND, "_", i), equipWeapons.leftHand, true, i);
                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                cancellationTokenSource.Dispose();
                                return;
                            }
                        }
                    }
                }
            }

            // Destroy unequipped item models, and show default models
            foreach (string unequippingSocket in unequippingSockets)
            {
                ClearEquippedModel(unequippingSocket);

                if (!CacheEquipmentModelContainers.TryGetValue(unequippingSocket, out tempContainer))
                    continue;

                tempContainer.DeactivateInstantiatedObjects();
                tempContainer.DeactivateInstantiatedObjectGroups();
                tempContainer.SetActiveDefaultModel(true);
                tempContainer.SetActiveDefaultModelGroup(true);
            }

            bool shouldClearLeftHandCache = true;
            bool shouldClearRightHandCache = true;
            foreach (string equipSocket in showingModels.Keys)
            {
                ClearEquippedModel(equipSocket, shouldClearLeftHandCache, shouldClearRightHandCache);

                if (!CacheEquipmentModelContainers.TryGetValue(equipSocket, out tempContainer))
                    continue;

                tempEquipmentModel = showingModels[equipSocket];
                tempEquipmentObject = null;
                tempEquipmentObjectGroup = null;
                if (tempEquipmentModel.useInstantiatedObject)
                {
                    bool modelActivated = false;

                    tempContainer.SetActiveDefaultModel(false);
                    if (!tempContainer.ActivateInstantiatedObject(tempEquipmentModel.instantiatedObjectIndex))
                    {
                        tempContainer.SetActiveDefaultModel(true);
                        tempEquipmentObject = tempContainer.defaultModel;
                        modelActivated = true;
                    }
                    else
                    {
                        tempEquipmentObject = tempContainer.instantiatedObjects[tempEquipmentModel.instantiatedObjectIndex];
                    }

                    tempContainer.SetActiveDefaultModelGroup(false);
                    if (!tempContainer.ActivateInstantiatedObjectGroup(tempEquipmentModel.instantiatedObjectIndex))
                    {
                        tempContainer.SetActiveDefaultModelGroup(true);
                        tempEquipmentObjectGroup = tempContainer.defaultInstantiatedObjectGroup;
                        modelActivated = true;
                    }
                    else
                    {
                        tempEquipmentObjectGroup = tempContainer.instantiatedObjectGroups[tempEquipmentModel.instantiatedObjectIndex];
                    }

                    if (!modelActivated)
                    {
                        continue;
                    }
                }
                else
                {
                    // Instantiate model, setup transform and activate game object
                    tempContainer.DeactivateInstantiatedObjects();
                    tempContainer.DeactivateInstantiatedObjectGroups();
                    tempContainer.SetActiveDefaultModel(false);
                    tempContainer.SetActiveDefaultModelGroup(false);
                    GameObject meshPrefab = await tempEquipmentModel.GetMeshPrefab();
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        cancellationTokenSource.Dispose();
                        return;
                    }
                    if (tempContainer.transform != null)
                    {
                        tempEquipmentObject = Instantiate(meshPrefab, tempContainer.transform);
                        tempEquipmentObject.transform.localPosition = tempEquipmentModel.localPosition;
                        tempEquipmentObject.transform.localEulerAngles = tempEquipmentModel.localEulerAngles;
                        if (!tempEquipmentModel.doNotChangeScale)
                            tempEquipmentObject.transform.localScale = tempEquipmentModel.localScale.Equals(Vector3.zero) ? Vector3.one : tempEquipmentModel.localScale;
                        tempEquipmentObject.gameObject.SetActive(true);
                        if (SetEquipmentLayerFollowEntity)
                            tempEquipmentObject.gameObject.GetOrAddComponent<SetLayerFollowGameObject>((comp) => comp.source = Entity?.gameObject ?? null);
                        else
                            tempEquipmentObject.gameObject.SetLayerRecursively(EquipmentLayer, true);
                        tempEquipmentObject.RemoveComponentsInChildren<Collider>(false);
                        EquippedModelObjects[equipSocket] = tempEquipmentObject;
                    }
                }

                // Setup equipment entity
                tempEquipmentEntity = null;
                if (tempEquipmentObject != null)
                {
                    tempEquipmentEntity = tempEquipmentObject.GetComponent<BaseEquipmentEntity>();
                    if (tempEquipmentEntity != null)
                    {
                        tempEquipmentEntity.Setup(this, equipSocket, tempEquipmentModel.equipPosition, tempEquipmentModel.item);
#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
                        GameObject editorAsset = null;
                        if (tempEquipmentModel.AddressableMeshPrefab.IsDataValid())
                            editorAsset = tempEquipmentModel.AddressableMeshPrefab.editorAsset;
                        if (editorAsset == null)
                            editorAsset = tempEquipmentModel.MeshPrefab;
                        tempEquipmentEntity.SetupRefToPrefab(editorAsset);
#endif
                    }
                    if (CacheRightHandEquipmentEntity == null && GameDataConst.EQUIP_POSITION_RIGHT_HAND.Equals(tempEquipmentModel.equipPosition))
                    {
                        CacheRightHandEquipmentEntity = tempEquipmentEntity;
                        shouldClearRightHandCache = false;
                    }
                    if (CacheLeftHandEquipmentEntity == null && GameDataConst.EQUIP_POSITION_LEFT_HAND.Equals(tempEquipmentModel.equipPosition))
                    {
                        CacheLeftHandEquipmentEntity = tempEquipmentEntity;
                        shouldClearLeftHandCache = false;
                    }
                }

                tempEquipmentModel.InvokeOnInstantiated(tempEquipmentObject, tempEquipmentEntity, tempEquipmentObjectGroup, tempContainer);
                OnInstantiatedEquipment(tempEquipmentModel, tempEquipmentObject, tempEquipmentEntity, tempEquipmentObjectGroup, tempContainer);
            }
            EquippedModels = storingModels;
        }

        private void ClearEquippedModel(string equipSocket, bool clearLeftHandCache = true, bool clearRightHandCache = true)
        {
            if (EquippedModels.TryGetValue(equipSocket, out EquipmentModel equipmentModel))
            {
                if (GameDataConst.EQUIP_POSITION_RIGHT_HAND.Equals(equipmentModel.equipPosition) && clearRightHandCache)
                    CacheRightHandEquipmentEntity = null;
                if (GameDataConst.EQUIP_POSITION_LEFT_HAND.Equals(equipmentModel.equipPosition) && clearLeftHandCache)
                    CacheLeftHandEquipmentEntity = null;
            }

            if (EquippedModelObjects.TryGetValue(equipSocket, out GameObject equipmentObject))
            {
                Destroy(equipmentObject);
                EquippedModelObjects.Remove(equipSocket);
            }
        }

        public async UniTask SetupEquippingModels(CancellationTokenSource cancellationTokenSource, Dictionary<string, EquipmentModel> showingModels, Dictionary<string, EquipmentModel> storingModels, HashSet<string> unequippingSockets, EquipmentModel[] equipmentModels, string equipPosition, CharacterItem item, bool isSheathModels = false, byte equipWeaponSet = 0, EquipmentModelDelegate onInstantiated = null)
        {
            if (equipmentModels == null || equipmentModels.Length == 0 || string.IsNullOrWhiteSpace(equipPosition))
                return;

            EquipmentModel tempModel;
            for (int i = 0; i < equipmentModels.Length; ++i)
            {
                tempModel = equipmentModels[i];
                GameObject meshPrefab = await tempModel.GetMeshPrefab();
                if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                {
                    // Cancelled
                    return;
                }
                if (string.IsNullOrEmpty(tempModel.equipSocket) || (!tempModel.useInstantiatedObject && meshPrefab == null))
                {
                    // Required data are empty, skip it
                    continue;
                }

                string equipSocket = tempModel.equipSocket;
                if (isSheathModels)
                {
                    // Can instantiates multiple sheath models into 1 socket, so create new collection name
                    equipSocket = ZString.Concat(equipSocket, "_SHEATH_", equipWeaponSet, "_", equipPosition);
                    // Create a new container for this sheath weapons
                    if (!CacheEquipmentModelContainers.ContainsKey(equipSocket) && CacheEquipmentModelContainers.ContainsKey(tempModel.equipSocket))
                        CacheEquipmentModelContainers[equipSocket] = CacheEquipmentModelContainers[tempModel.equipSocket];
                }

                if (!storingModels.TryGetValue(equipSocket, out EquipmentModel storedModel) || storedModel.priority < tempModel.priority || (storedModel.equipPosition == equipPosition && storedModel.item.level < item.level))
                {
                    if (isSheathModels && tempModel.useSpecificSheathEquipWeaponSet && tempModel.specificSheathEquipWeaponSet != equipWeaponSet)
                    {
                        // Don't show it because `specificSheathEquipWeaponSet` is not equals to `equipWeaponSet`
                        continue;
                    }

                    if (EquippedModels.TryGetValue(equipSocket, out EquipmentModel equippedModel)
                        && equippedModel.item.dataId == item.dataId
                        && equippedModel.item.level == item.level
                        && equippedModel.priority == tempModel.priority)
                    {
                        // Same view data, so don't destroy and don't instantiates this model object
                        showingModels.Remove(equipSocket);
                        storingModels[equipSocket] = equippedModel;
                        unequippingSockets.Remove(equipSocket);
                        continue;
                    }

                    EquipmentModel clonedModel = tempModel.Clone();
                    clonedModel.indexOfModel = i;
                    clonedModel.equipPosition = equipPosition;
                    clonedModel.item = item.Clone(false);
                    clonedModel.onInstantiated = onInstantiated;
                    showingModels[equipSocket] = clonedModel;
                    storingModels[equipSocket] = clonedModel;
                    unequippingSockets.Remove(equipSocket);
                }
            }
        }

        protected void CreateCacheEffect(string buffId, List<GameEffect> effects)
        {
            if (effects == null || CacheEffects.ContainsKey(buffId))
                return;
            CacheEffects[buffId] = effects;
        }

        protected void DestroyCacheEffect(string buffId)
        {
            if (!string.IsNullOrEmpty(buffId) && CacheEffects.TryGetValue(buffId, out List<GameEffect> oldEffects) && oldEffects != null)
            {
                foreach (GameEffect effect in oldEffects)
                {
                    if (effect == null) continue;
                    effect.DestroyEffect();
                }
                CacheEffects.Remove(buffId);
            }
        }

        protected void DestroyCacheEffects()
        {
            foreach (string buffId in CacheEffects.Keys)
            {
                DestroyCacheEffect(buffId);
            }
        }

        public virtual void SetBuffs(IList<CharacterBuff> buffs)
        {
            Buffs = buffs;
            // Temp old keys
            _tempCachedKeys.Clear();
            _tempCachedKeys.AddRange(CacheEffects.Keys);
            // Prepare data
            _tempAddingKeys.Clear();
            // Loop new buffs to prepare adding keys
            if (buffs != null && buffs.Count > 0)
            {
                string tempKey;
                foreach (CharacterBuff buff in buffs)
                {
                    // Buff effects
                    tempKey = buff.GetKey();
                    Buff buffData = buff.GetBuff().GetBuff();
                    if (!_tempCachedKeys.Contains(tempKey))
                    {
                        // If old buffs not contains this buff, add this buff effect
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                        InstantiateBuffEffect(tempKey, buffData.Effects);
#endif
#if !DISABLE_ADDRESSABLES
                        InstantiateBuffEffect(tempKey, buffData.AddressableEffects).Forget();
#endif
                        _tempCachedKeys.Add(tempKey);
                    }
                    _tempAddingKeys.Add(tempKey);
                    // Ailment effects
                    switch (buffData.ailment)
                    {
                        case AilmentPresets.Stun:
                            tempKey = nameof(AilmentPresets.Stun);
                            if (!_tempCachedKeys.Contains(tempKey))
                            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                                InstantiateBuffEffect(tempKey, GameInstance.Singleton.StunEffects);
#endif
#if !DISABLE_ADDRESSABLES
                                InstantiateBuffEffect(tempKey, GameInstance.Singleton.AddressableStunEffects).Forget();
#endif
                                _tempCachedKeys.Add(tempKey);
                            }
                            _tempAddingKeys.Add(tempKey);
                            break;
                        case AilmentPresets.Mute:
                            tempKey = nameof(AilmentPresets.Mute);
                            if (!_tempCachedKeys.Contains(tempKey))
                            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                                InstantiateBuffEffect(tempKey, GameInstance.Singleton.MuteEffects);
#endif
#if !DISABLE_ADDRESSABLES
                                InstantiateBuffEffect(tempKey, GameInstance.Singleton.AddressableMuteEffects).Forget();
#endif
                                _tempCachedKeys.Add(tempKey);
                            }
                            _tempAddingKeys.Add(tempKey);
                            break;
                        case AilmentPresets.Freeze:
                            tempKey = nameof(AilmentPresets.Freeze);
                            if (!_tempCachedKeys.Contains(tempKey))
                            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                                InstantiateBuffEffect(tempKey, GameInstance.Singleton.FreezeEffects);
#endif
#if !DISABLE_ADDRESSABLES
                                InstantiateBuffEffect(tempKey, GameInstance.Singleton.AddressableFreezeEffects).Forget();
#endif
                                _tempCachedKeys.Add(tempKey);
                            }
                            _tempAddingKeys.Add(tempKey);
                            break;
                    }
                }
            }
            // Remove effects which removed from new buffs list
            // Loop old keys to destroy removed buffs
            foreach (string key in _tempCachedKeys)
            {
                if (!_tempAddingKeys.Contains(key))
                {
                    // New buffs not contains old buff, remove effect
                    DestroyCacheEffect(key);
                }
            }
        }

        public void InstantiateBuffEffect(string buffId, GameEffect[] buffEffects)
        {
            if (buffEffects == null || buffEffects.Length == 0)
                return;
            CreateCacheEffect(buffId, InstantiateEffect(buffEffects));
        }

#if !DISABLE_ADDRESSABLES
        public async UniTaskVoid InstantiateBuffEffect(string buffId, AssetReferenceGameEffect[] buffEffects)
        {
            if (buffEffects == null || buffEffects.Length == 0)
                return;
            CreateCacheEffect(buffId, await InstantiateEffect(buffEffects));
        }
#endif

        public bool GetRandomRightHandAttackAnimation(
            WeaponType weaponType,
            int randomSeed,
            out int animationIndex,
            out float animSpeedRate,
            out float[] triggerDurations,
            out float totalDuration)
        {
            return GetRandomRightHandAttackAnimation(weaponType.DataId, randomSeed, out animationIndex, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public bool GetRandomLeftHandAttackAnimation(
            WeaponType weaponType,
            int randomSeed,
            out int animationIndex,
            out float animSpeedRate,
            out float[] triggerDurations,
            out float totalDuration)
        {
            return GetRandomLeftHandAttackAnimation(weaponType.DataId, randomSeed, out animationIndex, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public bool GetSkillActivateAnimation(
            BaseSkill skill,
            out float animSpeedRate,
            out float[] triggerDurations,
            out float totalDuration)
        {
            return GetSkillActivateAnimation(skill.DataId, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public bool GetRightHandReloadAnimation(
            WeaponType weaponType,
            out float animSpeedRate,
            out float[] triggerDurations,
            out float totalDuration)
        {
            return GetRightHandReloadAnimation(weaponType.DataId, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public bool GetLeftHandReloadAnimation(
            WeaponType weaponType,
            out float animSpeedRate,
            out float[] triggerDurations,
            out float totalDuration)
        {
            return GetLeftHandReloadAnimation(weaponType.DataId, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public SkillActivateAnimationType UseSkillActivateAnimationType(BaseSkill skill)
        {
            return GetSkillActivateAnimationType(skill.DataId);
        }

        public BaseEquipmentEntity GetRightHandEquipmentEntity()
        {
            return CacheRightHandEquipmentEntity;
        }

        public BaseEquipmentEntity GetLeftHandEquipmentEntity()
        {
            return CacheLeftHandEquipmentEntity;
        }

        public Transform GetRightHandMissileDamageTransform()
        {
            if (Manager != null && Manager.IsFps && BasePlayerCharacterController.Singleton is ShooterPlayerCharacterController shooterController)
                return shooterController.CacheGameplayCameraController.CameraTransform;
            if (CacheRightHandEquipmentEntity != null)
                return CacheRightHandEquipmentEntity.missileDamageTransform;
            return null;
        }

        public Transform GetLeftHandMissileDamageTransform()
        {
            if (Manager != null && Manager.IsFps && BasePlayerCharacterController.Singleton is ShooterPlayerCharacterController shooterController)
                return shooterController.CacheGameplayCameraController.CameraTransform;
            if (CacheLeftHandEquipmentEntity != null)
                return CacheLeftHandEquipmentEntity.missileDamageTransform;
            return null;
        }

        public void PlayEquippedWeaponLaunch(bool isLeftHand)
        {
            if (!isLeftHand && CacheRightHandEquipmentEntity != null)
                CacheRightHandEquipmentEntity.PlayLaunch();
            if (isLeftHand && CacheLeftHandEquipmentEntity != null)
                CacheLeftHandEquipmentEntity.PlayLaunch();
        }

        public void PlayEquippedWeaponReload(bool isLeftHand)
        {
            if (!isLeftHand && CacheRightHandEquipmentEntity != null)
                CacheRightHandEquipmentEntity.PlayReload();
            if (isLeftHand && CacheLeftHandEquipmentEntity != null)
                CacheLeftHandEquipmentEntity.PlayReload();
        }

        public void PlayEquippedWeaponReloaded(bool isLeftHand)
        {
            if (!isLeftHand && CacheRightHandEquipmentEntity != null)
                CacheRightHandEquipmentEntity.PlayReloaded();
            if (isLeftHand && CacheLeftHandEquipmentEntity != null)
                CacheLeftHandEquipmentEntity.PlayReloaded();
        }

        public void PlayEquippedWeaponCharge(bool isLeftHand)
        {
            if (!isLeftHand && CacheRightHandEquipmentEntity != null)
                CacheRightHandEquipmentEntity.PlayCharge();
            if (isLeftHand && CacheLeftHandEquipmentEntity != null)
                CacheLeftHandEquipmentEntity.PlayCharge();
        }

        public virtual void OnInstantiatedEquipment(EquipmentModel model, GameObject instantiatedObject, BaseEquipmentEntity instantiatedEntity, EquipmentInstantiatedObjectGroup instantiatedObjectGroup, EquipmentContainer equipmentContainer)
        {
            onInstantiatedEquipment?.Invoke(model, instantiatedObject, instantiatedEntity, instantiatedObjectGroup, equipmentContainer);
            if (model.useInstantiatedObject || model.doNotSetupBones)
                return;
            BaseEquipmentModelBonesSetupManager equipmentModelBonesSetupManager = GameInstance.Singleton.EquipmentModelBonesSetupManager;
            if (model.equipmentModelBonesSetupManager != null)
                equipmentModelBonesSetupManager = model.equipmentModelBonesSetupManager;
            equipmentModelBonesSetupManager.Setup(this, model, instantiatedObject, instantiatedEntity, instantiatedObjectGroup, equipmentContainer);
        }

        public void SetIsDead(bool isDead)
        {
            IsDead = isDead;
        }

        public void SetMoveAnimationSpeedMultiplier(float moveAnimationSpeedMultiplier)
        {
            MoveAnimationSpeedMultiplier = moveAnimationSpeedMultiplier;
        }

        public void SetMovementState(MovementState movementState, ExtraMovementState extraMovementState, Vector2 direction2D, bool isFreezeAnimation)
        {
            if (!Application.isPlaying)
                return;
            MovementState = movementState;
            ExtraMovementState = extraMovementState;
            Direction2D = direction2D;
            IsFreezeAnimation = isFreezeAnimation;
            PlayMoveAnimation();
        }

        public virtual void SetDefaultAnimations()
        {
            SetIsDead(false);
            SetMoveAnimationSpeedMultiplier(1f);
            SetMovementState(MovementState.IsGrounded, ExtraMovementState.None, Vector2.down, false);
        }

        public virtual void PlayHitAnimation() { }

        public virtual float GetJumpAnimationDuration()
        {
            return 0f;
        }

        public virtual void PlayJumpAnimation() { }

        public virtual void PlayPickupAnimation() { }

        public abstract void PlayMoveAnimation();
        public abstract void PlayActionAnimation(AnimActionType animActionType, int dataId, int index, out bool skipMovementValidation, out bool shouldUseRootMotion, float playSpeedMultiplier, float changeClipLength, float overrideClipLength);
        public abstract void PlaySkillCastClip(int dataId, float duration, out bool skipMovementValidation, out bool shouldUseRootMotion);
        public abstract void PlayWeaponChargeClip(int dataId, bool isLeftHand, out bool skipMovementValidation, out bool shouldUseRootMotion);
        public abstract void StopActionAnimation();
        public abstract void StopSkillCastAnimation();
        public abstract void StopWeaponChargeAnimation();
        public abstract int GetRightHandAttackRandomMax(int dataId);
        public abstract int GetLeftHandAttackRandomMax(int dataId);
        /// <summary>
        /// Get random right-hand attack animation, if `triggerDurations`'s length is 0/`totalDuration` <= 0, it will wait other methods to use as `triggerDurations`/`totalDuration` (such as animtion clip event, state machine behaviour).
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="randomSeed"></param>
        /// <param name="animationIndex"></param>
        /// <param name="animSpeedRate"></param>
        /// <param name="triggerDurations"></param>
        /// <param name="totalDuration"></param>
        /// <returns></returns>
        public abstract bool GetRandomRightHandAttackAnimation(int dataId, int randomSeed, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration);
        /// <summary>
        /// Get random left-hand attack animation, if `triggerDurations`'s length is 0/`totalDuration` <= 0, it will wait other methods to use as `triggerDurations`/`totalDuration` (such as animtion clip event, state machine behaviour).
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="randomSeed"></param>
        /// <param name="animationIndex"></param>
        /// <param name="animSpeedRate"></param>
        /// <param name="triggerDurations"></param>
        /// <param name="totalDuration"></param>
        /// 
        /// <returns></returns>
        public abstract bool GetRandomLeftHandAttackAnimation(int dataId, int randomSeed, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration);
        /// <summary>
        /// Get right-hand attack animation, if `triggerDurations`'s length is 0/`totalDuration` <= 0, it will wait other methods to use as `triggerDurations`/`totalDuration` (such as animtion clip event, state machine behaviour).
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="animationIndex"></param>
        /// <param name="animSpeedRate"></param>
        /// <param name="triggerDurations"></param>
        /// <param name="totalDuration"></param>
        /// 
        /// <returns></returns>
        public abstract bool GetRightHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration);
        /// <summary>
        /// Get left-hand attack animation, if `triggerDurations`'s length is 0/`totalDuration` <= 0, it will wait other methods to use as `triggerDurations`/`totalDuration` (such as animtion clip event, state machine behaviour).
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="animationIndex"></param>
        /// <param name="animSpeedRate"></param>
        /// <param name="triggerDurations"></param>
        /// <param name="totalDuration"></param>
        /// 
        /// <returns></returns>
        public abstract bool GetLeftHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration);
        /// <summary>
        /// Get skill activate animation, if `triggerDurations`'s length is 0/`totalDuration` <= 0, it will wait other methods to use as `triggerDurations`/`totalDuration` (such as animtion clip event, state machine behaviour).
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="animSpeedRate"></param>
        /// <param name="triggerDurations"></param>
        /// <param name="totalDuration"></param>
        /// 
        /// <returns></returns>
        public abstract bool GetSkillActivateAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration);
        /// <summary>
        /// Get right-hand reload animation, if `triggerDurations`'s length is 0/`totalDuration` <= 0, it will wait other methods to use as `triggerDurations`/`totalDuration` (such as animtion clip event, state machine behaviour).
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="animSpeedRate"></param>
        /// <param name="triggerDurations"></param>
        /// <param name="totalDuration"></param>
        /// 
        /// <returns></returns>
        public abstract bool GetRightHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration);
        /// <summary>
        /// Get left-hand reload animation, if `triggerDurations`'s length is 0/`totalDuration` <= 0, it will wait other methods to use as `triggerDurations`/`totalDuration` (such as animtion clip event, state machine behaviour).
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="animSpeedRate"></param>
        /// <param name="triggerDurations"></param>
        /// <param name="totalDuration"></param>
        /// 
        /// <returns></returns>
        public abstract bool GetLeftHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration);
        public abstract SkillActivateAnimationType GetSkillActivateAnimationType(int dataId);

        public static float GetAnimationSpeedRate(float clipLength, float playSpeedMultiplier, float changeClipLength)
        {
            if (-changeClipLength > clipLength)
                changeClipLength = -clipLength * 0.999f;
            float animLengthWithChange = clipLength + changeClipLength;
            float animLengthDiffToRate = animLengthWithChange / clipLength;
            return playSpeedMultiplier * animLengthDiffToRate;
        }

        public static float GetAnimationDuration(float clipLength, float playSpeedMultiplier, float changeClipLength)
        {
            if (clipLength <= 0f)
                return 0f;
            return clipLength / GetAnimationSpeedRate(clipLength, playSpeedMultiplier, changeClipLength);
        }
    }
}
