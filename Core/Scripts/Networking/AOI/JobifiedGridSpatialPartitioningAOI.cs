// ce stability: #46

using Insthync.SpatialPartitioningSystems;
using LiteNetLibManager;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine;

namespace MultiplayerARPG
{
    public class JobifiedGridSpatialPartitioningAOI : BaseInterestManager
    {
        protected static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("JobifiedGridSpatialPartitioningAOI - Update");

        public float cellSize = 64f;
        public int maxObjects = 10000;
        [Tooltip("Update every ? seconds")]
        public float updateInterval = 1.0f;
        public Vector3 bufferedCells = Vector3.one;
        public Color boundsGizmosColor = Color.green;

        private JobifiedGridSpatialPartitioningSystem _system;
        private float _updateCountDown;
        private Bounds _bounds;
        private List<SpatialObject> _spatialObjects = new List<SpatialObject>();
        private Dictionary<uint, HashSet<uint>> _playerSubscribings = new Dictionary<uint, HashSet<uint>>();
        private readonly Queue<HashSet<uint>> _hashSetPool = new Queue<HashSet<uint>>();
        private readonly HashSet<uint> _fallbackSubscribings = new HashSet<uint>();
        private HashSet<uint> _alwaysVisibleObjects = new HashSet<uint>();
        private bool _didLogMissingSystemWarning;

        public bool IsSystemReady => _system != null;

        private void OnDrawGizmosSelected()
        {
            Color color = Gizmos.color;
            Gizmos.color = boundsGizmosColor;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            Gizmos.color = color;
        }

        private void OnDestroy()
        {
            ReleaseSystem();
        }

        private void ReleaseSystem()
        {
            if (_system == null)
                return;
            _system.Dispose();
            _system = null;
        }

        public override void Setup(LiteNetLibGameManager manager)
        {
            base.Setup(manager);
            manager.Assets.onLoadSceneFinish.RemoveListener(OnLoadSceneFinish);
            PrepareSystem();
            manager.Assets.onLoadSceneFinish.AddListener(OnLoadSceneFinish);
        }

        private void OnLoadSceneFinish(string sceneName, bool isAdditive, bool isOnline, float progress)
        {
            if (!IsServer || !isOnline)
            {
                ReleaseSystem();
                _didLogMissingSystemWarning = false;
                return;
            }
            // Full online scene load is already handled by OnServerOnlineSceneLoaded().
            // Keep this callback for additive scene changes that can alter AOI bounds.
            if (!isAdditive)
                return;
            PrepareSystem();
        }

        public void PrepareSystem()
        {
            if (!IsServer || !Manager.ServerSceneInfo.HasValue)
            {
                ReleaseSystem();
                _didLogMissingSystemWarning = false;
                return;
            }
            ReleaseSystem();
            var mapBounds = GenericUtils.GetComponentsFromAllLoadedScenes<AOIMapBounds>(true);
            if (mapBounds.Count > 0)
            {
                _bounds = mapBounds[0].GetBounds();
                for (int i = 0; i < mapBounds.Count; ++i)
                {
                    _bounds.Encapsulate(mapBounds[i].GetBounds());
                }
                _bounds.extents += bufferedCells * cellSize * 2;
                switch (GameInstance.Singleton.DimensionType)
                {
                    case DimensionType.Dimension3D:
                        _system = new JobifiedGridSpatialPartitioningSystem(_bounds, cellSize, maxObjects, false, true, false);
                        break;
                    case DimensionType.Dimension2D:
                        _system = new JobifiedGridSpatialPartitioningSystem(_bounds, cellSize, maxObjects, false, false, true);
                        break;
                }
            }
            else
            {
                switch (GameInstance.Singleton.DimensionType)
                {
                    case DimensionType.Dimension3D:
                        var collider3Ds = GenericUtils.GetComponentsFromAllLoadedScenes<Collider>(true);
                        if (collider3Ds.Count > 0)
                        {
                            _bounds = collider3Ds[0].bounds;
                            for (int i = 1; i < collider3Ds.Count; ++i)
                            {
                                _bounds.Encapsulate(collider3Ds[i].bounds);
                            }
                            _bounds.extents += bufferedCells * cellSize * 2;
                            _system = new JobifiedGridSpatialPartitioningSystem(_bounds, cellSize, maxObjects, false, true, false);
                        }
                        break;
                    case DimensionType.Dimension2D:
                        var collider2Ds = GenericUtils.GetComponentsFromAllLoadedScenes<Collider2D>(true);
                        if (collider2Ds.Count > 0)
                        {
                            _bounds = collider2Ds[0].bounds;
                            for (int i = 1; i < collider2Ds.Count; ++i)
                            {
                                _bounds.Encapsulate(collider2Ds[i].bounds);
                            }
                            _bounds.extents += bufferedCells * cellSize * 2;
                            _system = new JobifiedGridSpatialPartitioningSystem(_bounds, cellSize, maxObjects, false, false, true);
                        }
                        break;
                }
            }

            if (_system == null && IsServer && Manager.ServerSceneInfo.HasValue)
            {
                if (!_didLogMissingSystemWarning)
                {
                    Debug.LogWarning("[AOI] PrepareSystem: No AOIMapBounds or scene colliders found. " +
                        "Grid AOI system could not initialize. Interest management will run in degraded " +
                        "fallback mode (O(N*M) range checks). Add an AOIMapBounds component to your map scene.");
                    _didLogMissingSystemWarning = true;
                }
            }
            else
            {
                _didLogMissingSystemWarning = false;
            }
        }

