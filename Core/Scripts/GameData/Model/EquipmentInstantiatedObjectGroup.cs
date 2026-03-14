using UnityEngine;

namespace MultiplayerARPG
{

    [System.Serializable]
    public class EquipmentInstantiatedObjectGroup
    {
        public GameObject[] instantiatedObjects;

        public void SetActive(bool isActive)
        {
            if (instantiatedObjects == null || instantiatedObjects.Length == 0)
                return;

            for (int i = 0; i < instantiatedObjects.Length; ++i)
            {
                if (instantiatedObjects[i].activeSelf != isActive)
                    instantiatedObjects[i].SetActive(isActive);
            }
        }
    }
}