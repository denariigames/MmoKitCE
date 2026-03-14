using UnityEngine;

namespace MultiplayerARPG
{
    public class UIBlendshapeManager : MonoBehaviour
    {
        public delegate void SetBlendshapeValueDelegate(int hashedSettingId, float value);

        [Header("Blendshape Option Settings")]
        public GameObject uiRoot;
        public UIBlendshapeOption uiDialog;
        public UIBlendshapeOption uiPrefab;
        public Transform uiContainer;

        public SetBlendshapeValueDelegate onSetBlendshapeValue;

        private UIList _cacheList;
        public UIList CacheList
        {
            get
            {
                if (_cacheList == null)
                {
                    _cacheList = gameObject.AddComponent<UIList>();
                    _cacheList.uiPrefab = uiPrefab.gameObject;
                    _cacheList.uiContainer = uiContainer;
                }
                return _cacheList;
            }
        }

        private UIBlendshapeListSelectionManager _cacheSelectionManager;
        public UIBlendshapeListSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIBlendshapeListSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        private UISelectionManagerShowOnSelectEventManager<PlayerCharacterBlendshapeComponent.BlendshapeOption, UIBlendshapeOption> _listEventSetupManager = new UISelectionManagerShowOnSelectEventManager<PlayerCharacterBlendshapeComponent.BlendshapeOption, UIBlendshapeOption>();
        private BaseCharacterModel _model;
        private PlayerCharacterBlendshapeComponent _component;

        private void OnEnable()
        {
            _listEventSetupManager.OnEnable(CacheSelectionManager, uiDialog);
        }

        private void OnDisable()
        {
            _listEventSetupManager.OnDisable();
        }

        public void SetCharacterModel(BaseCharacterModel model)
        {
            _model = model;
            _component = _model.transform.root.GetComponentInChildren<PlayerCharacterBlendshapeComponent>();
            SetupBlendshapeList();
        }


        private void SetupBlendshapeList()
        {
            if (_component == null || _component.options.Length <= 0)
            {
                if (uiRoot != null)
                    uiRoot.SetActive(false);
                return;
            }

            if (uiRoot != null)
                uiRoot.SetActive(true);

            // Setup model list
            CacheSelectionManager.DeselectSelectedUI();
            CacheSelectionManager.Clear();
            CacheList.HideAll();
            CacheList.Generate(_component.options, (index, data, ui) =>
            {
                UIBlendshapeOption uiComp = ui.GetComponent<UIBlendshapeOption>();
                uiComp.Manager = this;
                uiComp.Component = _component;
                uiComp.HashedSettingId = _component.GetHashedSettingId(data);
                uiComp.Index = index;
                uiComp.Data = data;
                CacheSelectionManager.Add(uiComp);
                if (index == 0)
                {
                    uiComp.SelectByManager();
                }
            });
        }
    }
}