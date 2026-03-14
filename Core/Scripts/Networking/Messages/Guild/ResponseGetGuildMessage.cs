using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseGetGuildMessage : INetSerializable
    {
        public GuildData guild;

        public void Deserialize(NetDataReader reader)
        {
            bool notNull = reader.GetBool();
            if (notNull)
                guild = reader.Get(() => new GuildData());
        }

        public void Serialize(NetDataWriter writer)
        {
            bool notNull = guild != null;
            writer.Put(notNull);
            if (notNull)
                writer.Put(guild);
        }
    }
}
