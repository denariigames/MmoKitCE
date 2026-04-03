namespace MultiplayerARPG
{
    public enum UIFormatKeys : ushort
    {
        UI_CUSTOM,

        // Format - Generic
        /// <summary>
        /// Format => {0} = {Value}
        /// </summary>
        UI_FORMAT_SIMPLE = 5,
        /// <summary>
        /// Format => {0} = {Value}
        /// </summary>
        UI_FORMAT_SIMPLE_PERCENTAGE,
        /// <summary>
        /// Format => {0} = {Min Value}, {1} = {Max Value}
        /// </summary>
        UI_FORMAT_SIMPLE_MIN_TO_MAX,
        /// <summary>
        /// Format => {0} = {Min Value}, {1} = {Max Value}
        /// </summary>
        UI_FORMAT_SIMPLE_MIN_BY_MAX,
        /// <summary>
        /// Format => {0} = {Level}
        /// </summary>
        UI_FORMAT_LEVEL,
        /// <summary>
        /// Format => {0} = {Current Exp}, {1} = {Exp To Level Up}
        /// </summary>
        UI_FORMAT_CURRENT_EXP,
        /// <summary>
        /// Format => {0} = {Stat Points}
        /// </summary>
        UI_FORMAT_STAT_POINTS,
        /// <summary>
        /// Format => {0} = {Skill Points}
        /// </summary>
        UI_FORMAT_SKILL_POINTS,
        /// <summary>
        /// Format => {0} = {Current Hp}, {1} = {Max Hp}
        /// </summary>
        UI_FORMAT_CURRENT_HP,
        /// <summary>
        /// Format => {0} = {Current Mp}, {1} = {Max Mp}
        /// </summary>
        UI_FORMAT_CURRENT_MP,
        /// <summary>
        /// Format => {0} = {Current Stamina}, {1} = {Max Stamina}
        /// </summary>
        UI_FORMAT_CURRENT_STAMINA,
        /// <summary>
        /// Format => {0} = {Current Food}, {1} = {Max Food}
        /// </summary>
        UI_FORMAT_CURRENT_FOOD,
        /// <summary>
        /// Format => {0} = {Current Water}, {1} = {Max Water}
        /// </summary>
        UI_FORMAT_CURRENT_WATER,
        /// <summary>
        /// Format => {0} = {Current Weight}, {1} = {Weight Limit}
        /// </summary>
        UI_FORMAT_CURRENT_WEIGHT,
        /// <summary>
        /// Format => {0} = {Current Slot}, {1} = {Slot Limit}
        /// </summary>
        UI_FORMAT_CURRENT_SLOT,

        // Format - Character Stats
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_HP,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_MP,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_STAMINA,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_FOOD,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_WATER,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_ACCURACY = 26,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_EVASION,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_CRITICAL_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_CRITICAL_DAMAGE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_BLOCK_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_BLOCK_DAMAGE_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_MOVE_SPEED,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_ATTACK_SPEED,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_WEIGHT,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_SLOT,
        /// <summary>
        /// Format => {0} = {Gold Amount}
        /// </summary>
        UI_FORMAT_GOLD = 38,
        /// <summary>
        /// Format => {0} = {Cash Amount}
        /// </summary>
        UI_FORMAT_CASH,
        /// <summary>
        /// Format => {0} = {Sell Price}
        /// </summary>
        UI_FORMAT_SELL_PRICE,
        /// <summary>
        /// Format => {0} = {Character Level}
        /// </summary>
        UI_FORMAT_REQUIRE_LEVEL,
        /// <summary>
        /// Format => {0} = {Character Classes}
        /// </summary>
        UI_FORMAT_REQUIRE_CLASS,
        /// <summary>
        /// Format => {0} = {List Of Weapon Type}
        /// </summary>
        UI_FORMAT_AVAILABLE_WEAPONS,
        /// <summary>
        /// Format => {0} = {Consume Mp}
        /// </summary>
        UI_FORMAT_CONSUME_MP,

        // Format - Skill
        /// <summary>
        /// Format => {0} = {Skill Cooldown Duration}
        /// </summary>
        UI_FORMAT_SKILL_COOLDOWN_DURATION,
        /// <summary>
        /// Format => {0} = {Skill Type}
        /// </summary>
        UI_FORMAT_SKILL_TYPE,

        // Format - Buff
        /// <summary>
        /// Format => {0} = {Buff Duration}
        /// </summary>
        UI_FORMAT_BUFF_DURATION = 50,
        /// <summary>
        /// Format => {0} = {Buff Recovery Hp}
        /// </summary>
        UI_FORMAT_BUFF_RECOVERY_HP,
        /// <summary>
        /// Format => {0} = {Buff Recovery Mp}
        /// </summary>
        UI_FORMAT_BUFF_RECOVERY_MP,
        /// <summary>
        /// Format => {0} = {Buff Recovery Stamina}
        /// </summary>
        UI_FORMAT_BUFF_RECOVERY_STAMINA,
        /// <summary>
        /// Format => {0} = {Buff Recovery Food}
        /// </summary>
        UI_FORMAT_BUFF_RECOVERY_FOOD,
        /// <summary>
        /// Format => {0} = {Buff Recovery Water}
        /// </summary>
        UI_FORMAT_BUFF_RECOVERY_WATER,

        // Format -  Item
        /// <summary>
        /// Format => {0} = {Level - 1}
        /// </summary>
        UI_FORMAT_ITEM_REFINE_LEVEL,
        /// <summary>
        /// Format => {0} = {Item Title}, {1} = {Level - 1}
        /// </summary>
        UI_FORMAT_ITEM_TITLE_WITH_REFINE_LEVEL,
        /// <summary>
        /// Format => {0} = {Item Type}
        /// </summary>
        UI_FORMAT_ITEM_TYPE,
        /// <summary>
        /// Format => {0} = {Item Rarity}
        /// </summary>
        UI_FORMAT_ITEM_RARITY = 66,
        /// <summary>
        /// Format => {0} = {Item Current Amount}, {1} = {Item Max Amount}
        /// </summary>
        UI_FORMAT_ITEM_STACK,
        /// <summary>
        /// Format => {0} = {Item Current Durability}, {1} = {Item Max Durability}
        /// </summary>
        UI_FORMAT_ITEM_DURABILITY,

        // Format -  Social
        /// <summary>
        /// Format => {0} = {Character Name}
        /// </summary>
        UI_FORMAT_SOCIAL_LEADER,
        /// <summary>
        /// Format => {0} = {Current Amount}, {1} = {Max Amount}
        /// </summary>
        UI_FORMAT_SOCIAL_MEMBER_AMOUNT,
        /// <summary>
        /// Format => {0} = {Current Amount}
        /// </summary>
        UI_FORMAT_SOCIAL_MEMBER_AMOUNT_NO_LIMIT,
        /// <summary>
        /// Format => {0} = {Share Exp}
        /// </summary>
        UI_FORMAT_SHARE_EXP_PERCENTAGE,
        /// <summary>
        /// Format => {0} = {Exp Amount}
        /// </summary>
        UI_FORMAT_REWARD_EXP,
        /// <summary>
        /// Format => {0} = {Gold Amount}
        /// </summary>
        UI_FORMAT_REWARD_GOLD,
        /// <summary>
        /// Format => {0} = {Cash Amount}
        /// </summary>
        UI_FORMAT_REWARD_CASH,

        // Format - Attribute Amount
        /// <summary>
        /// Format => {0} = {Attribute Title}, {1} = {Current Amount}, {2} = {Target Amount}
        /// </summary>
        UI_FORMAT_CURRENT_ATTRIBUTE,
        /// <summary>
        /// Format => {0} = {Attribute Title}, {1} = {Current Amount}, {2} = {Target Amount}
        /// </summary>
        UI_FORMAT_CURRENT_ATTRIBUTE_NOT_ENOUGH,
        /// <summary>
        /// Format => {0} = {Attribute Title}, {1} = {Amount}
        /// </summary>
        UI_FORMAT_ATTRIBUTE_AMOUNT,

        // Format - Resistance Amount
        /// <summary>
        /// Format => {0} = {Resistance Title}, {1} = {Amount * 100}
        /// </summary>
        UI_FORMAT_RESISTANCE_AMOUNT,

        // Format - Skill Level
        /// <summary>
        /// Format => {0} = {Skill Title}, {1} = {Current Level}, {2} = {Target Level}
        /// </summary>
        UI_FORMAT_CURRENT_SKILL,
        /// <summary>
        /// Format => {0} = {Skill Title}, {1} = {Current Level}, {2} = {Target Level}
        /// </summary>
        UI_FORMAT_CURRENT_SKILL_NOT_ENOUGH,
        /// <summary>
        /// Format => {0} = {Skill Title}, {1} = {Target Level}
        /// </summary>
        UI_FORMAT_SKILL_LEVEL,

        // Format - Item Amount
        /// <summary>
        /// Format => {0} = {Item Title}, {1} = {Current Amount}, {2} = {Target Amount}
        /// </summary>
        UI_FORMAT_CURRENT_ITEM,
        /// <summary>
        /// Format => {0} = {Item Title}, {1} = {Current Amount}, {2} = {Target Amount}
        /// </summary>
        UI_FORMAT_CURRENT_ITEM_NOT_ENOUGH,
        /// <summary>
        /// Format => {0} = {Item Title}, {1} = {Target Amount}
        /// </summary>
        UI_FORMAT_ITEM_AMOUNT,

        // Format - Damage
        /// <summary>
        /// Format => {0} = {Min Damage}, {1} = {Max Damage}
        /// </summary>
        UI_FORMAT_DAMAGE_AMOUNT,
        /// <summary>
        /// Format => {0} = {Damage Element Title}, {1} = {Min Damage}, {2} = {Max Damage}
        /// </summary>
        UI_FORMAT_DAMAGE_WITH_ELEMENTAL,
        /// <summary>
        /// Format => {0} = {Infliction * 100}
        /// </summary>
        UI_FORMAT_DAMAGE_INFLICTION,
        /// <summary>
        /// Format => {0} = {Damage Element Title}, {1} => {Infliction * 100}
        /// </summary>
        UI_FORMAT_DAMAGE_INFLICTION_AS_ELEMENTAL,

        // Format - Gold Amount
        /// <summary>
        /// Format => {0} = {Current Gold Amount}, {1} = {Target Amount}
        /// </summary>
        UI_FORMAT_REQUIRE_GOLD,
        /// <summary>
        /// Format => {0} = {Current Gold Amount}, {1} = {Target Amount}
        /// </summary>
        UI_FORMAT_REQUIRE_GOLD_NOT_ENOUGH,

        // Format - UI Equipment Set
        /// <summary>
        /// Format => {0} = {Set Title}, {1} = {List Of Effect}
        /// </summary>
        UI_FORMAT_EQUIPMENT_SET,
        /// <summary>
        /// Format => {0} = {Equip Amount}, {1} = {List Of Bonus}
        /// </summary>
        UI_FORMAT_EQUIPMENT_SET_APPLIED_EFFECT,
        /// <summary>
        /// Format => {0} = {Equip Amount}, {1} = {List Of Bonus}
        /// </summary>
        UI_FORMAT_EQUIPMENT_SET_UNAPPLIED_EFFECT,

        // Format - UI Equipment Socket
        /// <summary>
        /// Format => {0} = {Socket Index}, {1} = {Item Title}, {2} = {List Of Bonus}
        /// </summary>
        UI_FORMAT_EQUIPMENT_SOCKET_FILLED,
        /// <summary>
        /// Format => {0} = {Socket Index}
        /// </summary>
        UI_FORMAT_EQUIPMENT_SOCKET_EMPTY,

        // Refine Item
        /// <summary>
        /// Format => {0} = {Rate * 100}
        /// </summary>
        UI_FORMAT_REFINE_SUCCESS_RATE,
        /// <summary>
        /// Format => {0} = {Refining Level}
        /// </summary>
        UI_FORMAT_REFINING_LEVEL,

        // Format - Guild Bonus
        UI_FORMAT_INCREASE_MAX_MEMBER,
        UI_FORMAT_INCREASE_EXP_GAIN_PERCENTAGE,
        UI_FORMAT_INCREASE_GOLD_GAIN_PERCENTAGE,
        UI_FORMAT_INCREASE_SHARE_EXP_GAIN_PERCENTAGE,
        UI_FORMAT_INCREASE_SHARE_GOLD_GAIN_PERCENTAGE,
        UI_FORMAT_DECREASE_EXP_PENALTY_PERCENTAGE,

        // Format - UI Character Quest
        /// <summary>
        /// Format => {0} = {Title}
        /// </summary>
        UI_FORMAT_QUEST_TITLE_ON_GOING,
        /// <summary>
        /// Format => {0} = {Title}
        /// </summary>
        UI_FORMAT_QUEST_TITLE_TASKS_COMPLETE,
        /// <summary>
        /// Format => {0} = {Title}
        /// </summary>
        UI_FORMAT_QUEST_TITLE_COMPLETE,

        // Format - UI Quest Task
        /// <summary>
        /// Format => {0} = {Title}, {1} = {Progress}, {2} = {Amount}
        /// </summary>
        UI_FORMAT_QUEST_TASK_KILL_MONSTER,
        /// <summary>
        /// Format => {0} = {Title}, {1} = {Progress}, {2} = {Amount}
        /// </summary>
        UI_FORMAT_QUEST_TASK_COLLECT_ITEM,
        /// <summary>
        /// Format => {0} = {Title}, {1} = {Progress}, {2} = {Amount}
        /// </summary>
        UI_FORMAT_QUEST_TASK_KILL_MONSTER_COMPLETE,
        /// <summary>
        /// Format => {0} = {Title}, {1} = {Progress}, {2} = {Amount}
        /// </summary>
        UI_FORMAT_QUEST_TASK_COLLECT_ITEM_COMPLETE,

        // UI Chat Message
        /// <summary>
        /// Format => {0} = {Character Name}, {1} = {Message}
        /// </summary>
        UI_FORMAT_CHAT_LOCAL,
        /// <summary>
        /// Format => {0} = {Character Name}, {1} = {Message}
        /// </summary>
        UI_FORMAT_CHAT_GLOBAL,
        /// <summary>
        /// Format => {0} = {Character Name}, {1} = {Message}
        /// </summary>
        UI_FORMAT_CHAT_WHISPER,
        /// <summary>
        /// Format => {0} = {Character Name}, {1} = {Message}
        /// </summary>
        UI_FORMAT_CHAT_PARTY,
        /// <summary>
        /// Format => {0} = {Character Name}, {1} = {Message}
        /// </summary>
        UI_FORMAT_CHAT_GUILD,
        /// <summary>
        /// Format => {0} = {Message}
        /// </summary>
        UI_FORMAT_CHAT_SYSTEM,

        // Format - Armor Amount
        /// <summary>
        /// Format => {0} = {Damage Element Title}, {1} = {Target Amount}
        /// </summary>
        UI_FORMAT_ARMOR_AMOUNT = 197,
        // Format - Character Stats Rate
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_HP_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_MP_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_STAMINA_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_FOOD_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_WATER_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_ACCURACY_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_EVASION_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_CRITICAL_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_CRITICAL_DAMAGE_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_BLOCK_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_BLOCK_DAMAGE_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_MOVE_SPEED_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_ATTACK_SPEED_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_WEIGHT_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_SLOT_RATE,

        // Format - Attribute Amount Rate
        /// <summary>
        /// Format => {0} = {Attribute Title}, {1} = {Amount * 100}
        /// </summary>
        UI_FORMAT_ATTRIBUTE_RATE,

        // Format - Item Building
        /// <summary>
        /// Format => {0} = {Building Title}
        /// </summary>
        UI_FORMAT_ITEM_BUILDING,

        // Format - Item Pet
        /// <summary>
        /// Format => {0} = {Pet Title}
        /// </summary>
        UI_FORMAT_ITEM_PET,

        // Format - Item Mount
        /// <summary>
        /// Format => {0} = {Mount Title}
        /// </summary>
        UI_FORMAT_ITEM_MOUNT,

        // Format - Item Skill
        /// <summary>
        /// Format => {0} = {Skill Title}, {1} = {Skill Level}
        /// </summary>
        UI_FORMAT_ITEM_SKILL,

        // Format - Skill Summon
        /// <summary>
        /// Format => {0} = {Monster Title}, {1} = {Monster Level}, {2} = {Amount}, {3} = {Max Stack}, {4} = {Duration}
        /// </summary>
        UI_FORMAT_SKILL_SUMMON,
        // Format - Skill Mount
        /// <summary>
        /// Format => {0} = {Mount Title}
        /// </summary>
        UI_FORMAT_SKILL_MOUNT,

        // Format - Skip Title
        /// <summary>
        /// Format => {1} = {Value}
        /// </summary>
        UI_FORMAT_SKIP_TITLE,
        /// <summary>
        /// Format => {1} = {Value}
        /// </summary>
        UI_FORMAT_SKIP_TITLE_PERCENTAGE,

        // Format - Notify Rewards
        /// <summary>
        /// Format => {0} = {Exp Amount}
        /// </summary>
        UI_FORMAT_NOTIFY_REWARD_EXP,
        /// <summary>
        /// Format => {0} = {Gold Amount}
        /// </summary>
        UI_FORMAT_NOTIFY_REWARD_GOLD,
        /// <summary>
        /// Format => {0} = {Item Title}, {1} = {Amount}
        /// </summary>
        UI_FORMAT_NOTIFY_REWARD_ITEM,

        // 1.61 Talk to NPC quest task
        /// <summary>
        /// Format => {0} = {Title}
        /// </summary>
        UI_FORMAT_QUEST_TASK_TALK_TO_NPC,
        /// <summary>
        /// Format => {0} = {Title}
        /// </summary>
        UI_FORMAT_QUEST_TASK_TALK_TO_NPC_COMPLETE,
        // Format - Currency Amount
        /// <summary>
        /// Format => {0} = {Currency Title}, {1} = {Current Amount}, {2} = {Target Amount}
        /// </summary>
        UI_FORMAT_CURRENT_CURRENCY,
        /// <summary>
        /// Format => {0} = {Currency Title}, {1} = {Current Amount}, {2} = {Target Amount}
        /// </summary>
        UI_FORMAT_CURRENT_CURRENCY_NOT_ENOUGH,
        /// <summary>
        /// Format => {0} = {Currency Title}, {1} = {Amount}
        /// </summary>
        UI_FORMAT_CURRENCY_AMOUNT,

        // 1.61b New Formats
        /// <summary>
        /// Format => {0} = {Consume Hp}
        /// </summary>
        UI_FORMAT_CONSUME_HP,
        /// <summary>
        /// Format => {0} = {Consume Stamina}
        /// </summary>
        UI_FORMAT_CONSUME_STAMINA,
        /// <summary>
        /// Format => {0} = {Sender Name}
        /// </summary>
        UI_FORMAT_MAIL_SENDER_NAME,
        /// <summary>
        /// Format => {0} = {Title}
        /// </summary>
        UI_FORMAT_MAIL_TITLE,
        /// <summary>
        /// Format => {0} = {Content}
        /// </summary>
        UI_FORMAT_MAIL_CONTENT,
        /// <summary>
        /// Format => {0} = {Sent Date}
        /// </summary>
        UI_FORMAT_MAIL_SENT_DATE,

        // 1.62 New Formats
        /// <summary>
        /// Format => {0} = {List Of Armor Type}
        /// </summary>
        UI_FORMAT_AVAILABLE_ARMORS,
        /// <summary>
        /// Format => {0} = {List Of Vehicle Type}
        /// </summary>
        UI_FORMAT_AVAILABLE_VEHICLES,
        /// <summary>
        /// Format => {0} = {Craft Duration}
        /// </summary>
        UI_FORMAT_CRAFT_DURATION,

        // 1.63 New Formats
        /// <summary>
        /// Format => {0} = {Currency Title}, {1} = {Amount}
        /// </summary>
        UI_FORMAT_REWARD_CURRENCY,
        /// <summary>
        /// Format => {0} = {Currency Title}, {1} = {Amount}
        /// </summary>
        UI_FORMAT_NOTIFY_REWARD_CURRENCY,

        // 1.65f New Formats
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_HP_RECOVERY,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_HP_LEECH_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_MP_RECOVERY,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_MP_LEECH_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_STAMINA_RECOVERY,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_STAMINA_LEECH_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_HP_RECOVERY_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_HP_LEECH_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_MP_RECOVERY_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_MP_LEECH_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_STAMINA_RECOVERY_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_STAMINA_LEECH_RATE_RATE,

        // 1.66c New Formats
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_GOLD_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_EXP_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_GOLD_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_EXP_RATE_RATE,

        // 1.67b New Formats
        /// <summary>
        /// Format => {0} = {Item's Title}, {1} = {Amount}
        /// </summary>
        UI_FORMAT_GENERATE_CAST_SHOP_ITEM_TITLE,
        /// <summary>
        /// Format => {0} = {Item's Title}, {1} = {Amount}, {2} = {Item's Description}
        /// </summary>
        UI_FORMAT_GENERATE_CAST_SHOP_ITEM_DESCRIPTION,

        // 1.68b New Formats
        /// <summary>
        /// Format => {0} = {Loading Asset Bundle File Name}
        /// </summary>
        UI_FORMAT_LOADING_ASSET_BUNDLE_FILE_NAME,
        /// <summary>
        /// Format => {0} = {Current Loaded Asset Bundles Count}, {1} = {Total Loading Asset Bundles Count}
        /// </summary>
        UI_FORMAT_LOADED_ASSET_BUNDLES_COUNT,

        // 1.71 New Formats
        /// <summary>
        /// Format => {0} = {Character Level}, {1} = {Require Level}
        /// </summary>
        UI_FORMAT_REQUIRE_LEVEL_NOT_ENOUGH,
        /// <summary>
        /// Format => {0} = {Character Classes}
        /// </summary>
        UI_FORMAT_INVALID_REQUIRE_CLASS,

        // 1.71c New Formats
        UI_FORMAT_CORPSE_TITLE,

        // 1.74 New Formats
        /// <summary>
        /// Format => {0} = {Require Skill Point}
        /// </summary>
        UI_FORMAT_REQUIRE_SKILL_POINT,
        /// <summary>
        /// Format => {0} = {Current Skill Point}, {1} = {Require Skill Point}
        /// </summary>
        UI_FORMAT_REQUIRE_SKILL_POINT_NOT_ENOUGH,

        // 1.78 UI Chat Message With Guild Name
        /// <summary>
        /// Format => {0} = {Character Name}, {1} = {Message}, {2} = {Guild Name}
        /// </summary>
        UI_FORMAT_CHAT_LOCAL_WITH_GUILD_NAME,
        /// <summary>
        /// Format => {0} = {Character Name}, {1} = {Message}, {2} = {Guild Name}
        /// </summary>
        UI_FORMAT_CHAT_GLOBAL_WITH_GUILD_NAME,
        /// <summary>
        /// Format => {0} = {Character Name}, {1} = {Message}, {2} = {Guild Name}
        /// </summary>
        UI_FORMAT_CHAT_WHISPER_WITH_GUILD_NAME,
        /// <summary>
        /// Format => {0} = {Character Name}, {1} = {Message}, {2} = {Guild Name}
        /// </summary>
        UI_FORMAT_CHAT_PARTY_WITH_GUILD_NAME,

        // 1.80 New Formats
        /// <summary>
        /// Format => {0} = {Stat Points}
        /// </summary>
        UI_FORMAT_REWARD_STAT_POINTS,
        /// <summary>
        /// Format => {0} = {Skill Points}
        /// </summary>
        UI_FORMAT_REWARD_SKILL_POINTS,

        // 1.81c New Formats
        /// <summary>
        /// Format => {0} = {Duration to be expired}
        /// </summary>
        UI_FORMAT_ITEM_EXPIRE_DURATION,
        /// <summary>
        /// Format => {0} = {When it will be expired}
        /// </summary>
        UI_FORMAT_ITEM_EXPIRE_TIME,
        /// <summary>
        /// Format => {0} = {Battle Points}
        /// </summary>
        UI_FORMAT_BATTLE_POINTS,

        // 1.81h New Formats
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_ITEM_DROP_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_ITEM_DROP_RATE_RATE,

        // 1.83g New Formats
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_JUMP_HEIGHT,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_JUMP_HEIGHT_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_HEAD_DAMAGE_ABSORBS,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_HEAD_DAMAGE_ABSORBS_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_BODY_DAMAGE_ABSORBS,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_BODY_DAMAGE_ABSORBS_RATE,
        /// <summary>
        /// Format => {0} = {Factions}
        /// </summary>
        UI_FORMAT_REQUIRE_FACTION,
        /// <summary>
        /// Format => {0} = {Factions}
        /// </summary>
        UI_FORMAT_INVALID_REQUIRE_FACTION,

        // 1.83g2 New Stats
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_FALL_DAMAGE_ABSORBS,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_FALL_DAMAGE_ABSORBS_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_GRAVITY_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_GRAVITY_RATE_RATE,

        // 1.84 PK
        /// <summary>
        /// Format => {0} = {Hours}
        /// </summary>
        UI_FORMAT_PK_CAN_TURN_PK_AFTER_HOURS,
        /// <summary>
        /// Format => {0} = {Hours}
        /// </summary>
        UI_FORMAT_PK_TURN_ON_WARNING,

        // 1.85 Armor Rate
        /// <summary>
        /// Format => {0} = {Damage Element Title}, {1} = {Target Amount * 100}
        /// </summary>
        UI_FORMAT_ARMOR_RATE,

        // 1.85 Damage Rate
        /// <summary>
        /// Format => {0} = {Min Damage * 100}, {1} = {Max Damage * 100}
        /// </summary>
        UI_FORMAT_DAMAGE_RATE,
        /// <summary>
        /// Format => {0} = {Damage Element Title}, {1} = {Min Damage * 100}, {2} = {Max Damage * 100}
        /// </summary>
        UI_FORMAT_DAMAGE_WITH_ELEMENTAL_RATE,

        // Format - Item Attribute
        /// <summary>
        /// Format => {0} = {Attribute Title}, {1} = {Attribute Amount}
        /// </summary>
        UI_FORMAT_ITEM_ATTRIBUTE,

        // 1.86 Status Effect Applying
        /// <summary>
        /// Format => {0} = {Status Effect Title}, {1} = {Level}
        /// </summary>
        UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_SELF,
        /// <summary>
        /// Format => {0} = {Status Effect Title}, {1} = {Level}
        /// </summary>
        UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_ENEMY,
        /// <summary>
        /// Format => {0} = {Status Effect Title}, {1} = {Level}
        /// </summary>
        UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_SELF_WHEN_ATTACKING,
        /// <summary>
        /// Format => {0} = {Status Effect Title}, {1} = {Level}
        /// </summary>
        UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_ENEMY_WHEN_ATTACKING,
        /// <summary>
        /// Format => {0} = {Status Effect Title}, {1} = {Level}
        /// </summary>
        UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_SELF_WHEN_ATTACKED,
        /// <summary>
        /// Format => {0} = {Status Effect Title}, {1} = {Level}
        /// </summary>
        UI_FORMAT_STATUS_EFFECT_APPLYING_TARGET_ENEMY_WHEN_ATTACKED,

        // 1.86 Buff
        /// <summary>
        /// Format => {0} = {Max Stack}
        /// </summary>
        UI_FORMAT_BUFF_MAX_STACK,
        /// <summary>
        /// Format => {0} = {Chance * 100}
        /// </summary>
        UI_FORMAT_BUFF_REMOVE_BUFF_WHEN_ATTACK_CHANCE,
        /// <summary>
        /// Format => {0} = {Chance * 100}
        /// </summary>
        UI_FORMAT_BUFF_REMOVE_BUFF_WHEN_ATTACKED_CHANCE,
        /// <summary>
        /// Format => {0} = {Chance * 100}
        /// </summary>
        UI_FORMAT_BUFF_REMOVE_BUFF_USE_SKILL_CHANCE,
        /// <summary>
        /// Format => {0} = {Chance * 100}
        /// </summary>
        UI_FORMAT_BUFF_REMOVE_BUFF_USE_ITEM_CHANCE,
        /// <summary>
        /// Format => {0} = {Chance * 100}
        /// </summary>
        UI_FORMAT_BUFF_REMOVE_BUFF_PICKUP_ITEM_CHANCE,

        // 1.86 Status Effect Resistance
        /// <summary>
        /// Format => {0} = {Level}, {1} = {Chance * 100}
        /// </summary>
        UI_FORMAT_STATUS_EFFECT_RESISTANCE_ENTRY,
        /// <summary>
        /// Format => {0} = {Status Effect}, {1} = {Entries}
        /// </summary>
        UI_FORMAT_STATUS_EFFECT_RESISTANCE_ENTRIES,

        // 1.86 Buff Removal
        /// <summary>
        /// Format => {0} = {Level}, {1} = {Chance * 100}
        /// </summary>
        UI_FORMAT_BUFF_REMOVAL_ENTRY,
        /// <summary>
        /// Format => {0} = {Status Effect}, {1} = {Entries}
        /// </summary>
        UI_FORMAT_BUFF_REMOVAL_ENTRIES,

        // Character Info Formats
        UI_FORMAT_CHARACTER_GENDER,
        UI_FORMAT_CHARACTER_AGE,
        UI_FORMAT_CHARACTER_HEIGHT,
        UI_FORMAT_CHARACTER_WEIGHT,

        // 1.87
        UI_FORMAT_SKILL_ATTACK_WEAPON_DAMAGE_MULTIPLICATOR,

        // Format - Cash Amount
        /// <summary>
        /// Format => {0} = {Current Cash Amount}, {1} = {Target Amount}
        /// </summary>
        UI_FORMAT_REQUIRE_CASH,
        /// <summary>
        /// Format => {0} = {Current Cash Amount}, {1} = {Target Amount}
        /// </summary>
        UI_FORMAT_REQUIRE_CASH_NOT_ENOUGH,

        // 1.90
        /// <summary>
        /// Format => {0} = {Level}
        /// </summary>
        UI_FORMAT_LEVEL_UP,
        /// <summary>
        /// Format => {0} = {Status Effect}
        /// </summary>
        UI_FORMAT_BUFF_REMOVAL_NO_ENTRIES,

        // 1.91 New Stats
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_PROTECTED_SLOT_LIMIT,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_PROTECTED_SLOT_LIMIT_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_AMMO_CAPACITY,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_AMMO_CAPACITY_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_RECOIL_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_RECOIL_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_RATE_OF_FIRE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_RATE_OF_FIRE_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_RELOAD_DURATION,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_RELOAD_DURATION_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_FIRE_SPREAD_RANGE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_FIRE_SPREAD_RANGE_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_FIRE_SPREAD,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_FIRE_SPREAD_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_DECREASE_FOOD_DECREATION,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_DECREASE_FOOD_DECREATION_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_DECREASE_WATER_DECREATION,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_DECREASE_WATER_DECREATION_RATE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_DECREASE_STAMINA_DECREATION,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_DECREASE_STAMINA_DECREATION_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_BUY_ITEM_PRICE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_BUY_ITEM_PRICE_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_SELL_ITEM_PRICE_RATE,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_SELL_ITEM_PRICE_RATE_RATE,
        /// <summary>
        /// Format => {0} = {Tax Amount}
        /// </summary>
        UI_FORMAT_TAX,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_RECOIL_MODIFIER,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_RECOIL_MODIFIER_RATE,
        /// <summary>
        /// Format => {0} = {Fee Amount}
        /// </summary>
        UI_FORMAT_FEE,
        /// <summary>
        /// Format => {0} = {Amount}
        /// </summary>
        UI_FORMAT_SPRINT_SPEED,
        /// <summary>
        /// Format => {0} = {Amount * 100}
        /// </summary>
        UI_FORMAT_SPRINT_SPEED_RATE,
        /// <summary>
        /// Format => {0} = {Item Title}, {1} = {Amount}
        /// </summary>
        UI_FORMAT_NOTIFY_UNLOCKABLE_CONTENT_UPDATED,
        /// <summary>
        /// Format => {0} = {Item Title}
        /// </summary>
        UI_FORMAT_NOTIFY_UNLOCKABLE_CONTENT_UNLOCKED,
    }
}