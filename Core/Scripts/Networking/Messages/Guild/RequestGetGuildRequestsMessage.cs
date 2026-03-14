using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct RequestGetGuildRequestsMessage : INetSerializable
    {
        public int skip;
        public int limit;

        public void Deserialize(NetDataReader reader)
        {
            skip = reader.GetPackedInt();
            limit = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(skip);
            writer.PutPackedInt(limit);
        }
    }
}
