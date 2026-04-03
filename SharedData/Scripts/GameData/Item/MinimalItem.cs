namespace MultiplayerARPG
{
    [System.Serializable]
    public class MinimalItem
    {
        public string Id { get; set; }
        public int DataId { get; set; }
        public int ItemType { get; set; }
        public int SellPrice { get; set; }
        public float Weight { get; set; }
        public int MaxStack { get; set; }
        public int MaxLevel { get; set; }
        public float LockDuration { get; set; }
        public int ExpireDuration { get; set; }
        public float MaxDurability { get; set; }
        public bool DestroyIfBroken { get; set; }
        public int MaxSocket { get; set; }
        public int AmmoCapacity { get; set; }

        public bool IsDefendEquipment()
        {
            return IsArmor() || IsShield();
        }

        public bool IsEquipment()
        {
            return IsDefendEquipment() || IsWeapon();
        }

        public bool IsUsable()
        {
            return IsPotion() || IsBuilding() || IsPet() || IsMount() || IsSkill();
        }

        public bool IsJunk()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Junk;
        }

        public bool IsArmor()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Armor;
        }

        public bool IsShield()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Shield;
        }

        public bool IsWeapon()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Weapon;
        }

        public bool IsPotion()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Potion;
        }

        public bool IsAmmo()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Ammo;
        }

        public bool IsBuilding()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Building;
        }

        public bool IsPet()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Pet;
        }

        public bool IsSocketEnhancer()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.SocketEnhancer;
        }

        public bool IsMount()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Mount;
        }

        public bool IsSkill()
        {
            return ItemType == (int)MultiplayerARPG.ItemType.Skill;
        }
    }
}