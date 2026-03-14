using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIBulkSellItems : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Return Gold Amount}")]
        public UILocaleKeySetting formatKeyReturnGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);

        [Header("UI Elements")]
        [Tooltip("UI which showing items in inventory, will use it to select items to sell")]
        public UINonEquipItems uiNonEquipItems;
        public TextWrapper uiTextReturnGold;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiNonEquipItems = null;
            uiTextReturnGold = null;
        }

        private void OnEnable()
        {
            if (uiNonEquipItems == null)
                uiNonEquipItems = FindFirstObjectByType<UINonEquipItems>();
        }

        private void OnDisable()
        {
            uiNonEquipItems.CacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
        }

        private void Update()
        {
            uiNonEquipItems.CacheSelectionManager.selectionMode = UISelectionMode.SelectMultiple;
        }

        private void LateUpdate()
        {
            int returnGold = 0;
            CharacterItem tempCharacterItem;
            List<UICharacterItem> selectedUIs = uiNonEquipItems.CacheSelectionManager.GetSelectedUIs();
            foreach (UICharacterItem selectedUI in selectedUIs)
            {
                tempCharacterItem = selectedUI.Data.characterItem;
                if (tempCharacterItem.IsEmptySlot() || selectedUI.InventoryType != InventoryType.NonEquipItems)
                    continue;
                if (tempCharacterItem.GetItem().RestrictSelling)
                {
                    selectedUI.DeselectByManager();
                    ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_ITEM_SELLING_RESTRICTED);
                    continue;
                }
                returnGold += tempCharacterItem.GetItem().SellPrice * tempCharacterItem.amount;
            }

            if (uiTextReturnGold != null)
            {
                uiTextReturnGold.text = ZString.Format(
                        LanguageManager.GetText(formatKeyReturnGold),
                        returnGold.ToString("N0"));
            }
        }

        public void OnClickSellItems()
        {
            List<int> indexes = new List<int>();
            CharacterItem tempCharacterItem;
            List<UICharacterItem> selectedUIs = uiNonEquipItems.CacheSelectionManager.GetSelectedUIs();
            foreach (UICharacterItem selectedUI in selectedUIs)
            {
                tempCharacterItem = selectedUI.Data.characterItem;
                if (tempCharacterItem.IsEmptySlot() || selectedUI.InventoryType != InventoryType.NonEquipItems)
                    continue;
                indexes.Add(selectedUI.IndexOfData);
            }
            GameInstance.ClientInventoryHandlers.RequestSellItems(new RequestSellItemsMessage()
            {
                selectedIndexes = indexes.ToArray(),
            }, ClientInventoryActions.ResponseSellItems);
        }
    }
}
