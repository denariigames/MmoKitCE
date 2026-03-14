using LiteNetLib;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public partial class PlayerCharacterDealingComponent : BaseNetworkedGameEntityComponent<BasePlayerCharacterEntity>
    {
        protected DealingState _dealingState = DealingState.None;
        public DealingState DealingState
        {
            get { return _dealingState; }
            set
            {
                _dealingState = value;
                CallOwnerUpdateDealingState(value);
                if (DealingCharacter != null)
                    DealingCharacter.DealingComponent.CallOwnerUpdateAnotherDealingState(value);
            }
        }

        protected int _dealingGold = 0;
        public int DealingGold
        {
            get { return _dealingGold; }
            set
            {
                _dealingGold = value;
                CallOwnerUpdateDealingGold(value);
                if (DealingCharacter != null)
                    DealingCharacter.DealingComponent.CallOwnerUpdateAnotherDealingGold(value);
            }
        }

        protected DealingCharacterItems _dealingItems = new DealingCharacterItems();
        public DealingCharacterItems DealingItems
        {
            get { return _dealingItems; }
            set
            {
                _dealingItems = value;
                CallOwnerUpdateDealingItems(value);
                if (DealingCharacter != null)
                    DealingCharacter.DealingComponent.CallOwnerUpdateAnotherDealingItems(value);
            }
        }

        private BasePlayerCharacterEntity _dealingCharacter;
        public BasePlayerCharacterEntity DealingCharacter
        {
            get
            {
                if (DealingState == DealingState.None && Time.unscaledTime - DealingRequestTime >= CurrentGameInstance.dealingRequestDuration)
                    _dealingCharacter = null;
                return _dealingCharacter;
            }
            set
            {
                _dealingCharacter = value;
                DealingRequestTime = Time.unscaledTime;
            }
        }

        /// <summary>
        /// Action: BasePlayerCharacterEntity anotherCharacter
        /// </summary>
        public event System.Action<BasePlayerCharacterEntity> onRequestDealing;
        /// <summary>
        /// Action: BasePlayerCharacterEntity anotherCharacter
        /// </summary>
        public event System.Action<BasePlayerCharacterEntity> onStartDealing;
        public event System.Action<DealingState> onUpdateDealingState;
        public event System.Action<DealingState> onUpdateAnotherDealingState;
        public event System.Action<int> onUpdateDealingGold;
        public event System.Action<int> onUpdateAnotherDealingGold;
        public event System.Action<DealingCharacterItems> onUpdateDealingItems;
        public event System.Action<DealingCharacterItems> onUpdateAnotherDealingItems;

        public float DealingRequestTime { get; private set; }

        public bool DisableDealing
        {
            get
            {
                return Entity.IsDead() || CurrentGameInstance.disableDealing || BaseGameNetworkManager.CurrentMapInfo.DisableDealing;
            }
        }

        private void Awake()
        {
            Entity.onDead.AddListener(OnDead);
        }

        protected void OnDead()
        {
            if (!IsServer)
                return;
            StopDealing();
        }

        public int GetDealingFee()
        {
            return GameInstance.Singleton.GameplayRule.GetDealingFee(DealingItems, DealingGold);
        }

        public int GetRequiredGold()
        {
            return GetDealingFee() + DealingGold;
        }

        public bool HaveEnoughGold()
        {
            return Entity.Gold >= GetRequiredGold();
        }

        public bool ExchangingDealingItemsWillOverwhelming()
        {
            if (DealingCharacter == null)
                return true;
            List<ItemAmount> itemAmounts = new List<ItemAmount>();
            for (int i = 0; i < DealingItems.Count; ++i)
            {
                if (DealingItems[i].IsEmptySlot()) continue;
                itemAmounts.Add(new ItemAmount()
                {
                    item = DealingItems[i].GetItem(),
                    amount = DealingItems[i].amount,
                });
            }
            return DealingCharacter.IncreasingItemsWillOverwhelming(itemAmounts);
        }

        public void ExchangeDealingItemsAndGold()
        {
            if (DealingCharacter == null)
                return;
            List<CharacterItem> tempDealingItems = new List<CharacterItem>(DealingItems);
            CharacterItem tempNonEquipItem;
            CharacterItem tempDealingItem;
            int i, j;
            for (i = Entity.NonEquipItems.Count - 1; i >= 0; --i)
            {
                tempNonEquipItem = Entity.NonEquipItems[i];
                for (j = tempDealingItems.Count - 1; j >= 0; --j)
                {
                    tempDealingItem = tempDealingItems[j];
                    if (tempDealingItem.id != tempNonEquipItem.id || tempDealingItem.amount > tempNonEquipItem.amount)
                    {
                        // Not a item player wish to offer, or its amount more than in inventory
                        continue;
                    }
                    if (!DealingCharacter.IncreaseItems(tempDealingItem, characterItem => DealingCharacter.OnRewardItem(RewardGivenType.Dealing, characterItem)))
                    {
                        // Another character cannot increase an items
                        continue;
                    }
                    // Reduce item amount when able to increase item to co character
                    tempNonEquipItem.amount -= tempDealingItem.amount;
                    if (tempNonEquipItem.amount == 0)
                    {
                        // Amount is 0, remove it from inventory
                        Entity.NonEquipItems.RemoveOrPlaceEmptySlot(i);
                    }
                    else
                    {
                        // Update amount
                        Entity.NonEquipItems[i] = tempNonEquipItem;
                    }
                    tempDealingItems.RemoveAt(j);
                    break;
                }
            }
            Entity.FillEmptySlots();
            DealingCharacter.FillEmptySlots();
            Entity.Gold -= GetRequiredGold();
            DealingCharacter.Gold = DealingCharacter.Gold.Increase(DealingGold);
            DealingCharacter.OnRewardGold(RewardGivenType.Dealing, DealingGold);
            GameInstance.ServerLogHandlers.LogExchangeDealingItemsAndGold(Entity, DealingCharacter, DealingGold, DealingItems);
        }

        public void ClearDealingData()
        {
            DealingState = DealingState.None;
            DealingGold = 0;
            DealingItems.Clear();
        }

        public void StopDealing()
        {
            if (DealingCharacter == null)
            {
                ClearDealingData();
                return;
            }
            // Set dealing state/data for co player character entity
            DealingCharacter.DealingComponent.ClearDealingData();
            DealingCharacter.DealingComponent.DealingCharacter = null;
            // Set dealing state/data for player character entity
            ClearDealingData();
            DealingCharacter = null;
        }

        public bool CallCmdSendDealingRequest(uint objectId)
        {
            if (DisableDealing)
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_FEATURE_IS_DISABLED);
                return false;
            }
            RPC(CmdSendDealingRequest, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, objectId);
            return true;
        }

        [ServerRpc]
        protected void CmdSendDealingRequest(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DisableDealing)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_FEATURE_IS_DISABLED);
                return;
            }
            if (!Manager.TryGetEntityByObjectId(objectId, out BasePlayerCharacterEntity targetCharacterEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_NOT_FOUND);
                return;
            }
            if (targetCharacterEntity.DealingComponent.DealingCharacter != null)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_DEALING);
                return;
            }
            if (!Entity.IsGameEntityInDistance(targetCharacterEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }
            DealingCharacter = targetCharacterEntity;
            targetCharacterEntity.DealingComponent.DealingCharacter = Entity;
            // Send receive dealing request to player
            DealingCharacter.DealingComponent.CallOwnerReceiveDealingRequest(ObjectId);
#endif
        }

        public bool CallOwnerReceiveDealingRequest(uint objectId)
        {
            RPC(TargetRpcReceiveDealingRequest, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, ConnectionId, objectId);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcReceiveDealingRequest(uint objectId)
        {
            BasePlayerCharacterEntity playerCharacterEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out playerCharacterEntity))
                return;
            if (onRequestDealing != null)
                onRequestDealing.Invoke(playerCharacterEntity);
        }

        public bool CallCmdAcceptDealingRequest()
        {
            RPC(CmdAcceptDealingRequest, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
            return true;
        }

        [ServerRpc]
        protected void CmdAcceptDealingRequest()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DealingCharacter == null)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CANNOT_ACCEPT_DEALING_REQUEST);
                StopDealing();
                return;
            }
            if (!Entity.IsGameEntityInDistance(DealingCharacter))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                StopDealing();
                return;
            }
            // Set dealing state/data for co player character entity
            DealingCharacter.DealingComponent.ClearDealingData();
            DealingCharacter.DealingComponent.DealingState = DealingState.Dealing;
            DealingCharacter.DealingComponent.CallOwnerAcceptedDealingRequest(ObjectId);
            // Set dealing state/data for player character entity
            ClearDealingData();
            DealingState = DealingState.Dealing;
            CallOwnerAcceptedDealingRequest(DealingCharacter.ObjectId);
