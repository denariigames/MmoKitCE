using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct FindGuildNameReq : INetSerializable
    {
        public string? FinderId { get; set; }
        public string GuildName { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            FinderId = reader.GetString();
            GuildName = reader.GetString();
            Skip = reader.GetPackedInt();
            Limit = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(FinderId);
            writer.Put(GuildName);
            writer.PutPackedInt(Skip);
            writer.PutPackedInt(Limit);
        }
    }
}