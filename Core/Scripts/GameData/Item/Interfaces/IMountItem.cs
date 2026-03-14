namespace MultiplayerARPG
{
    public partial interface IMountItem : IUsableItem, IItemWithVehicleEntity
    {
        public IncrementalFloat MountDuration { get; }
        public bool NoMountDuration { get; }
    }
}
