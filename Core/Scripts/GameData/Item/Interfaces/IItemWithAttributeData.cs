namespace MultiplayerARPG
{
    public interface IItemWithAttributeData : IItem
    {
        /// <summary>
        /// Attribute data for this item
        /// </summary>
        Attribute AttributeData { get; }

        /// <summary>
        /// Attribute's amount for this item
        /// </summary>
        float AttributeAmount { get; }
    }
}