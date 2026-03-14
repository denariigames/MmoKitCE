#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceVehicleEntity : AssetReferenceLiteNetLibBehaviour<VehicleEntity>
    {
        public AssetReferenceVehicleEntity(string guid) : base(guid)
        {
        }
    }
}
#endif