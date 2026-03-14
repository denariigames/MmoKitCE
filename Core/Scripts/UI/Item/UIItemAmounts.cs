using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIItemAmounts : UISelectionEntry<Dictionary<BaseItem, int>>
    {
        public enum DisplayType
        {
            Simple,
            Requirement
        }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Item Title}, {1} = {Current Amount}, {2} = {Target Amount}")]
        public UILocaleKeySetting formatKeyAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_ITEM);
        [Tooltip("Format => {0} = {Item Title}, {1} = {Current Amount}, {2} = {Target Amount}")]
        public UILocaleKeySetting formatKeyAmountNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_ITEM_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Item Title}, {1} = {Amount}")]
        public UILocaleKeySetting formatKeySimpleAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_AMOUNT);

        [Header("UI Elements")]
        public TextWrapper uiTextAllAmounts;
        public UIItemTextPair[] textAmounts;

        [Header("List UI Elements")]
        public UICharacterItem uiEntryPrefab;
        public Transform uiListContainer;

        [Header("Options")]
        public DisplayType displayType;
        public string numberFormatSimple = "N0";
        public bool isBonus;
        public bool inactiveIfAmountZero;
        public bool useSimpleFormatIfAmountEnough = true;

        private Dictionary<BaseItem, UIItemTextPair> _cacheTextAmounts;
        public Dictionary<BaseItem, UIItemTextPair> CacheTextAmounts
        {
            get
            {
                if (_cacheTextAmounts == null)
                {
                    _cacheTextAmounts = new Dictionary<BaseItem, UIItemTextPair>();
                    BaseItem tempData;
                    foreach (UIItemTextPair componentPair in textAmounts)
                    {
                        if (componentPair.item == null || componentPair.uiText == null)
                            continue;
                        tempData = componentPair.item;
                        SetDefaultValue(componentPair);
                        _cacheTextAmounts[tempData] = componentPair;
                    }
                }
                return _cacheTextAmounts;
            }
        }


        private UIList _cacheList;
        public UIList CacheList
        {
            get
            {
                if (_cacheList == null)
                {
                    _cacheList = gameObject.AddComponent<UIList>();
                    _cacheList.uiPrefab = uiEntryPrefab.gameObject;
                    _cacheList.uiContainer = uiListContainer;
                }
                return _cacheList;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextAllAmounts = null;
            textAmounts = null;
            uiEntryPrefab = null;
            uiListContainer = null;
            _cacheTextAmounts?.Clear();
            _cacheList = null;
            _data?.Clear();
        }
        
        protected override void UpdateData()
        {
            // Reset number
            foreach (UIItemTextPair entry in CacheTextAmounts.Values)
            {
                SetDefaultValue(entry);
            }
            // Set number by updated data
            if (Data == null || Data.Count == 0)
            {
                if (uiTextAllAmounts != null)
                    uiTextAllAmounts.SetGameObjectActive(false);
            }
            else
            {
                using (Utf16ValueStringBuilder tempAllText = ZString.CreateStringBuilder(false))
                {
                    BaseItem tempData;
                    int tempCurrentAmount;
                    int tempTargetAmount;
                    bool tempAmountEnough;
                    string tempCurrentValue;
                    string tempTargetValue;
                    string tempFormat;
                    string tempAmountText;
                    UIItemTextPair tempComponentPair;
                    foreach (KeyValuePair<BaseItem, int> dataEntry in Data)
                    {
                        if (dataEntry.Key == null)
                            continue;
                        // Set temp data
                        tempData = dataEntry.Key;
                        tempTargetAmount = dataEntry.Value;
                        tempCurrentAmount = 0;
                        // Get item amount from character
                        if (GameInstance.PlayingCharacter != null)
                            tempCurrentAmount = GameInstance.PlayingCharacter.CountNonEquipItems(tempData.DataId);
                        // Use difference format by option 
                        switch (displayType)
                        {
                            case DisplayType.Requirement:
                                // This will show both current character item amount and target amount
                                tempAmountEnough = tempCurrentAmount >= tempTargetAmount;
                                tempFormat = LanguageManager.GetText(tempAmountEnough ? formatKeyAmount : formatKeyAmountNotEnough);
                                tempCurrentValue = tempCurrentAmount.ToString(numberFormatSimple);
                                tempTargetValue = tempTargetAmount.ToString(numberFormatSimple);
                                if (useSimpleFormatIfAmountEnough && tempAmountEnough)
                                    tempAmountText = ZString.Format(LanguageManager.GetText(formatKeySimpleAmount), tempData.Title, tempTargetValue);
                                else
                                    tempAmountText = ZString.Format(tempFormat, tempData.Title, tempCurrentValue, tempTargetValue);
                                break;
                            default:
                                // This will show only target amount, so current character item amount will not be shown
                                tempTargetValue = tempTargetAmount.ToString(numberFormatSimple);
                                tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeySimpleAmount),
                                    tempData.Title,
                                    tempTargetValue));
                                break;
                        }
                        // Append current item amount text
                        if (dataEntry.Value != 0 || !inactiveIfAmountZero)
                        {
                            // Add new line if text is not empty
                            if (tempAllText.Length > 0)
                                tempAllText.Append('\n');
                            tempAllText.Append(tempAmountText);
                        }
                        // Set current item text to UI
                        if (CacheTextAmounts.TryGetValue(tempData, out tempComponentPair))
                        {
                            tempComponentPair.uiText.text = tempAmountText;
                            if (tempComponentPair.root != null)
                                tempComponentPair.root.SetActive(!inactiveIfAmountZero || tempTargetAmount != 0);
                        }
                    }

                    if (uiTextAllAmounts != null)
                    {
                        uiTextAllAmounts.SetGameObjectActive(tempAllText.Length > 0 || !inactiveIfAmountZero);
                        uiTextAllAmounts.text = tempAllText.ToString();
                    }
                }
            }
            UpdateList();
        }

        private void SetDefaultValue(UIItemTextPair componentPair)
        {
            string zeroFormatSimple = 0f.ToString(numberFormatSimple);
            switch (displayType)
            {
                case DisplayType.Requirement:
                    if (useSimpleFormatIfAmountEnough)
                    {
                        componentPair.uiText.text = ZString.Format(
                            LanguageManager.GetText(formatKeySimpleAmount),
                            componentPair.item.Title,
                            zeroFormatSimple);
                    }
                    else
                    {
                        componentPair.uiText.text = ZString.Format(
                            LanguageManager.GetText(formatKeyAmount),
                            componentPair.item.Title,
                            zeroFormatSimple, zeroFormatSimple);
                    }
                    break;
                case DisplayType.Simple:
                    componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                        LanguageManager.GetText(formatKeySimpleAmount),
                        componentPair.item.Title,
                        zeroFormatSimple));
                    break;
            }
            componentPair.imageIcon.SetImageGameDataIcon(componentPair.item);
            if (inactiveIfAmountZero && componentPair.root != null)
                componentPair.root.SetActive(false);
        }

        private void UpdateList()
        {
            if (uiEntryPrefab == null || uiListContainer == null)
                return;
            CacheList.HideAll();
            UICharacterItem tempUI;
            CacheList.Generate(Data, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UICharacterItem>();
                tempUI.Data = new UICharacterItemData(CharacterItem.Create(data.Key, 1, data.Value), InventoryType.Unknow);
            });
        }
    }
}
