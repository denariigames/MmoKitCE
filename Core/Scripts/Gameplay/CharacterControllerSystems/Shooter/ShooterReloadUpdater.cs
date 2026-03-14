using UnityEngine;

namespace MultiplayerARPG
{
    public class ShooterReloadUpdater : MonoBehaviour
    {
        public ShooterPlayerCharacterController Controller { get; set; }
        public BasePlayerCharacterEntity PlayingCharacterEntity => Controller.PlayingCharacterEntity;
        public bool IsReloading { get; protected set; }
        public WeaponHandlingState? AttackingWeaponHandlingState { get; protected set; }
        [SerializeField]
        protected bool _continueReloadingAfterAttack = false;
        protected int? _reloadedDataIdR = null;
        protected int? _reloadedDataIdL = null;

        public virtual void Reload()
        {
            if (PlayingCharacterEntity.IsDead())
                return;
            if (PlayingCharacterEntity.IsPlayingActionAnimation())
                return;
            if (IsReloading)
                return;
            IsReloading = true;
            _reloadedDataIdR = null;
            _reloadedDataIdL = null;
            if (Controller.WeaponAbility != null &&
                Controller.WeaponAbility.ShouldDeactivateOnReload)
            {
                Controller.WeaponAbility.ForceDeactivated();
                Controller.WeaponAbilityState = WeaponAbilityState.Deactivated;
            }
        }

        public virtual void Interrupt()
        {
            IsReloading = false;
        }

        public virtual void InterruptByAttacking(WeaponHandlingState weaponHandlingState)
        {
            AttackingWeaponHandlingState = weaponHandlingState;
        }

        protected virtual void Update()
        {
            if (PlayingCharacterEntity.IsDead())
                IsReloading = false;
            if (!IsReloading)
                return;
            // Wait until animation end
            if (PlayingCharacterEntity.IsPlayingActionAnimation())
                return;
            bool continueReloadingR = false;
            bool continueReloadingL = false;
            IWeaponItem weaponItem;
            int ammoDataId;
            int ammoAmount;
            // Reload right-hand weapon
            if (!PlayingCharacterEntity.EquipWeapons.rightHand.IsAmmoFull(PlayingCharacterEntity) &&
                PlayingCharacterEntity.EquipWeapons.rightHand.HasAmmoToReload(PlayingCharacterEntity, out ammoDataId, out ammoAmount) &&
                (!_reloadedDataIdR.HasValue || _reloadedDataIdR.Value == ammoDataId))
            {
                _reloadedDataIdR = ammoDataId;
                weaponItem = PlayingCharacterEntity.EquipWeapons.GetRightHandWeaponItem();
                continueReloadingR = ShouldContinueReloading(weaponItem, ammoAmount);
                if (continueReloadingR && ProceedAttacking())
                {
                    IsReloading = _continueReloadingAfterAttack;
                    return;
                }
                continueReloadingR = ProceedReloading(false, weaponItem, ammoAmount) && continueReloadingR;
            }
            // Reload left-hand weapon
            if (!PlayingCharacterEntity.EquipWeapons.leftHand.IsAmmoFull(PlayingCharacterEntity) &&
                PlayingCharacterEntity.EquipWeapons.leftHand.HasAmmoToReload(PlayingCharacterEntity, out ammoDataId, out ammoAmount) &&
                (!_reloadedDataIdL.HasValue || _reloadedDataIdL.Value == ammoDataId))
            {
                _reloadedDataIdL = ammoDataId;
                weaponItem = PlayingCharacterEntity.EquipWeapons.GetLeftHandWeaponItem();
                continueReloadingL = ShouldContinueReloading(weaponItem, ammoAmount);
                if (continueReloadingL && ProceedAttacking())
                {
                    IsReloading = _continueReloadingAfterAttack;
                    return;
                }
                continueReloadingL = ProceedReloading(true, weaponItem, ammoAmount) && continueReloadingL;
            }
            if (!continueReloadingR && !continueReloadingL)
            {
                // Not continue reloading
                IsReloading = false;
            }
            AttackingWeaponHandlingState = null;
        }

        /// <summary>
        /// Return `TRUE` if attack proceeded
        /// </summary>
        /// <returns></returns>
        private bool ProceedAttacking()
        {
            if (!AttackingWeaponHandlingState.HasValue)
                return false;
            WeaponHandlingState weaponHandlingState = AttackingWeaponHandlingState.Value;
            PlayingCharacterEntity.Attack(ref weaponHandlingState);
            AttackingWeaponHandlingState = null;
            return true;
        }

        /// <summary>
        /// Return `TRUE` if reload proceeded
        /// </summary>
        /// <param name="isLeftHand"></param>
        /// <param name="weaponItem"></param>
        /// <returns></returns>
        private bool ProceedReloading(bool isLeftHand, IWeaponItem weaponItem, int ammoAmount)
        {
            if (weaponItem == null)
                return false;
            return PlayingCharacterEntity.Reload(isLeftHand);
        }

        private bool ShouldContinueReloading(IWeaponItem weaponItem, int ammoAmount)
        {
            return weaponItem.MaxAmmoEachReload > 0 && ammoAmount - weaponItem.MaxAmmoEachReload >= 0;
        }
    }
}
