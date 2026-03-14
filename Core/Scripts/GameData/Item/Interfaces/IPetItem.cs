namespace MultiplayerARPG
{
    public partial interface IPetItem : IUsableItem, IItemWithMonsterCharacterEntity
    {
        public IncrementalFloat SummonDuration { get; }
        public bool NoSummonDuration { get; }
    }
}
