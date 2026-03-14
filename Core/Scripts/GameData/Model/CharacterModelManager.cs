using Insthync.AddressableAssetTools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DefaultExecutionOrders.CHARACTER_MODEL_MANAGER)]
    public partial class CharacterModelManager : BaseGameEntityComponent<BaseGameEntity>
    {
        public const byte HIDE_SETTER_ENTITY = 0;
        public const byte HIDE_SETTER_CONTROLLER = 1;

        [Header("TPS Model Settings")]
        [SerializeField]
        [FormerlySerializedAs("mainModel")]
        private BaseCharacterModel mainTpsModel = null;
        public BaseCharacterModel MainTpsModel { get { return mainTpsModel; } set { mainTpsModel = value; } }

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Header("FPS Model Settings")]
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableFpsModelPrefab))]
#endif
        protected BaseCharacterModel fpsModelPrefab;
#endif
        public BaseCharacterModel FpsModelPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData) && metaData.OverrideFpsModel)
                    return metaData.FpsModelPrefab;
                return fpsModelPrefab;
#else
                return null;
#endif
            }
            set
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                fpsModelPrefab = value;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceBaseCharacterModel addressableFpsModelPrefab;
        public AssetReferenceBaseCharacterModel AddressableFpsModelPrefab
        {
            get
            {
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData) && metaData.OverrideFpsModel)
                    return metaData.AddressableFpsModelPrefab;
                return addressableFpsModelPrefab;
            }
            set { addressableFpsModelPrefab = value; }
        }
