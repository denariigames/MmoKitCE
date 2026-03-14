using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientStorageHandlers : MonoBehaviour, IClientStorageHandlers
    {
        public StorageType StorageType { get; set; }
        public string StorageOwnerId { get; set; }
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestOpenStorage(RequestOpenStorageMessage data, ResponseDelegate<ResponseOpenStorageMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.OpenStorage, data, responseDelegate: callback);
        }

        public bool RequestCloseStorage(ResponseDelegate<ResponseCloseStorageMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.CloseStorage, EmptyMessage.Value, responseDelegate: callback);
        }

        public bool RequestMoveItemFromStorage(RequestMoveItemFromStorageMessage data, ResponseDelegate<ResponseMoveItemFromStorageMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.MoveItemFromStorage, data, responseDelegate: callback);
        }

        public bool RequestMoveItemToStorage(RequestMoveItemToStorageMessage data, ResponseDelegate<ResponseMoveItemToStorageMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.MoveItemToStorage, data, responseDelegate: callback);
        }

        public bool RequestSwapOrMergeStorageItem(RequestSwapOrMergeStorageItemMessage data, ResponseDelegate<ResponseSwapOrMergeStorageItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SwapOrMergeStorageItem, data, responseDelegate: callback);
        }
    }
}
