using System.Collections.Generic;
using UnityEngine;

namespace Insthync.ManagedUpdating
{
    public sealed class Updater
    {
        private readonly List<IManagedUpdate> _updates = new();
        private readonly List<IManagedLateUpdate> _lateUpdates = new();
        private readonly List<IManagedFixedUpdate> _fixedUpdates = new();

        public void Register(IManagedUpdateBase item)
        {
            if (item is IManagedUpdate update)
            {
                _updates.Add(update);
            }
            if (item is IManagedLateUpdate lateUpdate)
            {
                _lateUpdates.Add(lateUpdate);
            }
            if (item is IManagedFixedUpdate fixedUpdate)
            {
                _fixedUpdates.Add(fixedUpdate);
            }
        }

        public void Unregister(IManagedUpdateBase item)
        {
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

        internal void Update()
        {
            for (int i = _updates.Count - 1; i >= 0; --i)
            {
                if (_updates[i] == null)
                {
                    continue;
                }
                try
                {
                    _updates[i].ManagedUpdate();
                }
                catch
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                (System.Exception ex)
#endif
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.LogException(ex);
#endif
                }
            }
        }

        internal void LateUpdate()
        {
            for (int i = _lateUpdates.Count - 1; i >= 0; --i)
            {
                if (_lateUpdates[i] == null)
                {
                    continue;
                }
                try
                {
                    _lateUpdates[i].ManagedLateUpdate();
                }
                catch
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                (System.Exception ex)
#endif
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.LogException(ex);
#endif
                }
            }
        }

        internal void FixedUpdate()
        {
            for (int i = _fixedUpdates.Count - 1; i >= 0; --i)
            {
                if (_fixedUpdates[i] == null)
                {
                    continue;
                }
                try
                {
                    _fixedUpdates[i].ManagedFixedUpdate();
                }
                catch
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                (System.Exception ex)
#endif
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.LogException(ex);
#endif
                }
            }
        }
    }
}
