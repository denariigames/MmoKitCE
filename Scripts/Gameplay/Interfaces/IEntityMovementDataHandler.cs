using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public interface IEntityMovementDataHandler
    {
        uint ObjectId { get; }
        long ConnectionId { get; }
        void ReadClientStateAtServer(long peerTimestamp, NetDataReader reader);
        void ReadServerStateAtClient(long peerTimestamp, NetDataReader reader);
        bool WriteClientState(long writeTimestamp, NetDataWriter writer, out bool shouldSendReliably);
        bool WriteServerState(long writeTimestamp, NetDataWriter writer, out bool shouldSendReliably);
    }
}
