using Cysharp.Text;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UINpcSellItem : UISelectionEntry<NpcSellItem>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Sell Price}")]
        public UILocaleKeySetting formatKeySellPrice = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SELL_PRICE);

        [Header("UI Elements")]
        public UICharacterItem uiCharacterItem;
        public TextWrapper uiTextSellPrice;
        public UICurrencyAmounts uiSellPrices;
        public UIInputDialog uiAmountInputDialog;
        public TextWrapper uiTextCalculatedSellPrice;

        [Header("Options")]
        [Tooltip("If this is `TRUE`, `uiTextSellPrice` will be inactivated when item's sell price is 0")]
        public bool inactiveSellPriceIfZero;

        public int IndexOfData { get; protected set; }

        protected Dictionary<Currency, int> _tempPriceCurrencies = new Dictionary<Currency, int>();

        protected override void Awake()
        {
            base.Awake();
            if (uiAmountInputDialog != null && uiAmountInputDialog.uiInputField != null)
                uiAmountInputDialog.uiInputField.onValueChanged.AddListener(OnBuyAmountChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiCharacterItem = null;
            uiTextSellPrice = null;
            uiSellPrices = null;
            uiAmountInputDialog = null;
            if (uiAmountInputDialog != null && uiAmountInputDialog.uiInputField != null)
                uiAmountInputDialog.uiInputField.onValueChanged.RemoveListener(OnBuyAmountChanged);
            _tempPriceCurrencies.Clear();
            _tempPriceCurrencies = null;
        }

        public void Setup(NpcSellItem data, int indexOfData)
        {
            IndexOfData = indexOfData;
            Data = data;
        }

        public override void CloneTo(UISelectionEntry<NpcSellItem> target)
        {
            base.CloneTo(target);
            if (target != null && target is UINpcSellItem castedTarget)
            {
                castedTarget.IndexOfData = IndexOfData;
            }
        }

        protected override void UpdateData()
        {
            if (uiCharacterItem != null)
            {
                if (Data.item == null)
                {
                    uiCharacterItem.Hide();
                }
                else
                {
                    uiCharacterItem.Setup(new UICharacterItemData(CharacterItem.Create(Data.item, Data.level, Data.amount > 0 ? Data.amount : Data.item.MaxStack), InventoryType.NonEquipItems), GameInstance.PlayingCharacter, -1);
                    uiCharacterItem.Show();
                }
            }

            // It's how much NPC selling item to player, so use `buyItemPriceRate`
            float sellPriceRate = 1f + GameInstance.PlayingCharacter.GetCaches().BuyItemPriceRate;
            if (uiTextSellPrice != null)
            {
                int sellPrice = Mathf.CeilToInt(Data.sellPrice * sellPriceRate);
                uiTextSellPrice.text = ZString.Format(
                    LanguageManager.GetText(formatKeySellPrice),
                    sellPrice.ToString("N0"));
                uiTextSellPrice.SetGameObjectActive(!inactiveSellPriceIfZero || sellPrice != 0);
            }

            if (uiSellPrices != null)
            {
                uiSellPrices.displayType = UICurrencyAmounts.DisplayType.Simple;
                uiSellPrices.isBonus = false;
                _tempPriceCurrencies.Clear();
                GameDataHelpers.CombineCurrencies(Data.sellPrices, _tempPriceCurrencies, sellPriceRate);
                uiSellPrices.Data = _tempPriceCurrencies;
            }
        }

        public void OnClickBuy()
        {
            BaseItem item = Data.item;
            if (item == null)
            {
                Debug.LogWarning("Cannot buy item, the item data is empty");
                return;
            }

            if (uiAmountInputDialog != null)
            {
                uiAmountInputDialog.Show(
                    LanguageManager.GetText(UITextKeys.UI_BUY_ITEM.ToString()),
                    LanguageManager.GetText(UITextKeys.UI_BUY_ITEM_DESCRIPTION.ToString()),
                    OnBuyAmountConfirmed,
                    1,  /* Min Amount */
                    null,
                    1   /* Start Amount */);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(
                    LanguageManager.GetText(UITextKeys.UI_BUY_ITEM.ToString()),
                    LanguageManager.GetText(UITextKeys.UI_BUY_ITEM_DESCRIPTION.ToString()),
                    OnBuyAmountConfirmed,
                    1,  /* Min Amount */
                    null,
                    1   /* Start Amount */);
            }
        }

        private void OnBuyAmountConfirmed(int amount)
        {
            GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdBuyNpcItem(IndexOfData, amount);
        }

        private void OnBuyAmountChanged(string amountText)
        {
            if (!int.TryParse(amountText, out int amount))
            {
                if (uiTextCalculatedSellPrice != null)
                    uiTextCalculatedSellPrice.text = "0";
                return;
            }
            if (uiTextCalculatedSellPrice != null)
                uiTextCalculatedSellPrice.text = (amount * Data.sellPrice).ToString("N0");
        }
    }
}
