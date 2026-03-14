using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIDismantleItem : UIBaseOwningCharacterItem
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Return Gold Amount}")]
        public UILocaleKeySetting formatKeyReturnGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);
        [Tooltip("Format => {0} = {Amount of Dismantle Item}")]
        public UILocaleKeySetting formatKeyDismantleAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements for UI Dismantle Item")]
        public UIItemAmounts uiReturnItems;
        public UICurrencyAmounts uiReturnCurrencies;
        public TextWrapper uiTextReturnGold;
        public TextWrapper uiTextDismantleAmount;

        private int _dismantleAmount;
        public int DismantleAmount
        {
            get { return _dismantleAmount; }
            private set
            {
                _dismantleAmount = value;
                if (uiTextDismantleAmount != null)
                    uiTextDismantleAmount.text = ZString.Format(LanguageManager.GetText(formatKeyDismantleAmount), _dismantleAmount.ToString("N0"));
            }
        }

        protected bool _activated;
        protected string _activeItemId;
        protected Dictionary<Currency, int> _tempReturningCurrencies = new Dictionary<Currency, int>();
        protected Dictionary<BaseItem, int> _tempReturningItems = new Dictionary<BaseItem, int>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiReturnItems = null;
            uiReturnCurrencies = null;
            uiTextReturnGold = null;
            uiTextDismantleAmount = null;
            _tempReturningCurrencies.Clear();
            _tempReturningCurrencies = null;
            _tempReturningItems.Clear();
            _tempReturningItems = null;
        }

        protected override void UpdateData()
        {
            base.UpdateData();
            DismantleAmount = Amount;
        }

        public override void OnUpdateCharacterItems()
        {
            if (!IsVisible())
                return;

            // Store data to variable so it won't lookup for data from property again
            CharacterItem characterItem = CharacterItem;

            if (_activated && (characterItem.IsEmptySlot() || !characterItem.id.Equals(_activeItemId)))
            {
                // Item's ID is difference to active item ID, so the item may be destroyed
                // So clear data
                Data = new UIOwningCharacterItemData(InventoryType.NonEquipItems, -1, 0);
                return;
            }

            if (uiCharacterItem != null)
            {
                if (characterItem.IsEmptySlot())
                {
                    uiCharacterItem.Hide();
                }
                else
                {
                    uiCharacterItem.Setup(new UICharacterItemData(characterItem, InventoryType), GameInstance.PlayingCharacter, IndexOfData);
                    uiCharacterItem.Show();
                }
            }

            List<ItemAmount> returningItems;
            List<CurrencyAmount> returningCurrencies;
            characterItem.GetDismantleReturnItems(DismantleAmount, out returningItems, out returningCurrencies);

            if (uiReturnItems != null)
            {
                if (characterItem.IsEmptySlot() || returningItems.Count == 0)
                {
                    uiReturnItems.Hide();
                }
                else
                {
                    uiReturnItems.displayType = UIItemAmounts.DisplayType.Simple;
                    uiReturnItems.Show();
                    _tempReturningItems.Clear();
                    GameDataHelpers.CombineItems(returningItems, _tempReturningItems);
                    uiReturnItems.Data = _tempReturningItems;
                }
            }

            if (uiReturnCurrencies != null)
            {
                if (returningCurrencies.Count == 0)
                {
                    uiReturnCurrencies.Hide();
                }
                else
                {
                    uiReturnCurrencies.displayType = UICurrencyAmounts.DisplayType.Simple;
                    uiReturnCurrencies.Show();
                    _tempReturningCurrencies.Clear();
                    GameDataHelpers.CombineCurrencies(returningCurrencies, _tempReturningCurrencies, 1f);
                    uiReturnCurrencies.Data = _tempReturningCurrencies;
                }
            }

            if (uiTextReturnGold != null)
            {
                if (characterItem.IsEmptySlot())
                {
                    uiTextReturnGold.text = ZString.Format(
                            LanguageManager.GetText(formatKeyReturnGold),
                            "0");
                }
                else
                {
                    uiTextReturnGold.text = ZString.Format(
                            LanguageManager.GetText(formatKeyReturnGold),
                            (characterItem.GetItem().DismantleReturnGold * DismantleAmount).ToString("N0"));
                }
            }
        }

        public override void Show()
        {
            base.Show();
            _activated = false;
            OnUpdateCharacterItems();
        }

        public override void Hide()
        {
            base.Hide();
            Data = new UIOwningCharacterItemData(InventoryType.NonEquipItems, -1, 0);
        }

        public void OnClickDismantle()
        {
            if (CharacterItem.IsEmptySlot() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;
            _activated = true;
            _activeItemId = CharacterItem.id;
            GameInstance.ClientInventoryHandlers.RequestDismantleItem(new RequestDismantleItemMessage()
            {
                inventoryType = InventoryType,
                index = IndexOfData,
                equipSlotIndex = EquipSlotIndex,
                amount = DismantleAmount,
            }, ClientInventoryActions.ResponseDismantleItem);
        }

        public void OnClickSetDismantleAmount()
        {
            if (Amount > 1)
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_DISMANTLE_ITEM.ToString()), LanguageManager.GetText(UITextKeys.UI_DISMANTLE_ITEM_DESCRIPTION.ToString()), OnDismantleItemAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
            else
                DismantleAmount = Amount;
        }

        private void OnDismantleItemAmountConfirmed(int amount)
        {
            DismantleAmount = amount;
        }
    }
}
