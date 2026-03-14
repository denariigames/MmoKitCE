namespace MultiplayerARPG
{
    public struct UIDamageElementInflictionData
    {
        public DamageElement damageElement;
        public float infliction;
        public UIDamageElementInflictionData(DamageElement damageElement, float infliction)
        {
            this.damageElement = damageElement;
            this.infliction = infliction;
        }
    }
}
