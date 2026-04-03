using UnityEngine;

namespace Insthync.ManagedUpdating
{
    public class ManagedUpdateRegisterer : MonoBehaviour
    {
        private IManagedUpdateBase[] _updaters = null;
        private bool _prepared = false;

        private void Prepare()
        {
            if (_prepared)
                return;
            _prepared = true;
            _updaters = GetComponents<IManagedUpdateBase>();
        }

        private void OnEnable()
        {
            Prepare();
            if (_updaters == null)
                return;
            for (int i = 0; i < _updaters.Length; ++i)
            {
                UpdateManager.Register(_updaters[i]);
            }
        }

        private void OnDisable()
        {
            if (_updaters == null)
                return;
            for (int i = 0; i < _updaters.Length; ++i)
            {
                UpdateManager.Unregister(_updaters[i]);
            }
        }
    }
}
