#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceGameInstance : AssetReferenceComponent<GameInstance>
    {
        public AssetReferenceGameInstance(string guid) : base(guid)
        {
        }
    }
}
#endif