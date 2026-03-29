// ce scalability: #4

using Insthync.ManagedUpdating;
using System;
using System.Reflection;
using System.Collections.Generic;
//using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;
using MultiplayerARPG.Server.AI;

namespace MultiplayerARPG
{
    public partial class MonsterActivityComponent : BaseMonsterActivityComponent, IManagedUpdate
    {

        // Track whether we are currently registered into UpdateManager.
        // This allows us to safely unregister if tick-driving gets enabled after we were enabled.
        private bool _registeredWithUpdateManager;

        // Tick registry is optional in some builds. Use reflection so this component can compile
        // even when the tick scheduler / registry scripts are removed.
        private static class TickRegistryBridge
        {
            private const string RegistryTypeName = "MultiplayerARPG.Server.AI.MonsterActivityTickRegistry";
            private static bool _resolved;
            private static bool _available;
            private static PropertyInfo _pTickDrivenEnabled;
            private static MethodInfo _mRegister;
            private static MethodInfo _mUnregister;

            private static void Resolve()
            {
                if (_resolved)
                    return;

                _resolved = true;

                try
                {
                    Type type = null;
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; ++i)
                    {
                        type = assemblies[i].GetType(RegistryTypeName, false);
                        if (type != null)
                            break;
                    }

                    if (type == null)
                        return;

                    _available = true;
                    _pTickDrivenEnabled = type.GetProperty("TickDrivenEnabled", BindingFlags.Public | BindingFlags.Static);
                    _mRegister = type.GetMethod("Register", BindingFlags.Public | BindingFlags.Static);
                    _mUnregister = type.GetMethod("Unregister", BindingFlags.Public | BindingFlags.Static);
                }
                catch
                {
                    _available = false;
                }
            }

            public static bool IsTickDrivenEnabled()
            {
                Resolve();
                if (!_available || _pTickDrivenEnabled == null)
                    return false;
                try
                {
                    return (bool)_pTickDrivenEnabled.GetValue(null, null);
                }
                catch
                {
                    return false;
                }
            }

            public static void Register(MonsterActivityComponent component)
            {
                Resolve();
                if (!_available || _mRegister == null)
                    return;
                try
                {
                    _mRegister.Invoke(null, new object[] { component });
                }
                catch
                {
                    // Ignore - registry is optional
                }
            }

            public static void Unregister(MonsterActivityComponent component)
            {
                Resolve();
                if (!_available || _mUnregister == null)
                    return;
                try
                {
                    _mUnregister.Invoke(null, new object[] { component });
                }
                catch
                {
                    // Ignore - registry is optional
                }
            }
        }

        //protected static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("MonsterActivityComponent - Update");

        // NOTE (CPU): Cache transforms and squared distances to avoid repeated property lookups and sqrt.
        // This is especially important at high monster counts.
        protected Transform _movementTransform;

        // NOTE (CPU): Throttle expensive physics/overlap fallback checks when target is outside the cheap sqr test.
        // This does not add any external tick system; it's a simple time-based gate.
        protected float _nextExpensiveOverlapCheckTime;
        protected const float ExpensiveOverlapCheckInterval = 0.20f;

        // NOTE (CPU): Avoid recomputing squares in hot loops.
        protected float _maxDistanceFromSpawnPointSqr;
        protected float _minFollowSummonerDistanceSqr;
        protected float _minRandomWanderDistanceSqr;
        protected float _leashTargetDistanceFromSpawnPointSqr;

        // NOTE (CPU): Small deadzone to avoid churny rotations when already facing the target.
        protected const float TurnDeadzoneDegrees = 2f;

        [SerializeField]
        protected float turnSmoothSpeed = 10f;

        [Tooltip("Min random delay for next wander")]
        public float randomWanderDelayMin = 2f;
        [Tooltip("Max random delay for next wander")]
        public float randomWanderDelayMax = 5f;
        [Tooltip("Random distance around spawn position to wander")]
        public float randomWanderDistance = 2f;

        [Tooltip("Max distance it can move from spawn point, if it's <= 0, it will be determined that it is no limit")]
        public float maxDistanceFromSpawnPoint = 5f;

