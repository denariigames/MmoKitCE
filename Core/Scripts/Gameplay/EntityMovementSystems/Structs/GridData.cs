using System;
using Unity.Mathematics;
using UnityEngine;

namespace MultiplayerARPG
{
    public struct GridData
    {
        public ushort cellSize;
        public ushort gridSize;
        public CompressionRange range;

        public GridData(ushort cellSize, ushort gridSize, CompressionRange range) : this()
        {
            this.cellSize = cellSize;
            this.gridSize = gridSize;
            this.range = range;
        }
    }
}

