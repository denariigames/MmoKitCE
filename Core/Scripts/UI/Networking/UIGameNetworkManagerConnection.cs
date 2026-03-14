using LiteNetLib;
using LiteNetLibManager;
using System.Net.Sockets;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIGameNetworkManagerConnection : BaseGameNetworkManagerComponent
    {
        public GameObject[] connectingObjects = new GameObject[0];
        public GameObject[] connectedObjects = new GameObject[0];
        public GameObject[] disconnectedObjects = new GameObject[0];

        public void SetConnectingActive(bool isActive)
        {
            for (int i = 0; i < connectingObjects.Length; ++i)
            {
                connectingObjects[i].SetActive(isActive);
            }
        }

        public void SetConnectedActive(bool isActive)
        {
            for (int i = 0; i < connectedObjects.Length; ++i)
            {
                connectedObjects[i].SetActive(isActive);
            }
        }

        public void SetDisconnectedActive(bool isActive)
        {
            for (int i = 0; i < disconnectedObjects.Length; ++i)
            {
                disconnectedObjects[i].SetActive(isActive);
            }
        }

        private void Start()
        {
            SetConnectingActive(false);
            SetConnectedActive(false);
            SetDisconnectedActive(false);
        }

        public override void OnStartClient(BaseGameNetworkManager networkManager, LiteNetLibClient client)
        {
            SetConnectingActive(true);
            SetConnectedActive(false);
            SetDisconnectedActive(false);
        }

        public override void OnStopClient(BaseGameNetworkManager networkManager)
        {
            SetConnectingActive(false);
            SetConnectedActive(false);
            SetDisconnectedActive(true);
        }

        public override void OnClientConnected(BaseGameNetworkManager networkManager)
        {
            SetConnectingActive(false);
            SetConnectedActive(true);
            SetDisconnectedActive(false);
        }

        public override void OnClientDisconnected(BaseGameNetworkManager networkManager, DisconnectReason reason, SocketError socketError, byte[] data)
        {
            SetConnectingActive(false);
            SetConnectedActive(false);
            SetDisconnectedActive(true);
        }

        public override void SendClientEnterGame(BaseGameNetworkManager networkManager)
        {
            SetConnectingActive(true);
            SetConnectedActive(false);
            SetDisconnectedActive(false);
        }

        public override void HandleEnterGameResponse(BaseGameNetworkManager networkManager, ResponseHandlerData responseHandler, AckResponseCode responseCode, EnterGameResponseMessage response)
        {
            SetConnectingActive(false);
            SetConnectedActive(responseCode == AckResponseCode.Success);
            SetDisconnectedActive(false);
        }

        public override void SendClientSafeDisconnect(BaseGameNetworkManager networkManager)
        {
            SetConnectingActive(true);
            SetConnectedActive(false);
            SetDisconnectedActive(false);
        }

        public override void HandleSafeDisconnectResponse(BaseGameNetworkManager networkManager, ResponseHandlerData responseHandler, AckResponseCode responseCode, EmptyMessage response)
        {
            SetConnectingActive(false);
            SetConnectedActive(responseCode == AckResponseCode.Success);
            SetDisconnectedActive(false);
        }
    }
}