#endif
        }

        public bool CallCmdDeclineDealingRequest()
        {
            RPC(CmdDeclineDealingRequest, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
            return true;
        }

        [ServerRpc]
        protected void CmdDeclineDealingRequest()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DealingCharacter != null)
                GameInstance.ServerGameMessageHandlers.SendGameMessage(DealingCharacter.ConnectionId, UITextKeys.UI_ERROR_DEALING_REQUEST_DECLINED);
            GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_DEALING_REQUEST_DECLINED);
            StopDealing();
#endif
        }

        public bool CallOwnerAcceptedDealingRequest(uint objectId)
        {
            RPC(TargetRpcAcceptedDealingRequest, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, ConnectionId, objectId);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcAcceptedDealingRequest(uint objectId)
        {
            BasePlayerCharacterEntity playerCharacterEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out playerCharacterEntity))
                return;
            if (!IsServer)
            {
                // Already setup in accept request function, so don't setup again
                DealingCharacter = playerCharacterEntity;
                DealingCharacter.DealingComponent.DealingCharacter = Entity;
            }
            if (onStartDealing != null)
                onStartDealing.Invoke(playerCharacterEntity);
        }

        public bool CallCmdSetDealingItem(string id, int amount)
        {
            RPC(CmdSetDealingItem, id, amount);
            return true;
        }

        [ServerRpc]
        protected void CmdSetDealingItem(string id, int amount)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DealingState != DealingState.Dealing)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DEALING_STATE);
                return;
            }

            int indexOfNonEquipItem = Entity.IndexOfNonEquipItem(id);
            if (indexOfNonEquipItem < 0)
                return;

            CharacterItem nonEquipItem = Entity.NonEquipItems[indexOfNonEquipItem];
            if (nonEquipItem.IsEmptySlot())
                return;

            if (nonEquipItem.GetItem().RestrictDealing)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_ITEM_DEALING_RESTRICTED);
                return;
            }

            DealingCharacterItems dealingItems = DealingItems;
            if (GameInstance.Singleton.dealingItemsLimit > 0 && dealingItems.Count + 1 > GameInstance.Singleton.dealingItemsLimit)
            {
                // Reached limit
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_REACHED_DEALING_ITEMS_LIMIT);
                return;
            }

            for (int i = dealingItems.Count - 1; i >= 0; --i)
            {
                if (id == dealingItems[i].id)
                {
                    dealingItems.RemoveAt(i);
                    break;
                }
            }
            CharacterItem dealingItem = Entity.NonEquipItems[indexOfNonEquipItem].Clone();
            dealingItem.amount = amount;
            dealingItems.Add(dealingItem);
            // Update to clients
            DealingItems = dealingItems;
