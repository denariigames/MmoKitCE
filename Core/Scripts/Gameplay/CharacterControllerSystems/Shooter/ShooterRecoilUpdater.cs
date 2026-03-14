using UnityEngine;

namespace MultiplayerARPG
{
    public class ShooterRecoilUpdater : MonoBehaviour
    {
        public ShooterPlayerCharacterController Controller { get; set; }
        public BasePlayerCharacterEntity PlayingCharacterEntity => Controller.PlayingCharacterEntity;
        public CrosshairSetting CurrentCrosshairSetting => PlayingCharacterEntity.GetCrosshairSetting();
        [Min(0.01f)]
        public float recoilRateWhileSwimming = 1.5f;
        [Min(0.01f)]
        public float recoilRateWhileSprinting = 2f;
        [Min(0.01f)]
        public float recoilRateWhileWalking = 0.5f;
        [Min(0.01f)]
        public float recoilRateWhileMoving = 1f;
        [Min(0.01f)]
        public float recoilRateWhileCrouching = 0.5f;
        [Min(0.01f)]
        public float recoilRateWhileCrawling = 0.5f;
        [Min(0f)]
        public float recoilPitchScale = 1f;
        [Min(0f)]
        public float recoilYawScale = 1f;
        [Min(0f)]
        public float recoilRollScale = 1f;

        public virtual void Trigger(
            bool isLeftHand,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            BaseSkill skill,
            int skillLevel)
        {
            IWeaponItem weaponItem = weapon.GetWeaponItem();
            if (weaponItem == null)
                return;

            float recoilPitch;
            float recoilYaw;
            float recoilRoll;

            float weaponRecoil = Controller.IsZoomAimming ? weaponItem.RecoilWhileAiming : weaponItem.Recoil;
            float recoilWithModifier = weaponRecoil + PlayingCharacterEntity.GetCaches().RecoilModifier;
            if (PlayingCharacterEntity.MovementState.Has(MovementState.Forward) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.Backward) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.Left) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.Right))
            {
                if (PlayingCharacterEntity.MovementState.Has(MovementState.IsUnderWater))
                {
                    recoilPitch = recoilYaw = recoilRoll = recoilWithModifier * recoilRateWhileSwimming;
                }
                else if (PlayingCharacterEntity.ExtraMovementState == ExtraMovementState.IsSprinting)
                {
                    recoilPitch = recoilYaw = recoilRoll = recoilWithModifier * recoilRateWhileSprinting;
                }
                else if (PlayingCharacterEntity.ExtraMovementState == ExtraMovementState.IsWalking)
                {
                    recoilPitch = recoilYaw = recoilRoll = recoilWithModifier * recoilRateWhileWalking;
                }
                else
                {
                    recoilPitch = recoilYaw = recoilRoll = recoilWithModifier * recoilRateWhileMoving;
                }
            }
            else if (PlayingCharacterEntity.ExtraMovementState == ExtraMovementState.IsCrouching)
            {
                recoilPitch = recoilYaw = recoilRoll = recoilWithModifier * recoilRateWhileCrouching;
            }
            else if (PlayingCharacterEntity.ExtraMovementState == ExtraMovementState.IsCrawling)
            {
                recoilPitch = recoilYaw = recoilRoll = recoilWithModifier * recoilRateWhileCrawling;
            }
            else
            {
                recoilPitch = recoilYaw = recoilRoll = recoilWithModifier;
            }

            recoilPitch += weaponRecoil * (1f + PlayingCharacterEntity.GetCaches().RecoilRate);
            recoilPitch *= recoilPitchScale;

            recoilYaw += weaponRecoil * (1f + PlayingCharacterEntity.GetCaches().RecoilRate);
            recoilYaw *= recoilYawScale;

            recoilRoll += weaponRecoil * (1f + PlayingCharacterEntity.GetCaches().RecoilRate);
            recoilRoll *= recoilRollScale;

            if (recoilPitch > 0f || recoilYaw > 0f || recoilRoll > 0f)
            {
                Controller.CacheGameplayCameraController.Recoil(-recoilPitch, Random.Range(-recoilYaw, recoilYaw), Random.Range(-recoilRoll, recoilRoll));
            }
        }
    }
}
