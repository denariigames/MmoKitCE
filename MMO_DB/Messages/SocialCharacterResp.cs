using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct SocialCharacterResp : INetSerializable
    {
        public SocialCharacterData SocialCharacterData { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            SocialCharacterData = reader.Get<SocialCharacterData>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(SocialCharacterData);
        }
    }
}