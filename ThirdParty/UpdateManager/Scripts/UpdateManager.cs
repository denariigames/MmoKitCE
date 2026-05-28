using System.Collections.Generic;
using UnityEngine;

namespace Insthync.ManagedUpdating
{
    public sealed class UpdateManager : MonoBehaviour
    {
        private static readonly Updater _defaultUpdater = new Updater();
        private static readonly SortedList<int, Updater> _updaters = new SortedList<int, Updater>();

        private static UpdateManager _instance;
        private static bool _applicationIsQuitting;

        public static UpdateManager Instance => GetOrCreateInstance();

        private static UpdateManager GetOrCreateInstance()
        {
            if (_applicationIsQuitting)
                return null;

            if (_instance != null)
                return _instance;

            _instance = FindObjectOfType<UpdateManager>();
            return _instance != null ? _instance : CreateInstance();
        }

        private static UpdateManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(UpdateManager))
            {
                hideFlags = HideFlags.DontSave,
            };

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            else
#endif
            {
                DontDestroyOnLoad(gameObject);
            }

            return gameObject.AddComponent<UpdateManager>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        private void OnDestroy()
        {
            if (_instance != this)
                return;

            _instance = null;
            _defaultUpdater.Clear();
            _updaters.Clear();
        }

        public static void Register(IManagedUpdateBase updater)
        {
            if (updater == null)
                return;

            UpdateManager instance = GetOrCreateInstance();
            if (instance == null)
                return;

            instance.Register_Implementation(updater);
        }

        private void Register_Implementation(IManagedUpdateBase updater)
        {
            _defaultUpdater.Register(updater);
        }

        public static void Unregister(IManagedUpdateBase updater)
        {
            if (updater == null || _instance == null)
                return;

            _instance.Unregister_Implementation(updater);
        }

        private void Unregister_Implementation(IManagedUpdateBase updater)
        {
            _defaultUpdater.Unregister(updater);
        }

        public static void Register(int order, IManagedUpdateBase updater)
        {
            if (updater == null)
                return;

            UpdateManager instance = GetOrCreateInstance();
            if (instance == null)
                return;

            instance.Register_Implementation(order, updater);
        }

        private void Register_Implementation(int order, IManagedUpdateBase updater)
        {
            if (!_updaters.TryGetValue(order, out Updater updaterGroup))
            {
                updaterGroup = new Updater();
                _updaters.Add(order, updaterGroup);
            }

            updaterGroup.Register(updater);
        }

        public static void Unregister(int order, IManagedUpdateBase updater)
        {
            if (updater == null || _instance == null)
                return;

            _instance.Unregister_Implementation(order, updater);
        }

        private void Unregister_Implementation(int order, IManagedUpdateBase updater)
        {
            if (!_updaters.TryGetValue(order, out Updater updaterGroup))
                return;

            updaterGroup.Unregister(updater);

            if (updaterGroup.IsEmpty)
                _updaters.Remove(order);
        }

        private void Update()
        {
            _defaultUpdater.Update();

            IList<Updater> orderedUpdaters = _updaters.Values;
            for (int i = 0, count = orderedUpdaters.Count; i < count; ++i)
            {
                orderedUpdaters[i].Update();
            }
        }

        private void LateUpdate()
        {
            _defaultUpdater.LateUpdate();

            IList<Updater> orderedUpdaters = _updaters.Values;
            for (int i = 0, count = orderedUpdaters.Count; i < count; ++i)
            {
                orderedUpdaters[i].LateUpdate();
            }
        }

        private void FixedUpdate()
        {
            _defaultUpdater.FixedUpdate();

            IList<Updater> orderedUpdaters = _updaters.Values;
            for (int i = 0, count = orderedUpdaters.Count; i < count; ++i)
            {
                orderedUpdaters[i].FixedUpdate();
            }
        }
    }
}
