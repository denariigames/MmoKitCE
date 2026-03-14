using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICharacterItems : UIBase
    {
        [Header("Filter")]
        public List<string> filterCategories = new List<string>();
        public List<ItemType> filterItemTypes = new List<ItemType>();
        public List<SocketEnhancerType> filterSocketEnhancerTypes = new List<SocketEnhancerType>();
        public bool doNotShowEmptySlots;
        public bool canSelectEmptySlots;

        [Header("UI Elements")]
        public GameObject listEmptyObject;
        [FormerlySerializedAs("uiItemDialog")]
        public UICharacterItem uiDialog;
        [FormerlySerializedAs("uiCharacterItemPrefab")]
        public UICharacterItem uiPrefab;
        [FormerlySerializedAs("uiCharacterItemContainer")]
        public Transform uiContainer;
        public InventoryType inventoryType = InventoryType.NonEquipItems;

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

        private UICharacterItemSelectionManager _cacheSelectionManager;
        public UICharacterItemSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UICharacterItemSelectionManager>();
                return _cacheSelectionManager;
            }
        }

        public delegate void OnGenerateEntryDelegate(int indexOfData, CharacterItem uiCharacterItem, int indexOfUi, UICharacterItem ui);
        public event OnGenerateEntryDelegate onGenerateEntry = null;
        public UICharacterItemsUtils.GetFilteredListDelegate overrideGetFilteredListFunction = null;
        public virtual ICharacterData Character { get; protected set; }
        public List<CharacterItem> LoadedList { get; private set; } = new List<CharacterItem>();

        private UISelectionMode _dirtySelectionMode;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            listEmptyObject = null;
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            _cacheList = null;
            _cacheSelectionManager = null;
            onGenerateEntry = null;
            Character = null;
            LoadedList?.Clear();
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
            CacheSelectionManager.eventOnSelected.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelected.AddListener(OnSelect);
            CacheSelectionManager.eventOnDeselected.RemoveListener(OnDeselect);
            CacheSelectionManager.eventOnDeselected.AddListener(OnDeselect);
            if (uiDialog != null)
                uiDialog.onHide.AddListener(OnDialogHide);
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

        protected virtual void OnSelect(UICharacterItem ui)
        {
            if (!canSelectEmptySlots && ui.Data.characterItem.IsEmptySlot())
            {
                CacheSelectionManager.DeselectSelectedUI();
                return;
            }
            if (uiDialog != null && !ui.Data.characterItem.IsEmptySlot() &&
                (CacheSelectionManager.selectionMode == UISelectionMode.SelectSingle ||
                CacheSelectionManager.selectionMode == UISelectionMode.Toggle))
            {
                uiDialog.selectionManager = CacheSelectionManager;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UICharacterItem ui)
        {
            if (uiDialog != null && (CacheSelectionManager.selectionMode == UISelectionMode.SelectSingle ||
                CacheSelectionManager.selectionMode == UISelectionMode.Toggle))
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        public void UpdateData(ICharacterData character, IDictionary<BaseItem, int> items)
        {
            Character = character;
            LoadedList.Clear();
            foreach (KeyValuePair<BaseItem, int> item in items)
            {
                LoadedList.Add(CharacterItem.Create(item.Key, 1, item.Value));
            }
            GenerateList();
        }

        public void UpdateData(ICharacterData character, IDictionary<int, int> items)
        {
            Character = character;
            LoadedList.Clear();
            BaseItem tempItem;
            foreach (KeyValuePair<int, int> item in items)
            {
                if (GameInstance.Items.TryGetValue(item.Key, out tempItem))
                    LoadedList.Add(CharacterItem.Create(tempItem, 1, item.Value));
            }
            GenerateList();
        }

        public void UpdateData(ICharacterData character, IList<ItemAmount> items)
        {
            Character = character;
            LoadedList.Clear();
            foreach (ItemAmount item in items)
            {
                LoadedList.Add(CharacterItem.Create(item.item, 1, item.amount));
            }
            GenerateList();
        }

        public void UpdateData(ICharacterData character, IList<RewardedItem> items)
        {
            Character = character;
            LoadedList.Clear();
            foreach (RewardedItem item in items)
            {
                LoadedList.Add(CharacterItem.Create(item.item, item.level, item.amount, item.randomSeed));
            }
            GenerateList();
        }

        public virtual void UpdateData(ICharacterData character, IList<CharacterItem> characterItems)
        {
            Character = character;
            LoadedList.Clear();
            if (characterItems != null && characterItems.Count > 0)
                LoadedList.AddRange(characterItems);
            GenerateList();
        }

        public virtual void GenerateList()
        {
            string selectedId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.CharacterItem.id : string.Empty;
            CacheSelectionManager.Clear();

            List<KeyValuePair<int, CharacterItem>> filteredList;
            if (overrideGetFilteredListFunction != null)
                filteredList = overrideGetFilteredListFunction(LoadedList, filterCategories, filterItemTypes, filterSocketEnhancerTypes, doNotShowEmptySlots);
            else
                filteredList = UICharacterItemsUtils.GetFilteredList(LoadedList, filterCategories, filterItemTypes, filterSocketEnhancerTypes, doNotShowEmptySlots);
            OnListFiltered(filteredList);

            if (Character == null || filteredList.Count == 0)
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

            InventoryType itemInventoryType;
            UICharacterItem selectedUI = null;
            UICharacterItem tempUI;
            CacheList.Generate(filteredList, (index, data, ui) =>
            {
                itemInventoryType = inventoryType;
                if (data.Key < 0)
                    itemInventoryType = InventoryType.Unknow;
                tempUI = ui.GetComponent<UICharacterItem>();
                tempUI.Setup(new UICharacterItemData(data.Value, itemInventoryType), Character, data.Key);
                tempUI.Show();
                if (onGenerateEntry != null)
                    onGenerateEntry.Invoke(data.Key, data.Value, index, tempUI);
                UICharacterItemDragHandler dragHandler = tempUI.GetComponentInChildren<UICharacterItemDragHandler>();
                if (dragHandler != null)
                {
                    switch (itemInventoryType)
                    {
                        case InventoryType.NonEquipItems:
                            dragHandler.SetupForNonEquipItems(tempUI);
                            break;
                        case InventoryType.EquipItems:
                        case InventoryType.EquipWeaponRight:
                        case InventoryType.EquipWeaponLeft:
                            dragHandler.SetupForEquipItems(tempUI);
                            break;
                        case InventoryType.StorageItems:
                            dragHandler.SetupForStorageItems(tempUI);
                            break;
                        case InventoryType.ItemsContainer:
                            dragHandler.SetupForItemsContainer(tempUI);
                            break;
                        case InventoryType.Vending:
                            dragHandler.SetupForVending(tempUI);
                            break;
                        case InventoryType.Unknow:
                            dragHandler.SetupForUnknow(tempUI);
                            break;
                    }
                }
                CacheSelectionManager.Add(tempUI);
                if (!string.IsNullOrEmpty(selectedId) && selectedId.Equals(data.Value.id))
                    selectedUI = tempUI;
            });

            if (selectedUI == null)
            {
                CacheSelectionManager.DeselectSelectedUI();
            }
            else
            {
                bool defaultDontShowComparingEquipments = uiDialog != null ? uiDialog.dontShowComparingEquipments : false;
                if (uiDialog != null)
                    uiDialog.dontShowComparingEquipments = true;
                selectedUI.SelectByManager();
                if (uiDialog != null)
                    uiDialog.dontShowComparingEquipments = defaultDontShowComparingEquipments;
            }
        }

        protected virtual void OnListFiltered(List<KeyValuePair<int, CharacterItem>> filteredList)
        {

        }

        protected virtual void Update()
        {
            if (CacheSelectionManager.selectionMode != _dirtySelectionMode)
            {
                CacheSelectionManager.DeselectAll();
                _dirtySelectionMode = CacheSelectionManager.selectionMode;
                if (uiDialog != null)
                {
                    uiDialog.onHide.RemoveListener(OnDialogHide);
                    uiDialog.Hide();
                    uiDialog.onHide.AddListener(OnDialogHide);
                }
            }
        }
    }
}
