using Unity.Profiling;

namespace MultiplayerARPG
{
    public class CharacterBuffCacheManager : BaseCacheManager<CharacterBuff, CharacterBuffCacheData>
    {
        protected static ProfilerMarker s_profilerMarker = new ProfilerMarker("CharacterBuffCacheManager");
        public override ProfilerMarker ProfilerMarker => s_profilerMarker;
        public BaseSkill GetSkill(in CharacterBuff data)
        {
            return GetOrMakeCache(data.id, in data)?.GetSkill();
        }

        public BaseItem GetItem(in CharacterBuff data)
        {
            return GetOrMakeCache(data.id, in data)?.GetItem();
        }

        public GuildSkill GetGuildSkill(in CharacterBuff data)
        {
            return GetOrMakeCache(data.id, in data)?.GetGuildSkill();
        }

        public StatusEffect GetStatusEffect(in CharacterBuff data)
        {
            return GetOrMakeCache(data.id, in data)?.GetStatusEffect();
        }

        public CalculatedBuff GetBuff(in CharacterBuff data)
        {
            return GetOrMakeCache(data.id, in data)?.GetBuff();
        }

        public string GetKey(in CharacterBuff data)
        {
            return GetOrMakeCache(data.id, in data)?.GetKey();
        }

        public void SetApplier(in CharacterBuff data, EntityInfo buffApplier, CharacterItem buffApplierWeapon)
        {
            GetOrMakeCache(data.id, in data)?.SetApplier(buffApplier, buffApplierWeapon);
        }

        public EntityInfo GetBuffApplier(in CharacterBuff data)
        {
            return GetOrMakeCache(data.id, in data)?.BuffApplier ?? EntityInfo.Empty;
        }

        public CharacterItem GetBuffApplierWeapon(in CharacterBuff data)
        {
            return GetOrMakeCache(data.id, in data)?.BuffApplierWeapon ?? CharacterItem.Empty;
        }
    }
}
