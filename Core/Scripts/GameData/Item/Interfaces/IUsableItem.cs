namespace MultiplayerARPG
{
    public partial interface IUsableItem : IItem, ICustomAimController, IItemWithRequirement
    {
        /// <summary>
        /// Cooldown duration before it is able to use again
        /// </summary>
        float UseItemCooldown { get; }
        bool UseItem(BaseCharacterEntity characterEntity, int itemIndex, CharacterItem characterItem);

    }
}
