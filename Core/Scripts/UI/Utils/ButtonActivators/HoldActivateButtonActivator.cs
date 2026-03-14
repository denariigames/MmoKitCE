using UnityEngine;

namespace MultiplayerARPG
{
    public class HoldActivateButtonActivator : MonoBehaviour
    {
        public GameObject[] activateObjects = new GameObject[0];

        private void LateUpdate()
        {
            bool canActivate = BasePlayerCharacterController.Singleton.ShouldShowHoldActivateButtons();
            foreach (GameObject obj in activateObjects)
            {
                obj.SetActive(canActivate);
            }
        }
    }
}
