using Cysharp.Threading.Tasks;
using LiteNetLib;
using LiteNetLibManager;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(CharacterActionComponentManager))]
    public class DefaultCharacterReloadComponent : BaseNetworkedGameEntityComponent<BaseCharacterEntity>, ICharacterReloadComponent
    {
        protected readonly List<CancellationTokenSource> _reloadCancellationTokenSources = new List<CancellationTokenSource>();
        public int ReloadingAmmoDataId { get; protected set; }
        public int ReloadingAmmoAmount { get; protected set; }
        public bool IsReloading { get; protected set; }
        public float LastReloadEndTime { get; protected set; }
        protected bool _skipMovementValidation;
        public bool IsSkipMovementValidationWhileReloading { get { return _skipMovementValidation; } set { _skipMovementValidation = value; } }
        protected bool _shouldUseRootMotion;
        public bool IsUseRootMotionWhileReloading { get { return _shouldUseRootMotion; } protected set { _shouldUseRootMotion = value; } }
        public float MoveSpeedRateWhileReloading { get; protected set; }
        public MovementRestriction MovementRestrictionWhileReloading { get; protected set; }
        public System.Action OnReloadStart { get; set; }
        public System.Action<int> OnReloadTrigger { get; set; }
        public System.Action OnReloadEnd { get; set; }
        public System.Action OnReloadInterupted { get; set; }
        public AnimActionType AnimActionType { get; protected set; }

        protected CharacterActionComponentManager _manager;
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
            CancelReload();
            ClearReloadStates();
            _manager = null;
            _entityIsPlayer = false;
            _playerCharacterEntity = null;
        }

        protected virtual void SetReloadActionStates(AnimActionType animActionType, int reloadingAmmoDataId, int reloadingAmmoAmount)
        {
            ClearReloadStates();
            AnimActionType = animActionType;
            ReloadingAmmoDataId = reloadingAmmoDataId;
            ReloadingAmmoAmount = reloadingAmmoAmount;
            IsReloading = true;
        }

        public virtual void ClearReloadStates()
        {
            ReloadingAmmoAmount = 0;
            IsReloading = false;
        }

        protected virtual async UniTaskVoid ReloadRoutine(bool isLeftHand, int reloadingAmmoDataId, int reloadingAmmoAmount)
        {
            // Prevent speed hack and also prevent data error
            // Reload is unlike attacking, I think it won't have someone who want to play reload very fast like attacking
            if (Time.unscaledTime - LastReloadEndTime < 0f)
                return;

            // Prepare cancellation
            CancellationTokenSource reloadCancellationTokenSource = new CancellationTokenSource();
            _reloadCancellationTokenSources.Add(reloadCancellationTokenSource);

            // Prepare requires data and get weapon data
            Entity.GetReloadingData(
                ref isLeftHand,
                out AnimActionType animActionType,
                out int animActionDataId,
                out CharacterItem weapon);

            // Prepare requires data and get animation data
            Entity.GetAnimationData(
                animActionType,
                animActionDataId,
                0,
                out float animSpeedRate,
                out float[] triggerDurations,
                out float totalDuration);

            // Set doing action state at clients and server
            SetReloadActionStates(animActionType, reloadingAmmoDataId, reloadingAmmoAmount);

            // Prepare requires data and get damages data
            IWeaponItem weaponItem = weapon.GetWeaponItem();
            float overrideDuration = 0f;
            if (weaponItem.ReloadDuration > 0f)
                totalDuration = overrideDuration = weaponItem.ReloadDuration;
            float changedDuration = Entity.CachedData.ReloadDuration;

            // Calculate move speed rate while doing action at clients and server
            MoveSpeedRateWhileReloading = Entity.GetMoveSpeedRateWhileReloading(weaponItem);
            MovementRestrictionWhileReloading = Entity.GetMovementRestrictionWhileReloading(weaponItem);

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
                if (vehicleModelAvailable)
                    vehicleModel.PlayActionAnimation(AnimActionType, animActionDataId, 0, out _skipMovementValidation, out _shouldUseRootMotion, animSpeedRate, changedDuration, overrideDuration);
                if (!overridePassengerActionAnimations)
                {
                    if (tpsModelAvailable)
                        tpsModel.PlayActionAnimation(AnimActionType, animActionDataId, 0, out _skipMovementValidation, out _shouldUseRootMotion, animSpeedRate, changedDuration, overrideDuration);
                    if (fpsModelAvailable)
                        fpsModel.PlayActionAnimation(AnimActionType, animActionDataId, 0, out _, out _, animSpeedRate, changedDuration, overrideDuration);
                }

                // Prepare action durations
                float remainsDuration = BaseCharacterModel.GetAnimationDuration(totalDuration, animSpeedRate, changedDuration);
                LastReloadEndTime = Time.unscaledTime + remainsDuration;
                if (weaponItem.ReloadDuration <= 0f)
                {
                    await _manager.PrepareActionDurations(triggerDurations, totalDuration, animSpeedRate, changedDuration, reloadCancellationTokenSource.Token,
                        (__triggerDurations, __totalDuration, __remainsDuration, __endTime) =>
                        {
                            triggerDurations = __triggerDurations;
                            totalDuration = __totalDuration;
                            remainsDuration = __remainsDuration;
                            LastReloadEndTime = __endTime;
                        });
                }

                // Special effects will plays on clients only
                if (IsClient)
                {
                    // Play weapon reload special effects
                    if (!overridePassengerActionAnimations)
                    {
                        if (tpsModelAvailable)
                            tpsModel.PlayEquippedWeaponReload(isLeftHand);
                        if (fpsModelAvailable)
                            fpsModel.PlayEquippedWeaponReload(isLeftHand);
                    }
                    else if (vehicleModelAvailable)
                    {
                        vehicleModel.PlayEquippedWeaponReload(isLeftHand);
                    }
                    // Play reload sfx
                    weaponItem.ReloadClip?.Play(tpsModel.GenericAudioSource);
                }

                // Reload starts
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogReloadStart(_playerCharacterEntity, triggerDurations);
                OnReloadStart?.Invoke();

                bool reloaded = false;
                for (byte triggerIndex = 0; triggerIndex < triggerDurations.Length; ++triggerIndex)
                {
                    // Wait until triggger before reload ammo
                    float tempTriggerDuration = triggerDurations[triggerIndex] / animSpeedRate;
                    remainsDuration -= tempTriggerDuration;
                    await GenericUtils.FrameBasedDelay(tempTriggerDuration, reloadCancellationTokenSource.Token);

                    // Special effects will plays on clients only
                    if (IsClient)
                    {
                        // Play weapon reload special effects
                        if (tpsModelAvailable)
                            tpsModel.PlayEquippedWeaponReloaded(isLeftHand);
                        if (fpsModelAvailable)
                            fpsModel.PlayEquippedWeaponReloaded(isLeftHand);
                        // Play reload sfx
                        weaponItem.ReloadedClip?.Play(tpsModel.GenericAudioSource);
                    }

                    // Reload / Fill ammo
                    if (!reloaded)
                    {
                        reloaded = true;
                        OnReloadTrigger?.Invoke(triggerIndex);
                        ActionTrigger(reloadingAmmoDataId, reloadingAmmoAmount, triggerIndex, isLeftHand, weapon.id);
                    }

                    if (remainsDuration <= 0f)
                    {
                        // Stop trigger animations loop
                        break;
                    }
                }

                if (remainsDuration > 0f)
                {
                    // Wait until animation ends to stop actions
                    await GenericUtils.FrameBasedDelay(remainsDuration, reloadCancellationTokenSource.Token);
                }
            }
            catch (System.OperationCanceledException)
            {
                // Catch the cancellation
                OnReloadInterupted?.Invoke();
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogReloadInterrupt(_playerCharacterEntity);
            }
            catch (System.Exception ex)
            {
                // Other errors
                Debug.LogError("Error occuring in `DefaultCharacterReloadComponent` -> `ReloadRoutine`, " + this);
                Debug.LogException(ex);
            }
            finally
            {
                LastReloadEndTime = Time.unscaledTime;
                reloadCancellationTokenSource.Dispose();
                _reloadCancellationTokenSources.Remove(reloadCancellationTokenSource);
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogReloadEnd(_playerCharacterEntity);
                OnReloadEnd?.Invoke();
            }
            // Clear action states at clients and server
            ClearReloadStates();
        }

        protected virtual void ActionTrigger(int reloadingAmmoDataId, int reloadingAmmoAmount, byte triggerIndex, bool isLeftHand, string id)
        {
            if (!IsServer)
                return;
            if (Entity.IsDead())
                return;
            // Prepare and validate item
            EquipWeapons equipWeapons = Entity.EquipWeapons;
            if ((isLeftHand && !string.Equals(equipWeapons.leftHand.id, id)) ||
                (!isLeftHand && !string.Equals(equipWeapons.rightHand.id, id)))
            {
                // Invalid item, player may change the item before the reloading is done
                return;
            }
            CharacterItem weapon = isLeftHand ? equipWeapons.leftHand : equipWeapons.rightHand;
            if (!Entity.DecreaseItems(reloadingAmmoDataId, reloadingAmmoAmount))
            {
                if (_entityIsPlayer && IsServer)
                    GameInstance.ServerLogHandlers.LogReloadTriggerFail(_playerCharacterEntity, triggerIndex, ActionTriggerFailReasons.NotEnoughResources);
                return;
            }
            IWeaponItem weaponItemData = weapon.GetWeaponItem();
            if (!weaponItemData.NoAmmoDataIdChange && weapon.ammo > 0 && weapon.ammoDataId != reloadingAmmoDataId)
            {
                // If ammo that stored in the weapon is difference
                // Then it will return ammo in the weapon, and replace amount with the new one
                Entity.IncreaseItems(CharacterItem.Create(weapon.ammoDataId, 1, weapon.ammo));
                weapon.ammo = 0;
            }
            Entity.FillEmptySlots();
            if (weaponItemData.NoAmmoDataIdChange && weaponItemData.AmmoItemIds.Count > 0)
                reloadingAmmoDataId = weaponItemData.AmmoItemIds.First();
            weapon.ammoDataId = reloadingAmmoDataId;
            weapon.ammo += reloadingAmmoAmount;
            if (isLeftHand)
            {
                equipWeapons.leftHand = weapon;
            }
            else
            {
                equipWeapons.rightHand = weapon;
            }
            Entity.EquipWeapons = equipWeapons;
            if (_entityIsPlayer && IsServer)
                GameInstance.ServerLogHandlers.LogReloadTrigger(_playerCharacterEntity, triggerIndex);
        }

        public virtual void CancelReload()
        {
            for (int i = _reloadCancellationTokenSources.Count - 1; i >= 0; --i)
            {
                if (!_reloadCancellationTokenSources[i].IsCancellationRequested)
                    _reloadCancellationTokenSources[i].Cancel();
                _reloadCancellationTokenSources.RemoveAt(i);
            }
            IsReloading = false;
        }

        public virtual void Reload(bool isLeftHand)
        {
            if (!IsServer && IsOwnerClient)
            {
                RPC(CmdReload, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, isLeftHand);
            }
            else if (IsOwnerClientOrOwnedByServer)
            {
                // Reload immediately at server
                ProceedCmdReload(isLeftHand);
            }
        }

        [ServerRpc]
        protected void CmdReload(bool isLeftHand)
        {
            ProceedCmdReload(isLeftHand);
        }

        protected void ProceedCmdReload(bool isLeftHand)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            // Prevent speed hack and also prevent data error
            // Reload is unlike attacking, I think it won't have someone who want to play reload very fast like attacking
            if (Time.unscaledTime - LastReloadEndTime < 0f)
                return;

            CharacterItem reloadingWeapon = isLeftHand ? Entity.EquipWeapons.leftHand : Entity.EquipWeapons.rightHand;
            if (reloadingWeapon.IsEmptySlot())
            {
                // Invalid item data
                return;
            }
            IWeaponItem reloadingWeaponItem = reloadingWeapon.GetWeaponItem();
            if (reloadingWeaponItem == null || reloadingWeaponItem.AmmoCapacity <= 0)
            {
                // This is not an items that have something like gun's magazine, it might be bow or crossbow :P
                return;
            }
            bool hasAmmoType = reloadingWeaponItem.WeaponType.AmmoType != null;
            bool hasAmmoItems = reloadingWeaponItem.AmmoItemIds.Count > 0;
            if (!hasAmmoType && !hasAmmoItems)
            {
                // This is not an items that have something like gun's magazine, it might be bow or crossbow :P
                return;
            }

            if (reloadingWeapon.IsAmmoFull(Entity))
            {
                // Full, don't reload
                return;
            }

            // Prepare reload data
            if (!reloadingWeapon.HasAmmoToReload(Entity, out int reloadingAmmoDataId, out int inventoryAmount))
            {
                // No ammo to reload
                return;
            }

            int ammoCapacity = reloadingWeaponItem.AmmoCapacity;
            if (!reloadingWeaponItem.NoAmmoCapacityOverriding &&
                GameInstance.Items.TryGetValue(reloadingAmmoDataId, out BaseItem ammoItem) &&
                ammoItem.OverrideAmmoCapacity > 0)
            {
                // Override capacity by the item
                ammoCapacity = ammoItem.OverrideAmmoCapacity;
            }
            ammoCapacity += Mathf.CeilToInt(Entity.CachedData.AmmoCapacity);

            int reloadingAmmoAmount = 0;
            if (!reloadingWeaponItem.NoAmmoDataIdChange && reloadingWeapon.ammoDataId != reloadingAmmoDataId)
            {
                // If ammo that stored in the weapon is difference
                // Then it will return ammo in the weapon, and replace amount with the new one
                reloadingAmmoAmount = ammoCapacity;
            }
            else
            {
                reloadingAmmoAmount = ammoCapacity - reloadingWeapon.ammo;
            }

            if (reloadingWeaponItem.MaxAmmoEachReload > 0 &&
                reloadingAmmoAmount > reloadingWeaponItem.MaxAmmoEachReload)
            {
                reloadingAmmoAmount = reloadingWeaponItem.MaxAmmoEachReload;
            }

            if (inventoryAmount < reloadingAmmoAmount)
            {
                // Ammo in inventory less than reloading amount, so use amount of ammo in inventory
                reloadingAmmoAmount = inventoryAmount;
            }

            if (reloadingAmmoAmount <= 0)
            {
                // No ammo to reload
                return;
            }

            RPC(RpcReload, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, isLeftHand, reloadingAmmoDataId, reloadingAmmoAmount);
            ReloadRoutine(isLeftHand, reloadingAmmoDataId, reloadingAmmoAmount).Forget();
#endif
        }

        [AllRpc]
        protected void RpcReload(bool isLeftHand, int reloadingAmmoDataId, int reloadingAmmoAmount)
        {
            if (IsServer)
            {
                // Don't play reloading animation again
                return;
            }
            ReloadRoutine(isLeftHand, reloadingAmmoDataId, reloadingAmmoAmount).Forget();
        }
    }
}
