#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceBaseCharacterModel : AssetReferenceComponent<BaseCharacterModel>
    {
        public AssetReferenceBaseCharacterModel(string guid) : base(guid)
        {
        }
    }
}
#endif