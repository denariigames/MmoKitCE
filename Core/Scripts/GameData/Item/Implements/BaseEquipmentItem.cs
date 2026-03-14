using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract partial class BaseEquipmentItem : BaseItem, IEquipmentItem
    {
        [Category("In-Scene Objects/Appearance")]
        [SerializeField]
        private EquipmentModel[] equipmentModels = new EquipmentModel[0];
        public EquipmentModel[] EquipmentModels
        {
            get { return equipmentModels; }
            set { equipmentModels = value; }
        }

        [Category(2, "Equipment Settings")]
        [Header("Generic Equipment Settings")]
        [SerializeField]
        private ItemRequirement requirement = new ItemRequirement();
        public ItemRequirement Requirement
        {
            get { return requirement; }
        }

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
        private EquipmentSet equipmentSet = null;
        public EquipmentSet EquipmentSet
        {
            get { return equipmentSet; }
        }

        [SerializeField]
        private float maxDurability = 0f;
        public float MaxDurability
        {
            get { return maxDurability; }
        }

        [SerializeField]
        private bool destroyIfBroken = false;
        public bool DestroyIfBroken
        {
            get { return destroyIfBroken; }
        }

        [Tooltip("Its length is max amount of enhancement sockets")]
        [SerializeField]
        private SocketEnhancerType[] availableSocketEnhancerTypes = new SocketEnhancerType[0];
        public SocketEnhancerType[] AvailableSocketEnhancerTypes
        {
            get { return availableSocketEnhancerTypes; }
        }

        [System.Obsolete("Deprecated, will be removed later")]
        [HideInInspector]
        [SerializeField]
        private byte maxSocket = 0;

        [Category(3, "Buff/Bonus Settings")]
        [SerializeField]
        private CharacterStatsIncremental increaseStats = default;
        public CharacterStatsIncremental IncreaseStats
        {
            get { return increaseStats; }
        }

        [SerializeField]
        private CharacterStatsIncremental increaseStatsRate = default;
        public CharacterStatsIncremental IncreaseStatsRate
        {
            get { return increaseStatsRate; }
        }

        [SerializeField]
        private AttributeIncremental[] increaseAttributes = new AttributeIncremental[0];
        public AttributeIncremental[] IncreaseAttributes
        {
            get { return increaseAttributes; }
        }

        [SerializeField]
        private AttributeIncremental[] increaseAttributesRate = new AttributeIncremental[0];
        public AttributeIncremental[] IncreaseAttributesRate
        {
            get { return increaseAttributesRate; }
        }

        [SerializeField]
        private ResistanceIncremental[] increaseResistances = new ResistanceIncremental[0];
        public ResistanceIncremental[] IncreaseResistances
        {
            get { return increaseResistances; }
        }

        [SerializeField]
        private ArmorIncremental[] increaseArmors = new ArmorIncremental[0];
        public ArmorIncremental[] IncreaseArmors
        {
            get { return increaseArmors; }
        }

        [SerializeField]
        private ArmorIncremental[] increaseArmorsRate = new ArmorIncremental[0];
        public ArmorIncremental[] IncreaseArmorsRate
        {
            get { return increaseArmorsRate; }
        }

        [SerializeField]
        private DamageIncremental[] increaseDamages = new DamageIncremental[0];
        public DamageIncremental[] IncreaseDamages
        {
            get { return increaseDamages; }
        }

        [SerializeField]
        private DamageIncremental[] increaseDamagesRate = new DamageIncremental[0];
        public DamageIncremental[] IncreaseDamagesRate
        {
            get { return increaseDamagesRate; }
        }

        [HideInInspector]
        [SerializeField]
        private SkillLevel[] increaseSkillLevels = new SkillLevel[0];
        [SerializeField]
        private SkillIncremental[] increaseSkills = new SkillIncremental[0];
        public SkillIncremental[] IncreaseSkills
        {
            get { return increaseSkills; }
        }

        [SerializeField]
        private StatusEffectResistanceIncremental[] increaseStatusEffectResistances = new StatusEffectResistanceIncremental[0];
        public StatusEffectResistanceIncremental[] IncreaseStatusEffectResistances
        {
            get { return increaseStatusEffectResistances; }
        }

        [SerializeField]
        private StatusEffectApplying[] selfStatusEffectsWhenAttacking = new StatusEffectApplying[0];
        public StatusEffectApplying[] SelfStatusEffectsWhenAttacking
        {
            get { return selfStatusEffectsWhenAttacking; }
        }

        [SerializeField]
        private StatusEffectApplying[] enemyStatusEffectsWhenAttacking = new StatusEffectApplying[0];
        public StatusEffectApplying[] EnemyStatusEffectsWhenAttacking
        {
            get { return enemyStatusEffectsWhenAttacking; }
        }

        [SerializeField]
        private StatusEffectApplying[] selfStatusEffectsWhenAttacked = new StatusEffectApplying[0];
        public StatusEffectApplying[] SelfStatusEffectsWhenAttacked
        {
            get { return selfStatusEffectsWhenAttacked; }
        }

        [SerializeField]
        private StatusEffectApplying[] enemyStatusEffectsWhenAttacked = new StatusEffectApplying[0];
        public StatusEffectApplying[] EnemyStatusEffectsWhenAttacked
        {
            get { return enemyStatusEffectsWhenAttacked; }
        }

        [SerializeField]
        private ItemRandomBonus randomBonus = new ItemRandomBonus();
        public ItemRandomBonus RandomBonus
        {
            get { return randomBonus; }
        }

        public override bool Validate()
        {
            bool hasChanges = false;
            if (increaseSkillLevels != null && increaseSkillLevels.Length > 0)
            {
                List<SkillIncremental> skills = new List<SkillIncremental>();
                foreach (SkillLevel increaseSkillLevel in increaseSkillLevels)
                {
                    if (increaseSkillLevel.skill == null)
                        continue;
                    skills.Add(new SkillIncremental()
                    {
                        skill = increaseSkillLevel.skill,
                        level = new IncrementalInt()
                        {
                            baseAmount = increaseSkillLevel.level
                        },
                    });
                }
                increaseSkills = skills.ToArray();
                increaseSkillLevels = null;
                hasChanges = true;
            }
#pragma warning disable CS0618 // Type or member is obsolete
            if (maxSocket > 0)
            {
                availableSocketEnhancerTypes = new SocketEnhancerType[maxSocket];
                for (byte i = 0; i < maxSocket; ++i)
                {
                    availableSocketEnhancerTypes[i] = SocketEnhancerType.Type1;
                }
                maxSocket = 0;
                hasChanges = true;
            }
#pragma warning restore CS0618 // Type or member is obsolete
            return hasChanges || base.Validate();
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddAttributes(IncreaseAttributes);
            GameInstance.AddAttributes(IncreaseAttributesRate);
            GameInstance.AddDamageElements(IncreaseResistances);
            GameInstance.AddDamageElements(increaseArmors);
            GameInstance.AddDamageElements(increaseArmorsRate);
            GameInstance.AddDamageElements(increaseDamages);
            GameInstance.AddDamageElements(increaseDamagesRate);
            GameInstance.AddSkills(IncreaseSkills);
            GameInstance.AddStatusEffects(IncreaseStatusEffectResistances);
            GameInstance.AddStatusEffects(SelfStatusEffectsWhenAttacking);
            GameInstance.AddStatusEffects(EnemyStatusEffectsWhenAttacking);
            GameInstance.AddStatusEffects(SelfStatusEffectsWhenAttacked);
            GameInstance.AddStatusEffects(EnemyStatusEffectsWhenAttacked);
            GameInstance.AddEquipmentSets(EquipmentSet);
            RandomBonus.PrepareRelatesData();
            // Data migration
            GameInstance.MigrateEquipmentEntities(EquipmentModels);
        }
    }
}
