using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
    public struct RequestForceDespawnCharacterMessage : INetSerializable
    {
        public string userId;
        public string characterId;
        public string channelId;

        public void Deserialize(NetDataReader reader)
        {
            userId = reader.GetString();
            characterId = reader.GetString();
            channelId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(userId);
            writer.Put(characterId);
            writer.Put(channelId);
        }
    }
}
