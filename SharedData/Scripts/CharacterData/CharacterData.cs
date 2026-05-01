using Newtonsoft.Json;
using NotifiableCollection;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class CharacterData : ICharacterData
    {
        private int _dataId;
        private int _entityId;
        private int _level;
        private byte _equipWeaponSet;
        private NotifiableList<EquipWeapons> _selectableEquipWeapons;
        private NotifiableList<CharacterAttribute> _attributes;
        private NotifiableList<CharacterSkill> _skills;
        private List<CharacterSkillUsage> _skillUsages;
        private NotifiableList<CharacterBuff> _buffs;
        private NotifiableList<CharacterItem> _equipItems;
        private NotifiableList<CharacterItem> _nonEquipItems;
        private NotifiableList<CharacterSummon> _summons;
        private int _titleDataId;
        private int _factionId;
        private CharacterMount _mount;

        ~CharacterData()
        {
#if !NET && !NETCOREAPP
            if (_selectableEquipWeapons != null)
            {
                _selectableEquipWeapons.Clear();
                _selectableEquipWeapons = null;
            }
            if (_attributes != null)
            {
                _attributes.Clear();
                _attributes = null;
            }
            if (_skills != null)
            {
                _skills.Clear();
                _skills = null;
            }
            if (_skillUsages != null)
            {
                _skillUsages.Clear();
                _skillUsages = null;
            }
            if (_buffs != null)
            {
                _buffs.Clear();
                _buffs = null;
            }
            if (_equipItems != null)
            {
                _equipItems.Clear();
                _equipItems = null;
            }
            if (_nonEquipItems != null)
            {
                _nonEquipItems.Clear();
                _nonEquipItems = null;
            }
            if (_summons != null)
            {
                _summons.Clear();
                _summons = null;
            }
            this.RemoveCaches();
#endif
        }

        public string Id { get; set; }
        public int DataId
        {
            get { return _dataId; }
            set
            {
                _dataId = value;
#if !NET && !NETCOREAPP
                this.MarkToMakeCaches();
#endif
            }
        }
        public int EntityId
        {
            get { return _entityId; }
            set
            {
                _entityId = value;
#if !NET && !NETCOREAPP
                this.MarkToMakeCaches();
#endif
            }
        }
        public string CharacterName { get; set; }
        public string Title
        {
            get { return CharacterName; }
            set { CharacterName = value; }
        }
        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
#if !NET && !NETCOREAPP
                this.MarkToMakeCaches();
#endif
            }
        }
        public int Exp { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }
        public int CurrentStamina { get; set; }
        public int CurrentFood { get; set; }
        public int CurrentWater { get; set; }
        public int IconDataId { get; set; }
        public int FrameDataId { get; set; }
        public int BackgroundDataId { get; set; }
        public int TitleDataId
        {
            get { return _titleDataId; }
            set
            {
                _titleDataId = value;
#if !NET && !NETCOREAPP
                this.MarkToMakeCaches();
#endif
            }
        }
        public int FactionId
        {
            get { return _factionId; }
            set
            {
                _factionId = value;
#if !NET && !NETCOREAPP
                this.MarkToMakeCaches();
#endif
            }
        }

        public int Reputation { get; set; }

        [JsonIgnore]
        public EquipWeapons EquipWeapons
        {
            get
            {
                if (EquipWeaponSet < SelectableWeaponSets.Count)
                    return SelectableWeaponSets[EquipWeaponSet];
                return new EquipWeapons();
            }
            set
            {
                this.FillWeaponSetsIfNeeded(EquipWeaponSet);
                SelectableWeaponSets[EquipWeaponSet] = value;
            }
        }

        public byte EquipWeaponSet
        {
            get { return _equipWeaponSet; }
            set
            {
                _equipWeaponSet = value;
#if !NET && !NETCOREAPP
                this.MarkToMakeCaches();
#endif
            }
        }

        public IList<EquipWeapons> SelectableWeaponSets
        {
            get
            {
                if (_selectableEquipWeapons == null)
                {
                    _selectableEquipWeapons = new NotifiableList<EquipWeapons>();
                    _selectableEquipWeapons.ListChangedWithoutItem += List_ListChanged;
                }
                return _selectableEquipWeapons;
            }
            set
            {
                if (_selectableEquipWeapons == null)
                {
                    _selectableEquipWeapons = new NotifiableList<EquipWeapons>();
                    _selectableEquipWeapons.ListChangedWithoutItem += List_ListChanged;
                }
                _selectableEquipWeapons.Clear();
                if (value != null)
                {
                    foreach (EquipWeapons entry in value)
                        _selectableEquipWeapons.Add(entry);
                }
            }
        }

        public IList<CharacterAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new NotifiableList<CharacterAttribute>();
                    _attributes.ListChangedWithoutItem += List_ListChanged;
                }
                return _attributes;
            }
            set
            {
                if (_attributes == null)
                {
                    _attributes = new NotifiableList<CharacterAttribute>();
                    _attributes.ListChangedWithoutItem += List_ListChanged;
                }
                _attributes.Clear();
                if (value != null)
                {
                    foreach (CharacterAttribute entry in value)
                        _attributes.Add(entry);
                }
            }
        }

        public IList<CharacterSkill> Skills
        {
            get
            {
                if (_skills == null)
                {
                    _skills = new NotifiableList<CharacterSkill>();
                    _skills.ListChangedWithoutItem += List_ListChanged;
                }
                return _skills;
            }
            set
            {
                if (_skills == null)
                {
                    _skills = new NotifiableList<CharacterSkill>();
                    _skills.ListChangedWithoutItem += List_ListChanged;
                }
                _skills.Clear();
                if (value != null)
                {
                    foreach (CharacterSkill entry in value)
                        _skills.Add(entry);
                }
            }
        }

        public IList<CharacterSkillUsage> SkillUsages
        {
            get
            {
                if (_skillUsages == null)
                    _skillUsages = new List<CharacterSkillUsage>();
                return _skillUsages;
            }
            set
            {
                if (_skillUsages == null)
                    _skillUsages = new List<CharacterSkillUsage>();
                _skillUsages.Clear();
                if (value != null)
                {
                    foreach (CharacterSkillUsage entry in value)
                        _skillUsages.Add(entry);
                }
            }
        }

        public IList<CharacterBuff> Buffs
        {
            get
            {
                if (_buffs == null)
                {
                    _buffs = new NotifiableList<CharacterBuff>();
                    _buffs.ListChangedWithoutItem += List_ListChanged;
                }
                return _buffs;
            }
            set
            {
                if (_buffs == null)
                {
                    _buffs = new NotifiableList<CharacterBuff>();
                    _buffs.ListChangedWithoutItem += List_ListChanged;
                }
                _buffs.Clear();
                if (value != null)
                {
                    foreach (CharacterBuff entry in value)
                        _buffs.Add(entry);
                }
            }
        }

        public IList<CharacterItem> EquipItems
        {
            get
            {
                if (_equipItems == null)
                {
                    _equipItems = new NotifiableList<CharacterItem>();
                    _equipItems.ListChangedWithoutItem += List_ListChanged;
                }
                return _equipItems;
            }
            set
            {
                if (_equipItems == null)
                {
                    _equipItems = new NotifiableList<CharacterItem>();
                    _equipItems.ListChangedWithoutItem += List_ListChanged;
                }
                _equipItems.Clear();
                if (value != null)
                {
                    foreach (CharacterItem entry in value)
                        _equipItems.Add(entry);
                }
            }
        }

        public IList<CharacterItem> NonEquipItems
        {
            get
            {
                if (_nonEquipItems == null)
                {
                    _nonEquipItems = new NotifiableList<CharacterItem>();
                    _nonEquipItems.ListChangedWithoutItem += List_ListChanged;
                }
                return _nonEquipItems;
            }
            set
            {
                if (_nonEquipItems == null)
                {
                    _nonEquipItems = new NotifiableList<CharacterItem>();
                    _nonEquipItems.ListChangedWithoutItem += List_ListChanged;
                }
                _nonEquipItems.Clear();
                if (value != null)
                {
                    foreach (CharacterItem entry in value)
                        _nonEquipItems.Add(entry);
                }
            }
        }

        public IList<CharacterSummon> Summons
        {
            get
            {
                if (_summons == null)
                {
                    _summons = new NotifiableList<CharacterSummon>();
                    _summons.ListChangedWithoutItem += List_ListChanged;
                }
                return _summons;
            }
            set
            {
                if (_summons == null)
                {
                    _summons = new NotifiableList<CharacterSummon>();
                    _summons.ListChangedWithoutItem += List_ListChanged;
                }
                _summons.Clear();
                if (value != null)
                {
                    foreach (CharacterSummon entry in value)
                        _summons.Add(entry);
                }
            }
        }

        public CharacterMount Mount
        {
            get { return _mount; }
            set
            {
                _mount = value;
#if !NET && !NETCOREAPP
                this.MarkToMakeCaches();
#endif
            }
        }

        private void List_ListChanged(NotifiableListAction action, int index)
        {
#if !NET && !NETCOREAPP
            this.MarkToMakeCaches();
#endif
        }
    }
}
