namespace MultiplayerARPG
{
    public partial class UISceneLoading : UIInstancedSceneLoading
    {
        public static UISceneLoading Singleton { get; private set; }

        protected override void Awake()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            base.Awake();
        }
    }
}