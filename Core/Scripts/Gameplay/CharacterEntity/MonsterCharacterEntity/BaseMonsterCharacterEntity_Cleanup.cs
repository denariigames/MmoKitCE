namespace MultiplayerARPG
{
    public partial class BaseMonsterCharacterEntity
    {
        public override void Clean()
        {
            base.Clean();
            characterDatabase = null;
            faction = null;
            summoner = null;
            SpawnArea = null;
            SpawnPrefab = null;
#if !DISABLE_ADDRESSABLES
            SpawnAddressablePrefab = null;
#endif
            _looters?.Clear();
            _droppingItems?.Clear();
        }
    }
}