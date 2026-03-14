using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestFindCharactersMessage : INetSerializable
    {
        public string characterName;
        public int skip;
        public int limit;

        public void Deserialize(NetDataReader reader)
        {
            characterName = reader.GetString();
            skip = reader.GetPackedInt();
            limit = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(characterName);
            writer.PutPackedInt(skip);
            writer.PutPackedInt(limit);
        }
    }
}
