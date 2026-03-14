using Cysharp.Text;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIRepairEquipItems : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD);
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGoldNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Target Amount}")]
        public UILocaleKeySetting formatKeySimpleRequireGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper uiTextRequireGold;
        public UIItemAmounts uiRequireItemAmounts;
        public UICurrencyAmounts uiRequireCurrencyAmounts;
        public TextWrapper uiTextSimpleRequireGold;

        protected Dictionary<Currency, int> _tempRequireCurrencies = new Dictionary<Currency, int>();
        protected Dictionary<BaseItem, int> _tempRequireItems = new Dictionary<BaseItem, int>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextRequireGold = null;
            uiRequireItemAmounts = null;
            uiRequireCurrencyAmounts = null;
            uiTextSimpleRequireGold = null;
            _tempRequireCurrencies.Clear();
            _tempRequireCurrencies = null;
            _tempRequireItems.Clear();
            _tempRequireItems = null;
        }

        private void LateUpdate()
        {
            int requireGold = 0;
            List<ItemAmount> requireItems = new List<ItemAmount>();
            List<CurrencyAmount> requireCurrencies = new List<CurrencyAmount>();
            ItemRepairPrice tempRepairPrice;
            EquipWeapons equipWeapons = GameInstance.PlayingCharacterEntity.EquipWeapons;
            if (!equipWeapons.IsEmptyRightHandSlot() &&
                equipWeapons.GetRightHandItem().TryGetRepairPrice(equipWeapons.rightHand.durability, out _, out tempRepairPrice))
            {
                requireGold += tempRepairPrice.RequireGold;
                if (tempRepairPrice.RequireItems != null && tempRepairPrice.RequireItems.Length > 0)
                    requireItems.AddRange(tempRepairPrice.RequireItems);
                if (tempRepairPrice.RequireCurrencies != null && tempRepairPrice.RequireCurrencies.Length > 0)
                    requireCurrencies.AddRange(tempRepairPrice.RequireCurrencies);
            }
            if (!equipWeapons.IsEmptyLeftHandSlot() &&
                equipWeapons.GetLeftHandItem().TryGetRepairPrice(equipWeapons.leftHand.durability, out _, out tempRepairPrice))
            {
                requireGold += tempRepairPrice.RequireGold;
                if (tempRepairPrice.RequireItems != null && tempRepairPrice.RequireItems.Length > 0)
                    requireItems.AddRange(tempRepairPrice.RequireItems);
                if (tempRepairPrice.RequireCurrencies != null && tempRepairPrice.RequireCurrencies.Length > 0)
                    requireCurrencies.AddRange(tempRepairPrice.RequireCurrencies);
            }
            foreach (CharacterItem equipItem in GameInstance.PlayingCharacterEntity.EquipItems)
            {
                if (!equipItem.IsEmptySlot() &&
                    equipItem.GetItem().TryGetRepairPrice(equipItem.durability, out _, out tempRepairPrice))
                {
                    requireGold += tempRepairPrice.RequireGold;
                    if (tempRepairPrice.RequireItems != null && tempRepairPrice.RequireItems.Length > 0)
                        requireItems.AddRange(tempRepairPrice.RequireItems);
                    if (tempRepairPrice.RequireCurrencies != null && tempRepairPrice.RequireCurrencies.Length > 0)
                        requireCurrencies.AddRange(tempRepairPrice.RequireCurrencies);
                }
            }

            if (uiRequireItemAmounts != null)
            {
                if (requireItems.Count == 0)
                {
                    uiRequireItemAmounts.Hide();
                }
                else
                {
                    uiRequireItemAmounts.displayType = UIItemAmounts.DisplayType.Requirement;
                    uiRequireItemAmounts.Show();
                    _tempRequireItems.Clear();
                    GameDataHelpers.CombineItems(requireItems, _tempRequireItems);
                    uiRequireItemAmounts.Data = _tempRequireItems;
                }
            }

            if (uiRequireCurrencyAmounts != null)
            {
                if (requireCurrencies.Count == 0)
                {
                    uiRequireCurrencyAmounts.Hide();
                }
                else
                {
                    uiRequireCurrencyAmounts.displayType = UICurrencyAmounts.DisplayType.Requirement;
                    uiRequireCurrencyAmounts.Show();
                    _tempRequireCurrencies.Clear();
                    GameDataHelpers.CombineCurrencies(requireCurrencies, _tempRequireCurrencies, 1f);
                    uiRequireCurrencyAmounts.Data = _tempRequireCurrencies;
                }
            }

            if (uiTextRequireGold != null)
            {
                uiTextRequireGold.text = ZString.Format(
                    GameInstance.PlayingCharacter.Gold >= requireGold ?
                        LanguageManager.GetText(formatKeyRequireGold) :
                        LanguageManager.GetText(formatKeyRequireGoldNotEnough),
                    GameInstance.PlayingCharacter.Gold.ToString("N0"),
                    requireGold.ToString("N0"));
            }

            if (uiTextSimpleRequireGold != null)
                uiTextSimpleRequireGold.text = ZString.Format(LanguageManager.GetText(formatKeySimpleRequireGold), requireGold.ToString("N0"));
        }

        public void OnClickRepairEquipItems()
        {
            GameInstance.ClientInventoryHandlers.RequestRepairEquipItems(ClientInventoryActions.ResponseRepairEquipItems);
        }
    }
}
