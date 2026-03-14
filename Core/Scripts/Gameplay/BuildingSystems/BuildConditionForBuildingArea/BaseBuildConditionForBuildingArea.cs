using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseBuildConditionForBuildingArea : MonoBehaviour
    {
        public abstract bool AllowToBuild(BuildingArea sourceArea, BuildingEntity newBuilding);
    }
}
