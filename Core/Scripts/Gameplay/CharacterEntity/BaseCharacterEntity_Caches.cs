namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        private CharacterDataCache _cachedData;
        public CharacterDataCache CachedData
        {
            get
            {
                if (_cachedData == null)
                    _cachedData = this.GetCaches();
                return _cachedData;
            }
            set => _cachedData = value;
        }

        /// <summary>
        /// This variable will be TRUE when cache data have to re-cache
        /// </summary>
        public bool IsRecaching { get; protected set; }

        /// <summary>
        /// Make caches for character stats / attributes / skills / resistances / increase damages and so on immdediately
        /// </summary>
        public void ForceMakeCaches()
        {
            IsRecaching = true;
            MakeCaches();
        }

        /// <summary>
        /// Make caches for character stats / attributes / skills / resistances / increase damages and so on when update calls
        /// </summary>
        protected virtual void MakeCaches()
        {
            if (!IsRecaching)
                return;
            IsRecaching = false;

            // Make caches with cache manager
            this.MarkToMakeCaches();
            CachedData = this.GetCaches();

            // Invoke recached event
            if (onRecached != null)
                onRecached.Invoke();
        }
    }
}
