namespace MultiplayerARPG
{
    public partial class BaseRewardDropEntity
    {
        public override void Clean()
        {
            base.Clean();
            for (int i = 0; i < appearanceSettings.Count; ++i)
            {
                appearanceSettings[i].Clean();
            }
            appearanceSettings?.Clear();
            onPickedUp?.RemoveAllListeners();
            onPickedUp = null;
            Looters?.Clear();
            SpawnArea = null;
            SpawnPrefab = null;
#if !DISABLE_ADDRESSABLES
            SpawnAddressablePrefab = null;
#endif
            _allActivatingObjects.Nullify();
            _allActivatingObjects?.Clear();
            _allActivatingObjects = null;
        }
    }
}