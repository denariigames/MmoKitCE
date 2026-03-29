using UnityEngine;

namespace MultiplayerARPG
{
    public partial class MonsterActivityComponent
    {
        // -----------------------------
        // Tick-scheduler state (intents)
        // -----------------------------
        private bool _hasMoveIntent;
        private Vector3 _moveIntentDestination;
        private float _moveIntentStoppingDistance;
        private ExtraMovementState _moveIntentState;

        private bool _hasCombatIntent;
        private AimPosition _combatAim;
        private bool _combatUseSkill;
        private int _combatSkillId;
        private WeaponHandlingState _combatWeaponHandling;

        // Scheduler-friendly timers
        private float _followEnemyElapsedTick;
        private float _leashFarElapsedTick;

        // Throttle movement-apply rate (replaces Time.unscaledTime gate)
        private float _setDestinationCooldown;

        // Exposed to scheduler systems
        public void TickAI(float deltaTime)
        {
            if (!Entity.IsServer || CharacterDatabase == null)
                return;

            // AOI gating: if nobody is subscribed to this entity, skip AI decisions.
            // (Legacy ManagedUpdate had the same guard.)
            if (Entity.Identity != null && Entity.Identity.CountSubscribers() == 0)
                return;

            if (Entity.IsDead())
            {
                Entity.StopMove();
                Entity.SetTargetEntity(null);
                ClearIntents();
                ResetTickTimers();
                return;
            }

            // Always clear intents at start of AI phase; AI decides new ones
            ClearIntents();

            if (_pauseCountdown > 0f)
            {
                _pauseCountdown -= deltaTime;
                if (_pauseCountdown < 0f) _pauseCountdown = 0f;
                // stunned => no intents
                return;
            }

            Entity.SetSmoothTurnSpeed(turnSmoothSpeed);

            Vector3 currentPos = Entity.MovementTransform.position;

            // Summoned branch: leave as-is (but still scheduler-phased)
            if (Entity.SummonerEntity != null)
            {
                // If you ALSO want leash for summons, add it here (usually distance to summoner)
                if (!Tick_AttackOrChase_AIOnly(deltaTime, currentPos))
                {
                    Tick_FindEnemy(deltaTime);

                    float distToSummoner = Vector3.Distance(currentPos, Entity.SummonerEntity.EntityTransform.position);
                    if (distToSummoner > CurrentGameInstance.minFollowSummonerDistance)
                    {
                        // follow summoner intent
                        Vector3 summonPos = CurrentGameplayRule.GetSummonPosition(Entity.SummonerEntity);
                        SetMoveIntentAggro(summonPos, 0f);
                    }
                    else
                    {
                        Tick_Wander(deltaTime);
                    }
                }
                return;
            }

            // -----------------------------
            // Non-summoned monster (world mob)
            // -----------------------------

            float distFromSpawn = Vector3.Distance(Entity.SpawnPosition, currentPos);

            if (!_reachedSpawnPoint)
            {
                if (distFromSpawn <= Mathf.Max(1f, randomWanderDistance))
                {
                    _reachedSpawnPoint = true;
                    Entity.SetTargetEntity(null);
                    ResetTickTimers();
                    ClearActionState();
                }
                return;
            }

            if (Entity.IsInSafeArea)
            {
                Entity.SetTargetEntity(null);
                ResetTickTimers();
                ClearActionState();
                Tick_MoveBackToSpawn(deltaTime);
                return;
            }

            // Existing hard clamp: monster displaced from spawn
            if (maxDistanceFromSpawnPoint > 0f && distFromSpawn >= maxDistanceFromSpawnPoint)
            {
                Entity.SetTargetEntity(null);
                ResetTickTimers();
                ClearActionState();
                Tick_MoveBackToSpawn(deltaTime);
                return;
            }

            // MMO leash: if target is too far FROM SPAWN for too long => drop + return
            if (Entity.TryGetTargetEntity(out IDamageableEntity leashTarget) &&
                leashTarget != null &&
                leashTarget.GetObjectId() != Entity.ObjectId &&
                !leashTarget.IsDeadOrHideFrom(Entity) &&
                leashTarget.CanReceiveDamageFrom(Entity.GetInfo()))
            {
                float targetDistFromSpawn = Vector3.Distance(Entity.SpawnPosition, leashTarget.GetTransform().position);
                bool tooFar = leashTargetDistanceFromSpawnPoint > 0f &&
                              targetDistFromSpawn > leashTargetDistanceFromSpawnPoint;

                if (tooFar)
                    _leashFarElapsedTick += deltaTime;
                else
                    _leashFarElapsedTick = 0f;

                if (leashFarDuration > 0f && _leashFarElapsedTick >= leashFarDuration)
                {
                    Entity.SetTargetEntity(null);
                    ResetTickTimers();
                    ClearActionState();
                    Tick_MoveBackToSpawn(deltaTime);
                    return;
                }
            }
            else
            {
                _leashFarElapsedTick = 0f;
            }

            // Optional: time-based pursuit stop (works properly now)
            if (followTargetDuration > 0f && _followEnemyElapsedTick >= followTargetDuration)
            {
                Entity.SetTargetEntity(null);
                ResetTickTimers();
                ClearActionState();
                Tick_MoveBackToSpawn(deltaTime);
                return;
            }

            if (!Tick_AttackOrChase_AIOnly(deltaTime, currentPos))
            {
                _enemyExisted = false;
                ResetTickTimers();

                Tick_FindEnemy(deltaTime);
                Tick_Wander(deltaTime);
            }
        }

