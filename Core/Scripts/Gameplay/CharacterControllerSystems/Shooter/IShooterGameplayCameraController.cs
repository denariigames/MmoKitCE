using UnityEngine;

namespace MultiplayerARPG
{
    public interface IShooterGameplayCameraController : IGameplayCameraController
    {
        ShooterControllerViewMode ActiveViewMode { get; set; }
        bool EnableAimAssist { get; set; }
        bool EnableAimAssistX { get; set; }
        bool EnableAimAssistY { get; set; }
        bool AimAssistPlayer { get; set; }
        bool AimAssistMonster { get; set; }
        bool AimAssistBuilding { get; set; }
        bool AimAssistHarvestable { get; set; }
        float AimAssistRadius { get; set; }
        float AimAssistXSpeed { get; set; }
        float AimAssistYSpeed { get; set; }
        float AimAssistMaxAngleFromFollowingTarget { get; set; }
        float CameraRotationSpeedScale { get; set; }
        bool IsLeftViewSide { get; set; }
        bool IsZoomAimming { get; set; }
        Transform LookForwardTransform { get; }
        void Recoil(float pitch, float yaw, float roll);
    }
}
