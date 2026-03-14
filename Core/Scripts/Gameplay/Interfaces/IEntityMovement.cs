using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public interface IEntityMovement
    {
        BaseGameEntity Entity { get; }
        float StoppingDistance { get; }
        MovementState MovementState { get; }
        ExtraMovementState ExtraMovementState { get; }
        DirectionVector2 Direction2D { get; set; }
        float CurrentMoveSpeed { get; }
        void StopMove();
        void KeyMovement(Vector3 moveDirection, MovementState movementState);
        void PointClickMovement(Vector3 position);
        void SetExtraMovementState(ExtraMovementState extraMovementState);
        void SetLookRotation(Quaternion rotation, bool immediately);
        Quaternion GetLookRotation();
        void SetSmoothTurnSpeed(float speed);
        float GetSmoothTurnSpeed();
        void Teleport(Vector3 position, Quaternion rotation, bool stillMoveAfterTeleport);
        bool FindGroundedPosition(Vector3 fromPosition, float findDistance, out Vector3 result);
        void ApplyForce(ApplyMovementForceMode mode, Vector3 direction, ApplyMovementForceSourceType sourceType, int sourceDataId, int sourceLevel, float force, float deceleration, float duration);
        EntityMovementForceApplier FindForceByActionKey(ApplyMovementForceSourceType sourceType, int sourceDataId);
        void ClearAllForces();
        bool AllowToJump();
        bool AllowToDash();
        bool AllowToCrouch();
        bool AllowToCrawl();
        bool AllowToStand();
        UniTask WaitClientTeleportConfirm();
        bool IsWaitingClientTeleportConfirm();
    }
}
