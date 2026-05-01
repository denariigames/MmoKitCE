using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateGuildMemberCountReq : INetSerializable
    {
        public int GuildId { get; set; }
        public int MaxGuildMember { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            GuildId = reader.GetInt();
            MaxGuildMember = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(GuildId);
            writer.Put(MaxGuildMember);
        }
    }
}