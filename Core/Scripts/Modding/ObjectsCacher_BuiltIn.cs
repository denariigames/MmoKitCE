using Insthync.DevExtension;
using LiteNetLibManager;
using MultiplayerARPG.GameData.Model.Playables;

namespace MultiplayerARPG
{
    public static partial class ObjectsCacher
    {
        public static partial void CacheDevExtMethods()
        {
            // Cache LiteNetLibBehaviour
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(PlayerCharacterEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(MonsterCharacterEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(NpcEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(HarvestableEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(CurrencyDropEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(ExpDropEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(GoldDropEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(ItemDropEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(ItemsContainerEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(VehicleEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(BuildingEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(CampFireEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(DoorEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(QueuedWorkbenchEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(StorageEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(WorkbenchEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(WarpPortalEntity));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(CharacterControllerEntityMovement));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(SimpleCharacterControllerEntityMovement));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(NavMeshEntityMovement));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(SimpleNavMeshEntityMovement));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(RigidBodyEntityMovement2D));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(DefaultCharacterAttackComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(DefaultCharacterChargeComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(DefaultCharacterReloadComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(DefaultCharacterUseSkillComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(CharacterLadderComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(PlayerCharacterBuildingComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(PlayerCharacterCraftingComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(PlayerCharacterDealingComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(PlayerCharacterDuelingComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(PlayerCharacterNpcActionComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(PlayerCharacterPkComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(PlayerCharacterVendingComponent));
            LiteNetLibBehaviour.CacheElementsAndRpcs(typeof(LiteNetLibTransform));
            // Cache DevExt
            DevExtUtils.CacheInstanceDevExtMethods(typeof(PlayerCharacterEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(PlayerCharacterEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(MonsterCharacterEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(MonsterCharacterEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(NpcEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(NpcEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(HarvestableEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(HarvestableEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(CurrencyDropEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(CurrencyDropEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(ExpDropEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(ExpDropEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(GoldDropEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(GoldDropEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(ItemDropEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(ItemDropEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(ItemsContainerEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(ItemsContainerEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(VehicleEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(VehicleEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(BuildingEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(BuildingEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(CampFireEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(CampFireEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(DoorEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(DoorEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(QueuedWorkbenchEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(QueuedWorkbenchEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(StorageEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(StorageEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(WorkbenchEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(WorkbenchEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(WarpPortalEntity), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(WarpPortalEntity), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(PlayableCharacterModel), "SetEffectContainersBySetters");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(PlayableCharacterModel), "SetEquipmentContainersBySetters");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(PlayerCharacterController), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(PlayerCharacterController), "OnDestroy");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(ShooterPlayerCharacterController), "Awake");
            DevExtUtils.CacheInstanceDevExtMethods(typeof(ShooterPlayerCharacterController), "OnDestroy");
        }
    }
}
