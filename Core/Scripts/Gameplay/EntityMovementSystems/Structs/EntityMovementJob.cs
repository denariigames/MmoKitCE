using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace MultiplayerARPG
{
    public struct MovementJob : IJob
    {
        public NativeHashMap<uint, byte> previousModes;
        [ReadOnly] public GridData gridData;
        [ReadOnly] public NativeArray<uint> objectIds;
        [ReadOnly] public NativeHashMap<uint, MovementData> entityData;
        [ReadOnly] public UnityEngine.Vector3 receivingPlayerPosition;

        // output results to be sent to clients after processing all entities in the job.
        public NativeList<MovementResult> movementResults;
        public NativeHashMap<uint, byte> ModesResults;

        public void Execute()
        {
            for (int i = 0; i < objectIds.Length; i++)
            {
                uint objectId = objectIds[i];

                if (!entityData.TryGetValue(objectId, out var entity))
                    continue;

                //Calculate distance to player to determine compression mode.
                float dx = receivingPlayerPosition.x - entity.worldPosition.x;
                float dz = receivingPlayerPosition.z - entity.worldPosition.z;
                float distSq = dx * dx + dz * dz;

                // Get compression mode by distance, use lower compression for longer distance to save bandwidth, and use higher compression for shorter distance to make it more accurate.
                int previousMode = previousModes.ContainsKey(objectId) ? previousModes[objectId] : 0;
                int compressionMode = GridUtility.GetCompressionMode(distSq, previousMode, gridData.range);
                ModesResults[objectId] = (byte)compressionMode;

                float3 localPosition = GridUtility.GetCellLocalPosition(entity.worldPosition, gridData.cellSize, gridData.gridSize, out byte cellId);

                int bx, by, bz;

                switch (compressionMode)
                {
                    case 3: bx = 10; by = 4; bz = 10; break;
                    case 4: bx = 11; by = 10; bz = 11; break;
                    case 5: bx = 14; by = 12; bz = 14; break;
                    case 6: bx = 16; by = 16; bz = 16; break;
                    default: throw new Exception("Invalid mode");
                }

                ulong data = 0;
                int shift = 0;

                ushort qx = GridUtility.Quantize(localPosition.x, gridData.cellSize, bx);
                ushort qy = GridUtility.Quantize(localPosition.y, gridData.cellSize, by);
                ushort qz = GridUtility.Quantize(localPosition.z, gridData.cellSize, bz);

                data |= ((ulong)qx << shift); shift += bx;
                data |= ((ulong)qz << shift); shift += bz;
                data |= ((ulong)qy << shift); shift += by;

                int totalBits = bx + by + bz;

                int byteCount = (totalBits + 7) / 8;

                // convert mode (3–6 → 0–3)
                int modeBits = compressionMode - 3;

                // first byte: store mode in top 2 bits
                byte first = (byte)(
                    ((ulong)modeBits << 6) | (data & 0x3F)
                );

                movementResults.Add(new MovementResult
                {
                    objectId = objectId,
                    movementState = entity.movementState,
                    extraMovementState = entity.extraMovementState,
                    compressionMode = first,
                    cellId = cellId,
                    byteCount = byteCount,
                    data = data,
                    compressedYAndle = GridUtility.CompressAngle(entity.yAngle)
                });
            }
        }
    }
}