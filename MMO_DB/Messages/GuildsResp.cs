using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GuildsResp : INetSerializable
    {
        public List<GuildListEntry> List { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            List = reader.GetList<GuildListEntry>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutList(List);
        }
    }
}