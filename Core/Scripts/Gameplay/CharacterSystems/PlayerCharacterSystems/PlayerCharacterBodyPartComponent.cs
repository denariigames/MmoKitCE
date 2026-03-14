using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public class PlayerCharacterBodyPartComponent : BaseGameEntityComponent<BasePlayerCharacterEntity>
    {
        [System.Serializable]
        public class MaterialPropertiesSetting
        {
            [Header("Texture")]
            public bool applyMaterialTexture = false;
            [Tooltip("Property name for main texture, usually `_BaseMap` for HDRP/URP, `_MainTex` for BRP")]
            public string materialTextureProperty = "_BaseMap";
            public Texture materialTexture = null;

            [Header("Color")]
            public bool applyMaterialColor = false;
            [Tooltip("Property name for main texture's color, usually `_BaseColor` for HDRP/URP, `_Color` for BRP")]
            public string materialColorProperty = "_BaseColor";
            public Color materialColor = Color.white;
            public bool useUpperLevelMaterialColorSetting;
        }

        [System.Serializable]
        public class MaterialGroup
        {
#if UNITY_EDITOR
            [Tooltip("Set any name for clarity; it doesn’t have to be unique and isn’t used anywhere.")]
            public string name;
#endif

            [Tooltip("Material Settings for each mesh's materials, its index is index of `MeshRenderer` -> `materials`")]
            public Material[] materials = new Material[0];
            public MaterialPropertiesSetting[] properties = new MaterialPropertiesSetting[0];
        }

        [System.Serializable]
        public class ModelColorSetting
        {
#if UNITY_EDITOR
            [Tooltip("Set any name for clarity; it doesn’t have to be unique and isn’t used anywhere.")]
            public string name;
#endif
            public bool useUpperLevelMaterialColorSetting;
            public Color materialColor = Color.white;

            [Header("Setting for model's single instantiated object setting")]
            [Tooltip("Material Settings for each mesh's materials, its index is index of `MeshRenderer` -> `materials`")]
            public Material[] materials = new Material[0];
            public MaterialPropertiesSetting[] properties = new MaterialPropertiesSetting[0];

            [Header("Setting for model's multiple instantiated object setting")]
            public MaterialGroup[] materialGroups = new MaterialGroup[0];
        }

        [System.Serializable]
        public class ColorOption
        {
            [Header("Settings for UIs")]
            public string defaultTitle = string.Empty;
            public LanguageData[] languageSpecificTitles = new LanguageData[0];
            [PreviewSprite(50)]
            public Sprite icon;
            public Color iconColor = Color.white;

            [Header("Settings for In-Game Appearances")]
            public Color materialColor = Color.white;
            [Tooltip("Color settings for each model, its index is index of `models`")]
            [FormerlySerializedAs("ModelColorSettings")]
            public ModelColorSetting[] modelColorSettings = new ModelColorSetting[0];

            public string Title
            {
                get { return Language.GetText(languageSpecificTitles, defaultTitle); }
            }
        }

        [System.Serializable]
        public class ModelOption
        {
            [Header("Settings for UIs")]
            public string defaultTitle = string.Empty;
            public LanguageData[] languageSpecificTitles = new LanguageData[0];
            [PreviewSprite(50)]
            public Sprite icon;

            [Header("Settings for In-Game Appearances")]
            public EquipmentModel[] models = new EquipmentModel[0];
            public ColorOption[] colors = new ColorOption[0];

            public string Title
            {
                get { return Language.GetText(languageSpecificTitles, defaultTitle); }
            }
        }

        public string modelSettingId;
        public string colorSettingId;
        public List<ModelOption> options = new List<ModelOption>();
        public List<PlayerCharacterBodyPartComponentOption> optionObjects = new List<PlayerCharacterBodyPartComponentOption>();
        protected int _currentModelIndex;
        protected int _currentColorIndex;
        public IEnumerable<ModelOption> ModelOptions { get => options; }
        public int MaxModelOptions { get => options.Count; }
        public IEnumerable<ColorOption> ColorOptions { get => options[_currentModelIndex].colors; }
        public int MaxColorOptions { get => options[_currentModelIndex].colors.Length; }

        private BaseCharacterModel[] _models;
        private bool _isInitialized = false;

        private void Awake()
        {
            if (optionObjects.Count > 0)
            {
                foreach (PlayerCharacterBodyPartComponentOption optionObject in optionObjects)
                {
                    options.Add(optionObject.data);
                }
            }
        }

        private void OnEnable()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _models = GetComponentsInChildren<BaseCharacterModel>(true);
                SetupEvents();
                ApplyModelAndColorBySavedData();
            }
        }

        protected override void OnDestroy()
        {
            ClearEvents();
            if (_models != null && _models.Length > 0)
            {
                for (int i = 0; i < _models.Length; ++i)
                {
                    _models[i] = null;
                }
            }
            base.OnDestroy();
        }

        public void SetupEvents()
        {
            ClearEvents();
            for (int i = 0; i < _models.Length; ++i)
            {
                SetupCharacterModelEvents(_models[i]);
            }
#if !DISABLE_CUSTOM_CHARACTER_DATA
            Entity.onPublicIntsOperation -= OnPublicIntsOperation;
            Entity.onPublicIntsOperation += OnPublicIntsOperation;
#endif
        }

        public void ClearEvents()
        {
            if (_models != null)
            {
                for (int i = 0; i < _models.Length; ++i)
                {
                    ClearCharacterModelEvents(_models[i]);
                }
            }
#if !DISABLE_CUSTOM_CHARACTER_DATA
            Entity.onPublicIntsOperation -= OnPublicIntsOperation;
#endif
        }

        public void SetupCharacterModelEvents(BaseCharacterModel model)
        {
            ClearCharacterModelEvents(model);
            model.onBeforeUpdateEquipmentModels += OnBeforeUpdateEquipmentModels;
        }

        public void ClearCharacterModelEvents(BaseCharacterModel model)
        {
            model.onBeforeUpdateEquipmentModels -= OnBeforeUpdateEquipmentModels;
        }

        public void ApplyModelAndColorBySavedData()
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            ApplyModelAndColorBySavedData(Entity.PublicInts);
#endif
        }

        public void ApplyModelAndColorBySavedData(IList<CharacterDataInt32> publicInts)
        {
            _currentModelIndex = 0;
            _currentColorIndex = 0;
            byte foundCount = 0;
            int hashedModelSettingId = GetHashedModelSettingId();
            int hashedColorSettingId = GetHashedColorSettingId();
            for (int i = 0; i < publicInts.Count; ++i)
            {
                if (publicInts[i].hashedKey == hashedModelSettingId)
                {
                    _currentModelIndex = publicInts[i].value;
                    foundCount++;
                }
                if (publicInts[i].hashedKey == hashedColorSettingId)
                {
                    _currentColorIndex = publicInts[i].value;
                    foundCount++;
                }
                if (foundCount == 2)
                    break;
            }
            ApplyModelAndColorBySavedData(_currentModelIndex, _currentColorIndex);
        }

        public void ApplyModelAndColorBySavedData(int modelIndex, int colorIndex)
        {
            _currentModelIndex = modelIndex;
            _currentColorIndex = colorIndex;
            if (_currentModelIndex >= MaxModelOptions)
                _currentModelIndex = 0;
            if (_currentColorIndex >= MaxColorOptions)
                _currentColorIndex = 0;
            // Update model later
            Entity.MarkToUpdateAppearances();
        }

        /// <summary>
        /// This function should be called by server or being called in character creation only, it is not allow client to set custom data.
        /// </summary>
        /// <param name="index"></param>
        public void SetModel(int index)
        {
            if (index < 0 || index >= MaxModelOptions)
                return;
            _currentModelIndex = index;
            _currentColorIndex = 0;
            // Save to entity's `PublicInts`
            Entity.SetPublicInt32(GetHashedModelSettingId(), _currentModelIndex);
            Entity.SetPublicInt32(GetHashedColorSettingId(), _currentColorIndex);
            // Update model later
            Entity.MarkToUpdateAppearances();
        }

        public int GetModel()
        {
            return _currentModelIndex;
        }

        /// <summary>
        /// This function should be called by server or being called in character creation only, it is not allow client to set custom data.
        /// </summary>
        /// <param name="index"></param>
        public void SetColor(int index)
        {
            if (index < 0 || index >= MaxColorOptions)
                return;
            _currentColorIndex = index;
            // Save to entity's `PublicInts`
            Entity.SetPublicInt32(GetHashedModelSettingId(), _currentModelIndex);
            Entity.SetPublicInt32(GetHashedColorSettingId(), _currentColorIndex);
            // Update model later
            Entity.MarkToUpdateAppearances();
        }

        public int GetColor()
        {
            return _currentColorIndex;
        }

        private void OnBeforeUpdateEquipmentModels(
            CancellationTokenSource cancellationTokenSource,
            BaseCharacterModel characterModel,
            Dictionary<string, EquipmentModel> showingModels,
            Dictionary<string, EquipmentModel> storingModels,
            HashSet<string> unequippingSockets)
        {
            characterModel.SetupEquippingModels(cancellationTokenSource, showingModels, storingModels, unequippingSockets, options[_currentModelIndex].models, CreateFakeEquipPosition(), CreateFakeCharacterItem(), false, 0, OnShowEquipmentModel).Forget();
        }

        private void OnPublicIntsOperation(LiteNetLibSyncListOp operation, int index, CharacterDataInt32 oldItem, CharacterDataInt32 newItem)
        {
            switch(operation)
            {
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    if (oldItem.hashedKey != newItem.hashedKey || oldItem.value != newItem.value)
                        ApplyModelAndColorBySavedData();
                    break;
                default:
                    ApplyModelAndColorBySavedData();
                    break;
            }
        }

        protected virtual void OnShowEquipmentModel(EquipmentModel model, GameObject modelObject, BaseEquipmentEntity equipmentEntity, EquipmentInstantiatedObjectGroup instantiatedObjectGroup, EquipmentContainer equipmentContainer)
        {
            // Get mesh's material to change color
            if (model == null)
                return;

            if (model.indexOfModel < 0)
            {
                Debug.LogError("Invalid index of model", this);
                return;
            }

            if (options.Count <= 0)
            {
                // No model options to select
                return;
            }

            if (_currentModelIndex >= options.Count)
            {
                Debug.LogError("Invalid index of model option", this);
                return;
            }

            ModelOption option = options[_currentModelIndex];
            if (option.colors.Length <= 0)
            {
                // No color options to select
                return;
            }

            if (_currentColorIndex >= option.colors.Length)
            {
                Debug.LogError("Invalid index of color option", this);
                return;
            }

            ColorOption colorOption = option.colors[_currentColorIndex];
            if (colorOption.modelColorSettings.Length <= 0)
            {
                // No model color setup
                return;
            }

            int indexOfModelColorSetting = model.indexOfModel;
            if (indexOfModelColorSetting >= colorOption.modelColorSettings.Length)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError("Invalid index of model color setting, will use the latest one. This message won't be written in build to improve performance", this);
#endif
                indexOfModelColorSetting = colorOption.modelColorSettings.Length - 1;
            }

            if (indexOfModelColorSetting < 0)
            {
                Debug.LogError("Ivalid index of model color setting");
                return;
            }

            ModelColorSetting modelColorSetting = colorOption.modelColorSettings[indexOfModelColorSetting];

            if (modelObject != null && modelColorSetting.materials.Length > 0)
            {
                Color materialColor = modelColorSetting.useUpperLevelMaterialColorSetting ? colorOption.materialColor : modelColorSetting.materialColor;
                SetMaterial(modelObject, materialColor, modelColorSetting.materials, modelColorSetting.properties);
            }

            if (instantiatedObjectGroup != null && modelColorSetting.materialGroups.Length > 0)
            {
                for (int i = 0; i < instantiatedObjectGroup.instantiatedObjects.Length; ++i)
                {
                    if (i >= modelColorSetting.materialGroups.Length)
                        break;
                    Color materialColor = modelColorSetting.useUpperLevelMaterialColorSetting ? colorOption.materialColor : modelColorSetting.materialColor;
                    SetMaterial(instantiatedObjectGroup.instantiatedObjects[i], materialColor, modelColorSetting.materialGroups[i].materials, modelColorSetting.materialGroups[i].properties);
                }
            }
        }

        private void SetMaterial(GameObject modelObject, Color upperLevelColorSetting, Material[] materials, MaterialPropertiesSetting[] properties)
        {
            Renderer renderer = modelObject.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.materials = materials;
                for (int i = 0; i < renderer.materials.Length; ++i)
                {
                    if (i >= properties.Length)
                        break;
                    MaterialPropertiesSetting property = properties[i];
                    if (property.applyMaterialTexture)
                        renderer.materials[i].SetTexture(property.materialTextureProperty, property.materialTexture);
                    if (property.applyMaterialColor)
                        renderer.materials[i].SetColor(property.materialColorProperty, property.useUpperLevelMaterialColorSetting ? upperLevelColorSetting : property.materialColor);
                }
            }
        }

        public int CreateFakeItemDataId()
        {
            return string.Concat("_BODY_PART_", modelSettingId, "_", _currentModelIndex, "_", _currentColorIndex).GenerateHashId();
        }

        public string CreateFakeEquipPosition()
        {
            return string.Concat("_BODY_PART_", modelSettingId);
        }

        public CharacterItem CreateFakeCharacterItem()
        {
            return new CharacterItem()
            {
                dataId = CreateFakeItemDataId(),
                level = 1,
            };
        }

        public int GetHashedModelSettingId()
        {
            return modelSettingId.GenerateHashId();
        }

        public int GetHashedColorSettingId()
        {
            return colorSettingId.GenerateHashId();
        }
    }
}
