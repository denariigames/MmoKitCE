using UnityEngine;

namespace MultiplayerARPG
{
    public class AllowToBuildOnSameAreaBelow : BaseBuildConditionForBuildingArea
    {
        public Vector3 raycastOriginOffsets = Vector3.up;
        public float raycastDistance = 10f;
        public LayerMask layerMask = 1;
        private RaycastHit[] _raycastHits = new RaycastHit[32];

        public override bool AllowToBuild(BuildingArea sourceArea, BuildingEntity newBuilding)
        {
            int hitCount = PhysicUtils.SortedRaycastNonAlloc3D(sourceArea.transform.position + raycastOriginOffsets, Vector3.down, _raycastHits, raycastDistance, layerMask.value);
            BuildingArea tempBuildingArea;
            for (int i = 0; i < hitCount; ++i)
            {
                if (_raycastHits[i].transform == sourceArea.transform)
                    continue;
                tempBuildingArea = _raycastHits[i].transform.GetComponent<BuildingArea>();
                if (tempBuildingArea == null)
                    continue;
                if (newBuilding.CanBuildOnAnySurface || newBuilding.BuildingTypes.Contains(tempBuildingArea.buildingType))
                    return true;
            }
            return false;
        }
    }
}
