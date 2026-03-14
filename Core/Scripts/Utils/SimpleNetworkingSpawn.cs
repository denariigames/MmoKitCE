using LiteNetLibManager;
using UnityEngine;

namespace UtilsComponents
{
    public class SimpleNetworkingSpawn : MonoBehaviour
    {
        public LiteNetLibIdentity prefab;

        public void Spawn()
        {
            if (prefab == null)
                return;
            LiteNetLibGameManager manager = FindFirstObjectByType<LiteNetLibGameManager>();
            if (manager == null || !manager.IsServer)
                return;
            manager.Assets.NetworkSpawn(prefab.HashAssetId, transform.position, transform.rotation);
        }
    }
}