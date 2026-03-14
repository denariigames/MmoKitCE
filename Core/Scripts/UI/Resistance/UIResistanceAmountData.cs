namespace MultiplayerARPG
{
    public struct UIResistanceAmountData
    {
        public DamageElement damageElement;
        public float amount;
        public UIResistanceAmountData(DamageElement damageElement, float amount)
        {
            this.damageElement = damageElement;
            this.amount = amount;
        }
    }
}
