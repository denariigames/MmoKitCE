using System.Collections.Generic;
using UnityEngine;

namespace Insthync.ManagedUpdating
{
    public class ManagedUpdateRegisterer : MonoBehaviour
    {
        private readonly List<IManagedUpdateBase> _updaters = new();
        private bool _prepared = false;

        private void Prepare()
        {
            if (_prepared)
                return;

            _prepared = true;
            GetComponents(_updaters);
        }

        private void OnEnable()
        {
            Prepare();

            for (int i = 0, count = _updaters.Count; i < count; ++i)
            {
                UpdateManager.Register(_updaters[i]);
            }
        }

        private void OnDisable()
        {
            for (int i = 0, count = _updaters.Count; i < count; ++i)
            {
                UpdateManager.Unregister(_updaters[i]);
            }
        }
    }
}
