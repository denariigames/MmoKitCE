namespace MultiplayerARPG
{
    [System.Serializable]
    public struct MovementRestriction
    {
        public static readonly MovementRestriction None = new MovementRestriction();
        public bool jumpRestricted;
        public bool dashRestricted;
        public bool turnRestricted;
    }
}
