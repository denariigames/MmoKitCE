using LiteNetLib.Utils;
using LiteNetLibManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultGridManagerComponent : MonoBehaviour, IBaseGridManagerComponenent
    {

        public static IBaseGridManagerComponenent Instance { get; private set; }

        [SerializeField]
        private bool isDisabled = false;
        public bool IsDisabled => isDisabled;

        [SerializeField]
        private ushort cellSize = 128;
        public ushort CellSize => cellSize;

        [SerializeField]
        [Range(0, 15)]
        private ushort gridSize = 15;
        public ushort GridSize => gridSize;

        [SerializeField]
        private CompressionRange compressionRange;

        public CompressionRange CompressionRange
        {
            get { return compressionRange; }
        }

        [SerializeField]
        private GridData gridData;

        public GridData GridData
        {
            get { return gridData; }
        }

        private Dictionary<byte, GridCell> _cellsById;

        public void SetupDynamicGrid()
        {
            Instance = this;

            if (IsDisabled)
                return;


            _cellsById = new Dictionary<byte, GridCell>();
            for (int z = 0; z < GridSize; z++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    byte id = (byte)(x + z * GridSize);

                    _cellsById[id] = new GridCell
                    {
                        Id = id,
                        GridX = x,
                        GridZ = z
                    };
                }
            }

            gridData = new GridData(cellSize, gridSize, compressionRange);

            Logging.Log($"[DefaultGridManagerComponent] Dynamic grid setup completed with {_cellsById.Count} cells.");
        }

        public Vector3 GetWorldPosition(byte cellId, Vector3 position)
        {
            GetCell(cellId, out GridCell gridCell);
            float offset = (GridSize * CellSize) * 0.5f;
            return new Vector3(
                (gridCell.GridX * cellSize) - offset + position.x,
                position.y,
                (gridCell.GridZ * cellSize) - offset + position.z);
        }

        void GetCell(byte id, out GridCell gridCell)
        {
            if (_cellsById.TryGetValue(id, out GridCell cell))
            {
                gridCell = cell;
                return;
            }
            else
            {
                gridCell = new GridCell();
                return;
            }
        }

        public bool WriteEntityServerState(NetDataWriter writer, MovementResult movementResult, List<EntityMovementForceApplier> forceAppliers)
        {
            if (IsDisabled || writer == null)
                return false;

            writer.PutPackedUInt(movementResult.movementState);
            writer.Put(movementResult.extraMovementState);
            writer.Put(movementResult.cellId);
            writer.Put(movementResult.compressionMode);

            ulong data = movementResult.data;
            data >>= 6;

            for (int i = 1; i < movementResult.byteCount; i++)
            {
                writer.Put((byte)(data & 0xFF));
                data >>= 8;
            }
            writer.Put(movementResult.compressedYAndle);
            writer.PutList(forceAppliers);

            return true;
        }
    }

    [Serializable]
    public struct CompressionRange
    {
        public float HighestPrecisionRange;
        public float HighPrecisionRange;
        public float MediumPrecisionRange;
        public float LowPrecisionRange;
    }

    public static class GridUtility
    {

        public static ushort Quantize(float value, ushort cellSize, int bits)
        {
            value = Math.Clamp(value, 0f, cellSize - 0.0001f);

            float normalized = value / cellSize;

            int maxInt = (1 << bits) - 1;
            return (ushort)(normalized * maxInt);
        }
        public static float3 GetCellLocalPosition(float3 worldPosition, float cellSize, ushort gridSize, out byte cellId)
        {
            float offset = (gridSize * cellSize) * 0.5f;

            // World → Cell
            ushort cellX = (ushort)Math.Floor((worldPosition.x + offset) / cellSize);
            ushort cellZ = (ushort)Math.Floor((worldPosition.z + offset) / cellSize);

            cellX = (ushort)Math.Clamp(cellX, 0, gridSize - 1);
            cellZ = (ushort)Math.Clamp(cellZ, 0, gridSize - 1);

            cellId = (byte)(cellX + cellZ * gridSize);

            return new Vector3(
               (worldPosition.x + offset) - (cellX * cellSize),
               worldPosition.y,
               (worldPosition.z + offset) - (cellZ * cellSize));
        }

        public static byte CompressAngle(float angle)
        {
            // normalize to 0–360
            angle %= 360f;
            if (angle < 0) angle += 360f;

            return (byte)(angle * (255f / 360f));
        }

        public static int GetCompressionMode(float distSq, int previousMode, CompressionRange compressionRange)
        {
            switch (previousMode)
            {
                case 6:
                    if (distSq > compressionRange.HighPrecisionRange * compressionRange.HighPrecisionRange) return 5;
                    return 6;

                case 5:
                    if (distSq < compressionRange.HighestPrecisionRange * compressionRange.HighestPrecisionRange) return 6;
                    if (distSq > compressionRange.MediumPrecisionRange * compressionRange.MediumPrecisionRange) return 4;
                    return 5;

                case 4:
                    if (distSq < compressionRange.HighPrecisionRange * compressionRange.HighPrecisionRange) return 5;
                    if (distSq > compressionRange.LowPrecisionRange * compressionRange.LowPrecisionRange) return 3;
                    return 4;

                default:
                    if (distSq < compressionRange.MediumPrecisionRange * compressionRange.MediumPrecisionRange) return 4;
                    return 3;
            }
        }
    }
}
