namespace MultiplayerARPG
{
    [System.Flags]
    public enum GuildListFieldOptions : int
    {
        None = 0,
        GuildMessage = 1 << 0,
        GuildMessage2 = 1 << 1,
        Score = 1 << 2,
        Options = 1 << 3,
        AutoAcceptRequests = 1 << 4,
        Rank = 1 << 5,
        CurrentMembers = 1 << 6,
        MaxMembers = 1 << 7,
        All = GuildMessage | GuildMessage2 | Score | Options | AutoAcceptRequests | Rank | CurrentMembers | MaxMembers,
    }

    public static class GuildListFieldOptionsExtensions
    {
        public static bool Has(this GuildListFieldOptions self, GuildListFieldOptions flag)
        {
            return (self & flag) == flag;
        }
    }
}
