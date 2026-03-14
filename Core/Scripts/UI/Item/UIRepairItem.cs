using Cysharp.Text;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIRepairItem : UIBaseOwningCharacterItem
    {
        public IEquipmentItem EquipmentItem { get { return CharacterItem.GetEquipmentItem(); } }
        public bool CanRepair { get; private set; }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD);
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGoldNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Target Amount}")]
        public UILocaleKeySetting formatKeySimpleRequireGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Current Durability}, {1} = {Max Durability}")]
        public UILocaleKeySetting formatKeyDurability = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_DURABILITY);

        [Header("UI Elements for UI Repair Item")]
        public TextWrapper uiTextRequireGold;
        public UIItemAmounts uiRequireItemAmounts;
        public UICurrencyAmounts uiRequireCurrencyAmounts;
        public TextWrapper uiTextSimpleRequireGold;
        public TextWrapper uiTextDurability;

        protected bool _activated;
        protected string _activeItemId;
        protected Dictionary<Currency, int> _tempRequireCurrencies = new Dictionary<Currency, int>();
        protected Dictionary<BaseItem, int> _tempRequireItems = new Dictionary<BaseItem, int>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextRequireGold = null;
            uiRequireItemAmounts = null;
            uiRequireCurrencyAmounts = null;
            uiTextSimpleRequireGold = null;
            uiTextDurability = null;
            _tempRequireCurrencies.Clear();
            _tempRequireCurrencies = null;
            _tempRequireItems.Clear();
            _tempRequireItems = null;
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

            CanRepair = false;
            float maxDurability = 0f;
            ItemRepairPrice repairPrice = null;
            if (!characterItem.IsEmptySlot())
                CanRepair = EquipmentItem != null && characterItem.GetItem().CanRepair(GameInstance.PlayingCharacter, characterItem.durability, out maxDurability, out repairPrice);

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

            if (uiRequireItemAmounts != null)
            {
                if (repairPrice == null || repairPrice.RequireItems == null || repairPrice.RequireItems.Length == 0)
                {
                    uiRequireItemAmounts.Hide();
                }
                else
                {
                    uiRequireItemAmounts.displayType = UIItemAmounts.DisplayType.Requirement;
                    uiRequireItemAmounts.Show();
                    _tempRequireItems.Clear();
                    GameDataHelpers.CombineItems(repairPrice.RequireItems, _tempRequireItems);
                    uiRequireItemAmounts.Data = _tempRequireItems;
                }
            }

            if (uiRequireCurrencyAmounts != null)
            {
                if (repairPrice == null || repairPrice.RequireCurrencies == null || repairPrice.RequireCurrencies.Length == 0)
                {
                    uiRequireCurrencyAmounts.Hide();
                }
                else
                {
                    uiRequireCurrencyAmounts.displayType = UICurrencyAmounts.DisplayType.Requirement;
                    uiRequireCurrencyAmounts.Show();
                    _tempRequireCurrencies.Clear();
                    GameDataHelpers.CombineCurrencies(repairPrice.RequireCurrencies, _tempRequireCurrencies, 1f);
                    uiRequireCurrencyAmounts.Data = _tempRequireCurrencies;
                }
            }

            if (uiTextRequireGold != null)
            {
                if (repairPrice == null || maxDurability <= 0)
                {
                    uiTextRequireGold.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRequireGold),
                        "0",
                        "0");
                }
                else
                {
                    uiTextRequireGold.text = ZString.Format(
                        GameInstance.PlayingCharacter.Gold >= repairPrice.RequireGold ?
                            LanguageManager.GetText(formatKeyRequireGold) :
                            LanguageManager.GetText(formatKeyRequireGoldNotEnough),
                        GameInstance.PlayingCharacter.Gold.ToString("N0"),
                        repairPrice.RequireGold.ToString("N0"));
                }
            }

            if (uiTextSimpleRequireGold != null)
                uiTextSimpleRequireGold.text = ZString.Format(LanguageManager.GetText(formatKeySimpleRequireGold), (repairPrice == null || maxDurability <= 0) ? "0" : repairPrice.RequireGold.ToString("N0"));

            if (uiTextDurability != null)
            {
                if (repairPrice == null || maxDurability <= 0)
                {
                    uiTextDurability.text = ZString.Format(
                        LanguageManager.GetText(formatKeyDurability),
                        "0",
                        "0");
                }
                else
                {
                    uiTextDurability.text = ZString.Format(
                        LanguageManager.GetText(formatKeyDurability),
                        characterItem.durability.ToString("N0"),
                        maxDurability.ToString("N0"));
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

        public void OnClickRepair()
        {
            if (CharacterItem.IsEmptySlot() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;
            _activated = true;
            _activeItemId = CharacterItem.id;
            GameInstance.ClientInventoryHandlers.RequestRepairItem(new RequestRepairItemMessage()
            {
                inventoryType = InventoryType,
                index = IndexOfData,
                equipSlotIndex = EquipSlotIndex,
            }, ClientInventoryActions.ResponseRepairItem);
        }
    }
}
