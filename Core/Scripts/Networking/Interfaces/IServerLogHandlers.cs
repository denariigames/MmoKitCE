using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial interface IServerLogHandlers
    {
        void LogEnterGame(IPlayerCharacterData playerCharacter);
        void LogExitGame(string characterId, string userId);

        void LogEnterChat(ChatMessage chatMessage);

        void LogAttackStart(IPlayerCharacterData playerCharacter, int simulateSeed, float[] triggerDurations, byte fireSpread, bool isLeftHand, CharacterItem weapon);
        void LogAttackTrigger(IPlayerCharacterData playerCharacter, int simulateSeed, byte triggerIndex);
        void LogAttackTriggerFail(IPlayerCharacterData playerCharacter, int simulateSeed, byte triggerIndex, ActionTriggerFailReasons reason);
        void LogAttackInterrupt(IPlayerCharacterData playerCharacter, int simulateSeed);
        void LogAttackEnd(IPlayerCharacterData playerCharacter, int simulateSeed);

        void LogUseSkillStart(IPlayerCharacterData playerCharacter, int simulateSeed, float[] triggerDurations, byte fireSpread, bool isLeftHand, CharacterItem weapon, BaseSkill skill, int skillLevel);
        void LogUseSkillTrigger(IPlayerCharacterData playerCharacter, int simulateSeed, byte triggerIndex);
        void LogUseSkillTriggerFail(IPlayerCharacterData playerCharacter, int simulateSeed, byte triggerIndex, ActionTriggerFailReasons reason);
        void LogUseSkillInterrupt(IPlayerCharacterData playerCharacter, int simulateSeed);
        void LogUseSkillEnd(IPlayerCharacterData playerCharacter, int simulateSeed);

        void LogReloadStart(IPlayerCharacterData playerCharacter, float[] triggerDurations);
        void LogReloadTrigger(IPlayerCharacterData playerCharacter, byte triggerIndex);
        void LogReloadTriggerFail(IPlayerCharacterData playerCharacter, byte triggerIndex, ActionTriggerFailReasons reason);
        void LogReloadInterrupt(IPlayerCharacterData playerCharacter);
        void LogReloadEnd(IPlayerCharacterData playerCharacter);

        void LogChargeStart(IPlayerCharacterData playerCharacter);
        void LogChargeEnd(IPlayerCharacterData playerCharacter, bool willDoActionWhenStopCharging);

        void LogBuffApply(IPlayerCharacterData playerCharacter, CharacterBuff characterBuff);
        void LogBuffRemove(IPlayerCharacterData playerCharacter, CharacterBuff characterBuff, BuffRemoveReasons reason);

        void LogDamageReceived(IPlayerCharacterData playerCharacter, HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CombatAmountType combatAmountType, int totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime);
        void LogKilled(IPlayerCharacterData playerCharacter, EntityInfo lastAttacker);

        void LogCraftItem(IPlayerCharacterData playerCharacter, ItemCraft itemCraft);
        void LogDismantleItems(IPlayerCharacterData playerCharacter, IList<ItemAmount> dismantleItems);
        void LogRefine(IPlayerCharacterData playerCharacter, CharacterItem refinedItem, IList<BaseItem> enhancerItems, float increaseSuccessRate, float decreaseRequireGoldRate, float chanceToNotDecreaseLevels, float chanceToNotDestroyItem, bool isSuccess, bool isDestroy, ItemRefineLevel itemRefineLevel, bool isReturning, ItemRefineFailReturning itemRefineFailReturning);
        void LogRepair(IPlayerCharacterData playerCharacter, CharacterItem repairedItem, ItemRepairPrice itemRepairPrice);
        void LogEnhanceSocketItem(IPlayerCharacterData playerCharacter, CharacterItem enhancedItem, BaseItem enhancerItem);
        void LogRemoveEnhancerFromItem(IPlayerCharacterData playerCharacter, CharacterItem enhancedItem, BaseItem returnEnhancerItem);
        void LogItemDurabilityChanged(IPlayerCharacterData playerCharacter, CharacterItem item, float oldDurability, float newDurability, bool destroyed);

        void LogBuyVendingItem(IPlayerCharacterData playerCharacter, IPlayerCharacterData sellerCharacter, CharacterItem buyItem, int price);
        void LogSellVendingItem(IPlayerCharacterData playerCharacter, IPlayerCharacterData buyerCharacter, CharacterItem buyItem, int price);

        void LogExchangeDealingItemsAndGold(IPlayerCharacterData playerCharacter, IPlayerCharacterData dealingCharacter, int dealingGold, IList<CharacterItem> dealingItems);

        void LogRewardItem(IPlayerCharacterData character, RewardGivenType givenType, BaseItem item, int amount);
        void LogRewardItem(IPlayerCharacterData character, RewardGivenType givenType, CharacterItem item);
        void LogRewardGold(IPlayerCharacterData character, RewardGivenType givenType, int gold);
        void LogRewardExp(IPlayerCharacterData character, RewardGivenType givenType, int exp, bool isLevelUp);
        void LogRewardCurrency(IPlayerCharacterData character, RewardGivenType givenType, Currency currency, int amount);
        void LogRewardCurrency(IPlayerCharacterData character, RewardGivenType givenType, CharacterCurrency currency);
        void LogAddAttribute(IPlayerCharacterData character, Attribute which, int howMuch, CharacterAttribute result);
        void LogResetAttributes(IPlayerCharacterData character);
        void LogAddSkill(IPlayerCharacterData character, BaseSkill which, int howMuch, CharacterSkill result);
        void LogResetSkills(IPlayerCharacterData character);

        void LogBuyNpcItem(IPlayerCharacterData character, NpcSellItem npcSellItem, int amount);
        void LogSellNpcItem(IPlayerCharacterData character, CharacterItem characterItem, int amount);

        void LogQuestAccept(IPlayerCharacterData character, Quest quest);
        void LogQuestAbandon(IPlayerCharacterData character, Quest quest);
        void LogQuestComplete(IPlayerCharacterData character, Quest quest, byte selectedRewardIndex);

        void LogMoveItemFromStorage(IPlayerCharacterData character, StorageId storageId, bool storageIsLimitWeight, float storageWeightLimit, bool storageIsLimitSlot, int storageSlotLimit, IList<CharacterItem> storageItems, int storageItemIndex, int storageItemAmount, InventoryType inventoryType, int inventoryItemIndex, byte equipSlotIndexOrWeaponSet, bool success, UITextKeys gameMessage);
        void LogMoveItemToStorage(IPlayerCharacterData character, StorageId storageId, bool storageIsLimitWeight, float storageWeightLimit, bool storageIsLimitSlot, int storageSlotLimit, IList<CharacterItem> storageItems, int storageItemIndex, InventoryType inventoryType, int inventoryItemIndex, int inventoryItemAmount, byte equipSlotIndexOrWeaponSet, bool success, UITextKeys gameMessage);
        void LogSwapOrMergeStorageItem(IPlayerCharacterData character, StorageId storageId, bool storageIsLimitSlot, int storageSlotLimit, IList<CharacterItem> storageItems, int fromIndex, int toIndex, bool success, UITextKeys gameMessage);
    }
}