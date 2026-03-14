namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class QuestRequirement
    {
        public PlayerCharacter character = null;
        public Faction faction = null;
        public int level = 0;
        public Quest[] completedQuests = new Quest[0];
    }
}
