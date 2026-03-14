using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct IncrementalShort
    {
        [Tooltip("Amount at level 1")]
        public short baseAmount;
        [Tooltip("Increase amount when level > 1 (it will be decreasing when level < 0)")]
        public float amountIncreaseEachLevel;
        [Tooltip("Percentage rate increase per level (0.05 = +5% per level)")]
        public float rateIncreaseEachLevel;
        [Tooltip("It won't automatically sort by `minLevel`, you have to sort it from low to high to make it calculate properly")]
        public IncrementalShortByLevel[] amountIncreaseEachLevelByLevels;

        public short GetAmount(int level)
        {
            if (amountIncreaseEachLevelByLevels == null || amountIncreaseEachLevelByLevels.Length == 0)
            {
                float result = baseAmount;
                int countLevel = 2;
                while (countLevel <= level)
                {
                    result += amountIncreaseEachLevel;
                    result += result * rateIncreaseEachLevel;
                    countLevel++;
                }
                return (short)result;
            }
            float resultWithByLevel = baseAmount;
            int countByLevel = 2;
            int indexOfIncremental = 0;
            int firstMinLevel = amountIncreaseEachLevelByLevels[indexOfIncremental].minLevel;
            while (countByLevel <= level)
            {
                float flat = amountIncreaseEachLevel;
                float rate = rateIncreaseEachLevel;
                if (countByLevel >= firstMinLevel)
                {
                    flat = amountIncreaseEachLevelByLevels[indexOfIncremental].amountIncreaseEachLevel;
                    rate = amountIncreaseEachLevelByLevels[indexOfIncremental].rateIncreaseEachLevel;
                }
                resultWithByLevel += flat;
                resultWithByLevel += resultWithByLevel * rate;
                countByLevel++;
                if (indexOfIncremental + 1 < amountIncreaseEachLevelByLevels.Length &&
                    countByLevel >= amountIncreaseEachLevelByLevels[indexOfIncremental + 1].minLevel)
                    indexOfIncremental++;
            }
            return (short)resultWithByLevel;
        }
    }

    [System.Serializable]
    public struct IncrementalShortByLevel
    {
        public int minLevel;
        public float amountIncreaseEachLevel;
        public float rateIncreaseEachLevel;
    }
}
