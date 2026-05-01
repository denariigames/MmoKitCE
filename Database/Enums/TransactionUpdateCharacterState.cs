namespace MultiplayerARPG.MMO
{
    [System.Flags]
    public enum TransactionUpdateCharacterState : uint
    {
        None = 0,
        Character = 1 << 0,
        Pk = 1 << 1,
        Mount = 1 << 2,
        Attributes = 1 << 3,
        Skills = 1 << 4,
        SkillUsages = 1 << 5,
        Buffs = 1 << 6,
        Items = 1 << 7,
        Summons = 1 << 8,
        Hotkeys = 1 << 9,
        Quests = 1 << 10,
        Currencies = 1 << 11,
        ServerCustomData = 1 << 12, // Server bools, ints, floats and so on
        PrivateCustomData = 1 << 13, // Private bools, ints, floats and so on
        PublicCustomData = 1 << 14, // Public bools, ints, floats and so on
        All = Character | Pk | Mount | Attributes | Skills | SkillUsages | Buffs | Items | Summons | Hotkeys | Quests | Currencies | ServerCustomData | PrivateCustomData | PublicCustomData,
    }

    public static class TransactionUpdateCharacterStateExtentions
    {
        public static bool Has(this TransactionUpdateCharacterState self, TransactionUpdateCharacterState flag)
        {
            return (self & flag) == flag;
        }
    }
}
