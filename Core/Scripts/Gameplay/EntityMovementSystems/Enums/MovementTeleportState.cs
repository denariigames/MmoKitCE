namespace MultiplayerARPG
{
    [System.Flags]
    public enum MovementTeleportState : byte
    {
        None = 0,
        Requesting = 1 << 0,
        StillMoveAfterTeleport = 1 << 1,
        Responding = 1 << 2,
        WaitingForResponse = 1 << 3,
        Responded = 1 << 4,
    }

    public static class ServerTeleportRequestStateExtensions
    {
        public static bool Has(this MovementTeleportState self, MovementTeleportState flag)
        {
            return (self & flag) == flag;
        }
    }
}