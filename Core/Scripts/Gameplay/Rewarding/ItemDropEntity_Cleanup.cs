namespace MultiplayerARPG
{
    public partial class ItemDropEntity
    {
        public override void Clean()
        {
            base.Clean();
            modelContainer = null;
            onPickedUp?.RemoveAllListeners();
            onPickedUp = null;
            DropItems?.Clear();
            Looters?.Clear();
            SpawnArea = null;
            SpawnPrefab = null;
#if !DISABLE_ADDRESSABLES
            SpawnAddressablePrefab = null;
#endif
            _dropModel = null;
        }
    }
}
