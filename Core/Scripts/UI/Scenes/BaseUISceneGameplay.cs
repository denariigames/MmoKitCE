using UnityEngine;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public abstract partial class BaseUISceneGameplay : MonoBehaviour, IItemUIVisibilityManager, IItemsContainerUIVisibilityManager
    {
        public static BaseUISceneGameplay Singleton { get; private set; }

        [Header("Combat Text")]
        public bool instantiateCombatTextToWorldTransform;
        public Transform combatTextTransform;
        public UICombatText uiCombatTextMiss;
        public UICombatText uiCombatTextNormalDamage;
        public UICombatText uiCombatTextCriticalDamage;
        public UICombatText uiCombatTextBlockedDamage;
        public UICombatText uiCombatTextHpRecovery;
        public UICombatText uiCombatTextMpRecovery;
        public UICombatText uiCombatTextStaminaRecovery;
        public UICombatText uiCombatTextFoodRecovery;
        public UICombatText uiCombatTextWaterRecovery;
        public UICombatText uiCombatTextHpDecrease;
        public UICombatText uiCombatTextMpDecrease;
        public UICombatText uiCombatTextStaminaDecrease;
        public UICombatText uiCombatTextFoodDecrease;
        public UICombatText uiCombatTextWaterDecrease;
        public UICombatText uiCombatTextHpLeech;
        public UICombatText uiCombatTextMpLeech;
        public UICombatText uiCombatTextStaminaLeech;
        public UICombatText uiCombatTextFallDamage;
        public UICombatText uiCombatTextImmune;

        private readonly Dictionary<DamageableEntity, Queue<KeyValuePair<CombatAmountType, int>>> _spawningCombatTexts = new Dictionary<DamageableEntity, Queue<KeyValuePair<CombatAmountType, int>>>();
        private readonly Dictionary<DamageableEntity, float> _spawningCombatTextTimes = new Dictionary<DamageableEntity, float>();

        protected virtual void Awake()
        {
            Singleton = this;
        }

        protected virtual void OnDestroy()
        {
            OnControllerDesetup(null);
            combatTextTransform = null;
            uiCombatTextMiss = null;
            uiCombatTextNormalDamage = null;
            uiCombatTextCriticalDamage = null;
            uiCombatTextBlockedDamage = null;
            uiCombatTextHpRecovery = null;
            uiCombatTextMpRecovery = null;
            uiCombatTextStaminaRecovery = null;
            uiCombatTextFoodRecovery = null;
            uiCombatTextWaterRecovery = null;
            uiCombatTextHpDecrease = null;
            uiCombatTextMpDecrease = null;
            uiCombatTextStaminaDecrease = null;
            uiCombatTextFoodDecrease = null;
            uiCombatTextWaterDecrease = null;
            uiCombatTextHpLeech = null;
            uiCombatTextMpLeech = null;
            uiCombatTextStaminaLeech = null;
            uiCombatTextFallDamage = null;
            uiCombatTextImmune = null;
            _spawningCombatTexts?.Clear();
            _spawningCombatTextTimes?.Clear();
        }

        protected virtual void OnEnable()
        {
            GameInstance.ItemUIVisibilityManager = this;
            GameInstance.ItemsContainerUIVisibilityManager = this;
        }

        protected virtual void OnDisable()
        {
            GameInstance.ItemUIVisibilityManager = null;
            GameInstance.ItemsContainerUIVisibilityManager = null;
        }

        protected virtual void Update()
        {
            float currentTime = Time.unscaledTime;
            KeyValuePair<CombatAmountType, int> combatTextData;
            foreach (DamageableEntity damageableEntity in _spawningCombatTexts.Keys)
            {
                if (damageableEntity == null || _spawningCombatTexts[damageableEntity].Count == 0)
                    continue;

                if (!_spawningCombatTextTimes.ContainsKey(damageableEntity))
                    _spawningCombatTextTimes[damageableEntity] = currentTime;

                if (currentTime - _spawningCombatTextTimes[damageableEntity] >= 0.1f)
                {
                    _spawningCombatTextTimes[damageableEntity] = currentTime;
                    combatTextData = _spawningCombatTexts[damageableEntity].Dequeue();
                    SpawnCombatText(damageableEntity.CombatTextTransform, combatTextData.Key, combatTextData.Value);
                }
            }
        }

        public void PrepareCombatText(DamageableEntity damageableEntity, CombatAmountType combatAmountType, int amount)
        {
            if (Vector3.Distance(GameInstance.PlayingCharacterEntity.EntityTransform.position, damageableEntity.EntityTransform.position) > GameInstance.Singleton.combatTextDistance)
                return;

            if (!_spawningCombatTexts.ContainsKey(damageableEntity))
                _spawningCombatTexts[damageableEntity] = new Queue<KeyValuePair<CombatAmountType, int>>();
            _spawningCombatTexts[damageableEntity].Enqueue(new KeyValuePair<CombatAmountType, int>(combatAmountType, amount));
        }

        public void SpawnCombatText(Transform followingTransform, CombatAmountType combatAmountType, int amount)
        {
            switch (combatAmountType)
            {
                case CombatAmountType.Miss:
                    SpawnCombatText(followingTransform, uiCombatTextMiss, amount);
                    break;
                case CombatAmountType.NormalDamage:
                    SpawnCombatText(followingTransform, uiCombatTextNormalDamage, amount);
                    break;
                case CombatAmountType.CriticalDamage:
                    SpawnCombatText(followingTransform, uiCombatTextCriticalDamage, amount);
                    break;
                case CombatAmountType.BlockedDamage:
                    SpawnCombatText(followingTransform, uiCombatTextBlockedDamage, amount);
                    break;
                case CombatAmountType.HpRecovery:
                    SpawnCombatText(followingTransform, uiCombatTextHpRecovery, amount);
                    break;
                case CombatAmountType.MpRecovery:
                    SpawnCombatText(followingTransform, uiCombatTextMpRecovery, amount);
                    break;
                case CombatAmountType.StaminaRecovery:
                    SpawnCombatText(followingTransform, uiCombatTextStaminaRecovery, amount);
                    break;
                case CombatAmountType.FoodRecovery:
                    SpawnCombatText(followingTransform, uiCombatTextFoodRecovery, amount);
                    break;
                case CombatAmountType.WaterRecovery:
                    SpawnCombatText(followingTransform, uiCombatTextWaterRecovery, amount);
                    break;
                case CombatAmountType.HpDecrease:
                    SpawnCombatText(followingTransform, uiCombatTextHpDecrease, amount);
                    break;
                case CombatAmountType.MpDecrease:
                    SpawnCombatText(followingTransform, uiCombatTextMpDecrease, amount);
                    break;
                case CombatAmountType.StaminaDecrease:
                    SpawnCombatText(followingTransform, uiCombatTextStaminaDecrease, amount);
                    break;
                case CombatAmountType.FoodDecrease:
                    SpawnCombatText(followingTransform, uiCombatTextFoodDecrease, amount);
                    break;
                case CombatAmountType.WaterDecrease:
                    SpawnCombatText(followingTransform, uiCombatTextWaterDecrease, amount);
                    break;
                case CombatAmountType.HpLeech:
                    SpawnCombatText(followingTransform, uiCombatTextHpLeech, amount);
                    break;
                case CombatAmountType.MpLeech:
                    SpawnCombatText(followingTransform, uiCombatTextMpLeech, amount);
                    break;
                case CombatAmountType.StaminaLeech:
                    SpawnCombatText(followingTransform, uiCombatTextStaminaLeech, amount);
                    break;
                case CombatAmountType.FallDamage:
                    SpawnCombatText(followingTransform, uiCombatTextFallDamage, amount);
                    break;
                case CombatAmountType.Immune:
                    SpawnCombatText(followingTransform, uiCombatTextImmune, amount);
                    break;
            }
        }

        public void SpawnCombatText(Transform followingTransform, UICombatText prefab, int amount)
        {
            if (prefab == null)
                return;

            UICombatText combatText;
            if (!instantiateCombatTextToWorldTransform && combatTextTransform)
            {
                combatText = Instantiate(prefab, combatTextTransform);
                combatText.transform.localScale = Vector3.one;
                combatText.gameObject.GetOrAddComponent<UIFollowWorldObject>().TargetObject = followingTransform;
            }
            else
            {
                combatText = Instantiate(prefab);
                combatText.transform.position = followingTransform.position;
            }
            combatText.Amount = amount;
        }

        public virtual bool IsBlockController()
        {
            if (UISceneGlobal.Singleton.uiMessageDialog.IsVisible() ||
                UISceneGlobal.Singleton.uiInputDialog.IsVisible() ||
                UISceneGlobal.Singleton.uiPasswordDialog.IsVisible())
                return true;

            if (UIBlockController.IsBlockController())
                return true;

            return false;
        }

        public virtual bool IsBlockActionController()
        {
            return UIBlockActionController.IsBlockController();
        }

        public virtual bool IsPointerOverUIObject()
        {
            return false;
        }

        // Abstract functions
        public abstract void SetTargetEntity(BaseGameEntity entity);
        public abstract void SetActivePlayerCharacter(BasePlayerCharacterEntity playerCharacter);
        public abstract void HideQuestRewardItemSelection();
        public abstract void HideNpcDialog();
        public abstract void ShowConstructBuildingDialog(BuildingEntity buildingEntity);
        public abstract void HideConstructBuildingDialog();
        public abstract void ShowCurrentBuildingDialog(BuildingEntity buildingEntity);
        public abstract void HideCurrentBuildingDialog();
        public abstract bool IsShopDialogVisible();
        public abstract bool IsRefineItemDialogVisible();
        public abstract bool IsDismantleItemDialogVisible();
        public abstract bool IsRepairItemDialogVisible();
        public abstract bool IsEnhanceSocketItemDialogVisible();
        public abstract bool IsStorageDialogVisible();
        public abstract bool IsItemsContainerDialogVisible();
        public abstract bool IsDealingDialogVisibleWithDealingState();
        public abstract bool IsStartVendingDialogVisible();
        public abstract void ShowRefineItemDialog(InventoryType inventoryType, int indexOfData, byte equipSlotIndex);
        public abstract void ShowDismantleItemDialog(InventoryType inventoryType, int indexOfData, byte equipSlotIndex);
        public abstract void ShowRepairItemDialog(InventoryType inventoryType, int indexOfData, byte equipSlotIndex);
        public abstract void ShowEnhanceSocketItemDialog(InventoryType inventoryType, int indexOfData, byte equipSlotIndex);
        public abstract void ShowStorageDialog(StorageType storageType, string storageOwnerId, uint objectId, int weightLimit, int slotLimit);
        public abstract void HideStorageDialog(StorageType storageType, string storageOwnerId);
        public abstract void ShowItemsContainerDialog(ItemsContainerEntity itemsContainerEntity);
        public abstract void HideItemsContainerDialog();
        public abstract void ShowWorkbenchDialog(WorkbenchEntity workbenchEntity);
        public abstract void ShowCraftingQueueItemsDialog(ICraftingQueueSource source);
        public abstract void OnControllerSetup(BasePlayerCharacterEntity playerCharacter);
        public abstract void OnControllerDesetup(BasePlayerCharacterEntity playerCharacter);
        public abstract void ShowVending(BasePlayerCharacterEntity playerCharacter);
        // Abstract properties
        public abstract ItemsContainerEntity ItemsContainerEntity { get; }
    }
}
