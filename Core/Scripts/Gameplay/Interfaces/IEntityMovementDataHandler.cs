using LiteNetLib.Utils;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public interface IEntityMovementDataHandler
    {
        uint ObjectId { get; }
        long ConnectionId { get; }
        //Create movement data from current entity state, this is used to create movement data and server
        MovementData CreateMovementData(out List<EntityMovementForceApplier> forceAppliers);
        void ReadClientStateAtServer(long peerTimestamp, NetDataReader reader);
        void ReadServerStateAtClient(long peerTimestamp, NetDataReader reader);
        bool WriteClientState(long writeTimestamp, NetDataWriter writer, out bool shouldSendReliably);
        bool WriteServerState(long writeTimestamp, NetDataWriter writer,out bool shouldSendReliably);
    }
}
