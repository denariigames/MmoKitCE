using System.Collections.Generic;
using UnityEngine;

namespace Insthync.ManagedUpdating
{
    public sealed class UpdateManager : MonoBehaviour
    {
        private static readonly Updater _defaultUpdater = new Updater();
        private static readonly SortedList<int, Updater> _updaters = new SortedList<int, Updater>();

        private static UpdateManager _instance;
        public static UpdateManager Instance => _instance != null ? _instance : (_instance = CreateInstance());

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

        public static void Register(IManagedUpdateBase updater)
        {
            Instance.Register_Implementation(updater);
        }

        private void Register_Implementation(IManagedUpdateBase updater)
        {
            _defaultUpdater.Register(updater);
        }

        public static void Unregister(IManagedUpdateBase updater)
        {
            Instance.Unregister_Implementation(updater);
        }

        private void Unregister_Implementation(IManagedUpdateBase updater)
        {
            _defaultUpdater.Unregister(updater);
        }

        public static void Register(int order, IManagedUpdateBase updater)
        {
            Instance.Register_Implementation(order, updater);
        }

        private void Register_Implementation(int order, IManagedUpdateBase updater)
        {
            if (!_updaters.ContainsKey(order))
                _updaters.Add(order, new Updater());
            _updaters[order].Register(updater);
        }

        public static void Unregister(int order, IManagedUpdateBase updater)
        {
            Instance.Unregister_Implementation(order, updater);
        }

        private void Unregister_Implementation(int order, IManagedUpdateBase updater)
        {
            if (!_updaters.ContainsKey(order))
                return;
            _updaters[order].Unregister(updater);
        }

        private void Update()
        {
            _defaultUpdater.Update();
            for (int i = 0; i < _updaters.Count; ++i)
            {
                _updaters.Values[i].Update();
            }
        }

        private void LateUpdate()
        {
            _defaultUpdater.LateUpdate();
            for (int i = 0; i < _updaters.Count; ++i)
            {
                _updaters.Values[i].LateUpdate();
            }
        }

        private void FixedUpdate()
        {
            _defaultUpdater.FixedUpdate();
            for (int i = 0; i < _updaters.Count; ++i)
            {
                _updaters.Values[i].FixedUpdate();
            }
        }
    }
}
