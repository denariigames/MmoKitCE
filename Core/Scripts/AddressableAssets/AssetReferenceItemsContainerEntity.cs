#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceItemsContainerEntity : AssetReferenceLiteNetLibBehaviour<ItemsContainerEntity>
    {
        public AssetReferenceItemsContainerEntity(string guid) : base(guid)
        {
        }
    }
}
#endif