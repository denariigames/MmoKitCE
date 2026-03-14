using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using LiteNetLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public partial class ItemDropEntity : BaseGameEntity, IPickupActivatableEntity
    {
        [Category("Relative GameObjects/Transforms")]
        [Tooltip("Item's `dropModel` will be instantiated to this transform for items which drops from characters")]
        [SerializeField]
        protected Transform modelContainer;

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

        [FormerlySerializedAs("onItemDropDestroy")]
        [SerializeField]
        protected UnityEvent onPickedUp = new UnityEvent();
        public UnityEvent OnPickedUp { get { return onPickedUp; } }

        [Category(6, "Drop Settings")]
        public ItemDropManager itemDropManager = new ItemDropManager();
        public ItemDropManager ItemDropManager { get { return itemDropManager; } }

        #region Being deprecated
        [HideInInspector]
        [SerializeField]
        [Tooltip("Max kind of items that will be dropped in ground")]
        protected byte maxDropItems = 5;

        [HideInInspector]
        [SerializeField]
        [ArrayElementTitle("item")]
        protected ItemDrop[] randomItems;

        [HideInInspector]
        [SerializeField]
        protected ItemDropTable itemDropTable;
        #endregion

        public bool PutOnPlaceholder { get; protected set; }

        public RewardGivenType GivenType { get; protected set; }

        public List<CharacterItem> DropItems { get; protected set; } = new List<CharacterItem>();

        public HashSet<string> Looters { get; protected set; } = new HashSet<string>();

        public GameSpawnArea<ItemDropEntity> SpawnArea { get; protected set; }

        public ItemDropEntity SpawnPrefab { get; protected set; }

#if !DISABLE_ADDRESSABLES
        public GameSpawnArea<ItemDropEntity>.AddressablePrefab SpawnAddressablePrefab { get; protected set; }
#endif

        public int SpawnLevel { get; protected set; }

        public Vector3 SpawnPosition { get; protected set; }

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

        private GameObject _dropModel;

        public override string EntityTitle
        {
            get
            {
                if (ItemDropData.putOnPlaceholder && GameInstance.Items.TryGetValue(ItemDropData.characterItem.dataId, out BaseItem item))
                    return item.Title;
                return base.EntityTitle;
            }
        }

        private bool _isModelContainerValidated = false;
        public Transform ModelContainer
        {
            get
            {
                if (!_isModelContainerValidated)
                {
                    if (modelContainer == null || modelContainer == transform)
                    {
                        modelContainer = new GameObject("_ModelContainer").transform;
                        modelContainer.transform.SetParent(transform);
                        modelContainer.transform.localPosition = Vector3.zero;
                        modelContainer.transform.localRotation = Quaternion.identity;
                        modelContainer.transform.localScale = Vector3.one;
                    }
                    _isModelContainerValidated = true;
                }
                return modelContainer;
            }
        }

        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldItemDropData itemDropData = new SyncFieldItemDropData();
        public ItemDropData ItemDropData
        {
            get { return itemDropData.Value; }
            set { itemDropData.Value = value; }
        }

        // Private variables
        protected bool _isPickedUp;
        protected float _dropTime;

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            ItemDropManager.PrepareRelatesData();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            bool hasChanges = false;
            if (MigrateItemDropData())
                hasChanges = true;
            if (hasChanges)
                EditorUtility.SetDirty(this);
        }
