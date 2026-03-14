namespace MultiplayerARPG
{
    public partial interface IDefendEquipmentItem : IEquipmentItem
    {
        /// <summary>
        /// Increasing armors stats while equipping this item
        /// </summary>
        ArmorIncremental ArmorAmount { get; }
    }
}
