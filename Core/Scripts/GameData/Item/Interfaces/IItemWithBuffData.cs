namespace MultiplayerARPG
{
    public interface IItemWithBuffData : IItem
    {
        /// <summary>
        /// Buff data for this item
        /// </summary>
        Buff BuffData { get; }
    }
}