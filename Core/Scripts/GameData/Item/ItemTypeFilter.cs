using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct ItemTypeFilter
    {
        public bool includeJunk;
        public bool includeArmor;
        public bool includeWeapon;
        public bool includeShield;
        public bool includePotion;
        public bool includeAmmo;
        public bool includeBuilding;
        public bool includePet;
        public bool includeSocketEnhancer;
        public bool includeMount;
        public bool includeSkill;

        public bool Filter(CharacterItem item)
        {
            if (item.IsEmptySlot())
                return false;
            if (includeJunk && item.GetItem().IsJunk())
                return true;
            if (includeArmor && item.GetItem().IsArmor())
                return true;
            if (includeWeapon && item.GetItem().IsWeapon())
                return true;
            if (includeShield && item.GetItem().IsShield())
                return true;
            if (includePotion && item.GetItem().IsPotion())
                return true;
            if (includeAmmo && item.GetItem().IsAmmo())
                return true;
            if (includeBuilding && item.GetItem().IsBuilding())
                return true;
            if (includePet && item.GetItem().IsPet())
                return true;
            if (includeSocketEnhancer && item.GetItem().IsSocketEnhancer())
                return true;
            if (includeMount && item.GetItem().IsMount())
                return true;
            if (includeSkill && item.GetItem().IsSkill())
                return true;
            return false;
        }

        public List<CharacterItem> Filters(IList<CharacterItem> fromList)
        {
            List<CharacterItem> resultList = new List<CharacterItem>();
            foreach (CharacterItem entry in fromList)
            {
                if (Filter(entry))
                    resultList.Add(entry);
            }
            return resultList;
        }
    }
}
