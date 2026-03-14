using Cysharp.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIGuildSkill : UISelectionEntry<UIGuildSkillData>
    {
        public GuildSkill GuildSkill { get { return Data.guildSkill; } }
        public int Level { get { return Data.targetLevel; } }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_LEVEL);
        [Tooltip("Format => {0} = {Duration}")]
        public UILocaleKeySetting formatKeyCoolDownDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SKILL_COOLDOWN_DURATION);
        [Tooltip("Format => {0} = {Remains Duration}")]
        public UILocaleKeySetting formatKeyCoolDownRemainsDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Skill Type Title}")]
        public UILocaleKeySetting formatKeySkillType = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SKILL_TYPE);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public TextWrapper uiTextLevel;
        public Image imageIcon;
        public TextWrapper uiTextSkillType;
        public TextWrapper uiTextCoolDownDuration;
        public TextWrapper uiTextCoolDownRemainsDuration;
        public Image imageCoolDownGage;
        public GameObject[] countDownObjects;
        public GameObject[] noCountDownObjects;

        [Header("Passive Bonus")]
        public TextWrapper uiTextIncreaseMaxMember;
        public TextWrapper uiTextIncreaseExpGainPercentage;
        public TextWrapper uiTextIncreaseGoldGainPercentage;
        public TextWrapper uiTextIncreaseShareExpGainPercentage;
        public TextWrapper uiTextIncreaseShareGoldGainPercentage;
        public TextWrapper uiTextDecreaseExpLostPercentage;

        [Header("Buff")]
        public UIBuff uiSkillBuff;
        public bool showBuff;

        [Header("Events")]
        public UnityEvent onSetLevelZeroData = new UnityEvent();
        public UnityEvent onSetNonLevelZeroData = new UnityEvent();
        public UnityEvent onAbleToLevelUp = new UnityEvent();
        public UnityEvent onUnableToLevelUp = new UnityEvent();
        public UnityEvent onAbleToUse = new UnityEvent();
        public UnityEvent onUnableToUse = new UnityEvent();
        public UnityEvent onMaxedLevel = new UnityEvent();
        public UnityEvent onNotMaxedLevel = new UnityEvent();

        [Header("Options")]
        public UIGuildSkill uiNextLevelSkill;

        protected float _coolDownRemainsDuration;
        protected bool _dirtyIsCountDown;
        protected bool _dirtyAbleToLevelUp;
        protected bool _dirtyAbleToUse;
        protected bool _dirtyMaxedLevel;
        protected bool _forceUpdateUi = true;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            uiTextDescription = null;
            uiTextLevel = null;
            imageIcon = null;
            uiTextSkillType = null;
            uiTextCoolDownDuration = null;
            uiTextCoolDownRemainsDuration = null;
            imageCoolDownGage = null;
            countDownObjects.Nullify();
            noCountDownObjects.Nullify();
            uiTextIncreaseMaxMember = null;
            uiTextIncreaseExpGainPercentage = null;
            uiTextIncreaseGoldGainPercentage = null;
            uiTextIncreaseShareExpGainPercentage = null;
            uiTextIncreaseShareGoldGainPercentage = null;
            uiTextDecreaseExpLostPercentage = null;
            uiSkillBuff = null;
            onSetLevelZeroData?.RemoveAllListeners();
            onSetLevelZeroData = null;
            onSetNonLevelZeroData?.RemoveAllListeners();
            onSetNonLevelZeroData = null;
            onAbleToLevelUp?.RemoveAllListeners();
            onAbleToLevelUp = null;
            onUnableToLevelUp?.RemoveAllListeners();
            onUnableToLevelUp = null;
            onAbleToUse?.RemoveAllListeners();
            onAbleToUse = null;
            onUnableToUse?.RemoveAllListeners();
            onUnableToUse = null;
            onMaxedLevel?.RemoveAllListeners();
            onMaxedLevel = null;
            onNotMaxedLevel?.RemoveAllListeners();
            onNotMaxedLevel = null;
            uiNextLevelSkill = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _coolDownRemainsDuration = 0f;
        }

        public override void ManagedUpdate()
        {
            base.ManagedUpdate();

            if (_coolDownRemainsDuration > 0f)
            {
                _coolDownRemainsDuration -= Time.deltaTime;
                if (_coolDownRemainsDuration <= 0f)
                    _coolDownRemainsDuration = 0f;
            }
            else
            {
                _coolDownRemainsDuration = 0f;
            }

            // Update UIs
            bool isSkillActive = GuildSkill != null && GuildSkill.IsActive;
            float coolDownDuration = isSkillActive ? GuildSkill.GetCoolDownDuration(Level) : 0f;

            if (uiTextCoolDownDuration != null)
            {
                uiTextCoolDownDuration.SetGameObjectActive(isSkillActive && coolDownDuration > 0f);
                uiTextCoolDownDuration.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCoolDownDuration),
                    coolDownDuration.ToString("N0"));
            }

            if (uiTextCoolDownRemainsDuration != null)
            {
                uiTextCoolDownRemainsDuration.SetGameObjectActive(isSkillActive && _coolDownRemainsDuration > 0);
                uiTextCoolDownRemainsDuration.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCoolDownRemainsDuration),
                    _coolDownRemainsDuration.ToString("N0"));
            }

            if (imageCoolDownGage != null)
            {
                imageCoolDownGage.fillAmount = coolDownDuration <= 0 ? 0 : _coolDownRemainsDuration / coolDownDuration;
                imageCoolDownGage.gameObject.SetActive(imageCoolDownGage.fillAmount > 0f);
            }

            bool isCountDown = _coolDownRemainsDuration > 0f;
            if (_forceUpdateUi || _dirtyIsCountDown != isCountDown)
            {
                _dirtyIsCountDown = isCountDown;
                if (countDownObjects != null)
                {
                    foreach (GameObject obj in countDownObjects)
                    {
                        obj.SetActive(isCountDown);
                    }
                }
                if (noCountDownObjects != null)
                {
                    foreach (GameObject obj in noCountDownObjects)
                    {
                        obj.SetActive(!isCountDown);
                    }
                }
            }
        }

        protected void UpdateCoolDownRemainsDuration(float diffToChangeRemainsDuration = 0f)
        {
            if (_coolDownRemainsDuration <= 0f && GameInstance.PlayingCharacter != null && GuildSkill != null)
            {
                int indexOfSkillUsage = GameInstance.PlayingCharacter.IndexOfSkillUsage(SkillUsageType.GuildSkill, GuildSkill.DataId);
                if (indexOfSkillUsage >= 0 && Mathf.Abs(GameInstance.PlayingCharacter.SkillUsages[indexOfSkillUsage].coolDownRemainsDuration - _coolDownRemainsDuration) > diffToChangeRemainsDuration)
                    _coolDownRemainsDuration = GameInstance.PlayingCharacter.SkillUsages[indexOfSkillUsage].coolDownRemainsDuration;
                else
                    _coolDownRemainsDuration = 0f;
            }
        }

        protected override void UpdateUI()
        {
            UpdateCoolDownRemainsDuration(0f);
            IPlayerCharacterData targetPlayer = GameInstance.PlayingCharacter;

            bool ableToLevelUp = targetPlayer != null &&
                GuildSkill != null && Level < GuildSkill.MaxLevel &&
                GameInstance.JoinedGuild != null &&
                GameInstance.JoinedGuild.IsLeader(targetPlayer.Id) &&
                GameInstance.JoinedGuild.skillPoint > 0;
            if (_forceUpdateUi || _dirtyAbleToLevelUp != ableToLevelUp)
            {
                _dirtyAbleToLevelUp = ableToLevelUp;
                if (ableToLevelUp)
                    onAbleToLevelUp.Invoke();
                else
                    onUnableToLevelUp.Invoke();
            }

            bool ableToUse = targetPlayer != null && GuildSkill != null && GuildSkill.IsActive && Level > 0;
            if (_forceUpdateUi || _dirtyAbleToUse != ableToUse)
            {
                _dirtyAbleToUse = ableToUse;
                if (ableToUse)
                    onAbleToUse.Invoke();
                else
                    onUnableToUse.Invoke();
            }

            bool maxedLevel = GuildSkill != null && GuildSkill.MaxLevel <= Level;
            if (_forceUpdateUi || _dirtyMaxedLevel != maxedLevel)
            {
                _dirtyMaxedLevel = maxedLevel;
                if (maxedLevel)
                    onMaxedLevel.Invoke();
                else
                    onNotMaxedLevel.Invoke();
            }
        }

        protected override void UpdateData()
        {
            UpdateCoolDownRemainsDuration(1f);

            if (Level <= 0)
            {
                onSetLevelZeroData.Invoke();
            }
            else
            {
                onSetNonLevelZeroData.Invoke();
            }

            if (uiTextTitle != null)
            {
                uiTextTitle.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    GuildSkill == null ? LanguageManager.GetUnknowTitle() : GuildSkill.Title);
            }

            if (uiTextDescription != null)
            {
                uiTextDescription.text = ZString.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    GuildSkill == null ? LanguageManager.GetUnknowDescription() : GuildSkill.Description);
            }

            if (uiTextLevel != null)
            {
                uiTextLevel.text = ZString.Format(
                    LanguageManager.GetText(formatKeyLevel),
                    Level.ToString("N0"));
            }

