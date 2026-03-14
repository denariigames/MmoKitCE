namespace MultiplayerARPG
{
    public interface ICharacterReloadComponent
    {
        int ReloadingAmmoAmount { get; }
        bool IsReloading { get; }
        float LastReloadEndTime { get; }
        bool IsSkipMovementValidationWhileReloading { get; }
        bool IsUseRootMotionWhileReloading { get; }
        float MoveSpeedRateWhileReloading { get; }
        MovementRestriction MovementRestrictionWhileReloading { get; }

        void CancelReload();
        void ClearReloadStates();
        void Reload(bool isLeftHand);
    }
}
