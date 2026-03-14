namespace MultiplayerARPG
{
    public struct UIStatusEffectResistanceData
    {
        public StatusEffect statusEffect;
        public float amount;
        public UIStatusEffectResistanceData(StatusEffect statusEffect, float amount)
        {
            this.statusEffect = statusEffect;
            this.amount = amount;
        }
    }
}
