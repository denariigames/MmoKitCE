using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IClientBankHandlers
    {
        bool RequestWithdrawUserGold(RequestWithdrawUserGoldMessage data, ResponseDelegate<ResponseWithdrawUserGoldMessage> callback);
        bool RequestDepositUserGold(RequestDepositUserGoldMessage data, ResponseDelegate<ResponseDepositUserGoldMessage> callback);
        bool RequestWithdrawGuildGold(RequestWithdrawGuildGoldMessage data, ResponseDelegate<ResponseWithdrawGuildGoldMessage> callback);
        bool RequestDepositGuildGold(RequestDepositGuildGoldMessage data, ResponseDelegate<ResponseDepositGuildGoldMessage> callback);
    }
}
