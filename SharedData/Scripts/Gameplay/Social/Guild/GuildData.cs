using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class GuildData : SocialGroupData, INetSerializable
    {
        public const byte LeaderRole = 0;

        public string guildName;
        public int level;
        public int exp;
        public int skillPoint;
        public string guildMessage;
        public string guildMessage2;
        public int gold;
        public int score;
        public string options;
        public bool autoAcceptRequests;
        public int rank;
        public List<GuildRoleData> roles;
        public Dictionary<string, byte> memberRoles;
        public Dictionary<int, int> skillLevels;

        public bool IsCached { get; private set; }

        public byte LowestMemberRole
        {
            get
            {
                if (roles == null || roles.Count < 2)
                    return 1;
                return (byte)(roles.Count - 1);
            }
        }

        public GuildData()
            : base()
        {
            guildName = string.Empty;
            level = 1;
            exp = 0;
            skillPoint = 0;
            guildMessage = string.Empty;
            guildMessage2 = string.Empty;
            score = 0;
            gold = 0;
            options = string.Empty;
            autoAcceptRequests = false;
            rank = 0;
            roles = new List<GuildRoleData>();
            memberRoles = new Dictionary<string, byte>();
            skillLevels = new Dictionary<int, int>();
        }

        public GuildData(int id, string guildName, string leaderId, IEnumerable<GuildRoleData> roles)
            : this()
        {
            this.id = id;
            this.guildName = guildName;
            this.leaderId = leaderId;
            this.roles = new List<GuildRoleData>(roles);
            AddMember(new SocialCharacterData() { id = leaderId });
        }

        public GuildData(int id, string guildName, SocialCharacterData leaderCharacter, IEnumerable<GuildRoleData> roles)
            : this(id, guildName, leaderCharacter.id, roles)
        {
            AddMember(leaderCharacter);
        }

        public void AddMember(SocialCharacterData memberData, byte guildRole)
        {
            base.AddMember(memberData);
            SetMemberRole(memberData.id, guildRole);
        }

        public void UpdateMember(SocialCharacterData memberData, byte guildRole)
        {
            base.UpdateMember(memberData);
            SetMemberRole(memberData.id, guildRole);
        }

        public override void AddMember(SocialCharacterData memberData)
        {
            base.AddMember(memberData);
            SetMemberRole(memberData.id, IsLeader(memberData.id) ? LeaderRole : LowestMemberRole);
        }

        public override bool RemoveMember(string characterId)
        {
            memberRoles.Remove(characterId);
            return base.RemoveMember(characterId);
        }

        public override void ClearMembers()
        {
            memberRoles.Clear();
            base.ClearMembers();
        }

        public override void SetLeader(string characterId)
        {
            if (members.ContainsKey(characterId))
            {
                memberRoles[leaderId] = LowestMemberRole;
                leaderId = characterId;
                memberRoles[leaderId] = LeaderRole;
            }
        }

        public bool TryGetMemberRole(string characterId, out byte role)
        {
            return memberRoles.TryGetValue(characterId, out role);
        }

        public void SetMemberRole(string characterId, byte guildRole)
        {
            if (members.ContainsKey(characterId))
            {
                if (!IsRoleAvailable(guildRole))
                    guildRole = IsLeader(characterId) ? LeaderRole : LowestMemberRole;
                // Validate role
                if (guildRole == LeaderRole && !IsLeader(characterId))
                    memberRoles[characterId] = LowestMemberRole;
                else
                    memberRoles[characterId] = guildRole;
            }
        }

        public bool IsRoleAvailable(byte guildRole)
        {
            return roles != null && guildRole >= 0 && guildRole < roles.Count;
        }

        public List<GuildRoleData> GetRoles()
        {
            return roles;
        }

        public GuildRoleData GetRole(byte guildRole)
        {
            if (!IsRoleAvailable(guildRole))
            {
                if (guildRole == LeaderRole)
                    return new GuildRoleData() { roleName = "Master", canInvite = true, canKick = true, canUseStorage = true };
                else
                    return new GuildRoleData() { roleName = "Member", canInvite = false, canKick = false, canUseStorage = false };
            }
            return roles[guildRole];
        }

        public void SetRole(byte guildRole, GuildRoleData role)
        {
            if (guildRole >= 0 && guildRole < roles.Count)
                roles[guildRole] = role;
        }

        public byte GetMemberRole(string characterId)
        {
            byte result;
            if (memberRoles.ContainsKey(characterId))
            {
                result = memberRoles[characterId];
                // Validate member role
                if (result == LeaderRole && !IsLeader(characterId))
                    result = memberRoles[characterId] = LowestMemberRole;
            }
            else
            {
                result = IsLeader(characterId) ? LeaderRole : LowestMemberRole;
            }
            return result;
        }

        public IEnumerable<KeyValuePair<int, int>> GetSkillLevels()
        {
            return skillLevels;
        }

        public bool TryGetSkillLevel(int dataId, out int guildSkillLevel)
        {
            return skillLevels.TryGetValue(dataId, out guildSkillLevel);
        }

        public int GetSkillLevel(int dataId)
        {
            if (skillLevels.ContainsKey(dataId))
                return skillLevels[dataId];
            return 0;
        }

        public bool AddSkillLevel(int dataId)
        {
            if (skillPoint <= 0)
                return false;
            int level = skillLevels.ContainsKey(dataId) ? skillLevels[dataId] : 0;
            level += 1;
            skillPoint -= 1;
            skillLevels[dataId] = level;
            IsCached = false;
            return true;
        }

        public void SetSkillLevel(int dataId, int level)
        {
            skillLevels[dataId] = level;
            IsCached = false;
        }

        public bool IncreaseGuildExp(
            int[] expTree,
            int increasingExp)
        {
            exp += increasingExp;
            bool isLevelUp = false;
            int nextLevelExp = GetNextLevelExp(expTree, level);
            while (nextLevelExp > 0 && exp >= nextLevelExp)
            {
                exp = exp - nextLevelExp;
                ++level;
                nextLevelExp = GetNextLevelExp(expTree, level);
                skillPoint += 1;
                isLevelUp = true;
            }
            return isLevelUp;
        }

        public int GetNextLevelExp(int[] expTree, int level)
        {
            if (level <= 0)
                return 0;
            if (level > expTree.Length)
                return 0;
            return expTree[level - 1];
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.Put(guildName);
            writer.PutPackedInt(level);
            writer.PutPackedInt(exp);
            writer.PutPackedInt(skillPoint);
            writer.Put(guildMessage);
            writer.Put(guildMessage2);
            writer.PutPackedInt(score);
            writer.PutPackedInt(gold);
            writer.Put(options);
            writer.Put(autoAcceptRequests);
            writer.PutPackedInt(rank);
            writer.PutList(roles);
            writer.PutDictionary(memberRoles);
            writer.PutDictionary(skillLevels);
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            guildName = reader.GetString();
            level = reader.GetPackedInt();
            exp = reader.GetPackedInt();
            skillPoint = reader.GetPackedInt();
            guildMessage = reader.GetString();
            guildMessage2 = reader.GetString();
            score = reader.GetPackedInt();
            gold = reader.GetPackedInt();
            options = reader.GetString();
            autoAcceptRequests = reader.GetBool();
            rank = reader.GetPackedInt();
            roles = reader.GetList<GuildRoleData>();
            memberRoles = reader.GetDictionary<string, byte>();
            skillLevels = reader.GetDictionary<int, int>();
        }
    }
}
