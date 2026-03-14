using System.Collections;
using System.Collections.Generic;
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
                    textRtt.text = "RTT: " + BaseGameNetworkManager.Singleton.Rtt.ToString("N0");
                if (textServerTimestamp)
                    textServerTimestamp.text = "ServerTimestamp: " + BaseGameNetworkManager.Singleton.ServerTimestamp.ToString("N0");
                return;
            }
            if (textRtt)
                textRtt.text = "RTT: N/A";
            if (textServerTimestamp)
                textServerTimestamp.text = "ServerTimestamp: N/A";
        }
    }
}
