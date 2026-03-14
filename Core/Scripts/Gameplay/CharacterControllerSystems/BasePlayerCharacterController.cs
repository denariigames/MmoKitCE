using Insthync.AddressableAssetTools;
using Insthync.DevExtension;
using Insthync.ManagedUpdating;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract partial class BasePlayerCharacterController : MonoBehaviour, IManagedUpdate
    {
        public struct UsingSkillData
        {
            public AimPosition aimPosition;
            public BaseSkill skill;
            public int level;
            public int itemIndex;
            public UsingSkillData(AimPosition aimPosition, BaseSkill skill, int level, int itemIndex)
            {
                this.aimPosition = aimPosition;
                this.skill = skill;
                this.level = level;
                this.itemIndex = itemIndex;
            }

            public UsingSkillData(AimPosition aimPosition, BaseSkill skill, int level)
            {
                this.aimPosition = aimPosition;
                this.skill = skill;
                this.level = level;
                this.itemIndex = -1;
            }
        }

        public static BasePlayerCharacterController Singleton { get; protected set; }
        public static BasePlayerCharacterController LastPrefab { get; set; }
        /// <summary>
        /// Controlled character, can use `GameInstance.PlayingCharacter` or `GameInstance.PlayingCharacterEntity` instead.
        /// </summary>
        public System.Action<BasePlayerCharacterController> onSetup;
        public System.Action<BasePlayerCharacterController> onDesetup;
        public GameInstance CurrentGameInstance { get { return GameInstance.Singleton; } }

        public BasePlayerCharacterEntity PlayingCharacterEntity
        {
            get { return GameInstance.PlayingCharacterEntity; }
            set
            {
                if (value.IsOwnerClient)
                {
                    Desetup(GameInstance.PlayingCharacterEntity);
                    GameInstance.PlayingCharacter = value;
                    GameInstance.OpenedStorages.Clear();
                    Setup(GameInstance.PlayingCharacterEntity);
                }
            }
        }

        public Transform CameraTargetTransform
        {
            get { return PlayingCharacterEntity.CameraTargetTransform; }
        }

        public Transform EntityTransform
        {
            get { return PlayingCharacterEntity.EntityTransform; }
        }

        public Transform MovementTransform
        {
            get { return PlayingCharacterEntity.MovementTransform; }
        }

        public float StoppingDistance
        {
            get { return PlayingCharacterEntity.StoppingDistance; }
        }

        public BaseUISceneGameplay UISceneGameplay { get; protected set; }
        public ITargetableEntity SelectedEntity { get; protected set; }
        public BaseGameEntity SelectedGameEntity
        {
            get
            {
                if (SelectedEntity is IGameEntity castedEntity && !castedEntity.IsNull())
                    return castedEntity.Entity;
                return null;
            }
        }
        public uint SelectedGameEntityObjectId
        {
            get { return SelectedGameEntity != null ? SelectedGameEntity.ObjectId : 0; }
        }
        public ITargetableEntity TargetEntity { get; protected set; }
        public BaseGameEntity TargetGameEntity
        {
            get
            {
                if (TargetEntity is IGameEntity castedEntity)
                    return castedEntity.Entity;
                return null;
            }
        }
        public uint TargetGameEntityObjectId
        {
            get { return TargetGameEntity != null ? TargetGameEntity.ObjectId : 0; }
        }
        public BuildingEntity ConstructingBuildingEntity { get; protected set; }
        public BuildingEntity TargetBuildingEntity
        {
            get { return TargetGameEntity as BuildingEntity; }
        }
        public IBuildAimController BuildAimController { get; protected set; }
        public IAreaSkillAimController AreaSkillAimController { get; protected set; }

        protected int _buildingItemIndex = -1;
        protected UsingSkillData _queueUsingSkill;

        protected virtual void Awake()
        {
            Singleton = this;
            this.InvokeInstanceDevExtMethods("Awake");
        }

        protected virtual void OnEnable()
        {
            UpdateManager.Register(DefaultExecutionOrders.PLAYER_CHARACTER_CONTROLLER, this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.Unregister(DefaultExecutionOrders.PLAYER_CHARACTER_CONTROLLER, this);
        }

        public virtual void ManagedUpdate()
        {
        }

        protected virtual void Setup(BasePlayerCharacterEntity characterEntity)
        {
            BaseUISceneGameplay tempPrefab = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            tempPrefab = CurrentGameInstance.UISceneGameplayPrefab;
#endif
#if !DISABLE_ADDRESSABLES
            AssetReferenceBaseUISceneGameplay tempAddressablePrefab = CurrentGameInstance.AddressableUISceneGameplayPrefab;
#endif
            BaseUISceneGameplay loadedPrefab;
#if !DISABLE_ADDRESSABLES
            loadedPrefab = tempAddressablePrefab.GetOrLoadAssetOrUsePrefab(tempPrefab);
#else
            loadedPrefab = tempPrefab;
#endif
            if (loadedPrefab != null)
                UISceneGameplay = Instantiate(loadedPrefab);
            if (UISceneGameplay != null)
                UISceneGameplay.OnControllerSetup(characterEntity);
            if (onSetup != null)
                onSetup.Invoke(this);
        }

        protected virtual void Desetup(BasePlayerCharacterEntity characterEntity)
        {
            if (UISceneGameplay != null)
            {
                UISceneGameplay.OnControllerDesetup(characterEntity);
                Destroy(UISceneGameplay.gameObject);
            }
            if (onDesetup != null)
                onDesetup.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            Desetup(PlayingCharacterEntity);
            if (Singleton == this)
                Singleton = null;
            this.InvokeInstanceDevExtMethods("OnDestroy");
        }

        public virtual void ConfirmBuild()
        {
            if (ConstructingBuildingEntity == null)
                return;
            if (ConstructingBuildingEntity.CanBuild())
            {
                uint parentObjectId = 0;
                if (ConstructingBuildingEntity.BuildingArea != null)
                    parentObjectId = ConstructingBuildingEntity.BuildingArea.GetEntityObjectId();
                PlayingCharacterEntity.BuildingComponent.CallCmdConstructBuilding(_buildingItemIndex, ConstructingBuildingEntity.EntityTransform.position, ConstructingBuildingEntity.EntityTransform.eulerAngles, parentObjectId);
            }
            DestroyConstructingBuilding();
        }

        public virtual void CancelBuild()
        {
            DestroyConstructingBuilding();
        }

        public virtual BuildingEntity InstantiateConstructingBuilding(BuildingEntity prefab)
        {
            ConstructingBuildingEntity = Instantiate(prefab);
            ConstructingBuildingEntity.name += "(Constructing)";
            ConstructingBuildingEntity.SetupAsBuildMode(PlayingCharacterEntity);
            ConstructingBuildingEntity.EntityTransform.parent = null;
            return ConstructingBuildingEntity;
        }

        public virtual void DestroyConstructingBuilding()
        {
            if (ConstructingBuildingEntity == null)
                return;
            Destroy(ConstructingBuildingEntity.gameObject);
            ConstructingBuildingEntity = null;
        }

        public virtual void DeselectBuilding()
        {
            TargetEntity = null;
        }

        public virtual void RepairBuilding(BuildingEntity targetBuildingEntity)
        {
            if (targetBuildingEntity == null)
                return;
            PlayingCharacterEntity.BuildingComponent.CallCmdRepairBuilding(targetBuildingEntity.ObjectId);
            DeselectBuilding();
        }

        public virtual void DestroyBuilding(BuildingEntity targetBuildingEntity)
        {
            if (targetBuildingEntity == null)
                return;
            PlayingCharacterEntity.BuildingComponent.CallCmdDestroyBuilding(targetBuildingEntity.ObjectId);
            DeselectBuilding();
        }

        public virtual void SetBuildingPassword(BuildingEntity targetBuildingEntity)
        {
            if (targetBuildingEntity == null)
                return;
            uint objectId = targetBuildingEntity.ObjectId;
            UISceneGlobal.Singleton.ShowPasswordDialog(
                LanguageManager.GetText(UITextKeys.UI_SET_BUILDING_PASSWORD.ToString()),
                LanguageManager.GetText(UITextKeys.UI_SET_BUILDING_PASSWORD_DESCRIPTION.ToString()),
                (password) =>
                {
                    PlayingCharacterEntity.BuildingComponent.CallCmdSetBuildingPassword(objectId, password);
                }, string.Empty, targetBuildingEntity.PasswordContentType, targetBuildingEntity.PasswordLength,
                LanguageManager.GetText(UITextKeys.UI_SET_BUILDING_PASSWORD.ToString()));
            DeselectBuilding();
        }

        public virtual void LockBuilding(BuildingEntity targetBuildingEntity)
        {
            if (targetBuildingEntity == null)
                return;
            PlayingCharacterEntity.BuildingComponent.CallCmdLockBuilding(targetBuildingEntity.ObjectId);
            DeselectBuilding();
        }

        public virtual void UnlockBuilding(BuildingEntity targetBuildingEntity)
        {
            if (targetBuildingEntity == null)
                return;
            PlayingCharacterEntity.BuildingComponent.CallCmdUnlockBuilding(targetBuildingEntity.ObjectId);
            DeselectBuilding();
        }

        public virtual void ActivateBuilding(BuildingEntity targetBuildingEntity)
        {
            if (targetBuildingEntity == null)
                return;
            targetBuildingEntity.OnActivate();
            DeselectBuilding();
        }

        protected void ShowConstructBuildingDialog()
        {
            if (!ConstructingBuildingEntity.CanBuild())
            {
                DestroyConstructingBuilding();
                UISceneGameplay.HideConstructBuildingDialog();
                return;
            }
            UISceneGameplay.ShowConstructBuildingDialog(ConstructingBuildingEntity);
        }

        protected void HideConstructBuildingDialog()
        {
            UISceneGameplay.HideConstructBuildingDialog();
        }

        protected void ShowCurrentBuildingDialog()
        {
            UISceneGameplay.ShowCurrentBuildingDialog(TargetBuildingEntity);
        }

        protected void HideCurrentBuildingDialog()
        {
            UISceneGameplay.HideCurrentBuildingDialog();
        }

        protected void ShowItemsContainerDialog(ItemsContainerEntity itemsContainerEntity)
        {
            UISceneGameplay.ShowItemsContainerDialog(itemsContainerEntity);
        }

        protected void HideItemsContainerDialog()
        {
            UISceneGameplay.HideItemsContainerDialog();
        }

        protected void HideNpcDialog()
        {
            UISceneGameplay.HideNpcDialog();
        }

        public void SetQueueUsingSkill(AimPosition aimPosition, BaseSkill skill, int level)
        {
            _queueUsingSkill = new UsingSkillData(aimPosition, skill, level);
        }

        public void SetQueueUsingSkill(AimPosition aimPosition, BaseSkill skill, int level, int itemIndex)
        {
            _queueUsingSkill = new UsingSkillData(aimPosition, skill, level, itemIndex);
        }

        public void ClearQueueUsingSkill()
        {
            _queueUsingSkill = new UsingSkillData(default, null, 0, -1);
        }

        public abstract bool UseHotkey(HotkeyType type, string relateId, AimPosition aimPosition);

        public bool CanActivate(IActivatableEntity entity)
        {
            return !entity.IsNull() && Vector3.Distance(EntityTransform.position, entity.EntityTransform.position) <= entity.GetActivatableDistance() && entity.CanActivate();
        }

        public bool CanHoldActivate(IHoldActivatableEntity entity)
        {
            return !entity.IsNull() && Vector3.Distance(EntityTransform.position, entity.EntityTransform.position) <= entity.GetActivatableDistance() && entity.CanHoldActivate();
        }

        public bool CanPickupActivate(IPickupActivatableEntity entity)
        {
            return !entity.IsNull() && Vector3.Distance(EntityTransform.position, entity.EntityTransform.position) <= entity.GetActivatableDistance() && entity.CanPickupActivate();
        }


        public virtual bool ShouldShowActivateButtons()
        {
            return CanActivate(SelectedGameEntity as IActivatableEntity);
        }

        public virtual bool ShouldShowHoldActivateButtons()
        {
            return CanHoldActivate(SelectedGameEntity as IHoldActivatableEntity);
        }

        public virtual bool ShouldShowPickUpButtons()
        {
            return CanPickupActivate(SelectedGameEntity as IPickupActivatableEntity);
        }
    }
}
