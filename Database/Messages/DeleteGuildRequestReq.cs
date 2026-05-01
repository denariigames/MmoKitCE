using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct DeleteGuildRequestReq : INetSerializable
    {
        public int GuildId { get; set; }
        public string RequesterId { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            GuildId = reader.GetPackedInt();
            RequesterId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(GuildId);
            writer.Put(RequesterId);
        }
    }
}