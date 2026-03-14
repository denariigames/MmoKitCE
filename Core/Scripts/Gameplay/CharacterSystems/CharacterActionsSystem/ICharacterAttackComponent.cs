namespace MultiplayerARPG
{
    public interface ICharacterAttackComponent
    {
        bool IsAttacking { get; }
        float LastAttackEndTime { get; }
        bool IsSkipMovementValidationWhileAttacking { get; }
        bool IsUseRootMotionWhileAttacking { get; }
        float MoveSpeedRateWhileAttacking { get; }
        MovementRestriction MovementRestrictionWhileAttacking { get; }
        System.Action OnAttackStart { get; set; }
        System.Action<int> OnAttackTrigger { get; set; }
        System.Action OnAttackEnd { get; set; }
        System.Action OnAttackInterupted { get; set; }

        void CancelAttack();
        void ClearAttackStates();
        void Attack(WeaponHandlingState weaponHandlingState);
    }
}