        public void TickMoveIntent(float deltaTime)
        {
            if (!Entity.IsServer || CharacterDatabase == null)
                return;

            // throttle movement intent application
            if (_setDestinationCooldown > 0f)
            {
                _setDestinationCooldown -= deltaTime;
                if (_setDestinationCooldown < 0f) _setDestinationCooldown = 0f;
            }

            if (_pauseCountdown > 0f)
            {
                Entity.StopMove();
                return;
            }

            if (!_hasMoveIntent)
                return;

            if (_setDestinationCooldown > 0f)
                return;

            _setDestinationCooldown = 0.10f; // matches old 0.1s gate

            Entity.SetExtraMovementState(_moveIntentState);

            Vector3 dir = (_moveIntentDestination - Entity.MovementTransform.position).normalized;
            Vector3 pos = _moveIntentDestination - (dir * (_moveIntentStoppingDistance - Entity.StoppingDistance));
            Entity.PointClickMovement(pos);
        }

        public void TickCombat(float deltaTime)
        {
            if (!Entity.IsServer || CharacterDatabase == null)
                return;

            if (_pauseCountdown > 0f)
                return;

            if (!_hasCombatIntent)
                return;

            // Aim already computed in AI tick
            Entity.AimPosition = _combatAim;

            if (Entity.IsPlayingActionAnimation())
                return;

            if (_combatUseSkill)
            {
                Entity.UseSkill(_combatSkillId, WeaponHandlingState.None, 0, _combatAim);
            }
            else
            {
                var wh = _combatWeaponHandling;
                if (Entity.Attack(ref wh))
                    _isLeftHandAttacking = wh.Has(WeaponHandlingState.IsLeftHand);
            }

            // After executing combat, clear action state like legacy
            ClearActionState();
            _hasCombatIntent = false;
        }

        // -----------------------------
        // Helpers: AI-only decision pass
        // -----------------------------

