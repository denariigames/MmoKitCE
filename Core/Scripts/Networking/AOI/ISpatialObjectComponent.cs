using Insthync.SpatialPartitioningSystems;
using Unity.Mathematics;

namespace MultiplayerARPG
{
    public interface ISpatialObjectComponent
    {
        uint SpatialObjectId { get; set; }
        bool SpatialObjectEnabled { get; }
        float3 SpatialObjectPosition { get; }
        SpatialObjectShape SpatialObjectShape { get; }
        // Sphere
        float SpatialObjectRadius { get; }
        // Box
        float3 SpatialObjectExtents { get; }

        void ClearSubscribers();
        void AddSubscriber(uint id);
    }
}
