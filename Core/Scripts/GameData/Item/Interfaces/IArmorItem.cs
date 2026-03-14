namespace MultiplayerARPG
{
    public partial interface IArmorItem : IDefendEquipmentItem
    {
        /// <summary>
        /// Armor type data
        /// </summary>
        ArmorType ArmorType { get; }
    }
}
