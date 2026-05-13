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
        private NativeList<SpatialObject> _spatialObjects;
        private NativeParallelMultiHashMap<int, SpatialObject> _cellToObjects;

        private readonly int _gridSizeX;
        private readonly int _gridSizeY;
        private readonly int _gridSizeZ;
        private readonly bool _disableXAxis;
        private readonly bool _disableYAxis;
        private readonly bool _disableZAxis;
        private readonly float _cellSize;
        private readonly float3 _worldMin;
        private JobHandle _jobHandle;

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

            _spatialObjects = new NativeList<SpatialObject>(1024, Allocator.Persistent);
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

        public void ClearObjects()
        {
            _spatialObjects.Clear();
        }

        public void AddObjectToGrid(SpatialObject spatialObject)
        {
            int index = _spatialObjects.Length;
            float3 postition = spatialObject.position;
            if (_disableXAxis)
                postition.x = 0f;
            if (_disableYAxis)
                postition.y = 0f;
            if (_disableZAxis)
                postition.z = 0f;
            spatialObject.position = postition;
            spatialObject.objectIndex = index;
            _spatialObjects.Add(spatialObject);
        }

        public JobHandle UpdateGrid()
        {
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

            _jobHandle = updateJob.Schedule(_spatialObjects.Length, 64);
            return _jobHandle;
        }

        public void Complete()
        {
            _jobHandle.Complete();
        }

        public JobHandle QuerySphere(Vector3 position, float radius, NativeList<SpatialObject> results)
        {
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

            _jobHandle = queryJob.Schedule(_jobHandle);
            return _jobHandle;
        }

        public JobHandle QueryBox(Vector3 center, Vector3 extents, NativeList<SpatialObject> results)
        {
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
                Results = results,
            };

            _jobHandle = queryJob.Schedule(_jobHandle);
            return _jobHandle;
        }
    }
}