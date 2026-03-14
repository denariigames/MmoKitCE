namespace MultiplayerARPG
{
    public interface IItemUIVisibilityManager
    {
        bool IsShopDialogVisible();
        bool IsRefineItemDialogVisible();
        bool IsDismantleItemDialogVisible();
        bool IsRepairItemDialogVisible();
        bool IsEnhanceSocketItemDialogVisible();
        bool IsStorageDialogVisible();
        bool IsDealingDialogVisibleWithDealingState();
        bool IsStartVendingDialogVisible();
        void ShowRefineItemDialog(InventoryType inventoryType, int indexOfData, byte equipSlotIndex);
        void ShowDismantleItemDialog(InventoryType inventoryType, int indexOfData, byte equipSlotIndex);
        void ShowRepairItemDialog(InventoryType inventoryType, int indexOfData, byte equipSlotIndex);
        void ShowEnhanceSocketItemDialog(InventoryType inventoryType, int indexOfData, byte equipSlotIndex);
        void ShowStorageDialog(StorageType storageType, string storageOwnerId, uint objectId, int weightLimit, int slotLimit);
        void HideStorageDialog(StorageType storageType, string storageOwnerId);
    }
}
