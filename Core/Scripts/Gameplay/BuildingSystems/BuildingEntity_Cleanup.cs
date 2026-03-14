namespace MultiplayerARPG
{
    public partial class BuildingEntity
    {
        public override void Clean()
        {
            base.Clean();
            buildingTypes?.Clear();
            droppingItems?.Clear();
            repairs?.Clear();
            onBuildingDestroy?.RemoveAllListeners();
            onBuildingDestroy = null;
            onBuildingConstruct?.RemoveAllListeners();
            onBuildingConstruct = null;
            BuildingTypes?.Clear();
            BuildingArea = null;
            Builder = null;
            _cacheRepairs?.Clear();
            _triggerObjects?.Clear();
            _children?.Clear();
            _buildingMaterials?.Clear();
        }
    }
}
