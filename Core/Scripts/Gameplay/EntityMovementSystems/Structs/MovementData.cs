using Unity.Mathematics;

namespace MultiplayerARPG
{
    public struct MovementData
    {
        public uint movementState;
        public byte extraMovementState;
        public float3 worldPosition;
        public float yAngle;
        public bool shouldSendReliably;
    }
}