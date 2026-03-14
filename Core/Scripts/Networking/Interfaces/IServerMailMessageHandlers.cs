using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerMailMessageHandlers
    {
        UniTaskVoid HandleRequestMailList(
            RequestHandlerData requestHandler, RequestMailListMessage request,
            RequestProceedResultDelegate<ResponseMailListMessage> result);

        UniTaskVoid HandleRequestReadMail(
            RequestHandlerData requestHandler, RequestReadMailMessage request,
            RequestProceedResultDelegate<ResponseReadMailMessage> result);

        UniTaskVoid HandleRequestClaimMailItems(
            RequestHandlerData requestHandler, RequestClaimMailItemsMessage request,
            RequestProceedResultDelegate<ResponseClaimMailItemsMessage> result);

        UniTaskVoid HandleRequestDeleteMail(
            RequestHandlerData requestHandler, RequestDeleteMailMessage request,
            RequestProceedResultDelegate<ResponseDeleteMailMessage> result);

        UniTaskVoid HandleRequestSendMail(
            RequestHandlerData requestHandler, RequestSendMailMessage request,
            RequestProceedResultDelegate<ResponseSendMailMessage> result);

        UniTaskVoid HandleRequestMailNotification(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseMailNotificationMessage> result);

        UniTaskVoid HandleRequestClaimAllMailsItems(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseClaimAllMailsItemsMessage> result);

        UniTaskVoid HandleRequestDeleteAllMails(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseDeleteAllMailsMessage> result);
    }
}
