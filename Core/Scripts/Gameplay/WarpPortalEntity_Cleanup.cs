namespace MultiplayerARPG
{
    public partial class WarpPortalEntity
    {
        public override void Clean()
        {
            base.Clean();
            warpSignals?.Nullify();
            warpToMapInfo = null;
        }
    }
}