#endif

        private bool MigrateItemDropData()
        {
            bool hasChanges = false;
            if (randomItems != null && randomItems.Length > 0)
            {
                hasChanges = true;
                List<ItemDrop> list = new List<ItemDrop>(itemDropManager.randomItems);
                list.AddRange(randomItems);
                itemDropManager.randomItems = list.ToArray();
                randomItems = null;
            }
            if (itemDropTable != null)
            {
                hasChanges = true;
                List<ItemDropTable> list = new List<ItemDropTable>(itemDropManager.itemDropTables)
                {
                    itemDropTable
                };
                itemDropManager.itemDropTables = list.ToArray();
                itemDropTable = null;
            }
            if (maxDropItems > 0)
            {
                hasChanges = true;
                itemDropManager.maxDropItems = maxDropItems;
                maxDropItems = 0;
            }
            return hasChanges;
        }

        protected override void EntityAwake()
        {
            base.EntityAwake();
            gameObject.tag = CurrentGameInstance.itemDropTag;
            gameObject.layer = CurrentGameInstance.itemDropLayer;
            ModelContainer.gameObject.SetActive(false);
        }

        protected override void EntityOnDisable()
        {
            base.EntityOnDisable();
            ModelContainer.gameObject.SetActive(false);
        }

        public virtual void Init()
        {
            _isPickedUp = false;
            _dropTime = Time.unscaledTime;
            if (!PutOnPlaceholder)
            {
                DropItems.Clear();
                ItemDropManager.RandomItems((item, level, amount) =>
                {
                    DropItems.Add(CharacterItem.Create(item.DataId, level, amount));
                });
            }
            if (DropItems.Count == 0)
            {
                // No drop items data, it may not setup properly
                return;
            }
            ItemDropData = new ItemDropData()
            {
                putOnPlaceholder = PutOnPlaceholder,
                characterItem = DropItems[0],
            };
        }

        public override void OnSetup()
        {
            base.OnSetup();
            itemDropData.onChange += OnItemDropDataChange;
            if (IsServer && IsSceneObject)
            {
                // Init just once when started, if this entity is scene object
                Init();
            }
        }

        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            itemDropData.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
        }

        public virtual void SetSpawnArea(GameSpawnArea<ItemDropEntity> spawnArea, ItemDropEntity spawnPrefab, int spawnLevel, Vector3 spawnPosition)
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
        public virtual void SetSpawnArea(GameSpawnArea<ItemDropEntity> spawnArea, GameSpawnArea<ItemDropEntity>.AddressablePrefab spawnAddressablePrefab, int spawnLevel, Vector3 spawnPosition)
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

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            itemDropData.onChange -= OnItemDropDataChange;
        }

        protected virtual async void OnItemDropDataChange(bool isInitial, ItemDropData oldItemDropData, ItemDropData itemDropData)
        {
#if !UNITY_SERVER
            // Instantiate model at clients
            if (!IsClient)
                return;
            // Activate container to show item drop model
            ModelContainer.gameObject.SetActive(true);
            if (_dropModel != null)
                Destroy(_dropModel);
            if (itemDropData.putOnPlaceholder && GameInstance.Items.TryGetValue(itemDropData.characterItem.dataId, out BaseItem item))
            {
                GameObject dropModel = await item.GetDropModel();
                if (dropModel == null)
                    return;
                _dropModel = Instantiate(dropModel, ModelContainer);
                _dropModel.gameObject.SetLayerRecursively(CurrentGameInstance.itemDropLayer, true);
                _dropModel.gameObject.SetActive(true);
                _dropModel.RemoveComponentsInChildren<Collider>(false);
                _dropModel.transform.localPosition = Vector3.zero;
                // Hide from host client
                if (IsServer && GameInstance.PlayingCharacterEntity != null && Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                {
                    Identity.OnServerSubscribingRemoved();
                }
            }
#endif
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
            CallRpcOnSpawned();
        }

        public static UniTask<ItemDropEntity> Drop(BaseGameEntity dropper, RewardGivenType givenType, CharacterItem dropData, IEnumerable<string> looters)
        {
            return Drop(dropper, givenType, dropData, looters, GameInstance.Singleton.itemAppearDuration);
        }

        public static async UniTask<ItemDropEntity> Drop(BaseGameEntity dropper, RewardGivenType givenType, CharacterItem dropData, IEnumerable<string> looters, float appearDuration)
        {
            ItemDropEntity entity = null;
            ItemDropEntity loadedPrefab = await GameInstance.Singleton.GetLoadedItemDropEntityPrefab();
            if (loadedPrefab != null)
            {
                entity = Drop(loadedPrefab, dropper, givenType, dropData, looters, appearDuration);
            }
            return entity;
        }

        public static ItemDropEntity Drop(ItemDropEntity prefab, BaseGameEntity dropper, RewardGivenType givenType, CharacterItem dropData, IEnumerable<string> looters, float appearDuration)
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
            ItemDropEntity entity = Drop(prefab, dropPosition, dropRotation, givenType, dropData, looters, appearDuration);
            if (entity != null)
            {
                // Set sub channel ID follow dropper, so it won't 
                entity.Identity.SubChannelId = dropper.SubChannelId;
            }
            return entity;
        }

        public static ItemDropEntity Drop(ItemDropEntity prefab, Vector3 dropPosition, Quaternion dropRotation, RewardGivenType givenType, CharacterItem dropItem, IEnumerable<string> looters, float appearDuration)
        {
            if (prefab == null)
                return null;

            LiteNetLibIdentity spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                prefab.Identity.HashAssetId,
                dropPosition, dropRotation);
            ItemDropEntity entity = spawnObj.GetComponent<ItemDropEntity>();
            entity.GivenType = givenType;
            entity.PutOnPlaceholder = true;
            entity.DropItems = new List<CharacterItem> { dropItem };
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
            if (CurrentGameInstance.itemLootRandomPartyMember && Time.unscaledTime - _dropTime < CurrentGameInstance.itemLootLockDuration)
            {
                if (GameInstance.ServerUserHandlers.TryGetPlayerCharacterById(Looters.ToArray()[Random.Range(0, Looters.Count)], out IPlayerCharacterData randomCharacter))
                {
                    characterEntity = randomCharacter as BaseCharacterEntity;
                }
            }
            if (characterEntity.IncreasingItemsWillOverwhelming(DropItems))
            {
                message = UITextKeys.UI_ERROR_WILL_OVERWHELMING;
                return false;
            }
            characterEntity.IncreaseItems(DropItems, characterItem => characterEntity.OnRewardItem(GivenType, characterItem));
            characterEntity.FillEmptySlots();
            PickedUp();
            message = UITextKeys.NONE;
            return true;
        }
    }
}
