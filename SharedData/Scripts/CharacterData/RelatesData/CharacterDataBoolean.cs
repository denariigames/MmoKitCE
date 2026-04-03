using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct CharacterDataBoolean : INetSerializable
    {
        public int hashedKey;
        public bool value;

        public CharacterDataBoolean Clone()
        {
            return new CharacterDataBoolean()
            {
                hashedKey = hashedKey,
                value = value,
            };
        }

        public static CharacterDataBoolean Create(int hashedKey, bool value = false)
        {
            return new CharacterDataBoolean()
            {
                hashedKey = hashedKey,
                value = value,
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(hashedKey);
            writer.Put(value);
        }

        public void Deserialize(NetDataReader reader)
        {
            hashedKey = reader.GetPackedInt();
            value = reader.GetBool();
        }
    }
}