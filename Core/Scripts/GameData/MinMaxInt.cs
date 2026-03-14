namespace MultiplayerARPG
{
    [System.Serializable]
    public struct MinMaxInt
    {
        public int min;
        public int max;

        public static MinMaxInt operator +(MinMaxInt a, MinMaxInt b)
        {
            MinMaxInt result = new MinMaxInt();
            result.min = a.min + b.min;
            result.max = a.max + b.max;
            return result;
        }

        public static MinMaxInt operator -(MinMaxInt a, MinMaxInt b)
        {
            MinMaxInt result = new MinMaxInt();
            result.min = a.min - b.min;
            result.max = a.max - b.max;
            return result;
        }

        public static MinMaxInt operator +(MinMaxInt a, int amount)
        {
            MinMaxInt result = new MinMaxInt();
            result.min = a.min + amount;
            result.max = a.max + amount;
            return result;
        }

        public static MinMaxInt operator -(MinMaxInt a, int amount)
        {
            MinMaxInt result = new MinMaxInt();
            result.min = a.min - amount;
            result.max = a.max - amount;
            return result;
        }

        public static MinMaxInt operator *(MinMaxInt a, float multiplier)
        {
            MinMaxInt result = new MinMaxInt();
            result.min = (int)(a.min * multiplier);
            result.max = (int)(a.max * multiplier);
            return result;
        }

        public static MinMaxInt operator /(MinMaxInt a, int divider)
        {
            MinMaxInt result = new MinMaxInt();
            result.min = a.min / divider;
            result.max = a.max / divider;
            return result;
        }

        public int Random(int seed)
        {
            return GenericUtils.RandomInt(seed, min, max);
        }

        public int Random()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}