#endif

        [SerializeField]
        [FormerlySerializedAs("fpsModelOffsets")]
        [Tooltip("Position offsets from fps model container (Camera's transform)")]
        private Vector3 fpsModelPositionOffsets = Vector3.zero;
        public Vector3 FpsModelPositionOffsets
        {
            get
            {
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData) && metaData.OverrideFpsModel)
                    return metaData.FpsModelPositionOffsets;
                return fpsModelPositionOffsets;
            }
            set { fpsModelPositionOffsets = value; }
        }

        [SerializeField]
        [Tooltip("Rotation offsets from fps model container (Camera's transform)")]
        private Vector3 fpsModelRotationOffsets = Vector3.zero;
        public Vector3 FpsModelRotationOffsets
        {
            get
            {
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData) && metaData.OverrideFpsModel)
                    return metaData.FpsModelRotationOffsets;
                return fpsModelRotationOffsets;
            }
            set { fpsModelRotationOffsets = value; }
        }

        public BaseCharacterModel ActiveTpsModel { get; private set; }
        public BaseCharacterModel ActiveFpsModel { get; private set; }
        public BaseCharacterModel MainFpsModel { get; set; }

        [HideInInspector]
        [SerializeField]
        // Vehicle models setup will be moved to `BaseCharacterModel`
        private VehicleCharacterModel[] vehicleModels = new VehicleCharacterModel[0];

        public bool IsHide
        {
            get
            {
                foreach (bool hideState in _hideStates.Values)
                {
                    if (hideState)
                        return true;
                }
                return false;
            }
        }
        public bool IsFps { get; private set; }

        /// <summary>
        /// Dictionary of hide setter (who is ordering to hide) and hidding state value
        /// </summary>
        private readonly Dictionary<byte, bool> _hideStates = new Dictionary<byte, bool>();
        private int _dirtyVehicleDataId;
        private byte _dirtySeatIndex;
        private byte _modelIdCounter = 0;

        private void Awake()
        {
            ValidateMainTpsModel();
            MigrateVehicleModels();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            mainTpsModel = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            fpsModelPrefab = null;
#endif
#if !DISABLE_ADDRESSABLES
            addressableFpsModelPrefab = null;
#endif
            ActiveTpsModel = null;
            ActiveFpsModel = null;
            MainFpsModel = null;
        }

        public bool TryGetMetaData(out PlayerCharacterEntityMetaData metaData)
        {
            metaData = null;
            if (Entity is BasePlayerCharacterEntity playerCharacterEntity)
                return playerCharacterEntity.TryGetMetaData(out metaData);
            return false;
        }

        internal byte InitTpsModel(BaseCharacterModel model)
        {
            model.Entity = Entity;
            model.MainModel = MainTpsModel;
            model.IsTpsModel = true;
            model.IsFpsModel = false;
            if (model == MainTpsModel)
            {
                MainTpsModel.InitCacheData();
                SwitchTpsModel(MainTpsModel);
            }
            return _modelIdCounter++;
        }

        internal void InitFpsModel(BaseCharacterModel model)
        {
            model.Manager = this;
            model.Entity = Entity;
            model.MainModel = MainFpsModel;
            model.IsTpsModel = false;
            model.IsFpsModel = true;
            if (model == MainFpsModel)
            {
                MainFpsModel.InitCacheData();
                SwitchFpsModel(MainFpsModel);
            }
        }

        public async UniTask<BaseCharacterModel> InstantiateFpsModel(Transform container)
        {
            BaseCharacterModel loadedPrefab;
#if !DISABLE_ADDRESSABLES
            loadedPrefab = await AddressableFpsModelPrefab.GetOrLoadAssetAsyncOrUsePrefab(FpsModelPrefab);
#else
            await UniTask.Yield();
            loadedPrefab = FpsModelPrefab;
#endif
            if (loadedPrefab == null)
                return null;
            MainFpsModel = Instantiate(loadedPrefab, container);
            MainFpsModel.transform.localPosition = FpsModelPositionOffsets;
            MainFpsModel.transform.localEulerAngles = FpsModelRotationOffsets;
            InitFpsModel(MainFpsModel);
            MainFpsModel.SetEquipItems(MainTpsModel.EquipItems, MainTpsModel.SelectableWeaponSets, MainTpsModel.EquipWeaponSet, MainTpsModel.IsWeaponsSheathed);
#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
            IComponentWithPrefabRef[] refs = MainFpsModel.GetComponents<IComponentWithPrefabRef>();
            for (int i = 0; i < refs.Length; ++i)
            {
                GameObject editorAsset = null;
                if (AddressableFpsModelPrefab.IsDataValid() && AddressableFpsModelPrefab.editorAsset is GameObject addressableEditorAsset)
                    editorAsset = addressableEditorAsset;
                if (editorAsset == null && FpsModelPrefab != null)
                    editorAsset = FpsModelPrefab.gameObject;
                refs[i].SetupRefToPrefab(editorAsset);
            }
#endif
            return MainFpsModel;
        }

        public bool ValidateMainTpsModel()
        {
            if (MainTpsModel == null)
            {
                MainTpsModel = GetComponent<BaseCharacterModel>();
                return true;
            }
            return false;
        }

        private bool MigrateVehicleModels()
        {
            if (MainTpsModel != null && vehicleModels != null && vehicleModels.Length > 0)
            {
                MainTpsModel.VehicleModels = vehicleModels;
                vehicleModels = new VehicleCharacterModel[0];
                return true;
            }
            return false;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            bool hasChanges = false;
            if (ValidateMainTpsModel())
                hasChanges = true;
            if (MigrateVehicleModels())
                hasChanges = true;
            if (hasChanges)
                EditorUtility.SetDirty(this);
#endif
        }

        public void UpdatePassengingVehicle(VehicleType vehicleType, byte seatIndex)
        {
            if (vehicleType != null)
            {
                if (_dirtyVehicleDataId != vehicleType.DataId ||
                    _dirtySeatIndex != seatIndex)
                {
                    _dirtyVehicleDataId = vehicleType.DataId;
                    _dirtySeatIndex = seatIndex;
                    VehicleCharacterModel tempData;
                    // Switch TPS model
                    if (MainTpsModel != null)
                    {
                        if (MainTpsModel.CacheVehicleModels.TryGetValue(_dirtyVehicleDataId, out tempData) &&
                            seatIndex < tempData.modelsForEachSeats.Length)
                            SwitchTpsModel(tempData.modelsForEachSeats[seatIndex]);
                        else
                            SwitchTpsModel(MainTpsModel);
                    }
                    // Switch FPS Model
                    if (MainFpsModel != null)
                    {
                        if (MainFpsModel.CacheVehicleModels.TryGetValue(_dirtyVehicleDataId, out tempData) &&
                            seatIndex < tempData.modelsForEachSeats.Length)
                            SwitchFpsModel(tempData.modelsForEachSeats[seatIndex]);
                        else
                            SwitchFpsModel(MainFpsModel);
                    }
                }
                return;
            }

            if (_dirtyVehicleDataId != 0)
            {
                _dirtyVehicleDataId = 0;
                _dirtySeatIndex = 0;
                if (MainTpsModel != null)
                    SwitchTpsModel(MainTpsModel);
                if (MainFpsModel != null)
                    SwitchFpsModel(MainFpsModel);
            }
        }

        private void SwitchTpsModel(BaseCharacterModel nextModel)
        {
            if (ActiveTpsModel != null && nextModel == ActiveTpsModel) return;
            BaseCharacterModel previousModel = ActiveTpsModel;
            ActiveTpsModel = nextModel;
            ActiveTpsModel.SwitchModel(previousModel);
        }

        private void SwitchFpsModel(BaseCharacterModel nextModel)
        {
            if (ActiveFpsModel != null && nextModel == ActiveFpsModel) return;
            BaseCharacterModel previousModel = ActiveFpsModel;
            ActiveFpsModel = nextModel;
            ActiveFpsModel.SwitchModel(previousModel);
        }

        public void SetIsHide(byte setter, bool isHide)
        {
            _hideStates[setter] = isHide;
            UpdateVisibleState();
        }

        public void SetIsFps(bool isFps)
        {
            if (IsFps == isFps)
                return;
            IsFps = isFps;
            UpdateVisibleState();
        }

        public void UpdateVisibleState()
        {
            GameEntityModel.EVisibleState tpsModelVisibleState = GameEntityModel.EVisibleState.Visible;
            GameEntityModel.EVisibleState fpsModelVisibleState = GameEntityModel.EVisibleState.Invisible;
            if (IsFps)
            {
                tpsModelVisibleState = GameEntityModel.EVisibleState.Fps;
                fpsModelVisibleState = GameEntityModel.EVisibleState.Visible;
            }
            if (IsHide)
            {
                tpsModelVisibleState = GameEntityModel.EVisibleState.Invisible;
            }
            // Set visible state to main model
            MainTpsModel.SetVisibleState(tpsModelVisibleState);
            // FPS model will be hidden when it's not FPS mode
            if (MainFpsModel != null)
            {
                MainFpsModel.SetVisibleState(fpsModelVisibleState);
            }
        }
    }
}
