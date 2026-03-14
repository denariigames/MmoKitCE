namespace MultiplayerARPG
{
    public interface ILagCompensationManager
    {
        bool IsDisabled { get; }
        float SnapShotInterval { get; }
        int MaxHistorySize { get; }
        bool ShouldStoreHitBoxesTransformHistory { get; }
        bool SimulateHitBoxes(long connectionId, long rewindTime, System.Action action);
        bool BeginSimlateHitBoxes(long connectionId, long rewindTime);
        bool SimulateHitBoxesByHalfRtt(long connectionId, System.Action action);
        bool BeginSimlateHitBoxesByHalfRtt(long connectionId);
        void EndSimulateHitBoxes();
        void AddDamageableEntity(DamageableEntity entity);
        void RemoveDamageableEntity(DamageableEntity entity);
    }
}
