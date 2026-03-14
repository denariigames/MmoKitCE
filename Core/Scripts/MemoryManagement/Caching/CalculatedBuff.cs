using System.Collections.Generic;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    public partial class CalculatedBuff
    {
        private Buff _buff;
        private int _level;
        private float _cacheDuration;
        private int _cacheRecoveryHp;
        private int _cacheRecoveryMp;
        private int _cacheRecoveryStamina;
        private int _cacheRecoveryFood;
        private int _cacheRecoveryWater;
        private CharacterStats _cacheIncreaseStats;
        private CharacterStats _cacheIncreaseStatsRate;
        private readonly Dictionary<Attribute, float> _cacheIncreaseAttributes;
        private readonly Dictionary<Attribute, float> _cacheIncreaseAttributesRate;
        private readonly Dictionary<DamageElement, float> _cacheIncreaseResistances;
        private readonly Dictionary<DamageElement, float> _cacheIncreaseArmors;
        private readonly Dictionary<DamageElement, float> _cacheIncreaseArmorsRate;
        private readonly Dictionary<DamageElement, MinMaxFloat> _cacheIncreaseDamages;
        private readonly Dictionary<DamageElement, MinMaxFloat> _cacheIncreaseDamagesRate;
        private readonly Dictionary<BaseSkill, int> _cacheIncreaseSkills;
        private readonly Dictionary<BaseSkill, int> _cacheOverrideSkills;
        private readonly Dictionary<StatusEffect, float> _cacheIncreaseStatusEffectResistances;
        private readonly Dictionary<BuffRemoval, float> _cacheBuffRemovals;
        private readonly Dictionary<DamageElement, MinMaxFloat> _cacheDamageOverTimes;
        private float _cacheRemoveBuffWhenAttackChance;
        private float _cacheRemoveBuffWhenAttackedChance;
        private float _cacheRemoveBuffWhenUseSkillChance;
        private float _cacheRemoveBuffWhenUseItemChance;
        private float _cacheRemoveBuffWhenPickupItemChance;
        private int _cacheMaxStack;
        private BuffMount _cacheMount;
        private int _cacheMountLevel;

        public CalculatedBuff()
        {
            _cacheIncreaseAttributes = CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Get();
            _cacheIncreaseAttributesRate = CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Get();
            _cacheIncreaseResistances = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseArmors = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseArmorsRate = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseDamages = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
            _cacheIncreaseDamagesRate = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
            _cacheIncreaseSkills = CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Get();
            _cacheOverrideSkills = CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Get();
            _cacheIncreaseStatusEffectResistances = CollectionPool<Dictionary<StatusEffect, float>, KeyValuePair<StatusEffect, float>>.Get();
            _cacheBuffRemovals = CollectionPool<Dictionary<BuffRemoval, float>, KeyValuePair<BuffRemoval, float>>.Get();
            _cacheDamageOverTimes = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
        }

        public CalculatedBuff(Buff buff, int level)
        {
            _cacheIncreaseAttributes = CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Get();
            _cacheIncreaseAttributesRate = CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Get();
            _cacheIncreaseResistances = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseArmors = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseArmorsRate = CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Get();
            _cacheIncreaseDamages = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
            _cacheIncreaseDamagesRate = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
            _cacheIncreaseSkills = CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Get();
            _cacheOverrideSkills = CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Get();
            _cacheIncreaseStatusEffectResistances = CollectionPool<Dictionary<StatusEffect, float>, KeyValuePair<StatusEffect, float>>.Get();
            _cacheBuffRemovals = CollectionPool<Dictionary<BuffRemoval, float>, KeyValuePair<BuffRemoval, float>>.Get();
            _cacheDamageOverTimes = CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get();
            Build(buff, level);
        }

        ~CalculatedBuff()
        {
            CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Release(_cacheIncreaseAttributes);
            CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Release(_cacheIncreaseAttributesRate);
            CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(_cacheIncreaseResistances);
            CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(_cacheIncreaseArmors);
            CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(_cacheIncreaseArmorsRate);
            CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(_cacheIncreaseDamages);
            CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(_cacheIncreaseDamagesRate);
            CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Release(_cacheIncreaseSkills);
            CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Release(_cacheOverrideSkills);
            CollectionPool<Dictionary<StatusEffect, float>, KeyValuePair<StatusEffect, float>>.Release(_cacheIncreaseStatusEffectResistances);
            CollectionPool<Dictionary<BuffRemoval, float>, KeyValuePair<BuffRemoval, float>>.Release(_cacheBuffRemovals);
            CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(_cacheDamageOverTimes);
        }

        public void Clear()
        {
            _cacheIncreaseAttributes?.Clear();
            _cacheIncreaseAttributesRate?.Clear();
            _cacheIncreaseResistances?.Clear();
            _cacheIncreaseArmors?.Clear();
            _cacheIncreaseArmorsRate?.Clear();
            _cacheIncreaseDamages?.Clear();
            _cacheIncreaseDamagesRate?.Clear();
            _cacheIncreaseSkills?.Clear();
            _cacheOverrideSkills?.Clear();
            _cacheIncreaseStatusEffectResistances?.Clear();
            _cacheBuffRemovals?.Clear();
            _cacheDamageOverTimes?.Clear();
            _cacheMount = null;
        }

        public void Build(Buff buff, int level)
        {
            _buff = buff;
            _level = level;

            Clear();

            if (buff != null)
            {
                _cacheDuration = buff.GetDuration(level);
                _cacheRecoveryHp = buff.GetRecoveryHp(level);
                _cacheRecoveryMp = buff.GetRecoveryMp(level);
                _cacheRecoveryStamina = buff.GetRecoveryStamina(level);
                _cacheRecoveryFood = buff.GetRecoveryFood(level);
                _cacheRecoveryWater = buff.GetRecoveryWater(level);
                _cacheIncreaseStats = buff.GetIncreaseStats(level);
                _cacheIncreaseStatsRate = buff.GetIncreaseStatsRate(level);
                buff.GetIncreaseAttributes(level, _cacheIncreaseAttributes);
                buff.GetIncreaseAttributesRate(level, _cacheIncreaseAttributesRate);
                buff.GetIncreaseResistances(level, _cacheIncreaseResistances);
                buff.GetIncreaseArmors(level, _cacheIncreaseArmors);
                buff.GetIncreaseArmorsRate(level, _cacheIncreaseArmorsRate);
                buff.GetIncreaseDamages(level, _cacheIncreaseDamages);
                buff.GetIncreaseDamagesRate(level, _cacheIncreaseDamagesRate);
                buff.GetIncreaseSkills(level, _cacheIncreaseSkills);
                if (buff.isOverrideSkills)
                    buff.GetOverrideSkills(level, _cacheOverrideSkills);
                buff.GetIncreaseStatusEffectResistances(level, _cacheIncreaseStatusEffectResistances);
                buff.GetBuffRemovals(level, _cacheBuffRemovals);
                buff.GetDamageOverTimes(level, _cacheDamageOverTimes);
                _cacheRemoveBuffWhenAttackChance = buff.GetRemoveBuffWhenAttackChance(level);
                _cacheRemoveBuffWhenAttackedChance = buff.GetRemoveBuffWhenAttackedChance(level);
                _cacheRemoveBuffWhenUseSkillChance = buff.GetRemoveBuffWhenUseSkillChance(level);
                _cacheRemoveBuffWhenUseItemChance = buff.GetRemoveBuffWhenUseItemChance(level);
                _cacheRemoveBuffWhenPickupItemChance = buff.GetRemoveBuffWhenPickupItemChance(level);
                _cacheMaxStack = buff.GetMaxStack(level);
                _cacheMountLevel = 0;
                if (buff.TryGetMount(out BuffMount mount))
                {
                    _cacheMount = mount;
                    _cacheMountLevel = mount.Level.GetAmount(_level);
                }
            }
            else
            {
                buff = Buff.Empty;
            }

            if (GameExtensionInstance.onBuildCalculatedBuff != null)
                GameExtensionInstance.onBuildCalculatedBuff(this);
        }

        public Buff GetBuff()
        {
            return _buff;
        }

        public int GetLevel()
        {
            return _level;
        }

        public float GetDuration()
        {
            // Intend to fix duration to 1 if no duration
            return NoDuration() ? 1f : _cacheDuration;
        }

        public bool NoDuration()
        {
            return _buff.noDuration;
        }

        public int GetRecoveryHp()
        {
            return _cacheRecoveryHp;
        }

        public int GetRecoveryMp()
        {
            return _cacheRecoveryMp;
        }

        public int GetRecoveryStamina()
        {
            return _cacheRecoveryStamina;
        }

        public int GetRecoveryFood()
        {
            return _cacheRecoveryFood;
        }

        public int GetRecoveryWater()
        {
            return _cacheRecoveryWater;
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

        public bool IsOverrideDamageInfo()
        {
            return _buff.isOverrideDamageInfo;
        }

        public DamageInfo GetOverrideDamageInfo()
        {
            return _buff.overrideDamageInfo;
        }

        public bool IsOverrideSkills()
        {
            return _buff.isOverrideSkills;
        }

        public Dictionary<BaseSkill, int> GetOverrideSkills()
        {
            return _cacheOverrideSkills;
        }

        public Dictionary<StatusEffect, float> GetIncreaseStatusEffectResistances()
        {
            return _cacheIncreaseStatusEffectResistances;
        }

        public Dictionary<BuffRemoval, float> GetBuffRemovals()
        {
            return _cacheBuffRemovals;
        }

        public Dictionary<DamageElement, MinMaxFloat> GetDamageOverTimes()
        {
            return _cacheDamageOverTimes;
        }

        public float GetRemoveBuffWhenAttackChance()
        {
            return _cacheRemoveBuffWhenAttackChance;
        }

        public float GetRemoveBuffWhenAttackedChance()
        {
            return _cacheRemoveBuffWhenAttackedChance;
        }

        public float GetRemoveBuffWhenUseSkillChance()
        {
            return _cacheRemoveBuffWhenUseSkillChance;
        }

        public float GetRemoveBuffWhenUseItemChance()
        {
            return _cacheRemoveBuffWhenUseItemChance;
        }

        public float GetRemoveBuffWhenPickupItemChance()
        {
            return _cacheRemoveBuffWhenPickupItemChance;
        }

        public int MaxStack()
        {
            return _cacheMaxStack;
        }

        public bool TryGetMount(out BuffMount mount)
        {
            mount = _cacheMount;
            return mount != null;
        }

        public int GetMountLevel()
        {
            return _cacheMountLevel;
        }
    }
}
