using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultServerLogHandlers : MonoBehaviour, IServerLogHandlers
    {
        // All functions in this class is intended to do nothing, you must implements `IServerLogHandlers` by yourself

        public void LogEnterGame(IPlayerCharacterData playerCharacter) { }
        public void LogExitGame(string characterId, string userId) { }

        public void LogEnterChat(ChatMessage chatMessage) { }

        public void LogAttackStart(IPlayerCharacterData playerCharacter, int simulateSeed, float[] triggerDurations, byte fireSpread, bool isLeftHand, CharacterItem weapon) { }
        public void LogAttackTrigger(IPlayerCharacterData playerCharacter, int simulateSeed, byte triggerIndex) { }
        public void LogAttackTriggerFail(IPlayerCharacterData playerCharacter, int simulateSeed, byte triggerIndex, ActionTriggerFailReasons reason) { }
        public void LogAttackInterrupt(IPlayerCharacterData playerCharacter, int simulateSeed) { }
        public void LogAttackEnd(IPlayerCharacterData playerCharacter, int simulateSeed) { }

        public void LogUseSkillStart(IPlayerCharacterData playerCharacter, int simulateSeed, float[] triggerDurations, byte fireSpread, bool isLeftHand, CharacterItem weapon, BaseSkill skill, int skillLevel) { }
        public void LogUseSkillTrigger(IPlayerCharacterData playerCharacter, int simulateSeed, byte triggerIndex) { }
        public void LogUseSkillTriggerFail(IPlayerCharacterData playerCharacter, int simulateSeed, byte triggerIndex, ActionTriggerFailReasons reason) { }
        public void LogUseSkillInterrupt(IPlayerCharacterData playerCharacter, int simulateSeed) { }
        public void LogUseSkillEnd(IPlayerCharacterData playerCharacter, int simulateSeed) { }

        public void LogReloadStart(IPlayerCharacterData playerCharacter, float[] triggerDurations) { }
        public void LogReloadTrigger(IPlayerCharacterData playerCharacter, byte triggerIndex) { }
        public void LogReloadTriggerFail(IPlayerCharacterData playerCharacter, byte triggerIndex, ActionTriggerFailReasons reason) { }
        public void LogReloadInterrupt(IPlayerCharacterData playerCharacter) { }
        public void LogReloadEnd(IPlayerCharacterData playerCharacter) { }

        public void LogChargeStart(IPlayerCharacterData playerCharacter) { }
        public void LogChargeEnd(IPlayerCharacterData playerCharacter, bool willDoActionWhenStopCharging) { }

        public void LogBuffApply(IPlayerCharacterData playerCharacter, CharacterBuff characterBuff) { }
        public void LogBuffRemove(IPlayerCharacterData playerCharacter, CharacterBuff characterBuff, BuffRemoveReasons reason) { }

        public void LogDamageReceived(IPlayerCharacterData playerCharacter, HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CombatAmountType combatAmountType, int totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime) { }
        public void LogKilled(IPlayerCharacterData playerCharacter, EntityInfo lastAttacker) { }

        public void LogCraftItem(IPlayerCharacterData playerCharacter, ItemCraft itemCraft) { }
        public void LogDismantleItems(IPlayerCharacterData playerCharacter, IList<ItemAmount> dismantleItems) { }
        public void LogRefine(IPlayerCharacterData playerCharacter, CharacterItem refinedItem, IList<BaseItem> enhancerItems, float increaseSuccessRate, float decreaseRequireGoldRate, float chanceToNotDecreaseLevels, float chanceToNotDestroyItem, bool isSuccess, bool isDestroy, ItemRefineLevel itemRefineLevel, bool isReturning, ItemRefineFailReturning itemRefineFailReturning) { }
        public void LogRepair(IPlayerCharacterData playerCharacter, CharacterItem repairedItem, ItemRepairPrice itemRepairPrice) { }
        public void LogEnhanceSocketItem(IPlayerCharacterData playerCharacter, CharacterItem enhancedItem, BaseItem enhancerItem) { }
        public void LogRemoveEnhancerFromItem(IPlayerCharacterData playerCharacter, CharacterItem enhancedItem, BaseItem returnEnhancerItem) { }
        public void LogItemDurabilityChanged(IPlayerCharacterData playerCharacter, CharacterItem item, float oldDurability, float newDurability, bool destroyed) { }

        public void LogBuyVendingItem(IPlayerCharacterData playerCharacter, IPlayerCharacterData sellerCharacter, CharacterItem buyItem, int price) { }
        public void LogSellVendingItem(IPlayerCharacterData playerCharacter, IPlayerCharacterData buyerCharacter, CharacterItem buyItem, int price) { }

        public void LogExchangeDealingItemsAndGold(IPlayerCharacterData playerCharacter, IPlayerCharacterData dealingCharacter, int dealingGold, IList<CharacterItem> dealingItems) { }

        public void LogRewardItem(IPlayerCharacterData character, RewardGivenType givenType, BaseItem item, int amount) { }
        public void LogRewardItem(IPlayerCharacterData character, RewardGivenType givenType, CharacterItem item) { }
        public void LogRewardGold(IPlayerCharacterData character, RewardGivenType givenType, int gold) { }
        public void LogRewardExp(IPlayerCharacterData character, RewardGivenType givenType, int exp, bool isLevelUp) { }
        public void LogRewardCurrency(IPlayerCharacterData character, RewardGivenType givenType, Currency currency, int amount) { }
        public void LogRewardCurrency(IPlayerCharacterData character, RewardGivenType givenType, CharacterCurrency currency) { }
        public void LogAddAttribute(IPlayerCharacterData character, Attribute which, int howMuch, CharacterAttribute result) { }
        public void LogResetAttributes(IPlayerCharacterData character) { }
        public void LogAddSkill(IPlayerCharacterData character, BaseSkill which, int howMuch, CharacterSkill result) { }
        public void LogResetSkills(IPlayerCharacterData character) { }

        public void LogBuyNpcItem(IPlayerCharacterData character, NpcSellItem npcSellItem, int amount) { }
        public void LogSellNpcItem(IPlayerCharacterData character, CharacterItem characterItem, int amount) { }

        public void LogQuestAccept(IPlayerCharacterData character, Quest quest) { }
        public void LogQuestAbandon(IPlayerCharacterData character, Quest quest) { }
        public void LogQuestComplete(IPlayerCharacterData character, Quest quest, byte selectedRewardIndex) { }

        public void LogMoveItemFromStorage(IPlayerCharacterData character, StorageId storageId, bool storageIsLimitWeight, float storageWeightLimit, bool storageIsLimitSlot, int storageSlotLimit, IList<CharacterItem> storageItems, int storageItemIndex, int storageItemAmount, InventoryType inventoryType, int inventoryItemIndex, byte equipSlotIndexOrWeaponSet, bool success, UITextKeys gameMessage) { }
        public void LogMoveItemToStorage(IPlayerCharacterData character, StorageId storageId, bool storageIsLimitWeight, float storageWeightLimit, bool storageIsLimitSlot, int storageSlotLimit, IList<CharacterItem> storageItems, int storageItemIndex, InventoryType inventoryType, int inventoryItemIndex, int inventoryItemAmount, byte equipSlotIndexOrWeaponSet, bool success, UITextKeys gameMessage) { }
        public void LogSwapOrMergeStorageItem(IPlayerCharacterData character, StorageId storageId, bool storageIsLimitSlot, int storageSlotLimit, IList<CharacterItem> storageItems, int fromIndex, int toIndex, bool success, UITextKeys gameMessage) { }
    }
}
