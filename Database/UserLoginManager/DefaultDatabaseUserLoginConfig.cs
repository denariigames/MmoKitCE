namespace MultiplayerARPG.MMO
{
    [System.Serializable]
    public struct DefaultDatabaseUserLoginConfig
    {
        public string PasswordSaltPrefix { get; set; }
        public string PasswordSaltPostfix { get; set; }
    }
}
