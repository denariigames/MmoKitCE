using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultEntitySetting : BaseEntitySetting
    {
        [Header("Settings for Player Character Entity")]
        public bool enablePlayerCharacterLadder = true;
        public bool enablePlayerCharacterBuilding = true;
        public bool enablePlayerCharacterCrafting = true;
        public bool enablePlayerCharacterDealing = true;
        public bool enablePlayerCharacterDueling = true;
        public bool enablePlayerCharacterVending = true;
        public bool enablePlayerCharacterPk = true;

        public override void InitialPlayerCharacterEntityComponents(BasePlayerCharacterEntity entity)
        {
            if (enablePlayerCharacterLadder)
                entity.gameObject.GetOrAddComponent<CharacterLadderComponent>();
            if (enablePlayerCharacterBuilding)
                entity.gameObject.GetOrAddComponent<PlayerCharacterBuildingComponent>();
            if (enablePlayerCharacterCrafting)
                entity.gameObject.GetOrAddComponent<PlayerCharacterCraftingComponent>();
            if (enablePlayerCharacterDealing)
                entity.gameObject.GetOrAddComponent<PlayerCharacterDealingComponent>();
            if (enablePlayerCharacterDueling)
                entity.gameObject.GetOrAddComponent<PlayerCharacterDuelingComponent>();
            if (enablePlayerCharacterVending)
                entity.gameObject.GetOrAddComponent<PlayerCharacterVendingComponent>();
            if (enablePlayerCharacterPk)
                entity.gameObject.GetOrAddComponent<PlayerCharacterPkComponent>();
        }

        public override void InitialMonsterCharacterEntityComponents(BaseMonsterCharacterEntity entity)
        {
        }

        public override void InitialHarvestableEntityComponents(HarvestableEntity entity)
        {
        }

        public override void InitialBuildingEntityComponents(BuildingEntity entity)
        {
        }

        public override void InitialVehicleEntityComponents(VehicleEntity entity)
        {
        }
    }
}
