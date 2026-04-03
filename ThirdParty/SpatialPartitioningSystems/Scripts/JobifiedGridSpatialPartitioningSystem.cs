using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;
using System.Collections.Generic;

namespace Insthync.SpatialPartitioningSystems
{
    [BurstCompile]
    public class JobifiedGridSpatialPartitioningSystem : System.IDisposable
    {
        private NativeArray<SpatialObject> _spatialObjects;
        private NativeParallelMultiHashMap<int, SpatialObject> _cellToObjects;

        private readonly int _gridSizeX;
        private readonly int _gridSizeY;
        private readonly int _gridSizeZ;
        private readonly bool _disableXAxis;
        private readonly bool _disableYAxis;
        private readonly bool _disableZAxis;
        private readonly float _cellSize;
        private readonly float3 _worldMin;

        public JobifiedGridSpatialPartitioningSystem(Bounds bounds, float cellSize, int maxObjects, bool disableXAxis, bool disableYAxis, bool disableZAxis)
        {
            _cellSize = cellSize;

            _disableXAxis = disableXAxis;
            _disableYAxis = disableYAxis;
            _disableZAxis = disableZAxis;

            _gridSizeX = disableXAxis ? 1 : Mathf.CeilToInt(bounds.size.x / cellSize);
            _gridSizeY = disableYAxis ? 1 : Mathf.CeilToInt(bounds.size.y / cellSize);
            _gridSizeZ = disableZAxis ? 1 : Mathf.CeilToInt(bounds.size.z / cellSize);

            _worldMin = new float3(
                disableXAxis ? 0 : bounds.min.x,
                disableYAxis ? 0 : bounds.min.y,
                disableZAxis ? 0 : bounds.min.z);

            _cellToObjects = new NativeParallelMultiHashMap<int, SpatialObject>(maxObjects, Allocator.Persistent); // Multiplied by 8 because objects can span multiple cells
        }

        public void Dispose()
        {
            if (_spatialObjects.IsCreated)
                _spatialObjects.Dispose();

            if (_cellToObjects.IsCreated)
                _cellToObjects.Dispose();
        }

        ~JobifiedGridSpatialPartitioningSystem()
        {
            Dispose();
        }

        public void UpdateGrid(List<SpatialObject> spatialObjects)
        {
            // Convert to SpatialObjects
            _spatialObjects = new NativeArray<SpatialObject>(spatialObjects.Count, Allocator.TempJob);

            for (int i = 0; i < spatialObjects.Count; i++)
            {
                SpatialObject spatialObject = spatialObjects[i];
                float3 postition = spatialObject.position;
                if (_disableXAxis)
                    postition.x = 0f;
                if (_disableYAxis)
                    postition.y = 0f;
                if (_disableZAxis)
                    postition.z = 0f;
                spatialObject.position = postition;
                spatialObject.objectIndex = i;
                _spatialObjects[i] = spatialObject;
            }

            // Clear previous grid data
            _cellToObjects.Clear();

            // Create and schedule update job
            var updateJob = new UpdateGridJob
            {
                Objects = _spatialObjects,
                CellToObjects = _cellToObjects.AsParallelWriter(),
                CellSize = _cellSize,
                WorldMin = _worldMin,
                GridSizeX = _gridSizeX,
                GridSizeY = _gridSizeY,
                GridSizeZ = _gridSizeZ,
                DisableXAxis = _disableXAxis,
                DisableYAxis = _disableYAxis,
                DisableZAxis = _disableZAxis
            };

            var handle = updateJob.Schedule(_spatialObjects.Length, 64);
            handle.Complete();
            _spatialObjects.Dispose();
        }

        public NativeList<SpatialObject> QuerySphere(Vector3 position, float radius)
        {
            var results = new NativeList<SpatialObject>(Allocator.TempJob);

            var queryJob = new QuerySphereJob
            {
                CellToObjects = _cellToObjects,
                QueryCenter = position,
                QueryRadius = radius,
                CellSize = _cellSize,
                WorldMin = _worldMin,
                GridSizeX = _gridSizeX,
                GridSizeY = _gridSizeY,
                GridSizeZ = _gridSizeZ,
                DisableXAxis = _disableXAxis,
                DisableYAxis = _disableYAxis,
                DisableZAxis = _disableZAxis,
                Results = results,
            };

            queryJob.Run();
            return results;
        }

        public NativeList<SpatialObject> QueryBox(Vector3 center, Vector3 extents)
        {
            var results = new NativeList<SpatialObject>(Allocator.TempJob);

            var queryJob = new QueryBoxJob
            {
                CellToObjects = _cellToObjects,
                QueryCenter = center,
                QueryExtents = extents,
                CellSize = _cellSize,
                WorldMin = _worldMin,
                GridSizeX = _gridSizeX,
                GridSizeY = _gridSizeY,
                GridSizeZ = _gridSizeZ,
                DisableXAxis = _disableXAxis,
                DisableYAxis = _disableYAxis,
                DisableZAxis = _disableZAxis,
                Results = results
            };

            queryJob.Run();
            return results;
        }
    }
}