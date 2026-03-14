using UnityEngine;

namespace MultiplayerARPG
{
    public interface IAreaSkillAimController
    {
        bool IsAiming { get; }
        AimPosition UpdateAimControls(Vector2 aimAxes, BaseAreaSkill skill, int skillLevel);
        void FinishAimControls(bool isCancel);
    }
}
