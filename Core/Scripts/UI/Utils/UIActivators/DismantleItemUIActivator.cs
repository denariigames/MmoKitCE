using UnityEngine;

namespace MultiplayerARPG
{
    public class DismantleItemUIActivator : MonoBehaviour
    {
        public GameObject[] activateObjects;

        private void LateUpdate()
        {
            if (activateObjects == null || GameInstance.ItemUIVisibilityManager == null)
                return;
            foreach (GameObject obj in activateObjects)
            {
                obj.SetActive(GameInstance.ItemUIVisibilityManager.IsDismantleItemDialogVisible());
            }
        }
    }
}
