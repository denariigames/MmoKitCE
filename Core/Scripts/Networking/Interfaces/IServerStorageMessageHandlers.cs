using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerStorageMessageHandlers
    {
        UniTaskVoid HandleRequestOpenStorage(
            RequestHandlerData requestHandler, RequestOpenStorageMessage request,
            RequestProceedResultDelegate<ResponseOpenStorageMessage> result);

        UniTaskVoid HandleRequestCloseStorage(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseCloseStorageMessage> result);

        UniTaskVoid HandleRequestMoveItemToStorage(
            RequestHandlerData requestHandler, RequestMoveItemToStorageMessage request,
            RequestProceedResultDelegate<ResponseMoveItemToStorageMessage> result);

        UniTaskVoid HandleRequestMoveItemFromStorage(
            RequestHandlerData requestHandler, RequestMoveItemFromStorageMessage request,
            RequestProceedResultDelegate<ResponseMoveItemFromStorageMessage> result);

        UniTaskVoid HandleRequestSwapOrMergeStorageItem(
            RequestHandlerData requestHandler, RequestSwapOrMergeStorageItemMessage request,
            RequestProceedResultDelegate<ResponseSwapOrMergeStorageItemMessage> result);
    }
}