#endif
        }

        public bool CallCmdSetDealingGold(int dealingGold)
        {
            RPC(CmdSetDealingGold, dealingGold);
            return true;
        }

        [ServerRpc]
        protected void CmdSetDealingGold(int gold)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DealingState != DealingState.Dealing)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DEALING_STATE);
                return;
            }
            if (gold > Entity.Gold)
                gold = Entity.Gold;
            if (gold < 0)
                gold = 0;
            DealingGold = gold;
#endif
        }

        public bool CallCmdLockDealing()
        {
            RPC(CmdLockDealing);
            return true;
        }

        [ServerRpc]
        protected void CmdLockDealing()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DealingState != DealingState.Dealing)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DEALING_STATE);
                return;
            }
            if (!HaveEnoughGold())
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD);
                return;
            }
            DealingState = DealingState.LockDealing;
#endif
        }

        public bool CallCmdConfirmDealing()
        {
            RPC(CmdConfirmDealing);
            return true;
        }

        [ServerRpc]
        protected void CmdConfirmDealing()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DealingState != DealingState.LockDealing || !(DealingCharacter.DealingComponent.DealingState == DealingState.LockDealing || DealingCharacter.DealingComponent.DealingState == DealingState.ConfirmDealing))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DEALING_STATE);
                return;
            }
            if (!HaveEnoughGold())
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD);
                return;
            }
            DealingState = DealingState.ConfirmDealing;
            if (DealingState == DealingState.ConfirmDealing && DealingCharacter.DealingComponent.DealingState == DealingState.ConfirmDealing)
            {
                if (!HaveEnoughGold() || !DealingCharacter.DealingComponent.HaveEnoughGold())
                {
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD);
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(DealingCharacter.ConnectionId, UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD);
                }
                else if (ExchangingDealingItemsWillOverwhelming())
                {
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_ANOTHER_CHARACTER_WILL_OVERWHELMING);
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(DealingCharacter.ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
                }
                else if (DealingCharacter.DealingComponent.ExchangingDealingItemsWillOverwhelming())
                {
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(DealingCharacter.ConnectionId, UITextKeys.UI_ERROR_ANOTHER_CHARACTER_WILL_OVERWHELMING);
                }
                else
                {
                    ExchangeDealingItemsAndGold();
                    DealingCharacter.DealingComponent.ExchangeDealingItemsAndGold();
                }
                StopDealing();
            }
