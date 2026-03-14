using Insthync.UnityEditorUtils;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class ItemCraft
    {
        [SerializeField]
        private BaseItem craftingItem;
        public BaseItem CraftingItem { get { return craftingItem; } }

        [SerializeField]
        private int amount = 1;
        public int Amount { get { return (amount > 0 ? amount : 1); } }

        [SerializeField]
        private int requireGold = 0;
        public int RequireGold { get { return requireGold; } }

        [SerializeField]
        [FormerlySerializedAs("craftRequirements")]
        [ArrayElementTitle("item")]
        private ItemAmount[] requireItems = new ItemAmount[0];
        public ItemAmount[] RequireItems { get { return requireItems; } }

        [SerializeField]
        [ArrayElementTitle("currency")]
        private CurrencyAmount[] requireCurrencies = new CurrencyAmount[0];
        public CurrencyAmount[] RequireCurrencies { get { return requireCurrencies; } }

        public bool CanCraft(IPlayerCharacterData character)
        {
            return CanCraft(character, out _);
        }

        public bool CanCraft(IPlayerCharacterData character, out UITextKeys gameMessage)
        {
            gameMessage = UITextKeys.NONE;
            if (craftingItem == null)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ITEM_DATA;
                return false;
            }
            if (!GameInstance.Singleton.GameplayRule.CurrenciesEnoughToCraftItem(character, this))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                return false;
            }
            if (character.IncreasingItemsWillOverwhelming(craftingItem.DataId, Amount))
            {
                gameMessage = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                return false;
            }
            if (requireItems == null || requireItems.Length == 0)
            {
                // No required items
                return true;
            }
            foreach (ItemAmount craftRequirement in requireItems)
            {
                if (craftRequirement.item != null && character.CountNonEquipItems(craftRequirement.item.DataId) < craftRequirement.amount)
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                    return false;
                }
            }
            return true;
        }

        public void CraftItem(IPlayerCharacterData character)
        {
            if (character.IncreaseItems(CharacterItem.Create(craftingItem, 1, Amount), characterItem =>
            {
                if (character is BasePlayerCharacterEntity entity)
                    entity.OnRewardItem(RewardGivenType.Crafting, characterItem);
            }))
            {
                // Reduce item when able to increase craft item
                foreach (ItemAmount craftRequirement in requireItems)
                {
                    if (craftRequirement.item != null && craftRequirement.amount > 0)
                        character.DecreaseItems(craftRequirement.item.DataId, craftRequirement.amount);
                }
                character.FillEmptySlots();
                // Decrease required gold
                GameInstance.Singleton.GameplayRule.DecreaseCurrenciesWhenCraftItem(character, this);
                GameInstance.ServerLogHandlers.LogCraftItem(character, this);
            }
        }
    }
}
