using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
    public partial struct GetGuildRequestsReq : INetSerializable
    {
        public int GuildId { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            GuildId = reader.GetPackedInt();
            Skip = reader.GetPackedInt();
            Limit = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(GuildId);
            writer.PutPackedInt(Skip);
            writer.PutPackedInt(Limit);
        }
    }
}