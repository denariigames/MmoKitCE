using Cysharp.Text;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIStatusEffectApplyings : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Status Effect Title}, {1} = {Level}")]
        public UILocaleKeySetting formatApplyingTargetSelf = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_SELF);
        [Tooltip("Format => {0} = {Status Effect Title}, {1} = {Level}")]
        public UILocaleKeySetting formatApplyingTargetEnemy = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_ENEMY);
        [Tooltip("Format => {0} = {Status Effect Title}, {1} = {Level}")]
        public UILocaleKeySetting formatApplyingTargetSelfWhenAttacking = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_SELF_WHEN_ATTACKING);
        [Tooltip("Format => {0} = {Status Effect Title}, {1} = {Level}")]
        public UILocaleKeySetting formatApplyingTargetEnemyWhenAttacking = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_ENEMY_WHEN_ATTACKING);
        [Tooltip("Format => {0} = {Status Effect Title}, {1} = {Level}")]
        public UILocaleKeySetting formatApplyingTargetSelfWhenAttacked = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_SELF_WHEN_ATTACKED);
        [Tooltip("Format => {0} = {Status Effect Title}, {1} = {Level}")]
        public UILocaleKeySetting formatApplyingTargetEnemyWhenAttacked = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_ENEMY_WHEN_ATTACKED);

        public UIStatusEffectApplying uiDialog;
        public UIStatusEffectApplying uiPrefab;
        public Transform uiContainer;
        public TextWrapper uiTextAllEntries;

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

        private UIStatusEffectApplyingSelectionManager _cacheSelectionManager;
        public UIStatusEffectApplyingSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIStatusEffectApplyingSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        private UISelectionManagerShowOnSelectEventManager<UIStatusEffectApplyingData, UIStatusEffectApplying> _listEventSetupManager = new UISelectionManagerShowOnSelectEventManager<UIStatusEffectApplyingData, UIStatusEffectApplying>();

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

        public virtual void UpdateData(IList<StatusEffectApplying> statusEffectApplyings, int level, UIStatusEffectApplyingTarget target)
        {
            if (uiPrefab != null && uiContainer != null)
            {
                CacheSelectionManager.DeselectSelectedUI();
                CacheSelectionManager.Clear();
                CacheList.HideAll();
                CacheList.Generate(statusEffectApplyings, (index, data, ui) =>
                {
                    UIStatusEffectApplying uiComp = ui.GetComponent<UIStatusEffectApplying>();
                    uiComp.Data = new UIStatusEffectApplyingData(data, level, target);
                    uiComp.Show();
                    CacheSelectionManager.Add(uiComp);
                });
            }

            if (statusEffectApplyings.Count == 0)
            {
                if (uiTextAllEntries != null)
                    uiTextAllEntries.SetGameObjectActive(false);
            }
            else
            {
                using (Utf16ValueStringBuilder tempAllText = ZString.CreateStringBuilder(false))
                {
                    string tempAmountText;
                    foreach (StatusEffectApplying dataEntry in statusEffectApplyings)
                    {
                        if (dataEntry.statusEffect == null)
                            continue;
                        tempAmountText = null;
                        switch (target)
                        {
                            case UIStatusEffectApplyingTarget.Self:
                                tempAmountText = ZString.Format(
                                    LanguageManager.GetText(formatApplyingTargetSelf),
                                    dataEntry.statusEffect.Title,
                                    level.ToString("N0"));
                                break;
                            case UIStatusEffectApplyingTarget.Enemy:
                                tempAmountText = ZString.Format(
                                    LanguageManager.GetText(formatApplyingTargetEnemy),
                                    dataEntry.statusEffect.Title,
                                    level.ToString("N0"));
                                break;
                            case UIStatusEffectApplyingTarget.SelfWhenAttacking:
                                tempAmountText = ZString.Format(
                                    LanguageManager.GetText(formatApplyingTargetSelfWhenAttacking),
                                    dataEntry.statusEffect.Title,
                                    level.ToString("N0"));
                                break;
                            case UIStatusEffectApplyingTarget.EnemyWhenAttacking:
                                tempAmountText = ZString.Format(
                                    LanguageManager.GetText(formatApplyingTargetEnemyWhenAttacking),
                                    dataEntry.statusEffect.Title,
                                    level.ToString("N0"));
                                break;
                            case UIStatusEffectApplyingTarget.SelfWhenAttacked:
                                tempAmountText = ZString.Format(
                                    LanguageManager.GetText(formatApplyingTargetSelfWhenAttacked),
                                    dataEntry.statusEffect.Title,
                                    level.ToString("N0"));
                                break;
                            case UIStatusEffectApplyingTarget.EnemyWhenAttacked:
                                tempAmountText = ZString.Format(
                                    LanguageManager.GetText(formatApplyingTargetEnemyWhenAttacked),
                                    dataEntry.statusEffect.Title,
                                    level.ToString("N0"));
                                break;
                        }
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