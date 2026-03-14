using UnityEngine;

namespace MultiplayerARPG
{
    public class PhysicFunctions2D : IPhysicFunctions
    {
        private RaycastHit2D[] _raycasts2D;
        private Collider2D[] _overlapColliders2D;

        public PhysicFunctions2D(int allocSize)
        {
            _raycasts2D = new RaycastHit2D[allocSize];
            _overlapColliders2D = new Collider2D[allocSize];
        }

        ~PhysicFunctions2D()
        {
            Clean();
        }

        public void Clean()
        {
            _raycasts2D = null;
            _overlapColliders2D.Nullify();
            _overlapColliders2D = null;
        }

        public bool SingleRaycast(Vector3 start, Vector3 end, out PhysicRaycastResult result, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            result = new PhysicRaycastResult();
            RaycastHit2D hit = Physics2D.Raycast(start, (end - start).normalized, Vector3.Distance(start, end), layerMask);
            if (hit.collider != null)
            {
                result.point = hit.point;
                result.normal = hit.normal;
                result.distance = hit.distance;
                result.transform = hit.transform;
                return true;
            }
            return false;
        }

        public bool SingleRaycast(Vector3 origin, Vector3 direction, out PhysicRaycastResult result, float distance, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            result = new PhysicRaycastResult();
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
            if (hit.collider != null)
            {
                result.point = hit.point;
                result.normal = hit.normal;
                result.distance = hit.distance;
                result.transform = hit.transform;
                return true;
            }
            return false;
        }

        public int Raycast(Vector3 start, Vector3 end, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            return PhysicUtils.SortedRaycastNonAlloc2D(start, (end - start).normalized, _raycasts2D, Vector3.Distance(start, end), layerMask);
        }

        public int Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            return PhysicUtils.SortedRaycastNonAlloc2D(origin, direction, _raycasts2D, distance, layerMask);
        }

        public int RaycastPickObjects(Camera camera, Vector3 mousePosition, int layerMask, float distance, out Vector3 raycastPosition, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            raycastPosition = camera.ScreenToWorldPoint(mousePosition);
            raycastPosition.z = 0;
            return PhysicUtils.SortedLinecastNonAlloc2D(raycastPosition, raycastPosition, _raycasts2D, layerMask);
        }

        public int RaycastDown(Vector3 position, int layerMask, float distance = 100f, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            return PhysicUtils.SortedLinecastNonAlloc2D(position, position, _raycasts2D, layerMask);
        }

        public int OverlapObjects(Vector3 position, float radius, int layerMask, bool sort = false, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            return sort ? PhysicUtils.SortedOverlapCircleNonAlloc(position, radius, _overlapColliders2D, layerMask) :
                Physics2D.OverlapCircleNonAlloc(position, radius, _overlapColliders2D, layerMask);
        }

        public bool GetRaycastIsTrigger(int index) => _raycasts2D[index].collider.isTrigger;
        public Vector3 GetRaycastPoint(int index) => _raycasts2D[index].point;
        public Vector3 GetRaycastNormal(int index) => _raycasts2D[index].normal;
        public Bounds GetRaycastColliderBounds(int index) => _raycasts2D[index].collider.bounds;
        public float GetRaycastDistance(int index) => _raycasts2D[index].distance;
        public Transform GetRaycastTransform(int index) => _raycasts2D[index].transform;
        public GameObject GetRaycastObject(int index) => _raycasts2D[index].transform.gameObject;
        public Vector3 GetRaycastColliderClosestPoint(int index, Vector3 position) => _raycasts2D[index].collider.ClosestPoint(position);

        public bool GetOverlapIsTrigger(int index) => _overlapColliders2D[index].isTrigger;
        public Object GetOverlapCollider(int index) => _overlapColliders2D[index];
        public GameObject GetOverlapObject(int index) => _overlapColliders2D[index].gameObject;
        public Vector3 GetOverlapColliderClosestPoint(int index, Vector3 position) => _overlapColliders2D[index].ClosestPoint(position);

        public bool GetOverlapColliderRaycast(int index, Vector3 origin, Vector3 direction, out Vector3 point, out Vector3 normal, out float distance, out Transform transform, float maxDistance)
        {
            RaycastHit2D[] hits = new RaycastHit2D[1];
            int hitCount = _overlapColliders2D[index].Raycast(direction, hits, maxDistance);
            if (hitCount > 0)
            {
                point = hits[0].point;
                normal = hits[0].normal;
                distance = hits[0].distance;
                transform = hits[0].transform;
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
