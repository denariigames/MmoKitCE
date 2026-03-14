using UnityEngine;

namespace MultiplayerARPG
{
    public class ViewModeButtonActivator : MonoBehaviour
    {
        public GameObject[] tpsActivateObjects = new GameObject[0];
        public GameObject[] fpsActivateObjects = new GameObject[0];

        private void LateUpdate()
        {
            bool isTps = false;
            bool isFps = false;
            if (BasePlayerCharacterController.Singleton is IWeaponAbilityController castedController)
            {
                isTps = castedController.ViewMode == ShooterControllerViewMode.Tps;
                isFps = castedController.ViewMode == ShooterControllerViewMode.Fps;
            }
            foreach (GameObject obj in tpsActivateObjects)
            {
                obj.SetActive(isTps);
            }
            foreach (GameObject obj in fpsActivateObjects)
            {
                obj.SetActive(isFps);
            }
        }
    }
}
