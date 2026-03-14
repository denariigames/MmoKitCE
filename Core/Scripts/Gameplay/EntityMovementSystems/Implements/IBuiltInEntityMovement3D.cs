using UnityEngine;

namespace MultiplayerARPG
{
    public partial interface IBuiltInEntityMovement3D
    {
        bool GroundCheck();
        bool AirborneCheck();
        void SetPosition(Vector3 position);
        void Move(MovementState movementState, ExtraMovementState extraMovementState, Vector3 motion, float deltaTime);
        void RotateY(float yAngle);
        void OnJumpForceApplied(float verticalVelocity);
        Bounds GetMovementBounds();
        Vector3 GetSnapToGroundMotion(Vector3 motion, Vector3 platformMotion, Vector3 forceMotion);
    }
}
