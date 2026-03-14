using UnityEngine;

namespace MultiplayerARPG
{
    public class ShooterCrosshairUpdater : MonoBehaviour
    {
        public ShooterPlayerCharacterController Controller { get; set; }
        public BasePlayerCharacterEntity PlayingCharacterEntity => Controller.PlayingCharacterEntity;
        public CrosshairSetting CurrentCrosshairSetting => PlayingCharacterEntity.GetCrosshairSetting();
        public RectTransform CrosshairRect => Controller.CrosshairRect;
        public bool HideCrosshair => Controller.HideCrosshair;
        public Vector2 CurrentCrosshairSize { get; protected set; }
        public float spreadUpdateScale = 50f;
        protected float _addSpread = 0f;

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
            bool isMoving = PlayingCharacterEntity.MovementState.Has(MovementState.Forward) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.Backward) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.Left) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.Right) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.IsJump) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.IsDash);
            float maxSpread = weaponItem.CrosshairSetting.maxSpread;
            if (isMoving)
                _addSpread = weaponItem.CrosshairSetting.addSpreadWhileAttackAndMoving;
            CurrentCrosshairSize = Vector2.one * (maxSpread + _addSpread);
        }

        protected virtual void Update()
        {
            bool isMoving = PlayingCharacterEntity.MovementState.Has(MovementState.Forward) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.Backward) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.Left) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.Right) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.IsJump) ||
                PlayingCharacterEntity.MovementState.Has(MovementState.IsDash);

            if (isMoving)
            {
                UpdateCrosshair(CurrentCrosshairSetting, CurrentCrosshairSetting.expandPerFrame);
            }
            else
            {
                UpdateCrosshair(CurrentCrosshairSetting, -CurrentCrosshairSetting.shrinkPerFrame);
            }
        }

        protected virtual void UpdateCrosshair(CrosshairSetting setting, float power)
        {
            if (CrosshairRect == null)
                return;
            // Show cross hair if weapon's crosshair setting isn't hidden or there is a constructing building
            CrosshairRect.gameObject.SetActive(((!setting.hidden && !HideCrosshair) || Controller.ConstructingBuildingEntity != null) && !UIBlockController.IsBlockController() && !UIBlockActionController.IsBlockController());
            // Not active?, don't update
            if (!CrosshairRect.gameObject)
                return;
            // Change crosshair size by power
            Vector3 sizeDelta = CurrentCrosshairSize;
            // Expanding
            power *= Time.deltaTime * spreadUpdateScale;
            sizeDelta.x += power;
            sizeDelta.y += power;
            float minSpread = setting.minSpread;
            float maxSpread = setting.maxSpread;
            _addSpread -= Time.deltaTime * spreadUpdateScale * setting.shrinkPerFrame;
            if (_addSpread < 0f)
                _addSpread = 0f;
            maxSpread += _addSpread;
            sizeDelta = new Vector2(Mathf.Clamp(sizeDelta.x, minSpread, maxSpread), Mathf.Clamp(sizeDelta.y, minSpread, maxSpread));
            CrosshairRect.sizeDelta = CurrentCrosshairSize = sizeDelta;
        }
    }
}