#if UNITY_EDITOR || !UNITY_SERVER
            imageIcon.SetImageGameDataIcon(GuildSkill);
#endif

            if (uiTextSkillType != null)
            {
                switch (GuildSkill.SkillType)
                {
                    case GuildSkillType.Active:
                        uiTextSkillType.text = ZString.Format(
                            LanguageManager.GetText(formatKeySkillType),
                            LanguageManager.GetText(UISkillTypeKeys.UI_SKILL_TYPE_ACTIVE.ToString()));
                        break;
                    case GuildSkillType.Passive:
                        uiTextSkillType.text = ZString.Format(
                            LanguageManager.GetText(formatKeySkillType),
                            LanguageManager.GetText(UISkillTypeKeys.UI_SKILL_TYPE_PASSIVE.ToString()));
                        break;
                }
            }

            if (uiTextIncreaseMaxMember != null)
            {
                int amount = GuildSkill.GetIncreaseMaxMember(Level);
                uiTextIncreaseMaxMember.SetGameObjectActive(amount != 0);
                uiTextIncreaseMaxMember.text = ZString.Format(
                    LanguageManager.GetText(UIFormatKeys.UI_FORMAT_INCREASE_MAX_MEMBER.ToString()),
                    amount.ToString("N0"));
            }

            if (uiTextIncreaseExpGainPercentage != null)
            {
                float amount = GuildSkill.GetIncreaseExpGainPercentage(Level);
                uiTextIncreaseExpGainPercentage.SetGameObjectActive(amount != 0);
                uiTextIncreaseExpGainPercentage.text = ZString.Format(
                    LanguageManager.GetText(UIFormatKeys.UI_FORMAT_INCREASE_EXP_GAIN_PERCENTAGE.ToString()),
                    amount.ToString("N2"));
            }

            if (uiTextIncreaseGoldGainPercentage != null)
            {
                float amount = GuildSkill.GetIncreaseGoldGainPercentage(Level);
                uiTextIncreaseGoldGainPercentage.SetGameObjectActive(amount != 0);
                uiTextIncreaseGoldGainPercentage.text = ZString.Format(
                    LanguageManager.GetText(UIFormatKeys.UI_FORMAT_INCREASE_GOLD_GAIN_PERCENTAGE.ToString()),
                    amount.ToString("N2"));
            }

            if (uiTextIncreaseShareExpGainPercentage != null)
            {
                float amount = GuildSkill.GetIncreaseShareExpGainPercentage(Level);
                uiTextIncreaseShareExpGainPercentage.SetGameObjectActive(amount != 0);
                uiTextIncreaseShareExpGainPercentage.text = ZString.Format(
                    LanguageManager.GetText(UIFormatKeys.UI_FORMAT_INCREASE_SHARE_EXP_GAIN_PERCENTAGE.ToString()),
                    amount.ToString("N2"));
            }

            if (uiTextIncreaseShareGoldGainPercentage != null)
            {
                float amount = GuildSkill.GetIncreaseShareGoldGainPercentage(Level);
                uiTextIncreaseShareGoldGainPercentage.SetGameObjectActive(amount != 0);
                uiTextIncreaseShareGoldGainPercentage.text = ZString.Format(
                    LanguageManager.GetText(UIFormatKeys.UI_FORMAT_INCREASE_SHARE_GOLD_GAIN_PERCENTAGE.ToString()),
                    amount.ToString("N2"));
            }

            if (uiTextDecreaseExpLostPercentage != null)
            {
                float amount = GuildSkill.GetDecreaseExpLostPercentage(Level);
                uiTextDecreaseExpLostPercentage.SetGameObjectActive(amount != 0);
                uiTextDecreaseExpLostPercentage.text = ZString.Format(
                    LanguageManager.GetText(UIFormatKeys.UI_FORMAT_DECREASE_EXP_PENALTY_PERCENTAGE.ToString()),
                    amount.ToString("N2"));
            }

            if (uiSkillBuff != null)
            {
                if (!GuildSkill.IsActive && !showBuff)
                {
                    uiSkillBuff.Hide();
                }
                else
                {
                    uiSkillBuff.Show();
                    uiSkillBuff.Data = new UIBuffData(GuildSkill.Buff, Level);
                }
            }

            if (uiNextLevelSkill != null)
            {
                if (Level + 1 > GuildSkill.MaxLevel)
                {
                    uiNextLevelSkill.Hide();
                }
                else
                {
                    uiNextLevelSkill.Data = new UIGuildSkillData(GuildSkill, Level + 1);
                    uiNextLevelSkill.Show();
                }
            }
        }

        public void OnClickAdd()
        {
            if (GameInstance.JoinedGuild == null)
                return;
            GameInstance.ClientGuildHandlers.RequestIncreaseGuildSkillLevel(new RequestIncreaseGuildSkillLevelMessage()
            {
                dataId = GuildSkill.DataId,
            }, ClientGuildActions.ResponseIncreaseGuildSkillLevel);
        }

        public void OnClickUse()
        {
            if (GameInstance.JoinedGuild == null)
                return;
            GameInstance.PlayingCharacterEntity.CallCmdUseGuildSkill(GuildSkill.DataId);
        }
    }
}
