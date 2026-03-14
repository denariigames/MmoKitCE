using LiteNetLibManager;

namespace MultiplayerARPG
{
    public static partial class NetworkBehaviourExtensions
    {
        public static bool TryGetEntityByObjectId<T>(this LiteNetLibGameManager manager, uint objectId, out T result)
        {
            result = default;

            LiteNetLibIdentity identity;
            if (!manager.Assets.TryGetSpawnedObject(objectId, out identity))
                return false;

            result = identity.GetComponent<T>();
            if (result == null)
                return false;

            return true;
        }
    }
}
