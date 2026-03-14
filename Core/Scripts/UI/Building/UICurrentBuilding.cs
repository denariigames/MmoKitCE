using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UICurrentBuilding : UIBase
    {
        public BasePlayerCharacterController Controller { get { return BasePlayerCharacterController.Singleton; } }
        public TextWrapper textTitle;
        [Tooltip("These game objects will be activate if target building entity's `isLocked` = `TRUE`")]
        public GameObject[] lockedObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building entity's `isLocked` = `FALSE`")]
        public GameObject[] unlockedObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building entity's `locakable` = `TRUE`")]
        public GameObject[] lockableObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building entity's `locakable` = `FALSE`")]
        public GameObject[] notLockableObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building can be repaired by menu")]
        public GameObject[] repairableObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building cannot be repaired by menu")]
        public GameObject[] notRepairableObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building can be destroyed")]
        public GameObject[] destroyableObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building cannot be destroyed")]
        public GameObject[] notDestroyableObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building's password can be defined")]
        public GameObject[] passwordDefinableObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building's password cannot be defined")]
        public GameObject[] notPasswordDefinableObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building can be activated")]
        public GameObject[] activatableObjects = new GameObject[0];
        [Tooltip("These game objects will be activate if target building cannot be activated")]
        public GameObject[] notActivatableObjects = new GameObject[0];

        public Button buttonLock;
        public Button buttonUnlock;
        public Button buttonRepair;
        public Button buttonDestroy;
        public Button buttonSetPassword;
        public Button buttonActivate;

        private BuildingEntity _buildingEntity;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            textTitle = null;
            lockedObjects.Nullify();
            unlockedObjects.Nullify();
            lockableObjects.Nullify();
            notLockableObjects.Nullify();
            repairableObjects.Nullify();
            notRepairableObjects.Nullify();
            destroyableObjects.Nullify();
            notDestroyableObjects.Nullify();
            passwordDefinableObjects.Nullify();
            notPasswordDefinableObjects.Nullify();
            activatableObjects.Nullify();
            notActivatableObjects.Nullify();
            buttonLock = null;
            buttonUnlock = null;
            buttonRepair = null;
            buttonDestroy = null;
            buttonSetPassword = null;
            buttonActivate = null;
            _buildingEntity = null;
        }

        public void Show(BuildingEntity buildingEntity)
        {
            if (buildingEntity == null)
            {
                // Don't show
                return;
            }
            this._buildingEntity = buildingEntity;
            base.Show();
        }

        protected virtual void OnEnable()
        {
            if (textTitle != null)
                textTitle.text = _buildingEntity.Title;

            bool isCreator = _buildingEntity.IsCreator(GameInstance.PlayingCharacterEntity);
            bool lockable = !_buildingEntity.IsLocked && _buildingEntity.Lockable && isCreator;
            bool unlockable = _buildingEntity.IsLocked && _buildingEntity.Lockable && isCreator;
            bool repairable = _buildingEntity.CanRepairByMenu() && _buildingEntity.TryGetRepairAmount(GameInstance.PlayingCharacterEntity, out _, out _);
            bool destroyable = isCreator;
            bool passwordDefinable = _buildingEntity.IsLocked && isCreator;
            bool activatable = _buildingEntity.CanActivate();

            foreach (GameObject obj in lockedObjects)
            {
                obj.SetActive(_buildingEntity.IsLocked);
            }
            foreach (GameObject obj in unlockedObjects)
            {
                obj.SetActive(!_buildingEntity.IsLocked);
            }

            foreach (GameObject obj in lockableObjects)
            {
                obj.SetActive(lockable);
            }
            foreach (GameObject obj in notLockableObjects)
            {
                obj.SetActive(unlockable);
            }

            foreach (GameObject obj in repairableObjects)
            {
                obj.SetActive(repairable);
            }
            foreach (GameObject obj in notRepairableObjects)
            {
                obj.SetActive(!repairable);
            }

            foreach (GameObject obj in destroyableObjects)
            {
                obj.SetActive(destroyable);
            }
            foreach (GameObject obj in notDestroyableObjects)
            {
                obj.SetActive(!destroyable);
            }

            foreach (GameObject obj in passwordDefinableObjects)
            {
                obj.SetActive(passwordDefinable);
            }
            foreach (GameObject obj in notPasswordDefinableObjects)
            {
                obj.SetActive(!passwordDefinable);
            }

            foreach (GameObject obj in activatableObjects)
            {
                obj.SetActive(activatable);
            }
            foreach (GameObject obj in notActivatableObjects)
            {
                obj.SetActive(!activatable);
            }

            if (buttonLock != null)
                buttonLock.interactable = lockable;
            if (buttonUnlock != null)
                buttonUnlock.interactable = unlockable;
            if (buttonRepair != null)
                buttonRepair.interactable = repairable;
            if (buttonDestroy != null)
                buttonDestroy.interactable = destroyable;
            if (buttonSetPassword != null)
                buttonSetPassword.interactable = passwordDefinable;
            if (buttonActivate != null)
                buttonActivate.interactable = activatable;
        }

        private void Update()
        {
            if (IsVisible() && (_buildingEntity == null || _buildingEntity.IsDead()))
                Hide();
        }

        public void OnClickDeselect()
        {
            Controller.DeselectBuilding();
            Hide();
        }

        public void OnClickRepair()
        {
            Controller.RepairBuilding(_buildingEntity);
            Hide();
        }

        public void OnClickDestroy()
        {
            Controller.DestroyBuilding(_buildingEntity);
            Hide();
        }

        public void OnClickSetPassword()
        {
            Controller.SetBuildingPassword(_buildingEntity);
            Hide();
        }

        public void OnClickLock()
        {
            Controller.LockBuilding(_buildingEntity);
            Hide();
        }

        public void OnClickUnlock()
        {
            Controller.UnlockBuilding(_buildingEntity);
            Hide();
        }

        public void OnClickActivate()
        {
            Controller.ActivateBuilding(_buildingEntity);
            Hide();
        }
    }
}
