namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        public virtual bool CanDoActions()
        {
            return !this.IsDead() && !IsAttacking && !IsUsingSkill && !IsReloading && !IsPlayingActionAnimation();
        }

        public virtual bool CanEquipItem()
        {
            return true;
        }

        public virtual bool CanUnEquipItem()
        {
            return true;
        }

        public virtual bool CanPickup()
        {
            return true;
        }

        public virtual bool CanDropItem()
        {
            return true;
        }

        public virtual bool CanRepairItem()
        {
            return true;
        }

        public virtual bool CanRefineItem()
        {
            return true;
        }

        public virtual bool CanEnhanceSocketItem()
        {
            return true;
        }

        public virtual bool CanRemoveEnhancerFromItem()
        {
            return true;
        }

        public virtual bool CanDismantleItem()
        {
            return true;
        }

        public virtual bool CanSellItem()
        {
            return true;
        }

        public virtual bool CanMoveItem()
        {
            return true;
        }

        public virtual bool CanAttack()
        {
            if (IsWeaponsSheathed)
                return false;
            if (!CanDoActions())
                return false;
            if (CachedData.DisallowAttack)
                return false;
            if (PassengingVehicleEntity != null &&
                PassengingVehicleSeat != null &&
                !PassengingVehicleSeat.canAttack)
                return false;
            return true;
        }

        public virtual bool CanUseSkill()
        {
            if (IsWeaponsSheathed)
                return false;
            if (!CanDoActions())
                return false;
            if (CachedData.DisallowUseSkill)
                return false;
            if (PassengingVehicleEntity != null &&
                PassengingVehicleSeat != null &&
                !PassengingVehicleSeat.canUseSkill)
                return false;
            return true;
        }

        public virtual bool CanUseSkillItem()
        {
            if (IsWeaponsSheathed)
                return false;
            if (!CanDoActions())
                return false;
            if (CachedData.DisallowUseSkill)
                return false;
            if (CachedData.DisallowUseItem)
                return false;
            if (PassengingVehicleEntity != null &&
                PassengingVehicleSeat != null &&
                !PassengingVehicleSeat.canUseSkill)
                return false;
            return true;
        }

        public virtual bool CanUseItem()
        {
            if (this.IsDead())
                return false;
            if (CachedData.DisallowUseItem)
                return false;
            return true;
        }

        public virtual bool CanChangeAmmoItem()
        {
            return true;
        }

        public virtual bool CanRemoveAmmoFromItem()
        {
            return true;
        }
    }
}
