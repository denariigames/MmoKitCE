using System.Net;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UISceneHome : UIHistory
    {
        public UILanConnection uiLanConnection;
        public UICharacterList uiCharacterList;

        public void OnClickSinglePlayer()
        {
            LanRpgNetworkManager networkManager = BaseGameNetworkManager.Singleton as LanRpgNetworkManager;
            networkManager.startType = LanRpgNetworkManager.GameStartType.SinglePlayer;
            Next(uiCharacterList);
        }

        public void OnClickMultiplayer()
        {
            Next(uiLanConnection);
        }

        public void OnClickJoin()
        {
            LanRpgNetworkManager networkManager = BaseGameNetworkManager.Singleton as LanRpgNetworkManager;
            networkManager.startType = LanRpgNetworkManager.GameStartType.Client;
            networkManager.networkAddress = uiLanConnection.NetworkAddress;
            Next(uiCharacterList);
        }

        public void OnClickJoinDiscovery()
        {
            LanRpgNetworkManager networkManager = BaseGameNetworkManager.Singleton as LanRpgNetworkManager;
            networkManager.startType = LanRpgNetworkManager.GameStartType.Client;

            IPEndPoint remoteEndPoint = uiLanConnection.GetSelectedRemoteEndPoint();
            networkManager.networkAddress = remoteEndPoint.Address.ToString();
            networkManager.networkPort = remoteEndPoint.Port;
            Next(uiCharacterList);
        }

        public void OnClickHost()
        {
            LanRpgNetworkManager networkManager = BaseGameNetworkManager.Singleton as LanRpgNetworkManager;
            networkManager.startType = LanRpgNetworkManager.GameStartType.Host;
            Next(uiCharacterList);
        }

        public void OnClickExit()
        {
            Application.Quit();
        }
    }
}