        // MMO-style leash: "leash if target is > X meters away for > Y seconds"
        // NOTE: This measures distance from SpawnPosition to TargetPosition.
        [Header("Leash (MMO style)")]
        [Tooltip("If > 0, when target is farther than this distance from monster spawn point, the leash timer starts (X).")]
        public float leashTargetDistanceFromSpawnPoint = 30f;
        [Tooltip("If > 0, monster will drop target and move back to spawn after target stays too far for this duration (Y).")]
        public float leashFarDuration = 3f;

        [Tooltip("Delay before find enemy again")]
        public float findEnemyDelayMin = 1f;
        [FormerlySerializedAs("findEnemyDelay")]
        public float findEnemyDelayMax = 3f;

        [Tooltip("If following target time reached this value it will stop following target")]
        public float followTargetDuration = 5f;

        [Tooltip("Turn to enemy speed")]
        public float turnToEnemySpeed = 800f;

        [Tooltip("Duration to pausing after received damage")]
        public float miniStunDuration = 0f;

        [Tooltip("If this is TRUE, monster will attacks buildings")]
        public bool isAttackBuilding = false;

        [Tooltip("If this is TRUE, monster will prioritize targetting buildings first")]
        [FormerlySerializedAs("isBuildingPriority")]
        public bool isAttackBuildingFirst = false;

        [Tooltip("If this is TRUE, monster will attacks targets while its summoner still idle")]
        [FormerlySerializedAs("isAggressiveWhileSummonerIdle")]
        public bool aggressiveWhileSummoned = false;

        [Tooltip("Delay before it can switch target again")]
        public float switchTargetDelay = 3;

        public ExtraMovementState stateWhileAggressive = ExtraMovementState.None;
        public ExtraMovementState stateWhileWander = ExtraMovementState.IsWalking;

        protected readonly List<DamageableEntity> _enemies = new List<DamageableEntity>();
        protected float _findEnemyCountDown;
        protected float _randomedWanderCountDown;
        protected float _randomedWanderDelay;

        // Legacy chase timer (now counts total time pursuing target, not "only while out of range")
        protected float _followEnemyElasped;

        // MMO leash timer (time target has been "too far")
        protected float _leashFarElapsed;

        protected Vector3 _lastPosition;
        protected BaseSkill _queueSkill;
        protected int _queueSkillLevel;
        protected bool _alreadySetActionState;
        protected bool _isLeftHandAttacking;
        protected float _lastSetDestinationTime;

        protected Vector3 _lastIssuedDestination;
        protected bool _hasLastIssuedDestination;
        protected bool _reachedSpawnPoint;
        protected bool _enemyExisted;
        protected float _pauseCountdown;
        protected float _lastSwitchTargetTime;

        protected virtual void Awake()
        {
            Entity.onNotifyEnemySpotted += Entity_onNotifyEnemySpotted;
            Entity.onNotifyEnemySpottedByAlly += Entity_onNotifyEnemySpottedByAlly;
            Entity.onReceivedDamage += Entity_onReceivedDamage;

            // NOTE (CPU): Cache MovementTransform reference for repeated reads.
            // This assumes MovementTransform is stable for the entity lifecycle.
            _movementTransform = Entity != null ? Entity.MovementTransform : null;
            RebuildCachedSqrs();
        }

        // NOTE: If you change any of the related public fields at runtime, call this.
        protected void RebuildCachedSqrs()
        {
            _maxDistanceFromSpawnPointSqr = maxDistanceFromSpawnPoint > 0f ? maxDistanceFromSpawnPoint * maxDistanceFromSpawnPoint : 0f;
            _minFollowSummonerDistanceSqr = CurrentGameInstance != null ? CurrentGameInstance.minFollowSummonerDistance * CurrentGameInstance.minFollowSummonerDistance : 0f;
            float minWander = Mathf.Max(1f, randomWanderDistance);
            _minRandomWanderDistanceSqr = minWander * minWander;
            _leashTargetDistanceFromSpawnPointSqr = leashTargetDistanceFromSpawnPoint > 0f ? leashTargetDistanceFromSpawnPoint * leashTargetDistanceFromSpawnPoint : 0f;
        }

