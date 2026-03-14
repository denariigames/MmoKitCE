namespace MultiplayerARPG
{
    public interface ICharacterUseSkillComponent
    {
        BaseSkill UsingSkill { get; }
        int UsingSkillLevel { get; }
        bool IsCastingSkillCanBeInterrupted { get; }
        bool IsCastingSkillInterrupted { get; }
        float CastingSkillDuration { get; }
        float CastingSkillCountDown { get; }
        bool IsUsingSkill { get; }
        float LastUseSkillEndTime { get; }
        bool IsSkipMovementValidationWhileUsingSkill { get; }
        bool IsUseRootMotionWhileUsingSkill { get; }
        float MoveSpeedRateWhileUsingSkill { get; }
        MovementRestriction MovementRestrictionWhileUsingSkill { get; }
        System.Action OnCastSkillStart { get; set; }
        System.Action OnCastSkillEnd { get; set; }
        System.Action OnUseSkillStart { get; set; }
        System.Action<int> OnUseSkillTrigger { get; set; }
        System.Action OnUseSkillEnd { get; set; }
        System.Action OnSkillInterupted { get; set; }

        void InterruptCastingSkill();
        void CancelSkill();
        void ClearUseSkillStates();
        void UseSkill(int dataId, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition);
        void UseSkillItem(int itemIndex, WeaponHandlingState weaponHandlingState, uint targetObjectId, AimPosition aimPosition);
    }
}
