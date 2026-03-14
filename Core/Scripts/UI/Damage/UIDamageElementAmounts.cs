using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIDamageElementAmounts : UISelectionEntry<Dictionary<DamageElement, MinMaxFloat>>
    {
        public enum DisplayType
        {
            Simple,
            Rate,
        }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Min Damage}, {2} = {Max Damage}")]
        public UILocaleKeySetting formatKeyDamage = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_WITH_ELEMENTAL);
        [Tooltip("Format => {0} = {Min Damage}, {1} = {Max Damage}")]
        public UILocaleKeySetting formatKeySumDamage = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_AMOUNT);
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Min Damage * 100}, {2} = {Max Damage * 100}")]
        public UILocaleKeySetting formatKeyDamageRate = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_WITH_ELEMENTAL_RATE);
        [Tooltip("Format => {0} = {Min Damage * 100}, {1} = {Max Damage * 100}")]
        public UILocaleKeySetting formatKeySumDamageRate = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_RATE);

        [Header("UI Elements")]
        public TextWrapper uiTextAllDamages;
        public TextWrapper uiTextSumDamage;
        public UIDamageElementTextPair[] textDamages;

        [Header("List UI Elements")]
        public UIDamageElementAmount uiEntryPrefab;
        public Transform uiListContainer;

        [Header("Options")]
        public DisplayType displayType;
        public string numberFormatSimple = "N0";
        public string numberFormatRate = "N2";
        public bool isBonus;
        public bool inactiveIfAmountZero;

        private Dictionary<DamageElement, UIDamageElementTextPair> _cacheTextDamages;
        public Dictionary<DamageElement, UIDamageElementTextPair> CacheTextDamages
        {
            get
            {
                if (_cacheTextDamages == null)
                {
                    _cacheTextDamages = new Dictionary<DamageElement, UIDamageElementTextPair>();
                    DamageElement tempElement;
                    foreach (UIDamageElementTextPair componentPair in textDamages)
                    {
                        if (componentPair.uiText == null)
                            continue;
                        tempElement = componentPair.damageElement == null ? GameInstance.Singleton.DefaultDamageElement : componentPair.damageElement;
                        SetDefaultValue(componentPair);
                        _cacheTextDamages[tempElement] = componentPair;
                    }
                }
                return _cacheTextDamages;
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
            uiTextAllDamages = null;
            uiTextSumDamage = null;
            textDamages = null;
            uiEntryPrefab = null;
            uiListContainer = null;
            _cacheTextDamages?.Clear();
            _cacheList = null;
            _data?.Clear();
        }

        protected override void UpdateData()
        {
            string zeroFormatRate = 0f.ToString(numberFormatRate);
            string zeroFormatSimple = 0f.ToString(numberFormatSimple);
            // Reset number
            if (uiTextSumDamage != null)
            {
                switch (displayType)
                {
                    case DisplayType.Rate:
                        uiTextSumDamage.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                            LanguageManager.GetText(formatKeySumDamageRate),
                            zeroFormatRate,
                            zeroFormatRate));
                        break;
                    case DisplayType.Simple:
                        uiTextSumDamage.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                            LanguageManager.GetText(formatKeySumDamage),
                            zeroFormatSimple,
                            zeroFormatSimple));
                        break;
                }
            }
            foreach (UIDamageElementTextPair entry in CacheTextDamages.Values)
            {
                SetDefaultValue(entry);
            }
            // Set number by updated data
            if (Data == null || Data.Count == 0)
            {
                if (uiTextAllDamages != null)
                    uiTextAllDamages.SetGameObjectActive(false);
            }
            else
            {
                using (Utf16ValueStringBuilder tempAllText = ZString.CreateStringBuilder(false))
                {
                    MinMaxFloat sumDamage = new MinMaxFloat();
                    DamageElement tempElement;
                    MinMaxFloat tempAmount;
                    string tempMinValue;
                    string tempMaxValue;
                    string tempAmountText;
                    UIDamageElementTextPair tempComponentPair;
                    foreach (KeyValuePair<DamageElement, MinMaxFloat> dataEntry in Data)
                    {
                        if (dataEntry.Key == null)
                            continue;
                        // Set temp data
                        tempElement = dataEntry.Key;
                        tempAmount = dataEntry.Value;
                        // Use difference format by option
                        switch (displayType)
                        {
                            case DisplayType.Rate:
                                tempMinValue = (tempAmount.min * 100).ToString(numberFormatRate);
                                tempMaxValue = (tempAmount.max * 100).ToString(numberFormatRate);
                                tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeyDamageRate),
                                    tempElement.Title,
                                    tempMinValue,
                                    tempMaxValue));
                                break;
                            default:
                                tempMinValue = tempAmount.min.ToString(numberFormatSimple);
                                tempMaxValue = tempAmount.max.ToString(numberFormatSimple);
                                tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeyDamage),
                                    tempElement.Title,
                                    tempMinValue,
                                    tempMaxValue));
                                break;
                        }
                        // Append current elemental damage text
                        if (dataEntry.Value.min != 0 || dataEntry.Value.max != 0)
                        {
                            // Add new line if text is not empty
                            if (tempAllText.Length > 0)
                                tempAllText.Append('\n');
                            tempAllText.Append(tempAmountText);
                        }
                        // Set current elemental damage text to UI
                        if (CacheTextDamages.TryGetValue(dataEntry.Key, out tempComponentPair))
                        {
                            tempComponentPair.uiText.text = tempAmountText;
                            if (tempComponentPair.root != null)
                                tempComponentPair.root.SetActive(!inactiveIfAmountZero || (tempAmount.min != 0 && tempAmount.max != 0));
                        }
                        sumDamage += tempAmount;
                    }

                    if (uiTextAllDamages != null)
                    {
                        uiTextAllDamages.SetGameObjectActive(tempAllText.Length > 0);
                        uiTextAllDamages.text = tempAllText.ToString();
                    }

                    if (uiTextSumDamage != null)
                    {
                        switch (displayType)
                        {
                            case DisplayType.Rate:
                                tempMinValue = sumDamage.min.ToString(numberFormatRate);
                                tempMaxValue = sumDamage.max.ToString(numberFormatRate);
                                uiTextSumDamage.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeySumDamageRate),
                                    tempMinValue,
                                    tempMaxValue));
                                break;
                            default:
                                tempMinValue = sumDamage.min.ToString(numberFormatSimple);
                                tempMaxValue = sumDamage.max.ToString(numberFormatSimple);
                                uiTextSumDamage.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeySumDamage),
                                    tempMinValue,
                                    tempMaxValue));
                                break;
                        }
                    }
                }
            }
            UpdateList();
        }

        private void SetDefaultValue(UIDamageElementTextPair componentPair)
        {
            string zeroFormatRate = 0f.ToString(numberFormatRate);
            string zeroFormatSimple = 0f.ToString(numberFormatSimple);
            DamageElement tempElement = componentPair.damageElement == null ? GameInstance.Singleton.DefaultDamageElement : componentPair.damageElement;
            switch (displayType)
            {
                case DisplayType.Rate:
                    componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                        LanguageManager.GetText(formatKeyDamageRate),
                        tempElement.Title,
                        zeroFormatRate,
                        zeroFormatRate));
                    break;
                case DisplayType.Simple:
                    componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                        LanguageManager.GetText(formatKeyDamage),
                        tempElement.Title,
                        zeroFormatSimple,
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
            UIDamageElementAmount tempUI;
            CacheList.Generate(Data, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIDamageElementAmount>();
                tempUI.Data = new UIDamageElementAmountData(data.Key, data.Value);
            });
        }
    }
}
