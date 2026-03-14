namespace MultiplayerARPG
{
    [System.Flags]
    public enum EntityMovementInputState : byte
    {
        None = 0,
        IsKeyMovement = 1 << 0,
        PositionChanged = 1 << 1,
        RotationChanged = 1 << 2,
        IsStopped = 1 << 3,
        Other = 1 << 4,
    }

    public static class EntityMovementInputStateExtensions
    {
        public static bool Has(this EntityMovementInputState self, EntityMovementInputState flag)
        {
            return (self & flag) == flag;
        }
    }
}
