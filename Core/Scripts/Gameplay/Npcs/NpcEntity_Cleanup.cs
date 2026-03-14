namespace MultiplayerARPG
{
    public partial class NpcEntity
    {
        public override void Clean()
        {
            base.Clean();
            startDialog = null;
            graph = null;
            characterUiTransform = null;
            miniMapUiTransform = null;
            questIndicatorContainer = null;
            if (_uiNpcEntity != null)
                Destroy(_uiNpcEntity.gameObject);
            _uiNpcEntity = null;
            if (_questIndicator != null)
                Destroy(_questIndicator.gameObject);
            _questIndicator = null;
        }
    }
}
