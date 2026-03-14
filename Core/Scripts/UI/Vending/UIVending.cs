using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIVending : UISelectionEntry<BasePlayerCharacterEntity>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper textTitle;
        public UIVendingItem uiSelectedItem;
        public UIVendingItem uiItemPrefab;
        public Transform uiItemContainer;
        public GameObject[] ownerObjects;
        public GameObject[] nonOwnerObjects;

        private UIList _itemList;
        public UIList ItemList
        {
            get
            {
                if (_itemList == null)
                {
                    _itemList = gameObject.AddComponent<UIList>();
                    _itemList.uiPrefab = uiItemPrefab.gameObject;
                    _itemList.uiContainer = uiItemContainer;
                }
                return _itemList;
            }
        }

        private UIVendingItemSelectionManager _itemSelectionManager;
        public UIVendingItemSelectionManager ItemSelectionManager
        {
            get
            {
                if (_itemSelectionManager == null)
                    _itemSelectionManager = gameObject.GetOrAddComponent<UIVendingItemSelectionManager>();
                _itemSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _itemSelectionManager;
            }
        }

        private UISelectionManagerShowOnSelectEventManager<VendingItem, UIVendingItem> _itemListEventSetupManager = new UISelectionManagerShowOnSelectEventManager<VendingItem, UIVendingItem>();
        private BasePlayerCharacterEntity _entity;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            textTitle = null;
            uiSelectedItem = null;
            uiItemPrefab = null;
            uiItemContainer = null;
            ownerObjects.Nullify();
            nonOwnerObjects.Nullify();
            _itemList = null;
            _itemSelectionManager = null;
            _itemListEventSetupManager = null;
            _entity = null;
            _data = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _itemListEventSetupManager.OnEnable(ItemSelectionManager, uiSelectedItem);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _itemListEventSetupManager.OnDisable();
            if (_entity != null)
            {
                _entity.VendingComponent.onVendingDataChange -= Vending_onVendingDataChange;
                _entity.VendingComponent.onUpdateItems -= Vending_onUpdateItems;
            }
            GameInstance.PlayingCharacterEntity.VendingComponent.CallCmdUnsubscribe();

            foreach (GameObject obj in ownerObjects)
            {
                obj.SetActive(false);
            }

            foreach (GameObject obj in nonOwnerObjects)
            {
                obj.SetActive(false);
            }
        }

        private void Vending_onVendingDataChange(VendingData data)
        {
            if (!data.isStarted)
                Hide();
        }

        private void Vending_onUpdateItems(VendingItems items)
        {
            UpdateItemList(items);
        }

        public void UpdateItemList(VendingItems items)
        {
            ItemSelectionManager.DeselectSelectedUI();
            ItemSelectionManager.Clear();
            ItemList.HideAll();
            ItemList.Generate(items, (index, data, ui) =>
            {
                UIVendingItem uiComp = ui.GetComponent<UIVendingItem>();
                uiComp.Setup(data, Data, index);
                ItemSelectionManager.Add(uiComp);
            });
        }

        protected override void UpdateData()
        {
            if (_entity != null)
            {
                _entity.VendingComponent.onVendingDataChange -= Vending_onVendingDataChange;
                _entity.VendingComponent.onUpdateItems -= Vending_onUpdateItems;
            }
            _entity = Data;
            if (_entity == null)
                return;
            _entity.VendingComponent.onVendingDataChange += Vending_onVendingDataChange;
            _entity.VendingComponent.onUpdateItems += Vending_onUpdateItems;

            if (textTitle != null)
            {
                textTitle.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.VendingComponent.Data.title);
            }
            ItemSelectionManager.Clear();
            ItemList.HideAll();
            GameInstance.PlayingCharacterEntity.VendingComponent.CallCmdSubscribe(Data.ObjectId);

            foreach (GameObject obj in ownerObjects)
            {
                obj.SetActive(Data.IsOwnerClient);
            }

            foreach (GameObject obj in nonOwnerObjects)
            {
                obj.SetActive(!Data.IsOwnerClient);
            }
        }

        public void OnClickStop()
        {
            UISceneGlobal.Singleton.ShowMessageDialog(
                LanguageManager.GetText(UITextKeys.UI_STOP_VENDING.ToString()),
                LanguageManager.GetText(UITextKeys.UI_STOP_VENDING_DESCRIPTION.ToString()),
                false, true, true, false, onClickYes: () =>
                {
                    GameInstance.PlayingCharacterEntity.VendingComponent.CallCmdStopVending();
                    Hide();
                });
        }
    }
}