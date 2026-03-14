using Cysharp.Threading.Tasks;
using LiteNetLib;
using LiteNetLibManager;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(CharacterActionComponentManager))]
    public class DefaultCharacterAttackComponent : BaseNetworkedGameEntityComponent<BaseCharacterEntity>, ICharacterAttackComponent
    {
        public const float RATE_OF_FIRE_BASE = 60f;

        protected struct AttackState
        {
            public int SimulateSeed;
            public WeaponHandlingState WeaponHandlingState;
        }

        protected readonly List<CancellationTokenSource> _attackCancellationTokenSources = new List<CancellationTokenSource>();
        public bool IsAttacking
        {
            get
            {
                return _simulateState.HasValue;
            }
        }
        public float LastAttackEndTime { get; protected set; }
        protected bool _skipMovementValidation;
        public bool IsSkipMovementValidationWhileAttacking { get { return _skipMovementValidation; } set { _skipMovementValidation = value; } }
        protected bool _shouldUseRootMotion;
        public bool IsUseRootMotionWhileAttacking { get { return _shouldUseRootMotion; } protected set { _shouldUseRootMotion = value; } }
        public float MoveSpeedRateWhileAttacking { get; protected set; }
        public MovementRestriction MovementRestrictionWhileAttacking { get; protected set; }
        public System.Action OnAttackStart { get; set; }
        public System.Action<int> OnAttackTrigger { get; set; }
        public System.Action OnAttackEnd { get; set; }
        public System.Action OnAttackInterupted { get; set; }
        public AnimActionType AnimActionType { get; protected set; }
        public int AnimActionDataId { get; protected set; }
        public IHitRegistrationManager HitRegistrationManager { get { return BaseGameNetworkManager.Singleton.HitRegistrationManager; } }

        public bool doNotRandomAnimation;
        public float animationResetDelay = 2f;

        protected CharacterActionComponentManager _manager;
        protected int _lastAttackAnimationIndex = 0;
        protected int _lastAttackDataId = 0;
        // Network data sending
        protected AttackState? _simulateState;
        // Logging data
        bool _entityIsPlayer = false;
        BasePlayerCharacterEntity _playerCharacterEntity = null;

        protected virtual void Start()
        {
            _manager = GetComponent<CharacterActionComponentManager>();
            if (Entity is BasePlayerCharacterEntity)
            {
                _entityIsPlayer = true;
                _playerCharacterEntity = Entity as BasePlayerCharacterEntity;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CancelAttack();
            ClearAttackStates();
            _manager = null;
            _entityIsPlayer = false;
            _playerCharacterEntity = null;
        }

        protected virtual void SetAttackActionStates(AnimActionType animActionType, int animActionDataId, AttackState simulateState)
        {
            ClearAttackStates();
            AnimActionType = animActionType;
            AnimActionDataId = animActionDataId;
            _simulateState = simulateState;
        }

        public virtual void ClearAttackStates()
        {
            _simulateState = null;
        }

        protected virtual async UniTaskVoid AttackRoutine(long peerTimestamp, AttackState simulateState)
        {
            CharacterDataCache entityCaches = Entity.GetCaches();
            int simulateSeed = GetSimulateSeed(peerTimestamp);
            WeaponHandlingState weaponHandlingState = simulateState.WeaponHandlingState;
            bool isLeftHand = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
            if (simulateState.SimulateSeed == 0)
                simulateState.SimulateSeed = simulateSeed;
            else
                simulateSeed = simulateState.SimulateSeed;

            // Prepare required data and get weapon data
            Entity.GetAttackingData(
                ref isLeftHand,
                out AnimActionType animActionType,
                out int animActionDataId,
                out CharacterItem weapon,
                out DamageInfo damageInfo);

            // Get playing animation index
            int randomMax = Entity.GetRandomMaxAnimationData(animActionType, animActionDataId);
            if (Time.unscaledTime - LastAttackEndTime > animationResetDelay || _lastAttackDataId != animActionDataId)
                _lastAttackAnimationIndex = 0;
            int animationIndex = _lastAttackAnimationIndex++;
            if (!doNotRandomAnimation)
                animationIndex = GenericUtils.RandomInt(simulateSeed, 0, randomMax);
            if (_lastAttackAnimationIndex >= randomMax)
                _lastAttackAnimationIndex = 0;
            _lastAttackDataId = animActionDataId;

            // Prepare required data and get animation data
            Entity.GetAnimationData(
                animActionType,
                animActionDataId,
                animationIndex,
                out float animSpeedRate,
                out float[] triggerDurations,
                out float totalDuration);

            // Set doing action state at clients and server
            SetAttackActionStates(animActionType, animActionDataId, simulateState);

            // Prepare required data and get damages data
            IWeaponItem weaponItem = weapon.GetWeaponItem();
            Dictionary<DamageElement, MinMaxFloat> baseDamageAmounts;
            if (isLeftHand && entityCaches.LeftHandDamages != null)
                baseDamageAmounts = new Dictionary<DamageElement, MinMaxFloat>(entityCaches.LeftHandDamages);
            else
                baseDamageAmounts = new Dictionary<DamageElement, MinMaxFloat>(entityCaches.RightHandDamages);

            // Calculate move speed rate while doing action at clients and server
            MoveSpeedRateWhileAttacking = Entity.GetMoveSpeedRateWhileAttacking(weaponItem);
            MovementRestrictionWhileAttacking = Entity.GetMovementRestrictionWhileAttacking(weaponItem);

            // Get play speed multiplier will use it to play animation faster or slower based on attack speed stats
            animSpeedRate *= Entity.GetAnimSpeedRate(AnimActionType);

            if (IsServer)
            {
                // Do something with buffs when attack
                Entity.SkillAndBuffComponent.OnAttack();
            }

            // Prepare cancellation
            CancellationTokenSource attackCancellationTokenSource = new CancellationTokenSource();
            _attackCancellationTokenSources.Add(attackCancellationTokenSource);

            try
            {
                BaseCharacterModel tpsModel = Entity.ActionModel;
                bool tpsModelAvailable = tpsModel != null && tpsModel.gameObject.activeSelf;
                BaseCharacterModel vehicleModel = Entity.PassengingVehicleModel as BaseCharacterModel;
                bool vehicleModelAvailable = vehicleModel != null;
                bool overridePassengerActionAnimations = vehicleModelAvailable && Entity.PassengingVehicleSeat.overridePassengerActionAnimations;
                BaseCharacterModel fpsModel = Entity.FpsModel;
                bool fpsModelAvailable = IsClient && fpsModel != null && fpsModel.gameObject.activeSelf;

                // Play animation
                if (weaponItem.DoRecoilingAsAttackAnimation)
                {
                    if (vehicleModelAvailable)
                        vehicleModel.CacheAttackRecoiler?.PlayRecoiling();
                    if (!overridePassengerActionAnimations)
                    {
                        if (tpsModelAvailable)
                            tpsModel.CacheAttackRecoiler?.PlayRecoiling();
                        if (fpsModelAvailable)
                            fpsModel.CacheAttackRecoiler?.PlayRecoiling();
                    }
                }
                else
                {
                    if (vehicleModelAvailable)
                        vehicleModel.PlayActionAnimation(AnimActionType, AnimActionDataId, animationIndex, out _skipMovementValidation, out _shouldUseRootMotion, animSpeedRate, 0f, 0f);
                    if (!overridePassengerActionAnimations)
                    {
                        if (tpsModelAvailable)
                            tpsModel.PlayActionAnimation(AnimActionType, AnimActionDataId, animationIndex, out _skipMovementValidation, out _shouldUseRootMotion, animSpeedRate, 0f, 0f);
                        if (fpsModelAvailable)
                            fpsModel.PlayActionAnimation(AnimActionType, AnimActionDataId, animationIndex, out _, out _, animSpeedRate, 0f, 0f);
                    }
                }

                // Prepare action durations
                float remainsDuration = totalDuration;
                if (weaponItem.RateOfFire > 0)
                {
                    totalDuration = RATE_OF_FIRE_BASE / (weaponItem.RateOfFire + entityCaches.RateOfFire);
                    remainsDuration = BaseCharacterModel.GetAnimationDuration(totalDuration, animSpeedRate, 0f);
                    triggerDurations = new float[] { 0f };
                    LastAttackEndTime = Time.unscaledTime + remainsDuration;
                }
                else if (weaponItem.DoRecoilingAsAttackAnimation)
                {
                    totalDuration = tpsModel.CacheAttackRecoiler?.DefaultRecoilDuration ?? 1f;
                    remainsDuration = BaseCharacterModel.GetAnimationDuration(totalDuration, animSpeedRate, 0f);
                    triggerDurations = new float[] { 0f };
                    LastAttackEndTime = Time.unscaledTime + remainsDuration;
                }
                else
                {
                    remainsDuration = BaseCharacterModel.GetAnimationDuration(totalDuration, animSpeedRate, 0f);
                    LastAttackEndTime = Time.unscaledTime + remainsDuration;
                    await _manager.PrepareActionDurations(triggerDurations, totalDuration, animSpeedRate, 0f, attackCancellationTokenSource.Token,
                        (__triggerDurations, __totalDuration, __remainsDuration, __endTime) =>
                        {
                            triggerDurations = __triggerDurations;
                            totalDuration = __totalDuration;
                            remainsDuration = __remainsDuration;
                            LastAttackEndTime = __endTime;
                        });
                }

                // Prepare damage amounts
                List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts = Entity.PrepareDamageAmounts(isLeftHand, baseDamageAmounts, triggerDurations.Length, 1);

                // Prepare hit register validation, it will be used later when receive attack start/end events from clients
                if ((IsServer && !IsOwnerClient) || !IsOwnedByServer)
                    HitRegistrationManager.PrepareHitRegValidation(Entity, simulateSeed, triggerDurations, weaponItem.FireSpreadAmount, damageInfo, damageAmounts, weaponHandlingState, weapon, null, 0);

                // Attack starts
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogAttackStart(_playerCharacterEntity, simulateSeed, triggerDurations, weaponItem.FireSpreadAmount, isLeftHand, weapon);
                OnAttackStart?.Invoke();

                for (byte triggerIndex = 0; triggerIndex < triggerDurations.Length; ++triggerIndex)
                {
                    // Wait until triggger before play special effects
                    float tempTriggerDuration = triggerDurations[triggerIndex] / animSpeedRate;
                    remainsDuration -= tempTriggerDuration;
                    await GenericUtils.FrameBasedDelay(tempTriggerDuration, attackCancellationTokenSource.Token);

                    // Special effects will plays on clients only
                    if (IsClient)
                    {
                        // Play weapon launch special effects
                        if (tpsModelAvailable)
                            tpsModel.PlayEquippedWeaponLaunch(isLeftHand);
                        if (fpsModelAvailable)
                            fpsModel.PlayEquippedWeaponLaunch(isLeftHand);
                        // Play launch sfx
                        AudioClipWithVolumeSettings launchClip = weaponItem.LaunchClip;
                        if (entityCaches.TryGetWeaponAbility(isLeftHand, LaunchSfxWeaponAbility.KEY, out BaseWeaponAbility ability) && ability is LaunchSfxWeaponAbility launchSfxAbility)
                            launchClip = launchSfxAbility.LaunchClip;
                        launchClip?.Play(tpsModel.GenericAudioSource);
                    }

                    // Get aim position by character's forward
                    AimPosition aimPosition = Entity.AimPosition;

                    // Trigger attack event
                    OnAttackTrigger?.Invoke(triggerIndex);

                    // Call on attack to extend attack functionality while attacking
                    bool overrideDefaultAttack = false;
                    foreach (KeyValuePair<BaseSkill, int> skillLevel in entityCaches.Skills)
                    {
                        if (skillLevel.Value <= 0)
                            continue;
                        if (skillLevel.Key.OnAttack(Entity, skillLevel.Value, isLeftHand, weapon, simulateSeed, triggerIndex, damageAmounts, aimPosition))
                            overrideDefaultAttack = true;
                    }

                    // Skip attack function when applied skills (buffs) will override default attack functionality
                    if (!overrideDefaultAttack)
                    {
                        // Trigger attack event
                        Entity.OnAttackRoutine(isLeftHand, weapon, simulateSeed, triggerIndex, damageInfo, damageAmounts, aimPosition);

                        // Apply attack damages
                        if (IsServer)
                        {
                            if (!Entity.DecreaseAmmos(isLeftHand, 1, out _))
                                continue;
                            if (!IsOwnerClient && !IsOwnedByServer)
                                continue;
                            if (!isLeftHand)
                                Entity.RightWeaponAmmoSim -= 1;
                            else
                                Entity.LeftWeaponAmmoSim -= 1;
                            RPC(RpcSimulateActionTrigger, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, new SimulateActionTriggerData()
                            {
                                simulateSeed = simulateSeed,
                                triggerIndex = triggerIndex,
                                aimPosition = aimPosition,
                            });
                            ApplyAttack(entityCaches, weaponHandlingState, weapon, simulateSeed, triggerIndex, damageInfo, damageAmounts, aimPosition);
                        }
                        else if (IsOwnerClient)
                        {
                            if (!isLeftHand)
                                Entity.RightWeaponAmmoSim -= 1;
                            else
                                Entity.LeftWeaponAmmoSim -= 1;
                            RPC(CmdSimulateActionTrigger, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, new SimulateActionTriggerData()
                            {
                                simulateSeed = simulateSeed,
                                triggerIndex = triggerIndex,
                                aimPosition = aimPosition,
                            });
                            ApplyAttack(entityCaches, weaponHandlingState, weapon, simulateSeed, triggerIndex, damageInfo, damageAmounts, aimPosition);
                        }
                    }

                    if (remainsDuration <= 0f)
                    {
                        // Stop trigger animations loop
                        break;
                    }
                }

                if (IsServer && weaponItem.DestroyImmediatelyAfterFired)
                {
                    EquipWeapons equipWeapons = Entity.EquipWeapons;
                    if (isLeftHand)
                        equipWeapons.leftHand = CharacterItem.Empty;
                    else
                        equipWeapons.rightHand = CharacterItem.Empty;
                    Entity.EquipWeapons = equipWeapons;
                }

                if (remainsDuration > 0f)
                {
                    // Wait until animation ends to stop actions
                    await GenericUtils.FrameBasedDelay(remainsDuration, attackCancellationTokenSource.Token);
                }
            }
            catch (System.OperationCanceledException)
            {
                // Catch the cancellation
                OnAttackInterupted?.Invoke();
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogAttackInterrupt(_playerCharacterEntity, simulateSeed);
            }
            catch (System.Exception ex)
            {
                // Other errors
                Debug.LogError("Error occuring in `DefaultCharacterAttackComponent` -> `AttackRoutine`, " + this);
                Debug.LogException(ex);
            }
            finally
            {
                LastAttackEndTime = Time.unscaledTime;
                attackCancellationTokenSource.Dispose();
                _attackCancellationTokenSources.Remove(attackCancellationTokenSource);
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogAttackEnd(_playerCharacterEntity, simulateSeed);
                OnAttackEnd?.Invoke();
            }
            // Clear action states at clients and server
            ClearAttackStates();
        }

        [ServerRpc]
        protected void CmdSimulateActionTrigger(SimulateActionTriggerData data)
        {
            HitValidateData validateData = HitRegistrationManager.GetHitValidateData(Entity, data.simulateSeed);
            if (validateData == null)
            {
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogAttackTriggerFail(_playerCharacterEntity, data.simulateSeed, data.triggerIndex, ActionTriggerFailReasons.NoValidateData);
                return;
            }
            if (data.triggerIndex >= validateData.DamageAmounts.Count)
            {
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogAttackTriggerFail(_playerCharacterEntity, data.simulateSeed, data.triggerIndex, ActionTriggerFailReasons.NotEnoughResources);
                return;
            }
            RPC(RpcSimulateActionTrigger, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, data);
            ApplyAttack(Entity.GetCaches(), validateData.WeaponHandlingState, validateData.Weapon, data.simulateSeed, data.triggerIndex, validateData.DamageInfo, validateData.DamageAmounts, data.aimPosition);
            if (_entityIsPlayer && IsServer)
                GameInstance.ServerLogHandlers.LogAttackTrigger(_playerCharacterEntity, data.simulateSeed, data.triggerIndex);
        }

        [AllRpc]
        protected void RpcSimulateActionTrigger(SimulateActionTriggerData data)
        {
            if (IsServer)
                return;
            if (IsOwnerClientOrOwnedByServer)
                return;
            HitValidateData validateData = HitRegistrationManager.GetHitValidateData(Entity, data.simulateSeed);
            if (validateData == null)
                return;
            ApplyAttack(Entity.GetCaches(), validateData.WeaponHandlingState, validateData.Weapon, data.simulateSeed, data.triggerIndex, validateData.DamageInfo, validateData.DamageAmounts, data.aimPosition);
        }

        protected virtual async void ApplyAttack(CharacterDataCache entityCaches, WeaponHandlingState weaponHandlingState, CharacterItem weapon, int simulateSeed, byte triggerIndex, DamageInfo damageInfo, List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts, AimPosition aimPosition)
        {
            if (triggerIndex >= damageAmounts.Count)
            {
                // No damage applied (may not have enough ammo)
                return;
            }

            bool isLeftHand = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
            byte fireSpreadAmount = 0;
            Vector3 fireSpreadRange = Vector3.zero;
            IWeaponItem weaponItem = weapon.GetWeaponItem();
            if (weaponItem != null)
            {
                // For monsters, their weapon can be null so have to avoid null exception
                fireSpreadAmount = weaponItem.FireSpreadAmount;
                fireSpreadRange = weaponItem.FireSpreadRange;
                if (weaponHandlingState.Has(WeaponHandlingState.IsAiming) && weaponItem.FireSpreadRangeWhileAiming.x > float.MinValue)
                {
                    fireSpreadRange = weaponItem.FireSpreadRangeWhileAiming;
                }
                else if (weaponHandlingState.Has(WeaponHandlingState.IsShoulderView) && weaponItem.FireSpreadRangeWhileShoulderViewMode.x > float.MinValue)
                {
                    fireSpreadRange = weaponItem.FireSpreadRangeWhileShoulderViewMode;
                }
                else if (weaponHandlingState.Has(WeaponHandlingState.IsFpsView) && weaponItem.FireSpreadRangeWhileFpsViewMode.x > float.MinValue)
                {
                    fireSpreadRange = weaponItem.FireSpreadRangeWhileFpsViewMode;
                }
            }

            fireSpreadAmount += (byte)Mathf.CeilToInt(entityCaches.FireSpread);
            fireSpreadRange *= 1 + entityCaches.FireSpreadRangeRate;

            // Make sure it won't increase damage to the wrong collction
            for (byte spreadIndex = 0; spreadIndex < fireSpreadAmount + 1; ++spreadIndex)
            {
                await damageInfo.LaunchDamageEntity(
                    Entity,
                    isLeftHand,
                    weapon,
                    simulateSeed,
                    triggerIndex,
                    spreadIndex,
                    fireSpreadRange,
                    damageAmounts,
                    null,
                    0,
                    aimPosition);
            }
        }

        public virtual void CancelAttack()
        {
            for (int i = _attackCancellationTokenSources.Count - 1; i >= 0; --i)
            {
                if (!_attackCancellationTokenSources[i].IsCancellationRequested)
                    _attackCancellationTokenSources[i].Cancel();
                _attackCancellationTokenSources.RemoveAt(i);
            }
        }

        public virtual void Attack(WeaponHandlingState weaponHandlingState)
        {
            long timestamp = Manager.ServerTimestamp;
            if (!IsServer && IsOwnerClient)
            {
                RPC(CmdAttack, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, timestamp, weaponHandlingState);
                ProceedAttack(timestamp, weaponHandlingState);
            }
            else if (IsOwnerClientOrOwnedByServer)
            {
                PreceedCmdAttack(timestamp, weaponHandlingState);
            }
        }

        [ServerRpc]
        protected void CmdAttack(long peerTimestamp, WeaponHandlingState weaponHandlingState)
        {
            PreceedCmdAttack(peerTimestamp, weaponHandlingState);
        }

        protected void PreceedCmdAttack(long peerTimestamp, WeaponHandlingState weaponHandlingState)
        {
            RPC(RpcAttack, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, peerTimestamp, weaponHandlingState);
            ProceedAttack(peerTimestamp, weaponHandlingState);
        }

        [AllRpc]
        protected void RpcAttack(long peerTimestamp, WeaponHandlingState weaponHandlingState)
        {
            if (IsServer || IsOwnerClient)
            {
                // Don't play attacking animation again
                return;
            }
            ProceedAttack(peerTimestamp, weaponHandlingState);
        }

        protected void ProceedAttack(long peerTimestamp, WeaponHandlingState weaponHandlingState)
        {
            AttackState simulateState = new AttackState()
            {
                SimulateSeed = GetSimulateSeed(peerTimestamp),
                WeaponHandlingState = weaponHandlingState,
            };
            AttackRoutine(peerTimestamp, simulateState).Forget();
        }

        private int GetSimulateSeed(long timestamp)
        {
            return (int)(timestamp % 16384);
        }
    }
}
