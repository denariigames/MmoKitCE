#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceProjectileEffect : AssetReferenceComponent<ProjectileEffect>
    {
        public AssetReferenceProjectileEffect(string guid) : base(guid)
        {
        }
    }
}
#endif