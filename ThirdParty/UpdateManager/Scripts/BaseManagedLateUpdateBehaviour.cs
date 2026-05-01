using UnityEngine;

namespace Insthync.ManagedUpdating
{
    public abstract class BaseManagedLateUpdateBehaviour : MonoBehaviour, IManagedLateUpdate
    {
        protected virtual void OnEnable()
        {
            UpdateManager.Register(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.Unregister(this);
        }

        public abstract void ManagedLateUpdate();
    }
}
