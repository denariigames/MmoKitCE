namespace MultiplayerARPG
{
    public partial class WorkbenchEntity
    {
        public override void Clean()
        {
            base.Clean();
            _cacheItemCrafts?.Clear();
        }
    }
}