        private bool Tick_AttackOrChase_AIOnly(float deltaTime, Vector3 currentPos)
        {
            if (Entity.Characteristic == MonsterCharacteristic.NoHarm ||
                !Entity.TryGetTargetEntity(out IDamageableEntity targetEnemy))
            {
                ClearActionState();
                ResetTickTimers();
                return false;
            }

            if (targetEnemy == null ||
                targetEnemy.GetObjectId() == Entity.ObjectId ||
                targetEnemy.IsDeadOrHideFrom(Entity) ||
                !targetEnemy.CanReceiveDamageFrom(Entity.GetInfo()))
            {
                Entity.SetTargetEntity(null);
                ClearActionState();
                ResetTickTimers();
                return false;
            }

            // Select next action once (same as legacy)
            if (!Entity.IsPlayingActionAnimation() && !_alreadySetActionState)
            {
                if (CharacterDatabase.RandomSkill(Entity, out _queueSkill, out _queueSkillLevel) && _queueSkill != null)
                {
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

            _followEnemyElapsedTick += deltaTime;

            Vector3 targetPos = targetEnemy.GetTransform().position;
            float attackDist = GetAttackDistance();

            if (OverlappedEntity(targetEnemy.Entity, GetDamageTransform().position, targetPos, attackDist))
            {
                // In range: stop and request combat in combat phase
                SetMoveIntentWander(Entity.MovementTransform.position); // basically stop

                // Turn toward target now (AI phase) so combat is valid
                Vector3 lookDir = (targetPos - currentPos).normalized;
                if (lookDir.sqrMagnitude > 0f)
                {
                    if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                    {
                        var currentRot = Entity.GetLookRotation();
                        var euler = Quaternion.LookRotation(lookDir).eulerAngles;
                        euler.x = 0; euler.z = 0;
                        currentRot = Quaternion.RotateTowards(
                            currentRot,
                            Quaternion.Euler(euler),
                            turnToEnemySpeed * deltaTime);
                        Entity.SetLookRotation(currentRot, false);

                        // If not turned enough, don’t queue combat yet
                        if (Mathf.Abs(Mathf.DeltaAngle(currentRot.eulerAngles.y, euler.y)) >= 15f)
                            return true;
                    }
                    else
                    {
                        Entity.SetLookRotation(Quaternion.LookRotation(lookDir), false);
                    }
                }

                // Build combat intent
                Entity.AimPosition = Entity.GetAttackAimPosition(ref _isLeftHandAttacking);

                var aim = new AimPosition
                {
                    type = AimPositionType.Position,
                    position = (_queueSkill != null)
                        ? _queueSkill.GetDefaultAttackAimPosition(Entity, _queueSkillLevel, _isLeftHandAttacking, targetEnemy)
                        : targetPos
                };

                if (_queueSkill != null && Entity.IndexOfSkillUsage(SkillUsageType.Skill, _queueSkill.DataId) < 0)
                {
                    _hasCombatIntent = true;
                    _combatUseSkill = true;
                    _combatSkillId = _queueSkill.DataId;
                    _combatAim = aim;
                }
                else
                {
                    _hasCombatIntent = true;
                    _combatUseSkill = false;
                    _combatAim = new AimPosition { type = AimPositionType.Position, position = aim.position };

                    WeaponHandlingState wh = WeaponHandlingState.None;
                    if (_isLeftHandAttacking)
                        wh |= WeaponHandlingState.IsLeftHand;
                    _combatWeaponHandling = wh;
                }

                return true;
            }

            // Out of range: chase (movement intent only)
            SetMoveIntentAggro(targetPos, attackDist);
            return true;
        }

        private void Tick_FindEnemy(float deltaTime)
        {
            _findEnemyCountDown -= deltaTime;
            if (_enemies.Count <= 0 && _findEnemyCountDown > 0f)
                return;

            // FIX: real bug in your original script
            _findEnemyCountDown = Random.Range(findEnemyDelayMin, findEnemyDelayMax);

            if (FindEnemy())
                _enemyExisted = true;
        }

        private void Tick_Wander(float deltaTime)
        {
            if (_enemyExisted)
                return;

            _randomedWanderCountDown -= deltaTime;
            if (_randomedWanderCountDown > 0f)
                return;

            _randomedWanderDelay = Random.Range(randomWanderDelayMin, randomWanderDelayMax);
            _randomedWanderCountDown = _randomedWanderDelay;

            if (!Entity.CanMove() || Entity.IsPlayingActionAnimation())
                return;

            Vector2 rnd = Random.insideUnitCircle * randomWanderDistance;
            Vector3 randomPos = (CurrentGameInstance.DimensionType == DimensionType.Dimension2D)
                ? Entity.SpawnPosition + new Vector3(rnd.x, rnd.y, 0f)
                : Entity.SpawnPosition + new Vector3(rnd.x, 0f, rnd.y);

            SetMoveIntentWander(randomPos);
        }

        private void Tick_MoveBackToSpawn(float deltaTime)
        {
            _randomedWanderCountDown -= deltaTime;
            if (_randomedWanderCountDown > 0f)
                return;

            _randomedWanderDelay = Random.Range(randomWanderDelayMin, randomWanderDelayMax);
            _randomedWanderCountDown = _randomedWanderDelay;

            SetMoveIntentWander(Entity.SpawnPosition);
            _reachedSpawnPoint = false;
        }

        private void SetMoveIntentAggro(Vector3 destination, float stoppingDistance)
        {
            _hasMoveIntent = true;
            _moveIntentDestination = destination;
            _moveIntentStoppingDistance = stoppingDistance;
            _moveIntentState = stateWhileAggressive;
        }

        private void SetMoveIntentWander(Vector3 destination)
        {
            _hasMoveIntent = true;
            _moveIntentDestination = destination;
            _moveIntentStoppingDistance = 0f;
            _moveIntentState = stateWhileWander;
        }

        private void ClearIntents()
        {
            _hasMoveIntent = false;
            _hasCombatIntent = false;
        }

        private void ResetTickTimers()
        {
            _followEnemyElapsedTick = 0f;
            _leashFarElapsedTick = 0f;
        }
    }
}
