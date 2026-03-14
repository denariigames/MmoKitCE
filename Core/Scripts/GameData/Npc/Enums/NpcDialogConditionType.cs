namespace MultiplayerARPG
{
    public enum NpcDialogConditionType : byte
    {
        LevelMoreThanOrEqual,
        LevelLessThanOrEqual,
        QuestNotStarted,
        QuestOngoing,
        QuestTasksCompleted,
        QuestCompleted,
        FactionIs,
        PlayerCharacterIs,
        CustomByScriptableObject = 253,
        CustomByCallback = 254,
    }
}
