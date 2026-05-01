namespace MultiplayerARPG.MMO
{
    public struct DatabaseCacheResult<T>
    {
        public bool HasValue { get; set; }
        public T Value { get; set; }

        public DatabaseCacheResult(T value)
        {
            HasValue = true;
            Value = value;
        }
    }
}
