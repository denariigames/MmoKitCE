using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    [DefaultExecutionOrder(-10000)]
    public class LanGameEnableDisableEvents : MonoBehaviour
    {
        public UnityEvent onEnable = new UnityEvent();
        public UnityEvent onDisable = new UnityEvent();

        private void OnEnable()
        {
            if (BaseGameNetworkManager.Singleton is LanRpgNetworkManager)
                onEnable.Invoke();
        }

        private void OnDisable()
        {
            if (BaseGameNetworkManager.Singleton is LanRpgNetworkManager)
                onDisable.Invoke();
        }
    }
}
