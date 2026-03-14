namespace MultiplayerARPG
{
    public struct UIDamageElementAmountData
    {
        public DamageElement damageElement;
        public MinMaxFloat amount;
        public UIDamageElementAmountData(DamageElement damageElement, MinMaxFloat amount)
        {
            this.damageElement = damageElement;
            this.amount = amount;
        }
    }
}
