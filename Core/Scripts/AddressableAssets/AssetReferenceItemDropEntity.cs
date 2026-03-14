#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceItemDropEntity : AssetReferenceLiteNetLibBehaviour<ItemDropEntity>
    {
        public AssetReferenceItemDropEntity(string guid) : base(guid)
        {
        }
    }
}
#endif