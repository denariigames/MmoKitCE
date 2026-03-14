namespace MultiplayerARPG
{
    public static partial class GameExtensionInstance
    {
        public static CharacterStatsDelegate onIncreaseCharacterStats;
        public static CharacterStatsDelegate onDecreaseCharacterStats;
        public static CharacterStatsAndNumberDelegate onMultiplyCharacterStatsWithNumber;
        public static CharacterStatsDelegate onMultiplyCharacterStats;
        public static RandomCharacterStatsDelegate onRandomCharacterStats;
        public static CalculatedBuffDelegate onBuildCalculatedBuff;
        public static CalculatedItemBuffDelegate onBuildCalculatedItemBuff;
    }
}
