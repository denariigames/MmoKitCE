using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct ItemDrop
    {
        public BaseItem item;
        [Tooltip("Set `minLevel` to <= `0` to not random level, it will use `maxLevel` as a dropped level")]
        public int minLevel;
        [Min(1)]
        public int maxLevel;
        [Tooltip("Set `minAmount` to <= `0` to not random amount, it will use `maxAmount` as a dropped amount")]
        public int minAmount;
        [FormerlySerializedAs("amount")]
        [Min(1)]
        public int maxAmount;
        [Range(0f, 1f)]
        public float dropRate;

        public void GetMinMaxLevel(out int min, out int max)
        {
            min = minLevel;
            max = maxLevel;
            if (max <= 0)
                max = 1;
            if (max < min)
                max = min;
            if (min <= 0)
                min = max;
        }

        public int GetRandomedLevel()
        {
            GetMinMaxLevel(out int min, out int max);
            return Random.Range(min, max + 1);
        }

        public void GetMinMaxAmount(out int min, out int max)
        {
            min = minAmount;
            max = maxAmount;
            if (max <= 0)
                max = 1;
            if (max < min)
                max = min;
            if (min <= 0)
                min = max;
        }

        public int GetRandomedAmount()
        {
            GetMinMaxAmount(out int min, out int max);
            return Random.Range(min, max + 1);
        }
    }
}
