namespace MultiplayerARPG.MMO
{
    [System.Serializable]
    public struct PostgreSQLConfig
    {
        public string pgAddress;
        public int? pgPort;
        public string pgUsername;
        public string pgPassword;
        public string pgDbName;
        public string pgConnectionString;
    }
}
