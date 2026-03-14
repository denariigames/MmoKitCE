using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.EQUIPMENT_MODEL_BONES_SETUP_BY_BONES_LIST_REPLACING_MANAGER_FILE, menuName = GameDataMenuConsts.EQUIPMENT_MODEL_BONES_SETUP_BY_BONES_LIST_REPLACING_MANAGER_MENU, order = GameDataMenuConsts.EQUIPMENT_MODEL_BONES_SETUP_BY_BONES_LIST_REPLACING_MANAGER_ORDER)]
    public class EquipmentModelBonesSetupByBonesListReplacingManager : BaseEquipmentModelBonesSetupManager
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

            if (!(characterModel is IModelWithSkinnedMeshRenderer skinnedMeshSrc))
            {
                Debug.LogWarning($"[{nameof(EquipmentModelBonesSetupByBonesListReplacingManager)}] Cannot setup bones for \"{instantiatedObject}\", character model \"{characterModel}\" is not a model with animator");
                return;
            }

            SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshSrc.SkinnedMeshRenderer;
            SkinnedMeshRenderer skinnedMesh = instantiatedObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMesh != null && skinnedMeshRenderer != null)
            {
                skinnedMesh.bones = skinnedMeshRenderer.bones;
                skinnedMesh.rootBone = skinnedMeshRenderer.rootBone;
                if (equipmentContainer.defaultModel != null)
                {
                    SkinnedMeshRenderer defaultSkinnedMesh = equipmentContainer.defaultModel.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (defaultSkinnedMesh != null)
                    {
                        skinnedMesh.bones = defaultSkinnedMesh.bones;
                        skinnedMesh.rootBone = defaultSkinnedMesh.rootBone;
                    }
                }
            }
        }
    }
}