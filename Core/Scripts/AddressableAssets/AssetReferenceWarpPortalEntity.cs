#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceWarpPortalEntity : AssetReferenceLiteNetLibBehaviour<WarpPortalEntity>
    {
        public AssetReferenceWarpPortalEntity(string guid) : base(guid)
        {
        }
    }
}
#endif