using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using LiteNetLibManager;
using System;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultGMCommands : BaseGMCommands
    {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
        /// <summary>
        /// Get list of all GM commands
        /// </summary>
        public const string Help = "/help";
        /// <summary>
        /// Set character level: /level {level}
        /// </summary>
        public const string Level = "/level";
        /// <summary>
        /// Set character stat point: /statpoint {Stat Point}
        /// </summary>
        public const string StatPoint = "/statpoint";
        /// <summary>
        /// Set character skill point: /skillpoint {Skill Point}
        /// </summary>
        public const string SkillPoint = "/skillpoint";
        /// <summary>
        /// Set character gold: /gold {Gold}
        /// </summary>
        public const string Gold = "/gold";
        /// <summary>
        /// Give character item: /item {Item Id} {amount}
        /// </summary>
        public const string AddItem = "/add_item";
        /// <summary>
        /// Give gold to another character: /give_gold {Character Name} {Gold}
        /// </summary>
        public const string GiveGold = "/give_gold";
        /// <summary>
        /// Give item to another character: /give_item {Character Name} {Item Id} {amount}
        /// </summary>
        public const string GiveItem = "/give_item";
        /// <summary>
        /// Set gold rate (0.1 = 1%, 1 = 100%)
        /// </summary>
        public const string GoldRate = "/gold_rate";
        /// <summary>
        /// Set exp rate (0.1 = 1%, 1 = 100%)
        /// </summary>
        public const string ExpRate = "/exp_rate";
        /// <summary>
        /// Warp to specific map
        /// </summary>
        public const string Warp = "/warp";
        /// <summary>
        /// Warp to specific character to specific map and position
        /// </summary>
        public const string WarpCharacter = "/warp_character";
        /// <summary>
        /// Warp to specific character
        /// </summary>
        public const string WarpToCharacter = "/warp_to_character";
        /// <summary>
        /// Summon specific character
        /// </summary>
        public const string Summon = "/summon";
        /// <summary>
        /// Summon monster
        /// </summary>
        public const string Monster = "/monster";
        /// <summary>
        /// Kill specific character
        /// </summary>
        public const string Kill = "/kill";
        /// <summary>
        /// Suicide
        /// </summary>
        public const string Suicide = "/suicide";
        /// <summary>
        /// Mute specific character
        /// </summary>
        public const string Mute = "/mute";
        /// <summary>
        /// Unmute specific character
        /// </summary>
        public const string Unmute = "/unmute";
        /// <summary>
        /// Ban specific character
        /// </summary>
        public const string Ban = "/ban";
        /// <summary>
        /// Unban specific character
        /// </summary>
        public const string Unban = "/unban";
        /// <summary>
        /// Kick specific character
        /// </summary>
        public const string Kick = "/kick";
        /// <summary>
        /// Disable force hide
        /// </summary>
        public const string Visible = "/visible";
        /// <summary>
        /// Enable force hide
        /// </summary>
        public const string Invisible = "/invisible";
        /// <summary>
        /// Close the servers
        /// </summary>
        public const string CloseServers = "/close_servers";
        /// <summary>
        /// Close the server by channel ID
        /// </summary>
        public const string CloseServer = "/close_server";

        public const string HelpResponse = "/level {level} = Set character's level to {level} value.\n" +
            "/statpoint {amount} = Set character's stat point to {amount} value.\n" +
            "/skillpoint {amount} = Set character's skill point to {amount} value.\n" +
            "/gold {amount} = Set character's gold to {amount} value.\n" +
            "/add_item {item_id} {amount} = Add item which its ID is {item_id} (if item ID have spaces, use _ for spaces) x {amount}.\n" +
            "/give_gold {name} {amount} = Increase {amount} of gold to character which its name is {name}.\n" +
            "/give_item {name} {item_id} {amount} = Add item which its ID is {item_id} (if item ID have spaces, use _ for spaces) x {amount} to character which its name is {name}.\n" +
            "/gold_rate {rate} = Set server's gold drop rate to {rate}.\n" +
            "/exp_rate {rate} = Set server's exp rewarding rate to {rate}.\n" +
            "/warp {map_id} = Warp to specific map (if map ID have spaces, use _ for spaces).\n" +
            "/warp_character {name} {map_id} {x} {y} {z} = Warp to specific character to specific map and position (if map ID have spaces, use _ for spaces).\n" +
            "/warp_to_character {name} = Warp to character which its name is {name}.\n" +
            "/summon {name} = Summon character which its name is {name}.\n" +
            "/monster {monster_id} {level} {amount} = Summon monster entity which its ID is {monster_id} (if prefab name have spaces, use _ for spaces), lv. {level}, amount {amount}.\n" +
            "/kill {name} = Kill character which its name is {name}.\n" +
            "/suicide = Kill yourself.\n" +
            "/mute {name} {duration} = Mute character which its name is {name} for {duration} minutes.\n" +
            "/unmute {name} = Unmute character which its name is {name}.\n" +
            "/ban {name} {duration} = Ban character's account which its name is {name} for {duration} days.\n" +
            "/unban {name} = Unban character's account which its name is {name}.\n" +
            "/kick {name} = Kick character which its name is {name}.\n" +
            "/visible = Show user character to other players.\n" +
            "/invisible = Hide user character from other players.\n" +
            "/close_servers = Close the servers.\n" +
            "/close_server {channel_id} = Close the server by channel ID.\n";
#endif

        public virtual bool IsDataLengthValid(string command, int dataLength)
        {
            if (string.IsNullOrEmpty(command))
                return false;
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (string.Equals(command, Help, StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(command, Level, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, StatPoint, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, SkillPoint, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, Gold, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, AddItem, StringComparison.OrdinalIgnoreCase) && dataLength == 3)
                return true;
            if (string.Equals(command, GiveGold, StringComparison.OrdinalIgnoreCase) && dataLength == 3)
                return true;
            if (string.Equals(command, GiveItem, StringComparison.OrdinalIgnoreCase) && dataLength == 4)
                return true;
            if (string.Equals(command, GoldRate, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, ExpRate, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, Warp, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, WarpCharacter, StringComparison.OrdinalIgnoreCase) && dataLength == 6)
                return true;
            if (string.Equals(command, WarpToCharacter, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, Summon, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, Monster, StringComparison.OrdinalIgnoreCase) && dataLength == 4)
                return true;
            if (string.Equals(command, Kill, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, Suicide, StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(command, Mute, StringComparison.OrdinalIgnoreCase) && dataLength == 3)
                return true;
            if (string.Equals(command, Unmute, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, Ban, StringComparison.OrdinalIgnoreCase) && dataLength == 3)
                return true;
            if (string.Equals(command, Unban, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, Kick, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
            if (string.Equals(command, Visible, StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(command, Invisible, StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(command, CloseServers, StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(command, CloseServer, StringComparison.OrdinalIgnoreCase) && dataLength == 2)
                return true;
#endif
            return false;
        }

        public override bool IsGMCommand(string chatMessage, out string command)
        {
            command = string.Empty;
            if (string.IsNullOrEmpty(chatMessage))
                return false;
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            string[] splited = chatMessage.Split(' ');
            command = splited[0];
            if (string.Equals(command, Help, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Level, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, StatPoint, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, SkillPoint, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Gold, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, AddItem, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, GiveGold, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, GiveItem, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, GoldRate, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, ExpRate, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Warp, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, WarpCharacter, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, WarpToCharacter, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Summon, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Monster, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Kill, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Suicide, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Mute, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Unmute, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Ban, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Unban, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Kick, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Visible, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, Invisible, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, CloseServers, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, CloseServer, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
#endif
            command = string.Empty;
            return false;
        }

        public override bool CanUseGMCommand(int userLevel, string command)
        {
            // TODO: May allow user to use some GM commands by their user level.
            return userLevel > 0;
        }

        public override async UniTask<string> HandleGMCommand(string sender, BasePlayerCharacterEntity senderCharacter, string chatMessage)
        {
            if (string.IsNullOrEmpty(chatMessage))
                return string.Empty;
            string response = string.Empty;
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            string[] data = chatMessage.Split(' ');
            string commandKey = data[0];
            string receiver;
            BasePlayerCharacterEntity targetCharacter;
            if (IsDataLengthValid(commandKey, data.Length))
            {
                if (string.Equals(commandKey, Help, StringComparison.OrdinalIgnoreCase))
                {
                    response = HelpResponse;
                }
                if (string.Equals(commandKey, Level, StringComparison.OrdinalIgnoreCase))
                {
                    int amount;
                    if (!int.TryParse(data[1], out amount) || amount <= 0)
                    {
                        response = "Wrong input data";
                    }
                    else if (senderCharacter != null)
                    {
                        senderCharacter.Level = amount;
                        response = $"Set character level to {amount}";
                    }
                }
                if (string.Equals(commandKey, StatPoint, StringComparison.OrdinalIgnoreCase))
                {
                    int amount;
                    if (!int.TryParse(data[1], out amount) || amount <= 0)
                    {
                        response = "Wrong input data";
                    }
                    else if (senderCharacter != null)
                    {
                        senderCharacter.StatPoint = amount;
                        response = $"Set character statpoint to {amount}";
                    }
                }
                if (string.Equals(commandKey, SkillPoint, StringComparison.OrdinalIgnoreCase))
                {
                    int amount;
                    if (!int.TryParse(data[1], out amount) || amount <= 0)
                    {
                        response = "Wrong input data";
                    }
                    else if (senderCharacter != null)
                    {
                        senderCharacter.SkillPoint = amount;
                        response = $"Set character skillpoint to {amount}";
                    }
                }
                if (string.Equals(commandKey, Gold, StringComparison.OrdinalIgnoreCase))
                {
                    int amount;
                    if (!int.TryParse(data[1], out amount) || amount <= 0)
                    {
                        response = "Wrong input data";
                    }
                    else if (senderCharacter != null)
                    {
                        senderCharacter.Gold = amount;
                        response = $"Set character gold to {amount}";
                    }
                }
                if (string.Equals(commandKey, AddItem, StringComparison.OrdinalIgnoreCase))
                {
                    BaseItem targetItem = null;
                    foreach (BaseItem item in GameInstance.Items.Values)
                    {
                        if (item.name.Equals(data[1]) ||
                            item.name.Replace(' ', '_').Equals(data[1]) ||
                            item.Id.Equals(data[1]) ||
                            item.Id.Replace(' ', '_').Equals(data[1]))
                        {
                            targetItem = item;
                            break;
                        }
                    }
                    int amount;
                    if (!int.TryParse(data[2], out amount) || amount <= 0)
                    {
                        response = "Wrong input data";
                    }
                    else if (targetItem == null)
                    {
                        response = "Cannot find the item";
                    }
                    else if (senderCharacter != null)
                    {
                        if (senderCharacter.IncreasingItemsWillOverwhelming(targetItem.DataId, amount))
                        {
                            response = $"Cannot add item {targetItem.Title}x{amount}, cannot carry any more of those items";
                        }
                        else
                        {
                            senderCharacter.IncreaseItems(CharacterItem.Create(targetItem, 1, amount));
                            GameInstance.ServerGameMessageHandlers.NotifyRewardItem(senderCharacter.ConnectionId, RewardGivenType.GM, targetItem.DataId, amount);
                            response = $"Add item {targetItem.Title}x{amount} to character's inventory";
                        }
                    }
                }
                if (string.Equals(commandKey, GiveGold, StringComparison.OrdinalIgnoreCase))
                {
                    receiver = data[1];
                    int amount;
                    if (!int.TryParse(data[2], out amount) || amount <= 0)
                    {
                        response = "Wrong input data";
                    }
                    else if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterByName(receiver, out targetCharacter))
                    {
                        targetCharacter.Gold = targetCharacter.Gold.Increase(amount);
                        GameInstance.ServerGameMessageHandlers.NotifyRewardGold(targetCharacter.ConnectionId, RewardGivenType.GM, amount);
                        response = $"Add gold for character: {receiver}";
                    }
                }
                if (string.Equals(commandKey, GiveItem, StringComparison.OrdinalIgnoreCase))
                {
                    receiver = data[1];
                    BaseItem targetItem = null;
                    foreach (BaseItem item in GameInstance.Items.Values)
                    {
                        if (item.name.Equals(data[2]) ||
                            item.name.Replace(' ', '_').Equals(data[2]) ||
                            item.Id.Equals(data[2]) ||
                            item.Id.Replace(' ', '_').Equals(data[2]))
                        {
                            targetItem = item;
                            break;
                        }
                    }
                    int amount;
                    if (!int.TryParse(data[3], out amount) || amount <= 0)
                    {
                        response = "Wrong input data";
                    }
                    else if (targetItem == null)
                    {
                        response = "Cannot find the item";
                    }
                    else if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterByName(receiver, out targetCharacter))
                    {
                        if (targetCharacter.IncreasingItemsWillOverwhelming(targetItem.DataId, amount))
                        {
                            response = $"Cannot add item {targetItem.Title}x{amount} to {receiver}'s inventory, cannot carry any more of those items";
                        }
                        else
                        {
                            targetCharacter.IncreaseItems(CharacterItem.Create(targetItem, 1, amount));
                            GameInstance.ServerGameMessageHandlers.NotifyRewardItem(targetCharacter.ConnectionId, RewardGivenType.GM, targetItem.DataId, amount);
                            response = $"Add item {targetItem.Title}x{amount} to {receiver}'s inventory";
                        }
                    }
                }
                if (string.Equals(commandKey, GoldRate, StringComparison.OrdinalIgnoreCase))
                {
                    float amount;
                    if (!float.TryParse(data[1], out amount) || amount < 0f)
                    {
                        response = "Wrong input data";
                    }
                    else
                    {
                        GameInstance.Singleton.GameplayRule.GoldRate = amount;
                        response = $"Set gold rate to {amount}";
                    }
                }
                if (string.Equals(commandKey, ExpRate, StringComparison.OrdinalIgnoreCase))
                {
                    float amount;
                    if (!float.TryParse(data[1], out amount) || amount < 0f)
                    {
                        response = "Wrong input data";
                    }
                    else
                    {
                        GameInstance.Singleton.GameplayRule.ExpRate = amount;
                        response = $"Set exp rate to {amount}";
                    }
                }
                if (string.Equals(commandKey, Warp, StringComparison.OrdinalIgnoreCase))
                {
                    BaseMapInfo targetMapInfo = null;
                    foreach (BaseMapInfo mapInfo in GameInstance.MapInfos.Values)
                    {
                        if (mapInfo.name.Equals(data[1]) ||
                            mapInfo.name.Replace(' ', '_').Equals(data[1]) ||
                            mapInfo.Id.Equals(data[1]) ||
                            mapInfo.Id.Replace(' ', '_').Equals(data[1]))
                        {
                            targetMapInfo = mapInfo;
                            break;
                        }
                    }
                    if (targetMapInfo == null)
                    {
                        response = "Cannot find the map";
                    }
                    else if (senderCharacter != null)
                    {
                        BaseMapInfo mapInfo = GameInstance.MapInfos[data[1]];
                        await BaseGameNetworkManager.Singleton.WarpCharacter(senderCharacter, data[1], mapInfo.StartPosition, false, Vector3.zero);
                        response = $"Warping to: {data[1]} {mapInfo.StartPosition}";
                    }
                }
                if (string.Equals(commandKey, WarpCharacter, StringComparison.OrdinalIgnoreCase))
                {
                    BaseMapInfo targetMapInfo = null;
                    foreach (BaseMapInfo mapInfo in GameInstance.MapInfos.Values)
                    {
                        if (mapInfo.name.Equals(data[2]) ||
                            mapInfo.name.Replace(' ', '_').Equals(data[2]) ||
                            mapInfo.Id.Equals(data[2]) ||
                            mapInfo.Id.Replace(' ', '_').Equals(data[2]))
                        {
                            targetMapInfo = mapInfo;
                            break;
                        }
                    }
                    float x, y, z;
                    if (!float.TryParse(data[3], out x) ||
                        !float.TryParse(data[4], out y) ||
                        !float.TryParse(data[5], out z))
                    {
                        response = "Wrong input data";
                    }
                    else if (targetMapInfo == null)
                    {
                        response = "Cannot find the map";
                    }
                    else if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterByName(data[1], out targetCharacter))
                    {
                        await BaseGameNetworkManager.Singleton.WarpCharacter(targetCharacter, data[2], new Vector3(x, y, z), false, Vector3.zero);
                    }
                }
                if (string.Equals(commandKey, WarpToCharacter, StringComparison.OrdinalIgnoreCase))
                {
                    if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterByName(data[1], out targetCharacter))
                    {
                        string resendCommand = $"{WarpCharacter} {sender} {BaseGameNetworkManager.CurrentMapInfo.Id.Replace(' ', '_')} {targetCharacter.MovementTransform.position.x} {targetCharacter.MovementTransform.position.y} {targetCharacter.MovementTransform.position.z}";
                        BaseGameNetworkManager.Singleton.ServerSendLocalMessage(sender, resendCommand);
                    }
                }
                if (string.Equals(commandKey, Summon, StringComparison.OrdinalIgnoreCase))
                {
                    if (senderCharacter != null)
                    {
                        string resendCommand = $"{WarpCharacter} {data[1]} {BaseGameNetworkManager.CurrentMapInfo.Id.Replace(' ', '_')} {senderCharacter.MovementTransform.position.x} {senderCharacter.MovementTransform.position.y} {senderCharacter.MovementTransform.position.z}";
                        BaseGameNetworkManager.Singleton.ServerSendLocalMessage(sender, resendCommand);
                    }
                }
                if (string.Equals(commandKey, Monster, StringComparison.OrdinalIgnoreCase))
                {
                    BaseMonsterCharacterEntity targetMonster = null;
#if !DISABLE_ADDRESSABLES
                    foreach (AssetReferenceBaseMonsterCharacterEntity addressableMonster in GameInstance.AddressableMonsterCharacterEntities.Values)
                    {
                        BaseMonsterCharacterEntity monster = await addressableMonster.GetOrLoadAssetAsync<BaseMonsterCharacterEntity>();
                        if (monster == null)
                            continue;
                        if (monster.name.Equals(data[1]) ||
                            monster.name.Replace(' ', '_').Equals(data[1]) ||
                            monster.Identity.AssetId.Equals(data[1]))
                        {
                            targetMonster = monster;
                            break;
                        }
                    }
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                    if (targetMonster == null)
                    {
                        foreach (BaseMonsterCharacterEntity monster in GameInstance.MonsterCharacterEntities.Values)
                        {
                            if (monster == null)
                                continue;
                            if (monster.name.Equals(data[1]) ||
                                monster.name.Replace(' ', '_').Equals(data[1]) ||
                                monster.Identity.AssetId.Equals(data[1]))
                            {
                                targetMonster = monster;
                                break;
                            }
                        }
                    }
#endif
                    int level;
                    int amount;
                    if (targetMonster == null)
                    {
                        response = "Cannot find the monster";
                    }
                    else if (!int.TryParse(data[2], out level) || !int.TryParse(data[3], out amount))
                    {
                        response = "Wrong input data";
                    }
                    else if (senderCharacter != null)
                    {
                        for (int i = 0; i < amount; ++i)
                        {
                            LiteNetLibIdentity spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                                targetMonster.Identity.HashAssetId,
                                senderCharacter.MovementTransform.position,
                                Quaternion.identity);
                            BaseMonsterCharacterEntity entity = spawnObj.GetComponent<BaseMonsterCharacterEntity>();
                            entity.Level = level;
                            BaseGameNetworkManager.Singleton.Assets.NetworkSpawn(spawnObj);
                        }
                    }
                }
                if (string.Equals(commandKey, Kill, StringComparison.OrdinalIgnoreCase))
                {
                    if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterByName(data[1], out targetCharacter))
                    {
                        if (targetCharacter.IsDead())
                        {
                            response = "Character is already dead";
                        }
                        else
                        {
                            targetCharacter.CurrentHp = 0;
                            targetCharacter.Killed(EntityInfo.Empty);
                            response = $"Kill character: {data[1]}";
                        }
                    }
                }
                if (string.Equals(commandKey, Suicide, StringComparison.OrdinalIgnoreCase))
                {
                    if (senderCharacter != null)
                    {
                        senderCharacter.CurrentHp = 0;
                        senderCharacter.Killed(EntityInfo.Empty);
                        response = "Suicided";
                    }
                }
                if (string.Equals(commandKey, Mute, StringComparison.OrdinalIgnoreCase))
                {
                    int amount;
                    if (!int.TryParse(data[2], out amount) || amount < 0)
                    {
                        response = "Wrong input data";
                    }
                    else
                    {
                        GameInstance.ServerUserHandlers.MuteCharacterByName(data[1], amount);
                        response = $"Mute character named: {data[1]}";
                    }
                }
                if (string.Equals(commandKey, Unmute, StringComparison.OrdinalIgnoreCase))
                {
                    GameInstance.ServerUserHandlers.UnmuteCharacterByName(data[1]);
                    response = $"Unmute character named: {data[1]}";
                }
                if (string.Equals(commandKey, Ban, StringComparison.OrdinalIgnoreCase))
                {
                    int amount;
                    if (!int.TryParse(data[2], out amount) || amount < 0)
                    {
                        response = "Wrong input data";
                    }
                    else
                    {
                        GameInstance.ServerUserHandlers.BanUserByCharacterName(data[1], amount);
                        response = $"Ban user's who own character named: {data[1]}";
                    }
                }
                if (string.Equals(commandKey, Unban, StringComparison.OrdinalIgnoreCase))
                {
                    GameInstance.ServerUserHandlers.UnbanUserByCharacterName(data[1]);
                    response = $"Unban user's who own character named: {data[1]}";
                }
                if (string.Equals(commandKey, Kick, StringComparison.OrdinalIgnoreCase))
                {
                    if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacterByName(data[1], out targetCharacter))
                    {
                        response = "Cannot find the character";
                    }
                    else
                    {
                        BaseGameNetworkManager.Singleton.KickClient(targetCharacter.ConnectionId, UITextKeys.UI_ERROR_KICKED_FROM_SERVER);
                        response = $"Kick character: {data[1]}";
                    }
                }
                if (string.Equals(commandKey, Visible, StringComparison.OrdinalIgnoreCase))
                {
                    if (senderCharacter != null)
                        senderCharacter.ForceHide = false;
                    response = "Your character is shown to other players";
                }
                if (string.Equals(commandKey, Invisible, StringComparison.OrdinalIgnoreCase))
                {
                    if (senderCharacter != null)
                        senderCharacter.ForceHide = true;
                    response = "Your character is hidden from other players";
                }
                if (string.Equals(commandKey, CloseServers, StringComparison.OrdinalIgnoreCase))
                {
                    var players = BaseGameNetworkManager.Singleton.GetPlayers();
                    foreach (var player in players)
                    {
                        BaseGameNetworkManager.Singleton.KickClient(player.ConnectionId, UITextKeys.UI_ERROR_SERVER_CLOSE);
                    }
                    BaseGameNetworkManager.Singleton.IsTemporarilyClose = true;
                    response = "Server closing";
                }
                if (string.Equals(commandKey, CloseServer, StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(data[1], BaseGameNetworkManager.Singleton.ChannelId, StringComparison.OrdinalIgnoreCase))
                    {
                        var players = BaseGameNetworkManager.Singleton.GetPlayers();
                        foreach (var player in players)
                        {
                            BaseGameNetworkManager.Singleton.KickClient(player.ConnectionId, UITextKeys.UI_ERROR_SERVER_CLOSE);
                        }
                        BaseGameNetworkManager.Singleton.IsTemporarilyClose = true;
                        response = "Server closing";
                    }
                }
            }
#endif
                    return response;
        }
    }
}
