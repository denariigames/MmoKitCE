using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UIDealing : UISelectionEntry<BasePlayerCharacterEntity>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Gold Amount}")]
        public UILocaleKeySetting formatKeyDealingGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);
        [Tooltip("Format => {0} = {Gold Amount}")]
        public UILocaleKeySetting formatKeyAnotherDealingGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);
        [Tooltip("Format => {0} = {Fee Amount}")]
        [FormerlySerializedAs("formatKeyTax")]
        public UILocaleKeySetting formatKeyFee = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FEE);

        [Header("UI Elements")]
        public UICharacterItem uiDealingItemPrefab;
        public UICharacterItem uiItemDialog;
        [Header("Owning Character UIs")]
        public TextWrapper uiTextDealingGold;
        public Transform uiDealingItemsContainer;
        [Header("Another Character UIs")]
        public UICharacter uiAnotherCharacter;
        public TextWrapper uiTextAnotherDealingGold;
        public Transform uiAnotherDealingItemsContainer;
        [Header("Fee UIs")]
        [FormerlySerializedAs("uiTextTax")]
        public TextWrapper uiTextFee;

        [Header("UI Events")]
        public UnityEvent onStateChangeToDealing = new UnityEvent();
        public UnityEvent onStateChangeToLock = new UnityEvent();
        public UnityEvent onStateChangeToConfirm = new UnityEvent();
        public UnityEvent onAnotherStateChangeToDealing = new UnityEvent();
        public UnityEvent onAnotherStateChangeToLock = new UnityEvent();
        public UnityEvent onAnotherStateChangeToConfirm = new UnityEvent();
        public UnityEvent onBothStateChangeToLock = new UnityEvent();

        public DealingState DealingState { get; private set; }
        public DealingState AnotherDealingState { get; private set; }
        public int DealingGold { get; private set; }
        public int AnotherDealingGold { get; private set; }
        public List<CharacterItem> DealingItems { get; private set; } = new List<CharacterItem>();
        public List<CharacterItem> AnotherDealingItems { get; private set; } = new List<CharacterItem>();
        public int Fee { get; private set; }

        private UIList _cacheDealingItemsList;
        public UIList CacheDealingItemsList
        {
            get
            {
                if (_cacheDealingItemsList == null)
                {
                    _cacheDealingItemsList = gameObject.AddComponent<UIList>();
                    _cacheDealingItemsList.uiPrefab = uiDealingItemPrefab.gameObject;
                    _cacheDealingItemsList.uiContainer = uiDealingItemsContainer;
                }
                return _cacheDealingItemsList;
            }
        }

        private UIList _cacheAnotherDealingItemsList;
        public UIList CacheAnotherDealingItemsList
        {
            get
            {
                if (_cacheAnotherDealingItemsList == null)
                {
                    _cacheAnotherDealingItemsList = gameObject.AddComponent<UIList>();
                    _cacheAnotherDealingItemsList.uiPrefab = uiDealingItemPrefab.gameObject;
                    _cacheAnotherDealingItemsList.uiContainer = uiAnotherDealingItemsContainer;
                }
                return _cacheAnotherDealingItemsList;
            }
        }

        private UICharacterItemSelectionManager _cacheItemSelectionManager;
        public UICharacterItemSelectionManager CacheItemSelectionManager
        {
            get
            {
                if (_cacheItemSelectionManager == null)
                    _cacheItemSelectionManager = gameObject.GetOrAddComponent<UICharacterItemSelectionManager>();
                _cacheItemSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheItemSelectionManager;
            }
        }

        private readonly List<UICharacterItem> _tempDealingItemUIs = new List<UICharacterItem>();
        private readonly List<UICharacterItem> _tempAnotherDealingItemUIs = new List<UICharacterItem>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DealingItems.Clear();
            AnotherDealingItems.Clear();
            uiDealingItemPrefab = null;
            uiItemDialog = null;
            uiTextDealingGold = null;
            uiDealingItemsContainer = null;
            uiAnotherCharacter = null;
            uiTextAnotherDealingGold = null;
            uiAnotherDealingItemsContainer = null;
            uiTextFee = null;
            onStateChangeToDealing?.RemoveAllListeners();
            onStateChangeToDealing = null;
            onStateChangeToLock?.RemoveAllListeners();
            onStateChangeToLock = null;
            onStateChangeToConfirm?.RemoveAllListeners();
            onStateChangeToConfirm = null;
            onAnotherStateChangeToDealing?.RemoveAllListeners();
            onAnotherStateChangeToDealing = null;
            onAnotherStateChangeToLock?.RemoveAllListeners();
            onAnotherStateChangeToLock = null;
            onAnotherStateChangeToConfirm?.RemoveAllListeners();
            onAnotherStateChangeToConfirm = null;
            onBothStateChangeToLock?.RemoveAllListeners();
            onBothStateChangeToLock = null;
            _cacheDealingItemsList = null;
            _cacheAnotherDealingItemsList = null;
            _cacheItemSelectionManager = null;
            _tempDealingItemUIs.Nullify();
            _tempDealingItemUIs?.Clear();
            _tempAnotherDealingItemUIs.Nullify();
            _tempAnotherDealingItemUIs?.Clear();
            _data = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CacheItemSelectionManager.eventOnSelect.RemoveListener(OnSelectCharacterItem);
            CacheItemSelectionManager.eventOnSelect.AddListener(OnSelectCharacterItem);
            CacheItemSelectionManager.eventOnDeselect.RemoveListener(OnDeselectCharacterItem);
            CacheItemSelectionManager.eventOnDeselect.AddListener(OnDeselectCharacterItem);
            if (uiItemDialog != null)
                uiItemDialog.onHide.AddListener(OnItemDialogHide);
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateDealingState += UpdateDealingState;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateDealingGold += UpdateDealingGold;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateDealingItems += UpdateDealingItems;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateAnotherDealingState += UpdateAnotherDealingState;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateAnotherDealingGold += UpdateAnotherDealingGold;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateAnotherDealingItems += UpdateAnotherDealingItems;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (uiItemDialog != null)
                uiItemDialog.onHide.RemoveListener(OnItemDialogHide);
            CacheItemSelectionManager.DeselectSelectedUI();
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateDealingState -= UpdateDealingState;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateDealingGold -= UpdateDealingGold;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateDealingItems -= UpdateDealingItems;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateAnotherDealingState -= UpdateAnotherDealingState;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateAnotherDealingGold -= UpdateAnotherDealingGold;
            GameInstance.PlayingCharacterEntity.DealingComponent.onUpdateAnotherDealingItems -= UpdateAnotherDealingItems;
            GameInstance.PlayingCharacterEntity.DealingComponent.CallCmdCancelDealing();
        }

        protected void OnItemDialogHide()
        {
            CacheItemSelectionManager.DeselectSelectedUI();
        }

        protected void OnSelectCharacterItem(UICharacterItem ui)
        {
            if (ui.Data.characterItem.IsEmptySlot())
            {
                CacheItemSelectionManager.DeselectSelectedUI();
                return;
            }
            if (uiItemDialog != null)
            {
                uiItemDialog.selectionManager = CacheItemSelectionManager;
                uiItemDialog.Setup(ui.Data, GameInstance.PlayingCharacterEntity, -1);
                uiItemDialog.Show();
            }
        }

        protected void OnDeselectCharacterItem(UICharacterItem ui)
        {
            if (uiItemDialog != null)
            {
                uiItemDialog.onHide.RemoveListener(OnItemDialogHide);
                uiItemDialog.Hide();
                uiItemDialog.onHide.AddListener(OnItemDialogHide);
            }
        }

        protected override void UpdateUI()
        {
            // In case that another character is exit or move so far hide the dialog
            if (Data == null)
            {
                Hide();
                return;
            }
        }

        protected override void UpdateData()
        {
            BasePlayerCharacterEntity anotherCharacter = Data;

            if (uiAnotherCharacter != null)
            {
                uiAnotherCharacter.NotForOwningCharacter = true;
                uiAnotherCharacter.Data = anotherCharacter;
            }

            DealingGold = 0;
            AnotherDealingGold = 0;
            DealingItems.Clear();
            AnotherDealingItems.Clear();
            DealingState = DealingState.None;
            AnotherDealingState = DealingState.None;
            UpdateDealingState(DealingState.Dealing);
            UpdateAnotherDealingState(DealingState.Dealing);
            UpdateDealingGold(0);
            UpdateAnotherDealingGold(0);
            UpdateFee(0);
            CacheDealingItemsList.HideAll();
            CacheAnotherDealingItemsList.HideAll();
            CacheItemSelectionManager.DeselectSelectedUI();
            CacheItemSelectionManager.Clear();
        }

        public void UpdateDealingState(DealingState state)
        {
            if (DealingState != state)
            {
                DealingState = state;
                switch (DealingState)
                {
                    case DealingState.None:
                        Hide();
                        break;
                    case DealingState.Dealing:
                        if (onStateChangeToDealing != null)
                            onStateChangeToDealing.Invoke();
                        break;
                    case DealingState.LockDealing:
                        if (onStateChangeToLock != null)
                            onStateChangeToLock.Invoke();
                        break;
                    case DealingState.ConfirmDealing:
                        if (onStateChangeToConfirm != null)
                            onStateChangeToConfirm.Invoke();
                        break;
                }
                if (DealingState == DealingState.LockDealing && AnotherDealingState == DealingState.LockDealing)
                {
                    if (onBothStateChangeToLock != null)
                        onBothStateChangeToLock.Invoke();
                }
            }
        }

        public void UpdateAnotherDealingState(DealingState state)
        {
            if (AnotherDealingState != state)
            {
                AnotherDealingState = state;
                switch (AnotherDealingState)
                {
                    case DealingState.Dealing:
                        if (onAnotherStateChangeToDealing != null)
                            onAnotherStateChangeToDealing.Invoke();
                        break;
                    case DealingState.LockDealing:
                        if (onAnotherStateChangeToLock != null)
                            onAnotherStateChangeToLock.Invoke();
                        break;
                    case DealingState.ConfirmDealing:
                        if (onAnotherStateChangeToConfirm != null)
                            onAnotherStateChangeToConfirm.Invoke();
                        break;
                }
                if (DealingState == DealingState.LockDealing && AnotherDealingState == DealingState.LockDealing)
                {
                    if (onBothStateChangeToLock != null)
                        onBothStateChangeToLock.Invoke();
                }
            }
        }

        public void UpdateDealingGold(int gold)
        {
            if (uiTextDealingGold != null)
            {
                uiTextDealingGold.text = ZString.Format(
                    LanguageManager.GetText(formatKeyDealingGold),
                    gold.ToString("N0"));
            }
            DealingGold = gold;
            UpdateFee(DealingItems, DealingGold);
        }

        public void UpdateAnotherDealingGold(int gold)
        {
            if (uiTextAnotherDealingGold != null)
            {
                uiTextAnotherDealingGold.text = ZString.Format(
                    LanguageManager.GetText(formatKeyAnotherDealingGold),
                    gold.ToString("N0"));
            }
            AnotherDealingGold = gold;
        }

        public void UpdateFee(int fee)
        {
            if (uiTextFee != null)
            {
                uiTextFee.text = ZString.Format(
                    LanguageManager.GetText(formatKeyFee),
                    fee.ToString("N0"));
            }
            Fee = fee;
        }

        public void UpdateFee(List<CharacterItem> dealingItems, int gold)
        {
            UpdateFee(GameInstance.Singleton.GameplayRule.GetDealingFee(dealingItems, gold));
        }

        public void UpdateDealingItems(DealingCharacterItems dealingItems)
        {
            DealingItems.Clear();
            DealingItems.AddRange(dealingItems);
            SetupItemList(CacheDealingItemsList, DealingItems, _tempDealingItemUIs);
            UpdateFee(DealingItems, DealingGold);
        }

        public void UpdateAnotherDealingItems(DealingCharacterItems dealingItems)
        {
            AnotherDealingItems.Clear();
            AnotherDealingItems.AddRange(dealingItems);
            SetupItemList(CacheAnotherDealingItemsList, AnotherDealingItems, _tempAnotherDealingItemUIs);
        }

        private void SetupItemList(UIList list, List<CharacterItem> dealingItems, List<UICharacterItem> uiList)
        {
            CacheItemSelectionManager.DeselectSelectedUI();
            uiList.Clear();

            UICharacterItem tempUiCharacterItem;
            list.Generate(dealingItems, (index, dealingItem, ui) =>
            {
                tempUiCharacterItem = ui.GetComponent<UICharacterItem>();
                if (!dealingItem.IsEmptySlot())
                {
                    tempUiCharacterItem.Setup(new UICharacterItemData(dealingItem, InventoryType.NonEquipItems), GameInstance.PlayingCharacterEntity, -1);
                    tempUiCharacterItem.Show();
                    uiList.Add(tempUiCharacterItem);
                }
                else
                {
                    tempUiCharacterItem.Hide();
                }
            });

            CacheItemSelectionManager.Clear();
            foreach (UICharacterItem tempDealingItemUI in _tempDealingItemUIs)
            {
                CacheItemSelectionManager.Add(tempDealingItemUI);
            }
            foreach (UICharacterItem tempAnotherDealingItemUI in _tempAnotherDealingItemUIs)
            {
                CacheItemSelectionManager.Add(tempAnotherDealingItemUI);
            }
        }

        public void OnClickSetDealingGold()
        {
            UISceneGlobal.Singleton.ShowInputDialog(
                LanguageManager.GetText(UITextKeys.UI_OFFER_GOLD.ToString()),
                LanguageManager.GetText(UITextKeys.UI_OFFER_GOLD_DESCRIPTION.ToString()),
                OnDealingGoldConfirmed,
                0, // Min amount is 0
                GameInstance.PlayingCharacterEntity.Gold, // Max amount is number of gold
                GameInstance.PlayingCharacterEntity.DealingComponent.DealingGold);
        }

        private void OnDealingGoldConfirmed(int amount)
        {
            GameInstance.PlayingCharacterEntity.DealingComponent.CallCmdSetDealingGold(amount);
        }

        public void OnClickLock()
        {
            GameInstance.PlayingCharacterEntity.DealingComponent.CallCmdLockDealing();
        }

        public void OnClickConfirm()
        {
            GameInstance.PlayingCharacterEntity.DealingComponent.CallCmdConfirmDealing();
        }

        public void OnClickCancel()
        {
            Hide();
        }
    }
}
