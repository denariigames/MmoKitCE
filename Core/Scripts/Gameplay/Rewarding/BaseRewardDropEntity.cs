using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public abstract partial class BaseRewardDropEntity : BaseGameEntity, IPickupActivatableEntity
    {
        [System.Serializable]
        public struct AppearanceSetting : System.IComparable<AppearanceSetting>
        {
            public int amount;
            public GameObject[] activatingObjects;

            public int CompareTo(AppearanceSetting other)
            {
                return amount.CompareTo(other.amount);
            }

            public void Clean()
            {
                activatingObjects.Nullify();
            }
        }

        [Category("Relative GameObjects/Transforms")]
        public List<AppearanceSetting> appearanceSettings = new List<AppearanceSetting>();

        [Category(5, "Respawn Settings")]
        [Tooltip("Delay before the entity destroyed, you may set some delay to play destroyed animation by `onItemDropDestroy` event before it's going to be destroyed from the game.")]
        [SerializeField]
        protected float destroyDelay = 0f;
        [SerializeField]
        protected float destroyRespawnDelay = 5f;

        [Category(99, "Events")]
        [SerializeField]
        protected UnityEvent onSpawned = new UnityEvent();
        public UnityEvent OnSpawned { get { return onSpawned; } }

        [SerializeField]
        protected UnityEvent onPickedUp = new UnityEvent();
        public UnityEvent OnPickedUp { get { return onPickedUp; } }

        public float Multiplier { get; protected set; }

        public RewardGivenType GivenType { get; protected set; }

        public int GiverLevel { get; protected set; }

        public int SourceLevel { get; protected set; }

        public HashSet<string> Looters { get; protected set; } = new HashSet<string>();

        public GameSpawnArea<BaseRewardDropEntity> SpawnArea { get; protected set; }

        public BaseRewardDropEntity SpawnPrefab { get; protected set; }

#if !DISABLE_ADDRESSABLES
        public GameSpawnArea<BaseRewardDropEntity>.AddressablePrefab SpawnAddressablePrefab { get; protected set; }
#endif

        public int SpawnLevel { get; protected set; }

        public Vector3 SpawnPosition { get; protected set; }

        public abstract BaseItem RepresentItem { get; }

        public float DestroyDelay
        {
            get { return destroyDelay; }
            set { destroyDelay = value; }
        }

        public float DestroyRespawnDelay
        {
            get { return destroyRespawnDelay; }
            set { destroyRespawnDelay = value; }
        }

        public override string EntityTitle
        {
            get
            {
                return Amount.ToString("N0");
            }
        }

        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldInt amount = new SyncFieldInt();
        public int Amount
        {
            get { return amount.Value; }
            set { amount.Value = value; }
        }

        // Private variables
        protected bool _isPickedUp;
        protected float _dropTime;
        private List<GameObject> _allActivatingObjects = new List<GameObject>();

        protected override void EntityAwake()
        {
            base.EntityAwake();
            gameObject.tag = CurrentGameInstance.itemDropTag;
            gameObject.layer = CurrentGameInstance.itemDropLayer;
            if (appearanceSettings != null && appearanceSettings.Count > 0)
            {
                appearanceSettings.Sort();
                foreach (AppearanceSetting setting in appearanceSettings)
                {
                    if (setting.activatingObjects == null || setting.activatingObjects.Length <= 0)
                        continue;
                    foreach (GameObject activatingObject in setting.activatingObjects)
                    {
                        activatingObject.SetActive(false);
                        _allActivatingObjects.Add(activatingObject);
                    }
                }
            }
        }

        public virtual void Init()
        {
            _isPickedUp = false;
            _dropTime = Time.unscaledTime;
        }

        public override void OnSetup()
        {
            base.OnSetup();
            amount.onChange += OnAmountChange;
            if (IsServer && IsSceneObject)
            {
                // Init just once when started, if this entity is scene object
                Init();
            }
        }

        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            amount.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
        }

        public virtual void SetSpawnArea(GameSpawnArea<BaseRewardDropEntity> spawnArea, BaseRewardDropEntity spawnPrefab, int spawnLevel, Vector3 spawnPosition)
        {
            SpawnArea = spawnArea;
            SpawnPrefab = spawnPrefab;
#if !DISABLE_ADDRESSABLES
            SpawnAddressablePrefab = null;
#endif
            SpawnLevel = spawnLevel;
            SpawnPosition = spawnPosition;
        }

