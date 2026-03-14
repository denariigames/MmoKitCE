using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UIItemCrafts : UIBase
    {
        [Header("UI Elements")]
        public GameObject listEmptyObject;
        [FormerlySerializedAs("uiCraftItemDialog")]
        public UIItemCraft uiDialog;
        [FormerlySerializedAs("uiCraftItemPrefab")]
        public UIItemCraft uiPrefab;
        [FormerlySerializedAs("uiCraftItemContainer")]
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

        private UIItemCraftSelectionManager _cacheSelectionManager;
        public UIItemCraftSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIItemCraftSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        public CrafterType CrafterType { get; private set; }
        public BaseGameEntity TargetEntity { get; private set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            listEmptyObject = null;
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            _cacheList = null;
            _cacheSelectionManager = null;
            TargetEntity = null;
        }

        public void Show(CrafterType crafterType, BaseGameEntity targetEntity)
        {
            CrafterType = crafterType;
            TargetEntity = targetEntity;
            switch (crafterType)
            {
                case CrafterType.Character:
                    BasePlayerCharacterEntity owningCharacter = GameInstance.PlayingCharacterEntity;
                    List<ItemCraft> itemCrafts = new List<ItemCraft>();
                    foreach (CharacterSkill characterSkill in owningCharacter.Skills)
                    {
                        if (characterSkill.GetSkill() == null || !characterSkill.GetSkill().TryGetItemCraft(out ItemCraft itemCraft))
                            continue;
                        itemCrafts.Add(itemCraft);
                    }
                    UpdateData(itemCrafts);
                    break;
                case CrafterType.Workbench:
                    if (targetEntity && targetEntity is WorkbenchEntity workbenchEntity)
                        UpdateData(workbenchEntity.ItemCrafts);
                    break;
            }
            Show();
        }

        protected virtual void OnEnable()
        {
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

        protected virtual void OnSelect(UIItemCraft ui)
        {
            if (uiDialog != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UIItemCraft ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        protected virtual void UpdateData(IList<ItemCraft> itemCrafts)
        {
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.CraftingItem.DataId : 0;
            CacheSelectionManager.Clear();

            UIItemCraft selectedUI = null;
            UIItemCraft tempUI;
            CacheList.Generate(itemCrafts, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIItemCraft>();
                tempUI.Setup(CrafterType, TargetEntity, data);
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if ((selectFirstEntryByDefault && index == 0) || selectedDataId == data.CraftingItem.DataId)
                    selectedUI = tempUI;
            });

            if (listEmptyObject != null)
                listEmptyObject.SetActive(itemCrafts.Count == 0);

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
