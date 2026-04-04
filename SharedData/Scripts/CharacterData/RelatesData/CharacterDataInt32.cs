using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct CharacterDataInt32 : INetSerializable
    {
        public int hashedKey;
        public int value;

        public CharacterDataInt32 Clone()
        {
            return new CharacterDataInt32()
            {
                hashedKey = hashedKey,
                value = value,
            };
        }

        public static CharacterDataInt32 Create(int hashedKey, int value = 0)
        {
            return new CharacterDataInt32()
            {
                hashedKey = hashedKey,
                value = value,
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(hashedKey);
            writer.PutPackedInt(value);
        }

        public void Deserialize(NetDataReader reader)
        {
            hashedKey = reader.GetPackedInt();
            value = reader.GetPackedInt();
        }
    }
}