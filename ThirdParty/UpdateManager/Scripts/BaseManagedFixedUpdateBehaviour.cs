using UnityEngine;

namespace Insthync.ManagedUpdating
{
    public abstract class BaseManagedFixedUpdateBehaviour : MonoBehaviour, IManagedFixedUpdate
    {
        protected virtual void OnEnable()
        {
            UpdateManager.Register(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.Unregister(this);
        }

        public abstract void ManagedFixedUpdate();
    }
}
