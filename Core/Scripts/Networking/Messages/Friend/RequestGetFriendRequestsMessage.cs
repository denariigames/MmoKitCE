using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestGetFriendRequestsMessage : INetSerializable
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