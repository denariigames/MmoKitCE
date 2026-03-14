namespace MultiplayerARPG
{
    public partial class ItemsContainerEntity
    {
        public override void Clean()
        {
            base.Clean();
            onPickedUp?.RemoveAllListeners();
            onPickedUp = null;
            Looters?.Clear();
        }
    }
}
