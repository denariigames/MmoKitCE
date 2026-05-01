using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(PlayerCharacterItemLockAndExpireComponent))]
    [RequireComponent(typeof(PlayerCharacterNpcActionComponent))]
    public abstract partial class BasePlayerCharacterEntity : BaseCharacterEntity, IPlayerCharacterData, IActivatableEntity
    {
        protected static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("BasePlayerCharacterEntity - Update");

        [Category("Character Settings")]
        [Tooltip("This is list which used as choice of character classes when create character")]
        [SerializeField]
        [FormerlySerializedAs("playerCharacters")]
        protected PlayerCharacter[] characterDatabases = new PlayerCharacter[0];
        public PlayerCharacter[] CharacterDatabases
        {
            get
            {
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData))
                    return metaData.CharacterDatabases;
                return characterDatabases;
            }
            set { characterDatabases = value; }
        }

        public PlayerCharacterItemLockAndExpireComponent ItemLockAndExpireComponent
        {
            get; private set;
        }

        public PlayerCharacterNpcActionComponent NpcActionComponent
        {
            get; private set;
        }

        public PlayerCharacterBuildingComponent BuildingComponent
        {
            get; private set;
        }

        public PlayerCharacterCraftingComponent CraftingComponent
        {
            get; private set;
        }

        public PlayerCharacterDealingComponent DealingComponent
        {
            get; private set;
        }

        public PlayerCharacterDuelingComponent DuelingComponent
        {
            get; private set;
        }

        public PlayerCharacterVendingComponent VendingComponent
        {
            get; private set;
        }

        public PlayerCharacterPkComponent PkComponent
        {
            get; private set;
        }

        public override CharacterRace Race
        {
            get
            {
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData))
                    return metaData.Race;
                return base.Race;
            }
            set { base.Race = value; }
        }

        public readonly List<GameObject> InstantiatedObjects = new List<GameObject>();

        public bool TryGetMetaData(out PlayerCharacterEntityMetaData metaData)
        {
            metaData = null;
            if (MetaDataId == 0 || !GameInstance.PlayerCharacterEntityMetaDataList.TryGetValue(MetaDataId, out metaData))
                return false;
            return true;
        }

        public int IndexOfCharacterDatabase(int dataId)
        {
            for (int i = 0; i < CharacterDatabases.Length; ++i)
            {
                if (CharacterDatabases[i] != null && CharacterDatabases[i].DataId == dataId)
                    return i;
            }
            return -1;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddCharacters(CharacterDatabases);
        }

        public override EntityInfo GetInfo()
        {
            return _info.SetEntityInfo(
                EntityTypes.Player,
                ObjectId,
                Id,
                SubChannelId,
                DataId,
                FactionId,
                PartyId,
                GuildId,
                IsInSafeArea,
                this,
                SummonerEntity);
        }

        protected override void EntityAwake()
        {
            base.EntityAwake();
            gameObject.tag = CurrentGameInstance.playerTag;
            gameObject.layer = CurrentGameInstance.playerLayer;
            //Create compression modes dictionary
            EntityCompressionModes = new NativeHashMap<uint, byte>(1024, Allocator.Persistent);
        }

        protected override void EntityOnSetOwnerClient(bool isOwnerClient)
        {
            base.EntityOnSetOwnerClient(isOwnerClient);
            gameObject.layer = isOwnerClient ? CurrentGameInstance.playingLayer : CurrentGameInstance.playerLayer;
            InstantiatePlayerCharacterObjects(isOwnerClient);
        }

        private async void InstantiatePlayerCharacterObjects(bool isOwnerClient)
        {
            InstantiatedObjects.DestroyAndNullify();
            InstantiatedObjects.Clear();
            // Setup relates elements
            if (isOwnerClient)
            {
                BasePlayerCharacterController controllerPrefab = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                if (CurrentGameInstance.DefaultControllerPrefab != null)
                {
                    controllerPrefab = CurrentGameInstance.DefaultControllerPrefab;
                }
#endif
                if (controllerPrefab != null)
                {
                    // Do nothing, just have it to make it able to compile properly (it have compile condition above)
                }
#if !DISABLE_ADDRESSABLES
                else if (CurrentGameInstance.AddressableDefaultControllerPrefab.IsDataValid())
                {
                    controllerPrefab = await CurrentGameInstance.AddressableDefaultControllerPrefab.GetOrLoadAssetAsync<BasePlayerCharacterController>();
                }
#endif
                else
                {
                    Logging.LogWarning(ToString(), "`Controller Prefab` is empty so it cannot be instantiated");
                    controllerPrefab = null;
                }
                if (controllerPrefab != null)
                {
                    BasePlayerCharacterController controller = Instantiate(controllerPrefab);
                    controller.PlayingCharacterEntity = this;
                    InstantiatedObjects.Add(controller.gameObject);
                }
#if !DISABLE_ADDRESSABLES
                // Instantiates owning objects
                await CurrentGameInstance.AddressableOwningCharacterObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.OwningCharacterObjects, EntityTransform, InstantiatedObjects);
#else
                foreach (var prefab in CurrentGameInstance.OwningCharacterObjects)
                {
                    if (prefab == null) continue;
                    InstantiatedObjects.Add(Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform));
                }
