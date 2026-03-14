using Cysharp.Text;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIItemCraft : UISelectionEntry<ItemCraft>
    {
        public ItemCraft ItemCraft { get { return Data; } }
        public BaseItem CraftingItem { get { return ItemCraft.CraftingItem; } }
        public int Amount { get { return ItemCraft.Amount; } }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD);
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGoldNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Target Amount}")]
        public UILocaleKeySetting formatKeySimpleRequireGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public UICharacterItem uiCraftingItem;
        public UIItemAmounts uiRequireItemAmounts;
        public UICurrencyAmounts uiRequireCurrencyAmounts;
        public TextWrapper uiTextRequireGold;
        public TextWrapper uiTextSimpleRequireGold;

        public CrafterType CrafterType { get; private set; }
        public BaseGameEntity TargetEntity { get; private set; }

        protected Dictionary<Currency, int> _tempRequireCurrencies = new Dictionary<Currency, int>();
        protected Dictionary<BaseItem, int> _tempRequireItems = new Dictionary<BaseItem, int>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiCraftingItem = null;
            uiRequireItemAmounts = null;
            uiRequireCurrencyAmounts = null;
            uiTextRequireGold = null;
            uiTextSimpleRequireGold = null;
            TargetEntity = null;
            _tempRequireCurrencies.Clear();
            _tempRequireCurrencies = null;
            _tempRequireItems.Clear();
            _tempRequireItems = null;
        }

        public void Setup(CrafterType crafterType, BaseGameEntity targetEntity, ItemCraft data)
        {
            CrafterType = crafterType;
            TargetEntity = targetEntity;
            Data = data;
        }

        public override void CloneTo(UISelectionEntry<ItemCraft> target)
        {
            base.CloneTo(target);
            if (target != null && target is UIItemCraft castedTarget)
            {
                castedTarget.CrafterType = CrafterType;
                castedTarget.TargetEntity = TargetEntity;
            }
        }

        public override void ManagedUpdate()
        {
            base.ManagedUpdate();

            if (uiRequireItemAmounts != null)
            {
                if (CraftingItem == null || ItemCraft.RequireItems == null || ItemCraft.RequireItems.Length == 0)
                {
                    uiRequireItemAmounts.Hide();
                }
                else
                {
                    uiRequireItemAmounts.displayType = UIItemAmounts.DisplayType.Requirement;
                    uiRequireItemAmounts.Show();
                    _tempRequireItems.Clear();
                    GameDataHelpers.CombineItems(ItemCraft.RequireItems, _tempRequireItems);
                    uiRequireItemAmounts.Data = _tempRequireItems;
                }
            }

            if (uiRequireCurrencyAmounts != null)
            {
                if (CraftingItem == null || ItemCraft.RequireCurrencies == null || ItemCraft.RequireCurrencies.Length == 0)
                {
                    uiRequireCurrencyAmounts.Hide();
                }
                else
                {
                    uiRequireCurrencyAmounts.displayType = UICurrencyAmounts.DisplayType.Requirement;
                    uiRequireCurrencyAmounts.Show();
                    _tempRequireCurrencies.Clear();
                    GameDataHelpers.CombineCurrencies(ItemCraft.RequireCurrencies, _tempRequireCurrencies, 1f);
                    uiRequireCurrencyAmounts.Data = _tempRequireCurrencies;
                }
            }

            if (uiTextRequireGold != null)
            {
                if (CraftingItem == null)
                {
                    uiTextRequireGold.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRequireGold),
                        "0",
                        "0");
                }
                else
                {
                    uiTextRequireGold.text = ZString.Format(
                        GameInstance.PlayingCharacter.Gold >= ItemCraft.RequireGold ?
                            LanguageManager.GetText(formatKeyRequireGold) :
                            LanguageManager.GetText(formatKeyRequireGoldNotEnough),
                        GameInstance.PlayingCharacter.Gold.ToString("N0"),
                        ItemCraft.RequireGold.ToString("N0"));
                }
            }

            if (uiTextSimpleRequireGold != null)
                uiTextSimpleRequireGold.text = ZString.Format(LanguageManager.GetText(formatKeySimpleRequireGold), CraftingItem == null ? "0" : ItemCraft.RequireGold.ToString("N0"));
        }

        protected override void UpdateData()
        {
            if (uiCraftingItem != null)
            {
                if (CraftingItem == null)
                {
                    // Hide if crafting item is null
                    uiCraftingItem.Hide();
                }
                else
                {
                    uiCraftingItem.Show();
                    uiCraftingItem.Data = new UICharacterItemData(CharacterItem.Create(CraftingItem, 1, Amount), InventoryType.Unknow);
                }
            }
        }

        public void OnClickCraft()
        {
            if (GameInstance.PlayingCharacterEntity != null && CraftingItem != null)
            {
                if (CrafterType == CrafterType.Workbench && TargetEntity)
                    GameInstance.PlayingCharacterEntity.BuildingComponent.CallCmdCraftItemByWorkbench(TargetEntity.ObjectId, CraftingItem.DataId);
            }
        }
    }
}
