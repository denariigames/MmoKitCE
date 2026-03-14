namespace MultiplayerARPG
{
    public partial class QueuedWorkbenchEntity
    {
        public override void Clean()
        {
            base.Clean();
            itemCraftFormulas.Nullify();
            _cacheItemCraftFormulas?.Clear();
        }
    }
}
