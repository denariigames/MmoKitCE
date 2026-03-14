#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceUINpcEntity : AssetReferenceComponent<UINpcEntity>
    {
        public AssetReferenceUINpcEntity(string guid) : base(guid)
        {
        }
    }
}
#endif