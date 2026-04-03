using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateGuildScoreReq : INetSerializable
    {
        public int GuildId { get; set; }
        public int Score { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            GuildId = reader.GetInt();
            Score = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(GuildId);
            writer.Put(Score);
        }
    }
}