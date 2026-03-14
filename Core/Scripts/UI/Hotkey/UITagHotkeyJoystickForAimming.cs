using UnityEngine;

namespace MultiplayerARPG
{
    public class UITagHotkeyJoystickForAimming : MonoBehaviour
    {
        public IHotkeyJoystickEventHandler HotkeyJoystickEventHandler { get; private set; }

        private void Awake()
        {
            HotkeyJoystickEventHandler = GetComponent<IHotkeyJoystickEventHandler>();
            if (HotkeyJoystickEventHandler != null)
                UICharacterHotkeys.s_hotkeyJoystickForAimming = HotkeyJoystickEventHandler;
            gameObject.SetActive(false);
        }
    }
}