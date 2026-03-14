#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceMissileDamageEntity : AssetReferenceComponent<MissileDamageEntity>
    {
        public AssetReferenceMissileDamageEntity(string guid) : base(guid)
        {
        }
    }
}
#endif