using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.NPC_DIALOG_FILE, menuName = GameDataMenuConsts.NPC_DIALOG_MENU, order = GameDataMenuConsts.NPC_DIALOG_ORDER)]
    public partial class NpcDialog : BaseNpcDialog
    {
        public const int QUEST_ACCEPT_MENU_INDEX = 0;
        public const int QUEST_DECLINE_MENU_INDEX = 1;
        public const int QUEST_ABANDON_MENU_INDEX = 2;
        public const int QUEST_COMPLETE_MENU_INDEX = 3;
        public const int CONFIRM_MENU_INDEX = 0;
        public const int CANCEL_MENU_INDEX = 1;

        public NpcDialogType type;
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override)]
        public NpcDialogMenu[] menus;
        [Tooltip("Requirement for `SaveRespawnPoint` and `Warp` dialog confirmation")]
        public NpcDialogConfirmRequirement confirmRequirement;

        // Quest
        public Quest quest;
        [Output(backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Override)]
        public BaseNpcDialog questAcceptedDialog;
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog questDeclinedDialog;
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog questAbandonedDialog;
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog questCompletedDialog;

        // Shop
        public NpcSellItem[] sellItems;

        // Craft Item
        public ItemCraft itemCraft = new ItemCraft();
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog craftDoneDialog;
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog craftItemWillOverwhelmingDialog;
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog craftNotMeetRequirementsDialog;
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog craftCancelDialog;

        // Save Spawn Point
        public BaseMapInfo saveRespawnMap;
        public Vector3 saveRespawnPosition;
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog saveRespawnConfirmDialog;
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog saveRespawnCancelDialog;

        // Warp
        public WarpPortalType warpPortalType;
        [Tooltip("Map which character will warp to when use the warp portal, leave this empty to warp character to other position in the same map")]
        public BaseMapInfo warpMap;
        [Tooltip("Position which character will warp to when use the warp portal")]
        public Vector3 warpPosition;
        [Tooltip("If this is `TRUE` it will change character's rotation when warp")]
        public bool warpOverrideRotation;
        [Tooltip("This will be used if `warpOverrideRotation` is `TRUE` to change character's rotation when warp")]
        public Vector3 warpRotation;
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog warpCancelDialog;

        // Refine Item
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog refineItemCancelDialog;

        // Dismantle Item
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog dismantleItemCancelDialog;

        // Storage
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog storageCancelDialog;

        // Repair Item
        [Output(connectionType = ConnectionType.Override)]
        public BaseNpcDialog repairItemCancelDialog;

#if UNITY_EDITOR
        [InspectorButton(nameof(SetAllAmountToItsMaxStack), "Set all amount to its max stack")]
        public bool btnSetAllAmountToItsMaxStack;

        public void SetAllAmountToItsMaxStack()
        {
            if (sellItems == null || sellItems.Length == 0)
                return;
            for (int i = 0; i < sellItems.Length; ++i)
            {
                NpcSellItem npcSellItem = sellItems[i];
                if (npcSellItem.item == null)
                    continue;
                npcSellItem.amount = npcSellItem.item.MaxStack;
                sellItems[i] = npcSellItem;
            }
            EditorUtility.SetDirty(this);
        }
