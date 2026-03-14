namespace MultiplayerARPG
{
    public partial class VehicleEntity
    {
        public override void Clean()
        {
            base.Clean();
            vehicleType = null;
            seats?.Clear();
            onVehicleDestroy?.RemoveAllListeners();
            onVehicleDestroy = null;
            Resistances?.Clear();
            Armors?.Clear();
            _passengers?.Clear();
            _spawnEvents?.Clear();
            _cacheBuff = null;
        }
    }
}