        public override void UpdateInterestManagementImmediate()
        {
            _updateCountDown = 0f;
            UpdateInterestManagement(0f);
        }

        public override void UpdateInterestManagement(float deltaTime)
        {
            if (_system == null)
            {
                FallbackUpdateInterestManagement(deltaTime);
                return;
            }

            _updateCountDown -= deltaTime;
            if (_updateCountDown > 0)
                return;
            _updateCountDown = updateInterval;

            foreach (KeyValuePair<uint, HashSet<uint>> kv in _playerSubscribings)
            {
                kv.Value.Clear();
                _hashSetPool.Enqueue(kv.Value);
            }
            _playerSubscribings.Clear();

            using (s_UpdateProfilerMarker.Auto())
            {
                _spatialObjects.Clear();
                foreach (LiteNetLibPlayer player in Manager.GetPlayers())
                {
                    if (!player.IsReady)
                    {
                        // Don't subscribe if player not ready
                        continue;
                    }
                    foreach (LiteNetLibIdentity playerObject in player.GetSpawnedObjects())
                    {
                        _spatialObjects.Add(new SpatialObject()
                        {
                            objectId = playerObject.ObjectId,
                            position = playerObject.transform.position,
                        });
                    }
                }
                _system.UpdateGrid(_spatialObjects);
                _alwaysVisibleObjects.Clear();
                NativeList<SpatialObject> queryResults;
                HashSet<uint> subscribings;
                LiteNetLibIdentity foundPlayerObject;
                foreach (LiteNetLibIdentity spawnedObject in Manager.Assets.GetSpawnedObjects())
                {
                    if (spawnedObject == null)
                        continue;
                    if (spawnedObject.AlwaysVisible)
                    {
                        _alwaysVisibleObjects.Add(spawnedObject.ObjectId);
                        continue;
                    }
                    _system.QuerySphere(spawnedObject.transform.position, GetVisibleRange(spawnedObject));
                    queryResults = _system.QueryResults;
                    for (int i = 0; i < queryResults.Length; ++i)
                    {
                        uint contactedObjectId = queryResults[i].objectId;
                        if (!Manager.Assets.TryGetSpawnedObject(contactedObjectId, out foundPlayerObject))
                        {
                            continue;
                        }
                        if (!ShouldSubscribe(foundPlayerObject, spawnedObject, false))
                        {
                            continue;
                        }
                        if (!_playerSubscribings.TryGetValue(contactedObjectId, out subscribings))
                            subscribings = _hashSetPool.Count > 0 ? _hashSetPool.Dequeue() : new HashSet<uint>();
                        subscribings.Add(spawnedObject.ObjectId);
                        _playerSubscribings[contactedObjectId] = subscribings;
                    }
                }

                foreach (ISpatialObjectComponent component in SpatialObjectContainer.GetValues())
                {
                    if (component == null)
                        continue;
                    component.ClearSubscribers();
                    if (!component.SpatialObjectEnabled)
                        continue;
                    switch (component.SpatialObjectShape)
                    {
                        case SpatialObjectShape.Box:
                            _system.QueryBox(component.SpatialObjectPosition, component.SpatialObjectExtents);
                            break;
                        default:
                            _system.QuerySphere(component.SpatialObjectPosition, component.SpatialObjectRadius);
                            break;
                    }
                    queryResults = _system.QueryResults;
                    for (int i = 0; i < queryResults.Length; ++i)
                    {
                        uint contactedObjectId = queryResults[i].objectId;
                        if (Manager.Assets.TryGetSpawnedObject(contactedObjectId, out foundPlayerObject))
                            component.AddSubscriber(foundPlayerObject.ObjectId);
                    }
                }

                foreach (LiteNetLibPlayer player in Manager.GetPlayers())
                {
                    if (!player.IsReady)
                    {
                        // Don't subscribe if player not ready
                        continue;
                    }
                    foreach (LiteNetLibIdentity playerObject in player.GetSpawnedObjects())
                    {
                        if (_playerSubscribings.TryGetValue(playerObject.ObjectId, out subscribings))
                        {
                            if (_alwaysVisibleObjects.Count > 0)
                            {
                                foreach (uint alwaysVisibleObject in _alwaysVisibleObjects)
                                {
                                    subscribings.Add(alwaysVisibleObject);
                                }
                            }
                            playerObject.UpdateSubscribings(subscribings);
                        }
                        else if (_alwaysVisibleObjects.Count > 0)
                        {
                            playerObject.UpdateSubscribings(_alwaysVisibleObjects);
                        }
                        else
                        {
                            playerObject.ClearSubscribings();
                        }
                    }
                }
            }
        }

