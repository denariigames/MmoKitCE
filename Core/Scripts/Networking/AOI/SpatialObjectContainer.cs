using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static class SpatialObjectContainer
    {
        private static readonly Dictionary<uint, ISpatialObjectComponent> s_components = new Dictionary<uint, ISpatialObjectComponent>();
        private static readonly Queue<uint> s_ids = new Queue<uint>();
        private static uint s_idCounter = 0;
        private static readonly object s_lock = new object();

        public static bool Add(ISpatialObjectComponent comp)
        {
            lock (s_lock)
            {
                if (comp.SpatialObjectId > 0 && s_components.ContainsKey(comp.SpatialObjectId))
                    return false;
                uint id = s_ids.Count > 0 ? s_ids.Dequeue() : ++s_idCounter;
                comp.SpatialObjectId = id;
                s_components[id] = comp;
                return true;
            }
        }

        public static bool Remove(ISpatialObjectComponent comp)
        {
            lock (s_lock)
            {
                uint id = comp.SpatialObjectId;
                if (!s_components.Remove(id))
                    return false;
                comp.SpatialObjectId = 0;
                s_ids.Enqueue(id);
                return true;
            }
        }

        public static bool TryGet(uint id, out ISpatialObjectComponent comp)
        {
            lock (s_lock)
            {
                return s_components.TryGetValue(id, out comp);
            }
        }

        public static bool Contains(uint id)
        {
            lock (s_lock)
            {
                return s_components.ContainsKey(id);
            }
        }

        public static IEnumerable<ISpatialObjectComponent> GetValues()
        {
            lock (s_lock)
            {
                return s_components.Values;
            }
        }

        public static void Clear()
        {
            lock (s_lock)
            {
                s_components.Clear();
                s_ids.Clear();
                s_idCounter = 0;
            }
        }
    }
}
