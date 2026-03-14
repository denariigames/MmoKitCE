using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIArmorAmounts : UISelectionEntry<Dictionary<DamageElement, float>>
    {
        public enum DisplayType
        {
            Simple,
            Rate,
        }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Armor Title}, {1} = {Amount}")]
        public UILocaleKeySetting formatKeySimpleAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ARMOR_AMOUNT);
        [Tooltip("Format => {0} = {Armor Title}, {1} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyRateAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ARMOR_RATE);

        [Header("UI Elements")]
        public TextWrapper uiTextAllAmounts;
        public UIArmorTextPair[] textAmounts;

        [Header("List UI Elements")]
        public UIArmorAmount uiEntryPrefab;
        public Transform uiListContainer;

        [Header("Options")]
        public DisplayType displayType;
        public string numberFormatSimple = "N0";
        public string numberFormatRate = "N2";
        public bool isBonus;
        public bool inactiveIfAmountZero;

        private Dictionary<DamageElement, UIArmorTextPair> _cacheTextAmounts;
        public Dictionary<DamageElement, UIArmorTextPair> CacheTextAmounts
        {
            get
            {
                if (_cacheTextAmounts == null)
                {
                    _cacheTextAmounts = new Dictionary<DamageElement, UIArmorTextPair>();
                    DamageElement tempElement;
                    foreach (UIArmorTextPair componentPair in textAmounts)
                    {
                        if (componentPair.uiText == null)
                            continue;
                        tempElement = componentPair.damageElement == null ? GameInstance.Singleton.DefaultDamageElement : componentPair.damageElement;
                        SetDefaultValue(componentPair);
                        _cacheTextAmounts[tempElement] = componentPair;
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
            foreach (UIArmorTextPair entry in CacheTextAmounts.Values)
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
                    DamageElement tempData;
                    float tempAmount;
                    string tempValue;
                    string tempAmountText;
                    UIArmorTextPair tempComponentPair;
                    foreach (KeyValuePair<DamageElement, float> dataEntry in Data)
                    {
                        if (dataEntry.Key == null)
                            continue;
                        // Set temp data
                        tempData = dataEntry.Key;
                        tempAmount = dataEntry.Value;
                        // Use difference format by option
                        switch (displayType)
                        {
                            case DisplayType.Rate:
                                tempValue = (tempAmount * 100).ToString(numberFormatRate);
                                tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeyRateAmount),
                                    tempData.Title,
                                    tempValue));
                                break;
                            default:
                                tempValue = tempAmount.ToString(numberFormatSimple);
                                tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeySimpleAmount),
                                    tempData.Title,
                                    tempValue));
                                break;
                        }
                        // Append current elemental armor text
                        if (dataEntry.Value != 0 || !inactiveIfAmountZero)
                        {
                            // Add new line if text is not empty
                            if (tempAllText.Length > 0)
                                tempAllText.Append('\n');
                            tempAllText.Append(tempAmountText);
                        }
                        // Set current elemental armor text to UI
                        if (CacheTextAmounts.TryGetValue(dataEntry.Key, out tempComponentPair))
                        {
                            tempComponentPair.uiText.text = tempAmountText;
                            if (tempComponentPair.root != null)
                                tempComponentPair.root.SetActive(!inactiveIfAmountZero || tempAmount != 0);
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

        private void SetDefaultValue(UIArmorTextPair componentPair)
        {
            DamageElement tempElement = componentPair.damageElement == null ? GameInstance.Singleton.DefaultDamageElement : componentPair.damageElement;
            string zeroFormatRate = 0f.ToString(numberFormatRate);
            string zeroFormatSimple = 0f.ToString(numberFormatSimple);
            switch (displayType)
            {
                case DisplayType.Rate:
                    componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                        LanguageManager.GetText(formatKeyRateAmount),
                        tempElement.Title,
                        zeroFormatRate));
                    break;
                case DisplayType.Simple:
                    componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                        LanguageManager.GetText(formatKeySimpleAmount),
                        tempElement.Title,
                        zeroFormatSimple));
                    break;
            }
            componentPair.imageIcon.SetImageGameDataIcon(tempElement);
            if (inactiveIfAmountZero && componentPair.root != null)
                componentPair.root.SetActive(false);
        }

        private void UpdateList()
        {
            if (uiEntryPrefab == null || uiListContainer == null)
                return;
            CacheList.HideAll();
            UIArmorAmount tempUI;
            CacheList.Generate(Data, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIArmorAmount>();
                tempUI.Data = new UIArmorAmountData(data.Key, data.Value);
            });
        }
    }
}
