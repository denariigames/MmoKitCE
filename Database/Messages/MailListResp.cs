using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct MailListResp : INetSerializable
    {
        public List<MailListEntry> List { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            List = reader.GetList<MailListEntry>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutList(List);
        }
    }
}