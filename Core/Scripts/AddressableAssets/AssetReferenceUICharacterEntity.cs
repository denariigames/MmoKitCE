#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceUICharacterEntity : AssetReferenceComponent<UICharacterEntity>
    {
        public AssetReferenceUICharacterEntity(string guid) : base(guid)
        {
        }
    }
}
#endif