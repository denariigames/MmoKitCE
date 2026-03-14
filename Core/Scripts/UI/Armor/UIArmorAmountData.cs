namespace MultiplayerARPG
{
    public struct UIArmorAmountData
    {
        public DamageElement damageElement;
        public float amount;
        public UIArmorAmountData(DamageElement damageElement, float amount)
        {
            this.damageElement = damageElement;
            this.amount = amount;
        }
    }
}
