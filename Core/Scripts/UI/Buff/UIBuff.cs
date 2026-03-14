using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIBuff : UISelectionEntry<UIBuffData>
    {
        public Buff Buff { get { return Data.buff; } }
        public int Level { get { return Data.targetLevel; } }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Buff Duration}")]
        public UILocaleKeySetting formatKeyDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_DURATION);
        [Tooltip("Format => {0} = {Max Stack}")]
        public UILocaleKeySetting formatKeyMaxStack = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_MAX_STACK);
        [Tooltip("Format => {0} = {Buff Recovery Hp}")]
        public UILocaleKeySetting formatKeyRecoveryHp = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_HP);
        [Tooltip("Format => {0} = {Buff Recovery Mp}")]
        public UILocaleKeySetting formatKeyRecoveryMp = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_MP);
        [Tooltip("Format => {0} = {Buff Recovery Stamina}")]
        public UILocaleKeySetting formatKeyRecoveryStamina = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_STAMINA);
        [Tooltip("Format => {0} = {Buff Recovery Food}")]
        public UILocaleKeySetting formatKeyRecoveryFood = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_FOOD);
        [Tooltip("Format => {0} = {Buff Recovery Water}")]
        public UILocaleKeySetting formatKeyRecoveryWater = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_WATER);
        [Tooltip("Format => {0} = {Chance * 100}")]
        public UILocaleKeySetting formatKeyRemoveBuffWhenAttackChance = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_WHEN_ATTACK_CHANCE);
        [Tooltip("Format => {0} = {Chance * 100}")]
        public UILocaleKeySetting formatKeyRemoveBuffWhenAttackedChance = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_WHEN_ATTACKED_CHANCE);
        [Tooltip("Format => {0} = {Chance * 100}")]
        public UILocaleKeySetting formatKeyRemoveBuffWhenUseSkillChance = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_USE_SKILL_CHANCE);
        [Tooltip("Format => {0} = {Chance * 100}")]
        public UILocaleKeySetting formatKeyRemoveBuffWhenUseItemChance = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_USE_ITEM_CHANCE);
        [Tooltip("Format => {0} = {Chance * 100}")]
        public UILocaleKeySetting formatKeyRemoveBuffWhenPickupItemChance = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_PICKUP_ITEM_CHANCE);

        [Header("UI Elements")]
        public TextWrapper uiTextDuration;
        public TextWrapper uiTextMaxStack;
        public TextWrapper uiTextRecoveryHp;
        public TextWrapper uiTextRecoveryMp;
        public TextWrapper uiTextRecoveryStamina;
        public TextWrapper uiTextRecoveryFood;
        public TextWrapper uiTextRecoveryWater;
        public TextWrapper uiTextRemoveBuffWhenAttackChance;
        public TextWrapper uiTextRemoveBuffWhenAttackedChance;
        public TextWrapper uiTextRemoveBuffWhenUseSkillChance;
        public TextWrapper uiTextRemoveBuffWhenUseItemChance;
        public TextWrapper uiTextRemoveBuffWhenPickupItemChance;
        public UICharacterStats uiBuffStats;
        public UICharacterStats uiBuffStatsRate;
        public UIAttributeAmounts uiBuffAttributes;
        public UIAttributeAmounts uiBuffAttributesRate;
        public UIResistanceAmounts uiBuffResistances;
        public UIArmorAmounts uiBuffArmors;
        public UIArmorAmounts uiBuffArmorsRate;
        public UIDamageElementAmounts uiBuffDamages;
        public UIDamageElementAmounts uiBuffDamagesRate;
        public UIDamageElementAmounts uiDamageOverTimes;
        public UIStatusEffectApplyings uiStatusEffectApplyingsSelfWhenAttacking;
        public UIStatusEffectApplyings uiStatusEffectApplyingsEnemyWhenAttacking;
        public UIStatusEffectApplyings uiStatusEffectApplyingsSelfWhenAttacked;
        public UIStatusEffectApplyings uiStatusEffectApplyingsEnemyWhenAttacked;
        public UISkillLevels uiIncreaseSkills;
        public UISkillLevels uiOverrideSkills;
        public UIStatusEffectResistances uiStatusEffectResistances;
        public UIBuffRemovals uiBuffRemovals;
        [Header("Extras")]
        [Tooltip("This will activate if buff's disallow move is `TRUE`, developer may set text or icon here")]
        public GameObject disallowMoveObject;
        [Tooltip("This will activate if buff's disallow sprint is `TRUE`, developer may set text or icon here")]
        public GameObject disallowSprintObject;
        [Tooltip("This will activate if buff's disallow walk is `TRUE`, developer may set text or icon here")]
        public GameObject disallowWalkObject;
        [Tooltip("This will activate if buff's disallow jump is `TRUE`, developer may set text or icon here")]
        public GameObject disallowJumpObject;
        [Tooltip("This will activate if buff's disallow dash is `TRUE`, developer may set text or icon here")]
        public GameObject disallowDashObject;
        [Tooltip("This will activate if buff's disallow crouch is `TRUE`, developer may set text or icon here")]
        public GameObject disallowCrouchObject;
        [Tooltip("This will activate if buff's disallow prone is `TRUE`, developer may set text or icon here")]
        public GameObject disallowCrawlObject;
        [Tooltip("This will activate if buff's disallow attack is `TRUE`, developer may set text or icon here")]
        public GameObject disallowAttackObject;
        [Tooltip("This will activate if buff's disallow use skill is `TRUE`, developer may set text or icon here")]
        public GameObject disallowUseSkillObject;
        [Tooltip("This will activate if buff's disallow use item is `TRUE`, developer may set text or icon here")]
        public GameObject disallowUseItemObject;
        [Tooltip("This will activate if buff's freeze animation is `TRUE`, developer may set text or icon here")]
        public GameObject freezeAnimationObject;
        [Tooltip("This will activate if buff's is hide is `TRUE`, developer may set text or icon here")]
        public GameObject isHideObject;
        [Tooltip("This will activate if buff's is reveals hide is `TRUE`, developer may set text or icon here")]
        public GameObject isRevealsHideObject;
        [Tooltip("This will activate if buff's is blind is `TRUE`, developer may set text or icon here")]
        public GameObject isBlindObject;
        [Tooltip("This will activate if buff's do not remove on dead is `TRUE`, developer may set text or icon here")]
        public GameObject doNotRemoveOnDeadObject;
        [Tooltip("This will activate if buff's mute footstep sound is `TRUE`, developer may set text or icon here")]
        public GameObject muteFootstepSoundObject;
        [Tooltip("This will activate if buff's is extend duration is `TRUE`, developer may set text or icon here")]
        public GameObject isExtendDurationObject;
        [Tooltip("Text of all extras will be written here")]
        public TextWrapper uiTextExtras;
        [Tooltip("Seperator for ailments")]
        public string extrasSeparator = ", ";

        protected Dictionary<Attribute, float> _tempBuffAttributes = new Dictionary<Attribute, float>();
        protected Dictionary<Attribute, float> _tempBuffAttributesRate = new Dictionary<Attribute, float>();
        protected Dictionary<DamageElement, float> _tempBuffResistances = new Dictionary<DamageElement, float>();
        protected Dictionary<DamageElement, float> _tempBuffArmors = new Dictionary<DamageElement, float>();
        protected Dictionary<DamageElement, float> _tempBuffArmorsRate = new Dictionary<DamageElement, float>();
        protected Dictionary<DamageElement, MinMaxFloat> _tempBuffDamages = new Dictionary<DamageElement, MinMaxFloat>();
        protected Dictionary<DamageElement, MinMaxFloat> _tempBuffDamagesRate = new Dictionary<DamageElement, MinMaxFloat>();
        protected Dictionary<DamageElement, MinMaxFloat> _tempDamageOverTimes = new Dictionary<DamageElement, MinMaxFloat>();
        protected Dictionary<BaseSkill, int> _tempIncreaseSkills = new Dictionary<BaseSkill, int>();
        protected Dictionary<BaseSkill, int> _tempOverrideSkills = new Dictionary<BaseSkill, int>();
        protected Dictionary<StatusEffect, float> _tempStatusEffectResistances = new Dictionary<StatusEffect, float>();
        protected Dictionary<BuffRemoval, float> _tempBuffRemovals = new Dictionary<BuffRemoval, float>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextDuration = null;
            uiTextMaxStack = null;
            uiTextRecoveryHp = null;
            uiTextRecoveryMp = null;
            uiTextRecoveryStamina = null;
            uiTextRecoveryFood = null;
            uiTextRecoveryWater = null;
            uiTextRemoveBuffWhenAttackChance = null;
            uiTextRemoveBuffWhenAttackedChance = null;
            uiTextRemoveBuffWhenUseSkillChance = null;
            uiTextRemoveBuffWhenUseItemChance = null;
            uiTextRemoveBuffWhenPickupItemChance = null;
            uiBuffStats = null;
            uiBuffStatsRate = null;
            uiBuffAttributes = null;
            uiBuffAttributesRate = null;
            uiBuffResistances = null;
            uiBuffArmors = null;
            uiBuffArmorsRate = null;
            uiBuffDamages = null;
            uiBuffDamagesRate = null;
            uiDamageOverTimes = null;
            uiStatusEffectApplyingsSelfWhenAttacking = null;
            uiStatusEffectApplyingsEnemyWhenAttacking = null;
            uiStatusEffectApplyingsSelfWhenAttacked = null;
            uiStatusEffectApplyingsEnemyWhenAttacked = null;
            uiStatusEffectResistances = null;
            uiBuffRemovals = null;
            disallowMoveObject = null;
            disallowSprintObject = null;
            disallowWalkObject = null;
            disallowJumpObject = null;
            disallowCrouchObject = null;
            disallowCrawlObject = null;
            disallowAttackObject = null;
            disallowUseSkillObject = null;
            disallowUseItemObject = null;
            freezeAnimationObject = null;
            isHideObject = null;
            isRevealsHideObject = null;
            isBlindObject = null;
            doNotRemoveOnDeadObject = null;
            muteFootstepSoundObject = null;
            isExtendDurationObject = null;
            uiTextExtras = null;
            _tempBuffAttributes.Clear();
            _tempBuffAttributes = null;
            _tempBuffAttributesRate.Clear();
            _tempBuffAttributesRate = null;
            _tempBuffResistances.Clear();
            _tempBuffResistances = null;
            _tempBuffArmors.Clear();
            _tempBuffArmors = null;
            _tempBuffArmorsRate.Clear();
            _tempBuffArmorsRate = null;
            _tempBuffDamages.Clear();
            _tempBuffDamages = null;
            _tempBuffDamagesRate.Clear();
            _tempBuffDamagesRate = null;
            _tempDamageOverTimes.Clear();
            _tempDamageOverTimes = null;
            _tempIncreaseSkills.Clear();
            _tempIncreaseSkills = null;
            _tempOverrideSkills.Clear();
            _tempOverrideSkills = null;
            _tempStatusEffectResistances.Clear();
            _tempStatusEffectResistances = null;
            _tempBuffRemovals.Clear();
            _tempBuffRemovals = null;
        }

        protected override void UpdateData()
        {
            if (uiTextDuration != null)
            {
                float value = Buff.noDuration ? 0f : Buff.GetDuration(Level);
                bool activated = value != 0f;
                uiTextDuration.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextDuration.text = ZString.Format(
                        LanguageManager.GetText(formatKeyDuration),
                        value.ToString("N0"));
                }
            }

            if (uiTextMaxStack != null)
            {
                int value = Buff.GetMaxStack(Level);
                bool activated = value != 0;
                uiTextMaxStack.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextMaxStack.text = ZString.Format(
                        LanguageManager.GetText(formatKeyMaxStack),
                        value.ToString("N0"));
                }
            }

            if (uiTextRecoveryHp != null)
            {
                int value = Buff.GetRecoveryHp(Level);
                bool activated = value != 0;
                uiTextRecoveryHp.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRecoveryHp.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRecoveryHp),
                        value.ToString("N0"));
                }
            }

            if (uiTextRecoveryMp != null)
            {
                int value = Buff.GetRecoveryMp(Level);
                bool activated = value != 0;
                uiTextRecoveryMp.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRecoveryMp.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRecoveryMp),
                        value.ToString("N0"));
                }
            }

            if (uiTextRecoveryStamina != null)
            {
                int value = Buff.GetRecoveryStamina(Level);
                bool activated = value != 0;
                uiTextRecoveryStamina.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRecoveryStamina.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRecoveryStamina),
                        value.ToString("N0"));
                }
            }

            if (uiTextRecoveryFood != null)
            {
                int value = Buff.GetRecoveryFood(Level);
                bool activated = value != 0;
                uiTextRecoveryFood.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRecoveryFood.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRecoveryFood),
                        value.ToString("N0"));
                }
            }

            if (uiTextRecoveryWater != null)
            {
                int value = Buff.GetRecoveryWater(Level);
                bool activated = value != 0;
                uiTextRecoveryWater.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRecoveryWater.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRecoveryWater),
                        value.ToString("N0"));
                }
            }

            if (uiTextRemoveBuffWhenAttackChance != null)
            {
                float value = Buff.GetRemoveBuffWhenAttackChance(Level);
                bool activated = value != 0;
                uiTextRemoveBuffWhenAttackChance.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRemoveBuffWhenAttackChance.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRemoveBuffWhenAttackChance),
                        (value * 100).ToString("N2"));
                }
            }

            if (uiTextRemoveBuffWhenAttackedChance != null)
            {
                float value = Buff.GetRemoveBuffWhenAttackedChance(Level);
                bool activated = value != 0;
                uiTextRemoveBuffWhenAttackedChance.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRemoveBuffWhenAttackedChance.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRemoveBuffWhenAttackedChance),
                        (value * 100).ToString("N2"));
                }
            }

            if (uiTextRemoveBuffWhenUseSkillChance != null)
            {
                float value = Buff.GetRemoveBuffWhenUseSkillChance(Level);
                bool activated = value != 0;
                uiTextRemoveBuffWhenUseSkillChance.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRemoveBuffWhenUseSkillChance.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRemoveBuffWhenUseSkillChance),
                        (value * 100).ToString("N2"));
                }
            }

            if (uiTextRemoveBuffWhenUseItemChance != null)
            {
                float value = Buff.GetRemoveBuffWhenUseItemChance(Level);
                bool activated = value != 0;
                uiTextRemoveBuffWhenUseItemChance.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRemoveBuffWhenUseItemChance.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRemoveBuffWhenUseItemChance),
                        (value * 100).ToString("N2"));
                }
            }

            if (uiTextRemoveBuffWhenPickupItemChance != null)
            {
                float value = Buff.GetRemoveBuffWhenPickupItemChance(Level);
                bool activated = value != 0;
                uiTextRemoveBuffWhenPickupItemChance.SetGameObjectActive(activated);
                if (activated)
                {
                    uiTextRemoveBuffWhenPickupItemChance.text = ZString.Format(
                        LanguageManager.GetText(formatKeyRemoveBuffWhenPickupItemChance),
                        (value * 100).ToString("N2"));
                }
            }

            if (uiTextExtras != null)
            {
                bool activated = Data.buff != null &&
                    (Data.buff.disallowMove ||
                    Data.buff.disallowSprint ||
                    Data.buff.disallowWalk ||
                    Data.buff.disallowJump ||
                    Data.buff.disallowDash ||
                    Data.buff.disallowCrouch ||
                    Data.buff.disallowCrawl ||
                    Data.buff.disallowAttack ||
                    Data.buff.disallowUseSkill ||
                    Data.buff.disallowUseItem ||
                    Data.buff.freezeAnimation ||
                    Data.buff.isHide ||
                    Data.buff.isRevealsHide ||
                    Data.buff.isBlind ||
                    Data.buff.doNotRemoveOnDead ||
                    Data.buff.muteFootstepSound ||
                    Data.buff.isExtendDuration);
                uiTextExtras.SetGameObjectActive(activated);
                if (activated)
                {
                    List<string> ailments = new List<string>();
                    if (Data.buff.disallowMove)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_MOVE.ToString(), "Disallow Move"));
                    if (Data.buff.disallowSprint)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_SPRINT.ToString(), "Disallow Sprint"));
                    if (Data.buff.disallowWalk)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_WALK.ToString(), "Disallow Walk"));
                    if (Data.buff.disallowJump)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_JUMP.ToString(), "Disallow Jump"));
                    if (Data.buff.disallowDash)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_DASH.ToString(), "Disallow Dash"));
                    if (Data.buff.disallowCrouch)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_CROUCH.ToString(), "Disallow Crouch"));
                    if (Data.buff.disallowCrawl)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_CRAWL.ToString(), "Disallow Crawl"));
                    if (Data.buff.disallowAttack)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_ATTACK.ToString(), "Disallow Attack"));
                    if (Data.buff.disallowUseSkill)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_USE_SKILL.ToString(), "Disallow Use Skill"));
                    if (Data.buff.disallowUseItem)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DISALLOW_USE_ITEM.ToString(), "Disallow Use Item"));
                    if (Data.buff.freezeAnimation)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_FREEZE_ANIMATION.ToString(), "Freeze Animation"));
                    if (Data.buff.isHide)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_IS_HIDE.ToString(), "Hide"));
                    if (Data.buff.isRevealsHide)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_IS_REVEALS_HIDE.ToString(), "Reveals Hide"));
                    if (Data.buff.isBlind)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_IS_BLIND.ToString(), "Blind"));
                    if (Data.buff.doNotRemoveOnDead)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_DO_NOT_REMOVE_ON_DEAD.ToString(), "Won't Be Removed On Dead"));
                    if (Data.buff.muteFootstepSound)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_MUTE_FOOTSTEP_SOUND.ToString(), "Mute Footstep Sound"));
                    if (Data.buff.isExtendDuration)
                        ailments.Add(LanguageManager.GetText(UITextKeys.UI_LABEL_BUFF_IS_EXTEND_DURATION.ToString(), "Extend Duration"));
                    uiTextExtras.text = string.Join(extrasSeparator, ailments);
                }
            }

            if (uiBuffStats != null)
            {
                CharacterStats stats = Buff.GetIncreaseStats(Level);
                if (stats.IsEmpty())
                {
                    uiBuffStats.Hide();
                }
                else
                {
                    uiBuffStats.displayType = UICharacterStats.DisplayType.Simple;
                    uiBuffStats.isBonus = true;
                    uiBuffStats.Show();
                    uiBuffStats.Data = stats;
                }
            }

            if (uiBuffStatsRate != null)
            {
                CharacterStats statsRate = Buff.GetIncreaseStatsRate(Level);
                if (statsRate.IsEmpty())
                {
                    uiBuffStatsRate.Hide();
                }
                else
                {
                    uiBuffStatsRate.displayType = UICharacterStats.DisplayType.Rate;
                    uiBuffStatsRate.isBonus = true;
                    uiBuffStatsRate.Show();
                    uiBuffStatsRate.Data = statsRate;
                }
            }

            if (uiBuffAttributes != null)
            {
                if (Buff.increaseAttributes == null || Buff.increaseAttributes.Length == 0)
                {
                    uiBuffAttributes.Hide();
                }
                else
                {
                    uiBuffAttributes.displayType = UIAttributeAmounts.DisplayType.Simple;
                    uiBuffAttributes.isBonus = true;
                    uiBuffAttributes.Show();
                    _tempBuffAttributes.Clear();
                    GameDataHelpers.CombineAttributes(Buff.increaseAttributes, _tempBuffAttributes, Level, 1f);
                    uiBuffAttributes.Data = _tempBuffAttributes;
                }
            }

            if (uiBuffAttributesRate != null)
            {
                if (Buff.increaseAttributesRate == null || Buff.increaseAttributesRate.Length == 0)
                {
                    uiBuffAttributesRate.Hide();
                }
                else
                {
                    uiBuffAttributesRate.displayType = UIAttributeAmounts.DisplayType.Rate;
                    uiBuffAttributesRate.isBonus = true;
                    uiBuffAttributesRate.Show();
                    _tempBuffAttributesRate.Clear();
                    GameDataHelpers.CombineAttributes(Buff.increaseAttributesRate, _tempBuffAttributesRate, Level, 1f);
                    uiBuffAttributesRate.Data = _tempBuffAttributesRate;
                }
            }

            if (uiBuffResistances != null)
            {
                if (Buff.increaseResistances == null || Buff.increaseResistances.Length == 0)
                {
                    uiBuffResistances.Hide();
                }
                else
                {
                    uiBuffResistances.isBonus = true;
                    uiBuffResistances.Show();
                    _tempBuffResistances.Clear();
                    GameDataHelpers.CombineResistances(Buff.increaseResistances, _tempBuffResistances, Level, 1f);
                    uiBuffResistances.Data = _tempBuffResistances;
                }
            }

            if (uiBuffArmors != null)
            {
                if (Buff.increaseArmors == null || Buff.increaseArmors.Length == 0)
                {
                    uiBuffArmors.Hide();
                }
                else
                {
                    uiBuffArmors.displayType = UIArmorAmounts.DisplayType.Simple;
                    uiBuffArmors.isBonus = true;
                    uiBuffArmors.Show();
                    _tempBuffArmors.Clear();
                    GameDataHelpers.CombineArmors(Buff.increaseArmors, _tempBuffArmors, Level, 1f);
                    uiBuffArmors.Data = _tempBuffArmors;
                }
            }

            if (uiBuffArmorsRate != null)
            {
                if (Buff.increaseArmorsRate == null || Buff.increaseArmorsRate.Length == 0)
                {
                    uiBuffArmorsRate.Hide();
                }
                else
                {
                    uiBuffArmorsRate.displayType = UIArmorAmounts.DisplayType.Rate;
                    uiBuffArmorsRate.isBonus = true;
                    uiBuffArmorsRate.Show();
                    _tempBuffArmorsRate.Clear();
                    GameDataHelpers.CombineArmors(Buff.increaseArmorsRate, _tempBuffArmorsRate, Level, 1f);
                    uiBuffArmorsRate.Data = _tempBuffArmorsRate;
                }
            }

            if (uiBuffDamages != null)
            {
                if (Buff.increaseDamages == null || Buff.increaseDamages.Length == 0)
                {
                    uiBuffDamages.Hide();
                }
                else
                {
                    uiBuffDamages.displayType = UIDamageElementAmounts.DisplayType.Simple;
                    uiBuffDamages.isBonus = true;
                    uiBuffDamages.Show();
                    _tempBuffDamages.Clear();
                    GameDataHelpers.CombineDamages(Buff.increaseDamages, _tempBuffDamages, Level, 1f);
                    uiBuffDamages.Data = _tempBuffDamages;
                }
            }

            if (uiBuffDamagesRate != null)
            {
                if (Buff.increaseDamagesRate == null || Buff.increaseDamagesRate.Length == 0)
                {
                    uiBuffDamagesRate.Hide();
                }
                else
                {
                    uiBuffDamagesRate.displayType = UIDamageElementAmounts.DisplayType.Rate;
                    uiBuffDamagesRate.isBonus = true;
                    uiBuffDamagesRate.Show();
                    _tempBuffDamagesRate.Clear();
                    GameDataHelpers.CombineDamages(Buff.increaseDamagesRate, _tempBuffDamagesRate, Level, 1f);
                    uiBuffDamagesRate.Data = _tempBuffDamagesRate;
                }
            }

            if (uiDamageOverTimes != null)
            {
                if (Buff.damageOverTimes == null || Buff.damageOverTimes.Length == 0)
                {
                    uiDamageOverTimes.Hide();
                }
                else
                {
                    uiDamageOverTimes.isBonus = false;
                    uiDamageOverTimes.Show();
                    _tempDamageOverTimes.Clear();
                    GameDataHelpers.CombineDamages(Buff.damageOverTimes, _tempDamageOverTimes, Level, 1f);
                    uiDamageOverTimes.Data = _tempDamageOverTimes;
                }
            }

            if (uiStatusEffectApplyingsSelfWhenAttacking != null)
            {
                if (Buff.selfStatusEffectsWhenAttacking == null || Buff.selfStatusEffectsWhenAttacking.Length == 0)
                {
                    uiStatusEffectApplyingsSelfWhenAttacking.Hide();
                }
                else
                {
                    uiStatusEffectApplyingsSelfWhenAttacking.UpdateData(Buff.selfStatusEffectsWhenAttacking, Level, UIStatusEffectApplyingTarget.SelfWhenAttacking);
                    uiStatusEffectApplyingsSelfWhenAttacking.Show();
                }
            }

            if (uiStatusEffectApplyingsEnemyWhenAttacking != null)
            {
                if (Buff.enemyStatusEffectsWhenAttacking == null || Buff.enemyStatusEffectsWhenAttacking.Length == 0)
                {
                    uiStatusEffectApplyingsEnemyWhenAttacking.Hide();
                }
                else
                {
                    uiStatusEffectApplyingsEnemyWhenAttacking.UpdateData(Buff.enemyStatusEffectsWhenAttacking, Level, UIStatusEffectApplyingTarget.EnemyWhenAttacking);
                    uiStatusEffectApplyingsEnemyWhenAttacking.Show();
                }
            }

            if (uiStatusEffectApplyingsSelfWhenAttacked != null)
            {
                if (Buff.selfStatusEffectsWhenAttacked == null || Buff.selfStatusEffectsWhenAttacked.Length == 0)
                {
                    uiStatusEffectApplyingsSelfWhenAttacked.Hide();
                }
                else
                {
                    uiStatusEffectApplyingsSelfWhenAttacked.UpdateData(Buff.selfStatusEffectsWhenAttacked, Level, UIStatusEffectApplyingTarget.SelfWhenAttacked);
                    uiStatusEffectApplyingsSelfWhenAttacked.Show();
                }
            }

            if (uiStatusEffectApplyingsEnemyWhenAttacked != null)
            {
                if (Buff.enemyStatusEffectsWhenAttacked == null || Buff.enemyStatusEffectsWhenAttacked.Length == 0)
                {
                    uiStatusEffectApplyingsEnemyWhenAttacked.Hide();
                }
                else
                {
                    uiStatusEffectApplyingsEnemyWhenAttacked.UpdateData(Buff.enemyStatusEffectsWhenAttacked, Level, UIStatusEffectApplyingTarget.EnemyWhenAttacked);
                    uiStatusEffectApplyingsEnemyWhenAttacked.Show();
                }
            }

            if (uiIncreaseSkills != null)
            {
                if (Buff.increaseSkills == null || Buff.increaseSkills.Length == 0)
                {
                    uiIncreaseSkills.Hide();
                }
                else
                {
                    uiIncreaseSkills.isBonus = true;
                    uiIncreaseSkills.Show();
                    _tempIncreaseSkills.Clear();
                    GameDataHelpers.CombineSkills(Buff.increaseSkills, _tempIncreaseSkills, Level, 1f);
                    uiIncreaseSkills.Data = _tempIncreaseSkills;
                }
            }

            if (uiOverrideSkills != null)
            {
                if (!Buff.isOverrideSkills || Buff.overrideSkills == null || Buff.overrideSkills.Length == 0)
                {
                    uiOverrideSkills.Hide();
                }
                else
                {
                    uiOverrideSkills.isBonus = true;
                    uiOverrideSkills.Show();
                    _tempOverrideSkills.Clear();
                    GameDataHelpers.CombineSkills(Buff.overrideSkills, _tempOverrideSkills, Level, 1f);
                    uiOverrideSkills.Data = _tempOverrideSkills;
                }
            }

            if (uiStatusEffectResistances != null)
            {
                if (Buff.increaseStatusEffectResistances == null || Buff.increaseStatusEffectResistances.Length == 0)
                {
                    uiStatusEffectResistances.Hide();
                }
                else
                {
                    uiStatusEffectResistances.isBonus = true;
                    uiStatusEffectResistances.Show();
                    _tempStatusEffectResistances.Clear();
                    GameDataHelpers.CombineStatusEffectResistances(Buff.increaseStatusEffectResistances, _tempStatusEffectResistances, Level, 1f);
                    uiStatusEffectResistances.UpdateData(_tempStatusEffectResistances);
                }
            }

            if (uiBuffRemovals != null)
            {
                if (Buff.buffRemovals == null || Buff.buffRemovals.Length == 0)
                {
                    uiBuffRemovals.Hide();
                }
                else
                {
                    uiBuffRemovals.Show();
                    _tempBuffRemovals.Clear();
                    GameDataHelpers.CombineBuffRemovals(Buff.buffRemovals, _tempBuffRemovals, Level, 1f);
                    uiBuffRemovals.UpdateData(_tempBuffRemovals);
                }
            }

            if (disallowMoveObject != null)
                disallowMoveObject.SetActive(Data.buff.disallowMove);

            if (disallowSprintObject != null)
                disallowSprintObject.SetActive(Data.buff.disallowSprint);

            if (disallowWalkObject != null)
                disallowWalkObject.SetActive(Data.buff.disallowWalk);

            if (disallowJumpObject != null)
                disallowJumpObject.SetActive(Data.buff.disallowJump);

            if (disallowDashObject != null)
                disallowDashObject.SetActive(Data.buff.disallowDash);

            if (disallowCrouchObject != null)
                disallowCrouchObject.SetActive(Data.buff.disallowCrouch);

            if (disallowCrawlObject != null)
                disallowCrawlObject.SetActive(Data.buff.disallowCrawl);

            if (disallowAttackObject != null)
                disallowAttackObject.SetActive(Data.buff.disallowAttack);

            if (disallowUseSkillObject != null)
                disallowUseSkillObject.SetActive(Data.buff.disallowUseSkill);

            if (disallowUseItemObject != null)
                disallowUseItemObject.SetActive(Data.buff.disallowUseItem);

            if (freezeAnimationObject != null)
                freezeAnimationObject.SetActive(Data.buff.freezeAnimation);

            if (isHideObject != null)
                isHideObject.SetActive(Data.buff.isHide);

            if (isRevealsHideObject != null)
                isRevealsHideObject.SetActive(Data.buff.isRevealsHide);

            if (isBlindObject != null)
                isBlindObject.SetActive(Data.buff.isBlind);

            if (doNotRemoveOnDeadObject != null)
                doNotRemoveOnDeadObject.SetActive(Data.buff.doNotRemoveOnDead);

            if (muteFootstepSoundObject != null)
                muteFootstepSoundObject.SetActive(Data.buff.muteFootstepSound);

            if (isExtendDurationObject != null)
                isExtendDurationObject.SetActive(Data.buff.isExtendDuration);
        }
    }
}
