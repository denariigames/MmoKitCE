using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class CharacterItemSort : IComparer<CharacterItem>
    {
        public int GetItemTypeWeight(ItemType type)
        {
            switch (type)
            {
                case ItemType.Armor:
                    return 10;
                case ItemType.Weapon:
                    return 9;
                case ItemType.Shield:
                    return 8;
                case ItemType.Skill:
                    return 7;
                case ItemType.SocketEnhancer:
                    return 6;
                case ItemType.Pet:
                    return 5;
                case ItemType.Mount:
                    return 4;
                case ItemType.Building:
                    return 3;
                case ItemType.Potion:
                    return 2;
                case ItemType.Ammo:
                    return 1;
            }
            return 0;
        }

        public int Compare(CharacterItem x, CharacterItem y)
        {
            if (x.IsEmptySlot() && y.IsEmptySlot()) return 0;
            else if (x.IsEmptySlot())
                return -1;
            else if (y.IsEmptySlot())
                return 1;
            int result = GetItemTypeWeight(x.GetItem().ItemType).CompareTo(GetItemTypeWeight(y.GetItem().ItemType));
            if (result != 0)
                return result;
            if (x.GetArmorItem() != null && y.GetArmorItem() != null)
            {
                if (x.GetArmorItem().ArmorType != null && y.GetArmorItem().ArmorType != null)
                    result = x.GetArmorItem().ArmorType.Id.CompareTo(y.GetArmorItem().ArmorType.Id);
            }
            if (x.GetWeaponItem() != null && y.GetWeaponItem() != null)
            {
                if (x.GetWeaponItem().WeaponType != null && y.GetWeaponItem().WeaponType != null)
                    result = x.GetWeaponItem().WeaponType.Id.CompareTo(y.GetWeaponItem().WeaponType.Id);
            }
            if (x.GetAmmoItem() != null && y.GetAmmoItem() != null)
            {
                if (x.GetAmmoItem().AmmoType != null && y.GetAmmoItem().AmmoType != null)
                    result = x.GetAmmoItem().AmmoType.Id.CompareTo(y.GetAmmoItem().AmmoType.Id);
            }
            if (result != 0)
                return result;
            result = x.GetItem().Id.CompareTo(y.GetItem().Id);
            if (result != 0)
                return result;
            return x.level.CompareTo(y.level);
        }
    }
}