using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static class DefaultLocale
    {
        public static readonly Dictionary<string, string> Texts = new Dictionary<string, string>();
        static DefaultLocale()
        {
            // UI Generic Title
            Texts.Add(UITextKeys.UI_LABEL_DISCONNECTED.ToString(), "Disconnected");
            Texts.Add(UITextKeys.UI_LABEL_SUCCESS.ToString(), "Success");
            Texts.Add(UITextKeys.UI_LABEL_ERROR.ToString(), "Error");
            Texts.Add(UITextKeys.UI_LABEL_NONE.ToString(), "None");
            Texts.Add(UITextKeys.UI_LABEL_MESSAGE.ToString(), "Message");
            Texts.Add(UITextKeys.UI_LABEL_WARNING.ToString(), "Warning");
            // Format - Generic
            Texts.Add(UIFormatKeys.UI_FORMAT_SIMPLE.ToString(), "{0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE.ToString(), "{0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_SIMPLE_MIN_TO_MAX.ToString(), "{0}~{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SIMPLE_MIN_BY_MAX.ToString(), "{0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_LEVEL.ToString(), "Lv: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_EXP.ToString(), "Exp: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_STAT_POINTS.ToString(), "Stat Points: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SKILL_POINTS.ToString(), "Skill Points: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_HP.ToString(), "Hp: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_MP.ToString(), "Mp: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_STAMINA.ToString(), "Stamina: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_FOOD.ToString(), "Food: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_WATER.ToString(), "Water: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_WEIGHT.ToString(), "Weight: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_SLOT.ToString(), "Slot: {0}/{1}");
            Texts.Add(UITextKeys.UI_LABEL_UNLIMIT_WEIGHT.ToString(), "Unlimit Weight");
            Texts.Add(UITextKeys.UI_LABEL_UNLIMIT_SLOT.ToString(), "Unlimit Slot");
            Texts.Add(UIFormatKeys.UI_FORMAT_HP.ToString(), "Hp: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_MP.ToString(), "Mp: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_STAMINA.ToString(), "Stamina: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_FOOD.ToString(), "Food: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_WATER.ToString(), "Water: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_ACCURACY.ToString(), "Accuracy: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_EVASION.ToString(), "Evasion: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CRITICAL_RATE.ToString(), "Cri. Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_CRITICAL_DAMAGE_RATE.ToString(), "Cri. Damage: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_BLOCK_RATE.ToString(), "Block Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_BLOCK_DAMAGE_RATE.ToString(), "Block Damage: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_MOVE_SPEED.ToString(), "Move Speed: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_ATTACK_SPEED.ToString(), "Attack Speed: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_WEIGHT.ToString(), "Weight: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SLOT.ToString(), "Slot: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_GOLD.ToString(), "Gold: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CASH.ToString(), "Cash: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SELL_PRICE.ToString(), "Sell Price: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_LEVEL.ToString(), "Require Level: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_LEVEL_NOT_ENOUGH.ToString(), "Require Level: <color=red>{0}/{1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_CLASS.ToString(), "Require Class: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_INVALID_REQUIRE_CLASS.ToString(), "Require Class: <color=red>{0}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_AVAILABLE_WEAPONS.ToString(), "Have to equip: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_AVAILABLE_ARMORS.ToString(), "Have to equip: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_AVAILABLE_VEHICLES.ToString(), "Have to drive: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CONSUME_HP.ToString(), "Consume Hp: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CONSUME_MP.ToString(), "Consume Mp: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CONSUME_STAMINA.ToString(), "Consume Stamina: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_LEVEL_UP.ToString(), "Level is up to {0}!");
            // Format - Skill
            Texts.Add(UIFormatKeys.UI_FORMAT_SKILL_COOLDOWN_DURATION.ToString(), "Cooldown: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SKILL_TYPE.ToString(), "Skill Type: {0}");
            Texts.Add(UISkillTypeKeys.UI_SKILL_TYPE_ACTIVE.ToString(), "Active");
            Texts.Add(UISkillTypeKeys.UI_SKILL_TYPE_PASSIVE.ToString(), "Passive");
            Texts.Add(UISkillTypeKeys.UI_SKILL_TYPE_CRAFT_ITEM.ToString(), "Craft Item");
            // Format - Buff
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_DURATION.ToString(), "Duration: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_HP.ToString(), "Recovery Hp: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_MP.ToString(), "Recovery Mp: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_STAMINA.ToString(), "Recovery Stamina: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_FOOD.ToString(), "Recovery Food: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_RECOVERY_WATER.ToString(), "Recovery Water: {0}");
            // Format - Item
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_REFINE_LEVEL.ToString(), "+{0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_TITLE_WITH_REFINE_LEVEL.ToString(), "{0} +{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_TYPE.ToString(), "Item Type: {0}");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_JUNK.ToString(), "Junk");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_SHIELD.ToString(), "Shield");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_CONSUMABLE.ToString(), "Consumable");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_POTION.ToString(), "Potion");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_AMMO.ToString(), "Ammo");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_BUILDING.ToString(), "Building");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_PET.ToString(), "Pet");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_SOCKET_ENHANCER.ToString(), "Socket Enhancer");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_MOUNT.ToString(), "Mount");
            Texts.Add(UIItemTypeKeys.UI_ITEM_TYPE_SKILL.ToString(), "Skill");
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_RARITY.ToString(), "Rarity: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_STACK.ToString(), "{0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_DURABILITY.ToString(), "Durability: {0}/{1}");
            // Format - Social
            Texts.Add(UIFormatKeys.UI_FORMAT_SOCIAL_LEADER.ToString(), "Leader: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SOCIAL_MEMBER_AMOUNT.ToString(), "Member: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SOCIAL_MEMBER_AMOUNT_NO_LIMIT.ToString(), "Member: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SHARE_EXP_PERCENTAGE.ToString(), "Share Exp: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_REWARD_EXP.ToString(), "Reward Exp: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_REWARD_GOLD.ToString(), "Reward Gold: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_REWARD_CASH.ToString(), "Reward Cash: {0}");
            // Format - Attribute Amount
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_ATTRIBUTE.ToString(), "{0}: {1}/{2}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_ATTRIBUTE_NOT_ENOUGH.ToString(), "{0}: <color=red>{1}/{2}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_ATTRIBUTE_AMOUNT.ToString(), "{0}: {1}");
            // Format - Resistance Amount
            Texts.Add(UIFormatKeys.UI_FORMAT_RESISTANCE_AMOUNT.ToString(), "{0} Resistance: {1}%");
            // Format - Armor Amount
            Texts.Add(UIFormatKeys.UI_FORMAT_ARMOR_AMOUNT.ToString(), "{0} Armor: {1}");
            // Format - Skill Level
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_SKILL.ToString(), "{0}: {1}/{2}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_SKILL_NOT_ENOUGH.ToString(), "{0}: <color=red>{1}/{2}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_SKILL_LEVEL.ToString(), "{0}: {1}");
            // Format - Item Amount
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_ITEM.ToString(), "{0}: {1}/{2}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_ITEM_NOT_ENOUGH.ToString(), "{0}: <color=red>{1}/{2}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_AMOUNT.ToString(), "{0}: {1}");
            // Format - Damage
            Texts.Add(UIFormatKeys.UI_FORMAT_DAMAGE_AMOUNT.ToString(), "{0}~{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_DAMAGE_WITH_ELEMENTAL.ToString(), "{0} Damage: {1}~{2}");
            Texts.Add(UIFormatKeys.UI_FORMAT_DAMAGE_INFLICTION.ToString(), "Inflict {0}% damage");
            Texts.Add(UIFormatKeys.UI_FORMAT_DAMAGE_INFLICTION_AS_ELEMENTAL.ToString(), "Inflict {1}% as {0} damage");
            // Format - Gold Amount
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD.ToString(), "Gold: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_GOLD_NOT_ENOUGH.ToString(), "Gold: <color=red>{0}/{1}</color>");
            // Format - Cash Amount
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_CASH.ToString(), "Cash: {0}/{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_CASH_NOT_ENOUGH.ToString(), "Cash: <color=red>{0}/{1}</color>");
            // Format - UI Equipment Set
            Texts.Add(UIFormatKeys.UI_FORMAT_EQUIPMENT_SET.ToString(), "<color=#ffa500ff>{0}</color>\n{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_EQUIPMENT_SET_APPLIED_EFFECT.ToString(), "<color=#ffa500ff>({0}) {1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_EQUIPMENT_SET_UNAPPLIED_EFFECT.ToString(), "({0}) {1}");
            // Format - UI Equipment Socket
            Texts.Add(UIFormatKeys.UI_FORMAT_EQUIPMENT_SOCKET_FILLED.ToString(), "<color=#800080ff>({0}) - {1}\n{2}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_EQUIPMENT_SOCKET_EMPTY.ToString(), "<color=#800080ff>({0}) - Empty</color>");
            // Format - Refine Item
            Texts.Add(UIFormatKeys.UI_FORMAT_REFINE_SUCCESS_RATE.ToString(), "Success Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_REFINING_LEVEL.ToString(), "Refining Level: +{0}");
            // Format - Guild Bonus
            Texts.Add(UIFormatKeys.UI_FORMAT_INCREASE_MAX_MEMBER.ToString(), "Max Member +{0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_INCREASE_EXP_GAIN_PERCENTAGE.ToString(), "Exp Gain +{0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_INCREASE_GOLD_GAIN_PERCENTAGE.ToString(), "Gold Gain +{0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_INCREASE_SHARE_EXP_GAIN_PERCENTAGE.ToString(), "Party Share Exp +{0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_INCREASE_SHARE_GOLD_GAIN_PERCENTAGE.ToString(), "Party Share Gold +{0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_DECREASE_EXP_PENALTY_PERCENTAGE.ToString(), "Exp Penalty -{0}%");
            // Format - UI Character Quest
            Texts.Add(UIFormatKeys.UI_FORMAT_QUEST_TITLE_ON_GOING.ToString(), "{0} (Ongoing)");
            Texts.Add(UIFormatKeys.UI_FORMAT_QUEST_TITLE_TASKS_COMPLETE.ToString(), "{0} (Task Completed)");
            Texts.Add(UIFormatKeys.UI_FORMAT_QUEST_TITLE_COMPLETE.ToString(), "{0} (Completed)");
            // Format - UI Quest Task
            Texts.Add(UIFormatKeys.UI_FORMAT_QUEST_TASK_KILL_MONSTER.ToString(), "Kills {0}: {1}/{2}");
            Texts.Add(UIFormatKeys.UI_FORMAT_QUEST_TASK_COLLECT_ITEM.ToString(), "Collects {0}: {1}/{2}");
            Texts.Add(UIFormatKeys.UI_FORMAT_QUEST_TASK_KILL_MONSTER_COMPLETE.ToString(), "Kills {0}: Complete");
            Texts.Add(UIFormatKeys.UI_FORMAT_QUEST_TASK_COLLECT_ITEM_COMPLETE.ToString(), "Collects {0}: Complete");
            // Format - UI Chat Message
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_LOCAL.ToString(), "<color=white>(LOCAL) {0}: {1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_GLOBAL.ToString(), "<color=white>(GLOBAL) {0}: {1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_WHISPER.ToString(), "<color=green>(WHISPER) {0}: {1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_PARTY.ToString(), "<color=cyan>(PARTY) {0}: {1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_GUILD.ToString(), "<color=blue>(GUILD) {0}: {1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_SYSTEM.ToString(), "<color=orange>{0}</color>");
            // Format - UI Mail
            Texts.Add(UIFormatKeys.UI_FORMAT_MAIL_SENDER_NAME.ToString(), "From: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_MAIL_TITLE.ToString(), "Title: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_MAIL_CONTENT.ToString(), "{0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_MAIL_SENT_DATE.ToString(), "Date: {0}");
            // Format - UI Crafting
            Texts.Add(UIFormatKeys.UI_FORMAT_CRAFT_DURATION.ToString(), "Duration: {0}");
            // Error - Generic Error
            Texts.Add(UITextKeys.UI_ERROR_BAD_REQUEST.ToString(), "Bad request");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ALLOWED.ToString(), "You're not allowed to do that");
            Texts.Add(UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE.ToString(), "Service is not available");
            Texts.Add(UITextKeys.UI_ERROR_CONTENT_NOT_AVAILABLE.ToString(), "Content is not available");
            Texts.Add(UITextKeys.UI_ERROR_FEATURE_IS_DISABLED.ToString(), "Feature is disabled");
            Texts.Add(UITextKeys.UI_ERROR_REQUEST_TIMEOUT.ToString(), "Request timeout");
            Texts.Add(UITextKeys.UI_ERROR_KICKED_FROM_SERVER.ToString(), "You have been kicked from server");
            Texts.Add(UITextKeys.UI_ERROR_CONNECTION_FAILED.ToString(), "Cannot connect to the server");
            Texts.Add(UITextKeys.UI_ERROR_CONNECTION_REJECTED.ToString(), "Connection rejected by server");
            Texts.Add(UITextKeys.UI_ERROR_REMOTE_CONNECTION_CLOSE.ToString(), "Server has been closed");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_PROTOCOL.ToString(), "Invalid protocol");
            Texts.Add(UITextKeys.UI_ERROR_HOST_UNREACHABLE.ToString(), "Host unreachable");
            Texts.Add(UITextKeys.UI_ERROR_CONNECTION_TIMEOUT.ToString(), "Connection timeout");
            Texts.Add(UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR.ToString(), "Internal server error");
            Texts.Add(UITextKeys.UI_ERROR_SERVER_NOT_FOUND.ToString(), "Server not found");
            Texts.Add(UITextKeys.UI_ERROR_USER_NOT_FOUND.ToString(), "User not found");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_NOT_FOUND.ToString(), "Character not found");
            Texts.Add(UITextKeys.UI_ERROR_ITEM_NOT_FOUND.ToString(), "Item not found");
            Texts.Add(UITextKeys.UI_ERROR_CASH_PACKAGE_NOT_FOUND.ToString(), "Cash package not found");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD.ToString(), "Not enough gold");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_CASH.ToString(), "Not enough cash");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS.ToString(), "Not enough items");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_STAT_POINT.ToString(), "Not enough stat points");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_SKILL_POINT.ToString(), "Not enough skill points");
            Texts.Add(UITextKeys.UI_ERROR_DISALLOW_SKILL_LEVEL_UP.ToString(), "Disallow to level up");
            Texts.Add(UITextKeys.UI_ERROR_NOT_LOGGED_IN.ToString(), "Not logged in");
            Texts.Add(UITextKeys.UI_ERROR_USERNAME_IS_EMPTY.ToString(), "Username is empty");
            Texts.Add(UITextKeys.UI_ERROR_PASSWORD_IS_EMPTY.ToString(), "Password is empty");
            Texts.Add(UITextKeys.UI_ERROR_WILL_OVERWHELMING.ToString(), "Cannot carry all items");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ABLE_TO_LOOT.ToString(), "Not allowed to loot");
            Texts.Add(UITextKeys.UI_ERROR_TOO_FAST_ACTION.ToString(), "Please wait a bit before trying that again");
            Texts.Add(UITextKeys.UI_ERROR_SERVER_CLOSE.ToString(), "Server is temporarily closed for maintenance");
            // Error - Game Data
            Texts.Add(UITextKeys.UI_ERROR_INVALID_DATA.ToString(), "Invalid data");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_CHARACTER_DATA.ToString(), "Invalid character data");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_CHARACTER_ENTITY.ToString(), "Invalid character entity");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_ITEM_DATA.ToString(), "Invalid item data");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_ITEM_INDEX.ToString(), "Invalid item index");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_ENHANCER_ITEM_INDEX.ToString(), "Invalid enhancer item index");
            Texts.Add(UITextKeys.UI_ERROR_ITEM_NOT_EQUIPMENT.ToString(), "Item is not a equipment item");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_ATTRIBUTE_DATA.ToString(), "Invalid attribute data");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_CURRENCY_DATA.ToString(), "Invalid currency data");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_SKILL_DATA.ToString(), "Invalid skill data");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_GUILD_SKILL_DATA.ToString(), "Invalid guild skill data");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_FACTION_DATA.ToString(), "Invalid faction data");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_BUILDING_DATA.ToString(), "Invalid building data");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_BUILDING_ENTITY.ToString(), "Invalid building entity");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_VEHICLE_ENTITY.ToString(), "Invalid vehicle entity");
            // Error - Vehicle
            Texts.Add(UITextKeys.UI_ERROR_SEAT_NOT_AVAILABLE.ToString(), "Seat is not available");
            // Error - Enhance
            // Error - UI Login
            Texts.Add(UITextKeys.UI_ERROR_INVALID_USERNAME_OR_PASSWORD.ToString(), "Invalid username or password");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_USER_TOKEN.ToString(), "Invalid user token");
            Texts.Add(UITextKeys.UI_ERROR_ALREADY_LOGGED_IN.ToString(), "User already logged in");
            Texts.Add(UITextKeys.UI_ERROR_ACCOUNT_LOGGED_IN_BY_OTHER.ToString(), "Your account was logged in by other");
            Texts.Add(UITextKeys.UI_ERROR_USER_BANNED.ToString(), "Your account was banned");
            Texts.Add(UITextKeys.UI_ERROR_EMAIL_NOT_VERIFIED.ToString(), "Email is not verified");
            // Error - UI Register
            Texts.Add(UITextKeys.UI_ERROR_INVALID_CONFIRM_PASSWORD.ToString(), "Invalid confirm password");
            Texts.Add(UITextKeys.UI_ERROR_USERNAME_TOO_SHORT.ToString(), "Username is too short");
            Texts.Add(UITextKeys.UI_ERROR_USERNAME_TOO_LONG.ToString(), "Username is too long");
            Texts.Add(UITextKeys.UI_ERROR_PASSWORD_TOO_SHORT.ToString(), "Password is too short");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_EMAIL.ToString(), "Invalid email format");
            Texts.Add(UITextKeys.UI_ERROR_EMAIL_ALREADY_IN_USE.ToString(), "Email is already in use");
            Texts.Add(UITextKeys.UI_ERROR_USERNAME_EXISTED.ToString(), "Username is already existed");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_USERNAME.ToString(), "Username is invalid, allow only a-z, A-Z, 0-9 and _ for username.");
            // Error - UI Lobby
            Texts.Add(UITextKeys.UI_ERROR_ALREADY_CONNECTED_TO_LOBBY.ToString(), "Already connected to lobby server");
            Texts.Add(UITextKeys.UI_ERROR_ALREADY_CONNECTED_TO_GAME.ToString(), "Already connected to game server");
            Texts.Add(UITextKeys.UI_ERROR_NO_SELECTED_REALM.ToString(), "Please select realm");
            Texts.Add(UITextKeys.UI_ERROR_NO_AVAILABLE_REALM.ToString(), "No available realm");
            Texts.Add(UITextKeys.UI_ERROR_NO_AVAILABLE_LOBBY.ToString(), "No available lobby");
            // Error - UI Character List
            Texts.Add(UITextKeys.UI_ERROR_NO_CHOSEN_CHARACTER_TO_START.ToString(), "Please choose character to start game");
            Texts.Add(UITextKeys.UI_ERROR_NO_CHOSEN_CHARACTER_TO_DELETE.ToString(), "Please choose character to delete");
            Texts.Add(UITextKeys.UI_ERROR_ALREADY_SELECT_CHARACTER.ToString(), "Already select character");
            Texts.Add(UITextKeys.UI_ERROR_MAP_SERVER_NOT_READY.ToString(), "Map server is not ready");
            // Error - UI Character Create
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_NAME_TOO_SHORT.ToString(), "Character name is too short");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_NAME_TOO_LONG.ToString(), "Character name is too long");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_NAME_EXISTED.ToString(), "Character name is already existed");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_CHARACTER_NAME.ToString(), "Character name is invalid, allow only a-z, A-Z, 0-9 and _ for character name.");
            // Error - UI Cash Packages
            Texts.Add(UITextKeys.UI_ERROR_INVALID_IAP_RECEIPT.ToString(), "Invalid IAP receipt");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_GET_CASH_PACKAGE_INFO.ToString(), "Cannot retrieve cash package info");
            // Error - UI Cash Shop
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_GET_CASH_SHOP_INFO.ToString(), "Cannot retrieve cash shop info");
            // Error - UI Guild Name
            Texts.Add(UITextKeys.UI_ERROR_GUILD_NAME_TOO_SHORT.ToString(), "Guild name is too short");
            Texts.Add(UITextKeys.UI_ERROR_GUILD_NAME_TOO_LONG.ToString(), "Guild name is too long");
            Texts.Add(UITextKeys.UI_ERROR_GUILD_NAME_EXISTED.ToString(), "Guild name is already existed");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_GUILD_NAME.ToString(), "Guild name is invalid, allow only a-z, A-Z, 0-9 and _ for guild name.");
            // Error - UI Guild Role Setting
            Texts.Add(UITextKeys.UI_ERROR_GUILD_ROLE_NAME_TOO_SHORT.ToString(), "Guild role name is too short");
            Texts.Add(UITextKeys.UI_ERROR_GUILD_ROLE_NAME_TOO_LONG.ToString(), "Guild role name is too long");
            Texts.Add(UITextKeys.UI_ERROR_GUILD_ROLE_SHARE_EXP_NOT_NUMBER.ToString(), "Share exp percentage must be number");
            // Error - UI Guild Member Role Setting
            Texts.Add(UITextKeys.UI_ERROR_INVALID_GUILD_ROLE.ToString(), "Invalid role");
            // Error - UI Guild Message Setting
            Texts.Add(UITextKeys.UI_ERROR_GUILD_MESSAGE_TOO_LONG.ToString(), "Guild message is too long");
            // Error - Equip
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_EQUIP.ToString(), "Cannot equip the item");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_RIGHT_HAND.ToString(), "Invalid equip position for right hand equipment");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_LEFT_HAND.ToString(), "Invalid equip position for left hand equipment");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_RIGHT_HAND_OR_LEFT_HAND.ToString(), "Invalid equip position for right hand or left hand equipment");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_EQUIP_POSITION_ARMOR.ToString(), "Invalid equip position for armor equipment");
            // Error - Refine
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_REFINE.ToString(), "Cannot refine the item");
            Texts.Add(UITextKeys.UI_ERROR_REFINE_ITEM_REACHED_MAX_LEVEL.ToString(), "Item reached max level");
            Texts.Add(UITextKeys.UI_REFINE_SUCCESS.ToString(), "Refine success");
            Texts.Add(UITextKeys.UI_REFINE_FAIL.ToString(), "Refine fail");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_ENHANCE_SOCKET.ToString(), "Cannot enhance the item");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_SOCKET_ENCHANER.ToString(), "Have not enough items");
            Texts.Add(UITextKeys.UI_ERROR_NO_EMPTY_SOCKET.ToString(), "No empty socket");
            Texts.Add(UITextKeys.UI_ERROR_SOCKET_NOT_EMPTY.ToString(), "Socket is not empty");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_REMOVE_ENHANCER.ToString(), "Cannot remove enhancer item from socket");
            Texts.Add(UITextKeys.UI_ERROR_NO_ENHANCER.ToString(), "No enhancer item");
            // Error - Repair
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_REPAIR.ToString(), "Cannot repair the item");
            Texts.Add(UITextKeys.UI_REPAIR_SUCCESS.ToString(), "Repair success");
            // Error - Dealing
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_IS_DEALING.ToString(), "Character is in another deal");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR.ToString(), "Character is too far");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_ACCEPT_DEALING_REQUEST.ToString(), "Cannot accept dealing request");
            Texts.Add(UITextKeys.UI_ERROR_DEALING_REQUEST_DECLINED.ToString(), "Dealing request declined");
            Texts.Add(UITextKeys.UI_ERROR_INVALID_DEALING_STATE.ToString(), "Invalid dealing state");
            Texts.Add(UITextKeys.UI_ERROR_DEALING_CANCELED.ToString(), "Dealing canceled");
            Texts.Add(UITextKeys.UI_ERROR_ANOTHER_CHARACTER_WILL_OVERWHELMING.ToString(), "Another character cannot carry all items");
            // Error - Restriction
            Texts.Add(UITextKeys.UI_ERROR_ITEM_DEALING_RESTRICTED.ToString(), "Cannot deal this item, item dealing restricted");
            Texts.Add(UITextKeys.UI_ERROR_ITEM_DROPPING_RESTRICTED.ToString(), "Cannot drop this item, item dropping restricted");
            Texts.Add(UITextKeys.UI_ERROR_ITEM_SELLING_RESTRICTED.ToString(), "Cannot sell this item, item selling restricted");
            // Error - Party
            Texts.Add(UITextKeys.UI_ERROR_PARTY_NOT_FOUND.ToString(), "Party not found");
            Texts.Add(UITextKeys.UI_ERROR_PARTY_INVITATION_NOT_FOUND.ToString(), "Party invitation not found");
            Texts.Add(UITextKeys.UI_PARTY_INVITATION_ACCEPTED.ToString(), "Party invitation accepted");
            Texts.Add(UITextKeys.UI_PARTY_INVITATION_DECLINED.ToString(), "Party invitation declined");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_SEND_PARTY_INVITATION.ToString(), "Cannot send party invitation");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_KICK_PARTY_MEMBER.ToString(), "Cannot kick party member");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_KICK_YOURSELF_FROM_PARTY.ToString(), "Cannot kick yourself from party");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_KICK_PARTY_LEADER.ToString(), "Cannot kick party leader");
            Texts.Add(UITextKeys.UI_ERROR_JOINED_ANOTHER_PARTY.ToString(), "Already joined another party");
            Texts.Add(UITextKeys.UI_ERROR_NOT_JOINED_PARTY.ToString(), "Not joined the party");
            Texts.Add(UITextKeys.UI_ERROR_NOT_PARTY_LEADER.ToString(), "Not a party leader");
            Texts.Add(UITextKeys.UI_ERROR_ALREADY_IS_PARTY_LEADER.ToString(), "You are already a leader");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_JOINED_ANOTHER_PARTY.ToString(), "Character already joined another party");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_NOT_JOINED_PARTY.ToString(), "Character not joined the party");
            Texts.Add(UITextKeys.UI_ERROR_PARTY_MEMBER_REACHED_LIMIT.ToString(), "Party member reached limit");
            Texts.Add(UITextKeys.UI_ERROR_PARTY_MEMBER_CANNOT_ENTER_INSTANCE.ToString(), "Only party leader can enter instance");
            // Error - Guild
            Texts.Add(UITextKeys.UI_ERROR_GUILD_NOT_FOUND.ToString(), "Guild not found");
            Texts.Add(UITextKeys.UI_ERROR_GUILD_INVITATION_NOT_FOUND.ToString(), "Guild invitation not found");
            Texts.Add(UITextKeys.UI_GUILD_INVITATION_ACCEPTED.ToString(), "Guild invitation accepted");
            Texts.Add(UITextKeys.UI_GUILD_INVITATION_DECLINED.ToString(), "Guild invitation declined");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_SEND_GUILD_INVITATION.ToString(), "Cannot send guild invitation");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_KICK_GUILD_MEMBER.ToString(), "Cannot kick guild member");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_KICK_YOURSELF_FROM_GUILD.ToString(), "Cannot kick yourself from guild");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_KICK_GUILD_LEADER.ToString(), "Cannot kick guild leader");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_KICK_HIGHER_GUILD_MEMBER.ToString(), "Cannot kick higher guild member");
            Texts.Add(UITextKeys.UI_ERROR_JOINED_ANOTHER_GUILD.ToString(), "Already joined another guild");
            Texts.Add(UITextKeys.UI_ERROR_NOT_JOINED_GUILD.ToString(), "Not joined the guild");
            Texts.Add(UITextKeys.UI_ERROR_NOT_GUILD_LEADER.ToString(), "Not a guild leader");
            Texts.Add(UITextKeys.UI_ERROR_ALREADY_IS_GUILD_LEADER.ToString(), "You are already a leader");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_CHANGE_GUILD_LEADER_ROLE.ToString(), "Cannot change guild leader's role");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_JOINED_ANOTHER_GUILD.ToString(), "Character already joined another guild");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_NOT_JOINED_GUILD.ToString(), "Character not joined the guild");
            Texts.Add(UITextKeys.UI_ERROR_GUILD_MEMBER_REACHED_LIMIT.ToString(), "Guild member reached limit");
            Texts.Add(UITextKeys.UI_ERROR_GUILD_ROLE_NOT_AVAILABLE.ToString(), "Guild role is not available");
            Texts.Add(UITextKeys.UI_ERROR_GUILD_SKILL_REACHED_MAX_LEVEL.ToString(), "Guild skill is reached max level");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_GUILD_SKILL_POINT.ToString(), "Not enough guild skill point");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_ACCEPT_GUILD_REQUEST.ToString(), "You're not allowed to accept guild request");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_DECLINE_GUILD_REQUEST.ToString(), "You're not allowed to decline guild request");
            // Error - Game Data
            Texts.Add(UITextKeys.UI_UNKNOW_GAME_DATA_TITLE.ToString(), "Unknow");
            Texts.Add(UITextKeys.UI_UNKNOW_GAME_DATA_DESCRIPTION.ToString(), "N/A");
            // Error - Bank
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD_TO_DEPOSIT.ToString(), "Not enough gold to deposit");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD_TO_WITHDRAW.ToString(), "Not enough gold to withdraw");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE.ToString(), "Cannot access storage");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_GET_STORAGE_ITEMS.ToString(), "Cannot get storage items");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_UPDATE_STORAGE_ITEMS.ToString(), "Cannot update storage items");
            Texts.Add(UITextKeys.UI_ERROR_OTHER_GUILD_MEMBER_ACCESSING_STORAGE.ToString(), "Your guild storage has already been opened by another member, try again later.");
            Texts.Add(UITextKeys.UI_ERROR_STORAGE_NOT_FOUND.ToString(), "Storage not found");
            // Error - Combatant
            Texts.Add(UITextKeys.UI_ERROR_NO_AMMO.ToString(), "No Ammo");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_HP.ToString(), "Not enough Hp");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_MP.ToString(), "Not enough Mp");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_STAMINA.ToString(), "Not enough Stamina");
            Texts.Add(UITextKeys.UI_ERROR_NOT_DEAD.ToString(), "Cannot respawn");
            // Error - Skill
            Texts.Add(UITextKeys.UI_ERROR_SKILL_LEVEL_IS_ZERO.ToString(), "Skill not trained yet");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_USE_SKILL_WITHOUT_SHIELD.ToString(), "Cannot use skill without shield");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_USE_SKILL_BY_CURRENT_WEAPON.ToString(), "Cannot use skill by current weapons");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_USE_SKILL_BY_CURRENT_ARMOR.ToString(), "Cannot use skill by current armors");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_USE_SKILL_BY_CURRENT_VEHICLE.ToString(), "Cannot use skill by current vehicle");
            Texts.Add(UITextKeys.UI_ERROR_SKILL_IS_COOLING_DOWN.ToString(), "Skill is cooling down");
            Texts.Add(UITextKeys.UI_ERROR_SKILL_IS_NOT_LEARNED.ToString(), "Skill is not learned");
            Texts.Add(UITextKeys.UI_ERROR_NO_SKILL_TARGET.ToString(), "No target");
            // Error - Item
            Texts.Add(UITextKeys.UI_ERROR_ITEM_IS_COOLING_DOWN.ToString(), "Item is cooling down");
            Texts.Add(UITextKeys.UI_ERROR_ITEM_IS_LOCKED.ToString(), "Item is locked");
            // Error - Requirement
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_LEVEL.ToString(), "Not enough level");
            Texts.Add(UITextKeys.UI_ERROR_NOT_MATCH_CHARACTER_CLASS.ToString(), "Not match character class");
            Texts.Add(UITextKeys.UI_ERROR_NOT_MATCH_CHARACTER_FACTION.ToString(), "Not match character faction");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_ATTRIBUTE_AMOUNTS.ToString(), "Not enough attribute amounts");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_SKILL_LEVELS.ToString(), "Not enough skill levels");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS.ToString(), "Not enough currency amounts");
            Texts.Add(UITextKeys.UI_ERROR_ATTRIBUTE_REACHED_MAX_AMOUNT.ToString(), "Attribute reached max amount");
            Texts.Add(UITextKeys.UI_ERROR_SKILL_REACHED_MAX_LEVEL.ToString(), "Skill reached max level");
            // Success - UI Cash Shop
            Texts.Add(UITextKeys.UI_CASH_SHOP_ITEM_BOUGHT.ToString(), "Cash shop item purchased");
            // Success - UI Gacha
            Texts.Add(UITextKeys.UI_GACHA_OPENED.ToString(), "Gacha opened");
            // UI Character Item
            Texts.Add(UITextKeys.UI_DROP_ITEM.ToString(), "Drop Item");
            Texts.Add(UITextKeys.UI_DROP_ITEM_DESCRIPTION.ToString(), "Enter amount of item");
            Texts.Add(UITextKeys.UI_DESTROY_ITEM.ToString(), "Destroy Item");
            Texts.Add(UITextKeys.UI_DESTROY_ITEM_DESCRIPTION.ToString(), "Do you want to destroy an items?");
            Texts.Add(UITextKeys.UI_SELL_ITEM.ToString(), "Sell Item");
            Texts.Add(UITextKeys.UI_SELL_ITEM_DESCRIPTION.ToString(), "Enter amount of item");
            Texts.Add(UITextKeys.UI_DISMANTLE_ITEM.ToString(), "Dismantle Item");
            Texts.Add(UITextKeys.UI_DISMANTLE_ITEM_DESCRIPTION.ToString(), "Enter amount of item");
            Texts.Add(UITextKeys.UI_OFFER_ITEM.ToString(), "Offer Item");
            Texts.Add(UITextKeys.UI_OFFER_ITEM_DESCRIPTION.ToString(), "Enter amount of item");
            Texts.Add(UITextKeys.UI_PUT_VENDING_ITEM_AMOUNT.ToString(), "Item Amount");
            Texts.Add(UITextKeys.UI_PUT_VENDING_ITEM_AMOUNT_DESCRIPTION.ToString(), "Enter amount of item");
            Texts.Add(UITextKeys.UI_PUT_VENDING_ITEM_PRICE.ToString(), "Item Price");
            Texts.Add(UITextKeys.UI_PUT_VENDING_ITEM_PRICE_DESCRIPTION.ToString(), "Enter selling price");
            Texts.Add(UITextKeys.UI_MOVE_ITEM_TO_STORAGE.ToString(), "Move To Storage");
            Texts.Add(UITextKeys.UI_MOVE_ITEM_TO_STORAGE_DESCRIPTION.ToString(), "Enter amount of item");
            Texts.Add(UITextKeys.UI_MOVE_ITEM_FROM_STORAGE.ToString(), "Move From Storage");
            Texts.Add(UITextKeys.UI_MOVE_ITEM_FROM_STORAGE_DESCRIPTION.ToString(), "Enter amount of item");
            Texts.Add(UITextKeys.UI_MOVE_ITEM_FROM_ITEMS_CONTAINER.ToString(), "Move From Container");
            Texts.Add(UITextKeys.UI_MOVE_ITEM_FROM_ITEMS_CONTAINER_DESCRIPTION.ToString(), "Enter amount of item");
            Texts.Add(UITextKeys.UI_ERROR_STORAGE_WILL_OVERWHELMING.ToString(), "Storage will overwhelming");
            // UI Bank
            Texts.Add(UITextKeys.UI_BANK_DEPOSIT.ToString(), "Deposit");
            Texts.Add(UITextKeys.UI_BANK_DEPOSIT_DESCRIPTION.ToString(), "Enter amount of gold");
            Texts.Add(UITextKeys.UI_BANK_WITHDRAW.ToString(), "Withdraw");
            Texts.Add(UITextKeys.UI_BANK_WITHDRAW_DESCRIPTION.ToString(), "Enter amount of gold");
            // UI Dealing
            Texts.Add(UITextKeys.UI_OFFER_GOLD.ToString(), "Offer Gold");
            Texts.Add(UITextKeys.UI_OFFER_GOLD_DESCRIPTION.ToString(), "Enter amount of gold");
            // UI Npc Sell Item
            Texts.Add(UITextKeys.UI_BUY_ITEM.ToString(), "Buy Item");
            Texts.Add(UITextKeys.UI_BUY_ITEM_DESCRIPTION.ToString(), "Enter amount of item");
            // UI Party
            Texts.Add(UITextKeys.UI_PARTY_CHANGE_LEADER.ToString(), "Change Leader");
            Texts.Add(UITextKeys.UI_PARTY_CHANGE_LEADER_DESCRIPTION.ToString(), "You sure you want to promote {0} to party leader?");
            Texts.Add(UITextKeys.UI_PARTY_KICK_MEMBER.ToString(), "Kick Member");
            Texts.Add(UITextKeys.UI_PARTY_KICK_MEMBER_DESCRIPTION.ToString(), "You sure you want to kick {0} from party?");
            Texts.Add(UITextKeys.UI_PARTY_LEAVE.ToString(), "Leave Party");
            Texts.Add(UITextKeys.UI_PARTY_LEAVE_DESCRIPTION.ToString(), "You sure you want to leave party?");
            // UI Guild
            Texts.Add(UITextKeys.UI_GUILD_CHANGE_LEADER.ToString(), "Change Leader");
            Texts.Add(UITextKeys.UI_GUILD_CHANGE_LEADER_DESCRIPTION.ToString(), "You sure you want to promote {0} to guild leader?");
            Texts.Add(UITextKeys.UI_GUILD_KICK_MEMBER.ToString(), "Kick Member");
            Texts.Add(UITextKeys.UI_GUILD_KICK_MEMBER_DESCRIPTION.ToString(), "You sure you want to kick {0} from guild?");
            Texts.Add(UITextKeys.UI_GUILD_LEAVE.ToString(), "Leave Guild");
            Texts.Add(UITextKeys.UI_GUILD_LEAVE_DESCRIPTION.ToString(), "You sure you want to leave guild?");
            Texts.Add(UITextKeys.UI_GUILD_REQUEST.ToString(), "Guild Application");
            Texts.Add(UITextKeys.UI_GUILD_REQUEST_DESCRIPTION.ToString(), "You want to request to join guild {0}?");
            Texts.Add(UITextKeys.UI_GUILD_REQUESTED.ToString(), "Guild request was sent to the guild");
            Texts.Add(UITextKeys.UI_GUILD_REQUEST_ACCEPTED.ToString(), "Guild request accepted");
            Texts.Add(UITextKeys.UI_GUILD_REQUEST_DECLINED.ToString(), "Guild request declined");
            // UI Guild Role
            Texts.Add(UITextKeys.UI_GUILD_ROLE_CAN_INVITE.ToString(), "Can invite");
            Texts.Add(UITextKeys.UI_GUILD_ROLE_CANNOT_INVITE.ToString(), "Cannot invite");
            Texts.Add(UITextKeys.UI_GUILD_ROLE_CAN_KICK.ToString(), "Can kick");
            Texts.Add(UITextKeys.UI_GUILD_ROLE_CANNOT_KICK.ToString(), "Cannot kick");
            Texts.Add(UITextKeys.UI_GUILD_ROLE_CAN_USE_STORAGE.ToString(), "Can use storage");
            Texts.Add(UITextKeys.UI_GUILD_ROLE_CANNOT_USE_STORAGE.ToString(), "Cannot use storage");
            // UI Friend
            Texts.Add(UITextKeys.UI_FRIEND_ADD.ToString(), "Add Friend");
            Texts.Add(UITextKeys.UI_FRIEND_ADD_DESCRIPTION.ToString(), "You want to add {0} to friend list?");
            Texts.Add(UITextKeys.UI_FRIEND_REMOVE.ToString(), "Remove Friend");
            Texts.Add(UITextKeys.UI_FRIEND_REMOVE_DESCRIPTION.ToString(), "You want to remove {0} from friend list?");
            Texts.Add(UITextKeys.UI_FRIEND_REQUEST.ToString(), "Friend Request");
            Texts.Add(UITextKeys.UI_FRIEND_REQUEST_DESCRIPTION.ToString(), "You want to request {0} to be friend?");
            Texts.Add(UITextKeys.UI_FRIEND_ADDED.ToString(), "The character was added to the friend list");
            Texts.Add(UITextKeys.UI_FRIEND_REMOVED.ToString(), "The character was removed from the friend list");
            Texts.Add(UITextKeys.UI_FRIEND_REQUESTED.ToString(), "Friend request was sent to the character");
            Texts.Add(UITextKeys.UI_FRIEND_REQUEST_ACCEPTED.ToString(), "Friend request accepted");
            Texts.Add(UITextKeys.UI_FRIEND_REQUEST_DECLINED.ToString(), "Friend request declined");
            // UI Password Dialogs
            Texts.Add(UITextKeys.UI_ENTER_BUILDING_PASSWORD.ToString(), "Enter password");
            Texts.Add(UITextKeys.UI_ENTER_BUILDING_PASSWORD_DESCRIPTION.ToString(), "Enter 6 digits number");
            Texts.Add(UITextKeys.UI_SET_BUILDING_PASSWORD.ToString(), "Set password");
            Texts.Add(UITextKeys.UI_SET_BUILDING_PASSWORD_DESCRIPTION.ToString(), "Enter 6 digits number");
            Texts.Add(UITextKeys.UI_ERROR_WRONG_BUILDING_PASSWORD.ToString(), "Wrong password");
            Texts.Add(UITextKeys.UI_ERROR_REACHED_BUILD_LIMIT.ToString(), "Cannot build this kind of building anymore");
            // UI Mail
            Texts.Add(UITextKeys.UI_ERROR_MAIL_SEND_NOT_ALLOWED.ToString(), "You're not allowed to send mail");
            Texts.Add(UITextKeys.UI_ERROR_MAIL_SEND_NO_RECEIVER.ToString(), "No receiver, you may entered wrong name");
            Texts.Add(UITextKeys.UI_MAIL_SENT.ToString(), "Mail sent");
            Texts.Add(UITextKeys.UI_ERROR_MAIL_READ_NOT_ALLOWED.ToString(), "You're not allowed to read the mail");
            Texts.Add(UITextKeys.UI_ERROR_MAIL_CLAIM_NOT_ALLOWED.ToString(), "You're not allowed to claim attached items");
            Texts.Add(UITextKeys.UI_ERROR_MAIL_CLAIM_ALREADY_CLAIMED.ToString(), "Cannot claim items, it was already claimed");
            Texts.Add(UITextKeys.UI_ERROR_MAIL_CLAIM_WILL_OVERWHELMING.ToString(), "Cannot carry all items");
            Texts.Add(UITextKeys.UI_MAIL_CLAIMED.ToString(), "Claimed an items");
            Texts.Add(UITextKeys.UI_ERROR_MAIL_DELETE_NOT_ALLOWED.ToString(), "You're not allowed to delete the mail");
            Texts.Add(UITextKeys.UI_MAIL_DELETED.ToString(), "Mail deleted");
            // Vending
            Texts.Add(UITextKeys.UI_START_VENDING.ToString(), "Confirmation");
            Texts.Add(UITextKeys.UI_START_VENDING_DESCRIPTION.ToString(), "Do you want to open your shop? please double check the prices before doing it.");
            Texts.Add(UITextKeys.UI_STOP_VENDING.ToString(), "Confirmation");
            Texts.Add(UITextKeys.UI_STOP_VENDING_DESCRIPTION.ToString(), "Do you want to close your shop?");
            Texts.Add(UITextKeys.UI_ERROR_NO_START_VENDING_ITEMS.ToString(), "No selling items");
            Texts.Add(UITextKeys.UI_ERROR_REACHED_VENDING_ITEMS_LIMIT.ToString(), "Cannot add more items to sell");
            // Enter Amount
            Texts.Add(UITextKeys.UI_ENTER_ITEM_AMOUNT.ToString(), "Enter Amount");
            Texts.Add(UITextKeys.UI_ENTER_ITEM_AMOUNT_DESCRIPTION.ToString(), "Enter amount of item");
            // Error - IAP
            Texts.Add(UITextKeys.UI_ERROR_IAP_NOT_INITIALIZED.ToString(), "In-App Purchasing system not initialized yet");
            Texts.Add(UITextKeys.UI_ERROR_IAP_PURCHASING_UNAVAILABLE.ToString(), "Purchasing is unavailable");
            Texts.Add(UITextKeys.UI_ERROR_IAP_EXISTING_PURCHASE_PENDING.ToString(), "Existing purchase pending");
            Texts.Add(UITextKeys.UI_ERROR_IAP_PRODUCT_UNAVAILABLE.ToString(), "Product is unavailable");
            Texts.Add(UITextKeys.UI_ERROR_IAP_SIGNATURE_INVALID.ToString(), "Invalid signature");
            Texts.Add(UITextKeys.UI_ERROR_IAP_USER_CANCELLED.ToString(), "Purchase was cancelled");
            Texts.Add(UITextKeys.UI_ERROR_IAP_PAYMENT_DECLINED.ToString(), "Payment was declined");
            Texts.Add(UITextKeys.UI_ERROR_IAP_DUPLICATE_TRANSACTION.ToString(), "Duplicate transaction");
            Texts.Add(UITextKeys.UI_ERROR_IAP_UNKNOW.ToString(), "Unknow");
            // Format - Character Stats Rate
            Texts.Add(UIFormatKeys.UI_FORMAT_HP_RATE.ToString(), "Hp: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_MP_RATE.ToString(), "Mp: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_STAMINA_RATE.ToString(), "Stamina: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_FOOD_RATE.ToString(), "Food: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_WATER_RATE.ToString(), "Water: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_ACCURACY_RATE.ToString(), "Accuracy: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_EVASION_RATE.ToString(), "Evasion: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_CRITICAL_RATE_RATE.ToString(), "% of Cri. Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_CRITICAL_DAMAGE_RATE_RATE.ToString(), "% of Cri. Damage: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_BLOCK_RATE_RATE.ToString(), "% of Block Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_BLOCK_DAMAGE_RATE_RATE.ToString(), "% of Block Damage: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_MOVE_SPEED_RATE.ToString(), "Move Speed: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_ATTACK_SPEED_RATE.ToString(), "Attack Speed: {0}%");
            // Format - Attribute Amount Rate
            Texts.Add(UIFormatKeys.UI_FORMAT_ATTRIBUTE_RATE.ToString(), "{0}: {1}%");
            // Format - Item Building
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_BUILDING.ToString(), "Build {0}");
            // Format - Item Pet
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_PET.ToString(), "Summon {0}");
            // Format - Item Mount
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_MOUNT.ToString(), "Mount {0}");
            // Format - Item Skill
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_SKILL.ToString(), "Skill {0}, Level: {1}");
            // Format - Item Attribute
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_ATTRIBUTE.ToString(), "Attribute {0}, Amount: {1}");
            // Format - Skill Summon
            Texts.Add(UIFormatKeys.UI_FORMAT_SKILL_SUMMON.ToString(), "Summon {0} Lv. {1} x {2} (Max: {3}), {4} Secs.");
            // Format - Skill Mount
            Texts.Add(UIFormatKeys.UI_FORMAT_SKILL_MOUNT.ToString(), "Mount {0}");
            // Format - Skip Title
            Texts.Add(UIFormatKeys.UI_FORMAT_SKIP_TITLE.ToString(), "{1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SKIP_TITLE_PERCENTAGE.ToString(), "{1}%");
            // Format - Notify Rewards
            Texts.Add(UIFormatKeys.UI_FORMAT_NOTIFY_REWARD_EXP.ToString(), "Obtain {0} Exp");
            Texts.Add(UIFormatKeys.UI_FORMAT_NOTIFY_REWARD_GOLD.ToString(), "Obtain {0} Gold");
            Texts.Add(UIFormatKeys.UI_FORMAT_NOTIFY_REWARD_ITEM.ToString(), "Obtain {0} x {1} ea");
            Texts.Add(UIFormatKeys.UI_FORMAT_NOTIFY_REWARD_CURRENCY.ToString(), "Obtain {1} {0}");
            // Format - 1.61 - Talk to NPC quest task
            Texts.Add(UIFormatKeys.UI_FORMAT_QUEST_TASK_TALK_TO_NPC.ToString(), "Talk to {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_QUEST_TASK_TALK_TO_NPC_COMPLETE.ToString(), "Talk to {0}: Complete");
            // Format - Currency Amount
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_CURRENCY.ToString(), "{0}: {1}/{2}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENT_CURRENCY_NOT_ENOUGH.ToString(), "{0}: <color=red>{1}/{2}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CURRENCY_AMOUNT.ToString(), "{0}: {1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_REWARD_CURRENCY.ToString(), "Reward {0}: {1}");
            // Format - 1.65f - New stats
            Texts.Add(UIFormatKeys.UI_FORMAT_HP_RECOVERY.ToString(), "Hp Recovery: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_HP_LEECH_RATE.ToString(), "Hp Leech Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_MP_RECOVERY.ToString(), "Mp Recovery: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_MP_LEECH_RATE.ToString(), "Mp Leech Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_STAMINA_RECOVERY.ToString(), "Stamina Recovery: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_STAMINA_LEECH_RATE.ToString(), "Stamina Leech Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_HP_RECOVERY_RATE.ToString(), "Hp Recovery Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_HP_LEECH_RATE_RATE.ToString(), "% of Hp Leech Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_MP_RECOVERY_RATE.ToString(), "Mp Recovery Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_MP_LEECH_RATE_RATE.ToString(), "% of Mp Leech Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_STAMINA_RECOVERY_RATE.ToString(), "Stamina Recovery Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_STAMINA_LEECH_RATE_RATE.ToString(), "% of Stamina Leech Rate: {0}%");
            // Format - 1.66c - New stats
            Texts.Add(UIFormatKeys.UI_FORMAT_GOLD_RATE.ToString(), "Gold Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_EXP_RATE.ToString(), "Exp Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_GOLD_RATE_RATE.ToString(), "% of Gold Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_EXP_RATE_RATE.ToString(), "% of Exp Rate: {0}%");
            // Format - 1.67b - Cash shop item generator
            Texts.Add(UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_TITLE.ToString(), "{0} x {1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_DESCRIPTION.ToString(), "Buy {0} x {1}\n\n{2}");
            // Format - 1.68b - Asset bundle
            Texts.Add(UIFormatKeys.UI_FORMAT_LOADING_ASSET_BUNDLE_FILE_NAME.ToString(), "Loading File: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_LOADED_ASSET_BUNDLES_COUNT.ToString(), "Loaded Files: {0}/{1}");
            // Format - 1.71c - Corpse items container
            Texts.Add(UIFormatKeys.UI_FORMAT_CORPSE_TITLE.ToString(), "{0}'s corpse");
            // 1.74 New Formats
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_SKILL_POINT.ToString(), "Require Skill Points: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_SKILL_POINT_NOT_ENOUGH.ToString(), "Require Skill Points: <color=red>{0}/{1}</color>");
            // 1.78 UI Chat Message With Guild Name
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_LOCAL_WITH_GUILD_NAME.ToString(), "<color=white>(LOCAL) {0}[{2}]: {1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_GLOBAL_WITH_GUILD_NAME.ToString(), "<color=white>(GLOBAL) {0}[{2}]: {1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_WHISPER_WITH_GUILD_NAME.ToString(), "<color=green>(WHISPER) {0}[{2}]: {1}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHAT_PARTY_WITH_GUILD_NAME.ToString(), "<color=cyan>(PARTY) {0}[{2}]: {1}</color>");
            // 1.80 New Formats
            Texts.Add(UIFormatKeys.UI_FORMAT_REWARD_STAT_POINTS.ToString(), "Reward Stat Points: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_REWARD_SKILL_POINTS.ToString(), "Reward Skill Points: {0}");
            // 1.81c New Formats
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_EXPIRE_DURATION.ToString(), "Duration: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_EXPIRE_TIME.ToString(), "Expires In: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_BATTLE_POINTS.ToString(), "Battle Points: {0}");
            // 1.81h New Formats
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_DROP_RATE.ToString(), "Item Drop Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_ITEM_DROP_RATE_RATE.ToString(), "% of Item Drop Rate: {0}%");
            // 1.83g New Formats
            Texts.Add(UIFormatKeys.UI_FORMAT_JUMP_HEIGHT.ToString(), "Jump Height: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_JUMP_HEIGHT_RATE.ToString(), "% of Jump Height: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_HEAD_DAMAGE_ABSORBS.ToString(), "Head Damage Absorbs: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_HEAD_DAMAGE_ABSORBS_RATE.ToString(), "% of Head Damage Absorbs: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_BODY_DAMAGE_ABSORBS.ToString(), "Body Damage Absorbs: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_BODY_DAMAGE_ABSORBS_RATE.ToString(), "% of Body Damage Absorbs: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_REQUIRE_FACTION.ToString(), "Require Faction: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_INVALID_REQUIRE_FACTION.ToString(), "Require Faction: <color=red>{0}</color>");
            // 1.83g2 New Formats
            Texts.Add(UIFormatKeys.UI_FORMAT_FALL_DAMAGE_ABSORBS.ToString(), "Fall Damage Absorbs: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_FALL_DAMAGE_ABSORBS_RATE.ToString(), "% of Fall Damage Absorbs: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_GRAVITY_RATE.ToString(), "Gravity Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_GRAVITY_RATE_RATE.ToString(), "% of Gravity Rate: {0}%");
            // PK
            Texts.Add(UIFormatKeys.UI_FORMAT_PK_CAN_TURN_PK_AFTER_HOURS.ToString(), "You can turn PK off after {0} hours");
            Texts.Add(UIFormatKeys.UI_FORMAT_PK_TURN_ON_WARNING.ToString(), "Do you want to turn PK on? if you do that then you will be able to turn it off after {0} hours");
            // Error - Dueling
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_IS_DUELING.ToString(), "Character is in another duel");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_ACCEPT_DUELING_REQUEST.ToString(), "Cannot accept dueling request");
            Texts.Add(UITextKeys.UI_ERROR_DUELING_REQUEST_DECLINED.ToString(), "Dueling request declined");
            // Error - Safe Zone
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_IS_IN_SAFE_AREA.ToString(), "Character is in safe area");
            // Error - Channels
            Texts.Add(UITextKeys.UI_ERROR_INVALID_CHANNEL_ID.ToString(), "Invalid channel");
            Texts.Add(UITextKeys.UI_ERROR_CHANNEL_IS_FULL.ToString(), "Channel is full");
            // 1.85 Armor Rate
            Texts.Add(UIFormatKeys.UI_FORMAT_ARMOR_RATE.ToString(), "{0} Armor: {1}%");
            // 1.85 Damage Rate
            Texts.Add(UIFormatKeys.UI_FORMAT_DAMAGE_RATE.ToString(), "{0}~{1}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_DAMAGE_WITH_ELEMENTAL_RATE.ToString(), "{0} Damage: {1}~{2}%");
            // 1.86 Status Effect Applying
            Texts.Add(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_SELF.ToString(), "Applies {0}, Lv. {1} to user");
            Texts.Add(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_ENEMY.ToString(), "Applies {0}, Lv. {1} to enemy");
            Texts.Add(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_SELF_WHEN_ATTACKING.ToString(), "Applies {0}, Lv. {1} to user when attacking");
            Texts.Add(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_ENEMY_WHEN_ATTACKING.ToString(), "Applies {0}, Lv. {1} to enemy when attacking");
            Texts.Add(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_SELF_WHEN_ATTACKED.ToString(), "Applies {0}, Lv. {1} to user when attacked");
            Texts.Add(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_ENEMY_WHEN_ATTACKED.ToString(), "Applies {0}, Lv. {1} to enemy when attacked");
            // 1.86 Buff formats
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_MAX_STACK.ToString(), "Max stack: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_WHEN_ATTACK_CHANCE.ToString(), "This buff have chance {0}% to be removed when attack");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_WHEN_ATTACKED_CHANCE.ToString(), "This buff have chance {0}% to be removed when attacked");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_USE_SKILL_CHANCE.ToString(), "This buff have chance {0}% to be removed when use skill");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_USE_ITEM_CHANCE.ToString(), "This buff have chance {0}% to be removed when use item");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_REMOVE_BUFF_PICKUP_ITEM_CHANCE.ToString(), "This buff have chance {0}% to be removed when pick up item");
            // 1.86 Buff labels
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_MOVE.ToString(), "Disallow to move while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_SPRINT.ToString(), "Disallow to sprint while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_WALK.ToString(), "Disallow to walk while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_JUMP.ToString(), "Disallow to jump while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_CROUCH.ToString(), "Disallow to crouch while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_CRAWL.ToString(), "Disallow to crawl while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_ATTACK.ToString(), "Disallow to attack while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_USE_SKILL.ToString(), "Disallow to use skill while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_USE_ITEM.ToString(), "Disallow to use item while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_FREEZE_ANIMATION.ToString(), "Character will be freezed while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_IS_HIDE.ToString(), "Character will be hidden from other characters");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_IS_REVEALS_HIDE.ToString(), "Character will be able to see hidden characters");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_IS_BLIND.ToString(), "Character will not be able to see other characters");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_MUTE_FOOTSTEP_SOUND.ToString(), "Mute footstep sound while this buff is activated");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DO_NOT_REMOVE_ON_DEAD.ToString(), "This buff will not be removed when character dead");
            Texts.Add(UITextKeys.UI_LABEL_BUFF_IS_EXTEND_DURATION.ToString(), "Buff duration can be extended by activate this buff again");
            // 1.86 Status Effect Resistance
            Texts.Add(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_RESISTANCE_ENTRY.ToString(), "Lv.{0} {1}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_STATUS_EFFECT_RESISTANCE_ENTRIES.ToString(), "{0} resistance: {1}");
            // 1.86 Buff Removal
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_REMOVAL_ENTRY.ToString(), "Lv.{0} {1}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_REMOVAL_ENTRIES.ToString(), "Chance to remove {0}: {1}");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUFF_REMOVAL_NO_ENTRIES.ToString(), "Have chance to remove {0}");
            // 1.86d Items Limit Error
            Texts.Add(UITextKeys.UI_ERROR_REACHED_DEALING_ITEMS_LIMIT.ToString(), "Cannot add more items to offer");
            Texts.Add(UITextKeys.UI_ERROR_REACHED_REFINE_ENHANCER_ITEMS_LIMIT.ToString(), "Cannot add more items to enhance");
            // Character Info Formats
            Texts.Add(UIGenderKeys.UI_GENDER_MALE.ToString(), "Male");
            Texts.Add(UIGenderKeys.UI_GENDER_FEMALE.ToString(), "Female");
            Texts.Add(UIGenderKeys.UI_GENDER_UNSPECIFIED.ToString(), "Unspecified");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHARACTER_GENDER.ToString(), "Gender: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHARACTER_AGE.ToString(), "Age: <color=#d44849ff>{0}</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHARACTER_HEIGHT.ToString(), "Height: <color=#d44849ff>{0} cm.</color>");
            Texts.Add(UIFormatKeys.UI_FORMAT_CHARACTER_WEIGHT.ToString(), "Weight: <color=#d44849ff>{0} kg.</color>");
            // 1.87
            Texts.Add(UITextKeys.UI_LABEL_BUFF_DISALLOW_DASH.ToString(), "Disallow Dash");
            Texts.Add(UIFormatKeys.UI_FORMAT_SKILL_ATTACK_WEAPON_DAMAGE_MULTIPLICATOR.ToString(), "Attack Weapon Damage Multiplicator: {0}");
            // 1.89
            Texts.Add(UITextKeys.UI_ERROR_CONTENT_IS_LOCKED.ToString(), "Content is not unlocked yet");
            Texts.Add(UITextKeys.UI_ERROR_CONTENT_IS_UNLOCKED.ToString(), "Content is already unlocked");
            Texts.Add(UITextKeys.UI_ERROR_NOT_ENOUGH_UNLOCK_PROGRESSION.ToString(), "Not enough unlock progression");
            Texts.Add(UITextKeys.UI_ERROR_CONTENT_IS_NOT_LOCKED.ToString(), "Content is not locked, don't have to unlock");
            // 1.91 New Stats
            Texts.Add(UIFormatKeys.UI_FORMAT_PROTECTED_SLOT_LIMIT.ToString(), "Protected Inventory Slots: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_PROTECTED_SLOT_LIMIT_RATE.ToString(), "Protected Inventory Slots Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_AMMO_CAPACITY.ToString(), "Ammo Capacity: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_AMMO_CAPACITY_RATE.ToString(), "Ammo Capacity Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_RECOIL_RATE.ToString(), "Recoil Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_RECOIL_RATE_RATE.ToString(), "% of Recoil Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_RATE_OF_FIRE.ToString(), "Rate Of Fire: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_RATE_OF_FIRE_RATE.ToString(), "Rate Of Fire Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_RELOAD_DURATION.ToString(), "Reload Duration: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_RELOAD_DURATION_RATE.ToString(), "Reload Duration Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_FIRE_SPREAD_RANGE_RATE.ToString(), "Fire Spread Range Rate: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_FIRE_SPREAD_RANGE_RATE_RATE.ToString(), "% of Fire Spread Range Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_FIRE_SPREAD.ToString(), "Fire Spread: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_FIRE_SPREAD_RATE.ToString(), "Fire Spread Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_DECREASE_FOOD_DECREATION.ToString(), "Food Decreation: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_DECREASE_WATER_DECREATION.ToString(), "Water Decreation: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_DECREASE_STAMINA_DECREATION.ToString(), "Stamina Decreation: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUY_ITEM_PRICE_RATE.ToString(), "Buy Item Price Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_BUY_ITEM_PRICE_RATE_RATE.ToString(), "% of Buy Item Price Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_SELL_ITEM_PRICE_RATE.ToString(), "Sell Item Price Rate: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_SELL_ITEM_PRICE_RATE_RATE.ToString(), "% of Sell Item Price Rate: {0}%");
            // 1.91 Repair Error Messages
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_REPAIR_ITEM_NOT_BROKEN.ToString(), "Cannot repair the item, it is not broken");
            Texts.Add(UITextKeys.UI_ERROR_CANNOT_REPAIR_ITEM_UNBREAKABLE.ToString(), "Cannot repair the item, it is unbreakable");
            // 1.92
            Texts.Add(UIFormatKeys.UI_FORMAT_TAX.ToString(), "Tax: {0}");
            // Chat Error
            Texts.Add(UITextKeys.UI_ERROR_CHAT_MUTED.ToString(), "You have been muted");
            Texts.Add(UITextKeys.UI_ERROR_CHAT_ENTER_TOO_FAST.ToString(), "Please wait a moment before sending your next chat message");
            // ID
            Texts.Add(UITextKeys.UI_USER_ID_COPIED.ToString(), "User's ID is copied");
            Texts.Add(UITextKeys.UI_CHARACTER_ID_COPIED.ToString(), "Character's ID is copied");
            // Character Name Changing
            Texts.Add(UITextKeys.UI_CHARACTER_NAME_CHANGE.ToString(), "Character Name Changing");
            Texts.Add(UITextKeys.UI_CHARACTER_NAME_CHANGE_DESCRIPTION.ToString(), "Which name do you want to make changes?");
            Texts.Add(UITextKeys.UI_CHARACTER_NAME_CHANGE_SUCCESS.ToString(), "Character name change success");
            // 1.94
            Texts.Add(UIFormatKeys.UI_FORMAT_RECOIL_MODIFIER.ToString(), "Recoil: {0}%");
            Texts.Add(UIFormatKeys.UI_FORMAT_RECOIL_MODIFIER_RATE.ToString(), "% of Recoil: {0}%");
            // 1.95
            Texts.Add(UIFormatKeys.UI_FORMAT_FEE.ToString(), "Fee: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SPRINT_SPEED.ToString(), "Sprint Speed: {0}");
            Texts.Add(UIFormatKeys.UI_FORMAT_SPRINT_SPEED_RATE.ToString(), "Sprint Speed: {0}%");
            Texts.Add(UITextKeys.UI_ERROR_UNABLE_TO_JUMP.ToString(), "Unable To Jump");
            Texts.Add(UITextKeys.UI_ERROR_UNABLE_TO_DASH.ToString(), "Unable To Dash");
            Texts.Add(UITextKeys.UI_ERROR_UNABLE_TO_CROUCH.ToString(), "Unable To Crouch");
            Texts.Add(UITextKeys.UI_ERROR_UNABLE_TO_CRAWL.ToString(), "Unable To Crawl");
            Texts.Add(UITextKeys.UI_ERROR_UNABLE_TO_STAND.ToString(), "Unable To Stand");
            // 1.96
            Texts.Add(UIFormatKeys.UI_FORMAT_NOTIFY_UNLOCKABLE_CONTENT_UPDATED.ToString(), "Obtain {0} x {1} ea");
            Texts.Add(UIFormatKeys.UI_FORMAT_NOTIFY_UNLOCKABLE_CONTENT_UNLOCKED.ToString(), "{0} is unlocked");
            // Enter game error
            Texts.Add(UITextKeys.UI_ERROR_INVALID_PACKET_VERSION.ToString(), "Invalid packet version");
            Texts.Add(UITextKeys.UI_ERROR_UNABLE_TO_VALIDATE_USER.ToString(), "Unable to validate user");
            Texts.Add(UITextKeys.UI_ERROR_UNABLE_TO_VALIDATE_CHARACTER.ToString(), "Unable to validate character");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_NOT_PLAYABLE.ToString(), "Character is not playable");
            Texts.Add(UITextKeys.UI_ERROR_USER_IS_ALREADY_ONLINE.ToString(), "User is already online");
            Texts.Add(UITextKeys.UI_ERROR_CHARACTER_IS_ALREADY_ONLINE.ToString(), "Character is already online");
        }
    }
}