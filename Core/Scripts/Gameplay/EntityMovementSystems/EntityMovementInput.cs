using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public class EntityMovementInput
    {
        public bool IsKeyMovement { get; set; }
        public bool IsStopped { get; set; }
        public MovementState MovementState { get; set; }
        public ExtraMovementState ExtraMovementState { get; set; }
        public Vector3 Position { get; set; }
        // For 3D games
        public float YAngle { get; set; }
        // For 2D games
        public DirectionVector2 Direction2D { get; set; }
    }

    public static class EntityMovementInputExtension
    {
        public static EntityMovementInput InitInput(this BaseGameEntity entity)
        {
            return new EntityMovementInput()
            {
                Position = entity.EntityTransform.position,
                YAngle = entity.EntityTransform.eulerAngles.y,
            };
        }

        public static EntityMovementInput SetInputIsKeyMovement(this BaseGameEntity entity, EntityMovementInput input, bool isKeyMovement)
        {
            if (input == null)
                input = entity.InitInput();
            input.IsKeyMovement = isKeyMovement;
            return input;
        }

        public static EntityMovementInput SetInputMovementState(this BaseGameEntity entity, EntityMovementInput input, MovementState movementState)
        {
            if (input == null)
                input = entity.InitInput();
            input.IsStopped = false;
            bool isJump = input.MovementState.Has(MovementState.IsJump);
            input.MovementState = movementState;
            if (isJump)
                input = entity.SetInputJump(input);
            // Update extra movement state because some movement state can affect extra movement state
            input = SetInputExtraMovementState(entity, input, input.ExtraMovementState);
            return input;
        }

        public static EntityMovementInput SetInputMovementState2D(this BaseGameEntity entity, EntityMovementInput input, MovementState movementState)
        {
            if (input == null)
                input = entity.InitInput();
            input.IsStopped = false;
            input.MovementState = movementState;
            // Update extra movement state because some movement state can affect extra movement state
            input = SetInputExtraMovementState(entity, input, input.ExtraMovementState);
            return input;
        }

        public static EntityMovementInput SetInputExtraMovementState(this BaseGameEntity entity, EntityMovementInput input, ExtraMovementState extraMovementState)
        {
            if (input == null)
                input = entity.InitInput();
            input.ExtraMovementState = entity.ValidateExtraMovementState(input.MovementState, extraMovementState);
            return input;
        }

        public static EntityMovementInput SetInputPosition(this BaseGameEntity entity, EntityMovementInput input, Vector3 position)
        {
            if (input == null)
                input = entity.InitInput();
            input.Position = position;
            return input;
        }

        public static EntityMovementInput SetInputYPosition(this BaseGameEntity entity, EntityMovementInput input, float yPosition)
        {
            if (input == null)
                input = entity.InitInput();
            Vector3 position = input.Position;
            position.y = yPosition;
            input.Position = position;
            return input;
        }

        public static EntityMovementInput SetInputYAngle(this BaseGameEntity entity, EntityMovementInput input, float yAngle)
        {
            if (input == null)
                input = entity.InitInput();
            input.YAngle = yAngle;
            return input;
        }

        public static EntityMovementInput SetInputDirection2D(this BaseGameEntity entity, EntityMovementInput input, DirectionVector2 direction)
        {
            if (input == null)
                input = entity.InitInput();
            input.Direction2D = direction;
            return input;
        }

        public static EntityMovementInput SetInputJump(this BaseGameEntity entity, EntityMovementInput input)
        {
            if (input == null)
                input = entity.InitInput();
            input.IsStopped = false;
            input.MovementState |= MovementState.IsJump;
            return input;
        }

        public static EntityMovementInput ClearInputJump(this BaseGameEntity entity, EntityMovementInput input)
        {
            if (input == null)
                input = entity.InitInput();
            input.MovementState &= ~MovementState.IsJump;
            return input;
        }

        public static EntityMovementInput SetInputDash(this BaseGameEntity entity, EntityMovementInput input)
        {
            if (input == null)
                input = entity.InitInput();
            input.IsStopped = false;
            input.MovementState |= MovementState.IsDash;
            return input;
        }

        public static EntityMovementInput ClearInputDash(this BaseGameEntity entity, EntityMovementInput input)
        {
            if (input == null)
                input = entity.InitInput();
            input.MovementState &= ~MovementState.IsDash;
            return input;
        }

        public static EntityMovementInput SetInputStop(this BaseGameEntity entity, EntityMovementInput input)
        {
            if (input == null)
                input = entity.InitInput();
            input.IsStopped = true;
            return input;
        }

        public static bool DifferInputEnoughToSend(this BaseGameEntity entity, EntityMovementInput oldInput, EntityMovementInput newInput, out EntityMovementInputState state)
        {
            state = EntityMovementInputState.None;
            if (newInput == null)
                return false;
            if (oldInput == null)
            {
                state = EntityMovementInputState.PositionChanged | EntityMovementInputState.RotationChanged;
                if (newInput.IsStopped)
                    state |= EntityMovementInputState.IsStopped;
                if (newInput.IsKeyMovement)
                    state |= EntityMovementInputState.IsKeyMovement;
                return true;
            }
            if (newInput.IsStopped && !oldInput.IsStopped)
                state |= EntityMovementInputState.IsStopped;
            if (newInput.IsKeyMovement)
                state |= EntityMovementInputState.IsKeyMovement;
            if (Vector3.Distance(newInput.Position, oldInput.Position) > 0.01f)
                state |= EntityMovementInputState.PositionChanged;
            if (Mathf.Abs(newInput.YAngle - oldInput.YAngle) > 0.01f)
                state |= EntityMovementInputState.RotationChanged;
            if (newInput.MovementState.Has(MovementState.IsJump) || newInput.MovementState.Has(MovementState.IsTeleport))
                state |= EntityMovementInputState.Other;
            if (newInput.ExtraMovementState != oldInput.ExtraMovementState)
                state |= EntityMovementInputState.Other;
            return state != EntityMovementInputState.None;
        }
    }
}
