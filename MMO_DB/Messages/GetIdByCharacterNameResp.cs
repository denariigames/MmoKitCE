using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetIdByCharacterNameResp : INetSerializable
    {
        public string Id { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
        }
    }
}