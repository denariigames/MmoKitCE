using UnityEngine;

namespace MultiplayerARPG
{
    public static partial class ServerBuildingHandlersExtensions
    {
        public static bool TryGetBuilding<T>(this IServerBuildingHandlers handlers, string id, out T building)
            where T : Component, IBuildingSaveData
        {
            building = null;
            IBuildingSaveData result;
            if (handlers.TryGetBuilding(id, out result))
            {
                building = result as T;
                return building != null;
            }
            return false;
        }
    }
}
