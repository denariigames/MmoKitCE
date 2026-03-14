using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICharacterCurrencies : UIBase
    {
        [FormerlySerializedAs("uiCurrencyDialog")]
        public UICharacterCurrency uiDialog;
        [FormerlySerializedAs("uiCharacterCurrencyPrefab")]
        public UICharacterCurrency uiPrefab;
        [FormerlySerializedAs("uiCharacterCurrencyContainer")]
        public Transform uiContainer;

        [Header("Options")]
        [Tooltip("If this is `TRUE` it won't update data when controlling character's data changes")]
        public bool notForOwningCharacter;

        public bool NotForOwningCharacter
        {
            get { return notForOwningCharacter; }
            set
            {
                notForOwningCharacter = value;
                RegisterOwningCharacterEvents();
            }
        }

        private UIList _cacheList;
        public UIList CacheList
        {
            get
            {
                if (_cacheList == null)
                {
                    _cacheList = gameObject.AddComponent<UIList>();
                    _cacheList.uiPrefab = uiPrefab.gameObject;
                    _cacheList.uiContainer = uiContainer;
                }
                return _cacheList;
            }
        }

        private UICharacterCurrencySelectionManager _cacheSelectionManager;
        public UICharacterCurrencySelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UICharacterCurrencySelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        public virtual IPlayerCharacterData Character { get; protected set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            _cacheList = null;
            _cacheSelectionManager = null;
            Character = null;
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelected.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelected.AddListener(OnSelect);
            CacheSelectionManager.eventOnDeselected.RemoveListener(OnDeselect);
            CacheSelectionManager.eventOnDeselected.AddListener(OnDeselect);
            if (uiDialog != null)
                uiDialog.onHide.AddListener(OnDialogHide);
            UpdateOwningCharacterData();
            RegisterOwningCharacterEvents();
        }

        protected virtual void OnDisable()
        {
            if (uiDialog != null)
                uiDialog.onHide.RemoveListener(OnDialogHide);
            CacheSelectionManager.DeselectSelectedUI();
            UnregisterOwningCharacterEvents();
        }

        public void RegisterOwningCharacterEvents()
        {
            UnregisterOwningCharacterEvents();
            if (notForOwningCharacter || !GameInstance.PlayingCharacterEntity) return;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            GameInstance.PlayingCharacterEntity.onCurrenciesOperation += OnCurrenciesOperation;
#endif
        }

        public void UnregisterOwningCharacterEvents()
        {
            if (!GameInstance.PlayingCharacterEntity) return;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            GameInstance.PlayingCharacterEntity.onCurrenciesOperation -= OnCurrenciesOperation;
#endif
        }

        private void OnCurrenciesOperation(LiteNetLibSyncListOp operation, int index, CharacterCurrency oldItem, CharacterCurrency newItem)
        {
            UpdateOwningCharacterData();
        }

        public void UpdateOwningCharacterData()
        {
            if (notForOwningCharacter || GameInstance.PlayingCharacter == null) return;
            UpdateData(GameInstance.PlayingCharacter);
        }

        protected virtual void OnDialogHide()
        {
            CacheSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnSelect(UICharacterCurrency ui)
        {
            if (uiDialog != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UICharacterCurrency ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        public void UpdateData(IPlayerCharacterData character)
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            UpdateData(character, character.Currencies);
#endif
        }

        public void UpdateData(IPlayerCharacterData character, IList<CurrencyAmount> currencyAmounts)
        {
            List<CharacterCurrency> characterCurrencies = new List<CharacterCurrency>();
            foreach (CurrencyAmount currencyAmount in currencyAmounts)
            {
                characterCurrencies.Add(CharacterCurrency.Create(currencyAmount.currency, currencyAmount.amount));
            }
            UpdateData(character, characterCurrencies);
        }

        public virtual void UpdateData(IPlayerCharacterData character, IList<CharacterCurrency> characterCurrencies)
        {
            Character = character;
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.characterCurrency.dataId : 0;
            CacheSelectionManager.Clear();

            if (character == null || characterCurrencies == null || characterCurrencies.Count == 0)
            {
                CacheSelectionManager.DeselectSelectedUI();
                if (uiDialog != null)
                    uiDialog.Hide();
                CacheList.HideAll();
                return;
            }

            UICharacterCurrency selectedUI = null;
            UICharacterCurrency tempUI;
            CacheList.Generate(characterCurrencies, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UICharacterCurrency>();
                tempUI.Setup(new UICharacterCurrencyData(data, data.amount), Character, index);
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if (selectedDataId == data.dataId)
                    selectedUI = tempUI;
            });

            if (selectedUI == null)
            {
                CacheSelectionManager.DeselectSelectedUI();
            }
            else
            {
                selectedUI.SelectByManager();
            }
        }
    }
}
