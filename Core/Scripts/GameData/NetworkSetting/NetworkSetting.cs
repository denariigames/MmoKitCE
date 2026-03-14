using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.NETWORK_SETTING_FILE, menuName = GameDataMenuConsts.NETWORK_SETTING_MENU, order = GameDataMenuConsts.NETWORK_SETTING_ORDER)]
    public class NetworkSetting : ScriptableObject
    {
        public string networkAddress = "127.0.0.1";
        public int networkPort = 7770;
        public int maxConnections = 4;
    }
}
