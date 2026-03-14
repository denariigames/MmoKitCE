using UnityEngine;

namespace UtilsComponents
{
    public class ActivateObjectIfAnyActivated : MonoBehaviour
    {
        public GameObject[] anyObjects = new GameObject[0];
        public GameObject[] activateObjects = new GameObject[0];

        private void Update()
        {
            bool isActive = false;
            int i;
            for (i = 0; i < anyObjects.Length; ++i)
            {
                if (anyObjects[i].activeInHierarchy)
                {
                    isActive = true;
                    break;
                }
            }
            for (i = 0; i < activateObjects.Length; ++i)
            {
                if (activateObjects[i].activeSelf != isActive)
                    activateObjects[i].SetActive(isActive);
            }
        }
    }
}