using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class LanguageManager : MonoBehaviour
    {
        public const string RETURN_KEY_AS_DEFAULT_VALUE_PREFIX = "K->";
        public static Dictionary<string, Dictionary<string, string>> Languages { get; protected set; } = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, string> Texts { get; protected set; } = new Dictionary<string, string>();
        public static string CurrentLanguageKey { get; protected set; }
        private static string currentPlayerPrefsKey = string.Empty;

        [Header("Language Manager Configs")]
        public string defaultLanguageKey = "ENG";
        public string playerPrefsKey = "USER_LANG";
        public List<Language> languageList = new List<Language>();

        [Header("Add New Language")]
        [Tooltip("You can add new language by `Add New Language` context menu")]
        public string newLanguageKey;
        [InspectorButton(nameof(AddNewLanguage))]
        public bool addNewLanguage;
        [InspectorButton(nameof(MigrateOldGameMessage))]
        public bool migrateOldGameMessage;

        [Header("Set Message Tool")]
        public string setMessageLanguageKey;
        public string setMessageKey;
        public string setMessageValue;
        [InspectorButton(nameof(SetMessage))]
        public bool setMessage;

        private void Awake()
        {
            currentPlayerPrefsKey = playerPrefsKey;
            CurrentLanguageKey = PlayerPrefs.GetString(currentPlayerPrefsKey, defaultLanguageKey);
            Languages.Clear();
            Dictionary<string, string> tempNewData;
            foreach (Language language in languageList)
            {
                tempNewData = new Dictionary<string, string>();
                foreach (LanguageData data in language.dataList)
                {
                    if (tempNewData.ContainsKey(data.key))
                    {
                        Debug.LogWarning("[LanguageManager] Language " + language.languageKey + " already contains " + data.key);
                        continue;
                    }
                    tempNewData.Add(data.key, data.value);
                }
                Languages[language.languageKey] = tempNewData;
            }
            ChangeLanguage(CurrentLanguageKey);
        }

        public Language GetLanguageFromList(string languageKey)
        {
            foreach (Language language in languageList)
            {
                if (language.languageKey == languageKey)
                    return language;
            }
            return null;
        }

        [ContextMenu("Add New Language")]
        public void AddNewLanguage()
        {
            if (string.IsNullOrEmpty(newLanguageKey))
            {
                Debug.LogWarning("`New Language Key` is null or empty");
                return;
            }

            Language newLang = GetLanguageFromList(newLanguageKey);
            if (newLang == null)
            {
                newLang = new Language();
                newLang.languageKey = newLanguageKey;
                languageList.Add(newLang);
            }

            List<string> keys = new List<string>(DefaultLocale.Texts.Keys);
            keys.Sort();
            foreach (string key in keys)
            {
                if (newLang.ContainKey(key))
                    continue;

                newLang.dataList.Add(new LanguageData()
                {
                    key = key,
                    value = DefaultLocale.Texts[key],
                });
            }
        }

        [ContextMenu("Migrate Old Game Message")]
        public void MigrateOldGameMessage()
        {
#if UNITY_EDITOR
            foreach (Language language in languageList)
            {
                for (int i = 0; i < language.dataList.Count; ++i)
                {
                    LanguageData data = language.dataList[i];
                    if ("ServiceNotAvailable".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE.ToString();
                    if ("RequestTimeout".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_REQUEST_TIMEOUT.ToString();
                    if ("InvalidItemData".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_INVALID_ITEM_DATA.ToString();
                    if ("InvalidAttributeData".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_INVALID_ATTRIBUTE_DATA.ToString();
                    if ("InvalidSkillData".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_INVALID_SKILL_DATA.ToString();
                    if ("InvalidGuildSkillData".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_INVALID_GUILD_SKILL_DATA.ToString();
                    if ("NotFoundCharacter".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CHARACTER_NOT_FOUND.ToString();
                    if ("NotAbleToLoot".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ABLE_TO_LOOT.ToString();
                    if ("NotEnoughGold".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD.ToString();
                    if ("NotEnoughItems".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS.ToString();
                    if ("CannotCarryAnymore".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_WILL_OVERWHELMING.ToString();
                    if ("CannotEquip".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_EQUIP.ToString();
                    if ("InvalidEquipPositionRightHand".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_RIGHT_HAND.ToString();
                    if ("InvalidEquipPositionLeftHand".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_LEFT_HAND.ToString();
                    if ("InvalidEquipPositionRightHandOrLeftHand".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_RIGHT_HAND_OR_LEFT_HAND.ToString();
                    if ("InvalidEquipPositionArmor".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_ARMOR.ToString();
                    if ("CannotRefine".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_REFINE.ToString();
                    if ("RefineItemReachedMaxLevel".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_REFINE_ITEM_REACHED_MAX_LEVEL.ToString();
                    if ("RefineSuccess".Equals(data.key))
                        data.key = UITextKeys.UI_REFINE_SUCCESS.ToString();
                    if ("RefineFail".Equals(data.key))
                        data.key = UITextKeys.UI_REFINE_FAIL.ToString();
                    if ("CannotEnhanceSocket".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_ENHANCE_SOCKET.ToString();
                    if ("NotEnoughSocketEnchaner".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_SOCKET_ENCHANER.ToString();
                    if ("NoEmptySocket".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NO_EMPTY_SOCKET.ToString();
                    if ("SocketNotEmpty".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_SOCKET_NOT_EMPTY.ToString();
                    if ("CannotRemoveEnhancer".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_REMOVE_ENHANCER.ToString();
                    if ("NoEnhancer".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NO_ENHANCER.ToString();
                    if ("CannotRepair".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_REPAIR.ToString();
                    if ("RepairSuccess".Equals(data.key))
                        data.key = UITextKeys.UI_REPAIR_SUCCESS.ToString();
                    if ("CharacterIsInAnotherDeal".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CHARACTER_IS_DEALING.ToString();
                    if ("CharacterIsTooFar".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR.ToString();
                    if ("CannotAcceptDealingRequest".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_ACCEPT_DEALING_REQUEST.ToString();
                    if ("DealingRequestDeclined".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_DEALING_REQUEST_DECLINED.ToString();
                    if ("InvalidDealingState".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_INVALID_DEALING_STATE.ToString();
                    if ("DealingCanceled".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_DEALING_CANCELED.ToString();
                    if ("AnotherCharacterCannotCarryAnymore".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_ANOTHER_CHARACTER_WILL_OVERWHELMING.ToString();
                    if ("NotFoundParty".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_PARTY_NOT_FOUND.ToString();
                    if ("NotFoundPartyInvitation".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_PARTY_INVITATION_NOT_FOUND.ToString();
                    if ("PartyInvitationAccepted".Equals(data.key))
                        data.key = UITextKeys.UI_PARTY_INVITATION_ACCEPTED.ToString();
                    if ("PartyInvitationDeclined".Equals(data.key))
                        data.key = UITextKeys.UI_PARTY_INVITATION_DECLINED.ToString();
                    if ("CannotSendPartyInvitation".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_SEND_PARTY_INVITATION.ToString();
                    if ("CannotKickPartyMember".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_KICK_PARTY_MEMBER.ToString();
                    if ("CannotKickYourSelfFromParty".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_KICK_YOURSELF_FROM_PARTY.ToString();
                    if ("CannotKickPartyLeader".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_KICK_PARTY_LEADER.ToString();
                    if ("JoinedAnotherParty".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_JOINED_ANOTHER_PARTY.ToString();
                    if ("NotJoinedParty".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_JOINED_PARTY.ToString();
                    if ("NotPartyLeader".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_PARTY_LEADER.ToString();
                    if ("CharacterJoinedAnotherParty".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CHARACTER_JOINED_ANOTHER_PARTY.ToString();
                    if ("CharacterNotJoinedParty".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CHARACTER_NOT_JOINED_PARTY.ToString();
                    if ("PartyMemberReachedLimit".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_PARTY_MEMBER_REACHED_LIMIT.ToString();
                    if ("NotFoundGuild".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_NOT_FOUND.ToString();
                    if ("NotFoundGuildInvitation".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_INVITATION_NOT_FOUND.ToString();
                    if ("GuildInvitationAccepted".Equals(data.key))
                        data.key = UITextKeys.UI_GUILD_INVITATION_ACCEPTED.ToString();
                    if ("GuildInvitationDeclined".Equals(data.key))
                        data.key = UITextKeys.UI_GUILD_INVITATION_DECLINED.ToString();
                    if ("CannotSendGuildInvitation".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_SEND_GUILD_INVITATION.ToString();
                    if ("CannotKickGuildMember".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_KICK_GUILD_MEMBER.ToString();
                    if ("CannotKickYourSelfFromGuild".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_KICK_YOURSELF_FROM_GUILD.ToString();
                    if ("CannotKickGuildLeader".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_KICK_GUILD_LEADER.ToString();
                    if ("CannotKickHigherGuildMember".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_KICK_HIGHER_GUILD_MEMBER.ToString();
                    if ("JoinedAnotherGuild".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_JOINED_ANOTHER_GUILD.ToString();
                    if ("NotJoinedGuild".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_JOINED_GUILD.ToString();
                    if ("NotGuildLeader".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_GUILD_LEADER.ToString();
                    if ("CharacterJoinedAnotherGuild".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CHARACTER_JOINED_ANOTHER_GUILD.ToString();
                    if ("CharacterNotJoinedGuild".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CHARACTER_NOT_JOINED_GUILD.ToString();
                    if ("GuildMemberReachedLimit".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_MEMBER_REACHED_LIMIT.ToString();
                    if ("GuildRoleNotAvailable".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_ROLE_NOT_AVAILABLE.ToString();
                    if ("GuildSkillReachedMaxLevel".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_SKILL_REACHED_MAX_LEVEL.ToString();
                    if ("NoGuildSkillPoint".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_GUILD_SKILL_POINT.ToString();
                    if ("UnknowGameDataTitle".Equals(data.key))
                        data.key = UITextKeys.UI_UNKNOW_GAME_DATA_TITLE.ToString();
                    if ("UnknowGameDataDescription".Equals(data.key))
                        data.key = UITextKeys.UI_UNKNOW_GAME_DATA_DESCRIPTION.ToString();
                    if ("NotEnoughGoldToDeposit".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD_TO_DEPOSIT.ToString();
                    if ("NotEnoughGoldToWithdraw".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD_TO_WITHDRAW.ToString();
                    if ("CannotAccessStorage".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE.ToString();
                    if ("NoAmmo".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NO_AMMO.ToString();
                    if ("NotEnoughHp".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_HP.ToString();
                    if ("NotEnoughMp".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_MP.ToString();
                    if ("NotEnoughStamina".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_STAMINA.ToString();
                    if ("TooShortGuildName".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_NAME_TOO_SHORT.ToString();
                    if ("TooLongGuildName".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_NAME_TOO_LONG.ToString();
                    if ("ExistedGuildName".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_NAME_EXISTED.ToString();
                    if ("TooShortGuildRoleName".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_ROLE_NAME_TOO_SHORT.ToString();
                    if ("TooLongGuildRoleName".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_ROLE_NAME_TOO_LONG.ToString();
                    if ("TooLongGuildMessage".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_GUILD_MESSAGE_TOO_LONG.ToString();
                    if ("SkillLevelIsZero".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_SKILL_LEVEL_IS_ZERO.ToString();
                    if ("CannotUseSkillByCurrentWeapon".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_CANNOT_USE_SKILL_BY_CURRENT_WEAPON.ToString();
                    if ("SkillIsCoolingDown".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_SKILL_IS_COOLING_DOWN.ToString();
                    if ("SkillIsNotLearned".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_SKILL_IS_NOT_LEARNED.ToString();
                    if ("NoSkillTarget".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NO_SKILL_TARGET.ToString();
                    if ("NotEnoughLevel".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_LEVEL.ToString();
                    if ("NotMatchCharacterClass".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_MATCH_CHARACTER_CLASS.ToString();
                    if ("NotEnoughAttributeAmounts".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_ATTRIBUTE_AMOUNTS.ToString();
                    if ("NotEnoughSkillLevels".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_SKILL_LEVELS.ToString();
                    if ("NotEnoughStatPoint".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_STAT_POINT.ToString();
                    if ("NotEnoughSkillPoint".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_NOT_ENOUGH_SKILL_POINT.ToString();
                    if ("AttributeReachedMaxAmount".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_ATTRIBUTE_REACHED_MAX_AMOUNT.ToString();
                    if ("SkillReachedMaxLevel".Equals(data.key))
                        data.key = UITextKeys.UI_ERROR_SKILL_REACHED_MAX_LEVEL.ToString();
                    if ("UI_SUCCESS_CASH_SHOP_BUY".Equals(data.key))
                        data.key = UITextKeys.UI_CASH_SHOP_ITEM_BOUGHT.ToString();
                    if ("UI_MAIL_SEND_SUCCESS".Equals(data.key))
                        data.key = UITextKeys.UI_MAIL_SENT.ToString();
                    if ("UI_MAIL_CLAIM_SUCCESS".Equals(data.key))
                        data.key = UITextKeys.UI_MAIL_CLAIMED.ToString();
                    if ("UI_MAIL_DELETE_SUCCESS".Equals(data.key))
                        data.key = UITextKeys.UI_MAIL_DELETED.ToString();
                    language.dataList[i] = data;
                }
            }
            EditorUtility.SetDirty(this);
#endif
        }

        [ContextMenu("Set Message")]
        public void SetMessage()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(setMessageLanguageKey) || 
                string.IsNullOrEmpty(setMessageKey))
            {
                Debug.LogError("Cannot set message, `setMessageLanguageKey` and `setMessageKey` must not empty");
                return;
            }
            for (int i = 0; i < languageList.Count; ++i)
            {
                if (setMessageLanguageKey.Equals(languageList[i].languageKey))
                {
                    for (int j = 0; j < languageList[i].dataList.Count; ++j)
                    {
                        if (setMessageKey.Equals(languageList[i].dataList[j].key))
                        {
                            LanguageData data = languageList[i].dataList[j];
                            data.value = setMessageValue;
                            languageList[i].dataList[j] = data;
                            Debug.Log($"Changed message {setMessageLanguageKey}.{setMessageKey} to {setMessageValue}");
                            setMessageLanguageKey = setMessageKey = setMessageValue = string.Empty;
                            EditorUtility.SetDirty(this);
                            return;
                        }
                    }
                    languageList[i].dataList.Add(new LanguageData()
                    {
                        key = setMessageKey,
                        value = setMessageValue,
                    });
                    Debug.Log($"Added new message {setMessageLanguageKey}.{setMessageKey} to {setMessageValue}");
                    setMessageLanguageKey = setMessageKey = setMessageValue = string.Empty;
                    EditorUtility.SetDirty(this);
                    return;
                }
            }
#endif
        }

        public static void ChangeLanguage(string languageKey)
        {
            if (!Languages.ContainsKey(languageKey))
                return;

            CurrentLanguageKey = languageKey;
            Texts = Languages[languageKey];
            PlayerPrefs.SetString(currentPlayerPrefsKey, CurrentLanguageKey);
            PlayerPrefs.Save();
        }

        public static string GetText(string key, string defaultValue = "")
        {
            if (string.IsNullOrWhiteSpace(key))
                return defaultValue;
            if (Texts.ContainsKey(key))
                return Texts[key];
            if (DefaultLocale.Texts.ContainsKey(key))
                return DefaultLocale.Texts[key];
            if (key.StartsWith(RETURN_KEY_AS_DEFAULT_VALUE_PREFIX))
                return key.Substring(RETURN_KEY_AS_DEFAULT_VALUE_PREFIX.Length);
            if (string.IsNullOrWhiteSpace(defaultValue))
                return key;
            return defaultValue;
        }

        public static string GetTextByLanguage(string languageKey, string key, string defaultValue = "")
        {
            if (string.IsNullOrWhiteSpace(languageKey) || string.IsNullOrWhiteSpace(key))
                return defaultValue;
            if (Languages.ContainsKey(languageKey) && Languages[languageKey].ContainsKey(key))
                return Languages[languageKey][key];
            if (DefaultLocale.Texts.ContainsKey(key))
                return DefaultLocale.Texts[key];
            if (key.StartsWith(RETURN_KEY_AS_DEFAULT_VALUE_PREFIX))
                return key.Substring(RETURN_KEY_AS_DEFAULT_VALUE_PREFIX.Length);
            return defaultValue;
        }

        public static string GetUnknowTitle()
        {
            return GetText(UITextKeys.UI_UNKNOW_GAME_DATA_TITLE.ToString(), "Unknow");
        }

        public static string GetUnknowDescription()
        {
            return GetText(UITextKeys.UI_UNKNOW_GAME_DATA_DESCRIPTION.ToString(), "N/A");
        }
    }
}
