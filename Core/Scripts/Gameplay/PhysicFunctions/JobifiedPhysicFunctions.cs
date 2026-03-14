using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace MultiplayerARPG
{
    public class JobifiedPhysicFunctions : IPhysicFunctions
    {
        private NativeArray<RaycastHit> _raycastResults;
        private NativeSlice<RaycastHit> _raycastSlices;
        private NativeArray<ColliderHit> _overlapResults;
        private NativeSlice<ColliderHit> _overlapSlices;
        private int _allocSize;

        public JobifiedPhysicFunctions(int allocSize)
        {
            _allocSize = allocSize;
            _raycastResults = new NativeArray<RaycastHit>(allocSize, Allocator.Persistent);
            _overlapResults = new NativeArray<ColliderHit>(allocSize, Allocator.Persistent);
        }

        public void Clean()
        {
            if (_raycastResults.IsCreated) _raycastResults.Dispose();
            if (_overlapResults.IsCreated) _overlapResults.Dispose();
        }

        ~JobifiedPhysicFunctions()
        {
            Clean();
        }

        public bool SingleRaycast(Vector3 start, Vector3 end, out PhysicRaycastResult result, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            return SingleRaycast(start, (end - start).normalized, out result, Vector3.Distance(start, end), layerMask, hitTriggers, hitBackfaces, hitMultipleFaces);
        }

        public bool SingleRaycast(Vector3 origin, Vector3 direction, out PhysicRaycastResult result, float distance, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            result = new PhysicRaycastResult();
            NativeArray<RaycastCommand> tempCommands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
            QueryParameters queryParameters = new QueryParameters(layerMask, hitMultipleFaces, hitTriggers, hitBackfaces);
            tempCommands[0] = new RaycastCommand(origin, direction, queryParameters, distance);
            JobHandle handle = RaycastCommand.ScheduleBatch(tempCommands, _raycastResults, 1, 1);
            handle.Complete();
            tempCommands.Dispose();
            if (_raycastResults[0].colliderInstanceID != 0)
            {
                result.point = _raycastResults[0].point;
                result.normal = _raycastResults[0].normal;
                result.distance = _raycastResults[0].distance;
                result.transform = _raycastResults[0].transform;
                return true;
            }
            return false;
        }

        public int Raycast(Vector3 start, Vector3 end, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            return Raycast(start, (end - start).normalized, Vector3.Distance(start, end), layerMask, hitTriggers, hitBackfaces, hitMultipleFaces);
        }

        public int Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            return Raycast(origin, direction, distance, layerMask, new PhysicUtils.RaycastHitComparer(), hitTriggers, hitBackfaces, hitMultipleFaces);
        }

        public int Raycast<TSorter>(Vector3 origin, Vector3 direction, float distance, int layerMask, TSorter sorter, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
            where TSorter : IComparer<RaycastHit>
        {
            NativeArray<RaycastCommand> tempCommands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
            QueryParameters queryParameters = new QueryParameters(layerMask, hitMultipleFaces, hitTriggers, hitBackfaces);
            tempCommands[0] = new RaycastCommand(origin, direction, queryParameters, distance);
            JobHandle handle = RaycastCommand.ScheduleBatch(tempCommands, _raycastResults, 1, _allocSize);
            handle.Complete();
            tempCommands.Dispose();
            int length = _allocSize;
            for (int i = 0; i < _raycastResults.Length; ++i)
            {
                if (_raycastResults[i].colliderInstanceID == 0)
                {
                    length = i;
                    break;
                }
            }
            _raycastSlices = _raycastResults.Slice(0, length);
            _raycastSlices.Sort(sorter);
            return _raycastSlices.Length;
        }

        public int RaycastPickObjects(Camera camera, Vector3 mousePosition, int layerMask, float distance, out Vector3 raycastPosition, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            Ray ray = camera.ScreenPointToRay(mousePosition);
            raycastPosition = ray.origin;
            return Raycast(ray.origin, ray.direction, distance, layerMask, hitTriggers, hitBackfaces, hitMultipleFaces);
        }

        public int RaycastDown(Vector3 position, int layerMask, float distance = 100, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            return Raycast(position + (Vector3.up * distance * 0.5f), Vector3.down, distance, layerMask, new PhysicUtils.RaycastHitComparerCustomOrigin(position), hitTriggers, hitBackfaces, hitMultipleFaces);
        }

        public int OverlapObjects(Vector3 position, float radius, int layerMask, bool sort = false, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            NativeArray<OverlapSphereCommand> tempCommands = new NativeArray<OverlapSphereCommand>(1, Allocator.TempJob);
            QueryParameters queryParameters = new QueryParameters(layerMask, hitMultipleFaces, hitTriggers, hitBackfaces);
            tempCommands[0] = new OverlapSphereCommand(position, radius, queryParameters);
            JobHandle handle = OverlapSphereCommand.ScheduleBatch(tempCommands, _overlapResults, 1, _allocSize);
            handle.Complete();
            tempCommands.Dispose();
            int length = _allocSize;
            for (int i = 0; i < _overlapResults.Length; ++i)
            {
                if (_overlapResults[i].instanceID == 0)
                {
                    length = i;
                    break;
                }
            }
            _overlapSlices = _overlapResults.Slice(0, length);
            if (sort)
                _overlapSlices.Sort(new PhysicUtils.ColliderHitComparer(position));
            return _overlapSlices.Length;
        }

        public bool GetRaycastIsTrigger(int index) => _raycastSlices[index].collider.isTrigger;
        public Vector3 GetRaycastPoint(int index) => _raycastSlices[index].point;
        public Vector3 GetRaycastNormal(int index) => _raycastSlices[index].normal;
        public Bounds GetRaycastColliderBounds(int index) => _raycastSlices[index].collider.bounds;
        public float GetRaycastDistance(int index) => _raycastSlices[index].distance;
        public Transform GetRaycastTransform(int index) => _raycastSlices[index].transform;
        public GameObject GetRaycastObject(int index) => _raycastSlices[index].transform.gameObject;
        public Vector3 GetRaycastColliderClosestPoint(int index, Vector3 position) => _raycastSlices[index].collider.ClosestPoint(position);

        public bool GetOverlapIsTrigger(int index) => _overlapResults[index].collider.isTrigger;
        public Object GetOverlapCollider(int index) => _overlapResults[index].collider;
        public GameObject GetOverlapObject(int index) => _overlapResults[index].collider.gameObject;
        public Vector3 GetOverlapColliderClosestPoint(int index, Vector3 position) => _overlapResults[index].collider.ClosestPoint(position);

        public bool GetOverlapColliderRaycast(int index, Vector3 origin, Vector3 direction, out Vector3 point, out Vector3 normal, out float distance, out Transform transform, float maxDistance)
        {
            if (_overlapResults[index].collider.Raycast(new Ray(origin, direction), out RaycastHit hitInfo, maxDistance))
            {
                point = hitInfo.point;
                normal = hitInfo.normal;
                distance = hitInfo.distance;
                transform = hitInfo.transform;
                return true;
            }
            point = origin + direction * maxDistance;
            normal = -direction;
            distance = maxDistance;
            transform = null;
            return false;
        }
    }
}
