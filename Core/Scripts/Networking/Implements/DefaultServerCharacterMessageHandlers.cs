using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    public partial class DefaultServerCharacterMessageHandlers : MonoBehaviour, IServerCharacterMessageHandlers
    {
        public BaseGameNetworkManager Manager { get; protected set; }

        protected virtual void Awake()
        {
            Manager = GetComponent<BaseGameNetworkManager>();
        }

        public UniTaskVoid HandleRequestIncreaseAttributeAmount(RequestHandlerData requestHandler, RequestIncreaseAttributeAmountMessage request, RequestProceedResultDelegate<ResponseIncreaseAttributeAmountMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseIncreaseAttributeAmountMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (!playerCharacter.AddAttribute(out UITextKeys gameMessage, request.dataId))
            {
                result.InvokeError(new ResponseIncreaseAttributeAmountMessage()
                {
                    message = gameMessage,
                });
                return default;
            }
            playerCharacter.StatPoint -= 1;
            result.InvokeSuccess(new ResponseIncreaseAttributeAmountMessage());
            return default;
        }

        public UniTaskVoid HandleRequestIncreaseSkillLevel(RequestHandlerData requestHandler, RequestIncreaseSkillLevelMessage request, RequestProceedResultDelegate<ResponseIncreaseSkillLevelMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseIncreaseSkillLevelMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            if (!playerCharacter.AddSkill(out UITextKeys gameMessage, request.dataId))
            {
                result.InvokeError(new ResponseIncreaseSkillLevelMessage()
                {
                    message = gameMessage,
                });
                return default;
            }
            int indexOfSkill = playerCharacter.IndexOfSkill(request.dataId);
            CharacterSkill characterSkill = playerCharacter.Skills[indexOfSkill];
            BaseSkill skill = characterSkill.GetSkill();
            int learnLevel = characterSkill.level - 1;
            float requireSkillPoint = skill.GetRequireCharacterSkillPoint(learnLevel);
            int requireGold = skill.GetRequireCharacterGold(learnLevel);
            playerCharacter.SkillPoint -= requireSkillPoint;
            playerCharacter.Gold -= requireGold;
            using (CollectionPool<Dictionary<Currency, int>, KeyValuePair<Currency, int>>.Get(out Dictionary<Currency, int> requireCurrencies))
            {
                skill.GetRequireCurrencyAmounts(learnLevel, requireCurrencies);
                playerCharacter.DecreaseCurrencies(requireCurrencies);
            }
            using (CollectionPool<Dictionary<BaseItem, int>, KeyValuePair<BaseItem, int>>.Get(out Dictionary<BaseItem, int> requireItems))
            {
                skill.GetRequireItemAmounts(learnLevel, requireItems);
                playerCharacter.DecreaseItems(requireItems);
                playerCharacter.FillEmptySlots();
            }
            result.InvokeSuccess(new ResponseIncreaseSkillLevelMessage());
            return default;
        }

        public async UniTaskVoid HandleRequestRespawn(RequestHandlerData requestHandler, RequestRespawnMessage request, RequestProceedResultDelegate<ResponseRespawnMessage> result)
        {
            long connectionId = requestHandler.ConnectionId;
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(connectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseRespawnMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return;
            }

            if (playerCharacter.CurrentHp > 0)
            {
                result.InvokeError(new ResponseRespawnMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_DEAD,
                });
                return;
            }

            if (Manager.proceedingConnectionIds.Contains(connectionId))
            {
                // TODO: Should send error message?
                result.InvokeError(new ResponseRespawnMessage()
                {
                    message = UITextKeys.NONE,
                });
                return;
            }
            Manager.proceedingConnectionIds.Add(connectionId);

            result.InvokeSuccess(new ResponseRespawnMessage());
            await GameInstance.ServerCharacterHandlers.Respawn(request.option, playerCharacter);
            Manager.proceedingConnectionIds.TryRemove(connectionId);
        }

        public UniTaskVoid HandleRequestSetIcon(RequestHandlerData requestHandler, RequestSetIconMessage request, RequestProceedResultDelegate<ResponseSetIconMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSetIconMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            // TODO: Implement data unlocking
            if (!GameInstance.PlayerIcons.TryGetValue(request.dataId, out PlayerIcon data) || data.UnlockRequirement.isLocked)
            {
                result.InvokeError(new ResponseSetIconMessage()
                {
                    message = UITextKeys.UI_ERROR_INVALID_DATA,
                });
                return default;
            }
            playerCharacter.IconDataId = request.dataId;
            result.InvokeSuccess(new ResponseSetIconMessage()
            {
                dataId = request.dataId,
            });
            return default;
        }

        public UniTaskVoid HandleRequestSetFrame(RequestHandlerData requestHandler, RequestSetFrameMessage request, RequestProceedResultDelegate<ResponseSetFrameMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSetFrameMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            // TODO: Implement data unlocking
            if (!GameInstance.PlayerFrames.TryGetValue(request.dataId, out PlayerFrame data) || data.UnlockRequirement.isLocked)
            {
                result.InvokeError(new ResponseSetFrameMessage()
                {
                    message = UITextKeys.UI_ERROR_INVALID_DATA,
                });
                return default;
            }
            playerCharacter.FrameDataId = request.dataId;
            result.InvokeSuccess(new ResponseSetFrameMessage()
            {
                dataId = request.dataId,
            });
            return default;
        }

        public UniTaskVoid HandleRequestSetBackground(RequestHandlerData requestHandler, RequestSetBackgroundMessage request, RequestProceedResultDelegate<ResponseSetBackgroundMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSetBackgroundMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            // TODO: Implement data unlocking
            if (!GameInstance.PlayerBackgrounds.TryGetValue(request.dataId, out PlayerBackground data) || data.UnlockRequirement.isLocked)
            {
                result.InvokeError(new ResponseSetBackgroundMessage()
                {
                    message = UITextKeys.UI_ERROR_INVALID_DATA,
                });
                return default;
            }
            playerCharacter.BackgroundDataId = request.dataId;
            result.InvokeSuccess(new ResponseSetBackgroundMessage()
            {
                dataId = request.dataId,
            });
            return default;
        }

        public UniTaskVoid HandleRequestSetTitle(RequestHandlerData requestHandler, RequestSetTitleMessage request, RequestProceedResultDelegate<ResponseSetTitleMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSetTitleMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }
            // TODO: Implement data unlocking
            if (!GameInstance.PlayerTitles.TryGetValue(request.dataId, out PlayerTitle data) || data.UnlockRequirement.isLocked)
            {
                result.InvokeError(new ResponseSetTitleMessage()
                {
                    message = UITextKeys.UI_ERROR_INVALID_DATA,
                });
                return default;
            }
            playerCharacter.TitleDataId = request.dataId;
            result.InvokeSuccess(new ResponseSetTitleMessage()
            {
                dataId = request.dataId,
            });
            return default;
        }

        public async UniTaskVoid HandleRequestPlayerCharacterTransform(RequestHandlerData requestHandler, EmptyMessage request, RequestProceedResultDelegate<ResponsePlayerCharacterTransformMessage> result)
        {
            if (GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeSuccess(new ResponsePlayerCharacterTransformMessage()
                {
                    position = playerCharacter.CurrentPosition,
                    rotation = playerCharacter.CurrentRotation,
                });
                return;
            }

            ResponsePlayerCharacterTransformMessage response = await BaseGameNetworkManager.Singleton.RequestPlayerCharacterTransform(requestHandler.ConnectionId);
            if (response.message != UITextKeys.NONE)
            {
                result.InvokeError(response);
                return;
            }
            result.InvokeSuccess(response);
        }
    }
}
