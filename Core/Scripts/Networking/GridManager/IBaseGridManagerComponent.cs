using LiteNetLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public interface IBaseGridManagerComponenent
    {
        bool IsDisabled { get; }
        ushort CellSize { get; }
        ushort GridSize { get; }
        CompressionRange CompressionRange { get; }

        GridData GridData { get; }
        void SetupDynamicGrid();

        Vector3 GetWorldPosition(byte cellId, Vector3 position);

        bool WriteEntityServerState(NetDataWriter writer, MovementResult movementResult, List<EntityMovementForceApplier> forceAppliers);
    }
}

