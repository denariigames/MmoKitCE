using Cysharp.Threading.Tasks;
using Insthync.ManagedUpdating;
using LiteNetLib;
using LiteNetLibManager;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(CharacterActionComponentManager))]
    public class DefaultCharacterUseSkillComponent : BaseNetworkedGameEntityComponent<BaseCharacterEntity>, ICharacterUseSkillComponent, IManagedUpdate
    {
        protected struct UseSkillState
        {
            public int SimulateSeed;
            public bool IsInterrupted;
            public bool UseItem;
            public int ItemIndex;
            public int? ItemDataId;
            public BaseSkill Skill;
            public int SkillLevel;
            public WeaponHandlingState WeaponHandlingState;
            public uint TargetObjectId;
            public AimPosition AimPosition;
        }

        protected readonly List<CancellationTokenSource> _skillCancellationTokenSources = new List<CancellationTokenSource>();
        public BaseSkill UsingSkill
        {
            get
            {
                if (!IsUsingSkill)
                    return null;
                return _simulateState.Value.Skill;
            }
        }
        public int UsingSkillLevel
        {
            get
            {
                if (!IsUsingSkill)
                    return 0;
                return _simulateState.Value.SkillLevel;
            }
        }
        public bool IsUsingSkill
        {
            get
            {
                return _simulateState.HasValue;
            }
        }
        public float LastUseSkillEndTime { get; protected set; }
        protected bool _skipMovementValidation;
        public bool IsSkipMovementValidationWhileUsingSkill { get { return _skipMovementValidation; } protected set { _skipMovementValidation = value; } }
        protected bool _shouldUseRootMotion;
        public bool IsUseRootMotionWhileUsingSkill { get { return _shouldUseRootMotion; } protected set { _shouldUseRootMotion = value; } }
        public bool IsCastingSkillCanBeInterrupted { get; protected set; }
        public bool IsCastingSkillInterrupted { get; protected set; }
        public float CastingSkillDuration { get; protected set; }
        public float CastingSkillCountDown { get; protected set; }
        public float MoveSpeedRateWhileUsingSkill { get; protected set; }
        public MovementRestriction MovementRestrictionWhileUsingSkill { get; protected set; }
        public System.Action OnCastSkillStart { get; set; }
        public System.Action OnCastSkillEnd { get; set; }
        public System.Action OnUseSkillStart { get; set; }
        public System.Action<int> OnUseSkillTrigger { get; set; }
        public System.Action OnUseSkillEnd { get; set; }
        public System.Action OnSkillInterupted { get; set; }
        public AnimActionType AnimActionType { get; protected set; }
        public int AnimActionDataId { get; protected set; }
        public IHitRegistrationManager HitRegistrationManager { get { return BaseGameNetworkManager.Singleton.HitRegistrationManager; } }

        protected CharacterActionComponentManager _manager;
        // Network data sending
        protected UseSkillState? _simulateState;
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
            CancelSkill();
            ClearUseSkillStates();
            _manager = null;
            _entityIsPlayer = false;
            _playerCharacterEntity = null;
        }

        private void OnEnable()
        {
            UpdateManager.Register(this);
        }

        private void OnDisable()
        {
            UpdateManager.Unregister(this);
        }

        public virtual void ManagedUpdate()
        {
            // Update casting skill count down, will show gage at clients
            if (CastingSkillCountDown > 0)
                CastingSkillCountDown -= Time.unscaledDeltaTime;
        }

        protected virtual void SetUseSkillActionStates(AnimActionType animActionType, int animActionDataId, UseSkillState simulateState)
        {
            ClearUseSkillStates();
            AnimActionType = animActionType;
            AnimActionDataId = animActionDataId;
            _simulateState = simulateState;
        }

        public virtual void ClearUseSkillStates()
        {
            _simulateState = null;
        }

        protected virtual async UniTaskVoid UseSkillRoutine(long peerTimestamp, UseSkillState simulateState)
        {
            int simulateSeed = GetSimulateSeed(peerTimestamp);
            WeaponHandlingState weaponHandlingState = simulateState.WeaponHandlingState;
            bool isLeftHand = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
            BaseSkill skill = simulateState.Skill;
            int skillLevel = simulateState.SkillLevel;
            uint targetObjectId = simulateState.TargetObjectId;
            AimPosition skillAimPosition = simulateState.AimPosition;
            int? itemDataId = simulateState.ItemDataId;
            if (simulateState.SimulateSeed == 0)
                simulateState.SimulateSeed = simulateSeed;
            else
                simulateSeed = simulateState.SimulateSeed;

            // Prepare required data and get skill data
            Entity.GetUsingSkillData(
                skill,
                ref isLeftHand,
                out AnimActionType animActionType,
                out int animActionDataId,
                out CharacterItem weapon,
                out DamageInfo damageInfo);

            // Prepare required data and get animation data
            Entity.GetRandomAnimationData(
                animActionType,
                animActionDataId,
                simulateSeed,
                out int animationIndex,
                out float animSpeedRate,
                out float[] triggerDurations,
                out float totalDuration);

            // Set doing action state at clients and server
            SetUseSkillActionStates(animActionType, animActionDataId, simulateState);

            if (IsServer)
            {
                // Update skill usage states at server only
                if (itemDataId.HasValue)
                {
                    Entity.AddOrUpdateSkillUsage(SkillUsageType.UsableItem, itemDataId.Value, skillLevel);
                }
                else
                {
                    Entity.AddOrUpdateSkillUsage(SkillUsageType.Skill, skill.DataId, skillLevel);
                }
                // Do something with buffs when use skill
                Entity.SkillAndBuffComponent.OnUseSkill();
            }

            // Prepare required data and get damages data
            IWeaponItem weaponItem = weapon.GetWeaponItem();
            Dictionary<DamageElement, MinMaxFloat> baseDamageAmounts = skill.GetAttackDamages(Entity, skillLevel, isLeftHand);

            // Calculate move speed rate while doing action at clients and server
            MoveSpeedRateWhileUsingSkill = skill.moveSpeedRateWhileUsingSkill;
            MovementRestrictionWhileUsingSkill = skill.movementRestrictionWhileUsingSkill;

            // Get play speed multiplier will use it to play animation faster or slower based on attack speed stats
            animSpeedRate *= Entity.GetAnimSpeedRate(AnimActionType);

            // Set doing action data
            IsCastingSkillCanBeInterrupted = skill.canBeInterruptedWhileCasting;
            IsCastingSkillInterrupted = false;

            // Get cast duration. Then if cast duration more than 0, it will play cast skill animation.
            CastingSkillDuration = CastingSkillCountDown = skill.GetCastDuration(skillLevel);

            // Prepare cancellation
            CancellationTokenSource skillCancellationTokenSource = new CancellationTokenSource();
            _skillCancellationTokenSources.Add(skillCancellationTokenSource);

            try
            {
                BaseCharacterModel tpsModel = Entity.ActionModel;
                bool tpsModelAvailable = tpsModel != null && tpsModel.gameObject.activeSelf;
                BaseCharacterModel vehicleModel = Entity.PassengingVehicleModel as BaseCharacterModel;
                bool vehicleModelAvailable = vehicleModel != null;
                bool overridePassengerActionAnimations = vehicleModelAvailable && Entity.PassengingVehicleSeat.overridePassengerActionAnimations;
                BaseCharacterModel fpsModel = Entity.FpsModel;
                bool fpsModelAvailable = IsClient && fpsModel != null && fpsModel.gameObject.activeSelf;

                // Prepare damage amounts
                List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts = skill.PrepareDamageAmounts(Entity, isLeftHand, baseDamageAmounts, triggerDurations.Length);

                // Prepare hit register validation, it will be used later when receive attack start/end events from clients
                if ((IsServer && !IsOwnerClient) || !IsOwnedByServer)
                    HitRegistrationManager.PrepareHitRegValidation(Entity, simulateSeed, triggerDurations, 0, damageInfo, damageAmounts, weaponHandlingState, weapon, skill, skillLevel);

                // Play special effect
                if (IsClient)
                {
                    if (!overridePassengerActionAnimations)
                    {
                        if (tpsModelAvailable)
                        {
                            tpsModel.InstantiateEffect(skill.SkillCastEffects);
#if !DISABLE_ADDRESSABLES
                            tpsModel.InstantiateEffect(skill.AddressableSkillCastEffects).Forget();
#endif
                        }
                        if (fpsModelAvailable)
                        {
                            fpsModel.InstantiateEffect(skill.SkillCastEffects);
#if !DISABLE_ADDRESSABLES
                            fpsModel.InstantiateEffect(skill.AddressableSkillCastEffects).Forget();
#endif
                        }
                    }
                    else if (vehicleModelAvailable)
                    {
                        vehicleModel.InstantiateEffect(skill.SkillCastEffects);
#if !DISABLE_ADDRESSABLES
                        vehicleModel.InstantiateEffect(skill.AddressableSkillCastEffects).Forget();
#endif
                    }
                }

                // Play cast animation
                if (CastingSkillDuration > 0f)
                {
                    LastUseSkillEndTime = Time.unscaledTime + CastingSkillDuration + 1f;
                    if (vehicleModelAvailable)
                        vehicleModel.PlaySkillCastClip(skill.DataId, CastingSkillDuration, out _skipMovementValidation, out _shouldUseRootMotion);
                    if (!overridePassengerActionAnimations)
                    {
                        if (tpsModelAvailable)
                            tpsModel.PlaySkillCastClip(skill.DataId, CastingSkillDuration, out _skipMovementValidation, out _shouldUseRootMotion);
                        if (fpsModelAvailable)
                            fpsModel.PlaySkillCastClip(skill.DataId, CastingSkillDuration, out _, out _);
                    }
                    // Wait until end of cast duration
                    OnCastSkillStart?.Invoke();
                    await GenericUtils.FrameBasedDelay(CastingSkillDuration, skillCancellationTokenSource.Token);
                    OnCastSkillEnd?.Invoke();
                }

                // Play special effect
                if (IsClient)
                {
                    if (vehicleModelAvailable)
                    {
                        vehicleModel.InstantiateEffect(skill.SkillActivateEffects);
#if !DISABLE_ADDRESSABLES
                        vehicleModel.InstantiateEffect(skill.AddressableSkillActivateEffects).Forget();
#endif
                    }
                    if (!overridePassengerActionAnimations)
                    {
                        if (tpsModelAvailable)
                        {
                            tpsModel.InstantiateEffect(skill.SkillActivateEffects);
#if !DISABLE_ADDRESSABLES
                            tpsModel.InstantiateEffect(skill.AddressableSkillActivateEffects).Forget();
#endif
                        }
                        if (fpsModelAvailable)
                        {
                            fpsModel.InstantiateEffect(skill.SkillActivateEffects);
#if !DISABLE_ADDRESSABLES
                            fpsModel.InstantiateEffect(skill.AddressableSkillActivateEffects).Forget();
#endif
                        }
                    }
                }

                // Play action animation
                if (vehicleModelAvailable)
                    vehicleModel.PlayActionAnimation(AnimActionType, AnimActionDataId, animationIndex, out _skipMovementValidation, out _shouldUseRootMotion, animSpeedRate, 0f, 0f);
                if (!overridePassengerActionAnimations)
                {
                    if (tpsModelAvailable)
                        tpsModel.PlayActionAnimation(AnimActionType, AnimActionDataId, animationIndex, out _skipMovementValidation, out _shouldUseRootMotion, animSpeedRate, 0f, 0f);
                    if (fpsModelAvailable)
                        fpsModel.PlayActionAnimation(AnimActionType, AnimActionDataId, animationIndex, out _, out _, animSpeedRate, 0f, 0f);
                }

                // Prepare action durations
                float remainsDuration = BaseCharacterModel.GetAnimationDuration(totalDuration, animSpeedRate, 0f);
                LastUseSkillEndTime = Time.unscaledTime + remainsDuration;
                await _manager.PrepareActionDurations(triggerDurations, totalDuration, animSpeedRate, 0f, skillCancellationTokenSource.Token,
                    (__triggerDurations, __totalDuration, __remainsDuration, __endTime) =>
                    {
                        triggerDurations = __triggerDurations;
                        totalDuration = __totalDuration;
                        remainsDuration = __remainsDuration;
                        LastUseSkillEndTime = __endTime + CastingSkillDuration;
                    });

                // Use skill starts
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogUseSkillStart(_playerCharacterEntity, simulateSeed, triggerDurations, weaponItem.FireSpreadAmount, isLeftHand, weapon, skill, skillLevel);
                OnUseSkillStart?.Invoke();

                for (byte triggerIndex = 0; triggerIndex < triggerDurations.Length; ++triggerIndex)
                {
                    // Play special effects after trigger duration
                    float tempTriggerDuration = triggerDurations[triggerIndex] / animSpeedRate;
                    remainsDuration -= tempTriggerDuration;
                    await GenericUtils.FrameBasedDelay(tempTriggerDuration, skillCancellationTokenSource.Token);

                    // Special effects will plays on clients only
                    if (IsClient && (AnimActionType == AnimActionType.AttackRightHand || AnimActionType == AnimActionType.AttackLeftHand))
                    {
                        // Play weapon launch special effects
                        if (tpsModelAvailable)
                            tpsModel.PlayEquippedWeaponLaunch(isLeftHand);
                        if (fpsModelAvailable)
                            fpsModel.PlayEquippedWeaponLaunch(isLeftHand);
                        // Play launch sfx
                        weaponItem.LaunchClip?.Play(tpsModel.GenericAudioSource);
                    }

                    // Get aim position by character's forward
                    AimPosition aimPosition;
                    if (skill.HasCustomAimControls() && skillAimPosition.type == AimPositionType.Position)
                        aimPosition = skillAimPosition;
                    else
                        aimPosition = Entity.AimPosition;

                    // Trigger skill event
                    OnUseSkillTrigger?.Invoke(triggerIndex);
                    Entity.OnUseSkillRoutine(skill, skillLevel, isLeftHand, weapon, simulateSeed, triggerIndex, damageAmounts, targetObjectId, aimPosition);

                    // Apply skill buffs, summons and attack damages
                    if (IsServer)
                    {
                        if (!skill.DecreaseResources(Entity, weapon, isLeftHand, out _))
                            continue;
                        if (!IsOwnerClient && !IsOwnedByServer)
                            continue;
                        RPC(RpcSimulateActionTrigger, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, new SimulateActionTriggerData()
                        {
                            simulateSeed = simulateSeed,
                            triggerIndex = triggerIndex,
                            targetObjectId = targetObjectId,
                            aimPosition = aimPosition,
                        });
                        ApplySkillUsing(skill, skillLevel, weaponHandlingState, weapon, simulateSeed, triggerIndex, damageAmounts, targetObjectId, aimPosition);
                    }
                    else if (IsOwnerClient)
                    {
                        RPC(CmdSimulateActionTrigger, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, new SimulateActionTriggerData()
                        {
                            simulateSeed = simulateSeed,
                            triggerIndex = triggerIndex,
                            targetObjectId = targetObjectId,
                            aimPosition = aimPosition,
                        });
                        ApplySkillUsing(skill, skillLevel, weaponHandlingState, weapon, simulateSeed, triggerIndex, damageAmounts, targetObjectId, aimPosition);
                    }

                    if (remainsDuration <= 0f)
                    {
                        // Stop trigger animations loop
                        break;
                    }
                }

                // Decrease items
                if (IsServer && itemDataId.HasValue && Entity.DecreaseItems(itemDataId.Value, 1))
                    Entity.FillEmptySlots();

                if (remainsDuration > 0f)
                {
                    // Wait until animation ends to stop actions
                    await GenericUtils.FrameBasedDelay(remainsDuration, skillCancellationTokenSource.Token);
                }
            }
            catch (System.OperationCanceledException)
            {
                // Catch the cancellation
                OnSkillInterupted?.Invoke();
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogUseSkillInterrupt(_playerCharacterEntity, simulateSeed);
            }
            catch (System.Exception ex)
            {
                // Other errors
                Debug.LogError("Error occuring in `DefaultCharacterUseSkillComponent` -> `UseSkillRoutine`, " + this);
                Debug.LogException(ex);
            }
            finally
            {
                LastUseSkillEndTime = Time.unscaledTime;
                skillCancellationTokenSource.Dispose();
                _skillCancellationTokenSources.Remove(skillCancellationTokenSource);
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogUseSkillEnd(_playerCharacterEntity, simulateSeed);
                OnUseSkillEnd?.Invoke();
            }
            // Clear action states at clients and server
            ClearUseSkillStates();
        }

        [ServerRpc]
        protected void CmdSimulateActionTrigger(SimulateActionTriggerData data)
        {
            HitValidateData validateData = HitRegistrationManager.GetHitValidateData(Entity, data.simulateSeed);
            if (validateData == null || validateData.Skill == null)
            {
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogUseSkillTriggerFail(_playerCharacterEntity, data.simulateSeed, data.triggerIndex, ActionTriggerFailReasons.NoValidateData);
                return;
            }
            if (data.triggerIndex >= validateData.DamageAmounts.Count)
            {
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogUseSkillTriggerFail(_playerCharacterEntity, data.simulateSeed, data.triggerIndex, ActionTriggerFailReasons.NotEnoughResources);
                return;
            }
            RPC(RpcSimulateActionTrigger, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, data);
            ApplySkillUsing(validateData.Skill, validateData.SkillLevel, validateData.WeaponHandlingState, validateData.Weapon, data.simulateSeed, data.triggerIndex, validateData.DamageAmounts, data.targetObjectId, data.aimPosition);
            if (_entityIsPlayer && IsServer)
                GameInstance.ServerLogHandlers.LogUseSkillTrigger(_playerCharacterEntity, data.simulateSeed, data.triggerIndex);
        }

        [AllRpc]
        protected void RpcSimulateActionTrigger(SimulateActionTriggerData data)
        {
            if (IsServer)
                return;
            if (IsOwnerClientOrOwnedByServer)
                return;
            HitValidateData validateData = HitRegistrationManager.GetHitValidateData(Entity, data.simulateSeed);
            if (validateData == null || validateData.Skill == null)
                return;
            ApplySkillUsing(validateData.Skill, validateData.SkillLevel, validateData.WeaponHandlingState, validateData.Weapon, data.simulateSeed, data.triggerIndex, validateData.DamageAmounts, data.targetObjectId, data.aimPosition);
        }

        protected virtual void ApplySkillUsing(BaseSkill skill, int skillLevel, WeaponHandlingState weaponHandlingState, CharacterItem weapon, int simulateSeed, byte triggerIndex, List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts, uint targetObjectId, AimPosition aimPosition)
        {
            if (triggerIndex >= damageAmounts.Count)
            {
                // No damage applied (may not have enough ammo)
                return;
            }

            skill.ApplySkill(
                Entity,
                skillLevel,
                weaponHandlingState,
                weapon,
                simulateSeed,
                triggerIndex,
                damageAmounts,
                targetObjectId,
                aimPosition);
        }

        public virtual void UseSkill(int dataId, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            bool isLeftHand = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
            long timestamp = Manager.ServerTimestamp;
            if (!IsServer && IsOwnerClient)
            {
                if (!Entity.ValidateSkillToUse(dataId, isLeftHand, targetObjectId, out BaseSkill skill, out int skillLevel, out UITextKeys gameMessage))
                {
                    ClientGenericActions.ClientReceiveGameMessage(gameMessage);
                    return;
                }
                RPC(CmdUseSkill, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, timestamp, dataId, weaponHandlingState, targetObjectId, aimPosition);
                ProceedUseSkill(timestamp, skill, skillLevel, weaponHandlingState, targetObjectId, aimPosition);
            }
            else if (IsOwnerClientOrOwnedByServer)
            {
                ProceedCmdUseSkill(timestamp, dataId, weaponHandlingState, targetObjectId, aimPosition);
            }
        }

        [ServerRpc]
        protected void CmdUseSkill(long peerTimestamp, int dataId, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            ProceedCmdUseSkill(peerTimestamp, dataId, weaponHandlingState, targetObjectId, aimPosition);
        }

        protected void ProceedCmdUseSkill(long peerTimestamp, int dataId, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            if (!Entity.ValidateSkillToUse(dataId, weaponHandlingState.Has(WeaponHandlingState.IsLeftHand), targetObjectId, out BaseSkill skill, out int skillLevel, out _))
                return;
            RPC(RpcUseSkill, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, peerTimestamp, dataId, skillLevel, weaponHandlingState, targetObjectId, aimPosition);
            ProceedUseSkill(peerTimestamp, skill, skillLevel, weaponHandlingState, targetObjectId, aimPosition);
        }

        [AllRpc]
        protected void RpcUseSkill(long peerTimestamp, int dataId, int skillLevel, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            if (IsServer || IsOwnerClient)
            {
                // Don't play attacking animation again
                return;
            }
            if (!GameInstance.Skills.TryGetValue(dataId, out BaseSkill skill))
                return;
            ProceedUseSkill(peerTimestamp, skill, skillLevel, weaponHandlingState, targetObjectId, aimPosition);
        }

        protected void ProceedUseSkill(long peerTimestamp, BaseSkill skill, int skillLevel, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            UseSkillState simulateState = new UseSkillState()
            {
                SimulateSeed = GetSimulateSeed(peerTimestamp),
                Skill = skill,
                SkillLevel = skillLevel,
                WeaponHandlingState = weaponHandlingState,
                TargetObjectId = targetObjectId,
                AimPosition = aimPosition,
            };
            UseSkillRoutine(peerTimestamp, simulateState).Forget();
        }

        public virtual void UseSkillItem(int itemIndex, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            bool isLeftHand = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
            long timestamp = Manager.ServerTimestamp;
            if (!IsServer && IsOwnerClient)
            {
                if (!Entity.ValidateSkillItemToUse(itemIndex, isLeftHand, targetObjectId, out ISkillItem skillItem, out BaseSkill skill, out int skillLevel, out UITextKeys gameMessage))
                {
                    ClientGenericActions.ClientReceiveGameMessage(gameMessage);
                    return;
                }
                Entity.LastUseItemTime = Time.unscaledTime;
                RPC(CmdUseSkillItem, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, timestamp, itemIndex, weaponHandlingState, targetObjectId, aimPosition);
                ProceedUseSkillItem(timestamp, skillItem, skill, skillLevel, weaponHandlingState, targetObjectId, aimPosition);
            }
            else if (IsOwnerClientOrOwnedByServer)
            {
                ProceedCmdUseSkillItem(timestamp, itemIndex, weaponHandlingState, targetObjectId, aimPosition);
            }
        }

        [ServerRpc]
        protected void CmdUseSkillItem(long peerTimestamp, int itemIndex, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            ProceedCmdUseSkillItem(peerTimestamp, itemIndex, weaponHandlingState, targetObjectId, aimPosition);
        }

        protected void ProceedCmdUseSkillItem(long peerTimestamp, int itemIndex, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            if (!Entity.ValidateSkillItemToUse(itemIndex, weaponHandlingState.Has(WeaponHandlingState.IsLeftHand), targetObjectId, out ISkillItem skillItem, out BaseSkill skill, out int skillLevel, out _))
                return;
            RPC(RpcUseSkillItem, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, peerTimestamp, skillItem.DataId, weaponHandlingState, targetObjectId, aimPosition);
            ProceedUseSkillItem(peerTimestamp, skillItem, skill, skillLevel, weaponHandlingState, targetObjectId, aimPosition);
        }

        [AllRpc]
        protected void RpcUseSkillItem(long peerTimestamp, int itemDataId, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            if (IsServer || IsOwnerClient)
            {
                // Don't play attacking animation again
                return;
            }
            if (!GameInstance.Items.TryGetValue(itemDataId, out BaseItem item) || !(item is ISkillItem skillItem))
                return;
            ProceedUseSkillItem(peerTimestamp, skillItem, skillItem.SkillData, skillItem.SkillLevel, weaponHandlingState, targetObjectId, aimPosition);
        }

        protected void ProceedUseSkillItem(long peerTimestamp, ISkillItem skillItem, BaseSkill skill, int skillLevel, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition)
        {
            UseSkillState simulateState = new UseSkillState()
            {
                SimulateSeed = GetSimulateSeed(peerTimestamp),
                ItemDataId = skillItem.DataId,
                Skill = skill,
                SkillLevel = skillLevel,
                WeaponHandlingState = weaponHandlingState,
                TargetObjectId = targetObjectId,
                AimPosition = aimPosition,
            };
            UseSkillRoutine(peerTimestamp, simulateState).Forget();
        }

        public virtual void InterruptCastingSkill()
        {
            if (!IsServer)
            {
                RPC(CmdInterruptCastingSkill, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered);
                return;
            }
            if (IsCastingSkillCanBeInterrupted && !IsCastingSkillInterrupted)
            {
                IsCastingSkillInterrupted = true;
                RPC(RpcInterruptCastingSkill, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered);
                ProceedInterruptCastingSkill();
            }
        }

        [ServerRpc]
        protected void CmdInterruptCastingSkill()
        {
            InterruptCastingSkill();
        }

        [AllRpc]
        protected void RpcInterruptCastingSkill()
        {
            if (IsServer)
            {
                // Don't interrupt using skill again
                return;
            }
            ProceedInterruptCastingSkill();
        }

        protected virtual void ProceedInterruptCastingSkill()
        {
            IsCastingSkillInterrupted = true;
            CastingSkillDuration = CastingSkillCountDown = 0;
            CancelSkill();

            BaseCharacterModel tpsModel = Entity.ActionModel;
            bool tpsModelAvailable = tpsModel != null && tpsModel.gameObject.activeSelf;
            BaseCharacterModel vehicleModel = Entity.PassengingVehicleModel as BaseCharacterModel;
            bool vehicleModelAvailable = vehicleModel != null;
            BaseCharacterModel fpsModel = Entity.FpsModel;
            bool fpsModelAvailable = IsClient && fpsModel != null && fpsModel.gameObject.activeSelf;

            if (tpsModelAvailable)
            {
                // TPS model
                tpsModel.StopActionAnimation();
                tpsModel.StopSkillCastAnimation();
                tpsModel.StopWeaponChargeAnimation();
            }
            if (vehicleModelAvailable)
            {
                // Vehicle model
                vehicleModel.StopActionAnimation();
                vehicleModel.StopSkillCastAnimation();
                vehicleModel.StopWeaponChargeAnimation();
            }
            if (fpsModelAvailable)
            {
                // FPS model
                fpsModel.StopActionAnimation();
                fpsModel.StopSkillCastAnimation();
                fpsModel.StopWeaponChargeAnimation();
            }
        }

        public virtual void CancelSkill()
        {
            for (int i = _skillCancellationTokenSources.Count - 1; i >= 0; --i)
            {
                if (!_skillCancellationTokenSources[i].IsCancellationRequested)
                    _skillCancellationTokenSources[i].Cancel();
                _skillCancellationTokenSources.RemoveAt(i);
            }
        }

        private int GetSimulateSeed(long timestamp)
        {
            return (int)(timestamp % 16384);
        }
    }
}
