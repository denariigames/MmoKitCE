namespace MultiplayerARPG
{
    public partial class HarvestableEntity
    {
        public override void Clean()
        {
            base.Clean();
            harvestable = null;
            onHarvestableDestroy?.RemoveAllListeners();
            onHarvestableDestroy = null;
            SpawnArea = null;
            SpawnPrefab = null;
#if !DISABLE_ADDRESSABLES
            SpawnAddressablePrefab = null;
#endif
        }
    }
}
