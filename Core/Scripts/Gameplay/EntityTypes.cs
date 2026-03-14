namespace MultiplayerARPG
{
    /// <summary>
    /// 0 = NULL
    /// 1 = Player Character Entity
    /// 2 = Monster Character Entity
    /// Add your as you wish, but must not use value existed here
    /// </summary>
    public static partial class EntityTypes
    {
        public static readonly byte Player = 1;
        public static readonly byte Monster = 2;
    }
}
