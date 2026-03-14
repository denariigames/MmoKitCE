using UnityEngine;

namespace MultiplayerARPG
{
    public class ZoomButtonActivator : MonoBehaviour
    {
        public GameObject[] activateObjects = new GameObject[0];

        private void LateUpdate()
        {
            bool canZoom = BasePlayerCharacterController.Singleton is IWeaponAbilityController castedController && castedController.WeaponAbility is ZoomWeaponAbility;
            foreach (GameObject obj in activateObjects)
            {
                obj.SetActive(canZoom);
            }
        }
    }
}
