namespace MultiplayerARPG
{
    public partial class BaseItem
    {
        public bool TryGetRepairPrice(float durability, out float maxDurability, out ItemRepairPrice repairPrice)
        {
            return TryGetRepairPrice(durability, out maxDurability, out repairPrice, out _);
        }

        public bool TryGetRepairPrice(float durability, out float maxDurability, out ItemRepairPrice repairPrice, out UITextKeys gameMessage)
        {
            maxDurability = 0f;
            repairPrice = null;
            gameMessage = UITextKeys.NONE;
            if (ItemRefine == null)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_DATA;
                return false;
            }
            if (ItemRefine.RepairPrices == null || ItemRefine.RepairPrices.Length == 0)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_DATA;
                return false;
            }
            maxDurability = (this as IEquipmentItem).MaxDurability;
            if (maxDurability <= 0f)
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_REPAIR_ITEM_UNBREAKABLE;
                return false;
            }
            float durabilityRate = durability / maxDurability;
            if (durabilityRate >= 1f)
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_REPAIR_ITEM_NOT_BROKEN;
                return false;
            }
            System.Array.Sort(ItemRefine.RepairPrices);
            for (int i = ItemRefine.RepairPrices.Length - 1; i >= 0; --i)
            {
                repairPrice = ItemRefine.RepairPrices[i];
                if (durabilityRate < repairPrice.DurabilityRate)
                    return true;
            }
            return true;
        }

        public bool CanRepair(IPlayerCharacterData character, float durability, out float maxDurability, out ItemRepairPrice repairPrice)
        {
            return CanRepair(character, durability, out maxDurability, out repairPrice, out _);
        }

        public bool CanRepair(IPlayerCharacterData character, float durability, out float maxDurability, out ItemRepairPrice repairPrice, out UITextKeys gameMessage)
        {
            maxDurability = 0f;
            repairPrice = null;
            if (!this.IsEquipment())
            {
                gameMessage = UITextKeys.UI_ERROR_CANNOT_REPAIR;
                return false;
            }
            if (!TryGetRepairPrice(durability, out maxDurability, out repairPrice, out gameMessage))
            {
                return false;
            }
            return repairPrice.CanRepair(character, out gameMessage);
        }
    }
}
