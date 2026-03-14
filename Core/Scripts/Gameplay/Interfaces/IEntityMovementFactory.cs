using UnityEngine;

namespace MultiplayerARPG
{
    public interface IEntityMovementFactory
    {
        string Name { get; }
        DimensionType DimensionType { get; }
        bool ValidateSourceObject(GameObject obj);
        IEntityMovementComponent Setup(GameObject obj, ref Bounds bounds);
    }
}
