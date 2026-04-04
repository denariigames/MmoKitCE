using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class PlayerCharacterData : CharacterData, IPlayerCharacterData, INetSerializable
    {
        private List<CharacterHotkey> _hotkeys = new List<CharacterHotkey>();
        private List<CharacterQuest> _quests = new List<CharacterQuest>();
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
        private List<CharacterCurrency> _currencies = new List<CharacterCurrency>();
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
        private List<CharacterDataBoolean> _serverBools = new List<CharacterDataBoolean>();
        private List<CharacterDataInt32> _serverInts = new List<CharacterDataInt32>();
        private List<CharacterDataFloat32> _serverFloats = new List<CharacterDataFloat32>();
        private List<CharacterDataBoolean> _privateBools = new List<CharacterDataBoolean>();
        private List<CharacterDataInt32> _privateInts = new List<CharacterDataInt32>();
        private List<CharacterDataFloat32> _privateFloats = new List<CharacterDataFloat32>();
        private List<CharacterDataBoolean> _publicBools = new List<CharacterDataBoolean>();
        private List<CharacterDataInt32> _publicInts = new List<CharacterDataInt32>();
        private List<CharacterDataFloat32> _publicFloats = new List<CharacterDataFloat32>();
#endif
        private List<CharacterSkill> _guildSkills = new List<CharacterSkill>();

        public string UserId { get; set; }
        public float StatPoint { get; set; }
        public float SkillPoint { get; set; }
        public int Gold { get; set; }
        public int UserGold { get; set; }
        public int UserCash { get; set; }
        public int PartyId { get; set; }
        public int GuildId { get; set; }
        public byte GuildRole { get; set; }
        public string CurrentChannel { get; set; }
        public string CurrentMapName { get; set; }
        public Vec3 CurrentPosition { get; set; }
        public Vec3 CurrentRotation { get; set; }
        public string CurrentSafeArea { get; set; }
#if !DISABLE_DIFFER_MAP_RESPAWNING
        public string RespawnMapName { get; set; }
        public Vec3 RespawnPosition { get; set; }
#endif
        public long LastDeadTime { get; set; }
        public long UnmuteTime { get; set; }
        public long LastUpdate { get; set; }
#if !DISABLE_CLASSIC_PK
        public bool IsPkOn { get; set; }
        public long LastPkOnTime { get; set; }
        public int PkPoint { get; set; }
        public int ConsecutivePkKills { get; set; }
        public int HighestPkPoint { get; set; }
        public int HighestConsecutivePkKills { get; set; }
#endif

        public IList<CharacterHotkey> Hotkeys
        {
            get { return _hotkeys; }
            set
            {
                _hotkeys = new List<CharacterHotkey>();
                if (value != null)
                    _hotkeys.AddRange(value);
            }
        }

        public IList<CharacterQuest> Quests
        {
            get { return _quests; }
            set
            {
                _quests = new List<CharacterQuest>();
                if (value != null)
                    _quests.AddRange(value);
            }
        }

#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
        public IList<CharacterCurrency> Currencies
        {
            get { return _currencies; }
            set
            {
                _currencies = new List<CharacterCurrency>();
                if (value != null)
                    _currencies.AddRange(value);
            }
        }
#endif

#if !DISABLE_CUSTOM_CHARACTER_DATA
        public IList<CharacterDataBoolean> ServerBools
        {
            get { return _serverBools; }
            set
            {
                _serverBools = new List<CharacterDataBoolean>();
                if (value != null)
                    _serverBools.AddRange(value);
            }
        }

        public IList<CharacterDataInt32> ServerInts
        {
            get { return _serverInts; }
            set
            {
                _serverInts = new List<CharacterDataInt32>();
                if (value != null)
                    _serverInts.AddRange(value);
            }
        }

        public IList<CharacterDataFloat32> ServerFloats
        {
            get { return _serverFloats; }
            set
            {
                _serverFloats = new List<CharacterDataFloat32>();
                if (value != null)
                    _serverFloats.AddRange(value);
            }
        }

        public IList<CharacterDataBoolean> PrivateBools
        {
            get { return _privateBools; }
            set
            {
                _privateBools = new List<CharacterDataBoolean>();
                if (value != null)
                    _privateBools.AddRange(value);
            }
        }

        public IList<CharacterDataInt32> PrivateInts
        {
            get { return _privateInts; }
            set
            {
                _privateInts = new List<CharacterDataInt32>();
                if (value != null)
                    _privateInts.AddRange(value);
            }
        }

        public IList<CharacterDataFloat32> PrivateFloats
        {
            get { return _privateFloats; }
            set
            {
                _privateFloats = new List<CharacterDataFloat32>();
                if (value != null)
                    _privateFloats.AddRange(value);
            }
        }

        public IList<CharacterDataBoolean> PublicBools
        {
            get { return _publicBools; }
            set
            {
                _publicBools = new List<CharacterDataBoolean>();
                if (value != null)
                    _publicBools.AddRange(value);
            }
        }

        public IList<CharacterDataInt32> PublicInts
        {
            get { return _publicInts; }
            set
            {
                _publicInts = new List<CharacterDataInt32>();
                if (value != null)
                    _publicInts.AddRange(value);
            }
        }

        public IList<CharacterDataFloat32> PublicFloats
        {
            get { return _publicFloats; }
            set
            {
                _publicFloats = new List<CharacterDataFloat32>();
                if (value != null)
                    _publicFloats.AddRange(value);
            }
        }
#endif

        public IList<CharacterSkill> GuildSkills
        {
            get { return _guildSkills; }
            set
            {
                _guildSkills = new List<CharacterSkill>();
                if (value != null)
                    _guildSkills.AddRange(value);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            this.DeserializeCharacterData(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            this.SerializeCharacterData(writer);
        }
    }

    public class PlayerCharacterDataLastUpdateComparer : IComparer<PlayerCharacterData>
    {
        private int _sortMultiplier = 1;

        public PlayerCharacterDataLastUpdateComparer Asc()
        {
            _sortMultiplier = 1;
            return this;
        }

        public PlayerCharacterDataLastUpdateComparer Desc()
        {
            _sortMultiplier = -1;
            return this;
        }

        public int Compare(PlayerCharacterData x, PlayerCharacterData y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null && y != null)
                return -1;

            if (x != null && y == null)
                return 1;

            return x.LastUpdate.CompareTo(y.LastUpdate) * _sortMultiplier;
        }
    }
}
