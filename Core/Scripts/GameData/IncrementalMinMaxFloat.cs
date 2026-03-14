using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct IncrementalMinMaxFloat
    {
        [Tooltip("Amount at level 1")]
        public MinMaxFloat baseAmount;
        [Tooltip("Increase amount when level > 1 (it will be decreasing when level < 0)")]
        public MinMaxFloat amountIncreaseEachLevel;
        [Tooltip("Percentage rate increase per level (0.05 = +5% per level)")]
        public MinMaxFloat rateIncreaseEachLevel;
        [Tooltip("It won't automatically sort by `minLevel`, you have to sort it from low to high to make it calculate properly")]
        public IncrementalMinMaxFloatByLevel[] amountIncreaseEachLevelByLevels;

        public MinMaxFloat GetAmount(int level)
        {
            if (amountIncreaseEachLevelByLevels == null || amountIncreaseEachLevelByLevels.Length == 0)
            {
                MinMaxFloat result = baseAmount;
                int countLevel = 2;
                while (countLevel <= level)
                {
                    result += amountIncreaseEachLevel;
                    result += result * rateIncreaseEachLevel;
                    countLevel++;
                }
                return result;
            }
            MinMaxFloat resultWithByLevel = baseAmount;
            int countByLevel = 2;
            int indexOfIncremental = 0;
            int firstMinLevel = amountIncreaseEachLevelByLevels[indexOfIncremental].minLevel;
            while (countByLevel <= level)
            {
                MinMaxFloat flat = amountIncreaseEachLevel;
                MinMaxFloat rate = rateIncreaseEachLevel;
                if (countByLevel >= firstMinLevel)
                {
                    flat = amountIncreaseEachLevelByLevels[indexOfIncremental].amountIncreaseEachLevel;
                    rate = amountIncreaseEachLevelByLevels[indexOfIncremental].rateIncreaseEachLevel;
                }
                resultWithByLevel += flat;
                resultWithByLevel += resultWithByLevel * rate;
                countByLevel++;
                if (indexOfIncremental + 1 < amountIncreaseEachLevelByLevels.Length && countByLevel >= amountIncreaseEachLevelByLevels[indexOfIncremental + 1].minLevel)
                    indexOfIncremental++;
            }
            return resultWithByLevel;
        }
    }

    [System.Serializable]
    public struct IncrementalMinMaxFloatByLevel
    {
        public int minLevel;
        public MinMaxFloat amountIncreaseEachLevel;
        public MinMaxFloat rateIncreaseEachLevel;
    }
}
