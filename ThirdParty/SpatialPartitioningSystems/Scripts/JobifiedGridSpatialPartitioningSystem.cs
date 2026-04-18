// CE scalability: #47
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
        private int _spatialObjectsCapacity;
        private NativeParallelMultiHashMap<int, SpatialObject> _cellToObjects;
        private NativeList<SpatialObject> _queryResults;

        private readonly int _gridSizeX;
        private readonly int _gridSizeY;
        private readonly int _gridSizeZ;
        private readonly bool _disableXAxis;
        private readonly bool _disableYAxis;
        private readonly bool _disableZAxis;
        private readonly float _cellSize;
        private readonly float3 _worldMin;

        public NativeList<SpatialObject> QueryResults => _queryResults;

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

            _cellToObjects = new NativeParallelMultiHashMap<int, SpatialObject>(maxObjects, Allocator.Persistent);
            _queryResults = new NativeList<SpatialObject>(maxObjects, Allocator.Persistent);
            _spatialObjectsCapacity = 0;
        }

        public void Dispose()
        {
            if (_spatialObjects.IsCreated)
                _spatialObjects.Dispose();

            if (_cellToObjects.IsCreated)
                _cellToObjects.Dispose();

            if (_queryResults.IsCreated)
                _queryResults.Dispose();
        }

        ~JobifiedGridSpatialPartitioningSystem()
        {
            Dispose();
        }

        public void UpdateGrid(List<SpatialObject> spatialObjects)
        {
            int count = spatialObjects.Count;

            if (count > _spatialObjectsCapacity)
            {
                if (_spatialObjects.IsCreated)
                    _spatialObjects.Dispose();
                _spatialObjectsCapacity = Mathf.NextPowerOfTwo(Mathf.Max(count, 64));
                _spatialObjects = new NativeArray<SpatialObject>(_spatialObjectsCapacity, Allocator.Persistent);
            }

            for (int i = 0; i < count; i++)
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

            _cellToObjects.Clear();

            var updateJob = new UpdateGridJob
            {
                Objects = _spatialObjects.GetSubArray(0, count),
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

            var handle = updateJob.Schedule(count, 64);
            handle.Complete();
        }

        public void QuerySphere(Vector3 position, float radius)
        {
            _queryResults.Clear();

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
                Results = _queryResults,
            };

            queryJob.Run();
        }

        public void QueryBox(Vector3 center, Vector3 extents)
        {
            _queryResults.Clear();

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
                Results = _queryResults
            };

            queryJob.Run();
        }
    }
}