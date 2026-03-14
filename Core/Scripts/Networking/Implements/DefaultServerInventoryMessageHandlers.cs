using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultServerInventoryMessageHandlers : MonoBehaviour, IServerInventoryMessageHandlers
    {
        public UniTaskVoid HandleRequestSwapOrMergeItem(RequestHandlerData requestHandler, RequestSwapOrMergeItemMessage request, RequestProceedResultDelegate<ResponseSwapOrMergeItemMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSwapOrMergeItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanMoveItem())
            {
                result.InvokeError(new ResponseSwapOrMergeItemMessage());
                return default;
            }

            if (!playerCharacter.SwapOrMergeItem(request.fromIndex, request.toIndex, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseSwapOrMergeItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }
            result.InvokeSuccess(new ResponseSwapOrMergeItemMessage());
            return default;
        }

        public UniTaskVoid HandleRequestEquipArmor(RequestHandlerData requestHandler, RequestEquipArmorMessage request, RequestProceedResultDelegate<ResponseEquipArmorMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseEquipArmorMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanEquipItem())
            {
                result.InvokeError(new ResponseEquipArmorMessage()
                {
                    message = UITextKeys.UI_ERROR_CANNOT_EQUIP,
                });
                return default;
            }

            if (!playerCharacter.EquipArmor(request.nonEquipIndex, request.equipSlotIndex, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseEquipArmorMessage()
                {
                    message = gameMessage,
                });
                return default;
            }
            result.InvokeSuccess(new ResponseEquipArmorMessage());
            return default;
        }

        public UniTaskVoid HandleRequestEquipWeapon(RequestHandlerData requestHandler, RequestEquipWeaponMessage request, RequestProceedResultDelegate<ResponseEquipWeaponMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseEquipWeaponMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanEquipItem())
            {
                result.InvokeError(new ResponseEquipWeaponMessage()
                {
                    message = UITextKeys.UI_ERROR_CANNOT_EQUIP,
                });
                return default;
            }

            if (!playerCharacter.EquipWeapon(request.nonEquipIndex, request.equipWeaponSet, request.isLeftHand, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseEquipWeaponMessage()
                {
                    message = gameMessage,
                });
                return default;
            }
            result.InvokeSuccess(new ResponseEquipWeaponMessage());
            return default;
        }

        public UniTaskVoid HandleRequestUnEquipArmor(RequestHandlerData requestHandler, RequestUnEquipArmorMessage request, RequestProceedResultDelegate<ResponseUnEquipArmorMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseUnEquipArmorMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanUnEquipItem())
            {
                result.InvokeError(new ResponseUnEquipArmorMessage());
                return default;
            }

            if (!playerCharacter.UnEquipArmor(request.equipIndex, false, out UITextKeys gameMessage, out _, request.nonEquipIndex))
            {
                result.InvokeError(new ResponseUnEquipArmorMessage()
                {
                    message = gameMessage,
                });
                return default;
            }
            result.InvokeSuccess(new ResponseUnEquipArmorMessage());
            return default;
        }

        public UniTaskVoid HandleRequestUnEquipWeapon(RequestHandlerData requestHandler, RequestUnEquipWeaponMessage request, RequestProceedResultDelegate<ResponseUnEquipWeaponMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseUnEquipWeaponMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanUnEquipItem())
            {
                result.InvokeError(new ResponseUnEquipWeaponMessage());
                return default;
            }

            if (!playerCharacter.UnEquipWeapon(request.equipWeaponSet, request.isLeftHand, false, out UITextKeys gameMessage, out _, request.nonEquipIndex))
            {
                result.InvokeError(new ResponseUnEquipWeaponMessage()
                {
                    message = gameMessage,
                });
                return default;
            }
            result.InvokeSuccess(new ResponseUnEquipWeaponMessage());
            return default;
        }

        public UniTaskVoid HandleRequestSwitchEquipWeaponSet(RequestHandlerData requestHandler, RequestSwitchEquipWeaponSetMessage request, RequestProceedResultDelegate<ResponseSwitchEquipWeaponSetMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSwitchEquipWeaponSetMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            byte equipWeaponSet = request.equipWeaponSet;
            if (playerCharacter.EquipWeaponSet == equipWeaponSet)
            {
                result.InvokeSuccess(new ResponseSwitchEquipWeaponSetMessage());
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null)
            {
                if (!playerCharacterEntity.CanEquipItem())
                {
                    result.InvokeError(new ResponseSwitchEquipWeaponSetMessage()
                    {
                        message = UITextKeys.UI_ERROR_CANNOT_EQUIP,
                    });
                    return default;
                }
            }

            if (equipWeaponSet >= GameInstance.Singleton.maxEquipWeaponSet)
                equipWeaponSet = 0;
            playerCharacter.FillWeaponSetsIfNeeded(equipWeaponSet);
            playerCharacter.EquipWeaponSet = equipWeaponSet;

            if (playerCharacterEntity != null)
                playerCharacterEntity.ForceMakeCaches();

            result.InvokeSuccess(new ResponseSwitchEquipWeaponSetMessage());
            return default;
        }

        public UniTaskVoid HandleRequestDismantleItem(RequestHandlerData requestHandler, RequestDismantleItemMessage request, RequestProceedResultDelegate<ResponseDismantleItemMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseDismantleItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanDismantleItem())
            {
                result.InvokeError(new ResponseDismantleItemMessage());
                return default;
            }

            if (!playerCharacter.DismantleItem(request.inventoryType, request.index, request.equipSlotIndex, request.amount, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseDismantleItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseDismantleItemMessage()
            {
                message = gameMessage,
            });
            return default;
        }

        public UniTaskVoid HandleRequestDismantleItems(RequestHandlerData requestHandler, RequestDismantleItemsMessage request, RequestProceedResultDelegate<ResponseDismantleItemsMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseDismantleItemsMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanDismantleItem())
            {
                result.InvokeError(new ResponseDismantleItemsMessage());
                return default;
            }

            if (!playerCharacter.DismantleItems(request.selectedIndexes, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseDismantleItemsMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseDismantleItemsMessage()
            {
                message = gameMessage,
            });
            return default;
        }

        public UniTaskVoid HandleRequestEnhanceSocketItem(RequestHandlerData requestHandler, RequestEnhanceSocketItemMessage request, RequestProceedResultDelegate<ResponseEnhanceSocketItemMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseEnhanceSocketItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanEnhanceSocketItem())
            {
                result.InvokeError(new ResponseEnhanceSocketItemMessage());
                return default;
            }

            if (!playerCharacter.EnhanceSocketItem(request.inventoryType, request.index, request.equipSlotIndex, request.enhancerId, request.socketIndex, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseEnhanceSocketItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseEnhanceSocketItemMessage()
            {
                message = gameMessage,
            });
            return default;
        }

        public UniTaskVoid HandleRequestRefineItem(RequestHandlerData requestHandler, RequestRefineItemMessage request, RequestProceedResultDelegate<ResponseRefineItemMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseRefineItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanRefineItem())
            {
                result.InvokeError(new ResponseRefineItemMessage());
                return default;
            }

            if (!playerCharacter.RefineItem(request.inventoryType, request.index, request.equipSlotIndex, request.enhancerDataIds, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseRefineItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseRefineItemMessage()
            {
                message = gameMessage,
            });
            return default;
        }

        public UniTaskVoid HandleRequestRemoveEnhancerFromItem(RequestHandlerData requestHandler, RequestRemoveEnhancerFromItemMessage request, RequestProceedResultDelegate<ResponseRemoveEnhancerFromItemMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseRemoveEnhancerFromItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanRemoveEnhancerFromItem())
            {
                result.InvokeError(new ResponseRemoveEnhancerFromItemMessage());
                return default;
            }

            if (!playerCharacter.RemoveEnhancerFromItem(request.inventoryType, request.index, request.equipSlotIndex, request.socketIndex, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseRemoveEnhancerFromItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseRemoveEnhancerFromItemMessage()
            {
                message = gameMessage,
            });
            return default;
        }

        public UniTaskVoid HandleRequestRepairItem(RequestHandlerData requestHandler, RequestRepairItemMessage request, RequestProceedResultDelegate<ResponseRepairItemMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseRepairItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanRepairItem())
            {
                result.InvokeError(new ResponseRepairItemMessage());
                return default;
            }

            if (!playerCharacter.RepairItem(request.inventoryType, request.index, request.equipSlotIndex, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseRepairItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseRepairItemMessage()
            {
                message = gameMessage,
            });
            return default;
        }

        public UniTaskVoid HandleRequestRepairEquipItems(RequestHandlerData requestHandler, EmptyMessage request, RequestProceedResultDelegate<ResponseRepairEquipItemsMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseRepairEquipItemsMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null && !playerCharacterEntity.CanRepairItem())
            {
                result.InvokeError(new ResponseRepairEquipItemsMessage());
                return default;
            }

            if (!playerCharacter.RepairEquipItems(out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseRepairEquipItemsMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseRepairEquipItemsMessage()
            {
                message = gameMessage,
            });
            return default;
        }

        public UniTaskVoid HandleRequestSellItem(RequestHandlerData requestHandler, RequestSellItemMessage request, RequestProceedResultDelegate<ResponseSellItemMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSellItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            if (request.amount <= 0)
            {
                result.InvokeError(new ResponseSellItemMessage());
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null)
            {
                if (!playerCharacterEntity.CanSellItem())
                {
                    result.InvokeError(new ResponseSellItemMessage());
                    return default;
                }
                if (!playerCharacterEntity.NpcActionComponent.AccessingNpcShopDialog(out _))
                {
                    result.InvokeError(new ResponseSellItemMessage());
                    return default;
                }
            }

            if (!playerCharacter.SellItem(request.inventoryType, request.index, request.equipSlotIndex, request.amount, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseSellItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseSellItemMessage()
            {
                message = gameMessage,
            });
            return default;
        }

        public UniTaskVoid HandleRequestSellItems(RequestHandlerData requestHandler, RequestSellItemsMessage request, RequestProceedResultDelegate<ResponseSellItemsMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSellItemsMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null)
            {
                if (!playerCharacterEntity.CanSellItem())
                {
                    result.InvokeError(new ResponseSellItemsMessage());
                    return default;
                }
                if (!playerCharacterEntity.NpcActionComponent.AccessingNpcShopDialog(out _))
                {
                    result.InvokeError(new ResponseSellItemsMessage());
                    return default;
                }
            }

            if (!playerCharacter.SellItems(request.selectedIndexes, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseSellItemsMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseSellItemsMessage()
            {
                message = gameMessage,
            });
            return default;
        }

        public UniTaskVoid HandleRequestSortItems(RequestHandlerData requestHandler, RequestSortItemsMessage request, RequestProceedResultDelegate<ResponseSortItemsMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseSortItemsMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            List<CharacterItem> nonEquipItems = new List<CharacterItem>(playerCharacterEntity.NonEquipItems);
            nonEquipItems.Sort(new CharacterItemSort());
            if (!request.asc)
                nonEquipItems.Reverse();
            playerCharacterEntity.NonEquipItems = nonEquipItems;
            result.InvokeSuccess(new ResponseSortItemsMessage());
            return default;
        }

        public UniTaskVoid HandleRequestChangeAmmoItem(RequestHandlerData requestHandler, RequestChangeAmmoItemMessage request, RequestProceedResultDelegate<ResponseChangeAmmoItemMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseChangeAmmoItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null)
            {
                if (!playerCharacterEntity.CanChangeAmmoItem())
                {
                    result.InvokeError(new ResponseChangeAmmoItemMessage());
                    return default;
                }
            }

            UITextKeys gameMessage;
            int ammo = playerCharacter.GetAmmo(request.inventoryType, request.index, request.equipSlotIndex);
            if (ammo > 0)
            {
                if (!playerCharacter.RemoveAmmoFromItem(request.inventoryType, request.index, request.equipSlotIndex, out gameMessage))
                {
                    result.InvokeError(new ResponseChangeAmmoItemMessage()
                    {
                        message = gameMessage,
                    });
                    return default;
                }
            }

            if (!playerCharacter.PutAmmoToItem(request.inventoryType, request.index, request.equipSlotIndex, request.ammoItemId, out gameMessage))
            {
                result.InvokeError(new ResponseChangeAmmoItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseChangeAmmoItemMessage());
            return default;
        }

        public UniTaskVoid HandleRequestRemoveAmmoFromItem(RequestHandlerData requestHandler, RequestRemoveAmmoFromItemMessage request, RequestProceedResultDelegate<ResponseRemoveAmmoFromItemMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacter(requestHandler.ConnectionId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseRemoveAmmoFromItemMessage()
                {
                    message = UITextKeys.UI_ERROR_NOT_LOGGED_IN,
                });
                return default;
            }

            BasePlayerCharacterEntity playerCharacterEntity = playerCharacter as BasePlayerCharacterEntity;
            if (playerCharacterEntity != null)
            {
                if (!playerCharacterEntity.CanRemoveAmmoFromItem())
                {
                    result.InvokeError(new ResponseRemoveAmmoFromItemMessage());
                    return default;
                }
            }

            if (!playerCharacter.RemoveAmmoFromItem(request.inventoryType, request.index, request.equipSlotIndex, out UITextKeys gameMessage))
            {
                result.InvokeError(new ResponseRemoveAmmoFromItemMessage()
                {
                    message = gameMessage,
                });
                return default;
            }

            result.InvokeSuccess(new ResponseRemoveAmmoFromItemMessage());
            return default;
        }
    }
}
