using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct CharacterSkill : INetSerializable
    {
        public static readonly CharacterSkill Empty = new CharacterSkill();
        public int dataId;
        public int level;

        public CharacterSkill Clone()
        {
            return new CharacterSkill()
            {
                dataId = dataId,
                level = level,
            };
        }

        public static CharacterSkill Create(int dataId, int level = 1)
        {
            return new CharacterSkill()
            {
                dataId = dataId,
                level = level,
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(dataId);
            writer.PutPackedInt(level);
        }

        public void Deserialize(NetDataReader reader)
        {
            dataId = reader.GetPackedInt();
            level = reader.GetPackedInt();
        }
    }
}
