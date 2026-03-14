using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.SPECIFIC_REWARDING_ITEM_FILE, menuName = GameDataMenuConsts.SPECIFIC_REWARDING_ITEM_MENU, order = GameDataMenuConsts.SPECIFIC_REWARDING_ITEM_ORDER)]
    public class SpecificRewardingItem : BaseItem, IUsableItem
    {
        [Category(2, "Requirements")]
        [SerializeField]
        private ItemRequirement requirement = new ItemRequirement();
        public ItemRequirement Requirement
        {
            get { return requirement; }
        }

        [SerializeField]
        private ItemAmount[] rewardingItems = new ItemAmount[0];

        [System.NonSerialized]
        private Dictionary<Attribute, float> _cacheRequireAttributeAmounts = null;
        public Dictionary<Attribute, float> RequireAttributeAmounts
        {
            get
            {
                if (_cacheRequireAttributeAmounts == null)
                {
                    _cacheRequireAttributeAmounts = new Dictionary<Attribute, float>();
                    GameDataHelpers.CombineAttributes(requirement.attributeAmounts, _cacheRequireAttributeAmounts, 1f);
                }
                return _cacheRequireAttributeAmounts;
            }
        }

        [SerializeField]
        private float useItemCooldown = 0f;
        public float UseItemCooldown
        {
            get { return useItemCooldown; }
        }
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_CONSUMABLE.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Potion; }
        }

        public bool HasCustomAimControls()
        {
            return false;
        }

        public AimPosition UpdateAimControls(Vector2 aimAxes, params object[] data)
        {
            return default;
        }

        public void FinishAimControls(bool isCancel)
        {

        }

        public bool IsChanneledAbility()
        {
            return false;
        }

        public bool UseItem(BaseCharacterEntity characterEntity, int itemIndex, CharacterItem characterItem)
        {
            BasePlayerCharacterEntity playerCharacterEntity = characterEntity as BasePlayerCharacterEntity;
            if (playerCharacterEntity == null)
                return false;
            if (!characterEntity.CanUseItem())
                return false;
            if (characterEntity.IncreasingItemsWillOverwhelming(rewardingItems))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(playerCharacterEntity.ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
                return false;
            }
            if (!characterEntity.DecreaseItemsByIndex(itemIndex, 1, false))
                return false;
            playerCharacterEntity.IncreaseItems(rewardingItems);
            return true;
        }
    }
}
