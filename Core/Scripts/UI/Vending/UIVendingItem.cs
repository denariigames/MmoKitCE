using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIVendingItem : UIDataForCharacter<VendingItem>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Open Price}")]
        public UILocaleKeySetting formatKeyPrice = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SELL_PRICE);

        [Header("UI Elements")]
        public UICharacterItem uiCharacterItem;
        public TextWrapper textPrice;
        public InputFieldWrapper inputPrice;
        public UIStartVending uiStartVending;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiCharacterItem = null;
            textPrice = null;
            inputPrice = null;
            uiStartVending = null;
        }

        protected override void UpdateData()
        {
            if (uiCharacterItem != null)
                uiCharacterItem.Setup(new UICharacterItemData(Data.item, InventoryType.Vending), Character, IndexOfData);

            if (textPrice != null)
            {
                textPrice.text = ZString.Format(
                    LanguageManager.GetText(formatKeyPrice),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.price.ToString("N0"));
            }

            if (inputPrice != null)
            {
                inputPrice.onValueChanged.RemoveListener(OnInputPriceValueChanged);
                inputPrice.onValueChanged.AddListener(OnInputPriceValueChanged);
                inputPrice.contentType = InputField.ContentType.IntegerNumber;
                inputPrice.SetTextWithoutNotify(Data.price.ToString());
            }
        }

        private void OnInputPriceValueChanged(string value)
        {
            if (!int.TryParse(value, out int price))
                return;

            if (textPrice != null)
            {
                textPrice.text = ZString.Format(
                    LanguageManager.GetText(formatKeyPrice),
                    Data == null ? LanguageManager.GetUnknowTitle() : price.ToString("N0"));
            }

            uiStartVending.SetPrice(IndexOfData, price);
        }

        public void OnClickBuy()
        {
            GameInstance.PlayingCharacterEntity.VendingComponent.CallCmdBuyItem(IndexOfData);
        }

        public void OnClickCancel()
        {
            uiStartVending.RemoveItem(IndexOfData);
        }
    }
}