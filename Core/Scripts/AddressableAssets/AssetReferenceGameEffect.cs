#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceGameEffect : AssetReferenceComponent<GameEffect>
    {
        public AssetReferenceGameEffect(string guid) : base(guid)
        {
        }
    }
}
#endif