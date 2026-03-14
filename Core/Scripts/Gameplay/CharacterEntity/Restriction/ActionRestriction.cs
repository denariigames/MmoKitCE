namespace MultiplayerARPG
{
    [System.Serializable]
    public struct ActionRestriction
    {
        public bool restrictedWhileAirborne;
        public bool restrictedWhileCrouching;
        public bool restrictedWhileCrouchMoving;
        public bool restrictedWhileCrawling;
        public bool restrictedWhileCrawlMoving;
    }
}
