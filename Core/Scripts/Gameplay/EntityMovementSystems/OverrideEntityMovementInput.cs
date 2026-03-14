using Cysharp.Text;
using LiteNetLib.Utils;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public struct OverrideEntityMovementInput : INetSerializable
    {
        public OverrideEntityMovementInputState State;
        public bool IsEnabled
        {
            get => State.Has(OverrideEntityMovementInputState.IsEnabled);
            set
            {
                if (value)
                    State = OverrideEntityMovementInputState.IsEnabled;
                else
                    State = OverrideEntityMovementInputState.None;
            }
        }

        public bool IsStopped
        {
            get => State.Has(OverrideEntityMovementInputState.IsStopped);
            set
            {
                if (value)
                    State |= OverrideEntityMovementInputState.IsStopped;
                else
                    State &= ~OverrideEntityMovementInputState.IsStopped;
            }
        }

        public bool IsPointClick
        {
            get => State.Has(OverrideEntityMovementInputState.IsPointClick);
            set
            {
                if (value)
                    State |= OverrideEntityMovementInputState.IsPointClick;
                else
                    State &= ~OverrideEntityMovementInputState.IsPointClick;
            }
        }
        public Vector3 Position;

        public bool IsKeyMovement
        {
            get => State.Has(OverrideEntityMovementInputState.IsKeyMovement);
            set
            {
                if (value)
                    State |= OverrideEntityMovementInputState.IsKeyMovement;
                else
                    State &= ~OverrideEntityMovementInputState.IsKeyMovement;
            }
        }
        public MovementState MovementState;
        public Vector3 MoveDirection;

        public bool IsSetExtraMovementState
        {
            get => State.Has(OverrideEntityMovementInputState.IsSetExtraMovementState);
            set
            {
                if (value)
                    State |= OverrideEntityMovementInputState.IsSetExtraMovementState;
                else
                    State &= ~OverrideEntityMovementInputState.IsSetExtraMovementState;
            }
        }
        public ExtraMovementState ExtraMovementState;

        public bool IsSetLookRotation
        {
            get => State.Has(OverrideEntityMovementInputState.IsSetLookRotation);
            set
            {
                if (value)
                    State |= OverrideEntityMovementInputState.IsSetLookRotation;
                else
                    State &= ~OverrideEntityMovementInputState.IsSetLookRotation;
            }
        }
        public Quaternion LookRotation;
        public bool TurnImmediately;

        public bool IsSetSmoothTurnSpeed
        {
            get => State.Has(OverrideEntityMovementInputState.IsSetSmoothTurnSpeed);
            set
            {
                if (value)
                    State |= OverrideEntityMovementInputState.IsSetSmoothTurnSpeed;
                else
                    State &= ~OverrideEntityMovementInputState.IsSetSmoothTurnSpeed;
            }
        }
        public float SmoothTurnSpeed;

        public override string ToString()
        {
            return ZString.Format("IsEnabled {0}, IsStopped {1}, IsPointClick {2}, Position {3}, IsKeyMovement {4}, MovementState {5}, MoveDirection {6}, IsSetExtraMovementState {7}, ExtraMovementState {8}, IsSetLookRotation {9}, LookRotation {10}, TurnImmediately {11}, IsSetSmoothTurnSpeed {12}, SmoothTurnSpeed {13}",
                IsEnabled,
                IsStopped,
                IsPointClick,
                Position,
                IsKeyMovement,
                MovementState,
                MoveDirection,
                IsSetExtraMovementState,
                ExtraMovementState,
                IsSetLookRotation,
                LookRotation,
                TurnImmediately,
                IsSetSmoothTurnSpeed,
                SmoothTurnSpeed);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)State);
            if (!IsEnabled)
                return;
            if (IsPointClick)
                writer.PutVector3(Position);
            if (IsKeyMovement)
            {
                writer.PutPackedUInt((uint)MovementState);
                writer.PutVector3(MoveDirection);
            }
            if (IsSetExtraMovementState)
                writer.PutPackedUInt((uint)ExtraMovementState);
            if (IsSetLookRotation)
            {
                writer.PutQuaternion(LookRotation);
                writer.Put(TurnImmediately);
            }
            if (IsSetSmoothTurnSpeed)
                writer.Put(SmoothTurnSpeed);
        }

        public void Deserialize(NetDataReader reader)
        {
            State = (OverrideEntityMovementInputState)reader.GetByte();
            if (!IsEnabled)
                return;
            if (IsPointClick)
                Position = reader.GetVector3();
            if (IsKeyMovement)
            {
                MovementState = (MovementState)reader.GetPackedUInt();
                MoveDirection = reader.GetVector3();
            }
            if (IsSetExtraMovementState)
                ExtraMovementState = (ExtraMovementState)reader.GetPackedUInt();
            if (IsSetLookRotation)
            {
                LookRotation = reader.GetQuaternion();
                TurnImmediately = reader.GetBool();
            }
            if (IsSetSmoothTurnSpeed)
                SmoothTurnSpeed = reader.GetFloat();
        }
    }

    [System.Serializable]
    public class SyncFieldOverrideEntityMovementInput : LiteNetLibSyncField<OverrideEntityMovementInput>
    {
        protected override bool IsValueChanged(OverrideEntityMovementInput oldValue, OverrideEntityMovementInput newValue)
        {
            return true;
        }
    }
}