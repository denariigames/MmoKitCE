using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(UICharacterItem))]
    public class ButtonsActivatorForUICharacterItem : MonoBehaviour
    {
        public Button buttonEquip;
        public Button buttonUnEquip;
        public Button buttonUse;
        public Button buttonRefine;
        public Button buttonDismantle;
        public Button buttonRepair;
        public Button buttonSocketEnhance;
        public Button buttonSell;
        public Button buttonOffer;
        public Button buttonMoveToStorage;
        public Button buttonMoveFromStorage;
        public Button buttonDrop;
        public Button buttonPickUpFromContainer;
        public Button buttonAddVendingItem;
        public Button buttonRemoveAmmo;
        private UICharacterItem _ui;

        private void Awake()
        {
            _ui = GetComponent<UICharacterItem>();
            _ui.onSetEquippedData.AddListener(OnSetEquippedData);
            _ui.onSetUnEquippedData.AddListener(OnSetUnEquippedData);
            _ui.onSetUnEquippableData.AddListener(OnSetUnEquippableData);
            _ui.onSetUsableData.AddListener(OnSetUsableData);
            _ui.onSetStorageItemData.AddListener(OnSetStorageItemData);
            _ui.onSetItemsContainerItemData.AddListener(OnSetItemsContainerItemData);
            _ui.onSetUnknowSourceData.AddListener(OnSetUnknowSourceData);
            _ui.onRefineItemDialogAppear.AddListener(OnRefineItemDialogAppear);
            _ui.onRefineItemDialogDisappear.AddListener(OnRefineItemDialogDisappear);
            _ui.onDismantleItemDialogAppear.AddListener(OnDismantleItemDialogAppear);
            _ui.onDismantleItemDialogDisappear.AddListener(OnDismantleItemDialogDisappear);
            _ui.onRepairItemDialogAppear.AddListener(OnRepairItemDialogAppear);
            _ui.onRepairItemDialogDisappear.AddListener(OnRepairItemDialogDisappear);
            _ui.onNpcSellItemDialogAppear.AddListener(OnNpcSellItemDialogAppear);
            _ui.onNpcSellItemDialogDisappear.AddListener(OnNpcSellItemDialogDisappear);
            _ui.onStorageDialogAppear.AddListener(OnStorageDialogAppear);
            _ui.onStorageDialogDisappear.AddListener(OnStorageDialogDisappear);
            _ui.onEnterDealingState.AddListener(OnEnterDealingState);
            _ui.onExitDealingState.AddListener(OnExitDealingState);
            _ui.onStartVendingDialogAppear.AddListener(OnStartVendingDialogAppear);
            _ui.onStartVendingDialogDisappear.AddListener(OnStartVendingDialogDisappear);
            _ui.onSetEquipmentWithAmmo.AddListener(OnSetEquipmentWithAmmo);
            _ui.onSetEquipmentWithoutAmmo.AddListener(OnSetEquipmentWithoutAmmo);
            // Refresh UI data to applies events
            _ui.ForceUpdate();
        }

        public void DeactivateAllButtons()
        {
            if (buttonEquip)
                buttonEquip.gameObject.SetActive(false);
            if (buttonUnEquip)
                buttonUnEquip.gameObject.SetActive(false);
            if (buttonUse)
                buttonUse.gameObject.SetActive(false);
            if (buttonRefine)
                buttonRefine.gameObject.SetActive(false);
            if (buttonDismantle)
                buttonDismantle.gameObject.SetActive(false);
            if (buttonRepair)
                buttonRepair.gameObject.SetActive(false);
            if (buttonSocketEnhance)
                buttonSocketEnhance.gameObject.SetActive(false);
            if (buttonSell)
                buttonSell.gameObject.SetActive(false);
            if (buttonOffer)
                buttonOffer.gameObject.SetActive(false);
            if (buttonMoveToStorage)
                buttonMoveToStorage.gameObject.SetActive(false);
            if (buttonMoveFromStorage)
                buttonMoveFromStorage.gameObject.SetActive(false);
            if (buttonDrop)
                buttonDrop.gameObject.SetActive(false);
            if (buttonAddVendingItem)
                buttonAddVendingItem.gameObject.SetActive(false);
            if (buttonPickUpFromContainer)
                buttonPickUpFromContainer.gameObject.SetActive(false);
            if (buttonRemoveAmmo)
                buttonRemoveAmmo.gameObject.SetActive(false);
        }

        public void OnSetEquippedData()
        {
            DeactivateAllButtons();
            if (buttonUnEquip)
                buttonUnEquip.gameObject.SetActive(true);
            if (buttonRefine)
                buttonRefine.gameObject.SetActive(GameInstance.Singleton.canRefineItemByPlayer);
            if (buttonRepair)
                buttonRepair.gameObject.SetActive(GameInstance.Singleton.canRepairItemByPlayer);
            if (buttonDismantle)
                buttonDismantle.gameObject.SetActive(GameInstance.Singleton.canDismantleItemByPlayer && GameInstance.Singleton.dismantleFilter.Filter(_ui.CharacterItem));
            if (buttonSocketEnhance)
                buttonSocketEnhance.gameObject.SetActive(true);
            if (buttonDrop)
                buttonDrop.gameObject.SetActive(!_ui.Item.RestrictDropping);
        }

        public void OnSetUnEquippedData()
        {
            DeactivateAllButtons();
            if (buttonEquip)
                buttonEquip.gameObject.SetActive(true);
            if (buttonRefine)
                buttonRefine.gameObject.SetActive(GameInstance.Singleton.canRefineItemByPlayer);
            if (buttonDismantle)
                buttonDismantle.gameObject.SetActive(GameInstance.Singleton.canDismantleItemByPlayer && GameInstance.Singleton.dismantleFilter.Filter(_ui.CharacterItem));
            if (buttonRepair)
                buttonRepair.gameObject.SetActive(GameInstance.Singleton.canRepairItemByPlayer);
            if (buttonSocketEnhance)
                buttonSocketEnhance.gameObject.SetActive(true);
            if (buttonDrop)
                buttonDrop.gameObject.SetActive(!_ui.Item.RestrictDropping);
        }

        public void OnSetUnEquippableData()
        {
            DeactivateAllButtons();
            if (buttonDismantle)
                buttonDismantle.gameObject.SetActive(GameInstance.Singleton.canDismantleItemByPlayer && GameInstance.Singleton.dismantleFilter.Filter(_ui.CharacterItem));
            if (buttonDrop)
                buttonDrop.gameObject.SetActive(!_ui.Item.RestrictDropping);
        }

        public void OnSetUsableData()
        {
            DeactivateAllButtons();
            if (buttonUse)
                buttonUse.gameObject.SetActive(true);
            if (buttonDismantle)
                buttonDismantle.gameObject.SetActive(GameInstance.Singleton.canDismantleItemByPlayer && GameInstance.Singleton.dismantleFilter.Filter(_ui.CharacterItem));
            if (buttonDrop)
                buttonDrop.gameObject.SetActive(!_ui.Item.RestrictDropping);
        }

        public void OnSetStorageItemData()
        {
            DeactivateAllButtons();
            if (buttonMoveFromStorage)
                buttonMoveFromStorage.gameObject.SetActive(true);
        }

        public void OnSetItemsContainerItemData()
        {
            DeactivateAllButtons();
            if (buttonPickUpFromContainer)
                buttonPickUpFromContainer.gameObject.SetActive(true);
        }

        public void OnSetUnknowSourceData()
        {
            DeactivateAllButtons();
        }

        public void OnRefineItemDialogAppear()
        {
            if (GameInstance.Singleton.canRefineItemByPlayer)
                return;

            if (buttonRefine)
                buttonRefine.gameObject.SetActive(true);
        }

        public void OnRefineItemDialogDisappear()
        {
            if (GameInstance.Singleton.canRefineItemByPlayer)
                return;

            if (buttonRefine)
                buttonRefine.gameObject.SetActive(false);
        }

        public void OnDismantleItemDialogAppear()
        {
            if (GameInstance.Singleton.canDismantleItemByPlayer)
                return;

            if (buttonDismantle)
                buttonDismantle.gameObject.SetActive(GameInstance.Singleton.dismantleFilter.Filter(_ui.CharacterItem));
        }

        public void OnDismantleItemDialogDisappear()
        {
            if (GameInstance.Singleton.canDismantleItemByPlayer)
                return;

            if (buttonDismantle)
                buttonDismantle.gameObject.SetActive(false);
        }
        public void OnRepairItemDialogAppear()
        {
            if (GameInstance.Singleton.canRepairItemByPlayer)
                return;

            if (buttonRepair)
                buttonRepair.gameObject.SetActive(true);
        }

        public void OnRepairItemDialogDisappear()
        {
            if (GameInstance.Singleton.canRepairItemByPlayer)
                return;

            if (buttonRepair)
                buttonRepair.gameObject.SetActive(false);
        }

        public void OnNpcSellItemDialogAppear()
        {
            if (buttonSell)
                buttonSell.gameObject.SetActive(!_ui.Item.RestrictSelling);
        }

        public void OnNpcSellItemDialogDisappear()
        {
            if (buttonSell)
                buttonSell.gameObject.SetActive(false);
        }

        public void OnStorageDialogAppear()
        {
            if (buttonMoveToStorage)
                buttonMoveToStorage.gameObject.SetActive(true);
        }

        public void OnStorageDialogDisappear()
        {
            if (buttonMoveToStorage)
                buttonMoveToStorage.gameObject.SetActive(false);
        }

        public void OnEnterDealingState()
        {
            if (buttonOffer)
                buttonOffer.gameObject.SetActive(!_ui.Item.RestrictDealing);
        }

        public void OnExitDealingState()
        {
            if (buttonOffer)
                buttonOffer.gameObject.SetActive(false);
        }

        public void OnStartVendingDialogAppear()
        {
            if (buttonAddVendingItem)
                buttonAddVendingItem.gameObject.SetActive(true);
        }

        public void OnStartVendingDialogDisappear()
        {
            if (buttonAddVendingItem)
                buttonAddVendingItem.gameObject.SetActive(false);
        }

        public void OnSetEquipmentWithAmmo()
        {
            if (buttonRemoveAmmo)
                buttonRemoveAmmo.gameObject.SetActive(true);
        }

        public void OnSetEquipmentWithoutAmmo()
        {
            if (buttonRemoveAmmo)
                buttonRemoveAmmo.gameObject.SetActive(false);
        }
    }
}
