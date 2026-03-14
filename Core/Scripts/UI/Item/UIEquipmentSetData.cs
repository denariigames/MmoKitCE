namespace MultiplayerARPG
{
    public struct UIEquipmentSetData
    {
        public EquipmentSet equipmentSet;
        public int equippedCount;
        public UIEquipmentSetData(EquipmentSet equipmentSet, int equippedCount)
        {
            this.equipmentSet = equipmentSet;
            this.equippedCount = equippedCount;
        }
    }
}
