namespace MultiplayerARPG
{
    public struct UIBuffRemovalData
    {
        public BuffRemoval removal;
        public float amount;
        public UIBuffRemovalData(BuffRemoval removal, float amount)
        {
            this.removal = removal;
            this.amount = amount;
        }
    }
}
