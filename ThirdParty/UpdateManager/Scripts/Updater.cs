//fixed Duplicate registration
//Fixed so only editor/development use try/catch.
// now has early outs
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Insthync.ManagedUpdating
{
    public sealed class Updater
    {
        private readonly List<IManagedUpdate> _updates = new();
        private readonly List<IManagedLateUpdate> _lateUpdates = new();
        private readonly List<IManagedFixedUpdate> _fixedUpdates = new();

        public bool IsEmpty => _updates.Count == 0 && _lateUpdates.Count == 0 && _fixedUpdates.Count == 0;

        public void Register(IManagedUpdateBase item)
        {
            if (item == null)
                return;

            if (item is IManagedUpdate update && !_updates.Contains(update))
            {
                _updates.Add(update);
            }

            if (item is IManagedLateUpdate lateUpdate && !_lateUpdates.Contains(lateUpdate))
            {
                _lateUpdates.Add(lateUpdate);
            }

            if (item is IManagedFixedUpdate fixedUpdate && !_fixedUpdates.Contains(fixedUpdate))
            {
                _fixedUpdates.Add(fixedUpdate);
            }
        }

        public void Unregister(IManagedUpdateBase item)
        {
            if (item == null)
                return;

            if (item is IManagedUpdate update)
            {
                _updates.Remove(update);
            }

            if (item is IManagedLateUpdate lateUpdate)
            {
                _lateUpdates.Remove(lateUpdate);
            }

            if (item is IManagedFixedUpdate fixedUpdate)
            {
                _fixedUpdates.Remove(fixedUpdate);
            }
        }

        public void Clear()
        {
            _updates.Clear();
            _lateUpdates.Clear();
            _fixedUpdates.Clear();
        }

        internal void Update()
        {
            for (int i = _updates.Count - 1; i >= 0; --i)
            {
                IManagedUpdate item = _updates[i];
                if (IsDestroyedOrNull(item))
                {
                    _updates.RemoveAt(i);
                    continue;
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                try
                {
                    item.ManagedUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
#else
                item.ManagedUpdate();
#endif
            }
        }

        internal void LateUpdate()
        {
            for (int i = _lateUpdates.Count - 1; i >= 0; --i)
            {
                IManagedLateUpdate item = _lateUpdates[i];
                if (IsDestroyedOrNull(item))
                {
                    _lateUpdates.RemoveAt(i);
                    continue;
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                try
                {
                    item.ManagedLateUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
#else
                item.ManagedLateUpdate();
#endif
            }
        }

        internal void FixedUpdate()
        {
            for (int i = _fixedUpdates.Count - 1; i >= 0; --i)
            {
                IManagedFixedUpdate item = _fixedUpdates[i];
                if (IsDestroyedOrNull(item))
                {
                    _fixedUpdates.RemoveAt(i);
                    continue;
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                try
                {
                    item.ManagedFixedUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
#else
                item.ManagedFixedUpdate();
#endif
            }
        }

        private static bool IsDestroyedOrNull<T>(T item) where T : class
        {
            if (item == null)
                return true;

            return item is Object unityObject && unityObject == null;
        }
    }
}
