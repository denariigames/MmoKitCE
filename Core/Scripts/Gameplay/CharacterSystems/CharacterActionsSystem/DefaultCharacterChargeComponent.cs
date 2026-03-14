using LiteNetLib;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(CharacterActionComponentManager))]
    public class DefaultCharacterChargeComponent : BaseNetworkedGameEntityComponent<BaseCharacterEntity>, ICharacterChargeComponent
    {
        public bool IsCharging { get; protected set; }
        protected bool _skipMovementValidation;
        public bool IsSkipMovementValidationWhileCharging { get { return _skipMovementValidation; } set { _skipMovementValidation = value; } }
        protected bool _shouldUseRootMotion;
        public bool IsUseRootMotionWhileCharging { get { return _shouldUseRootMotion; } protected set { _shouldUseRootMotion = value; } }

        protected struct ChargeState
        {
            public bool IsStopping;
            public bool IsLeftHand;
        }

        public bool WillDoActionWhenStopCharging
        {
            get
            {
                return IsCharging && (Time.unscaledTime - _chargeStartTime >= _chargeDuration) && !Entity.IsDead();
            }
        }
        public float MoveSpeedRateWhileCharging { get; protected set; }
        public MovementRestriction MovementRestrictionWhileCharging { get; protected set; }
        public System.Action OnChargeStart { get; set; }
        public System.Action OnChargeEnd { get; set; }

        protected CharacterActionComponentManager _manager;
        protected float _chargeStartTime;
        protected float _chargeDuration;
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
            ClearChargeStates();
            _manager = null;
            _entityIsPlayer = false;
            _playerCharacterEntity = null;
        }

        public virtual void ClearChargeStates()
        {
            IsCharging = false;
        }

        protected virtual void PlayChargeAnimation(bool isLeftHand)
        {
            // Get weapon type data
            IWeaponItem weaponItem = Entity.GetAvailableWeapon(ref isLeftHand).GetWeaponItem();
            int weaponTypeDataId = weaponItem.WeaponType.DataId;
            // Play animation
            BaseCharacterModel tpsModel = Entity.ActionModel;
            bool tpsModelAvailable = tpsModel != null && tpsModel.gameObject.activeSelf;
            BaseCharacterModel vehicleModel = Entity.PassengingVehicleModel as BaseCharacterModel;
            bool vehicleModelAvailable = vehicleModel != null;
            bool overridePassengerActionAnimations = vehicleModelAvailable && Entity.PassengingVehicleSeat.overridePassengerActionAnimations;
            BaseCharacterModel fpsModel = Entity.FpsModel;
            bool fpsModelAvailable = IsClient && fpsModel != null && fpsModel.gameObject.activeSelf;
            if (vehicleModelAvailable)
            {
                // Vehicle model
                vehicleModel.PlayWeaponChargeClip(weaponTypeDataId, isLeftHand, out _skipMovementValidation, out _shouldUseRootMotion);
                vehicleModel.PlayEquippedWeaponCharge(isLeftHand);
            }
            if (!overridePassengerActionAnimations)
            {
                if (tpsModelAvailable)
                {
                    // TPS model
                    tpsModel.PlayWeaponChargeClip(weaponTypeDataId, isLeftHand, out _skipMovementValidation, out _shouldUseRootMotion);
                    tpsModel.PlayEquippedWeaponCharge(isLeftHand);
                }
                if (fpsModelAvailable)
                {
                    // FPS model
                    fpsModel.PlayWeaponChargeClip(weaponTypeDataId, isLeftHand, out _, out _);
                    fpsModel.PlayEquippedWeaponCharge(isLeftHand);
                }
            }
            // Set weapon charging state
            MoveSpeedRateWhileCharging = Entity.GetMoveSpeedRateWhileCharging(weaponItem);
            MovementRestrictionWhileCharging = Entity.GetMovementRestrictionWhileCharging(weaponItem);
            IsCharging = true;
            _chargeStartTime = Time.unscaledTime;
            _chargeDuration = weaponItem.ChargeDuration;
            if (_entityIsPlayer && IsServer)
                GameInstance.ServerLogHandlers.LogChargeStart(_playerCharacterEntity);
            OnChargeStart?.Invoke();
        }

        protected virtual void StopChargeAnimation()
        {
            bool doActionWhenStopCharging = WillDoActionWhenStopCharging;
            // Play animation
            BaseCharacterModel tpsModel = Entity.ActionModel;
            bool tpsModelAvailable = tpsModel != null && tpsModel.gameObject.activeSelf;
            BaseCharacterModel vehicleModel = Entity.PassengingVehicleModel as BaseCharacterModel;
            bool vehicleModelAvailable = vehicleModel != null;
            bool overridePassengerActionAnimations = vehicleModelAvailable && Entity.PassengingVehicleSeat.overridePassengerActionAnimations;
            BaseCharacterModel fpsModel = Entity.FpsModel;
            bool fpsModelAvailable = IsClient && fpsModel != null && fpsModel.gameObject.activeSelf;
            if (vehicleModelAvailable)
            {
                // Vehicle model
                vehicleModel.StopWeaponChargeAnimation();
            }
            if (!overridePassengerActionAnimations)
            {
                if (tpsModelAvailable)
                {
                    // TPS model
                    tpsModel.StopWeaponChargeAnimation();
                }
                if (fpsModelAvailable)
                {
                    // FPS model
                    fpsModel.StopWeaponChargeAnimation();
                }
            }
            // Set weapon charging state
            IsCharging = false;
            if (_entityIsPlayer && IsServer)
                GameInstance.ServerLogHandlers.LogChargeEnd(_playerCharacterEntity, doActionWhenStopCharging);
            OnChargeEnd?.Invoke();
        }

        public virtual void StartCharge(bool isLeftHand)
        {
            if (!IsServer && IsOwnerClient)
            {
                RPC(CmdStartCharge, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, isLeftHand);
                PlayChargeAnimation(isLeftHand);
            }
            else if (IsOwnerClientOrOwnedByServer)
            {
                // Start charge immediately at server
                ProceedCmdStartCharge(isLeftHand);
            }
        }

        [ServerRpc]
        protected void CmdStartCharge(bool isLeftHand)
        {
            ProceedCmdStartCharge(isLeftHand);
        }

        protected void ProceedCmdStartCharge(bool isLeftHand)
        {
            RPC(RpcStartCharge, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered, isLeftHand);
            PlayChargeAnimation(isLeftHand);
        }

        [AllRpc]
        protected void RpcStartCharge(bool isLeftHand)
        {
            if (IsServer || IsOwnerClient)
            {
                // Don't stop charge again
                return;
            }
            PlayChargeAnimation(isLeftHand);
        }

        public virtual void StopCharge()
        {
            if (!IsServer && IsOwnerClient)
            {
                RPC(CmdStopCharge, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered);
                StopChargeAnimation();
            }
            else if (IsOwnerClientOrOwnedByServer)
            {
                // Stop charge immediately at server
                ProceedCmdStopCharge();
            }
        }

        [ServerRpc]
        protected void CmdStopCharge()
        {
            ProceedCmdStopCharge();
        }

        protected void ProceedCmdStopCharge()
        {
            RPC(RpcStopCharge, BaseGameEntity.ACTION_DATA_CHANNEL, DeliveryMethod.ReliableOrdered);
            StopChargeAnimation();
        }

        [AllRpc]
        protected void RpcStopCharge()
        {
            if (IsServer || IsOwnerClient)
            {
                // Don't stop charge again
                return;
            }
            StopChargeAnimation();
        }
    }
}