        protected virtual void OnEnable()
        {
            // Always register into tick registry so tick systems can drive this component.
            TickRegistryBridge.Register(this);

            // If tick scheduler is driving, DO NOT also register into frame-based UpdateManager on server
            if (Entity != null && Entity.IsServer && TickRegistryBridge.IsTickDrivenEnabled())
            {
                _registeredWithUpdateManager = false;
                return;
            }

            UpdateManager.Register(this);
            _registeredWithUpdateManager = true;

            // NOTE (CPU): Reset throttles/caches.
            _nextExpensiveOverlapCheckTime = 0f;
            if (_movementTransform == null && Entity != null)
                _movementTransform = Entity.MovementTransform;
            RebuildCachedSqrs();
        }

        protected virtual void OnDisable()
        {
            TickRegistryBridge.Unregister(this);

            if (_registeredWithUpdateManager)
            {
                _registeredWithUpdateManager = false;
                UpdateManager.Unregister(this);
            }
        }

        protected override void OnDestroy()
        {
            Entity.onNotifyEnemySpotted -= Entity_onNotifyEnemySpotted;
            Entity.onNotifyEnemySpottedByAlly -= Entity_onNotifyEnemySpottedByAlly;
            Entity.onReceivedDamage -= Entity_onReceivedDamage;
            base.OnDestroy();
        }

        private void Entity_onNotifyEnemySpotted(BaseCharacterEntity enemy)
        {
            if (Entity.Characteristic != MonsterCharacteristic.Assist)
                return;
            // Warn that this character received damage to nearby characters
            List<BaseCharacterEntity> foundCharacters = Entity.FindAliveEntities<BaseCharacterEntity>(
                CharacterDatabase.VisualRange, true, false, false,
                CurrentGameInstance.playerLayer.Mask | CurrentGameInstance.playingLayer.Mask | CurrentGameInstance.monsterLayer.Mask);
            if (foundCharacters == null || foundCharacters.Count == 0) return;
            foreach (BaseCharacterEntity foundCharacter in foundCharacters)
            {
                foundCharacter.NotifyEnemySpottedByAlly(Entity, enemy);
            }
        }

        private void Entity_onNotifyEnemySpottedByAlly(BaseCharacterEntity ally, BaseCharacterEntity enemy)
        {
            if ((Entity.SummonerEntity != null && Entity.SummonerEntity == ally) ||
                Entity.Characteristic == MonsterCharacteristic.Assist)
                Entity.SetAttackTarget(enemy);
        }


