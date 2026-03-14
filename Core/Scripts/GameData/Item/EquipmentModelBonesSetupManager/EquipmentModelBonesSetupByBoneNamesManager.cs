using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.EQUIPMENT_MODEL_BONES_SETUP_BY_BONE_NAMES_MANAGER_FILE, menuName = GameDataMenuConsts.EQUIPMENT_MODEL_BONES_SETUP_BY_BONE_NAMES_MANAGER_MENU, order = GameDataMenuConsts.EQUIPMENT_MODEL_BONES_SETUP_BY_BONE_NAMES_MANAGER_ORDER)]
    public class EquipmentModelBonesSetupByBoneNamesManager : BaseEquipmentModelBonesSetupManager
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
                Debug.LogWarning($"[{nameof(EquipmentModelBonesSetupByBoneNamesManager)}] Cannot setup bones for \"{instantiatedObject}\", character model \"{characterModel}\" is not a model with skinned mesh");
                return;
            }

            SkinnedMeshRenderer newSkinnedMesh = instantiatedObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (newSkinnedMesh == null)
                return;

            // Prepare bone maps by get bones from skinned mesh renderer
            Dictionary<string, Transform> defaultBoneMap = new Dictionary<string, Transform>();
            if (equipmentContainer.defaultModel != null)
                StoreToBoneMap(equipmentContainer.defaultModel.GetComponentInChildren<SkinnedMeshRenderer>(), defaultBoneMap);
            Dictionary<string, Transform> characterBoneMap = new Dictionary<string, Transform>();
            StoreToBoneMap(skinnedMeshSrc.SkinnedMeshRenderer, characterBoneMap);

            // Set new model bones by using default model bones or character model bones
            Transform[] newBones = new Transform[newSkinnedMesh.bones.Length];
            for (int i = 0; i < newSkinnedMesh.bones.Length; ++i)
            {
                Transform newBone = newSkinnedMesh.bones[i];
                // Set bone by using default model bones
                if (defaultBoneMap.TryGetValue(newBone.name, out newBones[i]))
                    continue;
                // Cannot find from default model?, so try find from character
                if (characterBoneMap.TryGetValue(newBone.name, out newBones[i]))
                    continue;
                // Really cannot find the bone, show error message
                Debug.LogWarning($"[{nameof(EquipmentModelBonesSetupByBoneNamesManager)}] {instantiatedObject} unable to find mapped bone for \"{newBone}\"");
            }
            newSkinnedMesh.bones = newBones;
        }

        private void StoreToBoneMap(SkinnedMeshRenderer renderer, Dictionary<string, Transform> map)
        {
            if (renderer == null)
                return;

            foreach (Transform bone in renderer.rootBone.GetComponentsInChildren<Transform>())
            {
                map[bone.name] = bone;
            }
        }
    }
}