        private void FallbackUpdateInterestManagement(float deltaTime)
        {
            _updateCountDown -= deltaTime;
            if (_updateCountDown > 0)
                return;
            _updateCountDown = updateInterval;

            foreach (LiteNetLibPlayer player in Manager.GetPlayers())
            {
                if (!player.IsReady)
                    continue;
                foreach (LiteNetLibIdentity playerObject in player.GetSpawnedObjects())
                {
                    _fallbackSubscribings.Clear();
                    foreach (LiteNetLibIdentity spawnedObject in Manager.Assets.GetSpawnedObjects())
                    {
                        if (ShouldSubscribe(playerObject, spawnedObject))
                            _fallbackSubscribings.Add(spawnedObject.ObjectId);
                    }
                    playerObject.UpdateSubscribings(_fallbackSubscribings);
                }
            }

            foreach (ISpatialObjectComponent component in SpatialObjectContainer.GetValues())
            {
                if (component == null)
                    continue;
                component.ClearSubscribers();
                if (!component.SpatialObjectEnabled)
                    continue;
                foreach (LiteNetLibPlayer player in Manager.GetPlayers())
                {
                    if (!player.IsReady)
                        continue;
                    foreach (LiteNetLibIdentity playerObject in player.GetSpawnedObjects())
                    {
                        if (IsWithinSpatialShape(playerObject.transform.position, component))
                        {
                            component.AddSubscriber(playerObject.ObjectId);
                        }
                    }
                }
            }
        }

        private bool IsWithinSpatialShape(Vector3 objectPosition, ISpatialObjectComponent component)
        {
            switch (component.SpatialObjectShape)
            {
                case SpatialObjectShape.Box:
                    return IsWithinBox(
                        objectPosition,
                        (Vector3)component.SpatialObjectPosition,
                        (Vector3)component.SpatialObjectExtents);
                default:
                    return IsWithinSphere(
                        objectPosition,
                        (Vector3)component.SpatialObjectPosition,
                        component.SpatialObjectRadius);
            }
        }

        private bool IsWithinBox(Vector3 objectPosition, Vector3 center, Vector3 extents)
        {
            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension2D:
                    return Mathf.Abs(objectPosition.x - center.x) <= extents.x &&
                        Mathf.Abs(objectPosition.y - center.y) <= extents.y;
                default:
                    return Mathf.Abs(objectPosition.x - center.x) <= extents.x &&
                        Mathf.Abs(objectPosition.z - center.z) <= extents.z;
            }
        }

        private bool IsWithinSphere(Vector3 objectPosition, Vector3 center, float radius)
        {
            float radiusSqr = radius * radius;
            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension2D:
                    float xDiff2D = objectPosition.x - center.x;
                    float yDiff2D = objectPosition.y - center.y;
                    return xDiff2D * xDiff2D + yDiff2D * yDiff2D <= radiusSqr;
                default:
                    float xDiff3D = objectPosition.x - center.x;
                    float zDiff3D = objectPosition.z - center.z;
                    return xDiff3D * xDiff3D + zDiff3D * zDiff3D <= radiusSqr;
            }
        }
    }
}
