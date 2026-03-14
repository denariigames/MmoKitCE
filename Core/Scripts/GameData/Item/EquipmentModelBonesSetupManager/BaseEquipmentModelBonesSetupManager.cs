using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseEquipmentModelBonesSetupManager : ScriptableObject
    {
        public abstract void Setup(BaseCharacterModel characterModel, EquipmentModel equipmentModel, GameObject instantiatedObject, BaseEquipmentEntity instantiatedEntity, EquipmentInstantiatedObjectGroup instantiatedObjectGroup, EquipmentContainer equipmentContainer);
    }
}