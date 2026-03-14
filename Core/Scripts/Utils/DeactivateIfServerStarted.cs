using UnityEngine;
using MultiplayerARPG;

namespace UtilsComponents
{
    public class DeactivateIfServerStarted : MonoBehaviour
    {
        private void Update()
        {
            if (BaseGameNetworkManager.Singleton != null && BaseGameNetworkManager.Singleton.IsServer)
                gameObject.SetActive(false);
        }
    }
}
