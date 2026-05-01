using Unity.Mathematics;

namespace MultiplayerARPG
{
    public struct MovementResult
    {
        public uint objectId;
        public bool reliably;
        public uint movementState;
        public byte extraMovementState;
        public byte compressionMode;
        public byte cellId;
        public int byteCount;
        public ulong data;
        public byte compressedYAndle;
    }
}