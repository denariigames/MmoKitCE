using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct ArmorAmount
    {
        [Tooltip("If `damageElement` is empty it will use default damage element from game instance")]
        public DamageElement damageElement;
        public float amount;
    }

    [System.Serializable]
    public struct ArmorRandomAmount
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

        public ArmorAmount GetRandomedAmount(System.Random random)
        {
            return new ArmorAmount()
            {
                damageElement = damageElement,
                amount = random.RandomFloat(minAmount, maxAmount),
            };
        }
    }

    [System.Serializable]
    public struct ArmorIncremental
    {
        [Tooltip("If `damageElement` is empty it will use default damage element from game instance")]
        public DamageElement damageElement;
        public IncrementalFloat amount;
    }
}
