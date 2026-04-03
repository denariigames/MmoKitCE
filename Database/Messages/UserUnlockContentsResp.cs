using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UserUnlockContentsResp : INetSerializable
    {
        public List<UnlockableContent> List { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            List = reader.GetList<UnlockableContent>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutList(List);
        }
    }
}