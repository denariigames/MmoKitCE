using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public enum SkillUsageType : byte
    {
        Skill,
        GuildSkill,
        UsableItem,
    }

    [System.Serializable]
    public partial struct CharacterSkillUsage : INetSerializable
    {
        public static readonly CharacterSkillUsage Empty = new CharacterSkillUsage();
        public SkillUsageType type;
        public int dataId;
        public float coolDownRemainsDuration;

        public CharacterSkillUsage Clone()
        {
            return new CharacterSkillUsage()
            {
                type = type,
                dataId = dataId,
                coolDownRemainsDuration = coolDownRemainsDuration,
            };
        }

        public static CharacterSkillUsage Create(SkillUsageType type, int dataId)
        {
            return new CharacterSkillUsage()
            {
                type = type,
                dataId = dataId,
                coolDownRemainsDuration = 0f,
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)type);
            writer.PutPackedInt(dataId);
            writer.Put(coolDownRemainsDuration);
        }

        public void Deserialize(NetDataReader reader)
        {
            type = (SkillUsageType)reader.GetByte();
            dataId = reader.GetPackedInt();
            coolDownRemainsDuration = reader.GetFloat();
        }
    }
}
