using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseEntitySetting : ScriptableObject
    {
        public abstract void InitialPlayerCharacterEntityComponents(BasePlayerCharacterEntity entity);
        public abstract void InitialMonsterCharacterEntityComponents(BaseMonsterCharacterEntity entity);
        public abstract void InitialHarvestableEntityComponents(HarvestableEntity entity);
        public abstract void InitialBuildingEntityComponents(BuildingEntity entity);
        public abstract void InitialVehicleEntityComponents(VehicleEntity entity);
    }
}