#endif
        }

        public bool CallCmdCancelDealing()
        {
            RPC(CmdCancelDealing);
            return true;
        }

        [ServerRpc]
        protected void CmdCancelDealing()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DealingCharacter != null && DealingCharacter.DealingComponent.DealingState != DealingState.None)
                GameInstance.ServerGameMessageHandlers.SendGameMessage(DealingCharacter.ConnectionId, UITextKeys.UI_ERROR_DEALING_CANCELED);
            if (DealingState != DealingState.None)
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_DEALING_CANCELED);
            StopDealing();
#endif
        }

        public bool CallOwnerUpdateDealingState(DealingState state)
        {
            RPC(TargetRpcUpdateDealingState, ConnectionId, state);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcUpdateDealingState(DealingState dealingState)
        {
            if (onUpdateDealingState != null)
                onUpdateDealingState.Invoke(dealingState);
        }

        public bool CallOwnerUpdateAnotherDealingState(DealingState state)
        {
            RPC(TargetRpcUpdateAnotherDealingState, ConnectionId, state);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcUpdateAnotherDealingState(DealingState dealingState)
        {
            if (onUpdateAnotherDealingState != null)
                onUpdateAnotherDealingState.Invoke(dealingState);
        }

        public bool CallOwnerUpdateDealingGold(int gold)
        {
            RPC(TargetRpcUpdateDealingGold, ConnectionId, gold);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcUpdateDealingGold(int gold)
        {
            if (onUpdateDealingGold != null)
                onUpdateDealingGold.Invoke(gold);
        }

        public bool CallOwnerUpdateAnotherDealingGold(int gold)
        {
            RPC(TargetRpcUpdateAnotherDealingGold, ConnectionId, gold);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcUpdateAnotherDealingGold(int gold)
        {
            if (onUpdateAnotherDealingGold != null)
                onUpdateAnotherDealingGold.Invoke(gold);
        }

        public bool CallOwnerUpdateDealingItems(DealingCharacterItems dealingItems)
        {
            RPC(TargetRpcUpdateDealingItems, ConnectionId, dealingItems);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcUpdateDealingItems(DealingCharacterItems items)
        {
            if (onUpdateDealingItems != null)
                onUpdateDealingItems.Invoke(items);
        }

        public bool CallOwnerUpdateAnotherDealingItems(DealingCharacterItems dealingItems)
        {
            RPC(TargetRpcUpdateAnotherDealingItems, ConnectionId, dealingItems);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcUpdateAnotherDealingItems(DealingCharacterItems items)
        {
            if (onUpdateAnotherDealingItems != null)
                onUpdateAnotherDealingItems.Invoke(items);
        }
    }
}
