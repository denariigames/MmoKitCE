using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.SOCIAL_SYSTEM_SETTING_FILE, menuName = GameDataMenuConsts.SOCIAL_SYSTEM_SETTING_MENU, order = GameDataMenuConsts.SOCIAL_SYSTEM_SETTING_ORDER)]
    public partial class SocialSystemSetting : ScriptableObject
    {
        [Header("Party Configs")]
        [SerializeField]
        private int maxPartyMember = 8;
        [SerializeField]
        private bool partyMemberCanInvite = false;
        [SerializeField]
        private bool partyMemberCanKick = false;
        [SerializeField]
        private float partyInvitationTimeout = 10f;

        public int MaxPartyMember { get { return maxPartyMember; } }
        public bool PartyMemberCanInvite { get { return partyMemberCanInvite; } }
        public bool PartyMemberCanKick { get { return partyMemberCanKick; } }
        public float PartyInvitationTimeout { get { return partyInvitationTimeout; } }

        [Header("Guild Configs")]
        [SerializeField]
        private int maxGuildMember = 50;
        [SerializeField]
        private int minGuildNameLength = 2;
        [SerializeField]
        private int maxGuildNameLength = 16;
        [SerializeField]
        private int minGuildRoleNameLength = 2;
        [SerializeField]
        private int maxGuildRoleNameLength = 16;
        [SerializeField]
        private int maxGuildMessageLength = 140;
        [SerializeField]
        private int maxGuildMessage2Length = 140;
        [Tooltip("Member roles from high to low priority")]
        [SerializeField]
        private GuildRoleData[] guildMemberRoles = new GuildRoleData[] {
            new GuildRoleData() { roleName = "Master", canInvite = true, canKick = true, canUseStorage = true },
            new GuildRoleData() { roleName = "Member 1", canInvite = false, canKick = false, canUseStorage = false },
            new GuildRoleData() { roleName = "Member 2", canInvite = false, canKick = false, canUseStorage = false },
            new GuildRoleData() { roleName = "Member 3", canInvite = false, canKick = false, canUseStorage = false },
            new GuildRoleData() { roleName = "Member 4", canInvite = false, canKick = false, canUseStorage = false },
            new GuildRoleData() { roleName = "Member 5", canInvite = false, canKick = false, canUseStorage = false },
        };
        [Range(0, 100)]
        [SerializeField]
        private byte maxShareExpPercentage = 20;
        [SerializeField]
        [ArrayElementTitle("item")]
        private ItemAmount[] createGuildRequireItems = new ItemAmount[0];
        [SerializeField]
        [ArrayElementTitle("currency")]
        private CurrencyAmount[] createGuildRequireCurrencies = new CurrencyAmount[0];
        [SerializeField]
        private int createGuildRequiredGold = 1000;
        [SerializeField]
        private int createGuildRequiredCash = 0;
        [SerializeField]
        [Tooltip("Exp tree for guild, this may be deprecated in the future, you should setup `Exp Table` instead.")]
        private int[] guildExpTree;
        [SerializeField]
        private ExpTable guildExpTable;
        [SerializeField]
        private float guildInvitationTimeout = 10f;

        public int MaxGuildMember { get { return maxGuildMember; } }
        public int MinGuildNameLength { get { return minGuildNameLength; } }
        public int MaxGuildNameLength { get { return maxGuildNameLength; } }
        public int MinGuildRoleNameLength { get { return minGuildRoleNameLength; } }
        public int MaxGuildRoleNameLength { get { return maxGuildRoleNameLength; } }
        public int MaxGuildMessageLength { get { return maxGuildMessageLength; } }
        public int MaxGuildMessage2Length { get { return maxGuildMessage2Length; } }
        public GuildRoleData[] GuildMemberRoles { get { return guildMemberRoles; } }
        public byte MaxShareExpPercentage { get { return maxShareExpPercentage; } }

        [System.NonSerialized]
        private Dictionary<BaseItem, int> _createGuildRequireItems = null;
        public Dictionary<BaseItem, int> CreateGuildRequireItems
        {
            get
            {
                if (_createGuildRequireItems == null)
                {
                    _createGuildRequireItems = new Dictionary<BaseItem, int>();
                    GameDataHelpers.CombineItems(createGuildRequireItems, _createGuildRequireItems);
                }
                return _createGuildRequireItems;
            }
        }

        [System.NonSerialized]
        private Dictionary<Currency, int> _createGuildRequireCurrencies = null;
        public Dictionary<Currency, int> CreateGuildRequireCurrencies
        {
            get
            {
                if (_createGuildRequireCurrencies == null)
                {
                    _createGuildRequireCurrencies = new Dictionary<Currency, int>();
                    GameDataHelpers.CombineCurrencies(createGuildRequireCurrencies, _createGuildRequireCurrencies, 1f);
                }
                return _createGuildRequireCurrencies;
            }
        }

        public int CreateGuildRequiredGold => createGuildRequiredGold;

        public int CreateGuildRequiredCash => createGuildRequiredCash;

        public ExpTable GuildExpTable
        {
            get { return guildExpTable; }
            set { guildExpTable = value; }
        }
        public float GuildInvitationTimeout { get { return guildInvitationTimeout; } }

        public bool CanCreateGuild(IPlayerCharacterData character)
        {
            return CanCreateGuild(character, out _);
        }

        public bool CanCreateGuild(IPlayerCharacterData character, out UITextKeys gameMessage)
        {
            gameMessage = UITextKeys.NONE;
            if (!GameInstance.Singleton.GameplayRule.CurrenciesEnoughToCreateGuild(character, this))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                return false;
            }
            if (createGuildRequireItems == null || createGuildRequireItems.Length == 0)
            {
                // No required items
                return true;
            }
            foreach (ItemAmount requireItem in createGuildRequireItems)
            {
                if (requireItem.item != null && character.CountNonEquipItems(requireItem.item.DataId) < requireItem.amount)
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                    return false;
                }
            }
            return true;
        }

        public void DecreaseCreateGuildResource(IPlayerCharacterData character)
        {
            if (createGuildRequireItems != null)
            {
                foreach (ItemAmount requireItem in createGuildRequireItems)
                {
                    if (requireItem.item != null && requireItem.amount > 0)
                        character.DecreaseItems(requireItem.item.DataId, requireItem.amount);
                }
                character.FillEmptySlots();
            }
            // Decrease required gold
            GameInstance.Singleton.GameplayRule.DecreaseCurrenciesWhenCreateGuild(character, this);
        }

        public void Migrate()
        {
            if (guildExpTable == null)
            {
                guildExpTable = CreateInstance<ExpTable>();
                guildExpTable.expTree = guildExpTree;
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (guildMemberRoles.Length < 2)
            {
                Debug.LogWarning($"[{nameof(SocialSystemSetting)}] `Guild Member Roles` must more or equals to 2");
                guildMemberRoles = new GuildRoleData[] {
                    guildMemberRoles[0],
                    new GuildRoleData() { roleName = "Member 1", canInvite = false, canKick = false, canUseStorage = false },
                };
                EditorUtility.SetDirty(this);
            }
            else if (guildMemberRoles.Length < 1)
            {
                Debug.LogWarning($"[{nameof(SocialSystemSetting)}] `Guild Member Roles` must more or equals to 2");
                guildMemberRoles = new GuildRoleData[] {
                    new GuildRoleData() { roleName = "Master", canInvite = true, canKick = true, canUseStorage = true },
                    new GuildRoleData() { roleName = "Member 1", canInvite = false, canKick = false, canUseStorage = false },
                };
                EditorUtility.SetDirty(this);
            }
            // Clear cache
            _createGuildRequireItems = null;
            _createGuildRequireCurrencies = null;
        }
#endif
    }
}
