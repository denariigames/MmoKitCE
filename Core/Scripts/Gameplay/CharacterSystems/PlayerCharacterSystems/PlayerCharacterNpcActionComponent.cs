using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public partial class PlayerCharacterNpcActionComponent : BaseNetworkedGameEntityComponent<BasePlayerCharacterEntity>
    {
        protected NpcEntity _currentNpcEntity;
        public NpcEntity CurrentNpcEntity
        {
            get => _currentNpcEntity;
            set => _currentNpcEntity = value;
        }

        protected BaseNpcDialog _currentNpcDialog;
        public BaseNpcDialog CurrentNpcDialog
        {
            get => _currentNpcDialog;
            set
            {
                if (value == null)
                {
                    _currentNpcEntity = null;
                    _currentNpcDialog = null;
                    return;
                }
                _currentNpcDialog = value;
            }
        }

        protected NpcEntity _npcEntityAfterSelectRewardItem;
        public NpcEntity NpcEntityAfterSelectRewardItem
        {
            get => _npcEntityAfterSelectRewardItem;
            set => _npcEntityAfterSelectRewardItem = value;
        }

        protected BaseNpcDialog _npcDialogAfterSelectRewardItem;
        public BaseNpcDialog NpcDialogAfterSelectRewardItem
        {
            get => _npcDialogAfterSelectRewardItem;
            set
            {
                if (value == null)
                {
                    _npcEntityAfterSelectRewardItem = null;
                    _npcDialogAfterSelectRewardItem = null;
                    return;
                }
                _npcDialogAfterSelectRewardItem = value;
            }
        }

        public Quest CompletingQuest { get; set; }
        public BaseNpcDialog CurrentDialog { get; protected set; }

        /// <summary>
        /// Action: int questDataId
        /// </summary>
        public event System.Action<Quest> onShowQuestRewardItemSelection;
        /// <summary>
        /// Action: int npcDialogDataId
        /// </summary>
        public event System.Action<BaseNpcDialog> onShowNpcDialog;
        public event System.Action onShowNpcRefineItem;
        public event System.Action onShowNpcDismantleItem;
        public event System.Action onShowNpcRepairItem;

        public async UniTask SetServerCurrentDialog(NpcEntity npc, BaseNpcDialog npcDialog)
        {
            if (!IsServer)
                return;
            if (npcDialog != null && npcDialog.EnterDialogActionsOnServer.Count > 0)
            {
                if (await npcDialog.IsPassEnterDialogActionConditionsOnServer(Entity))
                    await npcDialog.DoEnterDialogActionsOnServer(Entity);
                else
                    npcDialog = null;
            }
            CurrentNpcEntity = npc;
            CurrentNpcDialog = npcDialog;
        }

        public void SetServerDialogAfterSelectRewardItem(NpcEntity npc, BaseNpcDialog npcDialog)
        {
            if (!IsServer)
                return;
            NpcEntityAfterSelectRewardItem = npc;
            NpcDialogAfterSelectRewardItem = npcDialog;
        }

        public void ClearNpcDialogData()
        {
            CurrentNpcDialog = null;
            NpcDialogAfterSelectRewardItem = null;
            CompletingQuest = null;
        }

        public bool AccessingNpcShopDialog(out NpcDialog dialog)
        {
            dialog = null;

            if (Entity.IsDead())
                return false;

            if (CurrentNpcDialog == null)
                return false;

            // Dialog must be built-in shop dialog
            dialog = CurrentNpcDialog as NpcDialog;
            if (dialog == null || dialog.type != NpcDialogType.Shop)
                return false;

            return true;
        }

        #region Networking Functions
        public bool CallCmdNpcActivate(uint objectId)
        {
            if (Entity.IsDead())
                return false;
            RPC(CmdNpcActivate, objectId);
            return true;
        }

        [ServerRpc]
        protected async void CmdNpcActivate(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (!Entity.CanDoActions())
                return;

            _currentNpcEntity = null;

            if (!Manager.TryGetEntityByObjectId(objectId, out NpcEntity npcEntity))
            {
                // Can't find the entity
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INVALID_DATA);
                return;
            }

            if (!Entity.IsGameEntityInDistance(npcEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }

            // Show start dialog
            _currentNpcEntity = npcEntity;
            await SetServerCurrentDialog(npcEntity, npcEntity.StartDialog);

            try
            {
                // Update task
                CharacterQuest tempCharacterQuest;
                Quest tempQuest;
                int tempTaskIndex;
                BaseNpcDialog tempTalkToNpcTaskDialog;
                bool tempCompleteAfterTalked;
                for (int i = 0; i < Entity.Quests.Count; ++i)
                {
                    tempCharacterQuest = Entity.Quests[i];
                    if (tempCharacterQuest.isComplete)
                        continue;
                    tempQuest = tempCharacterQuest.GetQuest();
                    if (tempQuest == null || !tempQuest.HaveToTalkToNpc(Entity, npcEntity, tempCharacterQuest.randomTasksIndex, out tempTaskIndex, out tempTalkToNpcTaskDialog, out tempCompleteAfterTalked))
                        continue;
                    await SetServerCurrentDialog(npcEntity, tempTalkToNpcTaskDialog);
                    if (!tempCharacterQuest.completedTasks.Contains(tempTaskIndex))
                        tempCharacterQuest.completedTasks.Add(tempTaskIndex);
                    Entity.Quests[i] = tempCharacterQuest;
                    if (tempCompleteAfterTalked && tempCharacterQuest.IsAllTasksDone(Entity, out _))
                    {
                        if (tempQuest.selectableRewardItems != null &&
                            tempQuest.selectableRewardItems.Length > 0)
                        {
                            // Show quest reward dialog at client
                            CallOwnerShowQuestRewardItemSelection(tempQuest.DataId);
                            CompletingQuest = tempQuest;
                            await SetServerCurrentDialog(null, null);
                            SetServerDialogAfterSelectRewardItem(npcEntity, tempTalkToNpcTaskDialog);
                        }
                        else
                        {
                            // No selectable reward items, complete the quest immediately
                            if (!Entity.CompleteQuest(tempQuest.DataId, 0))
                                await SetServerCurrentDialog(null, null);
                        }
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR);
                Logging.LogException(LogTag, ex);
                return;
            }

            if (CurrentNpcDialog != null)
                CallOwnerShowNpcDialog(CurrentNpcDialog.DataId);
#endif
        }

        public bool CallOwnerShowQuestRewardItemSelection(int questDataId)
        {
            if (Entity.IsDead())
                return false;
            RPC(TargetRpcShowQuestRewardItemSelection, ConnectionId, questDataId);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcShowQuestRewardItemSelection(int questDataId)
        {
            // Hide npc dialog
            if (onShowNpcDialog != null)
                onShowNpcDialog.Invoke(null);

            if (!GameInstance.Quests.TryGetValue(questDataId, out Quest quest))
                quest = null;

            // Show quest reward dialog
            if (onShowQuestRewardItemSelection != null)
                onShowQuestRewardItemSelection.Invoke(quest);
        }

        public bool CallOwnerShowNpcDialog(int npcDialogDataId)
        {
            if (Entity.IsDead())
                return false;
            RPC(TargetRpcShowNpcDialog, ConnectionId, npcDialogDataId);
            return true;
        }

        [TargetRpc]
        protected async void TargetRpcShowNpcDialog(int npcDialogDataId)
        {
            // Show npc dialog by dataId, if it can't find dialog, it will hide
            if (!GameInstance.NpcDialogs.TryGetValue(npcDialogDataId, out BaseNpcDialog npcDialog))
                npcDialog = null;

            CurrentDialog = npcDialog;

            if (npcDialog != null && npcDialog.EnterDialogActionsOnClient.Count > 0)
            {
                if (await npcDialog.IsPassEnterDialogActionConditionsOnClient(Entity))
                    await npcDialog.DoEnterDialogActionsOnClient(Entity);
                else
                    npcDialog = null;
            }

            if (onShowNpcDialog != null)
                onShowNpcDialog.Invoke(npcDialog);
        }

        public bool CallOwnerShowNpcRefineItem()
        {
            if (Entity.IsDead())
                return false;
            RPC(TargetRpcShowNpcRefineItem, ConnectionId);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcShowNpcRefineItem()
        {
            // Hide npc dialog
            if (onShowNpcDialog != null)
                onShowNpcDialog.Invoke(null);

            // Show refine dialog
            if (onShowNpcRefineItem != null)
                onShowNpcRefineItem.Invoke();
        }

        public bool CallOwnerShowNpcDismantleItem()
        {
            if (Entity.IsDead())
                return false;
            RPC(TargetRpcShowNpcDismantleItem, ConnectionId);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcShowNpcDismantleItem()
        {
            // Hide npc dialog
            if (onShowNpcDialog != null)
                onShowNpcDialog.Invoke(null);

            // Show dismantle dialog
            if (onShowNpcDismantleItem != null)
                onShowNpcDismantleItem.Invoke();
        }

        public bool CallOwnerShowNpcRepairItem()
        {
            if (Entity.IsDead())
                return false;
            RPC(TargetRpcShowNpcRepairItem, ConnectionId);
            return true;
        }

        [TargetRpc]
        protected void TargetRpcShowNpcRepairItem()
        {
            // Hide npc dialog
            if (onShowNpcDialog != null)
                onShowNpcDialog.Invoke(null);

            // Show repair dialog
            if (onShowNpcRepairItem != null)
                onShowNpcRepairItem.Invoke();
        }

        public bool CallCmdSelectNpcDialogMenu(byte menuIndex)
        {
            if (Entity.IsDead())
                return false;
            RPC(CmdSelectNpcDialogMenu, menuIndex);
            return true;
        }

        [ServerRpc]
        protected async void CmdSelectNpcDialogMenu(byte menuIndex)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (CurrentNpcDialog == null)
                return;

            await CurrentNpcDialog.GoToNextDialog(Entity, menuIndex);
            if (CurrentNpcDialog != null)
            {
                // Show Npc dialog on client
                CallOwnerShowNpcDialog(CurrentNpcDialog.DataId);
            }
            else
            {
                // Hide Npc dialog on client
                CallOwnerShowNpcDialog(0);
            }
#endif
        }

        public bool CallCmdHideNpcDialog()
        {
            RPC(CmdHideNpcDialog);
            return true;
        }

        [ServerRpc]
        protected void CmdHideNpcDialog()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            ClearNpcDialogData();
            CallOwnerShowNpcDialog(0);
#endif
        }

        public bool CallCmdBuyNpcItem(int itemIndex, int amount)
        {
            if (amount <= 0 || Entity.IsDead())
                return false;
            RPC(CmdBuyNpcItem, itemIndex, amount);
            return true;
        }

        [ServerRpc]
        protected void CmdBuyNpcItem(int index, int amount)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (amount <= 0 || Entity.IsDead())
                return;

            // Dialog must be built-in shop dialog
            if (!AccessingNpcShopDialog(out NpcDialog dialog))
                return;

            // Found buying item or not?
            NpcSellItem[] sellItems = dialog.sellItems;
            if (sellItems == null || index >= sellItems.Length)
                return;

            // Currencies enough or not?
            NpcSellItem sellItem = sellItems[index];
            if (!CurrentGameplayRule.CurrenciesEnoughToBuyItem(Entity, sellItem, amount))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS);
                return;
            }

            int dataId = sellItem.item.DataId;
            int level = sellItem.level;
            int sellAmount = sellItem.amount > 0 ? sellItem.amount : sellItem.item.MaxStack;

            // Can carry or not?
            if (Entity.IncreasingItemsWillOverwhelming(dataId, amount * sellAmount))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
                return;
            }

            // Decrease currencies
            CurrentGameplayRule.DecreaseCurrenciesWhenBuyItem(Entity, sellItem, amount);

            // Add item to inventory
            Entity.IncreaseItems(CharacterItem.Create(dataId, level, amount * sellAmount), characterItem => Entity.OnRewardItem(RewardGivenType.NpcShop, characterItem));
            Entity.FillEmptySlots();

            GameInstance.ServerLogHandlers.LogBuyNpcItem(Entity, sellItem, amount);
#endif
        }

        public bool CallCmdSelectQuestRewardItem(byte itemIndex)
        {
            if (Entity.IsDead())
                return false;
            RPC(CmdSelectQuestRewardItem, itemIndex);
            return true;
        }

        [ServerRpc]
        protected async void CmdSelectQuestRewardItem(byte index)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (CompletingQuest == null)
                return;

            if (!Entity.CompleteQuest(CompletingQuest.DataId, index))
                return;

            await SetServerCurrentDialog(NpcEntityAfterSelectRewardItem, NpcDialogAfterSelectRewardItem);
            if (CurrentNpcDialog != null)
            {
                // Show Npc dialog on client
                CallOwnerShowNpcDialog(CurrentNpcDialog.DataId);
            }
            else
            {
                // Hide Npc dialog on client
                CallOwnerShowNpcDialog(0);
            }

            // Clear data
            CompletingQuest = null;
            NpcDialogAfterSelectRewardItem = null;
#endif
        }
        #endregion
    }
}
