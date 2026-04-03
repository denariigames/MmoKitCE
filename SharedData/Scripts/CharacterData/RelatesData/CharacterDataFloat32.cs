using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct CharacterDataFloat32 : INetSerializable
    {
        public int hashedKey;
        public float value;

        public CharacterDataFloat32 Clone()
        {
            return new CharacterDataFloat32()
            {
                hashedKey = hashedKey,
                value = value,
            };
        }

        public static CharacterDataFloat32 Create(int hashedKey, float value = 0f)
        {
            return new CharacterDataFloat32()
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
            value = reader.GetFloat();
        }
    }
}