using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIDamageElementInflictions : UISelectionEntry<Dictionary<DamageElement, float>>
    {
        [Header("String Formats")]
        [Tooltip("Format => {1} = {Infliction * 100}")]
        public UILocaleKeySetting formatKeyInfliction = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_INFLICTION);
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Infliction * 100}")]
        public UILocaleKeySetting formatKeyInflictionAsElemental = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_INFLICTION_AS_ELEMENTAL);

        [Header("UI Elements")]
        public TextWrapper uiTextAllInflictions;
        public UIDamageElementTextPair[] textInflictions;

        [Header("Options")]
        public bool inactiveIfAmountZero;

        private Dictionary<DamageElement, UIDamageElementTextPair> _cacheTextInflictions;
        public Dictionary<DamageElement, UIDamageElementTextPair> CacheTextInflictions
        {
            get
            {
                if (_cacheTextInflictions == null)
                {
                    _cacheTextInflictions = new Dictionary<DamageElement, UIDamageElementTextPair>();
                    DamageElement tempElement;
                    foreach (UIDamageElementTextPair componentPair in textInflictions)
                    {
                        if (componentPair.uiText == null)
                            continue;
                        tempElement = componentPair.damageElement == null ? GameInstance.Singleton.DefaultDamageElement : componentPair.damageElement;
                        SetDefaultValue(componentPair);
                        _cacheTextInflictions[tempElement] = componentPair;
                    }
                }
                return _cacheTextInflictions;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextAllInflictions = null;
            textInflictions = null;
            _cacheTextInflictions?.Clear();
            _data?.Clear();
        }

        protected override void UpdateData()
        {
            // Reset number
            foreach (UIDamageElementTextPair entry in CacheTextInflictions.Values)
            {
                SetDefaultValue(entry);
            }
            // Set number by updated data
            if (Data == null || Data.Count == 0)
            {
                if (uiTextAllInflictions != null)
                    uiTextAllInflictions.SetGameObjectActive(false);
            }
            else
            {
                using (Utf16ValueStringBuilder tempAllText = ZString.CreateStringBuilder(false))
                {
                    DamageElement tempElement;
                    float tempInfliction;
                    string tempAmountText;
                    UIDamageElementTextPair tempComponentPair;
                    foreach (KeyValuePair<DamageElement, float> dataEntry in Data)
                    {
                        if (dataEntry.Key == null)
                            continue;
                        // Set temp data
                        tempElement = dataEntry.Key;
                        tempInfliction = dataEntry.Value;
                        // Set current elemental damage infliction text
                        if (tempElement == null || tempElement == GameInstance.Singleton.DefaultDamageElement)
                        {
                            tempAmountText = ZString.Format(
                                LanguageManager.GetText(formatKeyInfliction),
                                (tempInfliction * 100f).ToString("N0"));
                        }
                        else
                        {
                            tempAmountText = ZString.Format(
                                LanguageManager.GetText(formatKeyInflictionAsElemental),
                                tempElement.Title,
                                (tempInfliction * 100f).ToString("N0"));
                        }
                        // Append current elemental damage infliction text
                        if (dataEntry.Value != 0 || !inactiveIfAmountZero)
                        {
                            // Add new line if text is not empty
                            if (tempAllText.Length > 0)
                                tempAllText.Append('\n');
                            tempAllText.Append(tempAmountText);
                        }
                        // Set current elemental damage infliction text to UI
                        if (CacheTextInflictions.TryGetValue(dataEntry.Key, out tempComponentPair))
                        {
                            tempComponentPair.uiText.text = tempAmountText;
                            if (tempComponentPair.root != null)
                                tempComponentPair.root.SetActive(!inactiveIfAmountZero || tempInfliction != 0);
                        }
                    }

                    if (uiTextAllInflictions != null)
                    {
                        uiTextAllInflictions.SetGameObjectActive(tempAllText.Length > 0 || !inactiveIfAmountZero);
                        uiTextAllInflictions.text = tempAllText.ToString();
                    }
                }
            }
        }

        private void SetDefaultValue(UIDamageElementTextPair componentPair)
        {
            DamageElement tempElement = componentPair.damageElement == null ? GameInstance.Singleton.DefaultDamageElement : componentPair.damageElement;
            if (tempElement == null || tempElement == GameInstance.Singleton.DefaultDamageElement)
            {
                componentPair.uiText.text = ZString.Format(
                    LanguageManager.GetText(formatKeyInfliction),
                    "0");
            }
            else
            {
                componentPair.uiText.text = ZString.Format(
                    LanguageManager.GetText(formatKeyInflictionAsElemental),
                    tempElement.Title,
                    "0");
            }
            componentPair.imageIcon.SetImageGameDataIcon(tempElement);
            if (inactiveIfAmountZero && componentPair.root != null)
                componentPair.root.SetActive(false);
        }
    }
}
