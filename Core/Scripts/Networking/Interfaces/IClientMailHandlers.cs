using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IClientMailHandlers
    {
        bool RequestMailList(RequestMailListMessage data, ResponseDelegate<ResponseMailListMessage> callback);
        bool RequestReadMail(RequestReadMailMessage data, ResponseDelegate<ResponseReadMailMessage> callback);
        bool RequestClaimMailItems(RequestClaimMailItemsMessage data, ResponseDelegate<ResponseClaimMailItemsMessage> callback);
        bool RequestDeleteMail(RequestDeleteMailMessage data, ResponseDelegate<ResponseDeleteMailMessage> callback);
        bool RequestSendMail(RequestSendMailMessage data, ResponseDelegate<ResponseSendMailMessage> callback);
        bool RequestMailNotification(ResponseDelegate<ResponseMailNotificationMessage> callback);
        bool RequestClaimAllMailsItems(ResponseDelegate<ResponseClaimAllMailsItemsMessage> callback);
        bool RequestDeleteAllMails(ResponseDelegate<ResponseDeleteAllMailsMessage> callback);
    }
}
