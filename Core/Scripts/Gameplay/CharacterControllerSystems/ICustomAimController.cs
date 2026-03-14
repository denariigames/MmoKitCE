using UnityEngine;

namespace MultiplayerARPG
{
    public interface ICustomAimController
    {
        bool HasCustomAimControls();
        AimPosition UpdateAimControls(Vector2 aimAxes, params object[] data);
        void FinishAimControls(bool isCancel);
        bool IsChanneledAbility();
    }
}