#endif
#if !DISABLE_ADDRESSABLES
                // Instantiates owning minimap objects
                await CurrentGameInstance.AddressableOwningCharacterMiniMapObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.OwningCharacterMiniMapObjects, EntityTransform, InstantiatedObjects);
#else
                foreach (var prefab in CurrentGameInstance.OwningCharacterMiniMapObjects)
                {
                    if (prefab == null) continue;
                    InstantiatedObjects.Add(Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform));
                }
#endif
                // Instantiates owning character UI
                InstantiateUI(await CurrentGameInstance.GetLoadedOwningCharacterUIPrefab());
            }
            else if (IsClient)
            {
#if !DISABLE_ADDRESSABLES
                // Instantiates non-owning objects
                await CurrentGameInstance.AddressableNonOwningCharacterObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.NonOwningCharacterObjects, EntityTransform, InstantiatedObjects);
#else
                foreach (var prefab in CurrentGameInstance.NonOwningCharacterObjects)
                {
                    if (prefab == null) continue;
                    InstantiatedObjects.Add(Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform));
                }
#endif
#if !DISABLE_ADDRESSABLES
                // Instantiates non-owning minimap objects
                await CurrentGameInstance.AddressableNonOwningCharacterMiniMapObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.NonOwningCharacterMiniMapObjects, EntityTransform, InstantiatedObjects);
#else
                foreach (var prefab in CurrentGameInstance.NonOwningCharacterMiniMapObjects)
                {
                    if (prefab == null) continue;
                    InstantiatedObjects.Add(Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform));
                }
#endif
                // Instantiates non-owning character UI
                InstantiateUI(await CurrentGameInstance.GetLoadedNonOwningCharacterUIPrefab());
            }
        }

        public override void InitialRequiredComponents()
        {
            CurrentGameInstance.EntitySetting.InitialPlayerCharacterEntityComponents(this);
            base.InitialRequiredComponents();
            ItemLockAndExpireComponent = gameObject.GetComponent<PlayerCharacterItemLockAndExpireComponent>();
            NpcActionComponent = gameObject.GetComponent<PlayerCharacterNpcActionComponent>();
            BuildingComponent = gameObject.GetComponent<PlayerCharacterBuildingComponent>();
            CraftingComponent = gameObject.GetComponent<PlayerCharacterCraftingComponent>();
            DealingComponent = gameObject.GetComponent<PlayerCharacterDealingComponent>();
            DuelingComponent = gameObject.GetComponent<PlayerCharacterDuelingComponent>();
            VendingComponent = gameObject.GetComponent<PlayerCharacterVendingComponent>();
            PkComponent = gameObject.GetComponent<PlayerCharacterPkComponent>();
        }

        protected override void EntityUpdate()
        {
            base.EntityUpdate();
            using (s_UpdateProfilerMarker.Auto())
            {
                if (this.IsDead())
                {
                    StopMove();
                    SetTargetEntity(null);
                    return;
                }
            }
        }

        public virtual float GetActivatableDistance()
        {
            return GameInstance.Singleton.conversationDistance;
        }

        public virtual bool ShouldClearTargetAfterActivated()
        {
            return false;
        }

        public virtual bool ShouldBeAttackTarget()
        {
            return !IsOwnerClient && !this.IsDeadOrHideFrom(GameInstance.PlayingCharacterEntity) && CanReceiveDamageFrom(GameInstance.PlayingCharacterEntity.GetInfo());
        }

        public virtual bool ShouldNotActivateAfterFollowed()
        {
            return true;
        }

        public virtual bool CanActivate()
        {
            return !IsOwnerClient;
        }

        public virtual void OnActivate()
        {
            BaseUISceneGameplay.Singleton.SetActivePlayerCharacter(this);
        }
    }
}
