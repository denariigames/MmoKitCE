using UnityEngine;

namespace MultiplayerARPG
{
    public class PhysicFunctions : IPhysicFunctions
    {
        private RaycastHit[] _raycasts;
        private Collider[] _overlapColliders;

        public PhysicFunctions(int allocSize)
        {
            _raycasts = new RaycastHit[allocSize];
            _overlapColliders = new Collider[allocSize];
        }

        ~PhysicFunctions()
        {
            Clean();
        }

        public void Clean()
        {
            _raycasts = null;
            _overlapColliders.Nullify();
            _overlapColliders = null;
        }

        public bool SingleRaycast(Vector3 start, Vector3 end, out PhysicRaycastResult result, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            result = new PhysicRaycastResult();
            RaycastHit hit;
            bool prevHitBackfaces = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = hitBackfaces;
            bool isHit = Physics.Raycast(start, (end - start).normalized, out hit, Vector3.Distance(start, end), layerMask, hitTriggers);
            Physics.queriesHitBackfaces = prevHitBackfaces;
            if (isHit)
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
            RaycastHit hit;
            bool prevHitBackfaces = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = hitBackfaces;
            bool isHit = Physics.Raycast(origin, direction, out hit, distance, layerMask, hitTriggers);
            Physics.queriesHitBackfaces = prevHitBackfaces;
            if (isHit)
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
            return PhysicUtils.SortedRaycastNonAlloc3D(start, (end - start).normalized, _raycasts, Vector3.Distance(start, end), layerMask, hitTriggers, hitBackfaces);
        }

        public int Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            return PhysicUtils.SortedRaycastNonAlloc3D(origin, direction, _raycasts, distance, layerMask, hitTriggers, hitBackfaces);
        }

        public int RaycastPickObjects(Camera camera, Vector3 mousePosition, int layerMask, float distance, out Vector3 raycastPosition, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            Ray ray = camera.ScreenPointToRay(mousePosition);
            raycastPosition = ray.origin;
            return PhysicUtils.SortedRaycastNonAlloc3D(ray, _raycasts, distance, layerMask, hitTriggers, hitBackfaces);
        }

        public int RaycastDown(Vector3 position, int layerMask, float distance = 100, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            // Raycast to find hit floor
            bool prevHitBackfaces = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = hitBackfaces;
            int hitCount = Physics.RaycastNonAlloc(position + (Vector3.up * distance * 0.5f), Vector3.down, _raycasts, distance, layerMask, hitTriggers);
            Physics.queriesHitBackfaces = prevHitBackfaces;
            System.Array.Sort(_raycasts, 0, hitCount, new PhysicUtils.RaycastHitComparerCustomOrigin(position));
            return hitCount;
        }

        public int OverlapObjects(Vector3 position, float radius, int layerMask, bool sort = false, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false, bool hitMultipleFaces = false)
        {
            if (sort)
            {
                return PhysicUtils.SortedOverlapSphereNonAlloc(position, radius, _overlapColliders, layerMask, hitTriggers, hitBackfaces);
            }
            else
            {
                bool prevHitBackfaces = Physics.queriesHitBackfaces;
                Physics.queriesHitBackfaces = hitBackfaces;
                int count = Physics.OverlapSphereNonAlloc(position, radius, _overlapColliders, layerMask, hitTriggers);
                Physics.queriesHitBackfaces = prevHitBackfaces;
                return count;
            }
        }

        public bool GetRaycastIsTrigger(int index) => _raycasts[index].collider.isTrigger;
        public Vector3 GetRaycastPoint(int index) => _raycasts[index].point;
        public Vector3 GetRaycastNormal(int index) => _raycasts[index].normal;
        public Bounds GetRaycastColliderBounds(int index) => _raycasts[index].collider.bounds;
        public float GetRaycastDistance(int index) => _raycasts[index].distance;
        public Transform GetRaycastTransform(int index) => _raycasts[index].transform;
        public GameObject GetRaycastObject(int index) => _raycasts[index].transform.gameObject;
        public Vector3 GetRaycastColliderClosestPoint(int index, Vector3 position) => _raycasts[index].collider.ClosestPoint(position);

        public bool GetOverlapIsTrigger(int index) => _overlapColliders[index].isTrigger;
        public Object GetOverlapCollider(int index) => _overlapColliders[index];
        public GameObject GetOverlapObject(int index) => _overlapColliders[index].gameObject;
        public Vector3 GetOverlapColliderClosestPoint(int index, Vector3 position) => _overlapColliders[index].ClosestPoint(position);

        public bool GetOverlapColliderRaycast(int index, Vector3 origin, Vector3 direction, out Vector3 point, out Vector3 normal, out float distance, out Transform transform, float maxDistance)
        {
            if (_overlapColliders[index].Raycast(new Ray(origin, direction), out RaycastHit hitInfo, maxDistance))
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