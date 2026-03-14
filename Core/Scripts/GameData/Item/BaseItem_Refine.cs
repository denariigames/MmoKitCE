namespace MultiplayerARPG
{
    public partial class BaseItem
    {
        public bool TryGetItemRefineLevel(int level, out ItemRefineLevel refineLevel)
        {
            return TryGetItemRefineLevel(level, out refineLevel, out _);
        }

        public bool TryGetItemRefineLevel(int level, out ItemRefineLevel refineLevel, out UITextKeys gameMessage)
        {
            refineLevel = null;
            gameMessage = UITextKeys.NONE;
            if (ItemRefine == null)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_DATA;
                return false;
            }
            if (level - 1 >= ItemRefine.Levels.Length)
            {
                gameMessage = UITextKeys.UI_ERROR_REFINE_ITEM_REACHED_MAX_LEVEL;
                return false;
            }
            refineLevel = ItemRefine.Levels[level - 1];
            return true;
        }

        public bool CanRefine(IPlayerCharacterData character, int level, int[] enhancerDataIds)
        {
            return CanRefine(character, level, enhancerDataIds, out _);
        }

        public bool CanRefine(IPlayerCharacterData character, int level, int[] enhancerDataIds, out UITextKeys gameMessage)
        {
            if (!this.IsEquipment())
            {
                gameMessage = UITextKeys.UI_ERROR_ITEM_NOT_EQUIPMENT;
                return false;
            }
            if (GameInstance.Singleton.refineEnhancerItemsLimit > 0 && enhancerDataIds.Length > GameInstance.Singleton.refineEnhancerItemsLimit)
            {
                gameMessage = UITextKeys.UI_ERROR_REACHED_REFINE_ENHANCER_ITEMS_LIMIT;
                return false;
            }
            if (!TryGetItemRefineLevel(level, out ItemRefineLevel refineLevel, out gameMessage))
            {
                return false;
            }
            return refineLevel.CanRefine(character, enhancerDataIds, out gameMessage);
        }
    }
}
