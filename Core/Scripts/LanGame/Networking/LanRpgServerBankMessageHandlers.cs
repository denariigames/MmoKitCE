using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class LanRpgServerBankMessageHandlers : MonoBehaviour, IServerBankMessageHandlers
    {
        public UniTaskVoid HandleRequestDepositGuildGold(RequestHandlerData requestHandler, RequestDepositGuildGoldMessage request, RequestProceedResultDelegate<ResponseDepositGuildGoldMessage> result)
        {
            if (request.gold <= 0)
            {
                result.InvokeError(new ResponseDepositGuildGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD,
                });
                return default;
            }
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseDepositGuildGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (!GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out GuildData guild))
            {
                result.InvokeError(new ResponseDepositGuildGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_JOINED_GUILD,
                });
                return default;
            }
            int requiredGold = GameInstance.Singleton.GameplayRule.GetGuildBankDepositFee(request.gold) + request.gold;
            if (playerCharacter.Gold < requiredGold)
            {
                result.InvokeError(new ResponseDepositGuildGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD_TO_DEPOSIT,
                });
                return default;
            }
            playerCharacter.Gold -= requiredGold;
            guild.gold += request.gold;
            GameInstance.ServerGuildHandlers.SetGuild(playerCharacter.GuildId, guild);
            GameInstance.ServerGameMessageHandlers.SendSetGuildGoldToMembers(guild);
            result.InvokeSuccess(new ResponseDepositGuildGoldMessage());
            return default;
        }

        public UniTaskVoid HandleRequestDepositUserGold(RequestHandlerData requestHandler, RequestDepositUserGoldMessage request, RequestProceedResultDelegate<ResponseDepositUserGoldMessage> result)
        {
            if (request.gold <= 0)
            {
                result.InvokeError(new ResponseDepositUserGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD,
                });
                return default;
            }
            if (GameInstance.Singleton.goldStoreMode == GoldStoreMode.UserGoldOnly)
            {
                result.InvokeError(new ResponseDepositUserGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE,
                });
                return default;
            }
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseDepositUserGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            int requiredGold = GameInstance.Singleton.GameplayRule.GetPlayerBankDepositFee(request.gold) + request.gold;
            if (playerCharacter.Gold < requiredGold)
            {
                result.InvokeError(new ResponseDepositUserGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD_TO_DEPOSIT,
                });
                return default;
            }
            playerCharacter.Gold -= requiredGold;
            playerCharacter.UserGold = playerCharacter.UserGold.Increase(request.gold);
            return default;
        }

        public UniTaskVoid HandleRequestWithdrawGuildGold(RequestHandlerData requestHandler, RequestWithdrawGuildGoldMessage request, RequestProceedResultDelegate<ResponseWithdrawGuildGoldMessage> result)
        {
            if (request.gold <= 0)
            {
                result.InvokeError(new ResponseWithdrawGuildGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD,
                });
                return default;
            }
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseWithdrawGuildGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (!GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out GuildData guild))
            {
                result.InvokeError(new ResponseWithdrawGuildGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_JOINED_GUILD,
                });
                return default;
            }
            int requiredGold = GameInstance.Singleton.GameplayRule.GetGuildBankWithdrawFee(request.gold) + request.gold;
            if (guild.gold < requiredGold)
            {
                result.InvokeError(new ResponseWithdrawGuildGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD_TO_WITHDRAW,
                });
                return default;
            }
            guild.gold -= requiredGold;
            playerCharacter.Gold = playerCharacter.Gold.Increase(request.gold);
            GameInstance.ServerGuildHandlers.SetGuild(playerCharacter.GuildId, guild);
            GameInstance.ServerGameMessageHandlers.SendSetGuildGoldToMembers(guild);
            result.InvokeSuccess(new ResponseWithdrawGuildGoldMessage());
            return default;
        }

        public UniTaskVoid HandleRequestWithdrawUserGold(RequestHandlerData requestHandler, RequestWithdrawUserGoldMessage request, RequestProceedResultDelegate<ResponseWithdrawUserGoldMessage> result)
        {
            if (request.gold <= 0)
            {
                result.InvokeError(new ResponseWithdrawUserGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD,
                });
                return default;
            }
            if (GameInstance.Singleton.goldStoreMode == GoldStoreMode.UserGoldOnly)
            {
                result.InvokeError(new ResponseWithdrawUserGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE,
                });
                return default;
            }
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseWithdrawUserGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            int requiredGold = GameInstance.Singleton.GameplayRule.GetPlayerBankWithdrawFee(request.gold) + request.gold;
            if (playerCharacter.UserGold < request.gold)
            {
                result.InvokeError(new ResponseWithdrawUserGoldMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD_TO_WITHDRAW,
                });
                return default;
            }
            playerCharacter.UserGold -= requiredGold;
            playerCharacter.Gold = playerCharacter.Gold.Increase(request.gold);
            result.InvokeSuccess(new ResponseWithdrawUserGoldMessage());
            return default;
        }
    }
}
