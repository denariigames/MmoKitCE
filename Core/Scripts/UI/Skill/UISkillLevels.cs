using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    public partial class UISkillLevels : UISelectionEntry<Dictionary<BaseSkill, int>>
    {
        public enum DisplayType
        {
            Simple,
            Requirement
        }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Skill Title}, {1} = {Current Level}, {2} = {Target Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_SKILL);
        [Tooltip("Format => {0} = {Skill Title}, {1} = {Current Level}, {2} = {Target Level}")]
        public UILocaleKeySetting formatKeyLevelNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_SKILL_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Skill Title}, {1} = {Target Level}")]
        public UILocaleKeySetting formatKeySimpleLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SKILL_LEVEL);

        [Header("UI Elements")]
        public TextWrapper uiTextAllLevels;
        public UISkillTextPair[] textLevels;

        [Header("Options")]
        public DisplayType displayType;
        public string numberFormatSimple = "N0";
        public bool includeEquipmentsForCurrentLevels;
        public bool isBonus;
        public bool inactiveIfLevelZero;
        public bool useSimpleFormatIfLevelEnough = true;

        private Dictionary<BaseSkill, UISkillTextPair> _cacheTextLevels;
        public Dictionary<BaseSkill, UISkillTextPair> CacheTextLevels
        {
            get
            {
                if (_cacheTextLevels == null)
                {
                    _cacheTextLevels = new Dictionary<BaseSkill, UISkillTextPair>();
                    BaseSkill tempData;
                    foreach (UISkillTextPair componentPair in textLevels)
                    {
                        if (componentPair.skill == null || componentPair.uiText == null)
                            continue;
                        tempData = componentPair.skill;
                        SetDefaultValue(componentPair);
                        _cacheTextLevels[tempData] = componentPair;
                    }
                }
                return _cacheTextLevels;
            }
        }

        protected Dictionary<BaseSkill, int> _tempSkills;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextAllLevels = null;
            textLevels = null;
            _cacheTextLevels?.Clear();
            _cacheTextLevels = null;
            _data?.Clear();
            _data = null;
            CleanTempData();
        }

        protected void CleanTempData()
        {
            if (_tempSkills != null)
            {
                CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Release(_tempSkills);
                _tempSkills = null;
            }
        }

        protected override void UpdateData()
        {
            CleanTempData();
            // Reset number
            foreach (UISkillTextPair entry in CacheTextLevels.Values)
            {
                SetDefaultValue(entry);
            }
            // Set number by updated data
            if (Data == null || Data.Count == 0)
            {
                if (uiTextAllLevels != null)
                    uiTextAllLevels.SetGameObjectActive(false);
            }
            else
            {
                // Prepare attribute data
                IPlayerCharacterData character = GameInstance.PlayingCharacter;
                if (character != null)
                    character.GetAllStats(includeEquipmentsForCurrentLevels, false, false, 
                        onGetSkills: skills => _tempSkills = skills,
                        willReleaseSkills: false);

                // In-loop temp data
                using (Utf16ValueStringBuilder tempAllText = ZString.CreateStringBuilder(false))
                {
                    BaseSkill tempData;
                    int tempCurrentLevel;
                    int tempTargetLevel;
                    bool tempLevelEnough;
                    string tempCurrentValue;
                    string tempValue;
                    string tempFormat;
                    string tempLevelText;
                    UISkillTextPair tempComponentPair;
                    foreach (KeyValuePair<BaseSkill, int> dataEntry in Data)
                    {
                        if (dataEntry.Key == null)
                            continue;
                        // Set temp data
                        tempData = dataEntry.Key;
                        tempTargetLevel = dataEntry.Value;
                        tempCurrentLevel = 0;
                        // Get skill level from character
                        _tempSkills.TryGetValue(tempData, out tempCurrentLevel);
                        // Use difference format by option
                        switch (displayType)
                        {
                            case DisplayType.Requirement:
                                // This will show both current character skill level and target level
                                tempLevelEnough = tempCurrentLevel >= tempTargetLevel;
                                tempFormat = LanguageManager.GetText(tempLevelEnough ? formatKeyLevel : formatKeyLevelNotEnough);
                                tempCurrentValue = tempCurrentLevel.ToString(numberFormatSimple);
                                tempValue = tempTargetLevel.ToString(numberFormatSimple);
                                if (useSimpleFormatIfLevelEnough && tempLevelEnough)
                                    tempLevelText = ZString.Format(LanguageManager.GetText(formatKeySimpleLevel), tempData.Title, tempValue);
                                else
                                    tempLevelText = ZString.Format(tempFormat, tempData.Title, tempCurrentValue, tempValue);
                                break;
                            default:
                                // This will show only target level, so current character skill level will not be shown
                                tempValue = tempTargetLevel.ToString(numberFormatSimple);
                                tempLevelText = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                                    LanguageManager.GetText(formatKeySimpleLevel),
                                    tempData.Title,
                                    tempValue));
                                break;
                        }
                        // Append current skill level text
                        if (dataEntry.Value != 0)
                        {
                            // Add new line if text is not empty
                            if (tempAllText.Length > 0)
                                tempAllText.Append('\n');
                            tempAllText.Append(tempLevelText);
                        }
                        // Set current skill text to UI
                        if (CacheTextLevels.TryGetValue(dataEntry.Key, out tempComponentPair))
                        {
                            tempComponentPair.uiText.text = tempLevelText;
                            if (tempComponentPair.root != null)
                                tempComponentPair.root.SetActive(!inactiveIfLevelZero || tempTargetLevel != 0);
                        }
                    }

                    if (uiTextAllLevels != null)
                    {
                        uiTextAllLevels.SetGameObjectActive(tempAllText.Length > 0);
                        uiTextAllLevels.text = tempAllText.ToString();
                    }
                }
            }
        }

        private void SetDefaultValue(UISkillTextPair componentPair)
        {
            string zeroFormatSimple = 0f.ToString(numberFormatSimple);
            switch (displayType)
            {
                case DisplayType.Requirement:
                    if (useSimpleFormatIfLevelEnough)
                    {
                        componentPair.uiText.text = ZString.Format(
                            LanguageManager.GetText(formatKeySimpleLevel),
                            componentPair.skill.Title,
                            zeroFormatSimple);
                    }
                    else
                    {
                        componentPair.uiText.text = ZString.Format(
                            LanguageManager.GetText(formatKeyLevel),
                            componentPair.skill.Title,
                            zeroFormatSimple, zeroFormatSimple);
                    }
                    break;
                case DisplayType.Simple:
                    componentPair.uiText.text = ZString.Concat(isBonus ? "+" : string.Empty, ZString.Format(
                        LanguageManager.GetText(formatKeySimpleLevel),
                        componentPair.skill.Title,
                        zeroFormatSimple));
                    break;
            }
            componentPair.imageIcon.SetImageGameDataIcon(componentPair.skill);
            if (inactiveIfLevelZero && componentPair.root != null)
                componentPair.root.SetActive(false);
        }
    }
}
