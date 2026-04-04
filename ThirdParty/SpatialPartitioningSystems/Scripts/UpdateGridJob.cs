using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Insthync.SpatialPartitioningSystems
{
    [BurstCompile]
    public struct UpdateGridJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<SpatialObject> Objects;
        public NativeParallelMultiHashMap<int, SpatialObject>.ParallelWriter CellToObjects;
        public float CellSize;
        public float3 WorldMin;
        public int GridSizeX;
        public int GridSizeY;
        public int GridSizeZ;
        public bool DisableXAxis;
        public bool DisableYAxis;
        public bool DisableZAxis;

        public void Execute(int index)
        {
            SpatialObject obj = Objects[index];

            // Only use center position to determine which cell to store into
            int3 cellIndex = QueryFunctions.GetCellIndex(obj.position, WorldMin, CellSize, DisableXAxis, DisableYAxis, DisableZAxis);

            // Clamp to grid bounds
            cellIndex = math.clamp(cellIndex, int3.zero, new int3(GridSizeX - 1, GridSizeY - 1, GridSizeZ - 1));

            int flatIndex = QueryFunctions.GetFlatIndex(cellIndex, GridSizeX, GridSizeY);
            CellToObjects.Add(flatIndex, obj);
        }
    }
}