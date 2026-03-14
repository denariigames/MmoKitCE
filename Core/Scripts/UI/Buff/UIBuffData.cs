namespace MultiplayerARPG
{
    public struct UIBuffData
    {
        public Buff buff;
        public int targetLevel;
        public UIBuffData(Buff buff, int targetLevel)
        {
            this.buff = buff;
            this.targetLevel = targetLevel;
        }
    }
}
