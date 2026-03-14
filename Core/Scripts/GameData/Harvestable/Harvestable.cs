using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.HARVESTABLE_FILE, menuName = GameDataMenuConsts.HARVESTABLE_MENU, order = GameDataMenuConsts.HARVESTABLE_ORDER)]
    public partial class Harvestable : BaseGameData
    {
        [Category("Harvestable Settings")]
        public HarvestEffectiveness[] harvestEffectivenesses = new HarvestEffectiveness[0];
        public SkillHarvestEffectiveness[] skillHarvestEffectivenesses = new SkillHarvestEffectiveness[0];
        [Tooltip("Ex. if this is 10 when damage to harvestable entity = 2, character will receives 20 exp")]
        public int expPerDamage = 0;

        [System.NonSerialized]
        private Dictionary<WeaponType, HarvestEffectiveness> _cacheHarvestEffectivenesses;
        public Dictionary<WeaponType, HarvestEffectiveness> CacheHarvestEffectivenesses
        {
            get
            {
                InitCaches();
                return _cacheHarvestEffectivenesses;
            }
        }

        [System.NonSerialized]
        private Dictionary<WeaponType, WeightedRandomizer<ItemDropForHarvestable>> _cacheHarvestItems;
        public Dictionary<WeaponType, WeightedRandomizer<ItemDropForHarvestable>> CacheHarvestItems
        {
            get
            {
                InitCaches();
                return _cacheHarvestItems;
            }
        }

        [System.NonSerialized]
        private Dictionary<BaseSkill, SkillHarvestEffectiveness> _cacheSkillHarvestEffectivenesses;
        public Dictionary<BaseSkill, SkillHarvestEffectiveness> CacheSkillHarvestEffectivenesses
        {
            get
            {
                InitCaches();
                return _cacheSkillHarvestEffectivenesses;
            }
        }

        [System.NonSerialized]
        private Dictionary<BaseSkill, WeightedRandomizer<ItemDropForHarvestable>> _cacheSkillHarvestItems;
        public Dictionary<BaseSkill, WeightedRandomizer<ItemDropForHarvestable>> CacheSkillHarvestItems
        {
            get
            {
                InitCaches();
                return _cacheSkillHarvestItems;
            }
        }

        private void InitCaches()
        {
            if (_cacheHarvestEffectivenesses == null || _cacheHarvestItems == null)
            {
                _cacheHarvestEffectivenesses = new Dictionary<WeaponType, HarvestEffectiveness>();
                _cacheHarvestItems = new Dictionary<WeaponType, WeightedRandomizer<ItemDropForHarvestable>>();
                foreach (HarvestEffectiveness harvestEffectiveness in harvestEffectivenesses)
                {
                    if (harvestEffectiveness.weaponType != null && harvestEffectiveness.damageEffectiveness > 0)
                    {
                        _cacheHarvestEffectivenesses[harvestEffectiveness.weaponType] = harvestEffectiveness;
                        List<WeightedRandomizerItem<ItemDropForHarvestable>> harvestItems = new List<WeightedRandomizerItem<ItemDropForHarvestable>>();
                        foreach (ItemDropForHarvestable item in harvestEffectiveness.items)
                        {
                            if (item.item == null || item.amountPerDamage <= 0 || item.randomWeight <= 0)
                                continue;
                            harvestItems.Add(new WeightedRandomizerItem<ItemDropForHarvestable>()
                            {
                                item = item,
                                weight = item.randomWeight,
                            });
                        }
                        _cacheHarvestItems[harvestEffectiveness.weaponType] = WeightedRandomizer.From(harvestItems);
                    }
                }
            }
            if (_cacheSkillHarvestEffectivenesses == null || _cacheSkillHarvestItems == null)
            {
                _cacheSkillHarvestEffectivenesses = new Dictionary<BaseSkill, SkillHarvestEffectiveness>();
                _cacheSkillHarvestItems = new Dictionary<BaseSkill, WeightedRandomizer<ItemDropForHarvestable>>();
                foreach (SkillHarvestEffectiveness skillHarvestEffectiveness in skillHarvestEffectivenesses)
                {
                    if (skillHarvestEffectiveness.skill != null && skillHarvestEffectiveness.damageEffectiveness > 0)
                    {
                        _cacheSkillHarvestEffectivenesses[skillHarvestEffectiveness.skill] = skillHarvestEffectiveness;
                        List<WeightedRandomizerItem<ItemDropForHarvestable>> harvestItems = new List<WeightedRandomizerItem<ItemDropForHarvestable>>();
                        foreach (ItemDropForHarvestable item in skillHarvestEffectiveness.items)
                        {
                            if (item.item == null || item.amountPerDamage <= 0 || item.randomWeight <= 0)
                                continue;
                            harvestItems.Add(new WeightedRandomizerItem<ItemDropForHarvestable>()
                            {
                                item = item,
                                weight = item.randomWeight,
                            });
                        }
                        _cacheSkillHarvestItems[skillHarvestEffectiveness.skill] = WeightedRandomizer.From(harvestItems);
                    }
                }
            }
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (harvestEffectivenesses != null && harvestEffectivenesses.Length > 0)
            {
                foreach (HarvestEffectiveness harvestEffectiveness in harvestEffectivenesses)
                {
                    GameInstance.AddWeaponTypes(harvestEffectiveness.weaponType);
                    GameInstance.AddItems(harvestEffectiveness.items);
                }
            }
            if (skillHarvestEffectivenesses != null && skillHarvestEffectivenesses.Length > 0)
            {
                foreach (SkillHarvestEffectiveness skillHarvestEffectiveness in skillHarvestEffectivenesses)
                {
                    GameInstance.AddSkills(skillHarvestEffectiveness.skill);
                    GameInstance.AddItems(skillHarvestEffectiveness.items);
                }
            }
        }
    }
}
