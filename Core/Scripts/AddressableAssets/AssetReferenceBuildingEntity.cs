#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceBuildingEntity : AssetReferenceLiteNetLibBehaviour<BuildingEntity>
    {
        public AssetReferenceBuildingEntity(string guid) : base(guid)
        {
        }
    }
}
#endif