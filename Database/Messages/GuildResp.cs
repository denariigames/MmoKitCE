using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GuildResp : INetSerializable
    {
        public GuildData GuildData { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            bool notNull = reader.GetBool();
            if (notNull)
                GuildData = reader.Get(() => new GuildData());
        }

        public void Serialize(NetDataWriter writer)
        {
            bool notNull = GuildData != null;
            writer.Put(notNull);
            if (notNull)
                writer.Put(GuildData);
        }
    }
}