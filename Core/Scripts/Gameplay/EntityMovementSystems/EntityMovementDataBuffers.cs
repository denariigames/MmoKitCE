using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public static class EntityMovementDataBuffers
    {
        internal static readonly NetDataWriter StateMessageWriter = new NetDataWriter(true, 64);
        internal static readonly NetDataWriter StateDataWriter = new NetDataWriter(true, 64);
        internal static readonly NetDataWriter ReliablePacketWriter = new NetDataWriter(true, 1024);
        internal static readonly NetDataWriter UnreliablePacketWriter = new NetDataWriter(true, 1024);
    }
}
