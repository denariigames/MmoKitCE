using UnityEngine;

namespace MultiplayerARPG
{
    public class ActivateButtonActivator : MonoBehaviour
    {
        public GameObject[] activateObjects = new GameObject[0];

        private void LateUpdate()
        {
            bool canActivate = BasePlayerCharacterController.Singleton.ShouldShowActivateButtons();
            foreach (GameObject obj in activateObjects)
            {
                obj.SetActive(canActivate);
            }
        }
    }
}
