using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientMailHandlers : MonoBehaviour, IClientMailHandlers
    {
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestMailList(RequestMailListMessage data, ResponseDelegate<ResponseMailListMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.MailList, data, responseDelegate: callback);
        }

        public bool RequestReadMail(RequestReadMailMessage data, ResponseDelegate<ResponseReadMailMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ReadMail, data, responseDelegate: callback);
        }

        public bool RequestClaimMailItems(RequestClaimMailItemsMessage data, ResponseDelegate<ResponseClaimMailItemsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ClaimMailItems, data, responseDelegate: callback);
        }

        public bool RequestDeleteMail(RequestDeleteMailMessage data, ResponseDelegate<ResponseDeleteMailMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DeleteMail, data, responseDelegate: callback);
        }

        public bool RequestSendMail(RequestSendMailMessage data, ResponseDelegate<ResponseSendMailMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SendMail, data, responseDelegate: callback);
        }

        public bool RequestMailNotification(ResponseDelegate<ResponseMailNotificationMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.MailNotification, EmptyMessage.Value, responseDelegate: callback);
        }

        public bool RequestClaimAllMailsItems(ResponseDelegate<ResponseClaimAllMailsItemsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ClaimAllMailsItems, EmptyMessage.Value, responseDelegate: callback);
        }

        public bool RequestDeleteAllMails(ResponseDelegate<ResponseDeleteAllMailsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DeleteAllMails, EmptyMessage.Value, responseDelegate: callback);
        }
    }
}
