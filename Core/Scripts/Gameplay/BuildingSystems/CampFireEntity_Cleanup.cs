namespace MultiplayerARPG
{
    public partial class CampFireEntity
    {
        public override void Clean()
        {
            base.Clean();
            onInitialTurnOn?.RemoveAllListeners();
            onInitialTurnOn = null;
            onInitialTurnOff?.RemoveAllListeners();
            onInitialTurnOff = null;
            onTurnOn?.RemoveAllListeners();
            onTurnOn = null;
            onTurnOff?.RemoveAllListeners();
            onTurnOff = null;
            _convertRemainsDuration?.Clear();
            _cacheFuelItems?.Clear();
            _cacheConvertItems?.Clear();
            _preparedConvertItems?.Clear();
        }
    }
}
