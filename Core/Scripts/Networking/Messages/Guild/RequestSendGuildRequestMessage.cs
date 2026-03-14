using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestSendGuildRequestMessage : INetSerializable
    {
        public int guildId;

        public void Deserialize(NetDataReader reader)
        {
            guildId = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(guildId);
        }
    }
}
