using UnityEngine;

namespace MultiplayerARPG
{
    public interface IShooterWeaponController
    {
        ShooterControllerViewMode ViewMode { get; set; }
        ShooterControllerViewMode ActiveViewMode { get; }
        float CameraZoomDistance { get; }
        Vector3 CameraTargetOffset { get; }
        float CameraFov { get; }
        float CameraNearClipPlane { get; }
        float CameraFarClipPlane { get; }
        float CurrentCameraFov { get; set; }
        float ThirdPersonCameraRotationSpeedScale { get; }
        float FirstPersonCameraRotationSpeedScale { get; }
        float CameraRotationSpeedScale { get; set; }
        bool HideCrosshair { get; set; }
        bool IsLeftViewSide { get; set; }
        bool IsZoomAimming { get; set; }
        void UpdateCameraSettings();
    }
}
