using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestChangeGuildAutoAcceptRequestsMessage : INetSerializable
    {
        public bool autoAcceptRequests;

        public void Deserialize(NetDataReader reader)
        {
            autoAcceptRequests = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(autoAcceptRequests);
        }
    }
}