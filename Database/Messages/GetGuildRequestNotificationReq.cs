using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetGuildRequestNotificationReq : INetSerializable
    {
        public int GuildId { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            GuildId = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(GuildId);
        }
    }
}