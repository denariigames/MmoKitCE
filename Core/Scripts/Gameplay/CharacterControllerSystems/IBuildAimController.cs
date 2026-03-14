using UnityEngine;

namespace MultiplayerARPG
{
    public interface IBuildAimController
    {
        void Init();
        AimPosition UpdateAimControls(Vector2 aimAxes, BuildingEntity prefab);
        void FinishAimControls(bool isCancel);
    }
}
