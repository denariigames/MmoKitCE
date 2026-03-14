using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLibManager;
using System.Collections.Concurrent;
using System.Net.Sockets;
using UnityEngine;

namespace MultiplayerARPG
{
    public class BaseGameNetworkManagerComponent : MonoBehaviour
    {
        public virtual void RegisterMessages(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void Clean(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void OnStartServer(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void OnStopServer(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void OnStartClient(BaseGameNetworkManager networkManager, LiteNetLibClient client)
        {

        }

        public virtual void OnStopClient(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void InitPrefabs(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void OnClientOnlineSceneLoaded(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void OnServerOnlineSceneLoaded(BaseGameNetworkManager networkManager)
        {

        }

        /// <summary>
        /// This function will be called to reader something after map info update data, when received from game-server
        /// You may use it to read seed for "Procedural Map Generation" system.
        /// </summary>
        /// <param name="networkManager"></param>
        /// <param name="reader"></param>
        public virtual void ReadMapInfoExtra(BaseGameNetworkManager networkManager, NetDataReader reader)
        {

        }

        /// <summary>
        /// This function will be called to write something after map info update data, which written before send to clients
        /// You may use it to write seed for "Procedural Map Generation" system.
        /// </summary>
        /// <param name="networkManager"></param>
        /// <param name="writer"></param>
        public virtual void WriteMapInfoExtra(BaseGameNetworkManager networkManager, NetDataWriter writer)
        {

        }

        /// <summary>
        /// This one do the same thing with `UpdateServerReadyToInstantiateObjectsStates` but it will be called before that one, just having it for backward compatibility
        /// </summary>
        /// <param name="networkManager"></param>
        /// <param name="serverReadyToInstantiateObjectsStates"></param>
        public virtual void UpdateReadyToInstantiateObjectsStates(BaseGameNetworkManager networkManager, ConcurrentDictionary<string, bool> serverReadyToInstantiateObjectsStates)
        {

        }

        /// <summary>
        /// This function will be called to update `serverReadyToInstantiateObjectsStates`, if all `serverReadyToInstantiateObjectsStates`'s values are `TRUE` the manager will determined that it is ready to instantiates objects (such as monster, harvestable and so on)
        /// You may use it in case that your game have an "Procedural Map Generation" system, which have to be proceeded before instantiates objects.
        /// </summary>
        /// <param name="networkManager"></param>
        /// <param name="serverReadyToInstantiateObjectsStates"></param>
        public virtual void UpdateServerReadyToInstantiateObjectsStates(BaseGameNetworkManager networkManager, ConcurrentDictionary<string, bool> serverReadyToInstantiateObjectsStates)
        {

        }

        /// <summary>
        /// This function will be called to update `clientReadyToInstantiateObjectsStates`, if all `clientReadyToInstantiateObjectsStates`'s values are `TRUE` the manager will determined that it is ready to instantiates objects (such as monster, harvestable and so on)
        /// You may use it in case that your game have an "Procedural Map Generation" system, which have to be proceeded before instantiates objects.
        /// </summary>
        /// <param name="networkManager"></param>
        /// <param name="clientReadyToInstantiateObjectsStates"></param>
        public virtual void UpdateClientReadyToInstantiateObjectsStates(BaseGameNetworkManager networkManager, ConcurrentDictionary<string, bool> serverReadyToInstantiateObjectsStates)
        {

        }

        public virtual void OnClientConnected(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void OnClientDisconnected(BaseGameNetworkManager networkManager, DisconnectReason reason, SocketError socketError, byte[] data)
        {

        }

        public virtual void OnPeerConnected(BaseGameNetworkManager networkManager, long connectionId)
        {

        }

        public virtual void OnPeerDisconnected(BaseGameNetworkManager networkManager, long connectionId, DisconnectReason reason, SocketError socketError)
        {

        }

        public virtual void SendClientEnterGame(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void SendClientReady(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void SendClientNotReady(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void SendClientSafeDisconnect(BaseGameNetworkManager networkManager)
        {

        }

        public virtual void HandleEnterGameResponse(BaseGameNetworkManager networkManager, ResponseHandlerData responseHandler, AckResponseCode responseCode, EnterGameResponseMessage response)
        {

        }

        public virtual void HandleClientReadyResponse(BaseGameNetworkManager networkManager, ResponseHandlerData responseHandler, AckResponseCode responseCode, EmptyMessage response)
        {

        }

        public virtual void HandleSafeDisconnectResponse(BaseGameNetworkManager networkManager, ResponseHandlerData responseHandler, AckResponseCode responseCode, EmptyMessage response)
        {

        }
    }
}
