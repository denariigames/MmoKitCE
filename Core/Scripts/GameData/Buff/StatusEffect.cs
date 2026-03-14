using Cysharp.Text;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.STATUS_EFFECT_FILE, menuName = GameDataMenuConsts.STATUS_EFFECT_MENU, order = GameDataMenuConsts.STATUS_EFFECT_ORDER)]
    public partial class StatusEffect : BaseGameData
    {
        [Category("Status Effect Settings")]
        [SerializeField]
        private Buff buff = new Buff();
        public Buff Buff
        {
            get { return buff; }
        }

        [SerializeField]
        [Min(0f)]
        [Tooltip("If status effect resistance is `1.5`, it will `100%` resist status effect level `1` and `50%` resist status effect level `2`. Set it to `0` to no limit.")]
        private float maxResistanceAmount = 1f;
        public float MaxResistanceAmount { get { return maxResistanceAmount; } }

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("If value is `[0.8, 0.5, 0.25]`, and your character's status effect resistance is `2.15`, it will have chance `80%` to resist status effect level `1`, `50%` to resist level `2`, and `15%` to resist level `3`.")]
        private float[] maxResistanceAmountEachLevels = new float[0];
        public float[] MaxResistanceAmountEachLevels { get { return maxResistanceAmountEachLevels; } }

        /// <summary>
        /// This will be called when the buff is applied to `target` (by `applier`)
        /// </summary>
        /// <param name="target">The status effect receiver</param>
        /// <param name="applier">The status effect applier</param>
        /// <param name="weapon">The applier's weapon</param>
        /// <param name="sourceLevel">Level of a *thing* (skill or buff) which causes this status effect</param>
        /// <param name="applyBuffLevel">Level of a buff which applied to target</param>
        public virtual void OnApply(BaseCharacterEntity target, EntityInfo applier, CharacterItem weapon, int sourceLevel, int applyBuffLevel)
        {

        }

        public float GetResistanceByLevel(float totalResistance, int level)
        {
            if (MaxResistanceAmount > 0f && totalResistance > MaxResistanceAmount)
                totalResistance = MaxResistanceAmount;
            float resistance = totalResistance / level;
            if (resistance > 1f)
                resistance = 1f;
            if (MaxResistanceAmountEachLevels == null || MaxResistanceAmountEachLevels.Length == 0)
                return resistance;
            int index = level - 1;
            if (index >= 0)
            {
                if (index < MaxResistanceAmountEachLevels.Length)
                    resistance = Mathf.Min(resistance, MaxResistanceAmountEachLevels[index]);
                else
                    resistance = Mathf.Min(resistance, MaxResistanceAmountEachLevels[MaxResistanceAmountEachLevels.Length - 1]);
            }
            return resistance;
        }

        public bool RandomResistOccurs(float totalResistance, int level)
        {
            float resistance = GetResistanceByLevel(totalResistance, level);
            return resistance > 0f && Random.value <= resistance;
        }

        public string GetResistanceEntriesText(float totalResistance, string format, string separator = ",")
        {
            if (totalResistance > MaxResistanceAmount)
                totalResistance = MaxResistanceAmount;
            List<string> entry = new List<string>();
            for (int i = 0; i < totalResistance; ++i)
            {
                int level = i + 1;
                float resistance = GetResistanceByLevel(totalResistance, level);
                entry.Add(ZString.Concat(ZString.Format(
                        format,
                        level.ToString("N0"),
                        (resistance * 100f).ToString("N2"))));
            }
            return string.Join(separator, entry);
        }
    }

    [System.Serializable]
    public struct StatusEffectApplying
    {
        public StatusEffect statusEffect;
        [Tooltip("Buff stats will be calculated by level")]
        public IncrementalInt buffLevel;
    }

    [System.Serializable]
    public struct StatusEffectResistanceAmount
    {
        public StatusEffect statusEffect;
        public float amount;
    }

    [System.Serializable]
    public struct StatusEffectResistanceIncremental
    {
        public StatusEffect statusEffect;
        public IncrementalFloat amount;
    }
}
