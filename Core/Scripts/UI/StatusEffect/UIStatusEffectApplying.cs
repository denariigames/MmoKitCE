using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIStatusEffectApplying : UISelectionEntry<UIStatusEffectApplyingData>
    {
        public StatusEffectApplying StatusEffectApplying { get { return Data.statusEffectApplying; } }
        public StatusEffect StatusEffect { get { return StatusEffectApplying.statusEffect; } }
        public int Level { get { return Data.targetLevel; } }
        public UIStatusEffectApplyingTarget ApplyingTarget { get { return Data.applyingTarget; } }
        public Buff Buff { get { return StatusEffect == null ? null : StatusEffect.Buff; } }
        public int BuffLevel { get { return StatusEffectApplying.buffLevel.GetAmount(Level); } }

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

        [Header("UI Elements")]
        public TextWrapper textApplyingTarget;
        public UIBuff uiBuff;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            textApplyingTarget = null;
            uiBuff = null;
        }

        protected override void UpdateData()
        {
            if (textApplyingTarget != null)
            {
                switch (ApplyingTarget)
                {
                    case UIStatusEffectApplyingTarget.Self:
                        textApplyingTarget.text = ZString.Format(
                            LanguageManager.GetText(formatApplyingTargetSelf),
                            StatusEffect == null ? LanguageManager.GetUnknowTitle() : StatusEffect.Title,
                            Level.ToString("N0"));
                        break;
                    case UIStatusEffectApplyingTarget.Enemy:
                        textApplyingTarget.text = ZString.Format(
                            LanguageManager.GetText(formatApplyingTargetEnemy),
                            StatusEffect == null ? LanguageManager.GetUnknowTitle() : StatusEffect.Title,
                            Level.ToString("N0"));
                        break;
                    case UIStatusEffectApplyingTarget.SelfWhenAttacking:
                        textApplyingTarget.text = ZString.Format(
                            LanguageManager.GetText(formatApplyingTargetSelfWhenAttacking),
                            StatusEffect == null ? LanguageManager.GetUnknowTitle() : StatusEffect.Title,
                            Level.ToString("N0"));
                        break;
                    case UIStatusEffectApplyingTarget.EnemyWhenAttacking:
                        textApplyingTarget.text = ZString.Format(
                            LanguageManager.GetText(formatApplyingTargetEnemyWhenAttacking),
                            StatusEffect == null ? LanguageManager.GetUnknowTitle() : StatusEffect.Title,
                            Level.ToString("N0"));
                        break;
                    case UIStatusEffectApplyingTarget.SelfWhenAttacked:
                        textApplyingTarget.text = ZString.Format(
                            LanguageManager.GetText(formatApplyingTargetSelfWhenAttacked),
                            StatusEffect == null ? LanguageManager.GetUnknowTitle() : StatusEffect.Title,
                            Level.ToString("N0"));
                        break;
                    case UIStatusEffectApplyingTarget.EnemyWhenAttacked:
                        textApplyingTarget.text = ZString.Format(
                            LanguageManager.GetText(formatApplyingTargetEnemyWhenAttacked),
                            StatusEffect == null ? LanguageManager.GetUnknowTitle() : StatusEffect.Title,
                            Level.ToString("N0"));
                        break;
                }
            }

            if (uiBuff != null)
            {
                uiBuff.Show();
                uiBuff.Data = new UIBuffData(Buff, BuffLevel);
            }
        }
    }
}
