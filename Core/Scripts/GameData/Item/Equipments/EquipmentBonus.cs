using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class EquipmentBonus
    {
        [SerializeField]
        private CharacterStats stats = new CharacterStats();
        public CharacterStats Stats => stats;

        [SerializeField]
        private CharacterStats statsRate = new CharacterStats();
        public CharacterStats StatsRate => statsRate;

        [ArrayElementTitle("attribute")]
        [SerializeField]
        private AttributeAmount[] attributes = new AttributeAmount[0];
        [System.NonSerialized]
        private Dictionary<Attribute, float> _cacheAttributes = null;
        public Dictionary<Attribute, float> Attributes
        {
            get
            {
                if (_cacheAttributes == null)
                {
                    _cacheAttributes = new Dictionary<Attribute, float>();
                    GameDataHelpers.CombineAttributes(attributes, _cacheAttributes, 1f);
                }
                return _cacheAttributes;
            }
        }

        [ArrayElementTitle("attribute")]
        [SerializeField]
        private AttributeAmount[] attributesRate = new AttributeAmount[0];
        [System.NonSerialized]
        private Dictionary<Attribute, float> _cacheAttributesRate = null;
        public Dictionary<Attribute, float> AttributesRate
        {
            get
            {
                if (_cacheAttributesRate == null)
                {
                    _cacheAttributesRate = new Dictionary<Attribute, float>();
                    GameDataHelpers.CombineAttributes(attributesRate, _cacheAttributesRate, 1f);
                }
                return _cacheAttributesRate;
            }
        }

        [ArrayElementTitle("damageElement")]
        [SerializeField]
        private ResistanceAmount[] resistances = new ResistanceAmount[0];
        [System.NonSerialized]
        private Dictionary<DamageElement, float> _cacheResistances = null;
        public Dictionary<DamageElement, float> Resistances
        {
            get
            {
                if (_cacheResistances == null)
                {
                    _cacheResistances = new Dictionary<DamageElement, float>();
                    GameDataHelpers.CombineResistances(resistances, _cacheResistances, 1f);
                }
                return _cacheResistances;
            }
        }

        [ArrayElementTitle("damageElement")]
        [SerializeField]
        private ArmorAmount[] armors = new ArmorAmount[0];
        [System.NonSerialized]
        private Dictionary<DamageElement, float> _cacheArmors = null;
        public Dictionary<DamageElement, float> Armors
        {
            get
            {
                if (_cacheArmors == null)
                {
                    _cacheArmors = new Dictionary<DamageElement, float>();
                    GameDataHelpers.CombineArmors(armors, _cacheArmors, 1f);
                }
                return _cacheArmors;
            }
        }

        [ArrayElementTitle("damageElement")]
        [SerializeField]
        private ArmorAmount[] armorsRate = new ArmorAmount[0];
        [System.NonSerialized]
        private Dictionary<DamageElement, float> _cacheArmorsRate = null;
        public Dictionary<DamageElement, float> ArmorsRate
        {
            get
            {
                if (_cacheArmorsRate == null)
                {
                    _cacheArmorsRate = new Dictionary<DamageElement, float>();
                    GameDataHelpers.CombineArmors(armorsRate, _cacheArmorsRate, 1f);
                }
                return _cacheArmorsRate;
            }
        }

        [ArrayElementTitle("damageElement")]
        [SerializeField]
        private DamageAmount[] damages = new DamageAmount[0];
        [System.NonSerialized]
        private Dictionary<DamageElement, MinMaxFloat> _cacheDamages = null;
        public Dictionary<DamageElement, MinMaxFloat> Damages
        {
            get
            {
                if (_cacheDamages == null)
                {
                    _cacheDamages = new Dictionary<DamageElement, MinMaxFloat>();
                    GameDataHelpers.CombineDamages(damages, _cacheDamages, 1f);
                }
                return _cacheDamages;
            }
        }

        [ArrayElementTitle("damageElement")]
        [SerializeField]
        private DamageAmount[] damagesRate = new DamageAmount[0];
        [System.NonSerialized]
        private Dictionary<DamageElement, MinMaxFloat> _cacheDamagesRate = null;
        public Dictionary<DamageElement, MinMaxFloat> DamagesRate
        {
            get
            {
                if (_cacheDamagesRate == null)
                {
                    _cacheDamagesRate = new Dictionary<DamageElement, MinMaxFloat>();
                    GameDataHelpers.CombineDamages(damagesRate, _cacheDamagesRate, 1f);
                }
                return _cacheDamagesRate;
            }
        }

        [ArrayElementTitle("skill")]
        [SerializeField]
        private SkillLevel[] skills = new SkillLevel[0];
        [System.NonSerialized]
        private Dictionary<BaseSkill, int> _cacheSkills = null;
        public Dictionary<BaseSkill, int> Skills
        {
            get
            {
                if (_cacheSkills == null)
                {
                    _cacheSkills = new Dictionary<BaseSkill, int>();
                    GameDataHelpers.CombineSkills(skills, _cacheSkills, 1f);
                }
                return _cacheSkills;
            }
        }

        [ArrayElementTitle("statusEffect")]
        [SerializeField]
        private StatusEffectResistanceAmount[] statusEffectResistances = new StatusEffectResistanceAmount[0];
        [System.NonSerialized]
        private Dictionary<StatusEffect, float> _cacheStatusEffectResistances = null;
        public Dictionary<StatusEffect, float> StatusEffectResistances
        {
            get
            {
                if (_cacheStatusEffectResistances == null)
                {
                    _cacheStatusEffectResistances = new Dictionary<StatusEffect, float>();
                    GameDataHelpers.CombineStatusEffectResistances(statusEffectResistances, _cacheStatusEffectResistances, 1f);
                }
                return _cacheStatusEffectResistances;
            }
        }
    }
}
