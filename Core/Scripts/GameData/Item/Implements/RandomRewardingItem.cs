using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.RANDOM_REWARDING_ITEM_FILE, menuName = GameDataMenuConsts.RANDOM_REWARDING_ITEM_MENU, order = GameDataMenuConsts.RANDOM_REWARDING_ITEM_ORDER)]
    public class RandomRewardingItem : BaseItem, IUsableItem
    {
        [Category(2, "Requirements")]
        [SerializeField]
        private ItemRequirement requirement = new ItemRequirement();
        public ItemRequirement Requirement
        {
            get { return requirement; }
        }

        [SerializeField]
        private ItemRandomByWeightTable rewardingItemsTable;

        [SerializeField]
        private int maxDropAmount;

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
            if (rewardingItemsTable == null)
                return false;
            int seed = characterItem.id.GenerateHashId();
            int rewardingExp = 0;
            int rewardingGold = 0;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            List<CurrencyAmount> rewardingCurrencies = new List<CurrencyAmount>();
#endif
            List<ItemAmount> rewardingItems = new List<ItemAmount>();
            for (int i = 0; i < maxDropAmount; ++i)
            {
                unchecked
                {
                    seed += i * 512;
                }
                rewardingItemsTable.RandomItem((item, level, amount) =>
                {
                    if (GameInstance.Singleton.IsExpDropRepresentItem(item))
                    {
                        rewardingExp += amount;
                    }
                    else if (GameInstance.Singleton.IsGoldDropRepresentItem(item))
                    {
                        rewardingGold += amount;
                    }
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
                    else if (GameInstance.Singleton.IsCurrencyDropRepresentItem(item, out Currency currency))
                    {
                        rewardingCurrencies.Add(new CurrencyAmount()
                        {
                            currency = currency,
                            amount = amount,
                        });
                    }
#endif
                    else
                    {
                        rewardingItems.Add(new ItemAmount()
                        {
                            item = item,
                            level = level,
                            amount = amount,
                        });
                    }
                }, seed);
            }
            if (characterEntity.IncreasingItemsWillOverwhelming(rewardingItems))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(characterEntity.ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
                return false;
            }
            if (!characterEntity.DecreaseItemsByIndex(itemIndex, 1, false))
                return false;

            characterEntity.RewardExp(rewardingExp, 1f, RewardGivenType.None, 1, 1);
            characterEntity.RewardGold(rewardingGold, 1f, RewardGivenType.None, 1, 1);
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            characterEntity.RewardCurrencies(rewardingCurrencies, 1f, RewardGivenType.None, 1, 1);
#endif
            characterEntity.IncreaseItems(rewardingItems);
            characterEntity.FillEmptySlots();
            return true;
        }
    }
}
