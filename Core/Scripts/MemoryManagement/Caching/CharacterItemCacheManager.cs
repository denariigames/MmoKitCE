using Unity.Profiling;

namespace MultiplayerARPG
{
    public class CharacterItemCacheManager : BaseCacheManager<CharacterItem, CharacterItemCacheData>
    {
        protected static ProfilerMarker s_profilerMarker = new ProfilerMarker("CharacterItemCacheManager");
        public override ProfilerMarker ProfilerMarker => s_profilerMarker;

        public BaseItem GetItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetItem();
        }

        public IUsableItem GetUsableItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetUsableItem();
        }

        public IEquipmentItem GetEquipmentItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetEquipmentItem();
        }

        public IDefendEquipmentItem GetDefendItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetDefendItem();
        }

        public IArmorItem GetArmorItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetArmorItem();
        }

        public IWeaponItem GetWeaponItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetWeaponItem();
        }

        public IShieldItem GetShieldItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetShieldItem();
        }

        public IPotionItem GetPotionItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetPotionItem();
        }

        public IAmmoItem GetAmmoItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetAmmoItem();
        }

        public IBuildingItem GetBuildingItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetBuildingItem();
        }

        public IPetItem GetPetItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetPetItem();
        }

        public ISocketEnhancerItem GetSocketEnhancerItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetSocketEnhancerItem();
        }

        public IMountItem GetMountItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetMountItem();
        }

        public ISkillItem GetSkillItem(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetSkillItem();
        }

        public CalculatedItemBuff GetBuff(in CharacterItem data)
        {
            return GetOrMakeCache(data.id, in data)?.GetBuff();
        }
    }
}