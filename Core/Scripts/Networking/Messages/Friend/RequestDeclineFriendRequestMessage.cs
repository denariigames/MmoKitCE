using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestDeclineFriendRequestMessage : INetSerializable
    {
        public string requesterId;

        public void Deserialize(NetDataReader reader)
        {
            requesterId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(requesterId);
        }
    }
}
