using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class SetCrosshairColorWhileHitDamageable : MonoBehaviour
    {
        public Color notHitColor = Color.green;
        public Color hitColor = Color.red;
        public Image[] crosshairImages;

        private void LateUpdate()
        {
            bool hit = BasePlayerCharacterController.Singleton.SelectedGameEntity && BasePlayerCharacterController.Singleton.SelectedGameEntity is IDamageableEntity;
            foreach (Image crosshairImage in crosshairImages)
            {
                crosshairImage.color = hit ? hitColor : notHitColor;
            }
        }
    }
}
