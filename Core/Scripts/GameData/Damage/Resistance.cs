using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct ResistanceAmount
    {
        [Tooltip("If `damageElement` is empty it will use default damage element from game instance")]
        public DamageElement damageElement;
        public float amount;
    }

    [System.Serializable]
    public struct ResistanceRandomAmount
    {
        public DamageElement damageElement;
        public float minAmount;
        public float maxAmount;
        [Range(0, 1f)]
        public float applyRate;

        public bool Apply(System.Random random)
        {
            return random.NextDouble() <= applyRate;
        }

        public ResistanceAmount GetRandomedAmount(System.Random random)
        {
            return new ResistanceAmount()
            {
                damageElement = damageElement,
                amount = random.RandomFloat(minAmount, maxAmount),
            };
        }
    }

    [System.Serializable]
    public struct ResistanceIncremental
    {
        [Tooltip("If `damageElement` is empty it will use default damage element from game instance")]
        public DamageElement damageElement;
        public IncrementalFloat amount;
    }
}

