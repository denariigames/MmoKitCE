using Unity.Profiling;

namespace MultiplayerARPG
{
    public class CharacterSummonCacheManager : BaseCacheManager<CharacterSummon, CharacterSummonCacheData>
    {
        protected static ProfilerMarker s_profilerMarker = new ProfilerMarker("CharacterSummonCacheManager");
        public override ProfilerMarker ProfilerMarker => s_profilerMarker;

        public BaseMonsterCharacterEntity GetEntity(in CharacterSummon data)
        {
            return GetOrMakeCache(data.id, in data)?.CacheEntity;
        }

        public void SetEntity(in CharacterSummon data, BaseMonsterCharacterEntity value)
        {
            CharacterSummonCacheData cachedData = GetOrMakeCache(data.id, in data);
            if (cachedData != null)
                cachedData.CacheEntity = value;
        }

        public BaseSkill GetSkill(in CharacterSummon data)
        {
            return GetOrMakeCache(data.id, in data)?.GetSkill();
        }

        public IPetItem GetPetItem(in CharacterSummon data)
        {
            return GetOrMakeCache(data.id, in data)?.GetPetItem();
        }

        /// <summary>
        /// Return `TRUE` if it is addressable
        /// </summary>
        /// <param name="data"></param>
        /// <param name="prefab"></param>
        /// <param name="addressablePrefab"></param>
        /// <returns></returns>
        public bool GetPrefab(in CharacterSummon data
            , out BaseMonsterCharacterEntity prefab
#if !DISABLE_ADDRESSABLES
            , out AssetReferenceBaseMonsterCharacterEntity addressablePrefab
#endif
            )
        {
            prefab = null;
#if !DISABLE_ADDRESSABLES
            addressablePrefab = null;
#endif
            CharacterSummonCacheData cachedData = GetOrMakeCache(data.id, in data);
            if (cachedData != null)
            {
                return cachedData.GetPrefab(out prefab
#if !DISABLE_ADDRESSABLES
                    , out addressablePrefab
#endif
                    );
            }
            return false;
        }

        public CalculatedBuff GetBuff(in CharacterSummon data)
        {
            CharacterSummonCacheData cachedData = GetOrMakeCache(data.id, in data);
            if (cachedData != null)
                return cachedData.GetBuff();
            return null;
        }
    }
}