#endif

        public override void PrepareRelatesData()
        {
            // Dialogs from menu
            if (menus != null && menus.Length > 0)
            {
                foreach (NpcDialogMenu menu in menus)
                {
                    GameInstance.AddNpcDialogs(menu.dialog);
                }
            }

            // Quest action dialogs
            GameInstance.AddNpcDialogs(questAcceptedDialog);
            GameInstance.AddNpcDialogs(questDeclinedDialog);
            GameInstance.AddNpcDialogs(questAbandonedDialog);
            GameInstance.AddNpcDialogs(questCompletedDialog);

            // Item craft action dialogs
            GameInstance.AddNpcDialogs(craftDoneDialog);
            GameInstance.AddNpcDialogs(craftItemWillOverwhelmingDialog);
            GameInstance.AddNpcDialogs(craftNotMeetRequirementsDialog);
            GameInstance.AddNpcDialogs(craftCancelDialog);

            // Respawn point save action dialogs
            GameInstance.AddNpcDialogs(saveRespawnConfirmDialog);
            GameInstance.AddNpcDialogs(saveRespawnCancelDialog);

            // Warp action dialogs
            GameInstance.AddNpcDialogs(warpCancelDialog);

            // Refine action dialogs
            GameInstance.AddNpcDialogs(refineItemCancelDialog);

            // Dismantle action dialogs
            GameInstance.AddNpcDialogs(dismantleItemCancelDialog);

            // Storage action dialogs
            GameInstance.AddNpcDialogs(storageCancelDialog);

            // Repair action dialogs
            GameInstance.AddNpcDialogs(repairItemCancelDialog);

            // Items
            if (sellItems != null && sellItems.Length > 0)
            {
                foreach (NpcSellItem sellItem in sellItems)
                {
                    GameInstance.AddItems(sellItem.item);
                    if (sellItem.sellPrices != null && sellItem.sellPrices.Length > 0)
                    {
                        foreach (CurrencyAmount rewardCurrency in sellItem.sellPrices)
                        {
                            GameInstance.AddCurrencies(rewardCurrency.currency);
                        }
                    }
                }
            }
            GameInstance.AddItems(itemCraft.CraftingItem);
            GameInstance.AddItems(itemCraft.RequireItems);
            GameInstance.AddCurrencies(itemCraft.RequireCurrencies);

            // Map infos
            GameInstance.AddMapInfos(saveRespawnMap);
            GameInstance.AddMapInfos(warpMap);

            // Quests
            GameInstance.AddQuests(quest);
        }

        public override UniTask<bool> IsPassMenuCondition(IPlayerCharacterData character)
        {
            if (type == NpcDialogType.Quest)
            {
                if (quest == null || !quest.CanReceiveQuest(character))
                    return UniTask.FromResult(false);
            }
            return UniTask.FromResult(true);
        }

        protected virtual async UniTask RenderNormalUI(UINpcDialog uiNpcDialog, BasePlayerCharacterEntity characterEntity, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToNormalDialog != null)
                uiNpcDialog.onSwitchToNormalDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            for (int i = 0; i < menus.Length; ++i)
            {
                NpcDialogMenu menu = menus[i];
                if (await menu.IsPassConditions(characterEntity))
                {
                    UINpcDialogMenuAction menuAction = new UINpcDialogMenuAction();
                    menuAction.title = menu.Title;
#if UNITY_EDITOR || !UNITY_SERVER
                    menuAction.icon = await menu.GetIcon();
#endif
                    menuAction.menuIndex = i;
                    menuActions.Add(menuAction);
                }
            }
        }

        protected virtual void RenderQuestUI(UINpcDialog uiNpcDialog, BasePlayerCharacterEntity characterEntity, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToQuestDialog != null)
                uiNpcDialog.onSwitchToQuestDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            if (uiNpcDialog.uiCharacterQuest != null)
            {
                if (quest != null)
                {
                    UINpcDialogMenuAction acceptMenuAction = new UINpcDialogMenuAction();
                    UINpcDialogMenuAction declineMenuAction = new UINpcDialogMenuAction();
                    UINpcDialogMenuAction abandonMenuAction = new UINpcDialogMenuAction();
                    UINpcDialogMenuAction completeMenuAction = new UINpcDialogMenuAction();

                    acceptMenuAction.title = uiNpcDialog.MessageQuestAccept;
                    acceptMenuAction.icon = uiNpcDialog.questAcceptIcon;
                    acceptMenuAction.menuIndex = QUEST_ACCEPT_MENU_INDEX;

                    declineMenuAction.title = uiNpcDialog.MessageQuestDecline;
                    declineMenuAction.icon = uiNpcDialog.questDeclineIcon;
                    declineMenuAction.menuIndex = QUEST_DECLINE_MENU_INDEX;

                    abandonMenuAction.title = uiNpcDialog.MessageQuestAbandon;
                    abandonMenuAction.icon = uiNpcDialog.questAbandonIcon;
                    abandonMenuAction.menuIndex = QUEST_ABANDON_MENU_INDEX;

                    completeMenuAction.title = uiNpcDialog.MessageQuestComplete;
                    completeMenuAction.icon = uiNpcDialog.questCompleteIcon;
                    completeMenuAction.menuIndex = QUEST_COMPLETE_MENU_INDEX;

                    CharacterQuest characterQuest;
                    int index = characterEntity.IndexOfQuest(quest.DataId);
                    if (index >= 0)
                    {
                        characterQuest = characterEntity.Quests[index];
                        Quest quest = characterQuest.GetQuest();
                        if (!characterQuest.isComplete)
                        {
                            if (characterQuest.IsAllTasksDoneAndIsCompletingTarget(characterEntity, characterEntity.GetTargetEntity() as NpcEntity))
                                menuActions.Add(completeMenuAction);
                            else if (quest.canAbandon)
                                menuActions.Add(abandonMenuAction);
                        }
                        else if (quest.CanReceiveQuest(characterEntity))
                        {
                            menuActions.Add(acceptMenuAction);
                            menuActions.Add(declineMenuAction);
                        }
                    }
                    else
                    {
                        characterQuest = CharacterQuest.Create(quest);
                        menuActions.Add(acceptMenuAction);
                        menuActions.Add(declineMenuAction);
                    }
                    uiNpcDialog.uiCharacterQuest.Setup(characterQuest, characterEntity, index);
                }
                uiNpcDialog.uiCharacterQuest.Show();
            }
        }

        protected virtual void RenderShopUI(UINpcDialog uiNpcDialog)
        {
            if (uiNpcDialog.onSwitchToSellItemDialog != null)
                uiNpcDialog.onSwitchToSellItemDialog.Invoke();

            if (uiNpcDialog.uiSellItemRoot != null)
                uiNpcDialog.uiSellItemRoot.SetActive(true);

            UINpcSellItem tempUiNpcSellItem;
            uiNpcDialog.CacheSellItemList.Generate(sellItems, (index, sellItem, ui) =>
            {
                tempUiNpcSellItem = ui.GetComponent<UINpcSellItem>();
                tempUiNpcSellItem.Setup(sellItem, index);
                tempUiNpcSellItem.Show();
                uiNpcDialog.CacheSellItemSelectionManager.Add(tempUiNpcSellItem);
            });
        }

        protected virtual void RenderCraftUI(UINpcDialog uiNpcDialog, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToCraftItemDialog != null)
                uiNpcDialog.onSwitchToCraftItemDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            if (uiNpcDialog.uiCraftItem != null)
            {
                BaseItem craftingItem = itemCraft.CraftingItem;
                if (craftingItem != null)
                {
                    UINpcDialogMenuAction confirmMenuAction = new UINpcDialogMenuAction();
                    UINpcDialogMenuAction cancelMenuAction = new UINpcDialogMenuAction();

                    confirmMenuAction.title = uiNpcDialog.MessageCraftItemConfirm;
                    confirmMenuAction.icon = uiNpcDialog.craftConfirmIcon;
                    confirmMenuAction.menuIndex = CONFIRM_MENU_INDEX;

                    cancelMenuAction.title = uiNpcDialog.MessageCraftItemCancel;
                    cancelMenuAction.icon = uiNpcDialog.craftCancelIcon;
                    cancelMenuAction.menuIndex = CANCEL_MENU_INDEX;

                    uiNpcDialog.uiCraftItem.Setup(CrafterType.Npc, null, itemCraft);
                    menuActions.Add(confirmMenuAction);
                    menuActions.Add(cancelMenuAction);
                }
                uiNpcDialog.uiCraftItem.Show();
            }
        }

        protected virtual void RenderSaveRespawnPointUI(UINpcDialog uiNpcDialog, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToSaveRespawnPointDialog != null)
                uiNpcDialog.onSwitchToSaveRespawnPointDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            if (uiNpcDialog.uiConfirmRequirement != null && confirmRequirement.HasConfirmConditions())
            {
                uiNpcDialog.uiConfirmRequirement.Data = confirmRequirement;
                uiNpcDialog.uiConfirmRequirement.Show();
            }

            if (!uiNpcDialog.enableSaveRespawnPointConfirmation || uiNpcDialog.uiMenuPrefab == null)
            {
                GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdSelectNpcDialogMenu(CONFIRM_MENU_INDEX);
                return;
            }

            UINpcDialogMenuAction confirmMenuAction = new UINpcDialogMenuAction();
            UINpcDialogMenuAction cancelMenuAction = new UINpcDialogMenuAction();

            confirmMenuAction.title = uiNpcDialog.MessageSaveRespawnPointConfirm;
            confirmMenuAction.icon = uiNpcDialog.saveRespawnPointConfirmIcon;
            confirmMenuAction.menuIndex = CONFIRM_MENU_INDEX;

            cancelMenuAction.title = uiNpcDialog.MessageSaveRespawnPointCancel;
            cancelMenuAction.icon = uiNpcDialog.saveRespawnPointCancelIcon;
            cancelMenuAction.menuIndex = CANCEL_MENU_INDEX;

            menuActions.Add(confirmMenuAction);
            menuActions.Add(cancelMenuAction);
        }

        protected virtual void RenderWarpUI(UINpcDialog uiNpcDialog, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToWarpDialog != null)
                uiNpcDialog.onSwitchToWarpDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            if (uiNpcDialog.uiConfirmRequirement != null && confirmRequirement.HasConfirmConditions())
            {
                uiNpcDialog.uiConfirmRequirement.Data = confirmRequirement;
                uiNpcDialog.uiConfirmRequirement.Show();
            }

            if (!uiNpcDialog.enableWarpConfirmation || uiNpcDialog.uiMenuPrefab == null)
            {
                GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdSelectNpcDialogMenu(CONFIRM_MENU_INDEX);
                return;
            }

            UINpcDialogMenuAction confirmMenuAction = new UINpcDialogMenuAction();
            UINpcDialogMenuAction cancelMenuAction = new UINpcDialogMenuAction();

            confirmMenuAction.title = uiNpcDialog.MessageWarpConfirm;
            confirmMenuAction.icon = uiNpcDialog.warpConfirmIcon;
            confirmMenuAction.menuIndex = CONFIRM_MENU_INDEX;

            cancelMenuAction.title = uiNpcDialog.MessageWarpCancel;
            cancelMenuAction.icon = uiNpcDialog.warpCancelIcon;
            cancelMenuAction.menuIndex = CANCEL_MENU_INDEX;

            menuActions.Add(confirmMenuAction);
            menuActions.Add(cancelMenuAction);
        }

        protected virtual void RenderRefineItemUI(UINpcDialog uiNpcDialog, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToRefineItemDialog != null)
                uiNpcDialog.onSwitchToRefineItemDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            if (!uiNpcDialog.enableRefineItemConfirmation || uiNpcDialog.uiMenuPrefab == null)
            {
                GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdSelectNpcDialogMenu(CONFIRM_MENU_INDEX);
                return;
            }

            UINpcDialogMenuAction confirmMenuAction = new UINpcDialogMenuAction();
            UINpcDialogMenuAction cancelMenuAction = new UINpcDialogMenuAction();

            confirmMenuAction.title = uiNpcDialog.MessageRefineItemConfirm;
            confirmMenuAction.icon = uiNpcDialog.refineItemConfirmIcon;
            confirmMenuAction.menuIndex = CONFIRM_MENU_INDEX;

            cancelMenuAction.title = uiNpcDialog.MessageRefineItemCancel;
            cancelMenuAction.icon = uiNpcDialog.refineItemCancelIcon;
            cancelMenuAction.menuIndex = CANCEL_MENU_INDEX;

            menuActions.Add(confirmMenuAction);
            menuActions.Add(cancelMenuAction);
        }

        protected virtual void RenderPlayerStorageUI(UINpcDialog uiNpcDialog, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToPlayerStorageDialog != null)
                uiNpcDialog.onSwitchToPlayerStorageDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            if (!uiNpcDialog.enablePlayerStorageConfirmation || uiNpcDialog.uiMenuPrefab == null)
            {
                GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdSelectNpcDialogMenu(CONFIRM_MENU_INDEX);
                return;
            }

            UINpcDialogMenuAction confirmMenuAction = new UINpcDialogMenuAction();
            UINpcDialogMenuAction cancelMenuAction = new UINpcDialogMenuAction();

            confirmMenuAction.title = uiNpcDialog.MessagePlayerStorageConfirm;
            confirmMenuAction.icon = uiNpcDialog.playerStorageConfirmIcon;
            confirmMenuAction.menuIndex = CONFIRM_MENU_INDEX;

            cancelMenuAction.title = uiNpcDialog.MessagePlayerStorageCancel;
            cancelMenuAction.icon = uiNpcDialog.playerStorageCancelIcon;
            cancelMenuAction.menuIndex = CANCEL_MENU_INDEX;

            menuActions.Add(confirmMenuAction);
            menuActions.Add(cancelMenuAction);
        }

        protected virtual void RenderGuildStorageUI(UINpcDialog uiNpcDialog, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToGuildStorageDialog != null)
                uiNpcDialog.onSwitchToGuildStorageDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            if (!uiNpcDialog.enableGuildStorageConfirmation || uiNpcDialog.uiMenuPrefab == null)
            {
                GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdSelectNpcDialogMenu(CONFIRM_MENU_INDEX);
                return;
            }

            UINpcDialogMenuAction confirmMenuAction = new UINpcDialogMenuAction();
            UINpcDialogMenuAction cancelMenuAction = new UINpcDialogMenuAction();

            confirmMenuAction.title = uiNpcDialog.MessageGuildStorageConfirm;
            confirmMenuAction.icon = uiNpcDialog.guildStorageConfirmIcon;
            confirmMenuAction.menuIndex = CONFIRM_MENU_INDEX;

            cancelMenuAction.title = uiNpcDialog.MessageGuildStorageCancel;
            cancelMenuAction.icon = uiNpcDialog.guildStorageCancelIcon;
            cancelMenuAction.menuIndex = CANCEL_MENU_INDEX;

            menuActions.Add(confirmMenuAction);
            menuActions.Add(cancelMenuAction);
        }

        protected virtual void RenderDismantleItemUI(UINpcDialog uiNpcDialog, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToDismantleItemDialog != null)
                uiNpcDialog.onSwitchToDismantleItemDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            if (!uiNpcDialog.enableDismantleItemConfirmation || uiNpcDialog.uiMenuPrefab == null)
            {
                GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdSelectNpcDialogMenu(CONFIRM_MENU_INDEX);
                return;
            }

            UINpcDialogMenuAction confirmMenuAction = new UINpcDialogMenuAction();
            UINpcDialogMenuAction cancelMenuAction = new UINpcDialogMenuAction();

            confirmMenuAction.title = uiNpcDialog.MessageDismantleItemConfirm;
            confirmMenuAction.icon = uiNpcDialog.dismantleItemConfirmIcon;
            confirmMenuAction.menuIndex = CONFIRM_MENU_INDEX;

            cancelMenuAction.title = uiNpcDialog.MessageDismantleItemCancel;
            cancelMenuAction.icon = uiNpcDialog.dismantleItemCancelIcon;
            cancelMenuAction.menuIndex = CANCEL_MENU_INDEX;

            menuActions.Add(confirmMenuAction);
            menuActions.Add(cancelMenuAction);
        }
        
        protected virtual void RenderRepairItemUI(UINpcDialog uiNpcDialog, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.onSwitchToRepairItemDialog != null)
                uiNpcDialog.onSwitchToRepairItemDialog.Invoke();

            if (uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(true);

            if (!uiNpcDialog.enableRepairItemConfirmation || uiNpcDialog.uiMenuPrefab == null)
            {
                GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdSelectNpcDialogMenu(CONFIRM_MENU_INDEX);
                return;
            }

            UINpcDialogMenuAction confirmMenuAction = new UINpcDialogMenuAction();
            UINpcDialogMenuAction cancelMenuAction = new UINpcDialogMenuAction();

            confirmMenuAction.title = uiNpcDialog.MessageRepairItemConfirm;
            confirmMenuAction.icon = uiNpcDialog.repairItemConfirmIcon;
            confirmMenuAction.menuIndex = CONFIRM_MENU_INDEX;

            cancelMenuAction.title = uiNpcDialog.MessageRepairItemCancel;
            cancelMenuAction.icon = uiNpcDialog.repairItemCancelIcon;
            cancelMenuAction.menuIndex = CANCEL_MENU_INDEX;

            menuActions.Add(confirmMenuAction);
            menuActions.Add(cancelMenuAction);
        }

        protected virtual void RenderMenuUI(UINpcDialog uiNpcDialog, List<UINpcDialogMenuAction> menuActions)
        {
            if (uiNpcDialog.uiMenuPrefab == null)
            {
                // No menus
                return;
            }

            if (uiNpcDialog.uiMenuRoot != null)
                uiNpcDialog.uiMenuRoot.SetActive(menuActions.Count > 0);

            UINpcDialogMenu tempUiNpcDialogMenu;
            uiNpcDialog.CacheMenuList.Generate(menuActions, (index, menuAction, ui) =>
            {
                tempUiNpcDialogMenu = ui.GetComponent<UINpcDialogMenu>();
                tempUiNpcDialogMenu.Data = menuAction;
                tempUiNpcDialogMenu.uiNpcDialog = uiNpcDialog;
                tempUiNpcDialogMenu.Show();
            });
        }

        public override async UniTask RenderUI(UINpcDialog uiNpcDialog)
        {
            BasePlayerCharacterEntity characterEntity = GameInstance.PlayingCharacterEntity;

            if (type != NpcDialogType.Normal && uiNpcDialog.uiDialogRoot != null)
                uiNpcDialog.uiDialogRoot.SetActive(false);

            if (type != NpcDialogType.Shop && uiNpcDialog.uiSellItemRoot != null)
                uiNpcDialog.uiSellItemRoot.SetActive(false);

            if (type != NpcDialogType.Shop && uiNpcDialog.uiSellItemDialog != null)
                uiNpcDialog.uiSellItemDialog.Hide();

            if (type != NpcDialogType.Quest && uiNpcDialog.uiCharacterQuest != null)
                uiNpcDialog.uiCharacterQuest.Hide();

            if (type != NpcDialogType.CraftItem && uiNpcDialog.uiCraftItem != null)
                uiNpcDialog.uiCraftItem.Hide();

            List<UINpcDialogMenuAction> menuActions = new List<UINpcDialogMenuAction>();
            switch (type)
            {
                case NpcDialogType.Normal:
                    await RenderNormalUI(uiNpcDialog, characterEntity, menuActions);
                    break;
                case NpcDialogType.Quest:
                    RenderQuestUI(uiNpcDialog, characterEntity, menuActions);
                    break;
                case NpcDialogType.Shop:
                    RenderShopUI(uiNpcDialog);
                    break;
                case NpcDialogType.CraftItem:
                    RenderCraftUI(uiNpcDialog, menuActions);
                    break;
                case NpcDialogType.SaveRespawnPoint:
                    RenderSaveRespawnPointUI(uiNpcDialog, menuActions);
                    break;
                case NpcDialogType.Warp:
                    RenderWarpUI(uiNpcDialog, menuActions);
                    break;
                case NpcDialogType.RefineItem:
                    RenderRefineItemUI(uiNpcDialog, menuActions);
                    break;
                case NpcDialogType.PlayerStorage:
                    RenderPlayerStorageUI(uiNpcDialog, menuActions);
                    break;
                case NpcDialogType.GuildStorage:
                    RenderGuildStorageUI(uiNpcDialog, menuActions);
                    break;
                case NpcDialogType.DismantleItem:
                    RenderDismantleItemUI(uiNpcDialog, menuActions);
                    break;
                case NpcDialogType.RepairItem:
                    RenderRepairItemUI(uiNpcDialog, menuActions);
                    break;
            }
            RenderMenuUI(uiNpcDialog, menuActions);
        }

        public override void UnrenderUI(UINpcDialog uiNpcDialog)
        {
            if (uiNpcDialog.uiMenuRoot != null)
                uiNpcDialog.uiMenuRoot.SetActive(false);

            if (uiNpcDialog.uiSellItemRoot != null)
                uiNpcDialog.uiSellItemRoot.SetActive(false);

            if (uiNpcDialog.uiSellItemDialog != null)
                uiNpcDialog.uiSellItemDialog.Hide();

            if (uiNpcDialog.uiCharacterQuest != null)
                uiNpcDialog.uiCharacterQuest.Hide();

            if (uiNpcDialog.uiCraftItem != null)
                uiNpcDialog.uiCraftItem.Hide();

            if (uiNpcDialog.uiConfirmRequirement != null)
                uiNpcDialog.uiConfirmRequirement.Hide();
        }

        public override bool ValidateDialog(BasePlayerCharacterEntity characterEntity)
        {
            switch (type)
            {
                case NpcDialogType.Quest:
                    if (quest == null)
                    {
                        // Validate quest data
                        Logging.LogWarning(ToString(), "Quest dialog's quest is empty");
                        return false;
                    }
                    break;
                case NpcDialogType.CraftItem:
                    if (itemCraft.CraftingItem == null)
                    {
                        // Validate crafting item
                        Logging.LogWarning(ToString(), "Item craft dialog's crafting item is empty");
                        return false;
                    }
                    break;
                case NpcDialogType.SaveRespawnPoint:
                    if (saveRespawnMap == null)
                    {
                        // Validate quest data
                        Logging.LogWarning(ToString(), "Save respawn point dialog's save respawn map is empty");
                        return false;
                    }
                    break;
                case NpcDialogType.Warp:
                    if (warpMap == null)
                    {
                        // Validate quest data
                        Logging.LogWarning(ToString(), "Warp dialog's warp map is empty");
                        return false;
                    }
                    break;
            }
            return true;
        }

        public override async UniTask GoToNextDialog(BasePlayerCharacterEntity characterEntity, byte menuIndex)
        {
            NpcEntity currentNpc = characterEntity.NpcActionComponent.CurrentNpcEntity;
            characterEntity.NpcActionComponent.ClearNpcDialogData();
            // This dialog is current NPC dialog
            switch (type)
            {
                case NpcDialogType.Normal:
                    if (menuIndex >= menus.Length)
                    {
                        // Invalid menu, so no next dialog, so return itself
                        await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(this, characterEntity));
                        return;
                    }
                    // Changing current npc dialog
                    NpcDialogMenu selectedMenu = menus[menuIndex];
                    if (!await selectedMenu.IsPassConditions(characterEntity) || selectedMenu.dialog == null || selectedMenu.isCloseMenu)
                    {
                        // Close dialog, so return null
                        return;
                    }
                    await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(selectedMenu.dialog, characterEntity));
                    return;
                case NpcDialogType.Quest:
                    switch (menuIndex)
                    {
                        case QUEST_ACCEPT_MENU_INDEX:
                            characterEntity.AcceptQuest(quest.DataId);
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(questAcceptedDialog, characterEntity));
                            return;
                        case QUEST_DECLINE_MENU_INDEX:
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(questDeclinedDialog, characterEntity));
                            return;
                        case QUEST_ABANDON_MENU_INDEX:
                            characterEntity.AbandonQuest(quest.DataId);
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(questAbandonedDialog, characterEntity));
                            return;
                        case QUEST_COMPLETE_MENU_INDEX:
                            if (quest.selectableRewardItems != null &&
                                quest.selectableRewardItems.Length > 0)
                            {
                                // Show quest reward dialog at client
                                characterEntity.NpcActionComponent.CallOwnerShowQuestRewardItemSelection(quest.DataId);
                                characterEntity.NpcActionComponent.SetServerDialogAfterSelectRewardItem(currentNpc, GetValidatedDialogOrNull(questCompletedDialog, characterEntity));
                                characterEntity.NpcActionComponent.CompletingQuest = quest;
                            }
                            else
                            {
                                // No selectable reward items, complete the quest immediately
                                if (characterEntity.CompleteQuest(quest.DataId, 0))
                                    await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(questCompletedDialog, characterEntity));
                            }
                            return;
                    }
                    return;
                case NpcDialogType.CraftItem:
                    switch (menuIndex)
                    {
                        case CONFIRM_MENU_INDEX:
                            UITextKeys gameMessage;
                            if (itemCraft.CanCraft(characterEntity, out gameMessage))
                            {
                                itemCraft.CraftItem(characterEntity);
                                await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(craftDoneDialog, characterEntity));
                                return;
                            }
                            // Cannot craft item
                            switch (gameMessage)
                            {
                                case UITextKeys.UI_ERROR_WILL_OVERWHELMING:
                                    await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(craftItemWillOverwhelmingDialog, characterEntity));
                                    return;
                                default:
                                    await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(craftNotMeetRequirementsDialog, characterEntity));
                                    return;
                            }
                        case CANCEL_MENU_INDEX:
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(craftCancelDialog, characterEntity));
                            return;
                    }
                    return;
                case NpcDialogType.SaveRespawnPoint:
                    switch (menuIndex)
                    {
                        case CONFIRM_MENU_INDEX:
                            if (PassConfirmConditions(characterEntity, out UITextKeys errorMessage))
                            {
#if !DISABLE_DIFFER_MAP_RESPAWNING
                                characterEntity.RespawnMapName = saveRespawnMap.Id;
                                characterEntity.RespawnPosition = saveRespawnPosition;
#endif
                                await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(saveRespawnConfirmDialog, characterEntity));
                            }
                            else
                            {
                                GameInstance.ServerGameMessageHandlers.SendGameMessage(characterEntity.ConnectionId, errorMessage);
                            }
                            return;
                        case CANCEL_MENU_INDEX:
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(saveRespawnCancelDialog, characterEntity));
                            return;
                    }
                    return;
                case NpcDialogType.Warp:
                    switch (menuIndex)
                    {
                        case CONFIRM_MENU_INDEX:
                            if (PassConfirmConditions(characterEntity, out UITextKeys errorMessage))
                            {
                                BaseGameNetworkManager.Singleton.WarpCharacter(warpPortalType, characterEntity, warpMap.Id, warpPosition, warpOverrideRotation, warpRotation);
                            }
                            else
                            {
                                GameInstance.ServerGameMessageHandlers.SendGameMessage(characterEntity.ConnectionId, errorMessage);
                            }
                            return;
                        case CANCEL_MENU_INDEX:
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(warpCancelDialog, characterEntity));
                            return;
                    }
                    return;
                case NpcDialogType.RefineItem:
                    switch (menuIndex)
                    {
                        case CONFIRM_MENU_INDEX:
                            characterEntity.NpcActionComponent.CallOwnerShowNpcRefineItem();
                            return;
                        case CANCEL_MENU_INDEX:
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(refineItemCancelDialog, characterEntity));
                            return;
                    }
                    return;
                case NpcDialogType.PlayerStorage:
                    switch (menuIndex)
                    {
                        case CONFIRM_MENU_INDEX:
                            if (characterEntity.GetStorageId(StorageType.Player, 0, out StorageId storageId))
                                GameInstance.ServerStorageHandlers.OpenStorage(characterEntity.ConnectionId, characterEntity, currentNpc, storageId);
                            else
                                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE);
                            return;
                        case CANCEL_MENU_INDEX:
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(storageCancelDialog, characterEntity));
                            return;
                    }
                    return;
                case NpcDialogType.GuildStorage:
                    switch (menuIndex)
                    {
                        case CONFIRM_MENU_INDEX:
                            if (characterEntity.GetStorageId(StorageType.Guild, 0, out StorageId storageId))
                                GameInstance.ServerStorageHandlers.OpenStorage(characterEntity.ConnectionId, characterEntity, currentNpc, storageId);
                            else
                                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CANNOT_ACCESS_STORAGE);
                            return;
                        case CANCEL_MENU_INDEX:
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(storageCancelDialog, characterEntity));
                            return;
                    }
                    return;
                case NpcDialogType.DismantleItem:
                    switch (menuIndex)
                    {
                        case CONFIRM_MENU_INDEX:
                            characterEntity.NpcActionComponent.CallOwnerShowNpcDismantleItem();
                            return;
                        case CANCEL_MENU_INDEX:
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(dismantleItemCancelDialog, characterEntity));
                            return;
                    }
                    return;
                case NpcDialogType.RepairItem:
                    switch (menuIndex)
                    {
                        case CONFIRM_MENU_INDEX:
                            characterEntity.NpcActionComponent.CallOwnerShowNpcRepairItem();
                            return;
                        case CANCEL_MENU_INDEX:
                            await characterEntity.NpcActionComponent.SetServerCurrentDialog(currentNpc, GetValidatedDialogOrNull(repairItemCancelDialog, characterEntity));
                            return;
                    }
                    return;
            }
        }

        protected override void SetDialogByPort(NodePort from, NodePort to)
        {
            if (from.node != this)
                return;

            BaseNpcDialog dialog = null;
            if (to != null && to.node != null)
                dialog = to.node as BaseNpcDialog;

            int arrayIndex;
            if (from.fieldName.Contains("menus ") && int.TryParse(from.fieldName.Split(' ')[1], out arrayIndex) && arrayIndex < menus.Length)
                menus[arrayIndex].dialog = dialog;

            if (from.fieldName.Equals(nameof(questAcceptedDialog)))
                questAcceptedDialog = dialog;

            if (from.fieldName.Equals(nameof(questDeclinedDialog)))
                questDeclinedDialog = dialog;

            if (from.fieldName.Equals(nameof(questAbandonedDialog)))
                questAbandonedDialog = dialog;

            if (from.fieldName.Equals(nameof(questCompletedDialog)))
                questCompletedDialog = dialog;

            if (from.fieldName.Equals(nameof(craftDoneDialog)))
                craftDoneDialog = dialog;

            if (from.fieldName.Equals(nameof(craftItemWillOverwhelmingDialog)))
                craftItemWillOverwhelmingDialog = dialog;

            if (from.fieldName.Equals(nameof(craftNotMeetRequirementsDialog)))
                craftNotMeetRequirementsDialog = dialog;

            if (from.fieldName.Equals(nameof(craftCancelDialog)))
                craftCancelDialog = dialog;

            if (from.fieldName.Equals(nameof(saveRespawnConfirmDialog)))
                saveRespawnConfirmDialog = dialog;

            if (from.fieldName.Equals(nameof(saveRespawnCancelDialog)))
                saveRespawnCancelDialog = dialog;

            if (from.fieldName.Equals(nameof(warpCancelDialog)))
                warpCancelDialog = dialog;

            if (from.fieldName.Equals(nameof(refineItemCancelDialog)))
                refineItemCancelDialog = dialog;

            if (from.fieldName.Equals(nameof(storageCancelDialog)))
                storageCancelDialog = dialog;

            if (from.fieldName.Equals(nameof(repairItemCancelDialog)))
                repairItemCancelDialog = dialog;
        }

        public override bool IsShop
        {
            get { return type == NpcDialogType.Shop; }
        }

        public bool PassConfirmConditions(IPlayerCharacterData character, out UITextKeys gameMessage)
        {
            if (character.Gold < confirmRequirement.gold)
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD;
                return false;
            }
            using (CollectionPool<Dictionary<Currency, int>, KeyValuePair<Currency, int>>.Get(out Dictionary<Currency, int> requireCurrencyAmounts))
            {
                GameDataHelpers.CombineCurrencies(confirmRequirement.currencyAmounts, requireCurrencyAmounts, 1f);
                if (!character.HasEnoughCurrencyAmounts(requireCurrencyAmounts, out gameMessage, out _))
                    return false;
            }
            using (CollectionPool<Dictionary<BaseItem, int>, KeyValuePair<BaseItem, int>>.Get(out Dictionary<BaseItem, int> requireItemAmounts))
            {
                GameDataHelpers.CombineItems(confirmRequirement.itemAmounts, requireItemAmounts);
                if (!character.HasEnoughNonEquipItemAmounts(requireItemAmounts, out gameMessage, out _))
                    return false;
            }
            gameMessage = UITextKeys.NONE;
            character.Gold -= confirmRequirement.gold;
            character.DecreaseCurrencies(confirmRequirement.currencyAmounts);
            character.DecreaseItems(confirmRequirement.itemAmounts);
            return true;
        }
    }
}
