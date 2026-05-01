using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public enum SummonType : byte
    {
        None,
        Skill,
        PetItem,
        Custom = 254,
    }

    [System.Serializable]
    public partial struct CharacterSummon : INetSerializable
    {
        public static readonly CharacterSummon Empty = new CharacterSummon();
        public string id;
        public SummonType type;
        public string sourceId;
        public int dataId;
        public float summonRemainsDuration;
        public uint objectId;
        public int level;
        public int exp;
        public int currentHp;
        public int currentMp;

        public CharacterSummon Clone(bool generateNewId = false)
        {
            CharacterSummon result = new CharacterSummon()
            {
                id = generateNewId || string.IsNullOrWhiteSpace(id) ? GenericUtils.GetUniqueId() : id,
                type = type,
                sourceId = sourceId,
                dataId = dataId,
                summonRemainsDuration = summonRemainsDuration,
                objectId = objectId,
                level = level,
                exp = exp,
                currentHp = currentHp,
                currentMp = currentMp,
            };
            return result;
        }

        public static CharacterSummon Create(SummonType type, string sourceId, int dataId)
        {
            return new CharacterSummon()
            {
                id = GenericUtils.GetUniqueId(),
                type = type,
                sourceId = sourceId,
                dataId = dataId,
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(id);
            writer.Put((byte)type);
            if (type != SummonType.None)
            {
                writer.Put(sourceId);
                writer.PutPackedInt(dataId);
                writer.Put(summonRemainsDuration);
                writer.PutPackedUInt(objectId);
                writer.PutPackedInt(level);
                writer.PutPackedInt(exp);
                writer.PutPackedInt(currentHp);
                writer.PutPackedInt(currentMp);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            id = reader.GetString();
            type = (SummonType)reader.GetByte();
            if (type != SummonType.None)
            {
                sourceId = reader.GetString();
                dataId = reader.GetPackedInt();
                summonRemainsDuration = reader.GetFloat();
                objectId = reader.GetPackedUInt();
                level = reader.GetPackedInt();
                exp = reader.GetPackedInt();
                currentHp = reader.GetPackedInt();
                currentMp = reader.GetPackedInt();
            }
        }
    }
}
