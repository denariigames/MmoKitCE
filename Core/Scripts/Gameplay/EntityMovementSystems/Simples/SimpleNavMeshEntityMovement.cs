using Cysharp.Threading.Tasks;
using Insthync.ManagedUpdating;
using LiteNetLib.Utils;
using LiteNetLibManager;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.AI;

namespace MultiplayerARPG
{
    /// <summary>
    /// This one is simple client-authoritative entity movement
    /// Being made to show how to simply implements entity movements by using `LiteNetLibTransform`
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(LiteNetLibTransform))]
    public class SimpleNavMeshEntityMovement : BaseNetworkedGameEntityComponent<BaseGameEntity>, IEntityMovementComponent, IManagedUpdate
    {
        protected const float MIN_MAGNITUDE_TO_DETERMINE_MOVING = 0.01f;
        protected const float MIN_DIRECTION_SQR_MAGNITUDE = 0.0001f;
        protected const float MIN_DISTANCE_TO_TELEPORT = 0.1f;
        protected static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("SimpleNavMeshEntityMovement - Update");

        [Header("Movement Settings")]
        public ObstacleAvoidanceType obstacleAvoidanceWhileMoving = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        public ObstacleAvoidanceType obstacleAvoidanceWhileStationary = ObstacleAvoidanceType.NoObstacleAvoidance;

        [Header("Dashing")]
        public EntityMovementForceApplierData dashingForceApplier = EntityMovementForceApplierData.CreateDefault();

        public LiteNetLibTransform NetworkedTransform { get; protected set; }
        public NavMeshAgent CacheNavMeshAgent { get; protected set; }
        public IEntityTeleportPreparer TeleportPreparer { get; protected set; }
        public bool IsPreparingToTeleport { get { return TeleportPreparer != null && TeleportPreparer.IsPreparingToTeleport; } }
        public float StoppingDistance
        {
            get { return CacheNavMeshAgent.stoppingDistance; }
        }
        public MovementState MovementState { get; protected set; }
        public ExtraMovementState ExtraMovementState { get; protected set; }
        public DirectionVector2 Direction2D { get { return Vector2.down; } set { } }
        public float CurrentMoveSpeed { get { return CacheNavMeshAgent.isStopped ? 0f : CacheNavMeshAgent.speed; } }

        // Input codes
        protected bool _isDashing;
        protected Vector3 _inputDirection;
        protected ExtraMovementState _tempExtraMovementState;

        // Teleportation
        protected MovementTeleportState _serverTeleportState;
        protected MovementTeleportState _clientTeleportState;

        // Force simulation
        protected readonly List<EntityMovementForceApplier> _movementForceAppliers = new List<EntityMovementForceApplier>();
        protected IEntityMovementForceUpdateListener[] _forceUpdateListeners;

        // Turn simulate codes
        protected bool _lookRotationApplied;
        protected float _yAngle;
        protected float _targetYAngle;
        protected float _yTurnSpeed;

        // Interpolation Data
        protected SortedList<uint, System.ValueTuple<MovementState, ExtraMovementState>> _interpExtra = new SortedList<uint, System.ValueTuple<MovementState, ExtraMovementState>>();

        private void Awake()
        {
            // Prepare nav mesh agent component
            NetworkedTransform = gameObject.GetOrAddComponent<LiteNetLibTransform>();
            CacheNavMeshAgent = gameObject.GetOrAddComponent<NavMeshAgent>();
            TeleportPreparer = gameObject.GetComponent<IEntityTeleportPreparer>();
            _forceUpdateListeners = gameObject.GetComponents<IEntityMovementForceUpdateListener>();
            Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.useGravity = false;
                rigidBody.isKinematic = true;
            }
            // Setup
            NetworkedTransform.syncByOwnerClient = true;
            NetworkedTransform.onWriteSyncBuffer += NetworkedTransform_onWriteSyncBuffer;
            NetworkedTransform.onReadInterpBuffer += NetworkedTransform_onReadInterpBuffer;
            NetworkedTransform.onValidateInterpolation += NetworkedTransform_onValidateInterpolation;
            NetworkedTransform.onInterpolate += NetworkedTransform_onInterpolate;
            CacheNavMeshAgent.enabled = false;
            _yAngle = _targetYAngle = EntityTransform.eulerAngles.y;
            _lookRotationApplied = true;
            _clientTeleportState = MovementTeleportState.Responding;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            NetworkedTransform.onWriteSyncBuffer -= NetworkedTransform_onWriteSyncBuffer;
            NetworkedTransform.onReadInterpBuffer -= NetworkedTransform_onReadInterpBuffer;
            NetworkedTransform.onValidateInterpolation -= NetworkedTransform_onValidateInterpolation;
            NetworkedTransform.onInterpolate -= NetworkedTransform_onInterpolate;
        }

        public override void OnSetOwnerClient(bool isOwnerClient)
        {
            CacheNavMeshAgent.enabled = CanSimulateMovement();
        }

        private void OnEnable()
        {
            CacheNavMeshAgent.enabled = CanSimulateMovement();
            UpdateManager.Register(this);
        }

        private void OnDisable()
        {
            CacheNavMeshAgent.enabled = false;
            UpdateManager.Unregister(this);
        }

        public bool CanSimulateMovement()
        {
            return Entity.IsOwnerClientOrOwnedByServer;
        }

        protected void NetworkedTransform_onWriteSyncBuffer(NetDataWriter writer, uint tick)
        {
            writer.PutPackedUInt((uint)MovementState);
            writer.PutPackedUInt((uint)ExtraMovementState);
        }

        protected void NetworkedTransform_onReadInterpBuffer(NetDataReader reader, uint tick)
        {
            _interpExtra[tick] = new System.ValueTuple<MovementState, ExtraMovementState>(
                (MovementState)reader.GetPackedUInt(),
                (ExtraMovementState)reader.GetPackedUInt());
            while (_interpExtra.Count > 30)
            {
                _interpExtra.RemoveAt(0);
            }
        }

        private bool NetworkedTransform_onValidateInterpolation(LiteNetLibTransform.TransformData interpFromData, LiteNetLibTransform.TransformData interpToData, LiteNetLibTransform.TransformData currentData, float interpTime)
        {
            if (IsServer && _serverTeleportState != MovementTeleportState.None)
            {
                // Waiting for client teleport confirmation
                return false;
            }
            return true;
        }

        protected void NetworkedTransform_onInterpolate(LiteNetLibTransform.TransformData interpFromData, LiteNetLibTransform.TransformData interpToData, float interpTime)
        {
            if (interpTime <= 0.75f)
            {
                if (_interpExtra.TryGetValue(interpFromData.Tick, out var states))
                {
                    MovementState = states.Item1;
                    ExtraMovementState = states.Item2;
                }
            }
            else
            {
                if (_interpExtra.TryGetValue(interpToData.Tick, out var states))
                {
                    MovementState = states.Item1;
                    ExtraMovementState = states.Item2;
                }
            }
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

        public bool FindGroundedPosition(Vector3 fromPosition, float findDistance, out Vector3 result)
        {
            result = fromPosition;
            float findDist = 1f;
            NavMeshHit navHit;
            while (!NavMesh.SamplePosition(fromPosition, out navHit, findDist, NavMesh.AllAreas))
            {
                findDist += 1f;
                if (findDist > findDistance)
                    return false;
            }
            result = navHit.position;
            return true;
        }

        public Bounds GetMovementBounds()
        {
            Vector3 agentPosition = transform.position;
            Vector3 lossyScale = transform.lossyScale;

            // Calculate the scaled extents using lossy scale
            float scaledRadius = CacheNavMeshAgent.radius * Mathf.Max(lossyScale.x, lossyScale.z);
            float scaledHeight = CacheNavMeshAgent.height * lossyScale.y;
            float baseOffset = CacheNavMeshAgent.baseOffset * lossyScale.y;

            // Adjust the center to include the baseOffset and scale
            Vector3 center = new Vector3(agentPosition.x, agentPosition.y + baseOffset + (scaledHeight * 0.5f), agentPosition.z);
            Vector3 size = new Vector3(scaledRadius * 2, scaledHeight, scaledRadius * 2);
            return new Bounds(center, size);
        }

        protected void SetMovePaths(Vector3 position)
        {
            if (!Entity.CanMove())
                return;
            _inputDirection = Vector3.zero;
            CacheNavMeshAgent.updatePosition = true;
            CacheNavMeshAgent.updateRotation = false;
            if (CacheNavMeshAgent.isOnNavMesh)
            {
                CacheNavMeshAgent.isStopped = false;

                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, position, CacheNavMeshAgent.areaMask, path);
                CacheNavMeshAgent.SetPath(path);
            }
        }

        public void SetSmoothTurnSpeed(float turnDuration)
        {
            _yTurnSpeed = turnDuration;
        }

        public float GetSmoothTurnSpeed()
        {
            return _yTurnSpeed;
        }

        public void KeyMovement(Vector3 moveDirection, MovementState movementState)
        {
            if (!Entity.IsOwnerClientOrOwnedByServer)
                return;
            _inputDirection = moveDirection;
            if (!_isDashing)
                _isDashing = movementState.Has(MovementState.IsDash);
        }

        public void PointClickMovement(Vector3 position)
        {
            if (!Entity.IsOwnerClientOrOwnedByServer)
                return;
            SetMovePaths(position);
        }

        public void SetExtraMovementState(ExtraMovementState extraMovementState)
        {
            if (!Entity.IsOwnerClientOrOwnedByServer)
                return;
            _tempExtraMovementState = extraMovementState;
        }

        public void SetLookRotation(Quaternion rotation, bool immediately)
        {
            if (!Entity.IsOwnerClientOrOwnedByServer)
                return;
            _targetYAngle = rotation.eulerAngles.y;
            _lookRotationApplied = false;
            if (immediately && Entity.CanTurn())
                TurnImmediately(_targetYAngle);
        }

        public Quaternion GetLookRotation()
        {
            return Quaternion.Euler(0f, EntityTransform.eulerAngles.y, 0f);
        }

        public void StopMove()
        {
            StopMoveFunction();
        }

        protected void StopMoveFunction()
        {
            _inputDirection = Vector3.zero;
            CacheNavMeshAgent.updatePosition = false;
            CacheNavMeshAgent.updateRotation = false;
            if (CacheNavMeshAgent.isOnNavMesh)
                CacheNavMeshAgent.isStopped = true;
        }

        public async void Teleport(Vector3 position, Quaternion rotation, bool stillMoveAfterTeleport)
        {
            if (!IsServer)
            {
                Logging.LogWarning(nameof(NavMeshEntityMovement), $"Teleport function shouldn't be called at client {name}");
                return;
            }
            if (_serverTeleportState != MovementTeleportState.None)
            {
                // Still waiting for teleport responding
                return;
            }
            await OnTeleport(position, rotation, stillMoveAfterTeleport);
        }

        protected async UniTask OnTeleport(Vector3 position, Quaternion rotation, bool stillMoveAfterTeleport)
        {
            if (Vector3.Distance(position, EntityTransform.position) <= MIN_DISTANCE_TO_TELEPORT)
            {
                // Too close to teleport
                return;
            }
            // Prepare before move
            if (IsServer && !IsOwnerClientOrOwnedByServer)
            {
                _serverTeleportState = MovementTeleportState.WaitingForResponse;
            }
            if (TeleportPreparer != null)
            {
                await TeleportPreparer.PrepareToTeleport(position, rotation);
            }
            // Move character to target position
            Vector3 beforeWarpDest = CacheNavMeshAgent.destination;
            CacheNavMeshAgent.Warp(position);
            if (!stillMoveAfterTeleport && CacheNavMeshAgent.isOnNavMesh)
            {
                CacheNavMeshAgent.isStopped = true;
            }
            if (stillMoveAfterTeleport && CacheNavMeshAgent.isOnNavMesh)
            {
                CacheNavMeshAgent.SetDestination(beforeWarpDest);
            }
            TurnImmediately(rotation.eulerAngles.y);
            // Prepare teleporation states
            if (IsServer && !IsOwnerClientOrOwnedByServer)
            {
                _serverTeleportState = MovementTeleportState.Requesting;
                if (stillMoveAfterTeleport)
                    _serverTeleportState |= MovementTeleportState.StillMoveAfterTeleport;
                _serverTeleportState |= MovementTeleportState.WaitingForResponse;
            }
            if (!IsServer && IsOwnerClient)
            {
                _clientTeleportState = MovementTeleportState.Responding;
            }
        }

        public async UniTask WaitClientTeleportConfirm()
        {
            while (this != null && _serverTeleportState != MovementTeleportState.None)
            {
                await UniTask.Delay(100);
            }
        }

        public bool IsWaitingClientTeleportConfirm()
        {
            return _serverTeleportState != MovementTeleportState.None;
        }

        protected float GetPathRemainingDistance()
        {
            if (CacheNavMeshAgent.pathPending ||
                CacheNavMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
                CacheNavMeshAgent.path.corners.Length == 0)
                return -1f;

            return CacheNavMeshAgent.remainingDistance;
        }

        protected float GetPathRemainingDistanceByCorners()
        {
            if (CacheNavMeshAgent.pathPending ||
                CacheNavMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                return -1f;

            Vector3[] corners = CacheNavMeshAgent.path.corners;
            float distance = 0.0f;
            for (int i = 0; i < corners.Length - 1; ++i)
            {
                distance += Vector3.Distance(corners[i], corners[i + 1]);
            }
            return distance;
        }

        public void ManagedUpdate()
        {
            if (!CanSimulateMovement())
                return;
            using (s_UpdateProfilerMarker.Auto())
            {
                float deltaTime = Time.deltaTime;
                UpdateMovement(deltaTime);
                UpdateRotation(deltaTime);
                _isDashing = false;
            }
        }

        public void UpdateMovement(float deltaTime)
        {
            if (IsPreparingToTeleport)
                return;

            ApplyMovementForceMode replaceMovementForceApplierMode = ApplyMovementForceMode.Default;
            // Update force applying
            // Dashing
            if (_isDashing)
            {
                // Can have only one replace movement force applier, so remove stored ones
                _movementForceAppliers.RemoveReplaceMovementForces();
                _movementForceAppliers.Add(new EntityMovementForceApplier().Apply(
                    ApplyMovementForceMode.Dash, EntityTransform.forward, ApplyMovementForceSourceType.None, 0, 0, dashingForceApplier));
            }

            // Apply Forces
            _forceUpdateListeners.OnPreUpdateForces(_movementForceAppliers);
            _movementForceAppliers.UpdateForces(deltaTime,
                Entity.GetMoveSpeed(MovementState.Forward, ExtraMovementState.None),
                out Vector3 forceMotion, out EntityMovementForceApplier replaceMovementForceApplier);
            _forceUpdateListeners.OnPostUpdateForces(_movementForceAppliers);

            // Replace player's movement by this
            if (replaceMovementForceApplier != null)
            {
                // Still dashing to add dash to movement state
                replaceMovementForceApplierMode = replaceMovementForceApplier.Mode;
                // Force turn to dashed direction
                _targetYAngle = Quaternion.LookRotation(replaceMovementForceApplier.Direction).eulerAngles.y;
                // Change move speed to dash force
                if (CacheNavMeshAgent.hasPath)
                {
                    CacheNavMeshAgent.isStopped = true;
                }
                if (CacheNavMeshAgent.isOnNavMesh)
                {
                    CacheNavMeshAgent.Move(replaceMovementForceApplier.CurrentSpeed * replaceMovementForceApplier.Direction * deltaTime);
                }
            }

            if (forceMotion.magnitude > MIN_DIRECTION_SQR_MAGNITUDE && CacheNavMeshAgent.isOnNavMesh)
            {
                CacheNavMeshAgent.Move(forceMotion * deltaTime);
            }

            bool isStationary = !CacheNavMeshAgent.isOnNavMesh || CacheNavMeshAgent.isStopped || GetPathRemainingDistance() <= CacheNavMeshAgent.stoppingDistance;

            CacheNavMeshAgent.obstacleAvoidanceType = isStationary ? obstacleAvoidanceWhileStationary : obstacleAvoidanceWhileMoving;
            MovementState = MovementState.IsGrounded;

            if (replaceMovementForceApplierMode == ApplyMovementForceMode.Dash)
                MovementState |= MovementState.IsDash;

            if (!replaceMovementForceApplierMode.IsReplaceMovement())
            {
                if (_inputDirection.sqrMagnitude > MIN_DIRECTION_SQR_MAGNITUDE)
                {
                    // Moving by WASD keys
                    MovementState |= MovementState.Forward;
                    ExtraMovementState = this.ValidateExtraMovementState(MovementState, _tempExtraMovementState);
                    CacheNavMeshAgent.speed = Entity.GetMoveSpeed(MovementState, ExtraMovementState);
                    CacheNavMeshAgent.updatePosition = true;
                    CacheNavMeshAgent.updateRotation = false;
                    if (CacheNavMeshAgent.isOnNavMesh)
                        CacheNavMeshAgent.isStopped = true;
                    CacheNavMeshAgent.Move(_inputDirection * CacheNavMeshAgent.speed * deltaTime);
                    // Turn character to destination
                    if (_lookRotationApplied && Entity.CanTurn())
                        _targetYAngle = Quaternion.LookRotation(_inputDirection).eulerAngles.y;
                }
                else
                {
                    // Moving by clicked position
                    bool isMoving = CacheNavMeshAgent.velocity.magnitude > MIN_MAGNITUDE_TO_DETERMINE_MOVING;
                    MovementState |= isMoving ? MovementState.Forward : MovementState.None;
                    ExtraMovementState = this.ValidateExtraMovementState(MovementState, _tempExtraMovementState);
                    CacheNavMeshAgent.speed = Entity.GetMoveSpeed(MovementState, ExtraMovementState);
                    // Turn character to destination
                    if (isMoving && _lookRotationApplied && Entity.CanTurn())
                        _targetYAngle = Quaternion.LookRotation(CacheNavMeshAgent.velocity.normalized).eulerAngles.y;
                }
            }
        }

        public void UpdateRotation(float deltaTime)
        {
            if (!Entity.CanTurn())
                return;

            if (_yTurnSpeed <= 0f)
                _yAngle = _targetYAngle;
            else if (Mathf.Abs(_yAngle - _targetYAngle) > 1f)
                _yAngle = Mathf.LerpAngle(_yAngle, _targetYAngle, _yTurnSpeed * deltaTime);

            _lookRotationApplied = true;
            RotateY();
        }

        protected void RotateY()
        {
            EntityTransform.eulerAngles = new Vector3(0f, _yAngle, 0f);
        }

        public bool WriteClientState(long writeTimestamp, NetDataWriter writer, out bool shouldSendReliably)
        {
            if (_clientTeleportState.Has(MovementTeleportState.Responding))
            {
                shouldSendReliably = true;
                writer.Put((byte)_clientTeleportState);
                _clientTeleportState = MovementTeleportState.None;
                return true;
            }
            shouldSendReliably = false;
            return false;
        }

        public bool WriteServerState(long writeTimestamp, NetDataWriter writer, out bool shouldSendReliably)
        {
            if (_serverTeleportState.Has(MovementTeleportState.Requesting))
            {
                shouldSendReliably = true;
                writer.Put((byte)_serverTeleportState);
                writer.Put(EntityTransform.position.x);
                writer.Put(EntityTransform.position.y);
                writer.Put(EntityTransform.position.z);
                writer.PutPackedUShort(Mathf.FloatToHalf(EntityTransform.eulerAngles.y));
                _serverTeleportState = MovementTeleportState.WaitingForResponse;
                return true;
            }
            shouldSendReliably = false;
            return false;
        }

        public void ReadClientStateAtServer(long peerTimestamp, NetDataReader reader)
        {
            MovementTeleportState movementTeleportState = (MovementTeleportState)reader.GetByte();
            if (movementTeleportState.Has(MovementTeleportState.Responding))
            {
                _serverTeleportState = MovementTeleportState.None;
                return;
            }
        }

        public async void ReadServerStateAtClient(long peerTimestamp, NetDataReader reader)
        {
            MovementTeleportState movementTeleportState = (MovementTeleportState)reader.GetByte();
            if (movementTeleportState.Has(MovementTeleportState.Requesting))
            {
                Vector3 position = new Vector3(
                    reader.GetFloat(),
                    reader.GetFloat(),
                    reader.GetFloat());
                float rotation = Mathf.HalfToFloat(reader.GetPackedUShort());
                bool stillMoveAfterTeleport = movementTeleportState.Has(MovementTeleportState.StillMoveAfterTeleport);
                if (!IsServer)
                    await OnTeleport(position, Quaternion.Euler(0f, rotation, 0f), stillMoveAfterTeleport);
                return;
            }
        }

        public void TurnImmediately(float yAngle)
        {
            _yAngle = _targetYAngle = yAngle;
            RotateY();
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
    }
}
