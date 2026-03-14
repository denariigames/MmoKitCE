using UnityEngine;

namespace MultiplayerARPG
{
    public interface IMoveableModel
    {
        void SetMoveAnimationSpeedMultiplier(float moveAnimationSpeedMultiplier);
        void SetMovementState(MovementState movementState, ExtraMovementState extraMovementState, Vector2 direction2D, bool isFreezeAnimation);
    }
}
