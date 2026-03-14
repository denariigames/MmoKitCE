using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.EQUIPMENT_MODEL_BONES_SETUP_BY_HUMAN_BODY_BONES_MANAGER_FILE, menuName = GameDataMenuConsts.EQUIPMENT_MODEL_BONES_SETUP_BY_HUMAN_BODY_BONES_MANAGER_MENU, order = GameDataMenuConsts.EQUIPMENT_MODEL_BONES_SETUP_BY_HUMAN_BODY_BONES_MANAGER_ORDER)]
    public class EquipmentModelBonesSetupByHumanBodyBonesManager : BaseEquipmentModelBonesSetupManager
    {
        public override void Setup(BaseCharacterModel characterModel, EquipmentModel equipmentModel, GameObject instantiatedObject, BaseEquipmentEntity instantiatedEntity, EquipmentInstantiatedObjectGroup instantiatedObjectGroup, EquipmentContainer equipmentContainer)
        {
            if (GameInstance.Singleton.DimensionType != DimensionType.Dimension3D)
                return;

            SetupForObject(characterModel, instantiatedObject, equipmentContainer);
            if (instantiatedObjectGroup != null && instantiatedObjectGroup.instantiatedObjects != null)
            {
                foreach (GameObject obj in instantiatedObjectGroup.instantiatedObjects)
                {
                    SetupForObject(characterModel, obj, equipmentContainer);
                }
            }
        }

        private void SetupForObject(BaseCharacterModel characterModel, GameObject instantiatedObject, EquipmentContainer equipmentContainer)
        {
            if (instantiatedObject == null)
                return;

            if (!(characterModel is IModelWithAnimator animatorSrc))
            {
                Debug.LogWarning($"[{nameof(EquipmentModelBonesSetupByBoneNamesManager)}] Cannot setup bones for \"{instantiatedObject}\", character model \"{characterModel}\" is not a model with animator");
                return;
            }

            EquipmentModelBonesSetupByHumanBodyBonesUpdater updater = instantiatedObject.GetOrAddComponent<EquipmentModelBonesSetupByHumanBodyBonesUpdater>();
            if (equipmentContainer.defaultModel != null)
                updater.PrepareTransforms(equipmentContainer.defaultModel.GetComponentInChildren<Animator>(), instantiatedObject.GetComponentInChildren<Animator>());
            else
                updater.PrepareTransforms(animatorSrc.Animator, instantiatedObject.GetComponentInChildren<Animator>());
        }
    }
}