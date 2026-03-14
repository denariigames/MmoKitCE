using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultClientGachaHandlers : MonoBehaviour, IClientGachaHandlers
    {
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestGachaInfo(ResponseDelegate<ResponseGachaInfoMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.GachaInfo, EmptyMessage.Value, responseDelegate: callback);
        }

        public bool RequestOpenGacha(RequestOpenGachaMessage data, ResponseDelegate<ResponseOpenGachaMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.OpenGacha, data, responseDelegate: callback);
        }
    }
}
