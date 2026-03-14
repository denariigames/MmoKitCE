using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICharacter : UISelectionEntry<ICharacterData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Character Id}")]
        public UILocaleKeySetting formatKeyId = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Character Name}")]
        public UILocaleKeySetting formatKeyName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_LEVEL);
        [Tooltip("Format => {0} = {Stat Points}")]
        public UILocaleKeySetting formatKeyStatPoint = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STAT_POINTS);
        [Tooltip("Format => {0} = {Skill Points}")]
        public UILocaleKeySetting formatKeySkillPoint = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SKILL_POINTS);
        [Tooltip("Format => {0} = {Battle Points}")]
        public UILocaleKeySetting formatKeyBattlePoint = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BATTLE_POINTS);
        [Tooltip("Format => {0} = {Gold Amount}")]
        public UILocaleKeySetting formatKeyGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);
        [Tooltip("Format => {0} = {Current Total Weights}, {1} = {Weight Limit}")]
        public UILocaleKeySetting formatKeyWeightLimitStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_WEIGHT);
        [Tooltip("Format => {0} = {Current Total Slots}, {1} = {Slot Limit}")]
        public UILocaleKeySetting formatKeySlotLimitStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_SLOT);
        [Tooltip("Format => {0} = {Min Damage}, {1} = {Max Damage}")]
        public UILocaleKeySetting formatKeyWeaponDamage = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_AMOUNT);

        [Header("UI Elements")]
        public TextWrapper uiTextId;
        public TextWrapper uiTextName;
        public TextWrapper uiTextLevel;
        public UIGageValue uiGageExp;
        public UIGageValue uiGageHp;
        public UIGageValue uiGageMp;
        public UIGageValue uiGageStamina;
        public UIGageValue uiGageFood;
        public UIGageValue uiGageWater;
        public TextWrapper uiTextStatPoint;
        public TextWrapper uiTextSkillPoint;
        public TextWrapper uiTextBattlePoint;
        public TextWrapper uiTextGold;
        public TextWrapper uiTextWeightLimit;
        public TextWrapper uiTextSlotLimit;
        [FormerlySerializedAs("uiTextWeaponDamages")]
        public TextWrapper uiTextAllDamages;
        public UIDamageElementAmounts uiRightHandDamages;
        public UIDamageElementAmounts uiLeftHandDamages;
        public UICharacterStats uiCharacterStats;
        public UICharacterBuffs uiCharacterBuffs;
        public UIResistanceAmounts uiCharacterResistances;
        public UIArmorAmounts uiCharacterArmors;
        public UIStatusEffectResistances uiCharacterStatusEffectResistances;
        public UICharacterAttributePair[] uiCharacterAttributes = new UICharacterAttributePair[0];
        public UICharacterCurrencyPair[] uiCharacterCurrencies = new UICharacterCurrencyPair[0];
        public UICharacterClass uiCharacterClass;
        public UIPlayerIcon uiPlayerIcon;
        public UIPlayerFrame uiPlayerFrame;
        public UIPlayerBackground uiPlayerBackground;
        public UIPlayerTitle uiPlayerTitle;
        public UIFaction uiFaction;

        [Header("Options")]
        [Tooltip("If this is `TRUE` it won't update data when controlling character's data changes")]
        public bool notForOwningCharacter;
        [Tooltip("If this is `TRUE` it will show stats which sum with buffs")]
        public bool showStatsWithBuffs;
        [Tooltip("If this is `TRUE` it will show attributes which sum with buffs")]
        public bool showAttributeWithBuffs;
        [Tooltip("If this is `TRUE` it will show resistances which sum with buffs")]
        public bool showResistanceWithBuffs;
        [Tooltip("If this is `TRUE` it will show armors which sum with buffs")]
        public bool showArmorWithBuffs;
        [Tooltip("If this is `TRUE` it will show status effects resistances which sum with buffs")]
        public bool showStatusEffectResistanceWithBuffs;
        [Tooltip("If this is `TRUE` it will show damages which sum with buffs")]
        public bool showDamageWithBuffs;

        // Improve garbage collector
        protected CharacterStats _tempStats;
        protected Dictionary<Attribute, float> _tempAttributes = null;
        protected Dictionary<DamageElement, float> _tempResistances = null;
        protected Dictionary<DamageElement, float> _tempArmors = null;
        protected Dictionary<StatusEffect, float> _tempStatusEffectResistances = null;
        protected Dictionary<DamageElement, MinMaxFloat> _tempRightHandDamages = null;
        protected Dictionary<DamageElement, MinMaxFloat> _tempLeftHandDamages = null;

        public bool NotForOwningCharacter
        {
            get { return notForOwningCharacter; }
            set
            {
                notForOwningCharacter = value;
                RegisterOwningCharacterEvents();
            }
        }

        private Dictionary<Attribute, UICharacterAttribute> _cacheUICharacterAttributes;
        public Dictionary<Attribute, UICharacterAttribute> CacheUICharacterAttributes
        {
            get
            {
                if (_cacheUICharacterAttributes == null)
                {
                    _cacheUICharacterAttributes = new Dictionary<Attribute, UICharacterAttribute>();
                    foreach (UICharacterAttributePair uiCharacterAttribute in uiCharacterAttributes)
                    {
                        if (uiCharacterAttribute.attribute != null &&
                            uiCharacterAttribute.ui != null &&
                            !_cacheUICharacterAttributes.ContainsKey(uiCharacterAttribute.attribute))
                            _cacheUICharacterAttributes.Add(uiCharacterAttribute.attribute, uiCharacterAttribute.ui);
                    }
                }
                return _cacheUICharacterAttributes;
            }
        }

        private Dictionary<Currency, UICharacterCurrency> _cacheUICharacterCurrencies;
        public Dictionary<Currency, UICharacterCurrency> CacheUICharacterCurrencies
        {
            get
            {
                if (_cacheUICharacterCurrencies == null)
                {
                    _cacheUICharacterCurrencies = new Dictionary<Currency, UICharacterCurrency>();
                    foreach (UICharacterCurrencyPair uiCharacterCurrency in uiCharacterCurrencies)
                    {
                        if (uiCharacterCurrency.currency != null &&
                            uiCharacterCurrency.ui != null &&
                            !_cacheUICharacterCurrencies.ContainsKey(uiCharacterCurrency.currency))
                            _cacheUICharacterCurrencies.Add(uiCharacterCurrency.currency, uiCharacterCurrency.ui);
                    }
                }
                return _cacheUICharacterCurrencies;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextId = null;
            uiTextName = null;
            uiTextLevel = null;
            uiGageExp = null;
            uiGageHp = null;
            uiGageMp = null;
            uiGageStamina = null;
            uiGageFood = null;
            uiGageWater = null;
            uiTextStatPoint = null;
            uiTextSkillPoint = null;
            uiTextBattlePoint = null;
            uiTextGold = null;
            uiTextWeightLimit = null;
            uiTextSlotLimit = null;
            uiTextAllDamages = null;
            uiRightHandDamages = null;
            uiLeftHandDamages = null;
            uiCharacterStats = null;
            uiCharacterBuffs = null;
            uiCharacterResistances = null;
            uiCharacterArmors = null;
            uiCharacterStatusEffectResistances = null;
            uiCharacterClass = null;
            uiPlayerIcon = null;
            uiPlayerFrame = null;
            uiPlayerBackground = null;
            uiPlayerTitle = null;
            uiFaction = null;
            _cacheUICharacterAttributes?.Clear();
            _cacheUICharacterAttributes = null;
            _cacheUICharacterCurrencies?.Clear();
            _cacheUICharacterCurrencies = null;
            _data = null;
            CleanTempData();
        }

        protected void CleanTempData()
        {
            _tempStats = new CharacterStats();
            if (_tempAttributes != null)
            {
                CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Release(_tempAttributes);
                _tempAttributes = null;
            }
            if (_tempResistances != null)
            {
                CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(_tempResistances);
                _tempResistances = null;
            }
            if (_tempArmors != null)
            {
                CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(_tempArmors);
                _tempArmors = null;
            }
            if (_tempRightHandDamages != null)
            {
                CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(_tempRightHandDamages);
                _tempRightHandDamages = null;
            }
            if (_tempLeftHandDamages != null)
            {
                CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(_tempLeftHandDamages);
                _tempLeftHandDamages = null;
            }
        }

        protected override void OnEnable()
        {
            UpdateOwningCharacterData();
            RegisterOwningCharacterEvents();
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            UnregisterOwningCharacterEvents();
            base.OnDisable();
        }

        public void RegisterOwningCharacterEvents()
        {
            UnregisterOwningCharacterEvents();
            if (notForOwningCharacter || !GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onRecached += UpdateOwningCharacterData;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            GameInstance.PlayingCharacterEntity.onCurrenciesOperation += OnCurrenciesOperation;
#endif
        }

        public void UnregisterOwningCharacterEvents()
        {
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onRecached -= UpdateOwningCharacterData;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            GameInstance.PlayingCharacterEntity.onCurrenciesOperation -= OnCurrenciesOperation;
#endif
        }

        private void OnCurrenciesOperation(LiteNetLibSyncListOp operation, int index, CharacterCurrency oldItem, CharacterCurrency newItem)
        {
            UpdateOwningCharacterData();
        }

        public void UpdateOwningCharacterData()
        {
            if (notForOwningCharacter || GameInstance.PlayingCharacter == null) return;
            Data = GameInstance.PlayingCharacter;
        }

        public override void ManagedUpdate()
        {
            base.ManagedUpdate();
            CharacterDataCache cache = Data.GetCaches();
            // Hp
            int currentHp = 0;
            int maxHp = 0;
            if (Data != null)
            {
                currentHp = Data.CurrentHp;
                maxHp = cache?.MaxHp ?? 0;
            }
            if (uiGageHp != null)
                uiGageHp.Update(currentHp, maxHp);

            // Mp
            int currentMp = 0;
            int maxMp = 0;
            if (Data != null)
            {
                currentMp = Data.CurrentMp;
                maxMp = cache?.MaxMp ?? 0;
            }
            if (uiGageMp != null)
                uiGageMp.Update(currentMp, maxMp);

            // Stamina
            int currentStamina = 0;
            int maxStamina = 0;
            if (Data != null)
            {
                currentStamina = Data.CurrentStamina;
                maxStamina = cache?.MaxStamina ?? 0;
            }
            if (uiGageStamina != null)
                uiGageStamina.Update(currentStamina, maxStamina);

            // Food
            int currentFood = 0;
            int maxFood = 0;
            if (Data != null)
            {
                currentFood = Data.CurrentFood;
                maxFood = cache?.MaxFood ?? 0;
            }
            if (uiGageFood != null)
                uiGageFood.Update(currentFood, maxFood);

            // Water
            int currentWater = 0;
            int maxWater = 0;
            if (Data != null)
            {
                currentWater = Data.CurrentWater;
                maxWater = cache?.MaxWater ?? 0;
            }
            if (uiGageWater != null)
                uiGageWater.Update(currentWater, maxWater);
        }

        protected override void UpdateUI()
        {
            if (uiTextId != null)
            {
                uiTextId.text = ZString.Format(
                    LanguageManager.GetText(formatKeyId),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.Id);
            }

            if (uiTextName != null)
            {
                uiTextName.text = ZString.Format(
                    LanguageManager.GetText(formatKeyName),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.Title);
            }

            if (uiTextLevel != null)
            {
                uiTextLevel.text = ZString.Format(
                    LanguageManager.GetText(formatKeyLevel),
                    Data == null ? "1" : Data.Level.ToString("N0"));
            }

            Data.GetProperCurrentByNextLevelExp(out int currentExp, out int nextLevelExp);
            if (uiGageExp != null)
                uiGageExp.Update(currentExp, nextLevelExp);

            // Player character data
            IPlayerCharacterData playerCharacter = Data as IPlayerCharacterData;
            if (uiTextStatPoint != null)
            {
                uiTextStatPoint.text = ZString.Format(
                    LanguageManager.GetText(formatKeyStatPoint),
                    playerCharacter == null ? "0" : playerCharacter.StatPoint.ToString("N0"));
            }

            if (uiTextSkillPoint != null)
            {
                uiTextSkillPoint.text = ZString.Format(
                    LanguageManager.GetText(formatKeySkillPoint),
                    playerCharacter == null ? "0" : playerCharacter.SkillPoint.ToString("N0"));
            }

            if (uiTextBattlePoint != null)
            {
                uiTextBattlePoint.text = ZString.Format(
                    LanguageManager.GetText(formatKeyBattlePoint),
                    playerCharacter == null ? "0" : playerCharacter.GetCaches().BattlePoints.ToString("N0"));
            }

            if (uiTextGold != null)
            {
                uiTextGold.text = ZString.Format(
                    LanguageManager.GetText(formatKeyGold),
                    playerCharacter == null ? "0" : playerCharacter.Gold.ToString("N0"));
            }

            // Icon
            if (uiPlayerIcon != null)
            {
                if (playerCharacter != null && GameInstance.PlayerIcons.ContainsKey(playerCharacter.IconDataId))
                    uiPlayerIcon.SetDataByDataId(playerCharacter.IconDataId);
                else
                    uiPlayerIcon.SetDataByDataId(GameInstance.PlayerIcons.Keys.FirstOrDefault());
                uiPlayerIcon.SetVisible(true);
            }

            // Frame
            if (uiPlayerFrame != null)
            {
                if (playerCharacter != null && GameInstance.PlayerFrames.ContainsKey(playerCharacter.FrameDataId))
                    uiPlayerFrame.SetDataByDataId(playerCharacter.FrameDataId);
                else
                    uiPlayerFrame.SetDataByDataId(GameInstance.PlayerFrames.Keys.FirstOrDefault());
                uiPlayerFrame.SetVisible(true);
            }

            // Background
            if (uiPlayerBackground != null)
            {
                if (playerCharacter != null && GameInstance.PlayerBackgrounds.ContainsKey(playerCharacter.BackgroundDataId))
                    uiPlayerBackground.SetDataByDataId(playerCharacter.BackgroundDataId);
                else
                    uiPlayerBackground.SetDataByDataId(GameInstance.PlayerBackgrounds.Keys.FirstOrDefault());
                uiPlayerBackground.SetVisible(true);
            }

            // Title
            if (uiPlayerTitle != null)
            {
                if (playerCharacter != null && GameInstance.PlayerTitles.ContainsKey(playerCharacter.TitleDataId))
                    uiPlayerTitle.SetDataByDataId(playerCharacter.TitleDataId);
                else
                    uiPlayerTitle.SetDataByDataId(GameInstance.PlayerTitles.Keys.FirstOrDefault());
                uiPlayerTitle.SetVisible(true);
            }

            // Faction
            if (uiFaction != null)
            {
                if (playerCharacter != null)
                    uiFaction.SetDataByDataId(playerCharacter.FactionId);
                uiFaction.SetVisible(playerCharacter != null);
            }
        }

        protected override void UpdateData()
        {
            IPlayerCharacterData playerCharacter = Data as IPlayerCharacterData;

            CleanTempData();
            Data.GetAllStats(true, showStatsWithBuffs, true,
                onGetStats: stats => _tempStats = stats);
            Data.GetAllStats(true, showAttributeWithBuffs, true,
                onGetAttributes: stats => _tempAttributes = stats,
                willReleaseAttributes: false);
            Data.GetAllStats(true, showResistanceWithBuffs, true,
                onGetResistances: stats => _tempResistances = stats,
                willReleaseResistances: false);
            Data.GetAllStats(true, showArmorWithBuffs, true,
                onGetArmors: stats => _tempArmors = stats,
                willReleaseArmors: false);
            Data.GetAllStats(true, showStatusEffectResistanceWithBuffs, true,
                onGetStatusEffectResistances: stats => _tempStatusEffectResistances = stats,
                willReleaseStatusEffectResistances: false);
            Data.GetAllStats(true, showDamageWithBuffs, true,
                onGetRightHandDamages: stats => _tempRightHandDamages = stats,
                onGetLeftHandDamages: stats => _tempLeftHandDamages = stats,
                willReleaseRightHandDamages: false,
                willReleaseLeftHandDamages: false);

            if (uiTextWeightLimit != null)
            {
                uiTextWeightLimit.text = ZString.Format(
                    LanguageManager.GetText(formatKeyWeightLimitStats),
                    Data.GetCaches().TotalItemWeight.ToString("N2"),
                    Data.GetCaches().LimitItemWeight.ToString("N2"));
            }

            if (uiTextSlotLimit != null)
            {
                uiTextSlotLimit.text = ZString.Format(
                    LanguageManager.GetText(formatKeySlotLimitStats),
                    Data.GetCaches().TotalItemSlot.ToString("N0"),
                    Data.GetCaches().LimitItemSlot.ToString("N0"));
            }

            if (uiTextAllDamages != null)
            {
                using (Utf16ValueStringBuilder textDamages = ZString.CreateStringBuilder(false))
                {
                    if (_tempRightHandDamages != null)
                    {
                        MinMaxFloat sumDamages = GameDataHelpers.GetSumDamages(_tempRightHandDamages);
                        if (textDamages.Length > 0)
                            textDamages.Append('\n');
                        textDamages.AppendFormat(
                            LanguageManager.GetText(formatKeyWeaponDamage),
                            sumDamages.min.ToString("N0"),
                            sumDamages.max.ToString("N0"));
                    }
                    if (_tempLeftHandDamages != null)
                    {
                        MinMaxFloat sumDamages = GameDataHelpers.GetSumDamages(_tempLeftHandDamages);
                        if (textDamages.Length > 0)
                            textDamages.Append('\n');
                        textDamages.AppendFormat(
                            LanguageManager.GetText(formatKeyWeaponDamage),
                            sumDamages.min.ToString("N0"),
                            sumDamages.max.ToString("N0"));
                    }
                    uiTextAllDamages.text = textDamages.ToString();
                }
            }

            if (uiRightHandDamages != null)
            {
                if (_tempRightHandDamages == null)
                {
                    uiRightHandDamages.Hide();
                }
                else
                {
                    uiRightHandDamages.isBonus = false;
                    uiRightHandDamages.Show();
                    uiRightHandDamages.Data = _tempRightHandDamages;
                }
            }

            if (uiLeftHandDamages != null)
            {
                if (_tempLeftHandDamages == null)
                {
                    uiLeftHandDamages.Hide();
                }
                else
                {
                    uiLeftHandDamages.isBonus = false;
                    uiLeftHandDamages.Show();
                    uiLeftHandDamages.Data = _tempLeftHandDamages;
                }
            }

            if (uiCharacterStats != null)
            {
                uiCharacterStats.displayType = UICharacterStats.DisplayType.Simple;
                uiCharacterStats.isBonus = false;
                uiCharacterStats.Data = _tempStats;
            }

            if (uiCharacterResistances != null)
            {
                uiCharacterResistances.isBonus = false;
                uiCharacterResistances.Data = _tempResistances;
            }

            if (uiCharacterArmors != null)
            {
                uiCharacterArmors.isBonus = false;
                uiCharacterArmors.Data = _tempArmors;
            }

            if (uiCharacterStatusEffectResistances != null)
            {
                uiCharacterStatusEffectResistances.isBonus = false;
                uiCharacterStatusEffectResistances.UpdateData(_tempStatusEffectResistances);
            }

            if (CacheUICharacterAttributes.Count > 0 && Data != null)
            {
                int tempIndexOfAttribute;
                CharacterAttribute tempCharacterAttribute;
                float tempAmount;
                foreach (Attribute attribute in CacheUICharacterAttributes.Keys)
                {
                    tempIndexOfAttribute = Data.IndexOfAttribute(attribute.DataId);
                    tempCharacterAttribute = tempIndexOfAttribute >= 0 ? Data.Attributes[tempIndexOfAttribute] : CharacterAttribute.Create(attribute, 0);
                    tempAmount = 0;
                    if (_tempAttributes.ContainsKey(attribute))
                        tempAmount = _tempAttributes[attribute];
                    CacheUICharacterAttributes[attribute].Setup(new UICharacterAttributeData(tempCharacterAttribute, tempAmount), Data, tempIndexOfAttribute);
                    CacheUICharacterAttributes[attribute].Show();
                }
            }

#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            if (CacheUICharacterCurrencies.Count > 0 && playerCharacter != null)
            {
                int tempIndexOfCurrency;
                CharacterCurrency tempCharacterCurrency;
                foreach (Currency currency in CacheUICharacterCurrencies.Keys)
                {
                    tempIndexOfCurrency = playerCharacter.IndexOfCurrency(currency.DataId);
                    tempCharacterCurrency = tempIndexOfCurrency >= 0 ? playerCharacter.Currencies[tempIndexOfCurrency] : CharacterCurrency.Create(currency, 0);
                    CacheUICharacterCurrencies[currency].Setup(new UICharacterCurrencyData(tempCharacterCurrency, tempCharacterCurrency.amount), Data, tempIndexOfCurrency);
                    CacheUICharacterCurrencies[currency].Show();
                }
            }
#endif

            if (uiCharacterBuffs != null)
                uiCharacterBuffs.UpdateData(Data);

            BaseCharacter character = Data == null ? null : Data.GetDatabase();
            if (uiCharacterClass != null)
                uiCharacterClass.Data = character;
        }
    }
}
