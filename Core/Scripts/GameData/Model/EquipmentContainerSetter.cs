using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class EquipmentContainerSetter : MonoBehaviour
    {

        [Header("Single instantiated object setting")]
        public GameObject defaultModel;
        public GameObject[] instantiatedObjects;

        [Header("Multiple instantiated objects setting")]
        public EquipmentInstantiatedObjectGroup defaultInstantiatedObjectGroup;
        public EquipmentInstantiatedObjectGroup[] instantiatedObjectGroups;

#if UNITY_EDITOR
        [Header("Set Default Model Tool")]
        public int childIndex;
        [InspectorButton(nameof(SetDefaultModelByChildIndex))]
        public bool setDefaultModelByChildIndex;
        [InspectorButton(nameof(SetInstantiatedObjectsByContainersChildren))]
        public bool setInstantiatedObjectsByContainersChildren;
#endif

        public void ApplyToCharacterModel(BaseCharacterModel characterModel)
        {
            if (characterModel == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Logging.LogWarning(ToString(), "Cannot find character model");
#endif
                return;
            }
            bool hasChanges = false;
            bool isFound = false;
            List<EquipmentContainer> equipmentContainers = new List<EquipmentContainer>();
            if (characterModel.EquipmentContainers != null)
                equipmentContainers.AddRange(characterModel.EquipmentContainers);
            for (int i = 0; i < equipmentContainers.Count; ++i)
            {
                EquipmentContainer equipmentContainer = equipmentContainers[i];
                if (equipmentContainer.transform == transform)
                {
                    isFound = true;
                    hasChanges = true;
                    equipmentContainer.equipSocket = name;
                    equipmentContainer.transform = transform;
                    equipmentContainer.defaultModel = defaultModel;
                    equipmentContainer.instantiatedObjects = instantiatedObjects;
                    equipmentContainer.instantiatedObjectGroups = instantiatedObjectGroups;
                    equipmentContainers[i] = equipmentContainer;
                    break;
                }
            }
            if (!isFound)
            {
                hasChanges = true;
                EquipmentContainer newEquipmentContainer = new EquipmentContainer();
                newEquipmentContainer.equipSocket = name;
                newEquipmentContainer.transform = transform;
                newEquipmentContainer.defaultModel = defaultModel;
                newEquipmentContainer.instantiatedObjects = instantiatedObjects;
                newEquipmentContainer.instantiatedObjectGroups = instantiatedObjectGroups;
                equipmentContainers.Add(newEquipmentContainer);
            }
            if (hasChanges)
            {
                characterModel.EquipmentContainers = equipmentContainers.ToArray();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Set Default Model By Child Index")]
        public virtual void SetDefaultModelByChildIndex()
        {
            if (childIndex < 0 || childIndex >= transform.childCount)
            {
                Debug.LogError("Can't set default model, invalid wrong child index.");
                return;
            }
            defaultModel = transform.GetChild(childIndex).gameObject;
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Set Instantiated Objects By Containers Children")]
        public void SetInstantiatedObjectsByContainersChildren()
        {
            if (transform == null || transform.childCount == 0)
                return;
            List<GameObject> instantiatedObjects = new List<GameObject>();
            int containersChildCount = transform.childCount;
            for (int i = 0; i < containersChildCount; ++i)
            {
                if (transform.GetChild(i) == null ||
                    transform.GetChild(i).gameObject == defaultModel)
                    continue;
                instantiatedObjects.Add(transform.GetChild(i).gameObject);
            }
            this.instantiatedObjects = instantiatedObjects.ToArray();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
