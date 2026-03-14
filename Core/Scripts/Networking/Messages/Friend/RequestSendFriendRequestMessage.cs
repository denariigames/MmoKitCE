using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestSendFriendRequestMessage : INetSerializable
    {
        public string requesteeId;

        public void Deserialize(NetDataReader reader)
        {
            requesteeId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(requesteeId);
        }
    }
}
