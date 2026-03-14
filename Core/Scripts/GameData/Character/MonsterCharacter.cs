using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public enum MonsterCharacteristic
    {
        Normal,
        Aggressive,
        Assist,
        NoHarm,
    }

    [System.Serializable]
    public struct MonsterCharacterAmount
    {
        public MonsterCharacter monster;
        public int amount;
    }

    [CreateAssetMenu(fileName = GameDataMenuConsts.MONSTER_CHARACTER_FILE, menuName = GameDataMenuConsts.MONSTER_CHARACTER_MENU, order = GameDataMenuConsts.MONSTER_CHARACTER_ORDER)]
    public partial class MonsterCharacter : BaseCharacter
    {
        [Category(2, "Monster Settings")]
        [Header("Monster Data")]
        [SerializeField]
        [Tooltip("This will be used to adjust stats. If this value is 100, it means current stats which set to this character data is stats for character level 100, it will be used to adjust stats for character level 1.")]
        private int defaultLevel = 1;
        public int DefaultLevel { get { return defaultLevel; } }
        [SerializeField]
        [Tooltip("`Normal` will attack when being attacked, `Aggressive` will attack when enemy nearby, `Assist` will attack when other with same `Ally Id` being attacked, `NoHarm` won't attack.")]
        private MonsterCharacteristic characteristic = MonsterCharacteristic.Normal;
        public MonsterCharacteristic Characteristic { get { return characteristic; } }
        [SerializeField]
        [Tooltip("This will work with assist characteristic only, to detect ally")]
        private ushort allyId = 0;
        public ushort AllyId { get { return allyId; } }
        [SerializeField]
        private int reputation;
        public int Reputation { get { return reputation; } }
        [SerializeField]
        [Tooltip("This move speed will be applies when it's wandering. if it's going to chase enemy, stats'moveSpeed will be applies")]
        private float wanderMoveSpeed = 1f;
        public float WanderMoveSpeed { get { return wanderMoveSpeed; } }
        [SerializeField]
        [Tooltip("Range to see an enemies and allies")]
        private float visualRange = 5f;
        public float VisualRange { get { return visualRange; } }
        [SerializeField]
        [Tooltip("Range to see an enemies and allies while summoned")]
        private float summonedVisualRange = 10f;
        public float SummonedVisualRange { get { return summonedVisualRange; } }

        [Category(3, "Character Stats")]
        [SerializeField]
        [FormerlySerializedAs("monsterSkills")]
        [ArrayElementTitle("skill")]
        public MonsterSkill[] skills = new MonsterSkill[0];
        [SerializeField]
        private Buff summonerBuff = new Buff();
        public Buff SummonerBuff { get { return summonerBuff; } }
        [SerializeField]
        private bool isHeadshotInstantDeathProtected;
        public bool IsHeadshotInstantDeathProtected { get { return isHeadshotInstantDeathProtected; } }

        [Category(4, "Attacking")]
        [SerializeField]
        private DamageInfo damageInfo = new DamageInfo();
        public DamageInfo DamageInfo { get { return damageInfo; } }
        [SerializeField]
        private DamageIncremental damageAmount = default;
        public DamageIncremental DamageAmount { get { return damageAmount; } }
        [SerializeField]
        private float moveSpeedRateWhileAttacking = 0f;
        public float MoveSpeedRateWhileAttacking { get { return moveSpeedRateWhileAttacking; } }

        [Category(5, "Killing Rewards")]
        [SerializeField]
        private IncrementalMinMaxInt randomExp = default;
        [SerializeField]
        private IncrementalMinMaxInt randomGold = default;
        [SerializeField]
        private IncrementalFloat chanceToNotDropGold = default;
        [SerializeField]
        [ArrayElementTitle("currency")]
        public CurrencyRandomAmount[] randomCurrencies = new CurrencyRandomAmount[0];
        public ItemDropManager itemDropManager = new ItemDropManager();
        public ItemDropManager ItemDropManager { get { return itemDropManager; } }

        #region Being deprecated
        [HideInInspector]
        [SerializeField]
        [ArrayElementTitle("item")]
        private ItemDrop[] randomItems = new ItemDrop[0];

        [HideInInspector]
        [SerializeField]
        private ItemDropTable[] itemDropTables = new ItemDropTable[0];

        [HideInInspector]
        [SerializeField]
        private ItemRandomByWeightTable[] itemRandomByWeightTables = new ItemRandomByWeightTable[0];

        [HideInInspector]
        [SerializeField]
        [Tooltip("Max kind of items that will be dropped in ground")]
        private byte maxDropItems = 5;

        [HideInInspector]
        [SerializeField]
        private int randomExpMin;

        [HideInInspector]
        [SerializeField]
        private int randomExpMax;

        [HideInInspector]
        [SerializeField]
        private int randomGoldMin;

        [HideInInspector]
        [SerializeField]
        private int randomGoldMax;

        [HideInInspector]
        [SerializeField]
        private ItemDropTable itemDropTable = null;
        #endregion

        [System.NonSerialized]
        private List<CurrencyRandomAmount> _cacheRandomCurrencies = null;
        public List<CurrencyRandomAmount> CacheRandomCurrencies
        {
            get
            {
                if (_cacheRandomCurrencies == null)
                {
                    int i;
                    _cacheRandomCurrencies = new List<CurrencyRandomAmount>();
                    if (randomCurrencies != null &&
                        randomCurrencies.Length > 0)
                    {
                        for (i = 0; i < randomCurrencies.Length; ++i)
                        {
                            if (randomCurrencies[i].currency == null ||
                                randomCurrencies[i].maxAmount <= 0)
                                continue;
                            _cacheRandomCurrencies.Add(randomCurrencies[i]);
                        }
                    }
                    if (itemDropTables != null &&
                        itemDropTables.Length > 0)
                    {
                        foreach (ItemDropTable itemDropTable in itemDropTables)
                        {
                            if (itemDropTable != null &&
                                itemDropTable.randomCurrencies != null &&
                                itemDropTable.randomCurrencies.Length > 0)
                            {
                                for (i = 0; i < itemDropTable.randomCurrencies.Length; ++i)
                                {
                                    if (itemDropTable.randomCurrencies[i].currency == null ||
                                        itemDropTable.randomCurrencies[i].maxAmount <= 0)
                                        continue;
                                    _cacheRandomCurrencies.Add(itemDropTable.randomCurrencies[i]);
                                }
                            }
                        }
                    }
                }
                return _cacheRandomCurrencies;
            }
        }

        private readonly List<MonsterSkill> _tempRandomSkills = new List<MonsterSkill>();

        public virtual int RandomExp(int level)
        {
            return randomExp.GetAmount(level).Random();
        }

        public virtual int RandomGold(int level)
        {
            return randomGold.GetAmount(level).Random();
        }

        public virtual float ChanceToNotDropGold(int level)
        {
            return chanceToNotDropGold.GetAmount(level);
        }

        public virtual void RandomItems(OnDropItemDelegate onRandomItem, float rate = 1f)
        {
            ItemDropManager.RandomItems(onRandomItem, rate);
        }

        public virtual CurrencyAmount[] RandomCurrencies()
        {
            if (CacheRandomCurrencies.Count == 0)
                return new CurrencyAmount[0];
            List<CurrencyAmount> currencies = new List<CurrencyAmount>();
            CurrencyRandomAmount randomCurrency;
            for (int count = 0; count < CacheRandomCurrencies.Count; ++count)
            {
                randomCurrency = CacheRandomCurrencies[count];
                currencies.Add(new CurrencyAmount()
                {
                    currency = randomCurrency.currency,
                    amount = randomCurrency.GetRandomedAmount(),
                });
            }
            return currencies.ToArray();
        }

        [System.NonSerialized]
        private HashSet<int> _learnableSkillIds;
        public override HashSet<int> GetLearnableSkillDataIds()
        {
            if (_learnableSkillIds == null)
            {
                _learnableSkillIds = new HashSet<int>();
                foreach (MonsterSkill skill in skills)
                {
                    if (skill.skill == null)
                        continue;
                    _learnableSkillIds.Add(skill.skill.DataId);
                }
            }
            return _learnableSkillIds;
        }

        public override void GetSkillLevels(int level, Dictionary<BaseSkill, int> result)
        {
            result.Clear();
            if (level > 0)
                GameDataHelpers.CombineSkills(skills, result, level);
        }

        public virtual bool RandomSkill(BaseMonsterCharacterEntity entity, out BaseSkill skill, out int level)
        {
            skill = null;
            level = 1;

            if (!entity.CanUseSkill())
                return false;

            if (skills == null || skills.Length == 0)
                return false;

            if (_tempRandomSkills.Count != skills.Length)
            {
                _tempRandomSkills.Clear();
                _tempRandomSkills.AddRange(skills);
            }

            float random = Random.value;
            foreach (MonsterSkill monsterSkill in _tempRandomSkills)
            {
                if (monsterSkill.skill == null)
                    continue;

                if (random < monsterSkill.useRate && (monsterSkill.useWhenHpRate <= 0 || entity.HpRate <= monsterSkill.useWhenHpRate))
                {
                    skill = monsterSkill.skill;
                    level = monsterSkill.skillLevel.GetAmount(entity.Level);
                    // Shuffle for next random
                    _tempRandomSkills.Shuffle();
                    return true;
                }
            }
            return false;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            DamageInfo.PrepareRelatesData();
            ItemDropManager.PrepareRelatesData();
            GameInstance.AddSkills(skills);
        }

        public override bool Validate()
        {
            bool hasChanges = false;
            if (randomExpMin != 0 ||
                randomExpMax != 0)
            {
                hasChanges = true;
                if (randomExp.baseAmount.min == 0 &&
                    randomExp.baseAmount.max == 0 &&
                    randomExp.amountIncreaseEachLevel.min == 0 &&
                    randomExp.amountIncreaseEachLevel.max == 0)
                {
                    IncrementalMinMaxInt result = randomExp;
                    result.baseAmount.min = randomExpMin;
                    result.baseAmount.max = randomExpMax;
                    randomExp = result;
                }
                randomExpMin = 0;
                randomExpMax = 0;
            }
            if (randomGoldMin != 0 ||
                randomGoldMax != 0)
            {
                hasChanges = true;
                if (randomGold.baseAmount.min == 0 &&
                    randomGold.baseAmount.max == 0 &&
                    randomGold.amountIncreaseEachLevel.min == 0 &&
                    randomGold.amountIncreaseEachLevel.max == 0)
                {
                    IncrementalMinMaxInt result = randomGold;
                    result.baseAmount.min = randomGoldMin;
                    result.baseAmount.max = randomGoldMax;
                    randomGold = result;
                }
                randomGoldMin = 0;
                randomGoldMax = 0;
            }
            if (randomItems != null && randomItems.Length > 0)
            {
                hasChanges = true;
                List<ItemDrop> list = new List<ItemDrop>(itemDropManager.randomItems);
                list.AddRange(randomItems);
                itemDropManager.randomItems = list.ToArray();
                randomItems = null;
            }
            if (itemDropTable != null)
            {
                hasChanges = true;
                List<ItemDropTable> list = new List<ItemDropTable>(itemDropManager.itemDropTables)
                {
                    itemDropTable
                };
                itemDropManager.itemDropTables = list.ToArray();
                itemDropTable = null;
            }
            if (itemDropTables != null && itemDropTables.Length > 0)
            {
                hasChanges = true;
                List<ItemDropTable> list = new List<ItemDropTable>(itemDropManager.itemDropTables);
                list.AddRange(itemDropTables);
                itemDropManager.itemDropTables = list.ToArray();
                itemDropTables = null;
            }
            if (itemRandomByWeightTables != null && itemRandomByWeightTables.Length > 0)
            {
                hasChanges = true;
                List<ItemRandomByWeightTable> list = new List<ItemRandomByWeightTable>(itemDropManager.itemRandomByWeightTables);
                list.AddRange(itemRandomByWeightTables);
                itemDropManager.itemRandomByWeightTables = list.ToArray();
                itemRandomByWeightTables = null;
            }
            if (maxDropItems > 0)
            {
                hasChanges = true;
                itemDropManager.maxDropItems = maxDropItems;
                maxDropItems = 0;
            }
            if (defaultLevel < 1)
            {
                hasChanges = true;
                defaultLevel = 1;
            }
            if (skills != null && skills.Length > 0)
            {
                for (int i = 0; i < skills.Length; ++i)
                {
                    MonsterSkill skill = skills[i];
                    if (skill.skillLevel.baseAmount < skill.level)
                    {
                        skill.skillLevel.baseAmount = skill.level;
                        skill.level = 0;
                        skills[i] = skill;
                        hasChanges = true;
                    }
                }
            }
            if (AdjustDamageAmount())
                hasChanges = true;
            if (AdjustStats())
                hasChanges = true;
            if (AdjustAttributes())
                hasChanges = true;
            if (AdjustResistances())
                hasChanges = true;
            if (AdjustArmors())
                hasChanges = true;
            if (AdjustRandomExp())
                hasChanges = true;
            if (AdjustRandomGold())
                hasChanges = true;
            return hasChanges || base.Validate();
        }

        public bool AdjustDamageAmount()
        {
            if (DefaultLevel <= 1)
            {
                // Min level is `1` don't have to adjust it
                return false;
            }
            bool hasChanges = false;
            var setting = damageAmount;
            var incrementalAmount = setting.amount;
            var baseAmount = incrementalAmount.baseAmount;
            var amountIncreaseEachLevel = incrementalAmount.amountIncreaseEachLevel;
            float adjustedValue;
            // Min
            if (AdjustFloatValue(baseAmount.min, amountIncreaseEachLevel.min, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.min = adjustedValue;
                incrementalAmount.amountIncreaseEachLevel = amountIncreaseEachLevel;
                Debug.LogWarning($"Invalid min damage amount's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // Max
            if (AdjustFloatValue(baseAmount.max, amountIncreaseEachLevel.max, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.max = adjustedValue;
                incrementalAmount.amountIncreaseEachLevel = amountIncreaseEachLevel;
                Debug.LogWarning($"Invalid max damage amount's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            if (hasChanges)
            {
                setting.amount = incrementalAmount;
                damageAmount = setting;
            }
            return hasChanges;
        }

        public bool AdjustStats()
        {
            if (DefaultLevel <= 1)
            {
                // Min level is `1` don't have to adjust it
                return false;
            }
            bool hasChanges = false;
            CharacterStatsIncremental setting = Stats;
            CharacterStats baseAmount = setting.baseStats;
            CharacterStats amountIncreaseEachLevel = setting.statsIncreaseEachLevel;
            float adjustedValue;
            // hp
            if (AdjustFloatValue(baseAmount.hp, amountIncreaseEachLevel.hp, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.hp = adjustedValue;
                Debug.LogWarning($"Invalid hp stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // hpRecovery
            if (AdjustFloatValue(baseAmount.hpRecovery, amountIncreaseEachLevel.hpRecovery, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.hpRecovery = adjustedValue;
                Debug.LogWarning($"Invalid hpRecovery stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // hpLeechRate
            if (AdjustFloatValue(baseAmount.hpLeechRate, amountIncreaseEachLevel.hpLeechRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.hpLeechRate = adjustedValue;
                Debug.LogWarning($"Invalid hpLeechRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // mp
            if (AdjustFloatValue(baseAmount.mp, amountIncreaseEachLevel.mp, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.mp = adjustedValue;
                Debug.LogWarning($"Invalid mp stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // mpRecovery
            if (AdjustFloatValue(baseAmount.mpRecovery, amountIncreaseEachLevel.mpRecovery, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.mpRecovery = adjustedValue;
                Debug.LogWarning($"Invalid mpRecovery stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // mpLeechRate
            if (AdjustFloatValue(baseAmount.mpLeechRate, amountIncreaseEachLevel.mpLeechRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.mpLeechRate = adjustedValue;
                Debug.LogWarning($"Invalid mpLeechRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // stamina
            if (AdjustFloatValue(baseAmount.stamina, amountIncreaseEachLevel.stamina, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.stamina = adjustedValue;
                Debug.LogWarning($"Invalid stamina stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // staminaRecovery
            if (AdjustFloatValue(baseAmount.staminaRecovery, amountIncreaseEachLevel.staminaRecovery, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.staminaRecovery = adjustedValue;
                Debug.LogWarning($"Invalid staminaRecovery stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // staminaLeechRate
            if (AdjustFloatValue(baseAmount.staminaLeechRate, amountIncreaseEachLevel.staminaLeechRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.staminaLeechRate = adjustedValue;
                Debug.LogWarning($"Invalid staminaLeechRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // food
            if (AdjustFloatValue(baseAmount.food, amountIncreaseEachLevel.food, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.food = adjustedValue;
                Debug.LogWarning($"Invalid food stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // water
            if (AdjustFloatValue(baseAmount.water, amountIncreaseEachLevel.water, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.water = adjustedValue;
                Debug.LogWarning($"Invalid water stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // accuracy
            if (AdjustFloatValue(baseAmount.accuracy, amountIncreaseEachLevel.accuracy, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.accuracy = adjustedValue;
                Debug.LogWarning($"Invalid accuracy stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // evasion
            if (AdjustFloatValue(baseAmount.evasion, amountIncreaseEachLevel.evasion, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.evasion = adjustedValue;
                Debug.LogWarning($"Invalid evasion stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // criRate
            if (AdjustFloatValue(baseAmount.criRate, amountIncreaseEachLevel.criRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.criRate = adjustedValue;
                Debug.LogWarning($"Invalid criRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // criDmgRate
            if (AdjustFloatValue(baseAmount.criDmgRate, amountIncreaseEachLevel.criDmgRate, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.criDmgRate = adjustedValue;
                Debug.LogWarning($"Invalid criDmgRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // blockRate
            if (AdjustFloatValue(baseAmount.blockRate, amountIncreaseEachLevel.blockRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.blockRate = adjustedValue;
                Debug.LogWarning($"Invalid blockRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // blockDmgRate
            if (AdjustFloatValue(baseAmount.blockDmgRate, amountIncreaseEachLevel.blockDmgRate, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.blockDmgRate = adjustedValue;
                Debug.LogWarning($"Invalid blockDmgRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // moveSpeed
            if (AdjustFloatValue(baseAmount.moveSpeed, amountIncreaseEachLevel.moveSpeed, out adjustedValue, 0.01f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.moveSpeed = adjustedValue;
                Debug.LogWarning($"Invalid moveSpeed stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // sprintSpeed
            if (AdjustFloatValue(baseAmount.sprintSpeed, amountIncreaseEachLevel.sprintSpeed, out adjustedValue, 0.01f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.sprintSpeed = adjustedValue;
                Debug.LogWarning($"Invalid sprintSpeed stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // atkSpeed
            if (AdjustFloatValue(baseAmount.atkSpeed, amountIncreaseEachLevel.atkSpeed, out adjustedValue, 0.01f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.atkSpeed = adjustedValue;
                Debug.LogWarning($"Invalid atkSpeed stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // weightLimit
            if (AdjustFloatValue(baseAmount.weightLimit, amountIncreaseEachLevel.weightLimit, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.weightLimit = adjustedValue;
                Debug.LogWarning($"Invalid weightLimit stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // slotLimit
            if (AdjustFloatValue(baseAmount.slotLimit, amountIncreaseEachLevel.slotLimit, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.slotLimit = adjustedValue;
                Debug.LogWarning($"Invalid slotLimit stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // goldRate
            if (AdjustFloatValue(baseAmount.goldRate, amountIncreaseEachLevel.goldRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.goldRate = adjustedValue;
                Debug.LogWarning($"Invalid goldRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // expRate
            if (AdjustFloatValue(baseAmount.expRate, amountIncreaseEachLevel.expRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.expRate = adjustedValue;
                Debug.LogWarning($"Invalid expRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // itemDropRate
            if (AdjustFloatValue(baseAmount.itemDropRate, amountIncreaseEachLevel.itemDropRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.itemDropRate = adjustedValue;
                Debug.LogWarning($"Invalid itemDropRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // jumpHeight
            if (AdjustFloatValue(baseAmount.jumpHeight, amountIncreaseEachLevel.jumpHeight, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.jumpHeight = adjustedValue;
                Debug.LogWarning($"Invalid jumpHeight stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // headDamageAbsorbs
            if (AdjustFloatValue(baseAmount.headDamageAbsorbs, amountIncreaseEachLevel.headDamageAbsorbs, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.headDamageAbsorbs = adjustedValue;
                Debug.LogWarning($"Invalid headDamageAbsorbs stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // bodyDamageAbsorbs
            if (AdjustFloatValue(baseAmount.bodyDamageAbsorbs, amountIncreaseEachLevel.bodyDamageAbsorbs, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.bodyDamageAbsorbs = adjustedValue;
                Debug.LogWarning($"Invalid bodyDamageAbsorbs stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // fallDamageAbsorbs
            if (AdjustFloatValue(baseAmount.fallDamageAbsorbs, amountIncreaseEachLevel.fallDamageAbsorbs, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.fallDamageAbsorbs = adjustedValue;
                Debug.LogWarning($"Invalid fallDamageAbsorbs stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // gravityRate
            if (AdjustFloatValue(baseAmount.gravityRate, amountIncreaseEachLevel.gravityRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.gravityRate = adjustedValue;
                Debug.LogWarning($"Invalid gravityRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // protectedSlotLimit
            if (AdjustFloatValue(baseAmount.protectedSlotLimit, amountIncreaseEachLevel.protectedSlotLimit, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.protectedSlotLimit = adjustedValue;
                Debug.LogWarning($"Invalid protectedSlotLimit stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // ammoCapacity
            if (AdjustFloatValue(baseAmount.ammoCapacity, amountIncreaseEachLevel.ammoCapacity, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.ammoCapacity = adjustedValue;
                Debug.LogWarning($"Invalid ammoCapacity stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // recoilModifier
            if (AdjustFloatValue(baseAmount.recoilModifier, amountIncreaseEachLevel.recoilModifier, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.recoilModifier = adjustedValue;
                Debug.LogWarning($"Invalid recoilModifier stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // recoilRate
            if (AdjustFloatValue(baseAmount.recoilRate, amountIncreaseEachLevel.recoilRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.recoilRate = adjustedValue;
                Debug.LogWarning($"Invalid recoilRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // rateOfFire
            if (AdjustFloatValue(baseAmount.rateOfFire, amountIncreaseEachLevel.rateOfFire, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.rateOfFire = adjustedValue;
                Debug.LogWarning($"Invalid rateOfFire stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // reloadDuration
            if (AdjustFloatValue(baseAmount.reloadDuration, amountIncreaseEachLevel.reloadDuration, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.reloadDuration = adjustedValue;
                Debug.LogWarning($"Invalid reloadDuration stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // fireSpreadRangeRate
            if (AdjustFloatValue(baseAmount.fireSpreadRangeRate, amountIncreaseEachLevel.fireSpreadRangeRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.fireSpreadRangeRate = adjustedValue;
                Debug.LogWarning($"Invalid fireSpreadRangeRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // fireSpread
            if (AdjustFloatValue(baseAmount.fireSpread, amountIncreaseEachLevel.fireSpread, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.fireSpread = adjustedValue;
                Debug.LogWarning($"Invalid fireSpread stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // decreaseFoodDecreation
            if (AdjustFloatValue(baseAmount.decreaseFoodDecreation, amountIncreaseEachLevel.decreaseFoodDecreation, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.decreaseFoodDecreation = adjustedValue;
                Debug.LogWarning($"Invalid decreaseFoodDecreation stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // decreaseWaterDecreation
            if (AdjustFloatValue(baseAmount.decreaseWaterDecreation, amountIncreaseEachLevel.decreaseWaterDecreation, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.decreaseWaterDecreation = adjustedValue;
                Debug.LogWarning($"Invalid decreaseWaterDecreation stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // decreaseStaminaDecreation
            if (AdjustFloatValue(baseAmount.decreaseStaminaDecreation, amountIncreaseEachLevel.decreaseStaminaDecreation, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.decreaseStaminaDecreation = adjustedValue;
                Debug.LogWarning($"Invalid decreaseStaminaDecreation stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // buyItemPriceRate
            if (AdjustFloatValue(baseAmount.buyItemPriceRate, amountIncreaseEachLevel.buyItemPriceRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.buyItemPriceRate = adjustedValue;
                Debug.LogWarning($"Invalid buyItemPriceRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // sellItemPriceRate
            if (AdjustFloatValue(baseAmount.sellItemPriceRate, amountIncreaseEachLevel.sellItemPriceRate, out adjustedValue, 0f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.sellItemPriceRate = adjustedValue;
                Debug.LogWarning($"Invalid sellItemPriceRate stats's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            if (hasChanges)
            {
                Stats = setting;
            }
            return false;
        }

        public bool AdjustAttributes()
        {
            if (DefaultLevel <= 1)
            {
                // Min level is `1` don't have to adjust it
                return false;
            }
            bool hasChanges = false;
            for (int i = 0; i < Attributes.Length; ++i)
            {
                var setting = Attributes[i];
                var incrementalAmount = setting.amount;
                var baseAmount = incrementalAmount.baseAmount;
                var amountIncreaseEachLevel = incrementalAmount.amountIncreaseEachLevel;
                float adjustedValue;
                if (AdjustFloatValue(baseAmount, amountIncreaseEachLevel, out adjustedValue, 1f))
                {
                    hasChanges = true;
                    incrementalAmount.amountIncreaseEachLevel = adjustedValue;
                    setting.amount = incrementalAmount;
                    Attributes[i] = setting;
                    Debug.LogWarning($"Invalid {setting.attribute} attribute's increase each level setting for {this} adjusted to {adjustedValue}");
                }
            }
            return hasChanges;
        }

        public bool AdjustResistances()
        {
            if (DefaultLevel <= 1)
            {
                // Min level is `1` don't have to adjust it
                return false;
            }
            bool hasChanges = false;
            for (int i = 0; i < Resistances.Length; ++i)
            {
                var setting = Resistances[i];
                var incrementalAmount = setting.amount;
                var baseAmount = incrementalAmount.baseAmount;
                var amountIncreaseEachLevel = incrementalAmount.amountIncreaseEachLevel;
                float adjustedValue;
                if (AdjustFloatValue(baseAmount, amountIncreaseEachLevel, out adjustedValue, 1f))
                {
                    hasChanges = true;
                    incrementalAmount.amountIncreaseEachLevel = adjustedValue;
                    setting.amount = incrementalAmount;
                    Resistances[i] = setting;
                    Debug.LogWarning($"Invalid {setting.damageElement} resistance's increase each level setting for {this} adjusted to {adjustedValue}");
                }
            }
            return hasChanges;
        }

        public bool AdjustArmors()
        {
            if (DefaultLevel <= 1)
            {
                // Min level is `1` don't have to adjust it
                return false;
            }
            bool hasChanges = false;
            for (int i = 0; i < Armors.Length; ++i)
            {
                var setting = Armors[i];
                var incrementalAmount = setting.amount;
                var baseAmount = incrementalAmount.baseAmount;
                var amountIncreaseEachLevel = incrementalAmount.amountIncreaseEachLevel;
                float adjustedValue;
                if (AdjustFloatValue(baseAmount, amountIncreaseEachLevel, out adjustedValue, 1f))
                {
                    hasChanges = true;
                    incrementalAmount.amountIncreaseEachLevel = adjustedValue;
                    setting.amount = incrementalAmount;
                    Armors[i] = setting;
                    Debug.LogWarning($"Invalid {setting.damageElement} armor's increase each level setting for {this} adjusted to {adjustedValue}");
                }
            }
            return hasChanges;
        }

        public bool AdjustRandomExp()
        {
            if (DefaultLevel <= 1)
            {
                // Min level is `1` don't have to adjust it
                return false;
            }
            bool hasChanges = false;
            var setting = randomExp;
            var baseAmount = setting.baseAmount;
            var amountIncreaseEachLevel = setting.amountIncreaseEachLevel;
            float adjustedValue;
            // Min
            if (AdjustFloatValue(baseAmount.min, amountIncreaseEachLevel.min, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.min = adjustedValue;
                setting.amountIncreaseEachLevel = amountIncreaseEachLevel;
                Debug.LogWarning($"Invalid min random exp's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // Max
            if (AdjustFloatValue(baseAmount.max, amountIncreaseEachLevel.max, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.max = adjustedValue;
                setting.amountIncreaseEachLevel = amountIncreaseEachLevel;
                Debug.LogWarning($"Invalid max random exp's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            if (hasChanges)
            {
                randomExp = setting;
            }
            return hasChanges;
        }

        public bool AdjustRandomGold()
        {
            if (DefaultLevel <= 1)
            {
                // Min level is `1` don't have to adjust it
                return false;
            }
            bool hasChanges = false;
            var setting = randomGold;
            var baseAmount = setting.baseAmount;
            var amountIncreaseEachLevel = setting.amountIncreaseEachLevel;
            float adjustedValue;
            // Min
            if (AdjustFloatValue(baseAmount.min, amountIncreaseEachLevel.min, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.min = adjustedValue;
                setting.amountIncreaseEachLevel = amountIncreaseEachLevel;
                Debug.LogWarning($"Invalid min random gold's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            // Max
            if (AdjustFloatValue(baseAmount.max, amountIncreaseEachLevel.max, out adjustedValue, 1f))
            {
                hasChanges = true;
                amountIncreaseEachLevel.max = adjustedValue;
                setting.amountIncreaseEachLevel = amountIncreaseEachLevel;
                Debug.LogWarning($"Invalid max random gold's increase each level setting for {this} adjusted to {adjustedValue}");
            }
            if (hasChanges)
            {
                randomGold = setting;
            }
            return hasChanges;
        }

        public bool AdjustFloatValue(float baseAmount, float amountIncreaseEachLevel, out float adjustedAmountIncreaseEachLevel, float minValue)
        {
            adjustedAmountIncreaseEachLevel = amountIncreaseEachLevel;
            float adjustedValue = baseAmount - (amountIncreaseEachLevel * (defaultLevel - 1));
            if (adjustedValue < minValue)
            {
                adjustedAmountIncreaseEachLevel = baseAmount / (defaultLevel - 1);
                // Cut to 3 decimal
                adjustedAmountIncreaseEachLevel *= 1000f;
                adjustedAmountIncreaseEachLevel = Mathf.FloorToInt(adjustedAmountIncreaseEachLevel);
                adjustedAmountIncreaseEachLevel /= 1000f;
                if (adjustedAmountIncreaseEachLevel > 0.001f)
                {
                    // Some buffer
                    adjustedAmountIncreaseEachLevel -= 0.001f;
                }
                return true;
            }
            return false;
        }
    }
}
