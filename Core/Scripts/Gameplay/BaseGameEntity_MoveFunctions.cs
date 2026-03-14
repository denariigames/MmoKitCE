using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BaseGameEntity
    {
        public const float GROUND_DETECTION_DISTANCE = 100f;

        [Category(3, "Entity Movement")]
        [SerializeField]
        protected bool canSideSprint = false;
        public bool CanSideSprint { get { return canSideSprint; } }

        [SerializeField]
        protected bool canBackwardSprint = false;
        public bool CanBackwardSprint { get { return canBackwardSprint; } }

        public IEntityMovementComponent Movement { get; private set; }

        public Transform MovementTransform
        {
            get
            {
                if (!PassengingVehicleEntity.IsNull())
                {
                    // Track movement position by vehicle entity
                    return PassengingVehicleEntity.Entity.EntityTransform;
                }
                return EntityTransform;
            }
        }

        public IEntityMovementComponent ActiveMovement
        {
            get
            {
                if (!PassengingVehicleEntity.IsNull())
                    return PassengingVehicleEntity.Entity.Movement;
                return Movement;
            }
        }

        public float StoppingDistance
        {
            get
            {
                return ActiveMovement.IsNull() ? 0.1f : ActiveMovement.StoppingDistance;
            }
        }

        public MovementState MovementState
        {
            get
            {
                return ActiveMovement.IsNull() ? MovementState.IsGrounded : ActiveMovement.MovementState;
            }
        }

        public ExtraMovementState ExtraMovementState
        {
            get
            {
                return ActiveMovement.IsNull() ? ExtraMovementState.None : ActiveMovement.ExtraMovementState;
            }
        }

        public DirectionVector2 Direction2D
        {
            get
            {
                return ActiveMovement.IsNull() ? (DirectionVector2)Vector2.down : ActiveMovement.Direction2D;
            }
            set
            {
                if (!ActiveMovement.IsNull())
                    ActiveMovement.Direction2D = value;
            }
        }

        public float CurrentMoveSpeed
        {
            get
            {
                return ActiveMovement.IsNull() ? 0f : ActiveMovement.CurrentMoveSpeed;
            }
        }

        public virtual bool ShouldUseRootMotion
        {
            get { return false; }
        }

        public virtual bool SkipMovementValidation
        {
            get { return false; }
        }

        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldOverrideEntityMovementInput syncOverrideInput = new SyncFieldOverrideEntityMovementInput();
        public OverrideEntityMovementInput OverrideInput
        {
            get { return syncOverrideInput.Value; }
            set { syncOverrideInput.Value = value; }
        }
        [SerializeField]
        protected SyncFieldFloat syncOverrideMoveSpeed = new SyncFieldFloat();
        public float OverrideMoveSpeed
        {
            get { return syncOverrideMoveSpeed.Value; }
            set { syncOverrideMoveSpeed.Value = value; }
        }
        [SerializeField]
        protected SyncFieldFloat syncOverrideJumpHeight = new SyncFieldFloat();
        public float OverrideJumpHeight
        {
            get { return syncOverrideJumpHeight.Value; }
            set { syncOverrideJumpHeight.Value = value; }
        }
        [SerializeField]
        protected SyncFieldFloat syncOverrideGravityRate = new SyncFieldFloat();
        public float OverrideGravityRate
        {
            get { return syncOverrideGravityRate.Value; }
            set { syncOverrideGravityRate.Value = value; }
        }

        public float GetMoveSpeed()
        {
            return GetMoveSpeed(MovementState, ExtraMovementState);
        }

        public float GetMoveSpeed(MovementState movementState, ExtraMovementState extraMovementState)
        {
            if (OverrideMoveSpeed >= 0f)
                return OverrideMoveSpeed;
            return GetMoveSpeed_Implementation(movementState, extraMovementState);
        }

        public virtual float GetMoveSpeed_Implementation(MovementState movementState, ExtraMovementState extraMovementState)
        {
            return 0f;
        }

        public float GetJumpHeight()
        {
            return GetJumpHeight(MovementState, ExtraMovementState);
        }

        public float GetJumpHeight(MovementState movementState, ExtraMovementState extraMovementState)
        {
            if (OverrideJumpHeight >= 0f)
                return OverrideJumpHeight;
            return GetJumpHeight_Implementation(movementState, extraMovementState);
        }

        public virtual float GetJumpHeight_Implementation(MovementState movementState, ExtraMovementState extraMovementState)
        {
            return 0f;
        }

        public float GetGravityRate()
        {
            return GetGravityRate(MovementState, ExtraMovementState);
        }

        public float GetGravityRate(MovementState movementState, ExtraMovementState extraMovementState)
        {
            if (OverrideGravityRate >= 0f)
                return OverrideGravityRate;
            return GetGravityRate_Implementation(movementState, extraMovementState);
        }

        public virtual float GetGravityRate_Implementation(MovementState movementState, ExtraMovementState extraMovementState)
        {
            return 1f;
        }

        public bool CanMove()
        {
            bool canMove = CanMove_Implementation();
            if (onCanMoveValidated != null)
                onCanMoveValidated(ref canMove);
            return canMove;
        }

        protected virtual bool CanMove_Implementation()
        {
            return false;
        }

        public bool CanSprint()
        {
            bool canSprint = CanSprint_Implementation();
            if (onCanSprintValidated != null)
                onCanSprintValidated(ref canSprint);
            return canSprint;
        }

        protected virtual bool CanSprint_Implementation()
        {
            return false;
        }

        public bool CanWalk()
        {
            bool canWalk = CanWalk_Implementation();
            if (onCanWalkValidated != null)
                onCanWalkValidated(ref canWalk);
            return canWalk;
        }

        protected virtual bool CanWalk_Implementation()
        {
            return false;
        }

        public bool CanCrouch()
        {
            bool canCrouch = CanCrouch_Implementation();
            if (onCanCrouchValidated != null)
                onCanCrouchValidated(ref canCrouch);
            return canCrouch;
        }

        protected virtual bool CanCrouch_Implementation()
        {
            return false;
        }

        public bool CanCrawl()
        {
            bool canCrawl = CanCrawl_Implementation();
            if (onCanCrawlValidated != null)
                onCanCrawlValidated(ref canCrawl);
            return canCrawl;
        }

        protected virtual bool CanCrawl_Implementation()
        {
            return false;
        }

        public bool CanJump()
        {
            bool canJump = CanJump_Implementation();
            if (onCanJumpValidated != null)
                onCanJumpValidated(ref canJump);
            return canJump;
        }

        protected virtual bool CanJump_Implementation()
        {
            return false;
        }

        public bool CanDash()
        {
            bool canDash = CanDash_Implementation();
            if (onCanDashValidated != null)
                onCanDashValidated(ref canDash);
            return canDash;
        }

        protected virtual bool CanDash_Implementation()
        {
            return false;
        }

        public bool CanTurn()
        {
            bool canTurn = CanTurn_Implementation();
            if (onCanTurnValidated != null)
                onCanTurnValidated(ref canTurn);
            return canTurn;
        }

        protected virtual bool CanTurn_Implementation()
        {
            return false;
        }

        public void StopMove()
        {
            if (OverrideInput.IsEnabled && OverrideInput.IsStopped)
                return;
            if (!ActiveMovement.IsNull())
                ActiveMovement.StopMove();
        }

        public void KeyMovement(Vector3 moveDirection, MovementState moveState)
        {
            if (OverrideInput.IsEnabled && OverrideInput.IsKeyMovement)
                return;
            if (!ActiveMovement.IsNull())
                ActiveMovement.KeyMovement(moveDirection, moveState);
        }

        public void PointClickMovement(Vector3 position)
        {
            if (OverrideInput.IsEnabled && OverrideInput.IsPointClick)
                return;
            if (!ActiveMovement.IsNull())
                ActiveMovement.PointClickMovement(position);
        }

        public void SetExtraMovementState(ExtraMovementState extraMovementState)
        {
            if (OverrideInput.IsEnabled && OverrideInput.IsSetExtraMovementState)
                return;
            if (!ActiveMovement.IsNull())
                ActiveMovement.SetExtraMovementState(extraMovementState);
        }

        public void SetLookRotation(Quaternion rotation, bool immediately)
        {
            if (OverrideInput.IsEnabled && OverrideInput.IsSetLookRotation)
                return;
            if (!ActiveMovement.IsNull())
                ActiveMovement.SetLookRotation(rotation, immediately);
        }

        public Quaternion GetLookRotation()
        {
            if (!ActiveMovement.IsNull())
                return ActiveMovement.GetLookRotation();
            return Quaternion.identity;
        }

        public void SetSmoothTurnSpeed(float speed)
        {
            if (OverrideInput.IsEnabled && OverrideInput.IsSetSmoothTurnSpeed)
                return;
            if (!ActiveMovement.IsNull())
                ActiveMovement.SetSmoothTurnSpeed(speed);
        }

        public float GetSmoothTurnSpeed()
        {
            if (!ActiveMovement.IsNull())
                return ActiveMovement.GetSmoothTurnSpeed();
            return 0f;
        }

        public void Teleport(Vector3 position, Quaternion rotation, bool stillMoveAfterTeleport)
        {
            if (ActiveMovement.IsNull())
            {
                // Can't teleport properly yet, try to teleport later
                _teleportingPosition = position;
                _teleportingRotation = rotation;
                _isTeleporting = true;
                _stillMoveAfterTeleport = stillMoveAfterTeleport;
                return;
            }
            if (FindGroundedPosition(position, GROUND_DETECTION_DISTANCE, out Vector3 groundedPosition))
            {
                // Set position to grounded position, to make it not float and fall
                position = groundedPosition;
            }
            if (IsServer)
            {
                // Teleport to the `position`, `rotation`
                ActiveMovement.Teleport(position, rotation, stillMoveAfterTeleport);
            }
            OnTeleport(position, rotation);
        }

        protected virtual void OnTeleport(Vector3 position, Quaternion rotation)
        {

        }

        public bool FindGroundedPosition(Vector3 fromPosition, float findDistance, out Vector3 result)
        {
            result = EntityTransform.position;
            if (!ActiveMovement.IsNull())
                return ActiveMovement.FindGroundedPosition(fromPosition, findDistance, out result);
            return true;
        }

        public void ApplyForce(ApplyMovementForceMode mode, Vector3 direction, ApplyMovementForceSourceType sourceType, int sourceDataId, int sourceLevel, float force, float deceleration, float duration)
        {
            if (!IsServer)
                return;
            if (!ActiveMovement.IsNull())
                ActiveMovement.ApplyForce(mode, direction, sourceType, sourceDataId, sourceLevel, force, deceleration, duration);
        }

        public EntityMovementForceApplier FindForceByActionKey(ApplyMovementForceSourceType sourceType, int sourceDataId)
        {
            return ActiveMovement.FindForceByActionKey(sourceType, sourceDataId);
        }

        public void ClearAllForces()
        {
            if (!IsServer)
                return;
            if (!ActiveMovement.IsNull())
                ActiveMovement.ClearAllForces();
        }

        public void OnJumpForceApplied(float verticalVelocity)
        {
            if (onJumpForceApplied != null)
                onJumpForceApplied.Invoke(verticalVelocity);
        }

        public bool AllowToJump()
        {
            if (!ActiveMovement.IsNull())
                return ActiveMovement.AllowToJump();
            return false;
        }

        public bool AllowToDash()
        {
            if (!ActiveMovement.IsNull())
                return ActiveMovement.AllowToDash();
            return false;
        }

        public bool AllowToCrouch()
        {
            if (!ActiveMovement.IsNull())
                return ActiveMovement.AllowToCrouch();
            return false;
        }

        public bool AllowToCrawl()
        {
            if (!ActiveMovement.IsNull())
                return ActiveMovement.AllowToCrawl();
            return false;
        }

        public bool AllowToStand()
        {
            if (!ActiveMovement.IsNull())
                return ActiveMovement.AllowToStand();
            return false;
        }

        public async UniTask WaitClientTeleportConfirm()
        {
            if (!ActiveMovement.IsNull())
                await ActiveMovement.WaitClientTeleportConfirm();
        }

        public bool IsWaitingClientTeleportConfirm()
        {
            if (!ActiveMovement.IsNull())
                return ActiveMovement.IsWaitingClientTeleportConfirm();
            return false;
        }
    }
}
