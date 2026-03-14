using LiteNetLib;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public class PlayerCharacterVendingComponent : BaseNetworkedGameEntityComponent<BasePlayerCharacterEntity>
    {
        [SerializeField]
        protected SyncFieldVendingData data = new SyncFieldVendingData();

        public VendingData Data => data.Value;

        protected PlayerCharacterVendingComponent _store;
        protected HashSet<PlayerCharacterVendingComponent> _customers = new HashSet<PlayerCharacterVendingComponent>();
        protected VendingItems _items = new VendingItems();

        public event System.Action<VendingItems> onUpdateItems;
        public event System.Action<VendingData> onVendingDataChange;

        public bool DisableVending
        {
            get
            {
                return Entity.IsDead() || CurrentGameInstance.disableVending || BaseGameNetworkManager.CurrentMapInfo.DisableVending;
            }
        }

        private void Awake()
        {
            data.onChange += OnDataChange;
            Entity.onDead.AddListener(OnDead);
        }

        protected override void OnDestroy()
        {
            data.onChange -= OnDataChange;
            base.OnDestroy();
        }

        protected void OnDead()
        {
            if (!IsServer)
                return;
            StopVending();
        }

        protected void OnDataChange(bool isInitial, VendingData oldData, VendingData data)
        {
            if (onVendingDataChange != null)
                onVendingDataChange.Invoke(data);
        }

        public void CallCmdStartVending(string title, StartVendingItems items)
        {
            if (DisableVending)
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_FEATURE_IS_DISABLED);
                return;
            }
            if (items.Count <= 0)
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_NO_START_VENDING_ITEMS);
                return;
            }
            RPC(CmdStartVending, title, items);
        }

        [ServerRpc]
        protected void CmdStartVending(string title, StartVendingItems items)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DisableVending)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_FEATURE_IS_DISABLED);
                return;
            }
            if (items.Count <= 0)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NO_START_VENDING_ITEMS);
                return;
            }
            _items.Clear();
            foreach (StartVendingItem item in items)
            {
                if (string.IsNullOrEmpty(item.id) || item.amount <= 0)
                    continue;
                int index = Entity.NonEquipItems.IndexOf(item.id);
                if (index < 0)
                    continue;
                CharacterItem storeItem = Entity.NonEquipItems[index];
                if (storeItem.IsEmptySlot())
                    continue;
                if (storeItem.GetItem().RestrictDealing)
                    continue;
                storeItem = Entity.NonEquipItems[index].Clone(false);
                storeItem.amount = item.amount;
                _items.Add(new VendingItem()
                {
                    item = storeItem,
                    price = item.price,
                });
                if (CurrentGameInstance.vendingItemsLimit > 0 && _items.Count >= CurrentGameInstance.vendingItemsLimit)
                    break;
            }
            if (_items.Count <= 0)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NO_START_VENDING_ITEMS);
                return;
            }
            data.Value = new VendingData()
            {
                isStarted = true,
                title = title,
            };
#endif
        }

        public void CallCmdStopVending()
        {
            RPC(CmdStopVending);
        }

        [ServerRpc]
        protected void CmdStopVending()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            StopVending();
#endif
        }

        public void StopVending()
        {
            data.Value = new VendingData();
            _items.Clear();
            foreach (PlayerCharacterVendingComponent customer in _customers)
            {
                if (customer == null)
                    continue;
            }
            _customers.Clear();
        }

        public void CallCmdSubscribe(uint objectId)
        {
            RPC(CmdSubscribe, objectId);
        }

        [ServerRpc]
        protected void CmdSubscribe(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            BasePlayerCharacterEntity playerCharacterEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out playerCharacterEntity))
                return;
            if (!playerCharacterEntity.VendingComponent.Data.isStarted)
                return;
            CmdUnsubscribe();
            _store = playerCharacterEntity.VendingComponent;
            _store.AddCustomer(this);
#endif
        }

        protected void AddCustomer(PlayerCharacterVendingComponent customer)
        {
            if (_customers.Add(customer))
                CallTargetRpcNotifyItems(customer.ConnectionId, _items);
        }

        public void CallCmdUnsubscribe()
        {
            RPC(CmdUnsubscribe);
        }

        [ServerRpc]
        protected void CmdUnsubscribe()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (_store == null)
                return;
            _store.RemoveCustomer(this);
            _store = null;
#endif
        }

        protected void RemoveCustomer(PlayerCharacterVendingComponent customer)
        {
            _customers.Remove(customer);
        }

        protected void NotifyItems()
        {
            foreach (PlayerCharacterVendingComponent comp in _customers)
            {
                if (comp == null)
                    continue;
                CallTargetRpcNotifyItems(comp.ConnectionId, _items);
            }
            CallTargetRpcNotifyItems(ConnectionId, _items);
        }

        public void CallTargetRpcNotifyItems(long connectionId, VendingItems items)
        {
            RPC(TargetRpcNotifyItems, connectionId, items);
        }

        [TargetRpc]
        protected void TargetRpcNotifyItems(VendingItems items)
        {
            if (onUpdateItems != null)
                onUpdateItems.Invoke(items);
        }

        public void CallCmdBuyItem(int index)
        {
            RPC(CmdBuyItem, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, index);
        }

        [ServerRpc]
        protected void CmdBuyItem(int index)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            _store.SellItem(this, index);
#endif
        }

        protected void SellItem(PlayerCharacterVendingComponent buyer, int index)
        {
            if (buyer == null || index < 0 || index >= _items.Count)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(buyer.ConnectionId, UITextKeys.UI_ERROR_INVALID_ITEM_INDEX);
                return;
            }
            int price = _items[index].price;
            if (buyer.Entity.Gold < price)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(buyer.ConnectionId, UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD);
                return;
            }
            CharacterItem sellingItem = _items[index].item;
            int inventoryItemIndex = Entity.NonEquipItems.IndexOf(sellingItem.id);
            if (inventoryItemIndex < 0)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(buyer.ConnectionId, UITextKeys.UI_ERROR_INVALID_ITEM_DATA);
                return;
            }
            if (buyer.Entity.IncreasingItemsWillOverwhelming(new List<CharacterItem> { sellingItem }))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(buyer.ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
                return;
            }
            if (!Entity.DecreaseItemsByIndex(inventoryItemIndex, sellingItem.amount, true))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS);
                return;
            }
            buyer.Entity.IncreaseItems(sellingItem, characterItem => buyer.Entity.OnRewardItem(RewardGivenType.Vending, characterItem));

            buyer.Entity.Gold -= price;
            Entity.Gold = Entity.Gold.Increase(price);
            Entity.OnRewardGold(RewardGivenType.Vending, price);
            _items.RemoveAt(index);
            if (_items.Count <= 0)
            {
                // No item to sell anymore
                StopVending();
            }
            else
            {
                // Update items to customer
                NotifyItems();
            }

            GameInstance.ServerLogHandlers.LogBuyVendingItem(buyer.Entity, Entity, sellingItem, price);
            GameInstance.ServerLogHandlers.LogSellVendingItem(Entity, buyer.Entity, sellingItem, price);
        }
    }
}