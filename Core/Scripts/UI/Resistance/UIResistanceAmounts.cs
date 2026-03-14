using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIResistanceAmounts : UISelectionEntry<Dictionary<DamageElement, float>>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Resistance Title}, {1} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RESISTANCE_AMOUNT);

        [Header("UI Elements")]
        public TextWrapper uiTextAllAmounts;
        public UIResistanceTextPair[] textAmounts;

        [Header("List UI Elements")]
        public UIResistanceAmount uiEntryPrefab;
        public Transform uiListContainer;

        [Header("Options")]
        public string numberFormatRate = "N2";
        public bool isBonus;

        private Dictionary<DamageElement, UIResistanceTextPair> _cacheTextAmounts;
        public Dictionary<DamageElement, UIResistanceTextPair> CacheTextAmounts
        {
            get
            {
                if (_cacheTextAmounts == null)
                {
                    _cacheTextAmounts = new Dictionary<DamageElement, UIResistanceTextPair>();
                    DamageElement tempElement;
                    foreach (UIResistanceTextPair componentPair in textAmounts)
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
            foreach (UIResistanceTextPair entry in CacheTextAmounts.Values)
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
                    UIResistanceTextPair tempComponentPair;
                    foreach (KeyValuePair<DamageElement, float> dataEntry in Data)
                    {
                        if (dataEntry.Key == null)
                            continue;
                        // Set temp data
                        tempData = dataEntry.Key;
                        tempAmount = dataEntry.Value;
                        // Set current elemental resistance text
                        tempValue = (tempAmount * 100).ToString(numberFormatRate);
                        tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                            LanguageManager.GetText(formatKeyAmount),
                            tempData.Title,
                            tempValue));
                        // Append current elemental resistance text
                        if (dataEntry.Value != 0)
                        {
                            // Add new line if text is not empty
                            if (tempAllText.Length > 0)
                                tempAllText.Append('\n');
                            tempAllText.Append(tempAmountText);
                        }
                        // Set current elemental resistance text to UI
                        if (CacheTextAmounts.TryGetValue(dataEntry.Key, out tempComponentPair))
                            tempComponentPair.uiText.text = tempAmountText;
                    }

                    if (uiTextAllAmounts != null)
                    {
                        uiTextAllAmounts.SetGameObjectActive(tempAllText.Length > 0);
                        uiTextAllAmounts.text = tempAllText.ToString();
                    }
                }
            }
            UpdateList();
        }

        private void SetDefaultValue(UIResistanceTextPair componentPair)
        {
            DamageElement tempElement = componentPair.damageElement == null ? GameInstance.Singleton.DefaultDamageElement : componentPair.damageElement;
            string zeroFormatRate = 0f.ToString(numberFormatRate);
            componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                LanguageManager.GetText(formatKeyAmount),
                tempElement.Title,
                zeroFormatRate));
            componentPair.imageIcon.SetImageGameDataIcon(tempElement);
        }

        private void UpdateList()
        {
            if (uiEntryPrefab == null || uiListContainer == null)
                return;
            CacheList.HideAll();
            UIResistanceAmount tempUI;
            CacheList.Generate(Data, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIResistanceAmount>();
                tempUI.Data = new UIResistanceAmountData(data.Key, data.Value);
            });
        }
    }
}
