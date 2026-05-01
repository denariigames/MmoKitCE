using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetCharacterReq : INetSerializable
    {
        public string UserId { get; set; }
        public string CharacterId { get; set; }
        public bool ForceClearCache { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            UserId = reader.GetString();
            CharacterId = reader.GetString();
            ForceClearCache = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserId);
            writer.Put(CharacterId);
            writer.Put(ForceClearCache);
        }
    }
}