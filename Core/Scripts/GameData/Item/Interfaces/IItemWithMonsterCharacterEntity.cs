namespace MultiplayerARPG
{
    public interface IItemWithMonsterCharacterEntity : IItem
    {
        /// <summary>
        /// Monster entity for this item
        /// </summary>
        BaseMonsterCharacterEntity MonsterCharacterEntity { get; }
#if !DISABLE_ADDRESSABLES
        AssetReferenceBaseMonsterCharacterEntity AddressableMonsterCharacterEntity { get; }
#endif
    }
}