using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
    public partial struct GetGuildReq : INetSerializable
    {
        public int GuildId { get; set; }
        public bool ForceClearCache { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            GuildId = reader.GetPackedInt();
            ForceClearCache = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(GuildId);
            writer.Put(ForceClearCache);
        }
    }
}