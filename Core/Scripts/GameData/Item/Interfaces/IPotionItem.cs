namespace MultiplayerARPG
{
    public partial interface IPotionItem : IUsableItem, IItemWithBuffData
    {
        /// <summary>
        /// Key for auto use setting saving for this item
        /// </summary>
        string AutoUseKey { get; }
    }
}
