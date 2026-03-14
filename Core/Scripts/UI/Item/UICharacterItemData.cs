namespace MultiplayerARPG
{
    public struct UICharacterItemData
    {
        public CharacterItem characterItem;
        public int targetLevel;
        public InventoryType inventoryType;
        public UICharacterItemData(CharacterItem characterItem, int targetLevel, InventoryType inventoryType)
        {
            this.characterItem = characterItem;
            this.targetLevel = targetLevel;
            this.inventoryType = inventoryType;
        }

        public UICharacterItemData(CharacterItem characterItem, InventoryType inventoryType) : this(characterItem, characterItem.level, inventoryType)
        {
        }

        public UICharacterItemData(BaseItem item, int targetLevel, InventoryType inventoryType) : this(CharacterItem.Create(item, targetLevel), targetLevel, inventoryType)
        {
        }
    }
}
