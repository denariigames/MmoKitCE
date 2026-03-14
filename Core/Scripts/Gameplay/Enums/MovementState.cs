namespace MultiplayerARPG
{
    [System.Flags]
    public enum MovementState : uint
    {
        None = 0,
        Forward = 1 << 0, // 1
        Backward = 1 << 1, // 2
        Left = 1 << 2, // 4
        Right = 1 << 3, // 8
        IsGrounded = 1 << 4, // 16
        IsUnderWater = 1 << 5, // 32
        IsJump = 1 << 6, // 64
        IsTeleport = 1 << 7, // 128, end of byte
        Up = 1 << 8, // 256
        Down = 1 << 9, // 512
        IsClimbing = 1 << 10, // 1024
        IsDash = 1 << 11, // 2048
        IsEvenStep = 1 << 12, // 4096
        IsStarting = 1 << 13, // 8192
        IsEnding = 1 << 14, // 16384
    }

    public static class MovementStateExtensions
    {
        public static bool Has(this MovementState self, MovementState flag)
        {
            return (self & flag) == flag;
        }

        public static bool HasDirectionMovement(this MovementState self)
        {
            return self.Has(MovementState.Forward) ||
                self.Has(MovementState.Backward) ||
                self.Has(MovementState.Right) ||
                self.Has(MovementState.Left);
        }
    }
}
