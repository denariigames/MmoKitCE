using UnityEngine;

namespace MultiplayerARPG
{
    public class MemoryManager : MonoBehaviour
    {

        private static MemoryManager s_Instance;
        public static MemoryManager Instance
        {
            get
            {
                PrepareInstance();
                return s_Instance;
            }
        }

        private static void PrepareInstance()
        {
            if (!Application.isPlaying)
                return;
            if (s_Instance != null)
                return;
            s_Instance = new GameObject("_MemoryManager").AddComponent<MemoryManager>();
            DontDestroyOnLoad(s_Instance.gameObject);
        }

        public static CharacterBuffCacheManager CharacterBuffs
        {
            get
            {
                PrepareInstance();
                return _characterBuffs;
            }
        }

        public static CharacterItemCacheManager CharacterItems
        {
            get
            {
                PrepareInstance();
                return _characterItems;
            }
        }

        public static CharacterSummonCacheManager CharacterSummons
        {
            get
            {
                PrepareInstance();
                return _characterSummons;
            }
        }

        public float updateDelay = 10f;

        private float _lastUpdateTime;
        private static readonly CharacterBuffCacheManager _characterBuffs = new CharacterBuffCacheManager();
        private static readonly CharacterItemCacheManager _characterItems = new CharacterItemCacheManager();
        private static readonly CharacterSummonCacheManager _characterSummons = new CharacterSummonCacheManager();

        private void Update()
        {
            float time = Time.unscaledTime;
            if (time - _lastUpdateTime < updateDelay)
                return;
            // Update other sub cache managers
            CharacterBuffs.OnUpdate();
            CharacterItems.OnUpdate();
            CharacterSummons.OnUpdate();
            _lastUpdateTime = time;
        }

        private void OnDestroy()
        {
            // Clear sub cache managers
            CharacterBuffs.Clear();
            CharacterItems.Clear();
            CharacterSummons.Clear();
        }
    }
}