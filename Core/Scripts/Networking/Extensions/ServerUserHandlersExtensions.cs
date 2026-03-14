using UnityEngine;

namespace MultiplayerARPG
{
    public static partial class ServerUserHandlersExtensions
    {
        public static bool TryGetPlayerCharacter<T>(this IServerUserHandlers handlers, long connectionId, out T playerCharacter)
            where T : Component, IPlayerCharacterData
        {
            playerCharacter = null;
            IPlayerCharacterData result;
            if (handlers.TryGetPlayerCharacter(connectionId, out result))
            {
                playerCharacter = result as T;
                return playerCharacter != null;
            }
            return false;
        }

        public static bool TryGetPlayerCharacterById<T>(this IServerUserHandlers handlers, string id, out T playerCharacter)
            where T : Component, IPlayerCharacterData
        {
            playerCharacter = null;
            IPlayerCharacterData result;
            if (!string.IsNullOrEmpty(id) && handlers.TryGetPlayerCharacterById(id, out result))
            {
                playerCharacter = result as T;
                return playerCharacter != null;
            }
            return false;
        }

        public static bool TryGetPlayerCharacterByName<T>(this IServerUserHandlers handlers, string name, out T playerCharacter)
            where T : Component, IPlayerCharacterData
        {
            playerCharacter = null;
            IPlayerCharacterData result;
            if (!string.IsNullOrEmpty(name) && handlers.TryGetPlayerCharacterByName(name, out result))
            {
                playerCharacter = result as T;
                return playerCharacter != null;
            }
            return false;
        }

        public static bool TryGetConnectionIdByName(this IServerUserHandlers handlers, string name, out long connectionId)
        {
            connectionId = -1;
            IPlayerCharacterData result;
            return !string.IsNullOrEmpty(name) && handlers.TryGetPlayerCharacterByName(name, out result) && handlers.TryGetConnectionIdById(result.Id, out connectionId);
        }
    }
}
