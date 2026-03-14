using UnityEngine;

namespace MultiplayerARPG
{
    public class PickUpButtonActivator : MonoBehaviour
    {
        public GameObject[] activateObjects = new GameObject[0];

        private void LateUpdate()
        {
            bool canActivate = BasePlayerCharacterController.Singleton.ShouldShowPickUpButtons();
            foreach (GameObject obj in activateObjects)
            {
                obj.SetActive(canActivate);
            }
        }
    }
}
