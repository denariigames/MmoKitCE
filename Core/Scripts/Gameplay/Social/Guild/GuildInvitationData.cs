using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct GuildInvitationData : INetSerializable
    {
        public string InviterId { get; set; }
        public string InviterName { get; set; }
        public int InviterLevel { get; set; }
        public int GuildId { get; set; }
        public string GuildName { get; set; }
        public int GuildLevel { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            InviterId = reader.GetString();
            InviterName = reader.GetString();
            InviterLevel = reader.GetPackedInt();
            GuildId = reader.GetPackedInt();
            GuildName = reader.GetString();
            GuildLevel = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(InviterId);
            writer.Put(InviterName);
            writer.PutPackedInt(InviterLevel);
            writer.PutPackedInt(GuildId);
            writer.Put(GuildName);
            writer.PutPackedInt(GuildLevel);
        }
    }
}
