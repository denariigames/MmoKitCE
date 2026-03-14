namespace MultiplayerARPG
{
    public interface ICustomSummonManager
    {
        /// <summary>
        /// Return `TRUE` if it is addressable
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="addressablePrefab"></param>
        /// <returns></returns>
        bool GetPrefab(
            out BaseMonsterCharacterEntity prefab
#if !DISABLE_ADDRESSABLES
            , out AssetReferenceBaseMonsterCharacterEntity addressablePrefab
#endif
            );
        void UnSummon(CharacterSummon characterSummon, BaseCharacterEntity summoner);
        void Update(CharacterSummon characterSummon, float deltaTime);
        bool ShouldRemove(CharacterSummon characterSummon, ICharacterData characterData);
    }
}