        private void Entity_onReceivedDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, CombatAmountType combatAmountType, int totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime)
        {
            if (!instigator.TryGetEntity(out BaseCharacterEntity attackerCharacter))
                return;

            // If character is not dead, try to attack
            if (!Entity.IsDead())
            {
                if (Entity.GetTargetEntity() == null)
                {
                    // If no target enemy, set target enemy as attacker
                    Entity.SetAttackTarget(attackerCharacter);
                }
                else if (attackerCharacter != Entity.GetTargetEntity() && UnityEngine.Random.value > 0.5f && Time.unscaledTime - _lastSwitchTargetTime > switchTargetDelay)
                {
                    // Random 50% to change target when receive damage from anyone
                    _lastSwitchTargetTime = Time.unscaledTime;
                    Entity.SetAttackTarget(attackerCharacter);
                }
                _pauseCountdown = miniStunDuration;
            }
        }

        public virtual void ManagedUpdate()
        {
            var entity = Entity;
            if (entity == null)
                return;
            // If server is driven by tick scheduler, skip frame-based UpdateManager path
            if (entity.IsServer && TickRegistryBridge.IsTickDrivenEnabled())
            {
                // Tick-driving may be enabled after this component was already enabled.
                // Ensure we unregister from UpdateManager to avoid paying per-frame overhead.
                if (_registeredWithUpdateManager)
                {
                    _registeredWithUpdateManager = true;
                    UpdateManager.Unregister(this);
                }
                return;
            }

            var identity = entity.Identity;
            if (!entity.IsServer || identity == null || identity.CountSubscribers() == 0 || CharacterDatabase == null)
                return;

            if (entity.IsDead())
            {
                entity.StopMove();
                entity.SetTargetEntity(null);
                _followEnemyElasped = 0f;
                _leashFarElapsed = 0f;
                return;
            }

            float deltaTime = Time.unscaledDeltaTime;

            if (_pauseCountdown > 0f)
            {
                _pauseCountdown -= deltaTime;
                if (_pauseCountdown <= 0f)
                    _pauseCountdown = 0f;
                entity.StopMove();
                return;
            }

            //using (s_UpdateProfilerMarker.Auto())
            {
                entity.SetSmoothTurnSpeed(turnSmoothSpeed);

                // NOTE (CPU): Cache transform/position once per update.
                Transform movementTransform = _movementTransform != null ? _movementTransform : entity.MovementTransform;
                Vector3 currentPosition = movementTransform.position;

                if (entity.SummonerEntity != null)
                {
                    // Summoned behavior unchanged here (leashing is usually controlled by summoner-follow distance)
                    if (!UpdateAttackEnemy(deltaTime, currentPosition))
                    {
                        UpdateEnemyFindingActivity(deltaTime);

                        if ((currentPosition - entity.SummonerEntity.EntityTransform.position).sqrMagnitude > _minFollowSummonerDistanceSqr)
                            FollowSummoner();
                        else
                            UpdateWanderDestinationRandomingActivity(deltaTime);
                    }
                }
                else
                {
                    float distFromSpawnPointSqr = (entity.SpawnPosition - currentPosition).sqrMagnitude;

                    if (!_reachedSpawnPoint)
                    {
                        if (distFromSpawnPointSqr <= _minRandomWanderDistanceSqr)
                        {
                            _reachedSpawnPoint = true;
                            _followEnemyElasped = 0f;
                            _leashFarElapsed = 0f;
                            entity.SetTargetEntity(null);
                            ClearActionState();
                        }
                        return;
                    }

                    if (entity.IsInSafeArea)
                    {
                        entity.SetTargetEntity(null);
                        _followEnemyElasped = 0f;
                        _leashFarElapsed = 0f;
                        ClearActionState();
                        UpdateMoveBackToSpawnPointActivity(deltaTime);
                        return;
                    }

                    // Hard clamp: monster displacement from spawn (existing behavior)
                    if (maxDistanceFromSpawnPoint > 0f && distFromSpawnPointSqr >= _maxDistanceFromSpawnPointSqr)
                    {
                        entity.SetTargetEntity(null);
                        _followEnemyElasped = 0f;
                        _leashFarElapsed = 0f;
                        ClearActionState();
                        UpdateMoveBackToSpawnPointActivity(deltaTime);
                        return;
                    }

                    // MMO-style leash: if target is too far from spawn for too long -> drop target and return
                    if (entity.TryGetTargetEntity(out IDamageableEntity leashTarget) && leashTarget != null &&
                        leashTarget.GetObjectId() != entity.ObjectId && !leashTarget.IsDeadOrHideFrom(Entity) &&
                        leashTarget.CanReceiveDamageFrom(entity.GetInfo()))
                    {
                        // NOTE (CPU): Use squared distance to avoid Vector3.Distance() sqrt.
                        Vector3 leashTargetPos = leashTarget.GetTransform().position;
                        float distTargetFromSpawnSqr = (entity.SpawnPosition - leashTargetPos).sqrMagnitude;
                        bool tooFar = leashTargetDistanceFromSpawnPoint > 0f && distTargetFromSpawnSqr > _leashTargetDistanceFromSpawnPointSqr;

                        if (tooFar)
                            _leashFarElapsed += deltaTime;
                        else
                            _leashFarElapsed = 0f;

                        if (leashFarDuration > 0f && _leashFarElapsed >= leashFarDuration)
                        {
                            entity.SetTargetEntity(null);
                            _followEnemyElasped = 0f;
                            _leashFarElapsed = 0f;
                            ClearActionState();
                            UpdateMoveBackToSpawnPointActivity(deltaTime);
                            return;
                        }
                    }
                    else
                    {
                        _leashFarElapsed = 0f;
                    }

                    // Legacy timer: now means "total time pursuing current target"
                    if (followTargetDuration > 0f && _followEnemyElasped >= followTargetDuration)
                    {
                        entity.SetTargetEntity(null);
                        _followEnemyElasped = 0f;
                        _leashFarElapsed = 0f;
                        ClearActionState();
                        UpdateMoveBackToSpawnPointActivity(deltaTime);
                        return;
                    }

                    if (!UpdateAttackEnemy(deltaTime, currentPosition))
                    {
                        // No enemy, try find it
                        _enemyExisted = false;
                        _followEnemyElasped = 0f;
                        _leashFarElapsed = 0f;

                        UpdateEnemyFindingActivity(deltaTime);
                        // Random movement (if no enemy existed)
                        UpdateWanderDestinationRandomingActivity(deltaTime);
                    }
                }
            }
        }

        /// <summary>
        /// Tick-driven entry point (used by ServerTickScheduler systems).
        /// 
        /// Minimal bridge for now: reuses the existing ManagedUpdate() logic so the component
        /// continues to work even when not hooked into the tick scheduler. When you later
        /// rebuild the activity pipeline, replace this with a true fixed-delta update split
        /// across AI / MoveIntent / Combat phases.
        /// </summary>
        //public void TickAI(float deltaTime)
        //{
            // Current implementation uses Time.unscaledDeltaTime internally.
            // This keeps behavior identical in both legacy UpdateManager and tick-driven modes.
        //    ManagedUpdate();
        //}

        /// <summary>
        /// Optional phase hook for future refactors (movement intent application).
        /// Kept as a no-op for the minimal integration.
        /// </summary>
        //public void TickMoveIntent(float deltaTime)
        //{
            // Intentionally empty (minimal integration).
        //}

        /// <summary>
        /// Optional phase hook for future refactors (combat execution).
        /// Kept as a no-op for the minimal integration.
        /// </summary>
        //public void TickCombat(float deltaTime)
        //{
            // Intentionally empty (minimal integration).
        //}

        protected virtual void UpdateEnemyFindingActivity(float deltaTime)
        {
            _findEnemyCountDown -= deltaTime;
            if (_enemies.Count <= 0 && _findEnemyCountDown > 0f)
                return;

            // FIX: was (min, min) so it never randomized
            _findEnemyCountDown = UnityEngine.Random.Range(findEnemyDelayMin, findEnemyDelayMax);

            if (!FindEnemy())
                return;

            _enemyExisted = true;
        }

        protected virtual void UpdateMoveBackToSpawnPointActivity(float deltaTime)
        {
            _randomedWanderCountDown -= deltaTime;
            if (_randomedWanderCountDown > 0f)
                return;
            _randomedWanderCountDown = _randomedWanderDelay;
            if (!RandomWanderDestination())
                return;
            _reachedSpawnPoint = false;
        }

        protected virtual void UpdateWanderDestinationRandomingActivity(float deltaTime)
        {
            if (_enemyExisted)
                return;
            _randomedWanderCountDown -= deltaTime;
            if (_randomedWanderCountDown > 0f)
                return;
            _randomedWanderCountDown = _randomedWanderDelay;
            if (!RandomWanderDestination())
                return;
        }

        /// <summary>
        /// Return `TRUE` if following / attacking enemy
        /// </summary>
        private bool UpdateAttackEnemy(float deltaTime, Vector3 currentPosition)
        {
            if (Entity.Characteristic == MonsterCharacteristic.NoHarm || !Entity.TryGetTargetEntity(out IDamageableEntity targetEnemy))
            {
                // No target, stop attacking
                ClearActionState();
                _followEnemyElasped = 0f;
                _leashFarElapsed = 0f;
                return false;
            }

            if (targetEnemy.GetObjectId() == Entity.ObjectId || targetEnemy.IsDeadOrHideFrom(Entity) || !targetEnemy.CanReceiveDamageFrom(Entity.GetInfo()))
            {
                // If target is dead or in safe area stop attacking
                Entity.SetTargetEntity(null);
                ClearActionState();
                _followEnemyElasped = 0f;
                _leashFarElapsed = 0f;
                return false;
            }

            // If it has target then go to target
            if (targetEnemy != null && !Entity.IsPlayingActionAnimation() && !_alreadySetActionState)
            {
                // Random action state to do next time
                if (CharacterDatabase.RandomSkill(Entity, out _queueSkill, out _queueSkillLevel) && _queueSkill != null)
                {
                    // Cooling down
                    if (Entity.IndexOfSkillUsage(SkillUsageType.Skill, _queueSkill.DataId) >= 0)
                    {
                        _queueSkill = null;
                        _queueSkillLevel = 0;
                    }
                }
                _isLeftHandAttacking = !_isLeftHandAttacking;
                _alreadySetActionState = true;
                return true;
            }

            // This now counts total "engagement time" with this target
            _followEnemyElasped += deltaTime;

            // NOTE (CPU): Cache target transform position once per decision.
            Vector3 targetPosition = targetEnemy.GetTransform().position;
            float attackDistance = GetAttackDistance();

            if (OverlappedEntity(targetEnemy.Entity, GetDamageTransform().position, targetPosition, attackDistance))
            {
                // Stop movement
                SetWanderDestination(CacheTransform.position);

                // Look at target then do something when it's in range
                Vector3 lookAtDirection = (targetPosition - currentPosition);
                bool turnedToEnemy = false;
                if (lookAtDirection.sqrMagnitude > 0.0001f)
                {
                    if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                    {
                        // NOTE (CPU): Avoid Quaternion->Euler conversions. Use SignedAngle on flattened vectors.
                        Quaternion currentLookAtRotation = Entity.GetLookRotation();
                        Vector3 toTarget = lookAtDirection;
                        toTarget.y = 0f;
                        Vector3 forward = (currentLookAtRotation * Vector3.forward);
                        forward.y = 0f;

                        float yawDiff = Vector3.SignedAngle(forward, toTarget, Vector3.up);

                        // Only rotate if meaningful
                        if (Mathf.Abs(yawDiff) > TurnDeadzoneDegrees)
                        {
                            Quaternion desired = Quaternion.LookRotation(toTarget);
                            currentLookAtRotation = Quaternion.RotateTowards(
                                currentLookAtRotation,
                                desired,
                                turnToEnemySpeed * deltaTime);
                            Entity.SetLookRotation(currentLookAtRotation, false);
                        }

                        turnedToEnemy = Mathf.Abs(yawDiff) < 15f;
                    }
                    else
                    {
                        // Update 2D direction
                        Entity.SetLookRotation(Quaternion.LookRotation(lookAtDirection.normalized), false);
                        turnedToEnemy = true;
                    }
                }

                if (!turnedToEnemy)
                    return true;

                Entity.AimPosition = Entity.GetAttackAimPosition(ref _isLeftHandAttacking);
                if (Entity.IsPlayingActionAnimation())
                    return true;

                if (_queueSkill != null && Entity.IndexOfSkillUsage(SkillUsageType.Skill, _queueSkill.DataId) < 0)
                {
                    // Use skill when there is queue skill or randomed skill that can be used
                    Entity.UseSkill(_queueSkill.DataId, WeaponHandlingState.None, 0, new AimPosition()
                    {
                        type = AimPositionType.Position,
                        position = _queueSkill.GetDefaultAttackAimPosition(Entity, _queueSkillLevel, _isLeftHandAttacking, targetEnemy),
                    });
                }
                else
                {
                    // Attack when no queue skill
                    WeaponHandlingState weaponHandlingState = WeaponHandlingState.None;
                    if (_isLeftHandAttacking)
                        weaponHandlingState |= WeaponHandlingState.IsLeftHand;
                    if (Entity.Attack(ref weaponHandlingState))
                        _isLeftHandAttacking = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
                }

                ClearActionState();
            }
            else
            {
                // Follow the enemy
                SetDestination(targetPosition, attackDistance);
            }

            return true;
        }

        public void SetDestination(Vector3 destination, float distance)
        {
            float time = Time.unscaledTime;
            if (time - _lastSetDestinationTime <= 0.1f)
                return;
            _lastSetDestinationTime = time;

            // NOTE (CPU): Use cached movement transform.
            Transform movementTransform = _movementTransform != null ? _movementTransform : Entity.MovementTransform;
            Vector3 direction = (destination - movementTransform.position).normalized;
            Vector3 position = destination - (direction * (distance - Entity.StoppingDistance));
            // Skip issuing movement if destination is effectively unchanged
            if (_hasLastIssuedDestination && (position - _lastIssuedDestination).sqrMagnitude <= 0.0025f) // 5cm^2
                return;
            _lastIssuedDestination = position;
            _hasLastIssuedDestination = true;
            Entity.SetExtraMovementState(stateWhileAggressive);
            Entity.PointClickMovement(position);
        }

        public bool SetWanderDestination(Vector3 destination)
        {
            float time = Time.unscaledTime;
            if (time - _lastSetDestinationTime <= 0.1f)
                return false;
            _lastSetDestinationTime = time;

            Entity.SetExtraMovementState(stateWhileWander);
            // Skip issuing movement if destination is effectively unchanged
            if (_hasLastIssuedDestination && (destination - _lastIssuedDestination).sqrMagnitude <= 0.0025f) // 5cm^2
                return false;
            _lastIssuedDestination = destination;
            _hasLastIssuedDestination = true;
            Entity.PointClickMovement(destination);
            return true;
        }

        public virtual bool RandomWanderDestination()
        {
            if (!Entity.CanMove() || Entity.IsPlayingActionAnimation())
                return false;

            _randomedWanderDelay = UnityEngine.Random.Range(randomWanderDelayMin, randomWanderDelayMax);

            Vector3 randomPosition;
            // Random position around summoner or around spawn point
            if (Entity.SummonerEntity != null)
            {
                // Random position around summoner
                randomPosition = CurrentGameplayRule.GetSummonPosition(Entity.SummonerEntity);
            }
            else
            {
                // Random position around spawn point
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * randomWanderDistance;
                if (CurrentGameInstance.DimensionType == DimensionType.Dimension2D)
                    randomPosition = Entity.SpawnPosition + new Vector3(randomCircle.x, randomCircle.y);
                else
                    randomPosition = Entity.SpawnPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
            }

            if (!SetWanderDestination(randomPosition))
                return false;

            Entity.SetTargetEntity(null);
            _followEnemyElasped = 0f;
            _leashFarElapsed = 0f;
            return true;
        }

        public virtual void FollowSummoner()
        {
            Vector3 randomPosition;
            // Random position around summoner or around spawn point
            if (Entity.SummonerEntity != null)
            {
                // Random position around summoner
                randomPosition = CurrentGameplayRule.GetSummonPosition(Entity.SummonerEntity);
            }
            else
            {
                // Random position around spawn point
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * randomWanderDistance;
                if (CurrentGameInstance.DimensionType == DimensionType.Dimension2D)
                    randomPosition = Entity.SpawnPosition + new Vector3(randomCircle.x, randomCircle.y);
                else
                    randomPosition = Entity.SpawnPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
            }

            Entity.SetTargetEntity(null);
            _followEnemyElasped = 0f;
            _leashFarElapsed = 0f;
            SetDestination(randomPosition, 0f);
        }

        /// <summary>
        /// Return `TRUE` if found enemy
        /// </summary>
        public virtual bool FindEnemy()
        {
            // No harm, don't find enemy
            if (Entity.Characteristic == MonsterCharacteristic.NoHarm)
                return false;

            // Aggressive monster or summoned monster will find target to attack
            bool isAggressive = Entity.Characteristic == MonsterCharacteristic.Aggressive;
            if (!isAggressive && Entity.SummonerEntity == null)
                return false;

            if (!Entity.TryGetTargetEntity(out IDamageableEntity targetEntity) ||
                targetEntity.GetObjectId() == Entity.ObjectId || targetEntity.IsDead() ||
                !targetEntity.CanReceiveDamageFrom(Entity.GetInfo()))
            {
                bool isSummonedAndSummonerExisted = Entity.IsSummonedAndSummonerExisted;
                DamageableEntity enemy;

                // Find one enemy from previously found list
                if (FindOneEnemyFromList(isSummonedAndSummonerExisted, out enemy))
                {
                    Entity.SetAttackTarget(enemy);
                    _followEnemyElasped = 0f;
                    _leashFarElapsed = 0f;
                    return true;
                }

                // If no target enemy or target enemy is dead, Find nearby character by layer mask
                _enemies.Clear();
                int overlapMask = CurrentGameInstance.playerLayer.Mask | CurrentGameInstance.monsterLayer.Mask;
                if (isAttackBuilding)
                    overlapMask |= CurrentGameInstance.buildingLayer.Mask;

                if (isSummonedAndSummonerExisted)
                {
                    isAggressive = isAggressive || aggressiveWhileSummoned;
                    // Find enemy around summoner
                    _enemies.AddRange(Entity.FindAliveEntities<DamageableEntity>(
                        Entity.SummonerEntity.EntityTransform.position,
                        CharacterDatabase.SummonedVisualRange,
                        false, /* Don't find allies */
                        isAggressive,  /* Find enemies */
                        isAggressive,  /* Find neutral */
                        overlapMask));
                }
                else
                {
                    _enemies.AddRange(Entity.FindAliveEntities<DamageableEntity>(
                        CharacterDatabase.VisualRange,
                        false, /* Don't find allies */
                        true,  /* Find enemies */
                        false, /* Don't find neutral */
                        overlapMask));
                }

                // Find one enemy from a found list
                if (FindOneEnemyFromList(isSummonedAndSummonerExisted, out enemy))
                {
                    Entity.SetAttackTarget(enemy);
                    _followEnemyElasped = 0f;
                    _leashFarElapsed = 0f;
                    return true;
                }
            }

            return false;
        }

        protected virtual bool FindOneEnemyFromList(bool isSummonedAndSummonerExisted, out DamageableEntity enemy)
        {
            enemy = null;
            DamageableEntity tempEntity;
            BuildingEntity tempBuildingEntity;

            // NOTE (CPU): Avoid List.RemoveAt() in the hot selection loop.
            // We only scan and select; the list is cleared/refilled when scanning via FindAliveEntities().
            // Behavior preserved for "building first": prefer building if found, otherwise fall back to any valid enemy.
            DamageableEntity fallbackEnemy = null;

            for (int i = _enemies.Count - 1; i >= 0; --i)
            {
                tempEntity = _enemies[i];
                if (tempEntity == null)
                    continue;

                if (tempEntity.GetObjectId() == Entity.ObjectId || tempEntity.IsDead() || !tempEntity.CanReceiveDamageFrom(Entity.GetInfo()))
                {
                    // If enemy is null or cannot receive damage from monster, skip it
                    continue;
                }

                tempBuildingEntity = tempEntity as BuildingEntity;
                if (isAttackBuilding && isSummonedAndSummonerExisted && tempBuildingEntity != null && Entity.SummonerEntity.Id == tempBuildingEntity.CreatorId)
                {
                    // If building was built by summoner, skip it
                    continue;
                }

                if (isAttackBuilding && isAttackBuildingFirst)
                {
                    if (tempBuildingEntity != null)
                    {
                        // Found a building target, prefer it immediately
                        enemy = tempEntity;
                        break;
                    }
                    // Not building; remember as fallback but keep searching
                    if (fallbackEnemy == null)
                        fallbackEnemy = tempEntity;
                    continue;
                }

                // No building-first requirement, pick first valid
                enemy = tempEntity;
                break;
            }

            if (enemy == null)
                enemy = fallbackEnemy;

            return enemy != null;
        }

        protected virtual void ClearActionState()
        {
            _queueSkill = null;
            _isLeftHandAttacking = false;
            _alreadySetActionState = false;
        }

        protected Transform GetDamageTransform()
        {
            return _queueSkill != null ? _queueSkill.GetApplyTransform(Entity, _isLeftHandAttacking) :
                Entity.GetAvailableWeaponDamageInfo(ref _isLeftHandAttacking).GetDamageTransform(Entity, _isLeftHandAttacking);
        }

        protected float GetAttackDistance()
        {
            return _queueSkill != null && _queueSkill.IsAttack ? _queueSkill.GetCastDistance(Entity, _queueSkillLevel, _isLeftHandAttacking) :
                Entity.GetAttackDistance(_isLeftHandAttacking);
        }

        protected virtual bool OverlappedEntity<T>(T entity, Vector3 measuringPosition, Vector3 targetPosition, float distance)
            where T : BaseGameEntity
        {
            float distanceSqr = distance * distance;
            if ((measuringPosition - targetPosition).sqrMagnitude <= distanceSqr)
                return true;
            // Target is far from controlling entity, try overlap the entity
            // NOTE (CPU): Throttle expensive fallback (may involve physics queries).
            float now = Time.unscaledTime;
            if (now < _nextExpensiveOverlapCheckTime)
                return false;
            _nextExpensiveOverlapCheckTime = now + ExpensiveOverlapCheckInterval;
            return Entity.FindPhysicFunctions.IsGameEntityInDistance(entity, measuringPosition, distance, false);
        }
    }
}
