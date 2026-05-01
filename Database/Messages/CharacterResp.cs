using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct CharacterResp : INetSerializable
    {
        public PlayerCharacterData CharacterData { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            bool isNull = reader.GetBool();
            if (!isNull)
                CharacterData = reader.Get(() => new PlayerCharacterData());
        }

        public void Serialize(NetDataWriter writer)
        {
            bool isNull = CharacterData == null;
            writer.Put(isNull);
            if (!isNull)
                writer.Put(CharacterData);
        }
    }
}