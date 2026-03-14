using UnityEngine;

namespace MultiplayerARPG
{
    public class UIStartVending : UIBase
    {
        public InputFieldWrapper inputTitle;
        public UIVendingItem uiSelectedItem;
        public UIVendingItem uiItemPrefab;
        public Transform uiItemContainer;

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
        private StartVendingItems _items = new StartVendingItems();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            inputTitle = null;
            uiSelectedItem = null;
            uiItemPrefab = null;
            uiItemContainer = null;
            _itemList = null;
            _itemSelectionManager = null;
            _itemListEventSetupManager = null;
        }

        private void OnEnable()
        {
            _itemListEventSetupManager.OnEnable(ItemSelectionManager, uiSelectedItem);
            inputTitle.text = string.Empty;
            _items.Clear();
            ItemSelectionManager.Clear();
            ItemList.HideAll();
        }

        private void OnDisable()
        {
            _itemListEventSetupManager.OnDisable();
        }

        public override void Show()
        {
            if (GameInstance.PlayingCharacterEntity.VendingComponent.Data.isStarted)
            {
                BaseUISceneGameplay.Singleton.ShowVending(GameInstance.PlayingCharacterEntity);
                if (IsVisible())
                    Hide();
                return;
            }
            base.Show();
        }

        public void AddItem(string id, int amount, int price)
        {
            if (GameInstance.Singleton.vendingItemsLimit > 0 && _items.Count + 1 > GameInstance.Singleton.vendingItemsLimit)
            {
                // Reached limit
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_REACHED_VENDING_ITEMS_LIMIT);
                return;
            }

            int indexOfData = GameInstance.PlayingCharacterEntity.NonEquipItems.IndexOf(id);
            if (indexOfData < 0)
            {
                // Invalid index
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_INVALID_ITEM_INDEX);
                return;
            }
            CharacterItem storeItem = GameInstance.PlayingCharacterEntity.NonEquipItems[indexOfData];
            if (storeItem.IsEmptySlot())
            {
                // Invalid data
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_INVALID_ITEM_DATA);
                return;
            }
            if (storeItem.GetItem().RestrictDealing)
            {
                // Restricted
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_ITEM_DEALING_RESTRICTED);
                return;
            }
            int countItem = 0;
            foreach (StartVendingItem item in _items)
            {
                if (id == item.id)
                    countItem += item.amount;
            }
            countItem += amount;
            if (GameInstance.PlayingCharacterEntity.NonEquipItems[indexOfData].amount < countItem)
            {
                // Invalid amount
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS);
                return;
            }
            _items.Add(new StartVendingItem()
            {
                id = id,
                amount = amount,
                price = price,
            });
            UpdateItemList();
        }

        public void RemoveItem(int index)
        {
            _items.RemoveAt(index);
            UpdateItemList();
        }

        public void UpdateItemList()
        {
            ItemSelectionManager.DeselectSelectedUI();
            ItemSelectionManager.Clear();
            ItemList.HideAll();
            ItemList.Generate(_items, (index, data, ui) =>
            {
                int indexOfItem = GameInstance.PlayingCharacterEntity.NonEquipItems.IndexOf(data.id);
                if (indexOfItem < 0)
                {
                    // Invalid index
                    return;
                }
                CharacterItem item = GameInstance.PlayingCharacterEntity.NonEquipItems[indexOfItem].Clone(false);
                item.amount = data.amount;
                UIVendingItem uiComp = ui.GetComponent<UIVendingItem>();
                uiComp.uiStartVending = this;
                uiComp.Setup(new VendingItem()
                {
                    item = item,
                    price = data.price,
                }, GameInstance.PlayingCharacterEntity, index);
                ItemSelectionManager.Add(uiComp);
            });
        }

        public void SetPrice(int index, int price)
        {
            StartVendingItem item = _items[index];
            item.price = price;
            _items[index] = item;
        }

        public void OnClickStart()
        {
            if (_items.Count <= 0)
            {
                UISceneGlobal.Singleton.ShowMessageDialog(
                    LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()),
                    LanguageManager.GetText(UITextKeys.UI_ERROR_NO_START_VENDING_ITEMS.ToString()));
                return;
            }
            UISceneGlobal.Singleton.ShowMessageDialog(
                LanguageManager.GetText(UITextKeys.UI_START_VENDING.ToString()),
                LanguageManager.GetText(UITextKeys.UI_START_VENDING_DESCRIPTION.ToString()),
                false, true, true, false, onClickYes: () =>
                {
                    GameInstance.PlayingCharacterEntity.VendingComponent.CallCmdStartVending(inputTitle.text, _items);
                    Hide();
                });
        }
    }
}