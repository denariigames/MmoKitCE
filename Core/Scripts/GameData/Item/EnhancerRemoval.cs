using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class EnhancerRemoval
    {
        [SerializeField]
        private bool returnEnhancerItem = false;
        public bool ReturnEnhancerItem { get { return returnEnhancerItem; } }

        [SerializeField]
        [ArrayElementTitle("item")]
        private ItemAmount[] requireItems = new ItemAmount[0];
        public ItemAmount[] RequireItems { get { return requireItems; } }

        [SerializeField]
        [ArrayElementTitle("currency")]
        private CurrencyAmount[] requireCurrencies = new CurrencyAmount[0];
        public CurrencyAmount[] RequireCurrencies { get { return requireCurrencies; } }

        [SerializeField]
        private int requireGold = 0;
        public int RequireGold { get { return requireGold; } }

        public bool CanRemove(IPlayerCharacterData character)
        {
            return CanRemove(character, out _);
        }

        public bool CanRemove(IPlayerCharacterData character, out UITextKeys gameMessage)
        {
            gameMessage = UITextKeys.NONE;
            if (!GameInstance.Singleton.GameplayRule.CurrenciesEnoughToRemoveEnhancer(character))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                return false;
            }
            if (requireItems == null || requireItems.Length == 0)
                return true;
            // Count required items
            foreach (ItemAmount requireItem in requireItems)
            {
                if (requireItem.item != null && character.CountNonEquipItems(requireItem.item.DataId) < requireItem.amount)
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                    return false;
                }
            }
            return true;
        }
    }
}
