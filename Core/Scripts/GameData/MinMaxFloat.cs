namespace MultiplayerARPG
{
    [System.Serializable]
    public struct MinMaxFloat
    {
        public float min;
        public float max;

        public static MinMaxFloat operator +(MinMaxFloat a, MinMaxFloat b)
        {
            MinMaxFloat result = new MinMaxFloat();
            result.min = a.min + b.min;
            result.max = a.max + b.max;
            return result;
        }

        public static MinMaxFloat operator -(MinMaxFloat a, MinMaxFloat b)
        {
            MinMaxFloat result = new MinMaxFloat();
            result.min = a.min - b.min;
            result.max = a.max - b.max;
            return result;
        }

        public static MinMaxFloat operator *(MinMaxFloat a, MinMaxFloat b)
        {
            MinMaxFloat result = new MinMaxFloat();
            result.min = a.min * b.min;
            result.max = a.max * b.max;
            return result;
        }

        public static MinMaxFloat operator /(MinMaxFloat a, MinMaxFloat b)
        {
            MinMaxFloat result = new MinMaxFloat();
            result.min = a.min / b.min;
            result.max = a.max / b.max;
            return result;
        }

        public static MinMaxFloat operator +(MinMaxFloat a, float amount)
        {
            MinMaxFloat result = new MinMaxFloat();
            result.min = a.min + amount;
            result.max = a.max + amount;
            return result;
        }

        public static MinMaxFloat operator -(MinMaxFloat a, float amount)
        {
            MinMaxFloat result = new MinMaxFloat();
            result.min = a.min - amount;
            result.max = a.max - amount;
            return result;
        }

        public static MinMaxFloat operator *(MinMaxFloat a, float multiplier)
        {
            MinMaxFloat result = new MinMaxFloat();
            result.min = a.min * multiplier;
            result.max = a.max * multiplier;
            return result;
        }

        public static MinMaxFloat operator /(MinMaxFloat a, float divider)
        {
            MinMaxFloat result = new MinMaxFloat();
            result.min = a.min / divider;
            result.max = a.max / divider;
            return result;
        }

        public float Random(int seed)
        {
            return GenericUtils.RandomFloat(seed, min, max);
        }

        public float Random()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}
