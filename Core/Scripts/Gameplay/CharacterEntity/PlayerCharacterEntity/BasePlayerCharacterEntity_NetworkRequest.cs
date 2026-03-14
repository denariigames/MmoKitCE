using LiteNetLib;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        public bool ValidateRequestUseItem(int itemIndex)
        {
            if (!CanUseItem())
                return false;

            if (!UpdateLastActionTime())
                return false;

            float time = Time.unscaledTime;

            if (!nonEquipItems.Get(itemIndex).IsRewardingItem() && time - LastUseItemTime < CurrentGameInstance.useItemDelay)
                return false;

            if (!this.ValidateUsableItemToUse(itemIndex, out _, out UITextKeys gameMessage))
            {
                ClientGenericActions.ClientReceiveGameMessage(gameMessage);
                return false;
            }

            LastUseItemTime = time;
            return true;
        }

        public bool CallCmdUseItem(int index)
        {
            if (!ValidateRequestUseItem(index))
                return false;
            if (nonEquipItems.Get(index).IsCharacterNameChangeItem())
            {
                UISceneGlobal.Singleton.ShowInputDialog(
                    LanguageManager.GetText(UITextKeys.UI_CHARACTER_NAME_CHANGE.ToString()),
                    LanguageManager.GetText(UITextKeys.UI_CHARACTER_NAME_CHANGE_DESCRIPTION.ToString()),
                    (newCharacterName) =>
                    {
                        RPC(CmdUseCharacterNameChangeItem, index, newCharacterName);
                    }, CharacterName);
            }
            else
            {
                RPC(CmdUseItem, index);
            }
            return true;
        }

        public bool CallCmdUseGuildSkill(int dataId)
        {
            if (this.IsDead())
                return false;
            RPC(CmdUseGuildSkill, dataId);
            return true;
        }

        public bool CallCmdAssignHotkey(string hotkeyId, HotkeyType type, string id)
        {
            RPC(CmdAssignHotkey, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, hotkeyId, type, id);
            return true;
        }

        public bool AssignSkillHotkey(string hotkeyId, CharacterSkill characterSkill)
        {
            // Use skil data id
            string relateId = characterSkill.GetSkill().Id;
            return CallCmdAssignHotkey(hotkeyId, HotkeyType.Skill, relateId);
        }

        public bool AssignItemHotkey(string hotkeyId, CharacterItem characterItem)
        {
            // Usable items will use item data id
            string relateId = characterItem.GetItem().Id;
            // For an equipments, it will use item unique id
            if (characterItem.GetEquipmentItem() != null)
                relateId = characterItem.id;
            return CallCmdAssignHotkey(hotkeyId, HotkeyType.Item, relateId);
        }

        public bool AssignGuildSkillHotkey(string hotkeyId, GuildSkill guildSkill)
        {
            // Use skil data id
            string relateId = guildSkill.Id;
            return CallCmdAssignHotkey(hotkeyId, HotkeyType.GuildSkill, relateId);
        }

        public bool UnAssignHotkey(string hotkeyId)
        {
            return CallCmdAssignHotkey(hotkeyId, HotkeyType.None, string.Empty);
        }

        public bool CallCmdEnterWarp(uint objectId)
        {
            if (!CanDoActions())
                return false;
            RPC(CmdEnterWarp, objectId);
            return true;
        }

        public bool CallCmdAppendCraftingQueueItem(uint sourceObjectId, int dataId, int amount)
        {
            if (!CurrentGameplayRule.CanInteractEntity(this, sourceObjectId))
                return false;
            RPC(CmdAppendCraftingQueueItem, sourceObjectId, dataId, amount);
            return true;
        }

        public bool CallCmdChangeCraftingQueueItem(uint sourceObjectId, int indexOfData, int amount)
        {
            if (!CurrentGameplayRule.CanInteractEntity(this, sourceObjectId))
                return false;
            RPC(CmdChangeCraftingQueueItem, sourceObjectId, indexOfData, amount);
            return true;
        }

        public bool CallCmdCancelCraftingQueueItem(uint sourceObjectId, int indexOfData)
        {
            if (!CurrentGameplayRule.CanInteractEntity(this, sourceObjectId))
                return false;
            RPC(CmdCancelCraftingQueueItem, sourceObjectId, indexOfData);
            return true;
        }

        public bool CallCmdChangeQuestTracking(int questDataId, bool isTracking)
        {
            RPC(CmdChangeQuestTracking, questDataId, isTracking);
            return true;
        }

        public bool CallCmdDropGold(int gold)
        {
            if (gold < 0)
                return false;

            if (gold > Gold)
                return false;

            RPC(CmdDropGold, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, gold);
            return true;
        }
    }
}
