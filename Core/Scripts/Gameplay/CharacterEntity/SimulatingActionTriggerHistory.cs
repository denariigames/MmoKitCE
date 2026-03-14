namespace MultiplayerARPG
{
    public struct SimulatingActionTriggerHistory
    {
        public int TriggeredIndex { get; set; }
        public int TriggerLength { get; private set; }

        public SimulatingActionTriggerHistory(int triggerLength)
        {
            TriggeredIndex = 0;
            TriggerLength = triggerLength;
        }
    }
}
