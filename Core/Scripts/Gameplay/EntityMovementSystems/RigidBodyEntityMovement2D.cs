using Cysharp.Threading.Tasks;
using Insthync.ManagedUpdating;
using LiteNetLib.Utils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidBodyEntityMovement2D : BaseNetworkedGameEntityComponent<BaseGameEntity>, IEntityMovementComponent, IManagedUpdate
    {
        protected const float MIN_DISTANCE_TO_SIMULATE_MOVEMENT = 0.01f;
        protected const float TIMESTAMP_TO_UNITY_TIME_MULTIPLIER = 0.001f;

        [Header("Movement Settings")]
        [Range(0.01f, 1f)]
        public float stoppingDistance = 0.1f;
        public MovementSecure movementSecure = MovementSecure.NotSecure;

        [Header("Dashing")]
        public EntityMovementForceApplierData dashingForceApplier = EntityMovementForceApplierData.CreateDefault();

        [Header("Networking Settings")]
        public float snapThreshold = 5.0f;

        public Rigidbody2D CacheRigidbody2D { get; private set; }
        public IEntityTeleportPreparer TeleportPreparer { get; protected set; }
        public bool IsPreparingToTeleport { get { return TeleportPreparer != null && TeleportPreparer.IsPreparingToTeleport; } }
        public float StoppingDistance
        {
            get { return stoppingDistance; }
        }
        public MovementState MovementState { get; protected set; }
        public ExtraMovementState ExtraMovementState { get; protected set; }
        public DirectionVector2 Direction2D { get; set; }
        public float CurrentMoveSpeed { get; private set; }

        public Queue<Vector2> NavPaths { get; protected set; }
        public bool HasNavPaths
        {
            get { return NavPaths != null && NavPaths.Count > 0; }
        }

        // Input codes
        protected bool _isDashing;
        protected Vector2 _inputDirection;
        protected MovementState _tempMovementState;
        protected ExtraMovementState _tempExtraMovementState;

        // Move simulate codes
        protected Vector2 _moveDirection;
        protected readonly List<EntityMovementForceApplier> _movementForceAppliers = new List<EntityMovementForceApplier>();
        protected IEntityMovementForceUpdateListener[] _forceUpdateListeners;

        // Teleport codes
        protected bool _isTeleporting;
        protected bool _stillMoveAfterTeleport;

        // Client state codes
        protected EntityMovementInput _oldInput;
        protected EntityMovementInput _currentInput;
        protected bool _sendingDash;

        // State simulate codes
        protected float? _lagMoveSpeedRate;
        protected bool _simulatingKeyMovement = false;

        // Peers accept codes
        protected bool _acceptedDash;
        protected long _acceptedPositionTimestamp;

        // Server validate codes
        private Vector3? _acceptedPosition = null;
        private float _accumulateDeltaTime = 0f;
        private float _accumulateDiffMoveDist = 0f;
        protected bool _isServerWaitingTeleportConfirm;

        // Client confirm codes
        protected bool _isClientConfirmingTeleport;

        protected virtual void Awake()
        {
            // Prepare rigidbody component
            CacheRigidbody2D = gameObject.GetOrAddComponent<Rigidbody2D>();
            TeleportPreparer = gameObject.GetComponent<IEntityTeleportPreparer>();
            _forceUpdateListeners = gameObject.GetComponents<IEntityMovementForceUpdateListener>();
            // Disable unused component
            LiteNetLibTransform disablingComp = gameObject.GetComponent<LiteNetLibTransform>();
            if (disablingComp != null)
            {
                Logging.LogWarning(nameof(RigidBodyEntityMovement2D), "You can remove `LiteNetLibTransform` component from game entity, it's not being used anymore [" + name + "]");
                disablingComp.enabled = false;
            }
            // Setup
            CacheRigidbody2D.gravityScale = 0;
            StopMoveFunction();
            _isClientConfirmingTeleport = true;
        }

        protected virtual void OnEnable()
        {
            CacheRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            UpdateManager.Register(this);
        }

        protected virtual void OnDisable()
        {
            CacheRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            UpdateManager.Unregister(this);
        }

        public override void OnSetOwnerClient(bool isOwnerClient)
        {
            NavPaths = null;
        }

        public virtual void StopMove()
        {
            if (movementSecure == MovementSecure.ServerAuthoritative)
            {
                // Send movement input to server, then server will apply movement and sync transform to clients
                _currentInput = Entity.SetInputStop(_currentInput);
            }
            StopMoveFunction();
        }

        private void StopMoveFunction()
        {
            NavPaths = null;
            _lagMoveSpeedRate = null;
        }

        public virtual void KeyMovement(Vector3 moveDirection, MovementState movementState)
        {
            if (!Entity.CanMove())
                return;
            if (CanPredictMovement())
            {
                // Always apply movement to owner client (it's client prediction for server auth movement)
                _inputDirection = moveDirection;
                _tempMovementState = movementState;
                if (_inputDirection.sqrMagnitude > 0)
                    NavPaths = null;
                if (!_isDashing)
                    _isDashing = _tempMovementState.Has(MovementState.IsDash);
            }
        }

        public virtual void PointClickMovement(Vector3 position)
        {
            if (!Entity.CanMove())
                return;
            if (CanPredictMovement())
            {
                // Always apply movement to owner client (it's client prediction for server auth movement)
                SetMovePaths(position, true);
            }
        }

        public void SetExtraMovementState(ExtraMovementState extraMovementState)
        {
            if (!Entity.CanMove())
                return;
            if (CanPredictMovement())
            {
                // Always apply movement to owner client (it's client prediction for server auth movement)
                _tempExtraMovementState = extraMovementState;
            }
        }

        public virtual void SetLookRotation(Quaternion rotation, bool immediately)
        {
            if (!Entity.CanMove() || !Entity.CanTurn())
                return;
            if (CanPredictMovement())
            {
                // Always apply movement to owner client (it's client prediction for server auth movement)
                Direction2D = (Vector2)(rotation * Vector3.forward);
            }
        }

        public Quaternion GetLookRotation()
        {
            return Quaternion.LookRotation(Direction2D);
        }

        public void SetSmoothTurnSpeed(float speed)
        {
            // 2D, do nothing
        }

        public float GetSmoothTurnSpeed()
        {
            // 2D, do nothing
            return 0f;
        }

        public async void Teleport(Vector3 position, Quaternion rotation, bool stillMoveAfterTeleport)
        {
            if (!IsServer)
            {
                Logging.LogWarning(nameof(RigidBodyEntityMovement2D), "Teleport function shouldn't be called at client [" + name + "]");
                return;
            }
            if (IsPreparingToTeleport)
                return;
            if (IsServer && !IsOwnerClientOrOwnedByServer)
                _isServerWaitingTeleportConfirm = true;
            _acceptedPosition = position;
            _isTeleporting = true;
            _stillMoveAfterTeleport = stillMoveAfterTeleport;
            if (TeleportPreparer != null)
                await TeleportPreparer.PrepareToTeleport(position, rotation);
            OnTeleport(position, stillMoveAfterTeleport);
        }

        public bool FindGroundedPosition(Vector3 fromPosition, float findDistance, out Vector3 result)
        {
            result = fromPosition;
            return true;
        }

        public void ApplyForce(ApplyMovementForceMode mode, Vector3 direction, ApplyMovementForceSourceType sourceType, int sourceDataId, int sourceLevel, float force, float deceleration, float duration)
        {
            if (!IsServer)
                return;
            if (mode.IsReplaceMovement())
            {
                // Can have only one replace movement force applier, so remove stored ones
                _movementForceAppliers.RemoveReplaceMovementForces();
            }
            _movementForceAppliers.Add(new EntityMovementForceApplier()
                .Apply(mode, direction, sourceType, sourceDataId, sourceLevel, force, deceleration, duration));
        }

        public EntityMovementForceApplier FindForceByActionKey(ApplyMovementForceSourceType sourceType, int sourceDataId)
        {
            return _movementForceAppliers.FindBySource(sourceType, sourceDataId);
        }

        public void ClearAllForces()
        {
            if (!IsServer)
                return;
            _movementForceAppliers.Clear();
        }

        public virtual void ManagedUpdate()
        {
            UpdateMovement(Time.fixedDeltaTime);
            if (CanPredictMovement())
            {
                _tempMovementState = _moveDirection.sqrMagnitude > 0f ? _tempMovementState : MovementState.None;
                _tempMovementState |= MovementState.IsGrounded;
                // Update movement state
                MovementState = _tempMovementState;
                // Update extra movement state
                ExtraMovementState = this.ValidateExtraMovementState(MovementState, _tempExtraMovementState);
                // Set current input
                _currentInput = Entity.SetInputMovementState(_currentInput, MovementState);
                _currentInput = Entity.SetInputExtraMovementState(_currentInput, ExtraMovementState);
            }
            else
            {
                // Update movement state
                if (HasNavPaths && !MovementState.Has(MovementState.Forward))
                    MovementState |= MovementState.Forward;
            }
            _isDashing = false;
            _acceptedDash = false;
        }

        protected virtual void UpdateMovement(float deltaTime)
        {
            if (IsPreparingToTeleport)
                return;

            float tempSqrMagnitude;
            float tempPredictSqrMagnitude;
            float tempTargetDistance;
            float tempEntityMoveSpeed;
            float tempMaxMoveSpeed;
            Vector2 tempMoveVelocity;
            Vector2 tempCurrentPosition;
            Vector2 tempTargetPosition;
            Vector2 tempPredictPosition;

            tempCurrentPosition = EntityTransform.position;
            tempMoveVelocity = Vector3.zero;
            _moveDirection = Vector2.zero;
            tempTargetDistance = 0f;

            if (HasNavPaths)
            {
                // Set `tempTargetPosition` and `tempCurrentPosition`
                tempTargetPosition = NavPaths.Peek();
                _moveDirection = (tempTargetPosition - tempCurrentPosition).normalized;
                tempTargetDistance = Vector2.Distance(tempTargetPosition, tempCurrentPosition);
                float stoppingDistance = _simulatingKeyMovement ? MIN_DISTANCE_TO_SIMULATE_MOVEMENT : StoppingDistance;
                bool shouldStop = tempTargetDistance < stoppingDistance;
                if (shouldStop)
                {
                    NavPaths.Dequeue();
                    if (!HasNavPaths)
                    {
                        StopMoveFunction();
                        _moveDirection = Vector2.zero;
                    }
                    else
                    {
                        if (!_tempMovementState.Has(MovementState.Forward))
                            _tempMovementState |= MovementState.Forward;
                    }
                }
                else
                {
                    if (!_tempMovementState.Has(MovementState.Forward))
                        _tempMovementState |= MovementState.Forward;
                    // Turn character to destination
                    Direction2D = _moveDirection;
                }
            }
            else if (_inputDirection.sqrMagnitude > 0f)
            {
                _moveDirection = _inputDirection.normalized;
                tempTargetPosition = tempCurrentPosition + _moveDirection;
            }
            else
            {
                tempTargetPosition = tempCurrentPosition;
                StopMove();
            }

            if (!Entity.CanMove())
            {
                _moveDirection = Vector2.zero;
            }

            if (!Entity.CanDash())
            {
                _isDashing = false;
            }

            // Prepare movement speed
            tempEntityMoveSpeed = Entity.GetMoveSpeed();
            tempMaxMoveSpeed = tempEntityMoveSpeed;

            // Dashing
            if (_acceptedDash || _isDashing)
            {
                _sendingDash = true;
                // Can have only one replace movement force applier, so remove stored ones
                _movementForceAppliers.RemoveReplaceMovementForces();
                _movementForceAppliers.Add(new EntityMovementForceApplier().Apply(
                    ApplyMovementForceMode.Dash, Direction2D, ApplyMovementForceSourceType.None, 0, 0, dashingForceApplier));
            }

            // Apply Forces
            _forceUpdateListeners.OnPreUpdateForces(_movementForceAppliers);
            _movementForceAppliers.UpdateForces(Time.deltaTime,
                Entity.GetMoveSpeed(MovementState.Forward, ExtraMovementState.None),
                out Vector3 forceMotion, out EntityMovementForceApplier replaceMovementForceApplier);
            _forceUpdateListeners.OnPostUpdateForces(_movementForceAppliers);

            // Replace player's movement by this
            if (replaceMovementForceApplier != null)
            {
                // Still dashing to add dash to movement state
                if (replaceMovementForceApplier.Mode == ApplyMovementForceMode.Dash)
                    _tempMovementState |= MovementState.IsDash;
                // Force turn to dashed direction
                _moveDirection = replaceMovementForceApplier.Direction;
                Direction2D = _moveDirection;
                // Change move speed to dash force
                tempMaxMoveSpeed = replaceMovementForceApplier.CurrentSpeed;
            }

            // Updating horizontal movement (WASD inputs)
            if (_moveDirection.sqrMagnitude > 0f)
            {
                // If character move backward
                CurrentMoveSpeed = CalculateCurrentMoveSpeed(tempMaxMoveSpeed, deltaTime);

                // NOTE: `tempTargetPosition` and `tempCurrentPosition` were set above
                tempSqrMagnitude = (tempTargetPosition - tempCurrentPosition).sqrMagnitude;
                tempPredictPosition = tempCurrentPosition + (_moveDirection * CurrentMoveSpeed * deltaTime);
                tempPredictSqrMagnitude = (tempPredictPosition - tempCurrentPosition).sqrMagnitude;
                if (HasNavPaths)
                {
                    // Check `tempSqrMagnitude` against the `tempPredictSqrMagnitude`
                    // if `tempPredictSqrMagnitude` is greater than `tempSqrMagnitude`,
                    // rigidbody will reaching target and character is moving pass it,
                    // so adjust move speed by distance and time (with physic formula: v=s/t)
                    if (tempPredictSqrMagnitude >= tempSqrMagnitude && tempTargetDistance > 0f)
                        CurrentMoveSpeed *= tempTargetDistance / deltaTime / CurrentMoveSpeed;
                }
                tempMoveVelocity = _moveDirection * CurrentMoveSpeed;
                // Set inputs
                if (HasNavPaths)
                {
                    _currentInput = Entity.SetInputPosition(_currentInput, tempTargetPosition);
                    _currentInput = Entity.SetInputIsKeyMovement(_currentInput, false);
                }
                else
                {
                    _currentInput = Entity.SetInputPosition(_currentInput, tempPredictPosition);
                    _currentInput = Entity.SetInputIsKeyMovement(_currentInput, true);
                }
            }
            _currentInput = Entity.SetInputDirection2D(_currentInput, Direction2D);
#if UNITY_6000_0_OR_NEWER
            CacheRigidbody2D.linearVelocity = tempMoveVelocity + new Vector2(forceMotion.x, forceMotion.y);
#else
            CacheRigidbody2D.velocity = tempMoveVelocity + new Vector2(forceMotion.x, forceMotion.y);
#endif
        }

        private float CalculateCurrentMoveSpeed(float maxMoveSpeed, float deltaTime)
        {
            // Adjust speed by rtt
            if (!IsServer && IsOwnerClient && movementSecure == MovementSecure.ServerAuthoritative)
            {
                float rtt = TIMESTAMP_TO_UNITY_TIME_MULTIPLIER * Entity.Manager.Rtt;
                float acc = 1f / rtt * deltaTime * 0.5f;
                if (!_lagMoveSpeedRate.HasValue)
                    _lagMoveSpeedRate = 0f;
                if (_lagMoveSpeedRate < 1f)
                    _lagMoveSpeedRate += acc;
                if (_lagMoveSpeedRate > 1f)
                    _lagMoveSpeedRate = 1f;
                return maxMoveSpeed * _lagMoveSpeedRate.Value;
            }
            // TODO: Adjust other's client move speed by rtt
            return maxMoveSpeed;
        }

        public Bounds GetMovementBounds()
        {
            return GameplayUtils.MakeLocalBoundsByCollider(transform);
        }

        protected virtual void SetMovePaths(Vector2 position, bool useNavMesh)
        {
            // TODO: Implement nav mesh
            NavPaths = new Queue<Vector2>();
            NavPaths.Enqueue(position);
        }

        public bool WriteClientState(long writeTimestamp, NetDataWriter writer, out bool shouldSendReliably)
        {
            shouldSendReliably = false;
            if (movementSecure == MovementSecure.NotSecure && IsOwnerClient && !IsServer)
            {
                // Sync transform from owner client to server (except it's both owner client and server)
                if (_sendingDash)
                {
                    shouldSendReliably = true;
                    MovementState |= MovementState.IsDash;
                }
                else
                {
                    MovementState &= ~MovementState.IsDash;
                }
                if (_isClientConfirmingTeleport)
                {
                    shouldSendReliably = true;
                    MovementState |= MovementState.IsTeleport;
                }
                this.ClientWriteSyncTransform2D(writer);
                _sendingDash = false;
                _isClientConfirmingTeleport = false;
                return true;
            }
            if (movementSecure == MovementSecure.ServerAuthoritative && IsOwnerClient && !IsServer)
            {
                _currentInput = Entity.SetInputExtraMovementState(_currentInput, _tempExtraMovementState);
                if (_sendingDash)
                {
                    shouldSendReliably = true;
                    _currentInput = Entity.SetInputDash(_currentInput);
                }
                else
                {
                    _currentInput = Entity.ClearInputDash(_currentInput);
                }
                if (_isClientConfirmingTeleport)
                {
                    shouldSendReliably = true;
                    _currentInput.MovementState |= MovementState.IsTeleport;
                }
                if (Entity.DifferInputEnoughToSend(_oldInput, _currentInput, out EntityMovementInputState inputState))
                {
                    if (!_currentInput.IsKeyMovement)
                    {
                        // Point click should be reliably
                        shouldSendReliably = true;
                    }
                    this.ClientWriteMovementInput2D(writer, inputState, _currentInput);
                    _sendingDash = false;
                    _isClientConfirmingTeleport = false;
                    _oldInput = _currentInput;
                    _currentInput = null;
                    return true;
                }
            }
            return false;
        }

        public bool WriteServerState(long writeTimestamp, NetDataWriter writer, out bool shouldSendReliably)
        {
            shouldSendReliably = false;
            if (_sendingDash)
            {
                shouldSendReliably = true;
                MovementState |= MovementState.IsDash;
            }
            else
            {
                MovementState &= ~MovementState.IsDash;
            }
            if (_isTeleporting)
            {
                shouldSendReliably = true;
                if (_stillMoveAfterTeleport)
                    MovementState |= MovementState.IsTeleport;
                else
                    MovementState = MovementState.IsTeleport;
            }
            else
            {
                MovementState &= ~MovementState.IsTeleport;
            }
            // Sync transform from server to all clients (include owner client)
            this.ServerWriteSyncTransform2D(_movementForceAppliers, writer);
            _sendingDash = false;
            _isTeleporting = false;
            _stillMoveAfterTeleport = false;
            return true;
        }

        public void ReadClientStateAtServer(long peerTimestamp, NetDataReader reader)
        {
            switch (movementSecure)
            {
                case MovementSecure.NotSecure:
                    ReadSyncTransformAtServer(peerTimestamp, reader);
                    break;
                case MovementSecure.ServerAuthoritative:
                    ReadMovementInputAtServer(peerTimestamp, reader);
                    break;
            }
        }

        public async void ReadServerStateAtClient(long peerTimestamp, NetDataReader reader)
        {
            if (IsServer)
            {
                // Don't read and apply transform, because it was done at server
                return;
            }
            if (IsPreparingToTeleport)
            {
                // Not ready to apply transform
                return;
            }
            reader.ClientReadSyncTransformMessage2D(out MovementState movementState, out ExtraMovementState extraMovementState, out Vector2 position, out DirectionVector2 direction2D, out List<EntityMovementForceApplier> movementForceAppliers);
            _movementForceAppliers.Clear();
            _movementForceAppliers.AddRange(movementForceAppliers);
            if (movementState.Has(MovementState.IsTeleport))
            {
                // Server requested to teleport
                if (TeleportPreparer != null)
                    await TeleportPreparer.PrepareToTeleport(position, Quaternion.Euler(0f, 0f, 0f));
                OnTeleport(position, movementState != MovementState.IsTeleport);
            }
            else if (_acceptedPositionTimestamp <= peerTimestamp)
            {
                if (Vector2.Distance(position, EntityTransform.position) >= snapThreshold)
                {
                    // Snap character to the position if character is too far from the position
                    if (movementSecure == MovementSecure.ServerAuthoritative || !IsOwnerClient)
                    {
                        Direction2D = direction2D;
                        EntityTransform.position = position;
                        CurrentGameManager.ShouldPhysicSyncTransforms2D = true;
                    }
                    MovementState = movementState;
                    ExtraMovementState = extraMovementState;
                }
                else if (!IsOwnerClient)
                {
                    Direction2D = direction2D;
                    _simulatingKeyMovement = true;
                    if (Vector2.Distance(position, EntityTransform.position.GetXY()) > MIN_DISTANCE_TO_SIMULATE_MOVEMENT)
                        SetMovePaths(position, false);
                    else
                        NavPaths = null;
                    MovementState = movementState;
                    ExtraMovementState = extraMovementState;
                }
                _acceptedPositionTimestamp = peerTimestamp;
            }
            if (!IsOwnerClient && movementState.Has(MovementState.IsDash))
            {
                _acceptedDash = true;
            }
        }

        public void ReadMovementInputAtServer(long peerTimestamp, NetDataReader reader)
        {
            if (IsOwnerClient)
            {
                // Don't read and apply inputs, because it was done (this is both owner client and server)
                return;
            }
            if (movementSecure == MovementSecure.NotSecure)
            {
                // Movement handling at client, so don't read movement inputs from client (but have to read transform)
                return;
            }
            reader.ReadMovementInputMessage2D(out EntityMovementInputState inputState, out EntityMovementInput entityMovementInput);
            if (entityMovementInput.MovementState.Has(MovementState.IsTeleport))
            {
                // Teleport confirming from client
                _isServerWaitingTeleportConfirm = false;
            }
            if (_isServerWaitingTeleportConfirm)
            {
                // Waiting for teleport confirming
                return;
            }
            if (!Entity.CanMove())
            {
                // It can't move, so don't move
                return;
            }
            if (_acceptedPositionTimestamp > peerTimestamp)
            {
                // Invalid timestamp
                return;
            }
            // Prepare time
            long deltaTime = _acceptedPositionTimestamp > 0 ? (peerTimestamp - _acceptedPositionTimestamp) : 0;
            float unityDeltaTime = (float)(deltaTime * TIMESTAMP_TO_UNITY_TIME_MULTIPLIER);
            NavPaths = null;
            _tempMovementState = entityMovementInput.MovementState;
            _tempExtraMovementState = entityMovementInput.ExtraMovementState;
            if (inputState.Has(EntityMovementInputState.PositionChanged))
            {
                _simulatingKeyMovement = inputState.Has(EntityMovementInputState.IsKeyMovement);
                SetMovePaths(entityMovementInput.Position, !_simulatingKeyMovement);
            }
            Direction2D = entityMovementInput.Direction2D;
            if (entityMovementInput.MovementState.Has(MovementState.IsDash))
            {
                _acceptedDash = true;
            }
            if (inputState.Has(EntityMovementInputState.IsStopped))
            {
                StopMoveFunction();
            }
            _acceptedPositionTimestamp = peerTimestamp;
        }

        public void ReadSyncTransformAtServer(long peerTimestamp, NetDataReader reader)
        {
            if (IsOwnerClient)
            {
                // Don't read and apply transform, because it was done (this is both owner client and server)
                return;
            }
            if (movementSecure == MovementSecure.ServerAuthoritative)
            {
                // Movement handling at server, so don't read sync transform from client
                return;
            }
            reader.ServerReadSyncTransformMessage2D(out MovementState movementState, out ExtraMovementState extraMovementState, out Vector2 position, out DirectionVector2 direction2D);
            if (movementState.Has(MovementState.IsTeleport))
            {
                // Teleport confirming from client
                _isServerWaitingTeleportConfirm = false;
            }
            if (_isServerWaitingTeleportConfirm)
            {
                // Waiting for teleport confirming
                return;
            }
            if (_acceptedPositionTimestamp > peerTimestamp)
            {
                // Invalid timestamp
                return;
            }
            // Prepare time
            long deltaTime = _acceptedPositionTimestamp > 0 ? (peerTimestamp - _acceptedPositionTimestamp) : 0;
            float unityDeltaTime = (float)(deltaTime * TIMESTAMP_TO_UNITY_TIME_MULTIPLIER);
            // Prepare movement state
            Direction2D = direction2D;
            MovementState = movementState;
            ExtraMovementState = extraMovementState;
            // Prepare data for validation
            Vector3 oldPos = _acceptedPosition.HasValue ? _acceptedPosition.Value : EntityTransform.position;
            Vector3 newPos = position;
            // Calculate moveable distance
            float moveSpd = Entity.GetMoveSpeed(movementState, extraMovementState);
            float moveableDist = moveSpd * unityDeltaTime;
            if (moveableDist < 0.001f)
                moveableDist = 0.001f;
            // Movement validating, if it is valid, set the position follow the client, if not set position to proper one and tell client to teleport
            float clientMoveDist = Vector3.Distance(oldPos.GetXY(), newPos.GetXY());
            // Increase accumulate data to detect hacking
            float moveDistDiff = clientMoveDist > moveableDist ? (clientMoveDist - moveableDist) : 0f;
            _accumulateDeltaTime += unityDeltaTime;
            _accumulateDiffMoveDist += moveDistDiff;
            float moveDiffTime = moveDistDiff / moveSpd;
            if (!IsClient)
            {
                // Movement validating, if it is valid, set the position follow the client, if not set position to proper one and tell client to teleport
                if (moveDiffTime < 0.1f)
                {
                    // Allow to move to the position
                    _acceptedPosition = newPos;
                    EntityTransform.position = newPos;
                    CurrentGameManager.ShouldPhysicSyncTransforms2D = true;
                }
                else
                {
                    // Client moves too fast, adjust it
                    newPos = GetMoveablePosition(oldPos, newPos, clientMoveDist, moveableDist);
                    // And also adjust client's position
                    Teleport(newPos, Quaternion.identity, true);
                }
            }
            else
            {
                // It's both server and client, simulate movement
                if (Vector3.Distance(position, EntityTransform.position) > MIN_DISTANCE_TO_SIMULATE_MOVEMENT)
                {
                    if (moveDiffTime < 0.1f)
                    {
                        // Allow to move to the position
                        _acceptedPosition = newPos;
                    }
                    else
                    {
                        // Client moves too fast, adjust it
                        newPos = GetMoveablePosition(oldPos, newPos, clientMoveDist, moveableDist);
                        // And also adjust client's position
                        Teleport(newPos, Quaternion.identity, true);
                    }
                    SetMovePaths(position, false);
                }
            }
            if (movementState.Has(MovementState.IsDash))
            {
                _acceptedDash = true;
            }
            _acceptedPositionTimestamp = peerTimestamp;
        }

        protected virtual Vector3 GetMoveablePosition(Vector3 oldPos, Vector3 newPos, float clientMoveDist, float moveableDist)
        {
            Vector3 dir = (newPos.GetXY() - oldPos.GetXY()).normalized;
            Vector3 deltaMove = dir * Mathf.Min(clientMoveDist, moveableDist);
            return oldPos + deltaMove;
        }

        protected virtual void OnTeleport(Vector2 position, bool stillMoveAfterTeleport)
        {
            if (!stillMoveAfterTeleport)
                NavPaths = null;
            EntityTransform.position = position;
            CurrentGameManager.ShouldPhysicSyncTransforms2D = true;
            if (!IsServer && IsOwnerClient)
                _isClientConfirmingTeleport = true;
        }

        public bool CanPredictMovement()
        {
            return Entity.IsOwnerClient || (Entity.IsOwnerClientOrOwnedByServer && movementSecure == MovementSecure.NotSecure) || (Entity.IsServer && movementSecure == MovementSecure.ServerAuthoritative);
        }

        public bool AllowToJump()
        {
            return false;
        }

        public bool AllowToDash()
        {
            return true;
        }

        public bool AllowToCrouch()
        {
            return true;
        }

        public bool AllowToCrawl()
        {
            return true;
        }

        public bool AllowToStand()
        {
            return true;
        }

        public async UniTask WaitClientTeleportConfirm()
        {
            while (this != null && _isServerWaitingTeleportConfirm)
            {
                await UniTask.Yield();
            }
        }

        public bool IsWaitingClientTeleportConfirm()
        {
            return _isServerWaitingTeleportConfirm;
        }
    }
}