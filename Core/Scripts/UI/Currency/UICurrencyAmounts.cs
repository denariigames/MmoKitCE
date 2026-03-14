using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UICurrencyAmounts : UISelectionEntry<Dictionary<Currency, int>>
    {
        public enum DisplayType
        {
            Simple,
            Requirement
        }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Currency Title}, {1} = {Current Amount}, {2} = {Target Amount}")]
        public UILocaleKeySetting formatKeyAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_CURRENCY);
        [Tooltip("Format => {0} = {Currency Title}, {1} = {Current Amount}, {2} = {Target Amount}")]
        public UILocaleKeySetting formatKeyAmountNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_CURRENCY_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Currency Title}, {1} = {Amount}")]
        public UILocaleKeySetting formatKeySimpleAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENCY_AMOUNT);

        [Header("UI Elements")]
        public TextWrapper uiTextAllAmounts;
        public UICurrencyTextPair[] textAmounts;

        [Header("List UI Elements")]
        public UICharacterCurrency uiEntryPrefab;
        public Transform uiListContainer;

        [Header("Options")]
        public DisplayType displayType;
        public string numberFormatSimple = "N0";
        public bool isBonus;
        public bool inactiveIfAmountZero;
        public bool useSimpleFormatIfAmountEnough = true;

        private Dictionary<Currency, UICurrencyTextPair> _cacheTextAmounts;
        public Dictionary<Currency, UICurrencyTextPair> CacheTextAmounts
        {
            get
            {
                if (_cacheTextAmounts == null)
                {
                    _cacheTextAmounts = new Dictionary<Currency, UICurrencyTextPair>();
                    Currency tempData;
                    foreach (UICurrencyTextPair componentPair in textAmounts)
                    {
                        if (componentPair.currency == null || componentPair.uiText == null)
                            continue;
                        tempData = componentPair.currency;
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
            foreach (UICurrencyTextPair entry in CacheTextAmounts.Values)
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
                    Currency tempData;
                    int tempCurrentAmount;
                    int tempTargetAmount;
                    bool tempAmountEnough;
                    string tempCurrentValue;
                    string tempValue;
                    string tempFormat;
                    string tempAmountText;
                    UICurrencyTextPair tempComponentPair;
                    foreach (KeyValuePair<Currency, int> dataEntry in Data)
                    {
                        if (dataEntry.Key == null)
                            continue;
                        // Set temp data
                        tempData = dataEntry.Key;
                        tempTargetAmount = dataEntry.Value;
                        tempCurrentAmount = 0;
                        // Get currency amount from character
                        if (GameInstance.PlayingCharacter != null)
                        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
                            int indexOfCurrency = GameInstance.PlayingCharacter.IndexOfCurrency(tempData.DataId);
                            if (indexOfCurrency >= 0)
                                tempCurrentAmount = GameInstance.PlayingCharacter.Currencies[indexOfCurrency].amount;
#endif
                        }
                        // Use difference format by option 
                        switch (displayType)
                        {
                            case DisplayType.Requirement:
                                // This will show both current character currency amount and target amount
                                tempAmountEnough = tempCurrentAmount >= tempTargetAmount;
                                tempFormat = LanguageManager.GetText(tempAmountEnough ? formatKeyAmount : formatKeyAmountNotEnough);
                                tempCurrentValue = tempCurrentAmount.ToString(numberFormatSimple);
                                tempValue = tempTargetAmount.ToString(numberFormatSimple);
                                if (useSimpleFormatIfAmountEnough && tempAmountEnough)
                                    tempAmountText = ZString.Format(LanguageManager.GetText(formatKeySimpleAmount), tempData.Title, tempValue);
                                else
                                    tempAmountText = ZString.Format(tempFormat, tempData.Title, tempCurrentValue, tempValue);
                                break;
                            default:
                                // This will show only target amount, so current character currency amount will not be shown
                                tempValue = tempTargetAmount.ToString(numberFormatSimple);
                                tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeySimpleAmount),
                                    tempData.Title,
                                    tempValue));
                                break;
                        }
                        // Append current currency amount text
                        if (dataEntry.Value != 0 || !inactiveIfAmountZero)
                        {
                            // Add new line if text is not empty
                            if (tempAllText.Length > 0)
                                tempAllText.Append('\n');
                            tempAllText.Append(tempAmountText);
                        }
                        // Set current currency text to UI
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

        private void SetDefaultValue(UICurrencyTextPair componentPair)
        {
            string zeroFormatSimple = 0f.ToString(numberFormatSimple);
            switch (displayType)
            {
                case DisplayType.Requirement:
                    if (useSimpleFormatIfAmountEnough)
                    {
                        componentPair.uiText.text = ZString.Format(
                            LanguageManager.GetText(formatKeySimpleAmount),
                            componentPair.currency.Title,
                            zeroFormatSimple);
                    }
                    else
                    {
                        componentPair.uiText.text = ZString.Format(
                            LanguageManager.GetText(formatKeyAmount),
                            componentPair.currency.Title,
                            zeroFormatSimple, zeroFormatSimple);
                    }
                    break;
                case DisplayType.Simple:
                    componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                        LanguageManager.GetText(formatKeySimpleAmount),
                        componentPair.currency.Title,
                        zeroFormatSimple));
                    break;
            }
            componentPair.imageIcon.SetImageGameDataIcon(componentPair.currency);
            if (inactiveIfAmountZero && componentPair.root != null)
                componentPair.root.SetActive(false);
        }

        private void UpdateList()
        {
            if (uiEntryPrefab == null || uiListContainer == null)
                return;
            CacheList.HideAll();
            UICharacterCurrency tempUI;
            CacheList.Generate(Data, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UICharacterCurrency>();
                tempUI.Data = new UICharacterCurrencyData(CharacterCurrency.Create(data.Key, Mathf.CeilToInt(data.Value)));
            });
        }
    }
}
