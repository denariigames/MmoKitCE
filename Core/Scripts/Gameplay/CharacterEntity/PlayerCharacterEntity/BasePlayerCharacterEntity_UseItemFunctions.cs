using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        /// <summary>
        /// This function will be called at server to order character to use item
        /// </summary>
        /// <param name="itemIndex"></param>
        [ServerRpc]
        protected void CmdUseItem(int itemIndex)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!CanUseItem())
                return;

            if (!this.ValidateUsableItemToUse(itemIndex, out IUsableItem usableItem, out UITextKeys gameMessage))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, gameMessage);
                return;
            }

            // Set cooldown to user
            this.AddOrUpdateSkillUsage(SkillUsageType.UsableItem, nonEquipItems[itemIndex].dataId, 1);

            // Use item
            if (!usableItem.UseItem(this, itemIndex, nonEquipItems[itemIndex]))
                return;

            // Do something with buffs when use item
            SkillAndBuffComponent.OnUseItem();
#endif
        }

        [ServerRpc]
        protected async void CmdUseCharacterNameChangeItem(int itemIndex, string newCharacterName)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!CanUseItem())
                return;

            if (!this.ValidateUsableItemToUse(itemIndex, out IUsableItem usableItem, out UITextKeys gameMessage))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, gameMessage);
                return;
            }

            // Validate item
            if (usableItem is not CharacterNameChangeItem)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_ITEM_DATA);
                return;
            }

            // Validate new character name
            UITextKeys validateCharacterName = await GameInstance.ServerUserHandlers.ValidateCharacterName(newCharacterName);
            if (validateCharacterName != UITextKeys.NONE)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, validateCharacterName);
                return;
            }

            // Use item
            if (!usableItem.UseItem(this, itemIndex, nonEquipItems[itemIndex]))
                return;

            // Name changed
            CharacterName = newCharacterName;
            GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_CHARACTER_NAME_CHANGE_SUCCESS);
#endif
        }
    }
}
