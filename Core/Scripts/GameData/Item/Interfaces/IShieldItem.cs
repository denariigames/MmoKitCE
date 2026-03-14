namespace MultiplayerARPG
{
    public partial interface IShieldItem : IDefendEquipmentItem
    {
        /// <summary>
        /// These models will be instantiated when this item not being equipped or sheathed
        /// </summary>
        EquipmentModel[] SheathModels { get; }
    }
}
