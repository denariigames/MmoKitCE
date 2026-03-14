namespace MultiplayerARPG
{
    public interface IGameData
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Hashed Id from string to integer
        /// </summary>
        int DataId { get; }

        /// <summary>
        /// Validate game data, return true if has changes
        /// </summary>
        /// <returns></returns>
        bool Validate();
        /// <summary>
        /// Prepare data that relates to this game data
        /// </summary>
        void PrepareRelatesData();
    }
}