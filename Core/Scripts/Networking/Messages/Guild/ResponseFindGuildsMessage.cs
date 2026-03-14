using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct ResponseFindGuildsMessage : INetSerializable
    {
        public UITextKeys message;
        public List<GuildListEntry> guilds;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            if (!message.IsError())
                guilds = reader.GetList<GuildListEntry>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            if (!message.IsError())
                writer.PutList(guilds);
        }
    }
}
