using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IServerCharacterMessageHandlers
    {
        UniTaskVoid HandleRequestIncreaseAttributeAmount(
            RequestHandlerData requestHandler, RequestIncreaseAttributeAmountMessage request,
            RequestProceedResultDelegate<ResponseIncreaseAttributeAmountMessage> result);

        UniTaskVoid HandleRequestIncreaseSkillLevel(
            RequestHandlerData requestHandler, RequestIncreaseSkillLevelMessage request,
            RequestProceedResultDelegate<ResponseIncreaseSkillLevelMessage> result);

        UniTaskVoid HandleRequestRespawn(
            RequestHandlerData requestHandler, RequestRespawnMessage request,
            RequestProceedResultDelegate<ResponseRespawnMessage> result);

        UniTaskVoid HandleRequestSetIcon(
            RequestHandlerData requestHandler, RequestSetIconMessage request,
            RequestProceedResultDelegate<ResponseSetIconMessage> result);

        UniTaskVoid HandleRequestSetFrame(
            RequestHandlerData requestHandler, RequestSetFrameMessage request,
            RequestProceedResultDelegate<ResponseSetFrameMessage> result);

        UniTaskVoid HandleRequestSetBackground(
            RequestHandlerData requestHandler, RequestSetBackgroundMessage request,
            RequestProceedResultDelegate<ResponseSetBackgroundMessage> result);

        UniTaskVoid HandleRequestSetTitle(
            RequestHandlerData requestHandler, RequestSetTitleMessage request,
            RequestProceedResultDelegate<ResponseSetTitleMessage> result);

        UniTaskVoid HandleRequestPlayerCharacterTransform(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponsePlayerCharacterTransformMessage> result);
    }
}
