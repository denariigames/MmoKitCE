using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GoldResp : INetSerializable
    {
        public int Gold { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            Gold = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Gold);
        }
    }
}