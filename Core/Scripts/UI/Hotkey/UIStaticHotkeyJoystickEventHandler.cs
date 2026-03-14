using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    [DefaultExecutionOrder(DefaultExecutionOrders.UI_CHARACTER_HOTKEY_JOYSTICK)]
    [RequireComponent(typeof(UICharacterHotkey))]
    public class UIStaticHotkeyJoystickEventHandler : MonoBehaviour, IHotkeyJoystickEventHandler
    {
        public UICharacterHotkey UICharacterHotkey { get; private set; }
        public bool Interactable { get { return UICharacterHotkey.IsAssigned(); } }
        public bool IsDragging { get { return false; } }
        public AimPosition AimPosition { get; private set; }

        private void Awake()
        {
            UICharacterHotkey = GetComponent<UICharacterHotkey>();
            if (TryGetComponent(out UICharacterHotkeys hotkeys))
                Debug.LogWarning("[UIStaticHotkeyJoystickEventHandler] this should not be in the same game object with `UICharacterHotkeys` component");
            UICharacterHotkeys.RegisterHotkeyJoystick(this);
        }

        public void UpdateEvent()
        {
            if (UICharacterHotkeys.UsingHotkey == null || UICharacterHotkeys.UsingHotkey != UICharacterHotkey)
                return;
            AimPosition = UICharacterHotkey.UpdateAimControls(Vector2.zero);
            if (InputManager.GetButtonUp("Fire1"))
                UICharacterHotkeys.FinishHotkeyAimControls(false);
        }

        public void OnClickUse()
        {
            if (UICharacterHotkey.HasCustomAimControls())
            {
                // Set hotkey to aim
                UICharacterHotkeys.SetUsingHotkey(UICharacterHotkey);
            }
            else
            {
                // Use it immediately
                UICharacterHotkeys.SetUsingHotkey(null);
                UICharacterHotkey.OnClickUse();
            }
        }
    }
}
