#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceBaseCharacterEntity : AssetReferenceLiteNetLibBehaviour<BaseCharacterEntity>
    {
        public AssetReferenceBaseCharacterEntity(string guid) : base(guid)
        {
        }
    }
}
#endif