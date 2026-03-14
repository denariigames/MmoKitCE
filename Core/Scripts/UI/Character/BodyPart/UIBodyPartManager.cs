using UnityEngine;

namespace MultiplayerARPG
{
    public class UIBodyPartManager : MonoBehaviour
    {
        public delegate void SetModelValueDelegate(int hashedSettingId, int value);
        public delegate void SetColorValueDelegate(int hashedSettingId, int value);
        public string modelSettingId;

        [Header("Model Option Settings")]
        public GameObject uiModelRoot;
        public UIBodyPartModelOption uiSelectedModel;
        public UIBodyPartModelOption uiModelPrefab;
        public Transform uiModelContainer;

        [Header("Color Option Settings")]
        public GameObject uiColorRoot;
        public UIBodyPartColorOption uiSelectedColor;
        public UIBodyPartColorOption uiColorPrefab;
        public Transform uiColorContainer;

        public SetModelValueDelegate onSetModelValue;
        public SetColorValueDelegate onSetColorValue;

        private UIList _modelList;
        public UIList ModelList
        {
            get
            {
                if (_modelList == null)
                {
                    _modelList = gameObject.AddComponent<UIList>();
                    _modelList.uiPrefab = uiModelPrefab.gameObject;
                    _modelList.uiContainer = uiModelContainer;
                }
                return _modelList;
            }
        }

        private UIBodyPartModelListSelectionManager _modelSelectionManager;
        public UIBodyPartModelListSelectionManager ModelSelectionManager
        {
            get
            {
                if (_modelSelectionManager == null)
                    _modelSelectionManager = gameObject.GetOrAddComponent<UIBodyPartModelListSelectionManager>();
                _modelSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _modelSelectionManager;
            }
        }

        private UIList _colorList;
        public UIList ColorList
        {
            get
            {
                if (_colorList == null)
                {
                    _colorList = gameObject.AddComponent<UIList>();
                    _colorList.uiPrefab = uiColorPrefab.gameObject;
                    _colorList.uiContainer = uiColorContainer;
                }
                return _colorList;
            }
        }


        private UIBodyPartColorListSelectionManager _colorSelectionManager;
        public UIBodyPartColorListSelectionManager ColorSelectionManager
        {
            get
            {
                if (_colorSelectionManager == null)
                    _colorSelectionManager = gameObject.GetOrAddComponent<UIBodyPartColorListSelectionManager>();
                _colorSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _colorSelectionManager;
            }
        }

        private UISelectionManagerShowOnSelectEventManager<PlayerCharacterBodyPartComponent.ModelOption, UIBodyPartModelOption> _modelUIEventSetupManager = new UISelectionManagerShowOnSelectEventManager<PlayerCharacterBodyPartComponent.ModelOption, UIBodyPartModelOption>();
        private UISelectionManagerShowOnSelectEventManager<PlayerCharacterBodyPartComponent.ColorOption, UIBodyPartColorOption> _colorUIEventSetupManager = new UISelectionManagerShowOnSelectEventManager<PlayerCharacterBodyPartComponent.ColorOption, UIBodyPartColorOption>();
        private BaseCharacterModel _model;
        private PlayerCharacterBodyPartComponent _component;

        private void OnEnable()
        {
            _modelUIEventSetupManager.OnEnable(ModelSelectionManager, uiSelectedModel);
            _colorUIEventSetupManager.OnEnable(ColorSelectionManager, uiSelectedColor);
            ModelSelectionManager.eventOnSelect.RemoveListener(OnSelectModelUI);
            ColorSelectionManager.eventOnSelect.RemoveListener(OnSelectColorUI);
            ModelSelectionManager.eventOnSelect.AddListener(OnSelectModelUI);
            ColorSelectionManager.eventOnSelect.AddListener(OnSelectColorUI);
        }

        private void OnDisable()
        {
            _modelUIEventSetupManager.OnDisable();
            _colorUIEventSetupManager.OnDisable();
            ModelSelectionManager.eventOnSelect.RemoveListener(OnSelectModelUI);
            ColorSelectionManager.eventOnSelect.RemoveListener(OnSelectColorUI);
        }

        private async void OnSelectModelUI(UIBodyPartModelOption ui)
        {
            if (_component == null)
                return;
            _component.SetModel(ui.Index);
            await _model.UpdateEquipmentModels(_model.EquipItems, _model.SelectableWeaponSets, _model.EquipWeaponSet, _model.IsWeaponsSheathed);
            SetupColorList();
            onSetModelValue?.Invoke(ui.HashedSettingId, ui.Index);
        }

        private async void OnSelectColorUI(UIBodyPartColorOption ui)
        {
            if (_component == null)
                return;
            _component.SetColor(ui.Index);
            await _model.UpdateEquipmentModels(_model.EquipItems, _model.SelectableWeaponSets, _model.EquipWeaponSet, _model.IsWeaponsSheathed);
            onSetColorValue?.Invoke(ui.HashedSettingId, ui.Index);
        }

        public void SetCharacterModel(BaseCharacterModel model)
        {
            _model = model;
            _component = null;
            PlayerCharacterBodyPartComponent[] comps = _model.transform.root.GetComponentsInChildren<PlayerCharacterBodyPartComponent>();
            for (int i = 0; i < comps.Length; ++i)
            {
                if (!modelSettingId.Equals(comps[i].modelSettingId))
                    continue;
                comps[i].SetupCharacterModelEvents(model);
                _component = comps[i];
                break;
            }
            SetupModelList();
        }

        private void SetupModelList()
        {
            if (_component == null || _component.MaxModelOptions <= 0)
            {
                if (uiModelRoot != null)
                    uiModelRoot.SetActive(false);
                if (uiColorRoot != null)
                    uiColorRoot.SetActive(false);
                return;
            }

            if (uiModelRoot != null)
                uiModelRoot.SetActive(true);

            // Setup model list
            ModelSelectionManager.DeselectSelectedUI();
            ModelSelectionManager.Clear();
            ModelList.HideAll();
            ModelList.Generate(_component.ModelOptions, (index, data, ui) =>
            {
                UIBodyPartModelOption uiComp = ui.GetComponent<UIBodyPartModelOption>();
                uiComp.Manager = this;
                uiComp.Component = _component;
                uiComp.HashedSettingId = _component.GetHashedModelSettingId();
                uiComp.Index = index;
                uiComp.Data = data;
                ModelSelectionManager.Add(uiComp);
                if (index == 0)
                {
                    uiComp.SelectByManager();
                    SetupColorList();
                }
            });
        }

        private void SetupColorList()
        {
            if (_component == null || _component.MaxColorOptions <= 0)
            {
                if (uiColorRoot != null)
                    uiColorRoot.SetActive(false);
                return;
            }

            if (uiColorRoot != null)
                uiColorRoot.SetActive(true);

            // Setup color list
            ColorSelectionManager.DeselectSelectedUI();
            ColorSelectionManager.Clear();
            ColorList.HideAll();
            ColorList.Generate(_component.ColorOptions, (index, data, ui) =>
            {
                UIBodyPartColorOption uiComp = ui.GetComponent<UIBodyPartColorOption>();
                uiComp.Manager = this;
                uiComp.Component = _component;
                uiComp.HashedSettingId = _component.GetHashedColorSettingId();
                uiComp.Index = index;
                uiComp.Data = data;
                ColorSelectionManager.Add(uiComp);
                if (index == 0)
                {
                    uiComp.SelectByManager();
                    OnSelectColorUI(uiComp);
                }
            });
        }
    }
}