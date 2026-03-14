using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        public bool IsUpdatingItems { get; set; } = false;

        public bool IsDealing
        {
            get { return DealingComponent != null && DealingComponent.DealingState != DealingState.None; }
        }

        public bool IsVendingStarted
        {
            get { return VendingComponent != null && VendingComponent.Data.isStarted; }
        }

        public override bool CanDoActions()
        {
            return base.CanDoActions() && !IsDealing && !IsVendingStarted && !IsWarping;
        }

        public bool CanManageItem()
        {
            if (IsWarping)
                return false;
            if (IsUpdatingItems)
                return false;
            if (IsDealing)
                return false;
            if (IsVendingStarted)
                return false;
            if (IsAttacking)
                return false;
            if (IsUsingSkill)
                return false;
            if (IsReloading)
                return false;
            if (IsPlayingActionAnimation())
                return false;
            return true;
        }

        public override bool CanEquipItem()
        {
            return CanManageItem() && Time.unscaledTime - LastActionEndTime > 0.5f;
        }

        public override bool CanUnEquipItem()
        {
            return CanManageItem() && Time.unscaledTime - LastActionEndTime > 0.5f;
        }

        public override bool CanPickup()
        {
            if (IsWarping)
                return false;
            if (IsUpdatingItems)
                return false;
            if (IsDealing)
                return false;
            if (this.IsDead())
                return false;
            return true;
        }

        public override bool CanDropItem()
        {
            if (IsWarping)
                return false;
            if (IsUpdatingItems)
                return false;
            if (IsDealing)
                return false;
            if (this.IsDead())
                return false;
            return true;
        }

        public override bool CanRepairItem()
        {
            return CanManageItem();
        }

        public override bool CanRefineItem()
        {
            return CanManageItem();
        }

        public override bool CanEnhanceSocketItem()
        {
            return CanManageItem();
        }

        public override bool CanRemoveEnhancerFromItem()
        {
            return CanManageItem();
        }

        public override bool CanDismantleItem()
        {
            return CanManageItem();
        }

        public override bool CanSellItem()
        {
            if (IsWarping)
                return false;
            if (IsUpdatingItems)
                return false;
            if (IsDealing)
                return false;
            if (!CanDoActions())
                return false;
            return true;
        }

        public override bool CanMoveItem()
        {
            return CanManageItem();
        }

        public override bool CanUseItem()
        {
            if (IsWarping)
                return false;
            if (IsDealing)
                return false;
            if (IsVendingStarted)
                return false;
            return base.CanUseItem();
        }

        public override bool CanChangeAmmoItem()
        {
            return CanManageItem();
        }

        public override bool CanRemoveAmmoFromItem()
        {
            return CanManageItem();
        }
    }
}