#if !DISABLE_ADDRESSABLES
        public virtual void SetSpawnArea(GameSpawnArea<BaseRewardDropEntity> spawnArea, GameSpawnArea<BaseRewardDropEntity>.AddressablePrefab spawnAddressablePrefab, int spawnLevel, Vector3 spawnPosition)
        {
            SpawnArea = spawnArea;
            SpawnPrefab = null;
            SpawnAddressablePrefab = spawnAddressablePrefab;
            SpawnLevel = spawnLevel;
            SpawnPosition = spawnPosition;
        }
#endif

        public void CallRpcOnSpawned()
        {
            RPC(RpcOnSpawned, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
        }

        [AllRpc]
        protected virtual void RpcOnSpawned()
        {
            if (onSpawned != null)
                onSpawned.Invoke();
        }

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            amount.onChange -= OnAmountChange;
        }

        public void CallRpcOnPickedUp()
        {
            RPC(RpcOnPickedUp, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
        }

        [AllRpc]
        protected virtual void RpcOnPickedUp()
        {
            if (onPickedUp != null)
                onPickedUp.Invoke();
        }

        protected virtual void OnAmountChange(bool isInitial, int oldAmount, int amount)
        {
            // Instantiate model at clients
            if (!IsClient)
                return;
            if (_allActivatingObjects != null && _allActivatingObjects.Count > 0)
            {
                foreach (GameObject obj in _allActivatingObjects)
                {
                    if (obj.activeSelf)
                        obj.SetActive(false);
                }
            }

            bool isFoundSetting = false;
            AppearanceSetting usingSetting = default;
            if (appearanceSettings != null && appearanceSettings.Count > 0)
            {
                foreach (AppearanceSetting setting in appearanceSettings)
                {
                    if (amount >= setting.amount)
                    {
                        isFoundSetting = true;
                        usingSetting = setting;
                    }
                    else
                        break;
                }
                if (isFoundSetting)
                {
                    if (usingSetting.activatingObjects != null && usingSetting.activatingObjects.Length > 0)
                    {
                        foreach (GameObject obj in usingSetting.activatingObjects)
                        {
                            obj.SetActive(true);
                        }
                    }
                }
            }
        }

        public bool IsAbleToLoot(BaseCharacterEntity characterEntity)
        {
            if (!string.Equals(characterEntity.SubChannelId, SubChannelId))
                return false;
            if ((Looters.Count == 0 || Looters.Contains(characterEntity.Id) ||
                Time.unscaledTime - _dropTime > CurrentGameInstance.itemLootLockDuration) && !_isPickedUp)
                return true;
            return false;
        }

        public void PickedUp()
        {
            if (!IsServer)
                return;
            if (_isPickedUp)
                return;
            // Mark as picked up
            _isPickedUp = true;
            // Tell clients that the entity is picked up
            CallRpcOnPickedUp();
            // Destroy this entity
            NetworkDestroy(DestroyDelay);
            // Respawning later
            if (SpawnArea != null)
            {
                SpawnArea.Spawn(SpawnPrefab
#if !DISABLE_ADDRESSABLES
                    , SpawnAddressablePrefab
#endif
                    , SpawnLevel, DestroyDelay + DestroyRespawnDelay, DestroyRespawnDelay);
            }
            else if (Identity.IsSceneObject)
            {
                RespawnRoutine(DestroyDelay + DestroyRespawnDelay).Forget();
            }
        }

        /// <summary>
        /// This function will be called if this object is placed in scene networked object
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        protected async UniTaskVoid RespawnRoutine(float delay)
        {
            await UniTask.Delay(Mathf.CeilToInt(delay * 1000));
            Looters.Clear();
            Init();
            Manager.Assets.NetworkSpawnScene(
                Identity.ObjectId,
                Identity.HashSceneObjectId,
                EntityTransform.position,
                CurrentGameInstance.DimensionType == DimensionType.Dimension3D ? Quaternion.Euler(Vector3.up * Random.Range(0, 360)) : Quaternion.identity);
        }

        public static T Drop<T>(T prefab, BaseGameEntity dropper, float multiplier, RewardGivenType rewardGivenType, int giverLevel, int sourceLevel, int amount, IEnumerable<string> looters, float appearDuration)
            where T : BaseRewardDropEntity
        {
            Vector3 dropPosition = dropper.EntityTransform.position;
            Quaternion dropRotation = Quaternion.identity;
            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension3D:
                    // Random position around dropper with its height
                    dropPosition = DropEntityUtils.GetDroppedPosition3D(dropPosition, GameInstance.Singleton.dropDistance);
                    // Random rotation
                    dropRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
                    break;
                case DimensionType.Dimension2D:
                    // Random position around dropper
                    dropPosition = DropEntityUtils.GetDroppedPosition2D(dropPosition, GameInstance.Singleton.dropDistance);
                    break;
            }
            T entity = Drop(prefab, dropPosition, dropRotation, multiplier, rewardGivenType, giverLevel, sourceLevel, amount, looters, appearDuration);
            if (entity != null)
            {
                // Set sub channel ID follow dropper, so it won't 
                entity.Identity.SubChannelId = dropper.SubChannelId;
            }
            return entity;
        }

        public static T Drop<T>(T prefab, Vector3 dropPosition, Quaternion dropRotation, float multiplier, RewardGivenType givenType, int giverLevel, int sourceLevel, int amount, IEnumerable<string> looters, float appearDuration)
            where T : BaseRewardDropEntity
        {
            if (prefab == null)
                return null;

            LiteNetLibIdentity spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                prefab.Identity.HashAssetId,
                dropPosition, dropRotation);
            T entity = spawnObj.GetComponent<T>();
            entity.Multiplier = multiplier;
            entity.GivenType = givenType;
            entity.GiverLevel = giverLevel;
            entity.SourceLevel = sourceLevel;
            entity.Amount = amount;
            entity.Looters = new HashSet<string>(looters);
            entity.Init();
            BaseGameNetworkManager.Singleton.Assets.NetworkSpawn(spawnObj);
            entity.NetworkDestroy(appearDuration);
            return entity;
        }

        public override bool SetAsTargetInOneClick()
        {
            return true;
        }

        public virtual float GetActivatableDistance()
        {
            return GameInstance.Singleton.pickUpItemDistance;
        }

        public virtual bool ShouldClearTargetAfterActivated()
        {
            return true;
        }

        public virtual bool CanPickupActivate()
        {
            if (IsServer && GameInstance.PlayingCharacterEntity != null && Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                return false;
            return true;
        }

        public virtual void OnPickupActivate()
        {
            GameInstance.PlayingCharacterEntity.CallCmdPickup(ObjectId);
        }

        public virtual bool ProceedPickingUpAtServer(BaseCharacterEntity characterEntity, out UITextKeys message)
        {
            if (!IsAbleToLoot(characterEntity))
            {
                message = UITextKeys.UI_ERROR_NOT_ABLE_TO_LOOT;
                return false;
            }
            if (!ProceedPickingUpAtServer_Implementation(characterEntity, out message))
                return false;
            PickedUp();
            return true;
        }

        protected abstract bool ProceedPickingUpAtServer_Implementation(BaseCharacterEntity characterEntity, out UITextKeys message);
    }
}
