using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiplayerARPG
{
    [DefaultExecutionOrder(DefaultExecutionOrders.UI_ITEM_CRAFT_FORMULAS)]
    public partial class UIItemCraftFormulas : UIBase
    {
        [Header("Filter")]
        public List<string> filterCategories = new List<string>();

        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIItemCraftFormula uiDialog;
        public UIItemCraftFormula uiPrefab;
        public Transform uiContainer;
        public bool selectFirstEntryByDefault;

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

        private UIItemCraftFormulaSelectionManager _cacheSelectionManager;
        public UIItemCraftFormulaSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIItemCraftFormulaSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        public UICraftingQueueItems CraftingQueueManager { get; set; }
        public List<ItemCraftFormula> LoadedList { get; private set; } = new List<ItemCraftFormula>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            filterCategories.Clear();
            listEmptyObject = null;
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            _cacheList = null;
            _cacheSelectionManager = null;
            CraftingQueueManager = null;
            LoadedList.Nullify();
            LoadedList?.Clear();
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelected.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelected.AddListener(OnSelect);
            CacheSelectionManager.eventOnDeselected.RemoveListener(OnDeselect);
            CacheSelectionManager.eventOnDeselected.AddListener(OnDeselect);
            if (uiDialog != null)
            {
                uiDialog.onHide.AddListener(OnDialogHide);
                uiDialog.CraftFormulaManager = this;
            }
            UpdateData();
        }

        protected virtual void OnDisable()
        {
            if (uiDialog != null)
                uiDialog.onHide.RemoveListener(OnDialogHide);
            CacheSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnDialogHide()
        {
            CacheSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnSelect(UIItemCraftFormula ui)
        {
            if (uiDialog != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UIItemCraftFormula ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        public virtual void UpdateData()
        {
            int sourceId = CraftingQueueManager != null && CraftingQueueManager.Source != null ? CraftingQueueManager.Source.SourceId : 0;
            LoadedList.Clear();
            LoadedList.AddRange(GameInstance.ItemCraftFormulas.Values.Where(o => o.SourceIds.Contains(sourceId)));
            GenerateList();
        }

        public virtual void GenerateList()
        {
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.DataId : 0;
            CacheSelectionManager.Clear();

            List<ItemCraftFormula> filteredList = UIItemCraftFormulasUtils.GetFilteredList(LoadedList, filterCategories);
            if (filteredList.Count == 0)
            {
                CacheSelectionManager.DeselectSelectedUI();
                if (uiDialog != null)
                    uiDialog.Hide();
                CacheList.HideAll();
                if (listEmptyObject != null)
                    listEmptyObject.SetActive(true);
                return;
            }

            if (listEmptyObject != null)
                listEmptyObject.SetActive(false);

            UIItemCraftFormula selectedUI = null;
            UIItemCraftFormula tempUI;
            CacheList.Generate(filteredList, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIItemCraftFormula>();
                tempUI.CraftFormulaManager = this;
                tempUI.Data = data;
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if ((selectFirstEntryByDefault && index == 0) || selectedDataId == data.DataId)
                    selectedUI = tempUI;
            });

            if (selectedUI == null)
            {
                CacheSelectionManager.DeselectSelectedUI();
            }
            else
            {
                selectedUI.SelectByManager();
            }
        }
    }
}
