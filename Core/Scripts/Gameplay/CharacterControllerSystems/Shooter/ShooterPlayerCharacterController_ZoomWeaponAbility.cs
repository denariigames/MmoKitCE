using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class ShooterPlayerCharacterController : IZoomWeaponAbilityController
    {
        [Header("Zoom Weapon Ability Settings")]
        [SerializeField]
        private Image zoomCrosshairImage = null;

        public bool ShowZoomCrosshair
        {
            get
            {
                return zoomCrosshairImage != null && zoomCrosshairImage.gameObject.activeSelf;
            }
            set
            {
                if (zoomCrosshairImage != null &&
                    zoomCrosshairImage.gameObject.activeSelf != value)
                {
                    // Hide crosshair when not active
                    zoomCrosshairImage.gameObject.SetActive(value);
                }
            }
        }

        public void InitialZoomCrosshair()
        {
            if (zoomCrosshairImage != null)
            {
                zoomCrosshairImage.preserveAspect = true;
                zoomCrosshairImage.raycastTarget = false;
            }
        }

        public void SetZoomCrosshairSprite(Sprite sprite)
        {
            if (zoomCrosshairImage != null)
                zoomCrosshairImage.sprite = sprite;
        }
    }
}
