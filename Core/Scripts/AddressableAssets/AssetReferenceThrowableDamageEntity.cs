#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceThrowableDamageEntity : AssetReferenceComponent<ThrowableDamageEntity>
    {
        public AssetReferenceThrowableDamageEntity(string guid) : base(guid)
        {
        }
    }
}
#endif