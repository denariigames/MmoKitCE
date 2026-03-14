using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerBankMessageHandlers
    {
        UniTaskVoid HandleRequestWithdrawUserGold(
            RequestHandlerData requestHandler, RequestWithdrawUserGoldMessage request,
            RequestProceedResultDelegate<ResponseWithdrawUserGoldMessage> result);

        UniTaskVoid HandleRequestDepositUserGold(
            RequestHandlerData requestHandler, RequestDepositUserGoldMessage request,
            RequestProceedResultDelegate<ResponseDepositUserGoldMessage> result);

        UniTaskVoid HandleRequestWithdrawGuildGold(
            RequestHandlerData requestHandler, RequestWithdrawGuildGoldMessage request,
            RequestProceedResultDelegate<ResponseWithdrawGuildGoldMessage> result);

        UniTaskVoid HandleRequestDepositGuildGold(
            RequestHandlerData requestHandler, RequestDepositGuildGoldMessage request,
            RequestProceedResultDelegate<ResponseDepositGuildGoldMessage> result);
    }
}
