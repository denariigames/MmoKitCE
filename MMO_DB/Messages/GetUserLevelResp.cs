using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetUserLevelResp : INetSerializable
    {
        public byte UserLevel { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            UserLevel = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserLevel);
        }
    }
}