using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    public partial class UIAttributeAmounts : UISelectionEntry<Dictionary<Attribute, float>>
    {
        public enum DisplayType
        {
            Simple,
            Rate,
            Requirement
        }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Attribute Title}, {1} = {Current Amount}, {2} = {Target Amount}")]
        public UILocaleKeySetting formatKeyAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_ATTRIBUTE);
        [Tooltip("Format => {0} = {Attribute Title}, {1} = {Current Amount}, {2} = {Target Amount}")]
        public UILocaleKeySetting formatKeyAmountNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_ATTRIBUTE_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Attribute Title}, {1} = {Amount}")]
        public UILocaleKeySetting formatKeySimpleAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ATTRIBUTE_AMOUNT);
        [Tooltip("Format => {0} = {Attribute Title}, {1} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyRateAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ATTRIBUTE_RATE);

        [Header("UI Elements")]
        public TextWrapper uiTextAllAmounts;
        public UIAttributeTextPair[] textAmounts;

        [Header("List UI Elements")]
        public UICharacterAttribute uiEntryPrefab;
        public Transform uiListContainer;

        [Header("Options")]
        public DisplayType displayType;
        public string numberFormatSimple = "N0";
        public string numberFormatRate = "N2";
        public bool includeEquipmentsForCurrentAmounts;
        public bool includeBuffsForCurrentAmounts;
        public bool includeSkillsForCurrentAmounts;
        public bool isBonus;
        public bool inactiveIfAmountZero;
        public bool useSimpleFormatIfAmountEnough = true;

        private Dictionary<Attribute, UIAttributeTextPair> _cacheTextAmounts;
        public Dictionary<Attribute, UIAttributeTextPair> CacheTextAmounts
        {
            get
            {
                if (_cacheTextAmounts == null)
                {
                    _cacheTextAmounts = new Dictionary<Attribute, UIAttributeTextPair>();
                    Attribute tempData;
                    foreach (UIAttributeTextPair componentPair in textAmounts)
                    {
                        if (componentPair.attribute == null || componentPair.uiText == null)
                            continue;
                        tempData = componentPair.attribute;
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

        protected Dictionary<Attribute, float> _tempAttributes;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextAllAmounts = null;
            textAmounts = null;
            uiEntryPrefab = null;
            uiListContainer = null;
            _cacheTextAmounts?.Clear();
            _cacheTextAmounts = null;
            _cacheList = null;
            _data?.Clear();
            _data = null;
            CleanTempData();
        }

        protected void CleanTempData()
        {
            if (_tempAttributes != null)
            {
                CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Release(_tempAttributes);
                _tempAttributes = null;
            }
        }

        protected override void UpdateData()
        {
            CleanTempData();
            // Reset number
            foreach (UIAttributeTextPair entry in CacheTextAmounts.Values)
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
                // Prepare attribute data
                IPlayerCharacterData character = GameInstance.PlayingCharacter;
                if (character != null)
                    character.GetAllStats(includeEquipmentsForCurrentAmounts, includeBuffsForCurrentAmounts, includeSkillsForCurrentAmounts, 
                        onGetAttributes: attributes => _tempAttributes = attributes,
                        willReleaseAttributes: false);
                // In-loop temp data
                using (Utf16ValueStringBuilder tempAllText = ZString.CreateStringBuilder(false))
                {
                    Attribute tempData;
                    float tempCurrentAmount;
                    float tempTargetAmount;
                    bool tempAmountEnough;
                    string tempCurrentValue;
                    string tempValue;
                    string tempFormat;
                    string tempAmountText;
                    UIAttributeTextPair tempComponentPair;
                    foreach (KeyValuePair<Attribute, float> dataEntry in Data)
                    {
                        if (dataEntry.Key == null)
                            continue;
                        // Set temp data
                        tempData = dataEntry.Key;
                        tempTargetAmount = dataEntry.Value;
                        tempCurrentAmount = 0;
                        // Get attribute amount
                        _tempAttributes.TryGetValue(tempData, out tempCurrentAmount);
                        // Use difference format by option
                        switch (displayType)
                        {
                            case DisplayType.Rate:
                                // This will show only target amount, so current character attribute amount will not be shown
                                tempValue = (tempTargetAmount * 100).ToString(numberFormatRate);
                                tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeyRateAmount),
                                    tempData.Title,
                                    tempValue));
                                break;
                            case DisplayType.Requirement:
                                // This will show both current character attribute amount and target amount
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
                                tempValue = tempTargetAmount.ToString(numberFormatSimple);
                                tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeySimpleAmount),
                                    tempData.Title,
                                    tempValue));
                                break;
                        }
                        // Append current attribute amount text
                        if (dataEntry.Value != 0 || !inactiveIfAmountZero)
                        {
                            // Add new line if text is not empty
                            if (tempAllText.Length > 0)
                                tempAllText.Append('\n');
                            tempAllText.Append(tempAmountText);
                        }
                        // Set current attribute text to UI
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

        private void SetDefaultValue(UIAttributeTextPair componentPair)
        {
            string zeroFormatRate = 0f.ToString(numberFormatRate);
            string zeroFormatSimple = 0f.ToString(numberFormatSimple);
            switch (displayType)
            {
                case DisplayType.Rate:
                    componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                        LanguageManager.GetText(formatKeyRateAmount),
                        componentPair.attribute.Title,
                        zeroFormatRate));
                    break;
                case DisplayType.Requirement:
                    if (useSimpleFormatIfAmountEnough)
                    {
                        componentPair.uiText.text = ZString.Format(
                            LanguageManager.GetText(formatKeySimpleAmount),
                            componentPair.attribute.Title,
                            zeroFormatSimple);
                    }
                    else
                    {
                        componentPair.uiText.text = ZString.Format(
                            LanguageManager.GetText(formatKeyAmount),
                            componentPair.attribute.Title,
                            zeroFormatSimple, zeroFormatSimple);
                    }
                    break;
                case DisplayType.Simple:
                    componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                        LanguageManager.GetText(formatKeySimpleAmount),
                        componentPair.attribute.Title,
                        zeroFormatSimple));
                    break;
            }
            componentPair.imageIcon.SetImageGameDataIcon(componentPair.attribute);
            if (inactiveIfAmountZero && componentPair.root != null)
                componentPair.root.SetActive(false);
        }

        private void UpdateList()
        {
            if (uiEntryPrefab == null || uiListContainer == null)
                return;
            CacheList.HideAll();
            UICharacterAttribute tempUI;
            CacheList.Generate(Data, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UICharacterAttribute>();
                tempUI.Data = new UICharacterAttributeData(CharacterAttribute.Create(data.Key, Mathf.CeilToInt(data.Value)));
            });
        }
    }
}
