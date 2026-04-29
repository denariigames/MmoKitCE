using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UINetworkTime : MonoBehaviour
    {
        public Text textRtt;
        public Text textServerTimestamp;

        private void Update()
        {
            if (BaseGameNetworkManager.Singleton.IsClientConnected || 
                BaseGameNetworkManager.Singleton.IsServer)
            {
                if (textRtt)
                    textRtt.text = ZString.Format("RTT: {0:N0}", BaseGameNetworkManager.Singleton.Rtt);
                if (textServerTimestamp)
                    textServerTimestamp.text = ZString.Format("ServerTimestamp: {0:N0}", BaseGameNetworkManager.Singleton.ServerTimestamp);
                return;
            }
            if (textRtt)
                textRtt.text = "RTT: N/A";
            if (textServerTimestamp)
                textServerTimestamp.text = "ServerTimestamp: N/A";
        }
    }
}
