using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientBankHandlers : MonoBehaviour, IClientBankHandlers
    {
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestDepositGuildGold(RequestDepositGuildGoldMessage data, ResponseDelegate<ResponseDepositGuildGoldMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DepositGuildGold, data, responseDelegate: callback);
        }

        public bool RequestDepositUserGold(RequestDepositUserGoldMessage data, ResponseDelegate<ResponseDepositUserGoldMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DepositUserGold, data, responseDelegate: callback);
        }

        public bool RequestWithdrawGuildGold(RequestWithdrawGuildGoldMessage data, ResponseDelegate<ResponseWithdrawGuildGoldMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.WithdrawGuildGold, data, responseDelegate: callback);
        }

        public bool RequestWithdrawUserGold(RequestWithdrawUserGoldMessage data, ResponseDelegate<ResponseWithdrawUserGoldMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.WithdrawUserGold, data, responseDelegate: callback);
        }
    }
}
