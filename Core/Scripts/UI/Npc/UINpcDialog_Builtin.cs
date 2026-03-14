using Insthync.DevExtension;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public partial class UINpcDialog
    {
        [Header("Built-in UIs")]
        public GameObject uiDialogRoot;

        [Header("Menu")]
        public UINpcDialogMenu uiMenuPrefab;
        public Transform uiMenuContainer;
        public GameObject uiMenuRoot;

        [Header("Shop Dialog")]
        public UINpcSellItem uiSellItemDialog;
        public UINpcSellItem uiSellItemPrefab;
        public Transform uiSellItemContainer;
        public GameObject uiSellItemRoot;

        [Header("Quest Dialog")]
        public UICharacterQuest uiCharacterQuest;

        [Header("Craft Item Dialog")]
        public UIItemCraft uiCraftItem;

        [Tooltip("Requirement for `SaveRespawnPoint` and `Warp` dialog confirmation")]
        public UINpcDialogConfirmRequirement uiConfirmRequirement;

        [Header("Quest Accept Menu Title")]
        public string messageQuestAccept = "Accept";
        public LanguageData[] messageQuestAcceptTitles;
        public string MessageQuestAccept
        {
            get { return Language.GetText(messageQuestAcceptTitles, messageQuestAccept); }
        }
        public Sprite questAcceptIcon;

        [Header("Quest Decline Menu Title")]
        public string messageQuestDecline = "Decline";
        public LanguageData[] messageQuestDeclineTitles;
        public string MessageQuestDecline
        {
            get { return Language.GetText(messageQuestDeclineTitles, messageQuestDecline); }
        }
        public Sprite questDeclineIcon;

        [Header("Quest Abandon Menu Title")]
        public string messageQuestAbandon = "Abandon";
        public LanguageData[] messageQuestAbandonTitles;
        public string MessageQuestAbandon
        {
            get { return Language.GetText(messageQuestAbandonTitles, messageQuestAbandon); }
        }
        public Sprite questAbandonIcon;

        [Header("Quest Complete Menu Title")]
        public string messageQuestComplete = "Complete";
        public LanguageData[] messageQuestCompleteTitles;
        public string MessageQuestComplete
        {
            get { return Language.GetText(messageQuestCompleteTitles, messageQuestComplete); }
        }
        public Sprite questCompleteIcon;

        [Header("Craft Item Confirm Menu Title")]
        public string messageCraftItemConfirm = "Craft";
        public LanguageData[] messageCraftItemConfirmTitles;
        public string MessageCraftItemConfirm
        {
            get { return Language.GetText(messageCraftItemConfirmTitles, messageCraftItemConfirm); }
        }
        public Sprite craftConfirmIcon;

        [Header("Craft Item Cancel Menu Title")]
        public string messageCraftItemCancel = "Cancel";
        public LanguageData[] messageCraftItemCancelTitles;
        public string MessageCraftItemCancel
        {
            get { return Language.GetText(messageCraftItemCancelTitles, messageCraftItemCancel); }
        }
        public Sprite craftCancelIcon;

        [Header("Save Respawn Point Confirm Menu Title")]
        public bool enableSaveRespawnPointConfirmation = true;
        public string messageSaveRespawnPointConfirm = "Confirm";
        public LanguageData[] messageSaveRespawnPointConfirmTitles;
        public string MessageSaveRespawnPointConfirm
        {
            get { return Language.GetText(messageSaveRespawnPointConfirmTitles, messageSaveRespawnPointConfirm); }
        }
        public Sprite saveRespawnPointConfirmIcon;

        [Header("Save Respawn Point Cancel Menu Title")]
        public string messageSaveRespawnPointCancel = "Cancel";
        public LanguageData[] messageSaveRespawnPointCancelTitles;
        public string MessageSaveRespawnPointCancel
        {
            get { return Language.GetText(messageSaveRespawnPointCancelTitles, messageSaveRespawnPointCancel); }
        }
        public Sprite saveRespawnPointCancelIcon;

        [Header("Warp Confirm Menu Title")]
        public bool enableWarpConfirmation = true;
        public string messageWarpConfirm = "Confirm";
        public LanguageData[] messageWarpConfirmTitles;
        public string MessageWarpConfirm
        {
            get { return Language.GetText(messageWarpConfirmTitles, messageWarpConfirm); }
        }
        public Sprite warpConfirmIcon;

        [Header("Warp Cancel Menu Title")]
        public string messageWarpCancel = "Cancel";
        public LanguageData[] messageWarpCancelTitles;
        public string MessageWarpCancel
        {
            get { return Language.GetText(messageWarpCancelTitles, messageWarpCancel); }
        }
        public Sprite warpCancelIcon;

        [Header("Refine Item Confirm Menu Title")]
        public bool enableRefineItemConfirmation = false;
        public string messageRefineItemConfirm = "Refine Item";
        public LanguageData[] messageRefineItemConfirmTitles;
        public string MessageRefineItemConfirm
        {
            get { return Language.GetText(messageRefineItemConfirmTitles, messageRefineItemConfirm); }
        }
        public Sprite refineItemConfirmIcon;

        [Header("Refine Item Cancel Menu Title")]
        public string messageRefineItemCancel = "Cancel";
        public LanguageData[] messageRefineItemCancelTitles;
        public string MessageRefineItemCancel
        {
            get { return Language.GetText(messageRefineItemCancelTitles, messageRefineItemCancel); }
        }
        public Sprite refineItemCancelIcon;

        [Header("Dismantle Item Confirm Menu Title")]
        public bool enableDismantleItemConfirmation = false;
        public string messageDismantleItemConfirm = "Dismantle Item";
        public LanguageData[] messageDismantleItemConfirmTitles;
        public string MessageDismantleItemConfirm
        {
            get { return Language.GetText(messageDismantleItemConfirmTitles, messageDismantleItemConfirm); }
        }
        public Sprite dismantleItemConfirmIcon;

        [Header("Dismantle Item Cancel Menu Title")]
        public string messageDismantleItemCancel = "Cancel";
        public LanguageData[] messageDismantleItemCancelTitles;
        public string MessageDismantleItemCancel
        {
            get { return Language.GetText(messageDismantleItemCancelTitles, messageDismantleItemCancel); }
        }
        public Sprite dismantleItemCancelIcon;

        [Header("Open Player Storage Confirm Menu Title")]
        public bool enablePlayerStorageConfirmation = false;
        public string messagePlayerStorageConfirm = "Open Storage";
        public LanguageData[] messagePlayerStorageConfirmTitles;
        public string MessagePlayerStorageConfirm
        {
            get { return Language.GetText(messagePlayerStorageConfirmTitles, messagePlayerStorageConfirm); }
        }
        public Sprite playerStorageConfirmIcon;

        [Header("Open Player Storage Cancel Menu Title")]
        public string messagePlayerStorageCancel = "Cancel";
        public LanguageData[] messagePlayerStorageCancelTitles;
        public string MessagePlayerStorageCancel
        {
            get { return Language.GetText(messagePlayerStorageCancelTitles, messagePlayerStorageCancel); }
        }
        public Sprite playerStorageCancelIcon;

        [Header("Open Guild Storage Confirm Menu Title")]
        public bool enableGuildStorageConfirmation = false;
        public string messageGuildStorageConfirm = "Open Storage";
        public LanguageData[] messageGuildStorageConfirmTitles;
        public string MessageGuildStorageConfirm
        {
            get { return Language.GetText(messageGuildStorageConfirmTitles, messageGuildStorageConfirm); }
        }
        public Sprite guildStorageConfirmIcon;

        [Header("Open Guild Storage Cancel Menu Title")]
        public string messageGuildStorageCancel = "Cancel";
        public LanguageData[] messageGuildStorageCancelTitles;
        public string MessageGuildStorageCancel
        {
            get { return Language.GetText(messageGuildStorageCancelTitles, messageGuildStorageCancel); }
        }
        public Sprite guildStorageCancelIcon;

        [Header("Repair Item Confirm Menu Title")]
        public bool enableRepairItemConfirmation = false;
        public string messageRepairItemConfirm = "Repair Item";
        public LanguageData[] messageRepairItemConfirmTitles;
        public string MessageRepairItemConfirm
        {
            get { return Language.GetText(messageRepairItemConfirmTitles, messageRepairItemConfirm); }
        }
        public Sprite repairItemConfirmIcon;

        [Header("Repair Item Cancel Menu Title")]
        public string messageRepairItemCancel = "Cancel";
        public LanguageData[] messageRepairItemCancelTitles;
        public string MessageRepairItemCancel
        {
            get { return Language.GetText(messageRepairItemCancelTitles, messageRepairItemCancel); }
        }
        public Sprite repairItemCancelIcon;

        [Header("Event")]
        public UnityEvent onSwitchToNormalDialog;
        public UnityEvent onSwitchToQuestDialog;
        public UnityEvent onSwitchToSellItemDialog;
        public UnityEvent onSwitchToCraftItemDialog;
        public UnityEvent onSwitchToSaveRespawnPointDialog;
        public UnityEvent onSwitchToWarpDialog;
        public UnityEvent onSwitchToRefineItemDialog;
        public UnityEvent onSwitchToDismantleItemDialog;
        public UnityEvent onSwitchToPlayerStorageDialog;
        public UnityEvent onSwitchToGuildStorageDialog;
        public UnityEvent onSwitchToRepairItemDialog;

        private UIList _cacheMenuList;
        public UIList CacheMenuList
        {
            get
            {
                if (_cacheMenuList == null)
                {
                    _cacheMenuList = gameObject.AddComponent<UIList>();
                    _cacheMenuList.uiPrefab = uiMenuPrefab.gameObject;
                    _cacheMenuList.uiContainer = uiMenuContainer;
                }
                return _cacheMenuList;
            }
        }

        private UIList _cacheSellItemList;
        public UIList CacheSellItemList
        {
            get
            {
                if (_cacheSellItemList == null)
                {
                    _cacheSellItemList = gameObject.AddComponent<UIList>();
                    _cacheSellItemList.uiPrefab = uiSellItemPrefab.gameObject;
                    _cacheSellItemList.uiContainer = uiSellItemContainer;
                }
                return _cacheSellItemList;
            }
        }

        private UINpcSellItemSelectionManager _cacheSellItemSelectionManager;
        public UINpcSellItemSelectionManager CacheSellItemSelectionManager
        {
            get
            {
                if (_cacheSellItemSelectionManager == null)
                    _cacheSellItemSelectionManager = gameObject.GetOrAddComponent<UINpcSellItemSelectionManager>();
                _cacheSellItemSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSellItemSelectionManager;
            }
        }

        [DevExtMethods("Show")]
        protected void Show_Builtin()
        {
            CacheSellItemSelectionManager.eventOnSelected.RemoveListener(OnSelectSellItem);
            CacheSellItemSelectionManager.eventOnSelected.AddListener(OnSelectSellItem);
            CacheSellItemSelectionManager.eventOnDeselected.RemoveListener(OnDeselectSellItem);
            CacheSellItemSelectionManager.eventOnDeselected.AddListener(OnDeselectSellItem);
            if (uiSellItemDialog != null)
                uiSellItemDialog.onHide.AddListener(OnSellItemDialogHide);
        }

        [DevExtMethods("Hide")]
        protected void Hide_Builtin()
        {
            if (uiSellItemDialog != null)
                uiSellItemDialog.onHide.RemoveListener(OnSellItemDialogHide);
            CacheSellItemSelectionManager.DeselectSelectedUI();
        }

        protected void OnSellItemDialogHide()
        {
            CacheSellItemSelectionManager.DeselectSelectedUI();
        }

        protected void OnSelectSellItem(UINpcSellItem ui)
        {
            if (uiSellItemDialog != null)
            {
                uiSellItemDialog.selectionManager = CacheSellItemSelectionManager;
                ui.CloneTo(uiSellItemDialog);
                uiSellItemDialog.Show();
            }
        }

        protected void OnDeselectSellItem(UINpcSellItem ui)
        {
            if (uiSellItemDialog != null)
            {
                uiSellItemDialog.onHide.RemoveListener(OnSellItemDialogHide);
                uiSellItemDialog.Hide();
                uiSellItemDialog.onHide.AddListener(OnSellItemDialogHide);
            }
        }
    }
}
