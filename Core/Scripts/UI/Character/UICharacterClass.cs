using Cysharp.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UICharacterClass : UISelectionEntry<BaseCharacter>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public Image imageIcon;
        public UICharacterStats uiStats;
        public UIAttributeAmounts uiAttributes;
        public UIResistanceAmounts uiResistances;
        public UISkillLevels uiSkills;
        public UIStatusEffectResistances uiStatusEffectResistances;

        protected Dictionary<Attribute, float> _tempAttributeAmounts = new Dictionary<Attribute, float>();
        protected Dictionary<DamageElement, float> _tempResistanceAmounts = new Dictionary<DamageElement, float>();
        protected Dictionary<BaseSkill, int> _tempSkillLevels = new Dictionary<BaseSkill, int>();
        protected Dictionary<StatusEffect, float> _tempStatusEffectResistances = new Dictionary<StatusEffect, float>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            uiTextDescription = null;
            imageIcon = null;
            uiStats = null;
            uiAttributes = null;
            uiResistances = null;
            uiSkills = null;
            _data = null;
            _tempAttributeAmounts.Clear();
            _tempAttributeAmounts = null;
            _tempResistanceAmounts.Clear();
            _tempResistanceAmounts = null;
            _tempSkillLevels.Clear();
            _tempSkillLevels = null;
            _tempStatusEffectResistances.Clear();
            _tempStatusEffectResistances = null;
        }

        protected override void UpdateData()
        {
            if (uiTextTitle != null)
            {
                uiTextTitle.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.Title);
            }

            if (uiTextDescription != null)
            {
                uiTextDescription.text = ZString.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    Data == null ? LanguageManager.GetUnknowDescription() : Data.Description);
            }

            imageIcon.SetImageGameDataIcon(Data);

            if (uiStats != null)
            {
                uiStats.displayType = UICharacterStats.DisplayType.Simple;
                uiStats.isBonus = false;
                uiStats.Data = Data.GetCharacterStats(1);
            }

            if (uiAttributes != null)
            {
                uiAttributes.displayType = UIAttributeAmounts.DisplayType.Simple;
                uiAttributes.isBonus = false;
                Data.GetCharacterAttributes(1, _tempAttributeAmounts);
                uiAttributes.Data = _tempAttributeAmounts;
            }

            if (uiResistances != null)
            {
                uiResistances.isBonus = false;
                Data.GetCharacterResistances(1, _tempResistanceAmounts);
                uiResistances.Data = _tempResistanceAmounts;
            }

            if (uiSkills != null)
            {
                uiSkills.displayType = UISkillLevels.DisplayType.Simple;
                uiSkills.isBonus = false;
                Data.GetSkillLevels(1, _tempSkillLevels);
                uiSkills.Data = _tempSkillLevels;
            }

            if (uiStatusEffectResistances != null)
            {
                Data.GetCharacterStatusEffectResistances(1, _tempStatusEffectResistances);
                uiStatusEffectResistances.UpdateData(_tempStatusEffectResistances);
            }
        }
    }
}
