namespace MultiplayerARPG
{
    public struct UIStatusEffectApplyingData
    {
        public StatusEffectApplying statusEffectApplying;
        public int targetLevel;
        public UIStatusEffectApplyingTarget applyingTarget;
        public UIStatusEffectApplyingData(StatusEffectApplying statusEffectApplying, int targetLevel, UIStatusEffectApplyingTarget applyingTarget)
        {
            this.statusEffectApplying = statusEffectApplying;
            this.targetLevel = targetLevel;
            this.applyingTarget = applyingTarget;
        }
    }
}