using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct FindCharacterNameReq : INetSerializable
    {
        public string? FinderId { get; set; }
        public string CharacterName { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            FinderId = reader.GetString();
            CharacterName = reader.GetString();
            Skip = reader.GetPackedInt();
            Limit = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(FinderId);
            writer.Put(CharacterName);
            writer.PutPackedInt(Skip);
            writer.PutPackedInt(Limit);
        }
    }
}
