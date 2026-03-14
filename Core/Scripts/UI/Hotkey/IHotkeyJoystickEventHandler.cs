using UnityEngine;

namespace MultiplayerARPG
{
    public interface IHotkeyJoystickEventHandler
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        UICharacterHotkey UICharacterHotkey { get; }
        bool Interactable { get; }
        bool IsDragging { get; }
        AimPosition AimPosition { get; }
        void UpdateEvent();
    }
}
