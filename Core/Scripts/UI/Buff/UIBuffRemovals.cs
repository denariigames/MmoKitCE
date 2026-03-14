using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIBuffRemovals : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Level}, {1} = {Chance * 100}")]
        public UILocaleKeySetting formatKeyEntry = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_REMOVAL_ENTRY);
        [Tooltip("Format => {0} = {Status Effect Title}, {1} = {Entries}")]
        public UILocaleKeySetting formatKeyEntries = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_REMOVAL_ENTRIES);
        public string entriesSeparator = ", ";

        [Header("UI Elements")]
        public UIBuffRemoval uiDialog;
        public UIBuffRemoval uiPrefab;
        public Transform uiContainer;
        public TextWrapper uiTextAllEntries;

        [Header("Options")]
        public bool isBonus;

        private UIList _cacheList;
        public UIList CacheList
        {
            get
            {
                if (_cacheList == null)
                {
                    _cacheList = gameObject.AddComponent<UIList>();
                    _cacheList.uiPrefab = uiPrefab.gameObject;
                    _cacheList.uiContainer = uiContainer;
                }
                return _cacheList;
            }
        }

        private UIBuffRemovalSelectionManager _cacheSelectionManager;
        public UIBuffRemovalSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIBuffRemovalSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        private UISelectionManagerShowOnSelectEventManager<UIBuffRemovalData, UIBuffRemoval> _listEventSetupManager = new UISelectionManagerShowOnSelectEventManager<UIBuffRemovalData, UIBuffRemoval>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            uiTextAllEntries = null;
            _cacheList = null;
            _cacheSelectionManager = null;
            _listEventSetupManager = null;
        }

        protected virtual void OnEnable()
        {
            _listEventSetupManager.OnEnable(CacheSelectionManager, uiDialog);
        }

        protected virtual void OnDisable()
        {
            _listEventSetupManager.OnDisable();
        }

        public virtual void UpdateData(Dictionary<BuffRemoval, float> statusEffectResistanceAmounts)
        {
            if (uiPrefab != null && uiContainer != null)
            {
                CacheSelectionManager.DeselectSelectedUI();
                CacheSelectionManager.Clear();
                CacheList.HideAll();
                CacheList.Generate(statusEffectResistanceAmounts, (index, data, ui) =>
                {
                    UIBuffRemoval uiComp = ui.GetComponent<UIBuffRemoval>();
                    uiComp.Data = new UIBuffRemovalData(data.Key, data.Value);
                    uiComp.Show();
                    CacheSelectionManager.Add(uiComp);
                });
            }

            if (statusEffectResistanceAmounts.Count == 0)
            {
                if (uiTextAllEntries != null)
                    uiTextAllEntries.SetGameObjectActive(false);
            }
            else
            {
                using (Utf16ValueStringBuilder tempAllText = ZString.CreateStringBuilder(false))
                {
                    string tempAmountText;
                    foreach (KeyValuePair<BuffRemoval, float> dataEntry in statusEffectResistanceAmounts)
                    {
                        if (dataEntry.Key == null)
                            continue;
                        tempAmountText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                            LanguageManager.GetText(formatKeyEntries),
                            dataEntry.Key.Title,
                            dataEntry.Key.GetChanceEntriesText(dataEntry.Value, LanguageManager.GetText(formatKeyEntry), entriesSeparator)));
                        if (!string.IsNullOrEmpty(tempAmountText))
                        {
                            // Add new line if text is not empty
                            if (tempAllText.Length > 0)
                                tempAllText.Append('\n');
                            tempAllText.Append(tempAmountText);
                        }
                    }

                    if (uiTextAllEntries != null)
                    {
                        uiTextAllEntries.SetGameObjectActive(tempAllText.Length > 0);
                        uiTextAllEntries.text = tempAllText.ToString();
                    }
                }
            }
        }
    }
}
