using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public abstract class UIDamageableEntity<T> : UIBaseGameEntity<T>
        where T : DamageableEntity
    {

        [Header("Options")]
        [Tooltip("This is duration before this will be invisible, if this is <= 0f it will always visible")]
        [FormerlySerializedAs("visibleWhenHitDuration")]
        public float visibleDurationAfterHit = 2f;

        [Header("Damageable Entity - UI Elements")]
        public UIGageValue uiGageHp;

        [Header("Damageable Entity - Options")]
        public bool hideWhileDead;

        protected float _receivedDamageTime;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiGageHp = null;
        }

        protected override void AddEvents(T entity)
        {
            if (entity == null)
                return;
            base.AddEvents(entity);
            entity.onReceivedDamage += OnReceivedDamage;
            entity.onCurrentHpChange += OnCurrentHpChange;
        }

        protected override void RemoveEvents(T entity)
        {
            if (entity == null)
                return;
            base.RemoveEvents(entity);
            entity.onReceivedDamage -= OnReceivedDamage;
            entity.onCurrentHpChange -= OnCurrentHpChange;
        }

        protected override bool ValidateToUpdateUI()
        {
            return base.ValidateToUpdateUI() && (!hideWhileDead || !Data.IsDead());
        }

        protected void OnReceivedDamage(
            DamageableEntity target,
            HitBoxPosition position,
            Vector3 fromPosition,
            EntityInfo instigator,
            CombatAmountType combatAmountType,
            int totalDamage,
            CharacterItem weapon,
            BaseSkill skill,
            int skillLevel,
            CharacterBuff buff,
            bool isDamageOverTime)
        {
            _receivedDamageTime = Time.unscaledTime;
        }

        protected void OnCurrentHpChange(DamageableEntity target, int oldHp, int hp)
        {
            UpdateHp();
        }

        protected override void UpdateUI()
        {
            if (!ValidateToUpdateUI())
            {
                CacheCanvas.enabled = false;
                return;
            }
            UpdateHp();

            if (Time.unscaledTime - _receivedDamageTime < visibleDurationAfterHit || visibleDurationAfterHit <= 0f)
            {
                CacheCanvas.enabled = true;
                return;
            }

            base.UpdateUI();
        }

        protected override void UpdateData()
        {
            base.UpdateData();
            UpdateHp();
        }

        /// <summary>
        /// Update the HP UI, if the selected damageable hit box is a segmented part, use the segmented HP
        /// </summary>
        protected void UpdateHp()
        {
            if (uiGageHp == null)
                return;
            int currentHp = 0;
            int maxHp = 0;
            if (Data != null)
            {
                currentHp = Data.CurrentHp;
                maxHp = Data.MaxHp;
                BasePlayerCharacterController controller = BasePlayerCharacterController.Singleton;
                if (Data is BuildingEntity building &&
                    controller != null &&
                    !string.IsNullOrEmpty(controller.SelectedSegmentedPartId) &&
                    controller.SelectedGameEntityObjectId == building.ObjectId &&
                    building.TryGetSegmentedPartHp(controller.SelectedSegmentedPartId, out int segmentedCurrentHp, out int segmentedMaxHp))
                {
                    currentHp = segmentedCurrentHp;
                    maxHp = segmentedMaxHp;
                }
            }
            uiGageHp.Update(currentHp, maxHp);
        }
    }

    public class UIDamageableEntity : UIDamageableEntity<DamageableEntity> { }
}
