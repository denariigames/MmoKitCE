using Cysharp.Text;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UISkillRequirement : UISelectionEntry<UICharacterSkillData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Require Level}")]
        public UILocaleKeySetting formatKeyRequireLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_LEVEL);
        [Tooltip("Format => {0} = {Current Level}, {1} = {Require Level}")]
        public UILocaleKeySetting formatKeyRequireLevelNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_LEVEL_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Require Skill Point}")]
        public UILocaleKeySetting formatKeyRequireSkillPoint = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_SKILL_POINT);
        [Tooltip("Format => {0} = {Current Skill Point}, {1} = {Require Skill Point}")]
        public UILocaleKeySetting formatKeyRequireSkillPointNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_SKILL_POINT_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD);
        [Tooltip("Format => {0} = {Current Gold Amount}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyRequireGoldNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD_NOT_ENOUGH);

        [Header("UI Elements")]
        public TextWrapper uiTextRequireLevel;
        public TextWrapper uiTextRequireSkillPoint;
        public TextWrapper uiTextRequireGold;
        public UIAttributeAmounts uiRequireAttributeAmounts;
        public UISkillLevels uiRequireSkillLevels;
        public UICurrencyAmounts uiRequireCurrencyAmounts;
        public UIItemAmounts uiRequireItemAmounts;
        public GameObject[] disallowStateObjects = new GameObject[0];

        protected Dictionary<Attribute, float> _tempRequireAttributes = new Dictionary<Attribute, float>();
        protected Dictionary<BaseSkill, int> _tempRequireSkills = new Dictionary<BaseSkill, int>();
        protected Dictionary<Currency, int> _tempRequireCurrencies = new Dictionary<Currency, int>();
        protected Dictionary<BaseItem, int> _tempRequireItems = new Dictionary<BaseItem, int>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextRequireLevel = null;
            uiTextRequireSkillPoint = null;
            uiTextRequireGold = null;
            uiRequireAttributeAmounts = null;
            uiRequireSkillLevels = null;
            uiRequireCurrencyAmounts = null;
            uiRequireItemAmounts = null;
            disallowStateObjects.Nullify();
            disallowStateObjects = null;
            _tempRequireAttributes.Clear();
            _tempRequireAttributes = null;
            _tempRequireSkills.Clear();
            _tempRequireSkills = null;
            _tempRequireCurrencies.Clear();
            _tempRequireCurrencies = null;
            _tempRequireItems.Clear();
            _tempRequireItems = null;
        }

        protected override void UpdateData()
        {
            BaseSkill skill = Data.characterSkill.GetSkill();
            int level = Data.targetLevel - 1;

            if (uiTextRequireLevel != null)
            {
                if (skill == null || skill.GetRequireCharacterLevel(level) <= 0)
                {
                    // Hide require level label when require level <= 0
                    uiTextRequireLevel.SetGameObjectActive(false);
                }
                else
                {
                    uiTextRequireLevel.SetGameObjectActive(true);
                    int characterLevel = GameInstance.PlayingCharacter != null ? GameInstance.PlayingCharacter.Level : 1;
                    int requireCharacterLevel = skill.GetRequireCharacterLevel(level);
                    if (characterLevel >= requireCharacterLevel)
                    {
                        uiTextRequireLevel.text = ZString.Format(
                            LanguageManager.GetText(formatKeyRequireLevel),
                            requireCharacterLevel.ToString("N0"));
                    }
                    else
                    {
                        uiTextRequireLevel.text = ZString.Format(
                            LanguageManager.GetText(formatKeyRequireLevelNotEnough),
                            characterLevel.ToString("N0"),
                            requireCharacterLevel.ToString("N0"));
                    }
                }
            }

            if (uiTextRequireSkillPoint != null)
            {
                if (skill == null || skill.GetRequireCharacterSkillPoint(level) <= 0)
                {
                    // Hide require level label when require level <= 0
                    uiTextRequireSkillPoint.SetGameObjectActive(false);
                }
                else
                {
                    uiTextRequireSkillPoint.SetGameObjectActive(true);
                    float characterSkillPoint = GameInstance.PlayingCharacter != null ? GameInstance.PlayingCharacter.SkillPoint : 0;
                    float requireCharacterSkillPoint = skill.GetRequireCharacterSkillPoint(level);
                    if (characterSkillPoint >= requireCharacterSkillPoint)
                    {
                        uiTextRequireSkillPoint.text = ZString.Format(
                            LanguageManager.GetText(formatKeyRequireSkillPoint),
                            requireCharacterSkillPoint.ToString("N0"));
                    }
                    else
                    {
                        uiTextRequireSkillPoint.text = ZString.Format(
                            LanguageManager.GetText(formatKeyRequireSkillPointNotEnough),
                            characterSkillPoint.ToString("N0"),
                            requireCharacterSkillPoint.ToString("N0"));
                    }
                }
            }

            if (uiTextRequireGold != null)
            {
                if (skill == null || skill.GetRequireCharacterGold(level) <= 0)
                {
                    // Hide require level label when require level <= 0
                    uiTextRequireGold.SetGameObjectActive(false);
                }
                else
                {
                    uiTextRequireGold.SetGameObjectActive(true);
                    float characterGold = GameInstance.PlayingCharacter != null ? GameInstance.PlayingCharacter.Gold : 0;
                    float requireCharacterGold = skill.GetRequireCharacterGold(level);
                    if (characterGold >= requireCharacterGold)
                    {
                        uiTextRequireGold.text = ZString.Format(
                            LanguageManager.GetText(formatKeyRequireGold),
                            characterGold.ToString("N0"),
                            requireCharacterGold.ToString("N0"));
                    }
                    else
                    {
                        uiTextRequireGold.text = ZString.Format(
                            LanguageManager.GetText(formatKeyRequireGoldNotEnough),
                            characterGold.ToString("N0"),
                            requireCharacterGold.ToString("N0"));
                    }
                }
            }

            if (uiRequireAttributeAmounts != null)
            {
                if (skill == null)
                {
                    uiRequireAttributeAmounts.Hide();
                }
                else
                {
                    uiRequireAttributeAmounts.displayType = UIAttributeAmounts.DisplayType.Requirement;
                    uiRequireAttributeAmounts.includeEquipmentsForCurrentAmounts = false;
                    uiRequireAttributeAmounts.includeBuffsForCurrentAmounts = false;
                    uiRequireAttributeAmounts.includeSkillsForCurrentAmounts = true;
                    uiRequireAttributeAmounts.isBonus = false;
                    uiRequireAttributeAmounts.Show();
                    _tempRequireAttributes.Clear();
                    skill.GetRequireAttributeAmounts(level, _tempRequireAttributes);
                    uiRequireAttributeAmounts.Data = _tempRequireAttributes;
                }
            }

            if (uiRequireSkillLevels != null)
            {
                if (skill == null)
                {
                    uiRequireSkillLevels.Hide();
                }
                else
                {
                    uiRequireSkillLevels.displayType = UISkillLevels.DisplayType.Requirement;
                    uiRequireSkillLevels.includeEquipmentsForCurrentLevels = false;
                    uiRequireSkillLevels.isBonus = false;
                    uiRequireSkillLevels.Show();
                    _tempRequireSkills.Clear();
                    skill.GetRequireSkillLevels(level, _tempRequireSkills);
                    uiRequireSkillLevels.Data = _tempRequireSkills;
                }
            }

            if (uiRequireCurrencyAmounts != null)
            {
                if (skill == null)
                {
                    uiRequireCurrencyAmounts.Hide();
                }
                else
                {
                    uiRequireCurrencyAmounts.displayType = UICurrencyAmounts.DisplayType.Requirement;
                    uiRequireCurrencyAmounts.isBonus = false;
                    uiRequireCurrencyAmounts.Show();
                    _tempRequireCurrencies.Clear();
                    skill.GetRequireCurrencyAmounts(level, _tempRequireCurrencies);
                    uiRequireCurrencyAmounts.Data = _tempRequireCurrencies;
                }
            }

            if (uiRequireItemAmounts != null)
            {
                if (skill == null)
                {
                    uiRequireItemAmounts.Hide();
                }
                else
                {
                    uiRequireItemAmounts.displayType = UIItemAmounts.DisplayType.Requirement;
                    uiRequireItemAmounts.isBonus = false;
                    uiRequireItemAmounts.Show();
                    _tempRequireItems.Clear();
                    skill.GetRequireItemAmounts(level, _tempRequireItems);
                    uiRequireItemAmounts.Data = _tempRequireItems;
                }
            }

            if (disallowStateObjects != null && disallowStateObjects.Length > 0)
            {
                bool disallow = skill.IsDisallowToLevelUp(level);
                foreach (GameObject obj in disallowStateObjects)
                {
                    if (obj == null)
                        continue;
                    obj.SetActive(disallow);
                }
            }
        }
    }
}
