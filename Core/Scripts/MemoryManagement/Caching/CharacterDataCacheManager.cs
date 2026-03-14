using System.Collections.Concurrent;
using UnityEngine;

namespace MultiplayerARPG
{
    public static class CharacterDataCacheManager
    {
        private static readonly ConcurrentDictionary<int, CharacterDataCache> s_caches = new ConcurrentDictionary<int, CharacterDataCache>();

        public static CharacterDataCache GetCaches(this ICharacterData characterData)
        {
            if (characterData == null)
                return null;
            if (characterData is Object unityObj && unityObj == null)
                return null;
            int hashCode = characterData.GetHashCode();
            if (!s_caches.ContainsKey(hashCode))
            {
                // Did not mark to mark cache yet, so mark it here before get caches
                return s_caches[hashCode] = new CharacterDataCache().MarkToMakeCaches().GetCaches(characterData);
            }
            return s_caches[hashCode].GetCaches(characterData);
        }

        public static CharacterDataCache MarkToMakeCaches(this ICharacterData characterData)
        {
            if (characterData == null)
                return null;
            if (characterData is Object unityObj && unityObj == null)
                return null;
            int hashCode = characterData.GetHashCode();
            if (!s_caches.ContainsKey(hashCode))
            {
                // No stored caching data yet, create a new one and store to a colelction
                return s_caches[hashCode] = new CharacterDataCache().MarkToMakeCaches();
            }
            return s_caches[hashCode].MarkToMakeCaches();
        }

        public static void RemoveCaches(this ICharacterData characterData)
        {
            if (characterData == null)
                return;
            s_caches.TryRemove(characterData.GetHashCode(), out _);
        }

        public static void Clear()
        {
            s_caches.Clear();
        }
    }
}
