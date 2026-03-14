using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract partial class BaseCharacter : BaseGameData
    {
        [Category("Generic Settings")]
        [SerializeField]
        protected ExpTable expTable;
        public ExpTable ExpTable
        {
            get
            {
                if (expTable == null)
                    return GameInstance.Singleton.ExpTable;
                return expTable;
            }
        }
        
        [Category(3, "Character Stats")]
        [SerializeField]
        private CharacterStatsIncremental stats;
        public virtual CharacterStatsIncremental Stats { get { return stats; } set { stats = value; } }

        [SerializeField]
        [ArrayElementTitle("attribute")]
        private AttributeIncremental[] attributes;
        public virtual AttributeIncremental[] Attributes { get { return attributes; } set { attributes = value; } }

        [SerializeField]
        [ArrayElementTitle("damageElement")]
        private ResistanceIncremental[] resistances;
        public virtual ResistanceIncremental[] Resistances { get { return resistances; } set { resistances = value; } }

        [SerializeField]
        [ArrayElementTitle("damageElement")]
        private ArmorIncremental[] armors;
        public virtual ArmorIncremental[] Armors { get { return armors; } set { armors = value; } }

        [SerializeField]
        [ArrayElementTitle("statusEffect")]
        private StatusEffectResistanceIncremental[] statusEffectResistances;
        public virtual StatusEffectResistanceIncremental[] StatusEffectResistances { get { return statusEffectResistances; } set { statusEffectResistances = value; } }

        public CharacterStats GetCharacterStats(int level)
        {
            return Stats.GetCharacterStats(level);
        }

        public void GetCharacterAttributes(int level, Dictionary<Attribute, float> result)
        {
            result.Clear();
            GameDataHelpers.CombineAttributes(Attributes, result, level, 1f);
        }

        public void GetCharacterResistances(int level, Dictionary<DamageElement, float> result)
        {
            result.Clear();
            GameDataHelpers.CombineResistances(Resistances, result, level, 1f);
        }

        public void GetCharacterArmors(int level, Dictionary<DamageElement, float> result)
        {
            result.Clear();
            GameDataHelpers.CombineArmors(Armors, result, level, 1f);
        }

        public void GetCharacterStatusEffectResistances(int level, Dictionary<StatusEffect, float> result)
        {
            result.Clear();
            GameDataHelpers.CombineStatusEffectResistances(StatusEffectResistances, result, level, 1f);
        }

        public abstract HashSet<int> GetLearnableSkillDataIds();

        public abstract void GetSkillLevels(int level, Dictionary<BaseSkill, int> result);

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddAttributes(Attributes);
            GameInstance.AddDamageElements(Resistances);
            GameInstance.AddDamageElements(Armors);
            GameInstance.AddStatusEffects(StatusEffectResistances);
        }
    }
}
