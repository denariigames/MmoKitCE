using UnityEngine;

namespace MultiplayerARPG
{
    public interface IEntityMovementComponent : IEntityMovement
    {
        bool enabled { get; set; }
        Bounds GetMovementBounds();
    }
}
