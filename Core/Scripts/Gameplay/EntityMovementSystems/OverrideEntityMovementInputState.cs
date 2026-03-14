namespace MultiplayerARPG
{
    [System.Flags]
    public enum OverrideEntityMovementInputState : byte
    {
        None = 0,
        IsEnabled = 1 << 0,
        IsStopped = 1 << 1,
        IsPointClick = 1 << 2,
        IsKeyMovement = 1 << 3,
        IsSetExtraMovementState = 1 << 4,
        IsSetLookRotation = 1 << 5,
        IsSetSmoothTurnSpeed = 1 << 6,
    }

    public static class OverrideEntityMovementInputStateExtensions
    {
        public static bool Has(this OverrideEntityMovementInputState self, OverrideEntityMovementInputState flag)
        {
            return (self & flag) == flag;
        }
    }
}