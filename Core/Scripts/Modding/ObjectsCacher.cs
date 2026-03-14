using UnityEngine;

namespace MultiplayerARPG
{
    public static partial class ObjectsCacher
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            CacheDevExtMethods();
        }

        public static partial void CacheDevExtMethods();
    }
}
