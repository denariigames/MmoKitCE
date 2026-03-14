namespace MultiplayerARPG
{
    public struct UIOwningCharacterItemData
    {
        public InventoryType inventoryType;
        public int indexOfData;
        public byte equipSlotIndex;
        public UIOwningCharacterItemData(InventoryType inventoryType, int indexOfData, byte equipSlotIndex)
        {
            this.inventoryType = inventoryType;
            this.indexOfData = indexOfData;
            this.equipSlotIndex = equipSlotIndex;
        }
    }
}
