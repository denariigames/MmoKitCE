using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientUserContentHandlers : MonoBehaviour, IClientUserContentHandlers
    {
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestUnlockContentProgression(RequestUnlockContentProgressionMessage data, ResponseDelegate<ResponseUnlockContentProgressionMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.UnlockContentProgression, data, responseDelegate: callback);
        }

        public bool RequestAvailableContents(RequestAvailableContentsMessage data, ResponseDelegate<ResponseAvailableContentsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.AvailableContents, data, responseDelegate: callback);
        }

        public bool RequestUnlockContent(RequestUnlockContentMessage data, ResponseDelegate<ResponseUnlockContentMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.UnlockContent, data, responseDelegate: callback);
        }
    }
}
