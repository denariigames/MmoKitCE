using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerGachaMessageHandlers
    {
        UniTaskVoid HandleRequestGachaInfo(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseGachaInfoMessage> result);

        UniTaskVoid HandleRequestOpenGacha(
            RequestHandlerData requestHandler, RequestOpenGachaMessage request,
            RequestProceedResultDelegate<ResponseOpenGachaMessage> result);
    }
}
