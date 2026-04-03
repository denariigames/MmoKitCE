using LiteNetLibManager;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private int gridSize = 15;
        public int GridSize => gridSize;

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
            Logging.Log($"[DefaultGridManagerComponent] Dynamic grid setup completed with {_cellsById.Count} cells.");
        }

        public byte GetCellId(Vector3 pos)
        {
            float offset = (GridSize * CellSize) * 0.5f;

            // World → Cell
            int cellX = (int)Math.Floor((pos.x + offset) / CellSize);
            int cellZ = (int)Math.Floor((pos.z + offset) / CellSize);

            cellX = Math.Clamp(cellX, 0, gridSize - 1);
            cellZ = Math.Clamp(cellZ, 0, gridSize - 1);

            return (byte)(cellX + cellZ * GridSize);
        }

        public void GetCell(byte id, out GridCell gridCell)
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

        public Vector3 GetCellLocalPosition(byte cellId, Vector3 position)
        {
            GetCell(cellId, out GridCell gridCell);
            float offset = (GridSize * CellSize) * 0.5f;

            return new Vector3(
               (position.x + offset) - (gridCell.GridX * CellSize),
               position.y,
               (position.z + offset) - (gridCell.GridZ * CellSize));
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
    }
}
