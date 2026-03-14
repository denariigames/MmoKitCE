namespace MultiplayerARPG
{
    public enum SkillType : byte
    {
        Active,
        Passive,
        [System.Obsolete]
        /// <summary>
        /// TODO: Planned to remove it in the future, use `Active` instead.
        /// </summary>
        CraftItem,
    }
}
