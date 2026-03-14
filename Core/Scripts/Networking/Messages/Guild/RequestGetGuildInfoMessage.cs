using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestGetGuildInfoMessage : INetSerializable
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
