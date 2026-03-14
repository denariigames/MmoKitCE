namespace MultiplayerARPG
{
    public struct UIQuestTaskData
    {
        public QuestTask questTask;
        public int progress;
        public UIQuestTaskData(QuestTask questTask, int progress)
        {
            this.questTask = questTask;
            this.progress = progress;
        }
    }
}
