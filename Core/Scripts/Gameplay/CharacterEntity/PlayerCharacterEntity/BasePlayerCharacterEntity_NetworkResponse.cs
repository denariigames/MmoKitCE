using LiteNetLibManager;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        [ServerRpc]
        protected void CmdUseGuildSkill(int dataId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (this.IsDead())
                return;

            GuildSkill guildSkill;
            if (!GameInstance.GuildSkills.TryGetValue(dataId, out guildSkill) || !guildSkill.IsActive)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_GUILD_SKILL_DATA);
                return;
            }

            GuildData guild;
            if (GuildId <= 0 || !GameInstance.ServerGuildHandlers.TryGetGuild(GuildId, out guild))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NOT_JOINED_GUILD);
                return;
            }

            int level = guild.GetSkillLevel(dataId);
            if (level <= 0)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_SKILL_LEVEL_IS_ZERO);
                return;
            }

            if (this.IndexOfSkillUsage(SkillUsageType.GuildSkill, dataId) >= 0)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_SKILL_IS_COOLING_DOWN);
                return;
            }

            // Set cooldown to user
            this.AddOrUpdateSkillUsage(SkillUsageType.GuildSkill, dataId, level);

            SocialCharacterData[] members = guild.GetMembers();
            BasePlayerCharacterEntity memberEntity;
            foreach (SocialCharacterData member in members)
            {
                if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterById(member.id, out memberEntity))
                {
                    memberEntity.ApplyBuff(dataId, BuffType.GuildSkillBuff, level, GetInfo(), CharacterItem.Empty);
                }
            }
#endif
        }

        [ServerRpc]
        protected void CmdAssignHotkey(string hotkeyId, HotkeyType type, string relateId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            CharacterHotkey characterHotkey = new CharacterHotkey();
            characterHotkey.hotkeyId = hotkeyId;
            characterHotkey.type = type;
            characterHotkey.relateId = relateId;
            int hotkeyIndex = this.IndexOfHotkey(hotkeyId);
            if (hotkeyIndex >= 0)
                hotkeys[hotkeyIndex] = characterHotkey;
            else
                hotkeys.Add(characterHotkey);
#endif
        }

        [ServerRpc]
        protected void CmdEnterWarp(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!CanDoActions())
                return;

            if (!Manager.TryGetEntityByObjectId(objectId, out WarpPortalEntity warpPortalEntity))
            {
                // Can't find the entity
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DATA);
                return;
            }

            if (!IsGameEntityInDistance(warpPortalEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            warpPortalEntity.EnterWarp(this);
#endif
        }

        [ServerRpc]
        protected void CmdAppendCraftingQueueItem(uint sourceObjectId, int dataId, int amount)
        {
            UITextKeys errorMessage;
            if (sourceObjectId == ObjectId)
            {
                if (!CraftingComponent.AppendCraftingQueueItem(this, dataId, amount, out errorMessage))
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, errorMessage);
            }
            else if (CurrentGameManager.TryGetEntityByObjectId(sourceObjectId, out ICraftingQueueSource source))
            {
                if (!source.AppendCraftingQueueItem(this, dataId, amount, out errorMessage))
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, errorMessage);
            }
            else
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DATA);
            }
        }

        [ServerRpc]
        protected void CmdChangeCraftingQueueItem(uint sourceObjectId, int indexOfData, int amount)
        {
            if (sourceObjectId == ObjectId)
            {
                CraftingComponent.ChangeCraftingQueueItem(this, indexOfData, amount);
            }
            else if (CurrentGameManager.TryGetEntityByObjectId(sourceObjectId, out ICraftingQueueSource source))
            {
                source.ChangeCraftingQueueItem(this, indexOfData, amount);
            }
            else
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DATA);
            }
        }

        [ServerRpc]
        protected void CmdCancelCraftingQueueItem(uint sourceObjectId, int indexOfData)
        {
            if (sourceObjectId == ObjectId)
            {
                CraftingComponent.CancelCraftingQueueItem(this, indexOfData);
            }
            else if (CurrentGameManager.TryGetEntityByObjectId(sourceObjectId, out ICraftingQueueSource source))
            {
                source.CancelCraftingQueueItem(this, indexOfData);
            }
            else
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DATA);
            }
        }

        [ServerRpc]
        protected void CmdDropGold(int gold)
        {
            if (gold < 0)
                return;

            if (gold > Gold)
                return;

            if (CurrentGameInstance.monsterGoldRewardingMode == RewardingMode.DropOnGround)
                GoldDropEntity.Drop(this, 1f, RewardGivenType.PlayerDrop, Level, Level, gold, System.Array.Empty<string>()).Forget();

            Gold -= gold;
        }
    }
}
