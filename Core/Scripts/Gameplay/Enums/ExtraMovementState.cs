//DG: 20260403 add sitting state

namespace MultiplayerARPG
{
    /// <summary>
    /// Toggleable movement states
    /// </summary>
    public enum ExtraMovementState : byte
    {
        None,
        IsSprinting,
        IsWalking,
        IsCrouching,
        IsCrawling,
        IsFlying,
        IsSitting,
    }
}
