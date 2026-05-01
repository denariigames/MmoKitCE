using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct UpdateGuildMessage : INetSerializable
    {
        public enum UpdateType : byte
        {
            Create,
            ChangeLeader,
            SetGuildMessage,
            SetGuildMessage2,
            SetGuildRole,
            SetGuildMemberRole,
            SetSkillLevel,
            LevelExpSkillPoint,
            Terminate,
            SetGold,
            SetScore,
            SetOptions,
            SetAutoAcceptRequests,
            SetRank,
            UpdateStorage,
            Member,
        }
        public UpdateType type;
        public int id;
        public string guildName;
        public string guildMessage;
        public string characterId;
        public byte guildRole;
        public GuildRoleData guildRoleData;
        public int level;
        public int exp;
        public int skillPoint;
        public int gold;
        public int score;
        public string options;
        public bool autoAcceptRequests;
        public int rank;
        public int dataId;

        public void Deserialize(NetDataReader reader)
        {
            type = (UpdateType)reader.GetByte();
            id = reader.GetPackedInt();
            switch (type)
            {
                case UpdateType.Create:
                    guildName = reader.GetString();
                    characterId = reader.GetString();
                    break;
                case UpdateType.ChangeLeader:
                    characterId = reader.GetString();
                    break;
                case UpdateType.SetGuildMessage:
                case UpdateType.SetGuildMessage2:
                    guildMessage = reader.GetString();
                    break;
                case UpdateType.SetGuildRole:
                    guildRole = reader.GetByte();
                    guildRoleData = reader.Get<GuildRoleData>();
                    break;
                case UpdateType.SetGuildMemberRole:
                    characterId = reader.GetString();
                    guildRole = reader.GetByte();
                    break;
                case UpdateType.SetSkillLevel:
                    dataId = reader.GetPackedInt();
                    level = reader.GetPackedInt();
                    break;
                case UpdateType.SetGold:
                    gold = reader.GetPackedInt();
                    break;
                case UpdateType.SetScore:
                    score = reader.GetPackedInt();
                    break;
                case UpdateType.SetOptions:
                    options = reader.GetString();
                    break;
                case UpdateType.SetAutoAcceptRequests:
                    autoAcceptRequests = reader.GetBool();
                    break;
                case UpdateType.SetRank:
                    rank = reader.GetPackedInt();
                    break;
                case UpdateType.LevelExpSkillPoint:
                    level = reader.GetPackedInt();
                    exp = reader.GetPackedInt();
                    skillPoint = reader.GetPackedInt();
                    break;
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)type);
            writer.PutPackedInt(id);
            switch (type)
            {
                case UpdateType.Create:
                    writer.Put(guildName);
                    writer.Put(characterId);
                    break;
                case UpdateType.ChangeLeader:
                    writer.Put(characterId);
                    break;
                case UpdateType.SetGuildMessage:
                case UpdateType.SetGuildMessage2:
                    writer.Put(guildMessage);
                    break;
                case UpdateType.SetGuildRole:
                    writer.Put(guildRole);
                    writer.Put(guildRoleData);
                    break;
                case UpdateType.SetGuildMemberRole:
                    writer.Put(characterId);
                    writer.Put(guildRole);
                    break;
                case UpdateType.SetSkillLevel:
                    writer.PutPackedInt(dataId);
                    writer.PutPackedInt(level);
                    break;
                case UpdateType.SetGold:
                    writer.PutPackedInt(gold);
                    break;
                case UpdateType.SetScore:
                    writer.PutPackedInt(score);
                    break;
                case UpdateType.SetOptions:
                    writer.Put(options);
                    break;
                case UpdateType.SetAutoAcceptRequests:
                    writer.Put(autoAcceptRequests);
                    break;
                case UpdateType.SetRank:
                    writer.PutPackedInt(rank);
                    break;
                case UpdateType.LevelExpSkillPoint:
                    writer.PutPackedInt(level);
                    writer.PutPackedInt(exp);
                    writer.PutPackedInt(skillPoint);
                    break;
            }
        }
    }
}
