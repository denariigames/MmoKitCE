using Cysharp.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UIItemRequirement : UISelectionEntry<IItemWithRequirement>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Require Level}")]
        public UILocaleKeySetting formatKeyRequireLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_LEVEL);
        [Tooltip("Format => {0} = {Current Level}, {1} = {Require Level}")]
        public UILocaleKeySetting formatKeyRequireLevelNotEnough = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_LEVEL_NOT_ENOUGH);
        [Tooltip("Format => {0} = {Require Classes Title}")]
        [FormerlySerializedAs("formatKeyRequireClasses")]
        public UILocaleKeySetting formatKeyRequireClasses = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_CLASS);
        [Tooltip("Format => {0} = {Require Classes Title}")]
        public UILocaleKeySetting formatKeyInvalidRequireClasses = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_INVALID_REQUIRE_CLASS);
        [Tooltip("Format => {0} = {Require Factions Title}")]
        public UILocaleKeySetting formatKeyRequireFactions = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REQUIRE_FACTION);
        [Tooltip("Format => {0} = {Require Factions Title}")]
        public UILocaleKeySetting formatKeyInvalidRequireFactions = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_INVALID_REQUIRE_FACTION);

        [Header("UI Elements")]
        public TextWrapper uiTextRequireLevel;
        [FormerlySerializedAs("uiTextRequireClass")]
        public TextWrapper uiTextRequireClasses;
        public TextWrapper uiTextRequireFactions;
        public UIAttributeAmounts uiRequireAttributeAmounts;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextRequireLevel = null;
            uiTextRequireClasses = null;
            uiTextRequireFactions = null;
            uiRequireAttributeAmounts = null;
            _data = null;
        }

        protected override void UpdateData()
        {
            if (uiTextRequireLevel != null)
            {
                if (Data == null || Data.Requirement.level <= 0)
                {
                    // Hide require level label when require level <= 0
                    uiTextRequireLevel.SetGameObjectActive(false);
                }
                else
                {
                    uiTextRequireLevel.SetGameObjectActive(true);
                    int characterLevel = GameInstance.PlayingCharacter != null ? GameInstance.PlayingCharacter.Level : 1;
                    int requireCharacterLevel = Data.Requirement.level;
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
                            characterLevel,
                            requireCharacterLevel.ToString("N0"));
                    }
                }
            }

            if (uiTextRequireClasses != null)
            {
                if (Data == null || !Data.Requirement.HasAvailableClasses())
                {
                    // Hide require class label when require character is null
                    uiTextRequireClasses.SetGameObjectActive(false);
                }
                else
                {
                    using (Utf16ValueStringBuilder str = ZString.CreateStringBuilder(false))
                    {
                        if (!GameInstance.PlayerCharacters.TryGetValue(GameInstance.PlayingCharacter.DataId, out PlayerCharacter playingCharacterClass))
                            playingCharacterClass = null;
                        bool available = false;
                        if (Data.Requirement.availableClass != null)
                        {
                            str.Append(Data.Requirement.availableClass.Title);
                            if (playingCharacterClass == Data.Requirement.availableClass)
                                available = true;
                        }
                        if (Data.Requirement.availableClasses != null &&
                            Data.Requirement.availableClasses.Count > 0)
                        {
                            foreach (PlayerCharacter characterClass in Data.Requirement.availableClasses)
                            {
                                if (characterClass == null)
                                    continue;
                                if (str.Length > 0)
                                    str.Append('/');
                                str.Append(characterClass.Title);
                                if (playingCharacterClass == characterClass)
                                    available = true;
                            }
                        }
                        uiTextRequireClasses.SetGameObjectActive(true);
                        uiTextRequireClasses.text = ZString.Format(
                            LanguageManager.GetText(available ? formatKeyRequireClasses : formatKeyInvalidRequireClasses),
                            str.ToString());
                    }
                }
            }

            if (uiTextRequireFactions != null)
            {
                if (Data == null || !Data.Requirement.HasAvailableFactions())
                {
                    // Hide require faction label when require character is null
                    uiTextRequireFactions.SetGameObjectActive(false);
                }
                else
                {
                    using (Utf16ValueStringBuilder str = ZString.CreateStringBuilder(false))
                    {
                        if (!GameInstance.Factions.TryGetValue(GameInstance.PlayingCharacter.FactionId, out Faction playingCharacterFaction))
                            playingCharacterFaction = null;
                        bool available = false;
                        if (Data.Requirement.availableFactions != null &&
                            Data.Requirement.availableFactions.Count > 0)
                        {
                            foreach (Faction faction in Data.Requirement.availableFactions)
                            {
                                if (faction == null)
                                    continue;
                                if (str.Length > 0)
                                    str.Append('/');
                                str.Append(faction.Title);
                                if (playingCharacterFaction == faction)
                                    available = true;
                            }
                        }
                        uiTextRequireFactions.SetGameObjectActive(true);
                        uiTextRequireFactions.text = ZString.Format(
                            LanguageManager.GetText(available ? formatKeyRequireFactions : formatKeyInvalidRequireFactions),
                            str.ToString());
                    }
                }
            }

            if (uiRequireAttributeAmounts != null)
            {
                if (Data == null)
                {
                    // Hide attribute amounts when item data is empty
                    uiRequireAttributeAmounts.Hide();
                }
                else
                {
                    uiRequireAttributeAmounts.displayType = UIAttributeAmounts.DisplayType.Requirement;
                    uiRequireAttributeAmounts.includeEquipmentsForCurrentAmounts = true;
                    uiRequireAttributeAmounts.includeBuffsForCurrentAmounts = false;
                    uiRequireAttributeAmounts.includeSkillsForCurrentAmounts = true;
                    uiRequireAttributeAmounts.isBonus = false;
                    uiRequireAttributeAmounts.Show();
                    uiRequireAttributeAmounts.Data = Data.RequireAttributeAmounts;
                }
            }
        }
    }
}
