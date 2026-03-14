#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceHarvestableEntity : AssetReferenceLiteNetLibBehaviour<HarvestableEntity>
    {
        public AssetReferenceHarvestableEntity(string guid) : base(guid)
        {
        }
    }
}
#endif