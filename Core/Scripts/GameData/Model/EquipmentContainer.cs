using UnityEngine;

namespace MultiplayerARPG
{

    [System.Serializable]
    public struct EquipmentContainer
    {
        public string equipSocket;
        public Transform transform;

        [Header("Single instantiated object setting")]
        public GameObject defaultModel;
        public GameObject[] instantiatedObjects;

        [Header("Multiple instantiated objects setting")]
        public EquipmentInstantiatedObjectGroup defaultInstantiatedObjectGroup;
        public EquipmentInstantiatedObjectGroup[] instantiatedObjectGroups;

#if UNITY_EDITOR
        [Header("Testing tools")]
        [Tooltip("Index of instantiate object which you want to test activation by character model's context menu")]
        public int activatingInstantiateObjectIndex;
#endif

        public void SetActiveDefaultModel(bool isActive)
        {
            if (defaultModel == null || defaultModel.activeSelf == isActive)
                return;
            defaultModel.SetActive(isActive);
        }

        public void SetActiveDefaultModelGroup(bool isActive)
        {
            if (defaultInstantiatedObjectGroup == null)
                return;
            defaultInstantiatedObjectGroup.SetActive(isActive);
        }

        public void DeactivateInstantiatedObjects()
        {
            if (instantiatedObjects == null || instantiatedObjects.Length == 0)
                return;
            // Deactivate all objects
            foreach (GameObject instantiatedObject in instantiatedObjects)
            {
                if (instantiatedObject == null || !instantiatedObject.activeSelf) continue;
                instantiatedObject.SetActive(false);
            }
        }

        public bool ActivateInstantiatedObject(int index)
        {
            if (instantiatedObjects == null || instantiatedObjects.Length == 0)
                return false;
            // Deactivate all objects
            DeactivateInstantiatedObjects();
            if (index < 0 || index >= instantiatedObjects.Length)
                return false;
            // Activate only one object
            if (instantiatedObjects[index] == null || instantiatedObjects[index].activeSelf)
                return false;
            instantiatedObjects[index].SetActive(true);
            return true;
        }

        public void DeactivateInstantiatedObjectGroups()
        {
            if (instantiatedObjectGroups == null || instantiatedObjectGroups.Length == 0)
                return;
            // Deactivate all objects
            foreach (EquipmentInstantiatedObjectGroup instantiatedObject in instantiatedObjectGroups)
            {
                if (instantiatedObject == null) continue;
                instantiatedObject.SetActive(false);
            }
        }

        public bool ActivateInstantiatedObjectGroup(int index)
        {
            if (instantiatedObjectGroups == null || instantiatedObjectGroups.Length == 0)
                return false;
            // Deactivate all objects
            DeactivateInstantiatedObjectGroups();
            if (index < 0 || index >= instantiatedObjectGroups.Length)
                return false;
            // Activate only one object
            if (instantiatedObjectGroups[index] == null)
                return false;
            instantiatedObjectGroups[index].SetActive(true);
            return true;
        }
    }
}