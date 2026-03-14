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
        private HashSet<uint> _alwaysVisibleObjects = new HashSet<uint>();

        private void OnDrawGizmosSelected()
        {
            Color color = Gizmos.color;
            Gizmos.color = boundsGizmosColor;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            Gizmos.color = color;
        }

        private void OnDestroy()
        {
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
                _system = null;
                return;
            }
            PrepareSystem();
        }

        public void PrepareSystem()
        {
            if (!IsServer || !Manager.ServerSceneInfo.HasValue)
            {
                _system = null;
                return;
            }
            _system = null;
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
        }

        public override void UpdateInterestManagement(float deltaTime)
        {
            if (_system == null)
                return;

            _updateCountDown -= deltaTime;
            if (_updateCountDown > 0)
                return;
            _updateCountDown = updateInterval;

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
                NativeList<SpatialObject> queryResult;
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
                    queryResult = _system.QuerySphere(spawnedObject.transform.position, GetVisibleRange(spawnedObject));
                    for (int i = 0; i < queryResult.Length; ++i)
                    {
                        uint contactedObjectId = queryResult[i].objectId;
                        if (!Manager.Assets.TryGetSpawnedObject(contactedObjectId, out foundPlayerObject))
                        {
                            continue;
                        }
                        if (!ShouldSubscribe(foundPlayerObject, spawnedObject, false))
                        {
                            continue;
                        }
                        if (!_playerSubscribings.TryGetValue(contactedObjectId, out subscribings))
                            subscribings = new HashSet<uint>();
                        subscribings.Add(spawnedObject.ObjectId);
                        _playerSubscribings[contactedObjectId] = subscribings;
                    }
                    queryResult.Dispose();
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
                            queryResult = _system.QueryBox(component.SpatialObjectPosition, component.SpatialObjectExtents);
                            break;
                        default:
                            queryResult = _system.QuerySphere(component.SpatialObjectPosition, component.SpatialObjectRadius);
                            break;
                    }
                    for (int i = 0; i < queryResult.Length; ++i)
                    {
                        uint contactedObjectId = queryResult[i].objectId;
                        if (Manager.Assets.TryGetSpawnedObject(contactedObjectId, out foundPlayerObject))
                            component.AddSubscriber(foundPlayerObject.ObjectId);
                    }
                    queryResult.Dispose();
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
                            subscribings.Clear();
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
    }
}
