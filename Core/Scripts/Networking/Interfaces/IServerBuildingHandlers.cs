using System.Collections.Generic;

namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerBuildingHandlers
    {
        /// <summary>
        /// Count placed buildings
        /// </summary>
        int BuildingsCount { get; }

        /// <summary>
        /// Get placed buildings
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBuildingSaveData> GetBuildings();

        /// <summary>
        /// Get character from server's collection
        /// </summary>
        /// <param name="id"></param>
        /// <param name="building"></param>
        /// <returns></returns>
        bool TryGetBuilding(string id, out IBuildingSaveData building);

        /// <summary>
        /// Add character to server's collection
        /// </summary>
        /// <param name="id"></param>
        /// <param name="building"></param>
        /// <returns></returns>
        bool AddBuilding(string id, IBuildingSaveData building);

        /// <summary>
        /// Remove character from server's collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool RemoveBuilding(string id);

        /// <summary>
        /// Clear server's collection (and other relates variables)
        /// </summary>
        void ClearBuildings();

        /// <summary>
        /// Count buildings which build by player
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        int CountPlayerBuildings(string characterId, int entityId);
    }
}
