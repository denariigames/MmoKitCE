using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IServerUserContentMessageHandlers
    {
        UniTaskVoid HandleRequestUnlockContentProgression(
            RequestHandlerData requestHandler, RequestUnlockContentProgressionMessage request,
            RequestProceedResultDelegate<ResponseUnlockContentProgressionMessage> result);

        UniTaskVoid HandleRequestAvailableContents(
            RequestHandlerData requestHandler, RequestAvailableContentsMessage request,
            RequestProceedResultDelegate<ResponseAvailableContentsMessage> result);

        UniTaskVoid HandleRequestUnlockContent(
            RequestHandlerData requestHandler, RequestUnlockContentMessage request,
            RequestProceedResultDelegate<ResponseUnlockContentMessage> result);
    }
}