// ce scalability: #4
/*
* PERF NOTES / CHANGELOG (since original baseline)
*  Removed per-call NavMeshPath allocations by reusing a single NavMeshPath instance for SetMovePaths().
*  Removed client-side pathfinding for remote proxies when receiving replicated positions.
*    Remote proxies now converge to replicated positions without NavMesh.CalculatePath.
*  Further: remote proxies now disable NavMeshAgent entirely (big CPU win at scale) and use
*    MoveTowards transform smoothing.
*  Further: RemainingDistance is now cached/throttled to reduce per-frame NavMeshAgent.remainingDistance cost.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using MultiplayerARPG.Server.Scheduling;
using Cysharp.Threading.Tasks;
using LiteNetLib.Utils;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.AI;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshEntityMovement : BaseNetworkedGameEntityComponent<BaseGameEntity>, IEntityMovementComponent
    {

        protected static readonly float s_minMagnitudeToDetermineMoving = 0.01f;
        protected static readonly float s_minDistanceToSimulateMovement = 0.01f;
        protected static readonly float s_timestampToUnityTimeMultiplier = 0.001f;

        [Header("Movement Settings")]
        public ObstacleAvoidanceType obstacleAvoidanceWhileMoving = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        public ObstacleAvoidanceType obstacleAvoidanceWhileStationary = ObstacleAvoidanceType.NoObstacleAvoidance;
        public MovementSecure movementSecure = MovementSecure.NotSecure;

        [Header("Dashing")]
        public EntityMovementForceApplierData dashingForceApplier = EntityMovementForceApplierData.CreateDefault();

        [Header("Networking Settings")]
        public float snapThreshold = 5.0f;

        public NavMeshAgent CacheNavMeshAgent { get; private set; }
        public IEntityTeleportPreparer TeleportPreparer { get; protected set; }
        public bool IsPreparingToTeleport { get { return TeleportPreparer != null && TeleportPreparer.IsPreparingToTeleport; } }
        public float StoppingDistance
        {
            get { return CacheNavMeshAgent.stoppingDistance; }
        }
        public MovementState MovementState { get; protected set; }
        public ExtraMovementState ExtraMovementState { get; protected set; }
        public DirectionVector2 Direction2D { get { return Vector2.down; } set { } }
        public float CurrentMoveSpeed { get { return (CacheNavMeshAgent == null || !CacheNavMeshAgent.enabled || CacheNavMeshAgent.isStopped) ? 0f : CacheNavMeshAgent.speed; } }

        // Input codes
        protected bool _isDashing;
        protected Vector3 _inputDirection;
        protected ExtraMovementState _tempExtraMovementState;
        protected bool _moveByDestination;

        // NavMesh path caching (avoid per-call allocations)
        protected NavMeshPath _reusablePath;

        // Remote proxy movement target (avoid client-side pathfinding for replicated agents)
        protected bool _hasRemoteTargetPosition;
        protected Vector3 _remoteTargetPosition;

        // Remote proxy: optionally disable NavMeshAgent completely to reduce CPU.
        protected bool _remoteProxyAgentDisabled;

        // RemainingDistance caching/throttling (NavMeshAgent.remainingDistance is not free)
        protected const float k_RemainingDistanceUpdateInterval = 0.15f;
        protected float _cachedRemainingDistance = -1f;
        protected float _nextRemainingDistanceUpdateTime;
        protected bool _cachedRemainingDistanceValid;

        // Move simulate codes
        protected readonly List<EntityMovementForceApplier> _movementForceAppliers = new List<EntityMovementForceApplier>();
        protected IEntityMovementForceUpdateListener[] _forceUpdateListeners;

        // Client state codes
        protected EntityMovementInput _oldInput;
        protected EntityMovementInput _currentInput;
        protected bool _sendingDash;

        // State simulate codes
        protected float? _lagMoveSpeedRate;

        // Turn simulate codes
        protected bool _lookRotationApplied;
        protected float _yAngle;
        protected float _targetYAngle;
        protected float _yTurnSpeed;
        private float? _remoteTargetYAngle;

        // Teleport codes
        protected bool _isTeleporting;
        protected bool _stillMoveAfterTeleport;

        // Peers accept codes
        protected bool _acceptedDash;
        protected long _acceptedPositionTimestamp;
        protected MovementState _acceptedMovementStateBeforeStopped;
        protected ExtraMovementState _acceptedExtraMovementStateBeforeStopped;

        // Server validate codes
        private Vector3? _acceptedPosition = null;
        private float _accumulateDeltaTime = 0f;
        private float _accumulateDiffMoveDist = 0f;
        protected bool _isServerWaitingTeleportConfirm;

        // Client confirm codes
        protected bool _isClientConfirmingTeleport;
        protected bool _isStarted;

        // ---------------------------------------------------------------------
        // Tick-driven execution (no UpdateManager)
        // ---------------------------------------------------------------------
        // This component is driven ONLY by ServerTickScheduler ticks. It no longer
        // registers with UpdateManager.
        //
        // Implementation detail:
        // - We keep a static registry of enabled NavMeshEntityMovement instances.
        // - A single tick system (NavMeshEntityMovementTickSystem) iterates and
        //   executes movement/rotation using the scheduler-provided tick delta.
        // - The tick system is registered at runtime via reflection against
        //   ServerRuntimeSimulation's private `scheduler` field (keeps this change
        //   isolated to this file for testing).
        private static readonly List<NavMeshEntityMovement> s_tickInstances = new List<NavMeshEntityMovement>(1024);
        private static NavMeshEntityMovementTickSystem s_tickSystem;
        private static bool s_tickSystemRegistered;
        private static bool s_tickSystemRegisterAttempted;

        protected virtual void Awake()
        {
            // Prepare nav mesh agent component
            CacheNavMeshAgent = gameObject.GetOrAddComponent<NavMeshAgent>();
            _reusablePath = new NavMeshPath();
            TeleportPreparer = gameObject.GetComponent<IEntityTeleportPreparer>();
            _forceUpdateListeners = gameObject.GetComponents<IEntityMovementForceUpdateListener>();

            // Disable unused component
            LiteNetLibTransform disablingComp = gameObject.GetComponent<LiteNetLibTransform>();
            if (disablingComp != null)
            {
                Logging.LogWarning(nameof(NavMeshEntityMovement), "You can remove `LiteNetLibTransform` component from game entity, it's not being used anymore [" + name + "]");
                disablingComp.enabled = false;
            }
            Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.useGravity = false;
                rigidBody.isKinematic = true;
            }
            // Setup
            _yAngle = _targetYAngle = EntityTransform.eulerAngles.y;
            _lookRotationApplied = true;
            _isClientConfirmingTeleport = true;
            _isStarted = true;

        }

        private void OnEnable()
        {
            // Agent may be disabled for remote proxies (see UpdateMovement).
            bool disableForRemoteProxy = IsClient && !IsOwnerClient && !IsServer && !CanPredictMovement();
            CacheNavMeshAgent.enabled = !disableForRemoteProxy;
            _remoteProxyAgentDisabled = disableForRemoteProxy;

            // Tick-driven registration
            if (!s_tickInstances.Contains(this))
                s_tickInstances.Add(this);
            EnsureTickSystemRegistered();
        }

        private void OnDisable()
        {
            CacheNavMeshAgent.enabled = false;

            // Tick-driven unregistration
            int idx = s_tickInstances.IndexOf(this);
            if (idx >= 0)
                s_tickInstances.RemoveAt(idx);
        }

        public void KeyMovement(Vector3 moveDirection, MovementState movementState)
        {
            if (!Entity.CanMove())
                return;
            if (CanPredictMovement())
            {
                // Always apply movement to owner client (it's client prediction for server auth movement)
                _inputDirection = moveDirection;
                if (_inputDirection.sqrMagnitude > 0)
                {
                    _moveByDestination = false;
                    CacheNavMeshAgent.updatePosition = true;
                    CacheNavMeshAgent.updateRotation = false;
                    if (CacheNavMeshAgent.isOnNavMesh)
                        CacheNavMeshAgent.isStopped = true;
                }
                if (!_isDashing)
                    _isDashing = movementState.Has(MovementState.IsDash);
            }
        }

        public void PointClickMovement(Vector3 position)
        {
            if (!Entity.CanMove())
                return;
            if (CanPredictMovement())
            {
                // Always apply movement to owner client (it's client prediction for server auth movement)
                SetMovePaths(position);
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

        public void StopMove()
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
            _inputDirection = Vector3.zero;
            _moveByDestination = false;
            CacheNavMeshAgent.updatePosition = false;
            CacheNavMeshAgent.updateRotation = false;
            if (CacheNavMeshAgent.isOnNavMesh)
                CacheNavMeshAgent.isStopped = true;
        }

        public void SetLookRotation(Quaternion rotation, bool immediately)
        {
            if (!Entity.CanMove() || !Entity.CanTurn())
                return;
            if (CanPredictMovement())
            {
                // Always apply movement to owner client (it's client prediction for server auth movement)
                _targetYAngle = rotation.eulerAngles.y;
                _lookRotationApplied = false;
                if (immediately)
                    TurnImmediately(_targetYAngle);
            }
        }

        public Quaternion GetLookRotation()
        {
            return Quaternion.Euler(0f, EntityTransform.eulerAngles.y, 0f);
        }

        public void SetSmoothTurnSpeed(float turnDuration)
        {
            _yTurnSpeed = turnDuration;
        }

        public float GetSmoothTurnSpeed()
        {
            return _yTurnSpeed;
        }

        public async void Teleport(Vector3 position, Quaternion rotation, bool stillMoveAfterTeleport)
        {
            if (!IsServer)
            {
                Logging.LogWarning(nameof(NavMeshEntityMovement), "Teleport function shouldn't be called at client [" + name + "]");
                return;
            }
            if (IsPreparingToTeleport)
                return;
            _acceptedPosition = position;
            _isTeleporting = true;
            _stillMoveAfterTeleport = stillMoveAfterTeleport;
            if (TeleportPreparer != null)
                await TeleportPreparer.PrepareToTeleport(position, rotation);
            OnTeleport(position, rotation.eulerAngles.y, stillMoveAfterTeleport);
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

        protected float GetPathRemainingDistance()
        {
            // Throttle expensive remainingDistance reads.
            if (!CacheNavMeshAgent.enabled)
                return -1f;
            if (CacheNavMeshAgent.pathPending ||
                CacheNavMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                return -1f;

            float now = Time.time;
            if (_cachedRemainingDistanceValid && now < _nextRemainingDistanceUpdateTime)
                return _cachedRemainingDistance;

                _cachedRemainingDistance = CacheNavMeshAgent.remainingDistance;
                _cachedRemainingDistanceValid = true;
                _nextRemainingDistanceUpdateTime = now + k_RemainingDistanceUpdateInterval;
                return _cachedRemainingDistance;
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

        /// <summary>
        /// Tick-driven update entry point (called by NavMeshEntityMovementTickSystem).
        /// </summary>
        public virtual void TickUpdate(float deltaTime)
        {
            UpdateMovement(deltaTime);
            UpdateRotation(deltaTime);
            _isDashing = false;
            _acceptedDash = false;
        }

        

        // ---------------------------------------------------------------------
        // Tick-system binding (ServerTickScheduler)
        // ---------------------------------------------------------------------
        // The scheduler lives on GameInstance (see ServerRuntimeSimulation.cs: private ServerTickScheduler _serverScheduler).
        // We bind once (lazily) and register a single system that iterates all active NavMeshEntityMovement instances.
        //
        // This file is tick-only: if the scheduler cannot be found, this component will not update.
        // ---------------------------------------------------------------------

        private static ServerTickScheduler TryGetServerScheduler()
        {
            // Prefer GameInstance.Singleton if present
            try
            {
                var prop = typeof(GameInstance).GetProperty("Singleton", BindingFlags.Public | BindingFlags.Static);
                if (prop != null)
                {
                    var gi = prop.GetValue(null, null) as GameInstance;
                    if (gi != null)
                    {
                        var fi = typeof(GameInstance).GetField("_serverScheduler", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (fi != null)
                            return fi.GetValue(gi) as ServerTickScheduler;
                    }
                }
            }
            catch { }

            // Fallback: find an instance in scene (editor / unusual bootstrap)
            try
            {
                GameInstance gi = UnityEngine.Object.FindObjectOfType<GameInstance>();
                if (gi != null)
                {
                    var fi = typeof(GameInstance).GetField("_serverScheduler", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fi != null)
                        return fi.GetValue(gi) as ServerTickScheduler;
                }
            }
            catch { }

            return null;
        }

        private static void EnsureTickSystemRegistered()
        {
            if (s_tickSystemRegistered || s_tickSystemRegisterAttempted)
                return;

            s_tickSystemRegisterAttempted = true;

            ServerTickScheduler scheduler = TryGetServerScheduler();
            if (scheduler == null)
            {
                Logging.LogWarning(nameof(NavMeshEntityMovement), "ServerTickScheduler not found; NavMeshEntityMovement will not tick.");
                return;
            }

            if (s_tickSystem == null)
                s_tickSystem = new NavMeshEntityMovementTickSystem();

            // Register under the Movement channel.
            // If the channel doesn't exist yet, scheduler will create one at 20 Hz (matches common movement rates).
            scheduler.RegisterSystem(s_tickSystem, "Movement", 20);
            s_tickSystemRegistered = true;
        }

        private sealed class NavMeshEntityMovementTickSystem : ITickSystem
        {
            public string Name => "NavMeshEntityMovement";

            public void Prepare(in TickContext ctx)
            {
                // No pre-pass required.
            }

            public void Execute(in TickContext ctx)
            {
                float dt = ctx.FixedDelta;

                for (int i = 0; i < s_tickInstances.Count; ++i)
                {
                    NavMeshEntityMovement inst = s_tickInstances[i];
                    if (inst == null || !inst.isActiveAndEnabled)
                        continue;

                    if (!inst._isStarted)
                        continue;

                    inst.TickUpdate(dt);
                }
            }

            public void Commit(in TickContext ctx)
            {
                // No post-pass required.
            }
        }



        public void UpdateMovement(float deltaTime)
        {
            if (IsPreparingToTeleport)
                return;

            // Remote proxy: disable NavMeshAgent entirely and converge using simple transform smoothing.
            // This avoids NavMeshAgent internal update costs across large numbers of replicated entities.
            if (IsClient && !IsOwnerClient && !IsServer && !CanPredictMovement())
            {
                if (!_remoteProxyAgentDisabled)
                {
                    if (CacheNavMeshAgent.enabled)
                        CacheNavMeshAgent.enabled = false;
                    _remoteProxyAgentDisabled = true;
                    _cachedRemainingDistanceValid = false;
                }

                if (_hasRemoteTargetPosition)
                {
                        float moveSpeed = Entity.GetMoveSpeed();
                        Vector3 currentPos = EntityTransform.position;
                        Vector3 targetPos = _remoteTargetPosition;
                        Vector3 toTarget = targetPos - currentPos;
                        float toTargetSqr = toTarget.sqrMagnitude;
                        float minSqr = s_minDistanceToSimulateMovement * s_minDistanceToSimulateMovement;

                        if (toTargetSqr > minSqr)
                        {
                            float maxStep = moveSpeed * deltaTime;
                            Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, maxStep);
                            Vector3 step = newPos - currentPos;
                            EntityTransform.position = newPos;
                            CurrentGameManager.ShouldPhysicSyncTransforms = true;

                            if (_lookRotationApplied && Entity.CanTurn() && step.sqrMagnitude > 0.000001f)
                                _targetYAngle = Quaternion.LookRotation(step).eulerAngles.y;

                            // Drive animation states heuristically for remote proxies.
                            MovementState = MovementState.IsGrounded | MovementState.Forward;
                            ExtraMovementState = _acceptedExtraMovementStateBeforeStopped;
                        }
                        else
                        {
                            _hasRemoteTargetPosition = false;
                            MovementState = MovementState.IsGrounded;
                            ExtraMovementState = ExtraMovementState.None;
                        }
                }
                else
                {
                    MovementState = MovementState.IsGrounded;
                    ExtraMovementState = ExtraMovementState.None;
                }

                _currentInput = Entity.SetInputYAngle(_currentInput, EntityTransform.eulerAngles.y);
                return;
            }

            // Ensure agent is enabled for non-remote-proxy simulation paths.
            if (_remoteProxyAgentDisabled)
            {
                CacheNavMeshAgent.enabled = true;
                _remoteProxyAgentDisabled = false;
                _cachedRemainingDistanceValid = false;
            }

            // Prepare speed
            CacheNavMeshAgent.speed = Entity.GetMoveSpeed();

            ApplyMovementForceMode replaceMovementForceApplierMode = ApplyMovementForceMode.Default;
            // Update force applying
            // Dashing
            if (_acceptedDash || _isDashing)
            {
                _sendingDash = true;
                // Can have only one replace movement force applier, so remove stored ones
                _movementForceAppliers.RemoveReplaceMovementForces();
                _movementForceAppliers.Add(new EntityMovementForceApplier().Apply(
                    ApplyMovementForceMode.Dash, EntityTransform.forward, ApplyMovementForceSourceType.None, 0, 0, dashingForceApplier));
            }

            // Apply Forces
            _forceUpdateListeners.OnPreUpdateForces(_movementForceAppliers);
            Vector3 forceMotion;
            EntityMovementForceApplier replaceMovementForceApplier;
                _movementForceAppliers.UpdateForces(deltaTime,
                    Entity.GetMoveSpeed(MovementState.Forward, ExtraMovementState.None),
                    out forceMotion, out replaceMovementForceApplier);
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

            if (forceMotion.sqrMagnitude > 0f && CacheNavMeshAgent.isOnNavMesh)
            {
                CacheNavMeshAgent.Move(forceMotion * deltaTime);
            }

            bool isStationary = !CacheNavMeshAgent.isOnNavMesh || CacheNavMeshAgent.isStopped || GetPathRemainingDistance() <= CacheNavMeshAgent.stoppingDistance;
            if (CanPredictMovement())
            {
                CacheNavMeshAgent.obstacleAvoidanceType = isStationary ? obstacleAvoidanceWhileStationary : obstacleAvoidanceWhileMoving;
                MovementState = MovementState.IsGrounded;
                if (!replaceMovementForceApplierMode.IsReplaceMovement())
                {
                    if (_inputDirection.sqrMagnitude > 0f)
                    {
                        // Moving by WASD keys
                        CacheNavMeshAgent.Move(_inputDirection * CacheNavMeshAgent.speed * deltaTime);
                        MovementState |= MovementState.Forward;
                        // Turn character to destination
                        if (_lookRotationApplied && Entity.CanTurn())
                            _targetYAngle = Quaternion.LookRotation(_inputDirection).eulerAngles.y;
                    }
                    else
                    {
                        // Moving by clicked position
                        float velSqr = CacheNavMeshAgent.velocity.sqrMagnitude;
                        MovementState |= velSqr > (s_minMagnitudeToDetermineMoving * s_minMagnitudeToDetermineMoving) ? MovementState.Forward : MovementState.None;
                        // Turn character to destination
                        if (_lookRotationApplied && Entity.CanTurn() && velSqr > (s_minMagnitudeToDetermineMoving * s_minMagnitudeToDetermineMoving))
                            _targetYAngle = Quaternion.LookRotation(CacheNavMeshAgent.velocity.normalized).eulerAngles.y;
                    }
                }
                // Update extra movement state
                ExtraMovementState = this.ValidateExtraMovementState(MovementState, _tempExtraMovementState);
                // Set current input
                _currentInput = Entity.SetInputMovementState(_currentInput, MovementState);
                _currentInput = Entity.SetInputExtraMovementState(_currentInput, ExtraMovementState);
                if (_inputDirection.sqrMagnitude > 0f)
                {
                    _currentInput = Entity.SetInputIsKeyMovement(_currentInput, true);
                    _currentInput = Entity.SetInputPosition(_currentInput, EntityTransform.position);
                }
                else if (_moveByDestination)
                {
                    _currentInput = Entity.SetInputIsKeyMovement(_currentInput, false);
                    _currentInput = Entity.SetInputPosition(_currentInput, CacheNavMeshAgent.destination);
                }
            }
            else
            {
                // Disable obstacle avoidance because it won't predict movement, it is just moving to destination without obstacle avoidance
                CacheNavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                if (CacheNavMeshAgent.velocity.sqrMagnitude > (s_minMagnitudeToDetermineMoving * s_minMagnitudeToDetermineMoving))
                {
                    MovementState = _acceptedMovementStateBeforeStopped;
                    ExtraMovementState = _acceptedExtraMovementStateBeforeStopped;
                }
                else
                {
                    MovementState = MovementState.IsGrounded;
                    ExtraMovementState = ExtraMovementState.None;
                }
            }

            if (replaceMovementForceApplierMode == ApplyMovementForceMode.Dash)
                MovementState |= MovementState.IsDash;

            _currentInput = Entity.SetInputYAngle(_currentInput, EntityTransform.eulerAngles.y);
        }

        public void UpdateRotation(float deltaTime)
        {
            if (_yTurnSpeed <= 0f)
                _yAngle = _targetYAngle;
            else if (Mathf.Abs(_yAngle - _targetYAngle) > 1f)
                _yAngle = Mathf.LerpAngle(_yAngle, _targetYAngle, _yTurnSpeed * deltaTime);
            _lookRotationApplied = true;
            RotateY();
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

        private void RotateY()
        {
            EntityTransform.eulerAngles = new Vector3(0f, _yAngle, 0f);
        }

        private void SetMovePaths(Vector3 position)
        {
            if (!Entity.CanMove())
                return;
                _inputDirection = Vector3.zero;
                _moveByDestination = true;
                CacheNavMeshAgent.updatePosition = true;
                CacheNavMeshAgent.updateRotation = false;
                if (CacheNavMeshAgent.isOnNavMesh)
                {
                    CacheNavMeshAgent.isStopped = false;
                    // Reuse path allocation to avoid GC spikes
                    _reusablePath.ClearCorners();
                    {
                        NavMesh.CalculatePath(transform.position, position, CacheNavMeshAgent.areaMask, _reusablePath);
                    }
                    CacheNavMeshAgent.SetPath(_reusablePath);
                }
        }

        public bool WriteClientState(long writeTimestamp, NetDataWriter writer, out bool shouldSendReliably)
        {
            shouldSendReliably = false;
            if (!_isStarted)
                return false;
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
                this.ClientWriteSyncTransform3D(writer);
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
                    this.ClientWriteMovementInput3D(writer, inputState, _currentInput);
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
            if (!_isStarted)
                return false;
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
            this.ServerWriteSyncTransform3D(_movementForceAppliers, writer);
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
            reader.ClientReadSyncTransformMessage3D(out MovementState movementState, out ExtraMovementState extraMovementState, out Vector3 position, out float yAngle, out List<EntityMovementForceApplier> movementForceAppliers);
            _movementForceAppliers.Clear();
            _movementForceAppliers.AddRange(movementForceAppliers);
            if (movementState.Has(MovementState.IsTeleport))
            {
                // Server requested to teleport
                if (TeleportPreparer != null)
                    await TeleportPreparer.PrepareToTeleport(position, Quaternion.Euler(0f, yAngle, 0f));
                OnTeleport(position, yAngle, movementState != MovementState.IsTeleport);
            }
            else if (_acceptedPositionTimestamp <= peerTimestamp)
            {
                // Prepare time
                long deltaTime = _acceptedPositionTimestamp > 0 ? (peerTimestamp - _acceptedPositionTimestamp) : 0;
                float unityDeltaTime = (float)(deltaTime * s_timestampToUnityTimeMultiplier);
                if (Vector3.Distance(position, EntityTransform.position) >= snapThreshold)
                {
                    // Snap character to the position if character is too far from the position
                    if (movementSecure == MovementSecure.ServerAuthoritative || !IsOwnerClient)
                    {
                        EntityTransform.eulerAngles = new Vector3(0, yAngle, 0);
                        CacheNavMeshAgent.Warp(position);
                        _hasRemoteTargetPosition = false;
                    }
                }
                else if (!IsOwnerClient)
                {
                    RemoteTurnSimulation(yAngle, unityDeltaTime);
                    // Do not pathfind to replicated position (very expensive at scale).
                    // Remote proxies converge via transform smoothing with NavMeshAgent disabled.
                    _remoteTargetPosition = position;
                    _hasRemoteTargetPosition = true;
                }
                if (movementState.HasDirectionMovement())
                {
                    _acceptedMovementStateBeforeStopped = movementState;
                    _acceptedExtraMovementStateBeforeStopped = extraMovementState;
                }
                _acceptedPositionTimestamp = peerTimestamp;
            }
            if (!IsOwnerClient && movementState.Has(MovementState.IsDash))
            {
                _acceptedDash = true;
                TurnImmediately(yAngle);
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
            reader.ReadMovementInputMessage3D(out EntityMovementInputState inputState, out EntityMovementInput entityMovementInput);
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
            float unityDeltaTime = (float)(deltaTime * s_timestampToUnityTimeMultiplier);
            _tempExtraMovementState = entityMovementInput.ExtraMovementState;
            if (inputState.Has(EntityMovementInputState.PositionChanged))
            {
                SetMovePaths(entityMovementInput.Position);
            }
            if (inputState.Has(EntityMovementInputState.RotationChanged))
            {
                RemoteTurnSimulation(entityMovementInput.YAngle, unityDeltaTime);
            }
            if (entityMovementInput.MovementState.Has(MovementState.IsDash))
            {
                _acceptedDash = true;
                if (_remoteTargetYAngle.HasValue)
                    TurnImmediately(_remoteTargetYAngle.Value);
                else
                    TurnImmediately(_targetYAngle);
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
            reader.ServerReadSyncTransformMessage3D(out MovementState movementState, out ExtraMovementState extraMovementState, out Vector3 position, out float yAngle);
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
            float unityDeltaTime = (float)(deltaTime * s_timestampToUnityTimeMultiplier);
            // Prepare movement state
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
            if (!IsClient)
            {
                // If it is not a client, don't have to simulate movement, just set the position (but still simulate gravity)
                _acceptedPosition = newPos;
                EntityTransform.position = newPos;
                CurrentGameManager.ShouldPhysicSyncTransforms = true;
                // Update character rotation
                RemoteTurnSimulation(yAngle, unityDeltaTime);
            }
            else
            {
                // It's both server and client, simulate movement
                if (Vector3.Distance(position, EntityTransform.position) > s_minDistanceToSimulateMovement)
                {
                    _acceptedPosition = newPos;
                    SetMovePaths(position);
                }
                RemoteTurnSimulation(yAngle, unityDeltaTime);
            }
            if (movementState.Has(MovementState.IsDash))
            {
                _acceptedDash = true;
                TurnImmediately(yAngle);
            }
            _acceptedPositionTimestamp = peerTimestamp;
        }

        protected virtual Vector3 GetMoveablePosition(Vector3 oldPos, Vector3 newPos, float clientMoveDist, float moveableDist)
        {
            Vector3 dir = (newPos.GetXZ() - oldPos.GetXZ()).normalized;
            Vector3 deltaMove = dir * Mathf.Min(clientMoveDist, moveableDist);
            return oldPos + deltaMove;
        }

        protected virtual void OnTeleport(Vector3 position, float yAngle, bool stillMoveAfterTeleport)
        {
            _inputDirection = Vector3.zero;
            _moveByDestination = false;
            _hasRemoteTargetPosition = false;
            Vector3 beforeWarpDest = CacheNavMeshAgent.destination;
            CacheNavMeshAgent.Warp(position);
            if (!stillMoveAfterTeleport && CacheNavMeshAgent.isOnNavMesh)
                CacheNavMeshAgent.isStopped = true;
            if (stillMoveAfterTeleport && CacheNavMeshAgent.isOnNavMesh)
                CacheNavMeshAgent.SetDestination(beforeWarpDest);
            TurnImmediately(yAngle);
            if (IsServer && !IsOwnedByServer)
                _isServerWaitingTeleportConfirm = true;
            if (!IsServer && IsOwnerClient)
                _isClientConfirmingTeleport = true;
        }

        public bool CanPredictMovement()
        {
            return Entity.IsOwnerClient || (Entity.IsOwnerClientOrOwnedByServer && movementSecure == MovementSecure.NotSecure) || (Entity.IsServer && movementSecure == MovementSecure.ServerAuthoritative);
        }

        public void RemoteTurnSimulation(float yAngle, float deltaTime)
        {
            if (!IsClient)
            {
                // Turn to target immediately
                TurnImmediately(yAngle);
                return;
            }
            // Will turn smoothly later
            _targetYAngle = yAngle;
            _yTurnSpeed = 1f / deltaTime;
        }

        public void TurnImmediately(float yAngle)
        {
            _yAngle = _targetYAngle = yAngle;
            RotateY();
        }

        public void ApplyRemoteTurnAngle()
        {
            if (_remoteTargetYAngle.HasValue)
            {
                _targetYAngle = _remoteTargetYAngle.Value;
                _remoteTargetYAngle = null;
            }
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
                await UniTask.Delay(1000);
            }
        }

        public bool IsWaitingClientTeleportConfirm()
        {
            return _isServerWaitingTeleportConfirm;
        }
    }
}