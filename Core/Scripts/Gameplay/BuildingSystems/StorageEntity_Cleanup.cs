namespace MultiplayerARPG
{
    public partial class StorageEntity
    {
        public override void Clean()
        {
            base.Clean();
            onInitialOpen?.RemoveAllListeners();
            onInitialOpen = null;
            onInitialClose?.RemoveAllListeners();
            onInitialClose = null;
            onOpen?.RemoveAllListeners();
            onOpen = null;
            onClose?.RemoveAllListeners();
            onClose = null;
        }
    }
}
