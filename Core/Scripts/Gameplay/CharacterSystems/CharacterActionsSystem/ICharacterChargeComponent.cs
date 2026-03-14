namespace MultiplayerARPG
{
    public interface ICharacterChargeComponent
    {
        bool IsCharging { get; }
        bool WillDoActionWhenStopCharging { get; }
        bool IsSkipMovementValidationWhileCharging { get; }
        bool IsUseRootMotionWhileCharging { get; }
        float MoveSpeedRateWhileCharging { get; }
        MovementRestriction MovementRestrictionWhileCharging { get; }

        void ClearChargeStates();
        void StartCharge(bool isLeftHand);
        void StopCharge();
    }
}
