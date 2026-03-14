using System.Collections.Generic;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    public partial class CalculatedItemBuff
    {
        private IEquipmentItem _item;
        private int _level;
        private int _randomSeed;
        private byte _version;
        private CharacterStats _cacheIncreaseStats = new CharacterStats();
        private CharacterStats _cacheIncreaseStatsRate = new CharacterStats();
        private readonly Dictionary<Attribute, float> _cacheIncreaseAttributes;
        private readonly Dictionary<Attribute, float> _cacheIncreaseAttributesRate;
        private readonly Dictionary<DamageElement, float> _cacheIncreaseResistances;
        private readonly Dictionary<DamageElement, float> _cacheIncreaseArmors;
        private readonly Dictionary<DamageElement, float> _cacheIncreaseArmorsRate;
        private readonly Dictionary<DamageElement, MinMaxFloat> _cacheIncreaseDamages;
        private readonly Dictionary<DamageElement, MinMaxFloat> _cacheIncreaseDamagesRate;
        private readonly Dictionary<BaseSkill, int> _cacheIncreaseSkills;
        private readonly Dictionary<StatusEffect, float> _cacheIncreaseStatusEffectResistances;
        private CalculatedItemRandomBonus _cacheRandomBonus = new CalculatedItemRandomBonus();

        public CalculatedItemBuff()
        {
            _cacheIncreaseAttributes = CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Get();
            _cacheIncreaseAttributesRate = CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Get();
            _cacheIncreaseResistances = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseArmors = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseArmorsRate = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseDamages = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
            _cacheIncreaseDamagesRate = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
            _cacheIncreaseSkills = CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Get();
            _cacheIncreaseStatusEffectResistances = CollectionPool<Dictionary<StatusEffect, float>, KeyValuePair<StatusEffect, float>>.Get();
        }

        public CalculatedItemBuff(IEquipmentItem item, int level, int randomSeed, byte version)
        {
            _cacheIncreaseAttributes = CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Get();
            _cacheIncreaseAttributesRate = CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Get();
            _cacheIncreaseResistances = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseArmors = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseArmorsRate = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseDamages = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
            _cacheIncreaseDamagesRate = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
            _cacheIncreaseSkills = CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Get();
            _cacheIncreaseStatusEffectResistances = CollectionPool<Dictionary<StatusEffect, float>, KeyValuePair<StatusEffect, float>>.Get();
            Build(item, level, randomSeed, version);
        }

        ~CalculatedItemBuff()
        {
            CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Release(_cacheIncreaseAttributes);
            CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Release(_cacheIncreaseAttributesRate);
            CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(_cacheIncreaseResistances);
            CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(_cacheIncreaseArmors);
            CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(_cacheIncreaseArmorsRate);
            CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(_cacheIncreaseDamages);
            CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(_cacheIncreaseDamagesRate);
            CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Release(_cacheIncreaseSkills);
            CollectionPool<Dictionary<StatusEffect, float>, KeyValuePair<StatusEffect, float>>.Release(_cacheIncreaseStatusEffectResistances);
            _cacheRandomBonus = null;
        }

        public void Clear()
        {
            _cacheIncreaseStats = new CharacterStats();
            _cacheIncreaseStatsRate = new CharacterStats();
            _cacheIncreaseAttributes.Clear();
            _cacheIncreaseAttributesRate.Clear();
            _cacheIncreaseResistances.Clear();
            _cacheIncreaseArmors.Clear();
            _cacheIncreaseArmorsRate.Clear();
            _cacheIncreaseDamages.Clear();
            _cacheIncreaseDamagesRate.Clear();
            _cacheIncreaseSkills.Clear();
            _cacheIncreaseStatusEffectResistances.Clear();
        }

        public void Build(IEquipmentItem item, int level, int randomSeed, byte version)
        {
            // Don't rebuild if it has no difference
            if (_item != null && _item.DataId == item.DataId && _level == level && _randomSeed == randomSeed && _version == version)
                return;

            _item = item;
            _level = level;
            _randomSeed = randomSeed;
            _version = version;

            Clear();

            if (item == null || !item.IsEquipment())
                return;

            _cacheRandomBonus.Build(item, level, randomSeed, version);

            _cacheIncreaseStats = item.GetIncreaseStats(_level) + _cacheRandomBonus.GetIncreaseStats();
            _cacheIncreaseStatsRate = item.GetIncreaseStatsRate(_level) + _cacheRandomBonus.GetIncreaseStatsRate();
            item.GetIncreaseAttributes(_level, _cacheIncreaseAttributes);
            GameDataHelpers.CombineAttributes(_cacheIncreaseAttributes, _cacheRandomBonus.GetIncreaseAttributes());
            item.GetIncreaseAttributesRate(_level, _cacheIncreaseAttributesRate);
            GameDataHelpers.CombineAttributes(_cacheIncreaseAttributesRate, _cacheRandomBonus.GetIncreaseAttributesRate());
            item.GetIncreaseResistances(_level, _cacheIncreaseResistances);
            GameDataHelpers.CombineResistances(_cacheIncreaseResistances, _cacheRandomBonus.GetIncreaseResistances());
            item.GetIncreaseArmors(_level, _cacheIncreaseArmors);
            GameDataHelpers.CombineArmors(_cacheIncreaseArmors, _cacheRandomBonus.GetIncreaseArmors());
            item.GetIncreaseArmorsRate(_level, _cacheIncreaseArmorsRate);
            GameDataHelpers.CombineArmors(_cacheIncreaseArmorsRate, _cacheRandomBonus.GetIncreaseArmorsRate());
            item.GetIncreaseDamages(_level, _cacheIncreaseDamages);
            GameDataHelpers.CombineDamages(_cacheIncreaseDamages, _cacheRandomBonus.GetIncreaseDamages());
            item.GetIncreaseDamagesRate(_level, _cacheIncreaseDamagesRate);
            GameDataHelpers.CombineDamages(_cacheIncreaseDamagesRate, _cacheRandomBonus.GetIncreaseDamagesRate());
            item.GetIncreaseSkills(_level, _cacheIncreaseSkills);
            GameDataHelpers.CombineSkills(_cacheIncreaseSkills, _cacheRandomBonus.GetIncreaseSkills());
            // TODO: Implement random bonus for increase status effect resistances
            item.GetIncreaseStatusEffectResistances(_level, _cacheIncreaseStatusEffectResistances);

            if (GameExtensionInstance.onBuildCalculatedItemBuff != null)
                GameExtensionInstance.onBuildCalculatedItemBuff(this);
        }

        public IEquipmentItem GetItem()
        {
            return _item;
        }

        public int GetLevel()
        {
            return _level;
        }

        public int GetRandomSeed()
        {
            return _randomSeed;
        }

        public CharacterStats GetIncreaseStats()
        {
            return _cacheIncreaseStats;
        }

        public CharacterStats GetIncreaseStatsRate()
        {
            return _cacheIncreaseStatsRate;
        }

        public Dictionary<Attribute, float> GetIncreaseAttributes()
        {
            return _cacheIncreaseAttributes;
        }

        public Dictionary<Attribute, float> GetIncreaseAttributesRate()
        {
            return _cacheIncreaseAttributesRate;
        }

        public Dictionary<DamageElement, float> GetIncreaseResistances()
        {
            return _cacheIncreaseResistances;
        }

        public Dictionary<DamageElement, float> GetIncreaseArmors()
        {
            return _cacheIncreaseArmors;
        }

        public Dictionary<DamageElement, float> GetIncreaseArmorsRate()
        {
            return _cacheIncreaseArmorsRate;
        }

        public Dictionary<DamageElement, MinMaxFloat> GetIncreaseDamages()
        {
            return _cacheIncreaseDamages;
        }

        public Dictionary<DamageElement, MinMaxFloat> GetIncreaseDamagesRate()
        {
            return _cacheIncreaseDamagesRate;
        }

        public Dictionary<BaseSkill, int> GetIncreaseSkills()
        {
            return _cacheIncreaseSkills;
        }

        public Dictionary<StatusEffect, float> GetIncreaseStatusEffectResistances()
        {
            return _cacheIncreaseStatusEffectResistances;
        }
    }
}
