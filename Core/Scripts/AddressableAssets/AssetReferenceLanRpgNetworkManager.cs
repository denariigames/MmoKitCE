#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceLanRpgNetworkManager : AssetReferenceComponent<LanRpgNetworkManager>
    {
        public AssetReferenceLanRpgNetworkManager(string guid) : base(guid)
        {
        }
    }
}
#endif