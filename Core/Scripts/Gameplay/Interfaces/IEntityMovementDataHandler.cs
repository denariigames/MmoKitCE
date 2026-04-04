using LiteNetLib.Utils;
using UnityEngine;

namespace MultiplayerARPG
{
    public interface IEntityMovementDataHandler
    {
        uint ObjectId { get; }
        long ConnectionId { get; }
        //Last Data Compression Mode, used to determine which compression mode to use
        int LastDataCompressionMode { get; set; }
        void ReadClientStateAtServer(long peerTimestamp, NetDataReader reader);
        void ReadServerStateAtClient(long peerTimestamp, NetDataReader reader);
        bool WriteClientState(long writeTimestamp, NetDataWriter writer, out bool shouldSendReliably);
        bool WriteServerState(long writeTimestamp, NetDataWriter writer, Vector3 currentPlayerPosition, out bool shouldSendReliably);
    }
}
